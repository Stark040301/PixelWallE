using System;
using System.Collections.Generic;
using PixelWallE.Core.Evaluator.Runtime;
using PixelWallE.Core.Evaluator.Runtime.Functions;
using PixelWallE.Core.Lexer;
using PixelWallE.Core.Parser.AST;
using PixelWallE.Core.Parser.AST.Expressions;
using PixelWallE.Core.Parser.AST.Statements;
using PixelWallE.WallE;

namespace PixelWallE.Core.Evaluator;

public class Interpreter: IExpressionVisitor<Object>, IStatementVisitor
{
    private readonly Dictionary<string, INativeFunction> nativeFunctions;
    private readonly WallEContext wallEContext;
    private Environment environment = new Environment();
    private LabelTable labelTable = new LabelTable();
    private int current;

    public Interpreter(WallEContext context)
    {
        wallEContext = context;
        this.nativeFunctions = new()
        {
            { "GetActualX", new GetActualXFunction(context) },
            { "GetActualY", new GetActualYFunction(context) },
            { "GetCanvasSize", new GetCanvasSizeFunction(context) },
            { "GetColorCount", new GetColorCountFunction(context) },
            { "IsBrushColor", new IsBrushColorFunction(context) },
            { "IsBrushSize", new IsBrushSizeFunction(context) },
            { "IsCanvasColor", new IsCanvasColorFunction(context) }
        };
    }
    public void Interpret(List<Statement> statements) 
    {
        try 
        {
            labelTable = new LabelTable();
            for (int i = 0; i < statements.Count; i++)
            {
                if (statements[i] is LabelStatement labelStatement)
                {
                    labelTable.Define(labelStatement.Name, i);
                }
            }

            current = 0;
            while (current < statements.Count)
            {
                int previous = current;
                Execute(statements[current]);
                if (current == previous) current++;
            }

        } 
        catch (RuntimeError error) 
        {
            MainWallE.RuntimeError(error);
        }
    }
    private Object Evaluate(Expression expression)
    {
        return expression.Accept(this);
    }
    private void Execute(Statement stmt) 
    {
        stmt.Accept(this);
    }

    public void Visit(ExprStatement exprStatement)
    {
        Object value = Evaluate(exprStatement.Expression);
        string result = Stringify(value);
        Output.Invoke(result);
    }

    public void Visit(VarDecl varDecl)
    {
        Object value = null;
        if (varDecl.Initializer != null)
        {
            value = Evaluate(varDecl.Initializer);
        }
        environment.Define(varDecl.VariableName, value);
        Output?.Invoke($"Variable '{varDecl.VariableName}' = {value}");
    }
    public void Visit(GotoStatement stmt)
    {
        var result = Evaluate(stmt.Condition);
    
        if (result is not bool condition)
        {
            throw new RuntimeError(null, "La condición del GoTo debe ser booleana.");
        }

        if (condition)
        {
            current = labelTable.Resolve(stmt.Label);
        }
    }
    public void Visit(SpawnStatement stmt)
    {
        object xValue = Evaluate(stmt.X);
        object yValue = Evaluate(stmt.Y);

        if (xValue is not int x || yValue is not int y)
        {
            throw new RuntimeError(stmt.Keyword, "Spawn requiere coordenadas numéricas.");
        }

        wallEContext.Spawn(x, y);
    }
    public void Visit(ColorStatement stmt)
    {
        object value = Evaluate(stmt.ColorExpr);

        if (value is not string colorName)
        {
            throw new RuntimeError(stmt.Keyword, "El comando Color espera un string como nombre de color.");
        }

        wallEContext.SetBrushColor(colorName);
    }
    public void Visit(SizeStatement stmt)
    {
        object value = Evaluate(stmt.SizeExpr);

        if (value is not int size)
        {
            throw new RuntimeError(stmt.Keyword, "El comando Size requiere un número entero.");
        }

        wallEContext.SetBrushSize(size);
    }
    public void Visit(DrawLineStatement stmt)
    {
        object x = Evaluate(stmt.DirX);
        object y = Evaluate(stmt.DirY);
        object d = Evaluate(stmt.Distance);

        if (x is not int dx || y is not int dy || d is not int dist)
        {
            throw new RuntimeError(stmt.Keyword, "DrawLine espera tres enteros.");
        }

        wallEContext.DrawLine(dx, dy, dist);
    }
    public void Visit(DrawCircleStatement stmt)
    {
        object x = Evaluate(stmt.DirX);
        object y = Evaluate(stmt.DirY);
        object r = Evaluate(stmt.Radius);

        if (x is not int dx || y is not int dy || r is not int radius)
            throw new RuntimeError(stmt.Keyword, "DrawCircle espera tres enteros.");

        wallEContext.DrawCircle(dx, dy, radius);
    }
    public void Visit(DrawRectangleStatement stmt)
    {
        object x = Evaluate(stmt.DirX);
        object y = Evaluate(stmt.DirY);
        object d = Evaluate(stmt.Distance);
        object w = Evaluate(stmt.Width);
        object h = Evaluate(stmt.Height);

        if (x is not int dx || y is not int dy || d is not int dist || w is not int width || h is not int height)
        {
            throw new RuntimeError(stmt.Keyword, "DrawRectangle requiere cinco enteros.");
        }

        wallEContext.DrawRectangle(dx, dy, dist, width, height);
    }
    public void Visit(FillStatement stmt)
    {
        wallEContext.Fill();
    }

    
    public Object Visit(LiteralExpression literalExpression)
    {
        return literalExpression.Value;
    }

