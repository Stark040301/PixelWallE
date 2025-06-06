using System;
using PixelWallE.Core.Evaluator.Runtime;
using PixelWallE.Core.Lexer;
using PixelWallE.Core.Parser.AST;
using PixelWallE.Core.Parser.AST.Expressions;

namespace PixelWallE.Core.Evaluator;

public class Interpreter: IVisitor<Object>
{
    public void Interpret(Expression expression)
    {
        try
        {
            Object value = Evaluate(expression);
            string result = Stringify(value);
            Output.Invoke(result);
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
    public Object Visit(LiteralExpression literalExpression)
    {
        return literalExpression.Value;
    }

    public Object Visit(GroupingExpression groupingExpression)
    {
        return Evaluate(groupingExpression.Expression);
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
    public object Visit(BinaryExpression binaryExpression)
    {
        Object right = Evaluate(binaryExpression.Right);
        Object left = Evaluate(binaryExpression.Left);
    
        try
        {
            switch (binaryExpression.Operator.Type)
            {
                case TokenType.Plus:
                    return HandlePlusOperator(left, right, binaryExpression.Operator);
                case TokenType.Minus:
                case TokenType.Multiply:
                case TokenType.Divide:
                case TokenType.Modulo:
                    return HandleNumericOperation((int)left, (int)right, binaryExpression.Operator);
                case TokenType.Power:
                    return HandlePowerOperation(left, right, binaryExpression.Operator);
                case TokenType.Equal:
                case TokenType.NotEqual:
                    return HandleEquality(left, right, binaryExpression.Operator);
                case TokenType.Less:
                case TokenType.LessEqual:
                case TokenType.Greater:
                case TokenType.GreaterEqual:
                    return HandleComparison((int)left, (int)right, binaryExpression.Operator);
                case TokenType.And:
                case TokenType.Or:
                    return HandleLogicalOperation(left, right, binaryExpression.Operator);
                default:
                    throw new RuntimeError(binaryExpression.Operator, "Operador desconocido.");
            }
        }
        catch (InvalidCastException)
        {
            throw new RuntimeError(binaryExpression.Operator, 
                $"Tipos incompatibles: {left?.GetType().Name ?? "null"} y " +
                $"{right?.GetType().Name ?? "null"} para el operador " +
                $"{binaryExpression.Operator.Lexeme}");
        }
        catch (OverflowException)
        {
            throw new RuntimeError(binaryExpression.Operator, $"Overflow en operación: {left} {binaryExpression.Operator.Lexeme} {right}");
        }
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
    private bool HandleLogicalOperation(object left, object right, Token op)
    {
        if (!(left is bool leftBool))
        {
            throw new RuntimeError(op, $"Operando izquierdo debe ser booleano. Recibido: {left?.GetType().Name ?? "null"}");
        }
    
        if (!(right is bool rightBool))
        {
            throw new RuntimeError(op, $"Operando derecho debe ser booleano. Recibido: {right?.GetType().Name ?? "null"}");
        }
    
        return op.Type == TokenType.And 
            ? leftBool && rightBool 
            : leftBool || rightBool;
    }
}