    public Object Visit(GroupingExpression groupingExpression)
    {
        return Evaluate(groupingExpression.Expression);
    }

    public object Visit(CallExpression expr)
    {
        if (!nativeFunctions.TryGetValue(expr.FunctionName.Lexeme, out var function))
        {
            throw new RuntimeError(expr.FunctionName, $"Función no definida: {expr.FunctionName.Lexeme}");
        }

        if (expr.Arguments.Count != function.Arity)
        {
            throw new RuntimeError(expr.FunctionName, $"La función {expr.FunctionName.Lexeme} espera {function.Arity} argumentos.");
        }

        var args = new List<object>();
        foreach (var arg in expr.Arguments)
        {
            args.Add(Evaluate(arg));
        }

        return function.Call(args);
    }

    public Object Visit(UnaryExpression unaryExpression)
    {
        Object right = Evaluate(unaryExpression.Right);
        switch (unaryExpression.Operator.Type)
        {
            case TokenType.Minus:
                try
                {
                    return checked (-(int)right);
                }
                catch (InvalidCastException)
                {
                    throw new RuntimeError(unaryExpression.Operator, 
                        "Operador '-' requiere un valor numérico. Se recibió: " + 
                        right?.GetType().Name ?? "null");
                }
            case TokenType.Not:
                try 
                {
                    return !(bool)right;
                }
                catch (InvalidCastException)
                {
                    throw new RuntimeError(unaryExpression.Operator, 
                        "Operador '!' requiere un valor booleano. Se recibió: " + 
                        right?.GetType().Name ?? "null");
                }
            
            default:
                throw new RuntimeError(unaryExpression.Operator, "Operador desconocido");
        }
    }

    public Object Visit(VariableExpression variableExpression)
    {
        return environment.Get(variableExpression.Token);
    }
    public Object Visit(BinaryExpression binaryExpression)
    {
        Token op = binaryExpression.Operator;

        if (op.Type == TokenType.And || op.Type == TokenType.Or)
        {
            return HandleLogicalBinary(binaryExpression);
        }

        Object left = Evaluate(binaryExpression.Left);
        Object right = Evaluate(binaryExpression.Right);

        try
        {
            switch (op.Type)
            {
                case TokenType.Plus:
                    return HandlePlusOperator(left, right, op);

                case TokenType.Minus:
                case TokenType.Multiply:
                case TokenType.Divide:
                case TokenType.Modulo:
                    return HandleNumericOperation((int)left, (int)right, op);

                case TokenType.Power:
                    return HandlePowerOperation(left, right, op);

                case TokenType.Equal:
                case TokenType.NotEqual:
                    return HandleEquality(left, right, op);

                case TokenType.Less:
                case TokenType.LessEqual:
                case TokenType.Greater:
                case TokenType.GreaterEqual:
                    return HandleComparison((int)left, (int)right, op);

                default:
                    throw new RuntimeError(op, $"Operador desconocido: {op.Lexeme}");
            }
        }
        catch (InvalidCastException)
        {
            throw new RuntimeError(op, $"Tipos incompatibles para el operador {op.Lexeme}: {left?.GetType().Name ?? "null"} y {right?.GetType().Name ?? "null"}");
        }
    }

    public void Visit(LabelStatement stmt)
    {
        // Las etiquetas no hacen nada en tiempo de ejecución.
    }

    private string Stringify(object value)
    {
        
        //Manejo especial para números enteros
        if (value is int intValue)
        {
            return intValue.ToString();
        }
        
        // Manejo de booleanos
        if (value is bool boolValue)
        {
            return boolValue ? "true" : "false";
        }
        
        // Caso por defecto
        return value.ToString();
    }
    // Delegados para manejar salida y errores
    public Action<string> Output { get; set; } = Console.WriteLine;
    
    //Métodos auxiliares
    private object HandlePlusOperator(object left, object right, Token op)
    {
        // Caso 1: Ambos son enteros
        if (left is int leftInt && right is int rightInt)
        {
            try
            {
                return checked(leftInt + rightInt);
            }
            catch (OverflowException)
            {
                throw new RuntimeError(op, $"El resultado de {left} {op.Lexeme} {right} excede el rango permitido");
            }
        }
    
        // Caso 2: Ambos son strings
        if (left is string leftStr && right is string rightStr)
        {
            return leftStr + rightStr;
        }
    
        // Error para cualquier otro caso
        throw new RuntimeError(op, $"Operador '+' no soportado para tipos: {left?.GetType().Name ?? "null"} y {right?.GetType().Name ?? "null"}");
    }
    private int HandleNumericOperation(int left, int right, Token op)
    {
        switch (op.Type)
        {
            case TokenType.Minus:
                try
                {
                    return checked(left - right); // checked para overflow
                }
                catch (OverflowException)
                {
                    throw new RuntimeError(
                        op,
                        $"El resultado de {left} {op.Lexeme} {right} excede el rango permitido");
                }
            
            case TokenType.Multiply:
                try
                {
                    return checked(left * right); // checked para overflow
                }
                catch (OverflowException)
                {
                    throw new RuntimeError(
                        op,
                        $"El resultado de {left} {op.Lexeme} {right} excede el rango permitido");
                }
            
            case TokenType.Divide:
                if (right == 0)
                {
                    throw new RuntimeError(op, "División por cero");
                }
                return left / right;
            
            case TokenType.Modulo:
                if (right == 0)
                {
                    throw new RuntimeError(op, "Módulo por cero");
                }
                return left % right;
            
            default:
                throw new RuntimeError(op, $"Operador numérico no válido: {op.Type}");
        }
    }
    private int HandlePowerOperation(object left, object right,  Token op)
    {
        if (!(left is int baseValue))
        {
            throw new RuntimeError(op, $"Base debe ser entero. Recibido: {left?.GetType().Name ?? "null"}");
        }
    
        if (!(right is int exponentValue))
        {
            throw new RuntimeError(op, $"Exponente debe ser entero. Recibido: {right?.GetType().Name ?? "null"}");
        }
    
        if (exponentValue < 0)
        {
            throw new RuntimeError(op, "Exponente negativo no soportado");
        }
    
        try
        {
            return checked((int)Math.Pow(baseValue, exponentValue));
        }
        catch (OverflowException)
        {
            throw new RuntimeError(op, "Resultado demasiado grande para tipo entero");
        }
    }
    private bool HandleEquality(object left, object right, Token op)
    {
        bool areEqual = object.Equals(left, right); // Maneja null automáticamente
    
        return op.Type == TokenType.Equal ? areEqual : !areEqual;
    }
    private bool HandleComparison(int left, int right, Token op)
    {
        switch (op.Type)
        {
            case TokenType.Less:
                return left < right;
            
            case TokenType.LessEqual:
                return left <= right;
            
            case TokenType.Greater:
                return left > right;
            
            case TokenType.GreaterEqual:
                return left >= right;
            
            default:
                throw new RuntimeError(op, $"Operador de comparación no válido: {op}");
        }
    }
    private object HandleLogicalBinary(BinaryExpression expr)
    {
        Token op = expr.Operator;
        var left = Evaluate(expr.Left);

        if (left is not bool leftBool)
        {
            throw new RuntimeError(op, "Operando izquierdo no es booleano.");
        }

        // OR (||): corto si es true
        if (op.Type == TokenType.Or && leftBool)
        {
            return true;
        }

        // AND (&&): corto si es false
        if (op.Type == TokenType.And && !leftBool)
        {
            return false;
        }

        // Evaluar el derecho solo si es necesario
        var right = Evaluate(expr.Right);

        if (right is not bool rightBool)
        {
            throw new RuntimeError(op, "Operando derecho no es booleano.");
        }

        return op.Type == TokenType.And
            ? leftBool && rightBool
            : leftBool || rightBool;
    }

}