using System.Collections.Generic;
using PixelWallE.Core.Parser.AST;
using PixelWallE.Core.Parser.AST.Expressions;
using System.Text;
using PixelWallE.Core.Parser.AST.Statements;

namespace PixelWallE.Core.Common;

public class AstPrinter : IVisitor<string>
    {
        public string Print(Expression expr)
        {
            return expr.Accept(this);
        }

        public string Print(Statement stmt)
        {
            return stmt.Accept(this);
        }

        public string PrintProgram(List<Statement> statements)
        {
            var sb = new StringBuilder();
            foreach (var stmt in statements)
            {
                sb.AppendLine(stmt.Accept(this));
            }
            return sb.ToString();
        }

        // Métodos para Expressions
        public string Visit(LiteralExpression expr)
        {
            return expr.Value?.ToString() ?? "null";
        }

        /*public string Visit(VariableExpression expr)
        {
            return expr.Name;
        }*/

        public string Visit(BinaryExpression expr)
        {
            return Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);
        }

        public string Visit(UnaryExpression expr)
        {
            return Parenthesize(expr.Operator.Lexeme, expr.Right);
        }

        public string Visit(GroupingExpression expr)
        {
            return Parenthesize("group", expr.Expression);
        }

        /*public string Visit(FunctionCallExpression expr)
        {
            var sb = new StringBuilder();
            sb.Append($"(call {expr.FunctionName.Lexeme}");
            
            foreach (var arg in expr.Arguments)
            {
                sb.Append($" {arg.Accept(this)}");
            }
            
            sb.Append(")");
            return sb.ToString();
        }

        // Métodos para Statements
        public string Visit(CommandStatement stmt)
        {
            var sb = new StringBuilder();
            sb.Append($"(cmd {stmt.CommandName}");
            
            foreach (var arg in stmt.Arguments)
            {
                sb.Append($" {arg.Accept(this)}");
            }
            
            sb.Append(")");
            return sb.ToString();
        }

        public string Visit(VariableDeclaration stmt)
        {
            return Parenthesize($"var {stmt.VariableName}", stmt.Initializer);
        }

        public string Visit(ConditionalGoto stmt)
        {
            return $"(goto [{stmt.Label}] {stmt.Condition.Accept(this)})";
        }*/



        // Método auxiliar para formatear expresiones
        private string Parenthesize(string name, params Expression[] exprs)
        {
            var sb = new StringBuilder();
            sb.Append($"({name}");
            
            foreach (var expr in exprs)
            {
                sb.Append($" {expr.Accept(this)}");
            }
            
            sb.Append(")");
            return sb.ToString();
        }
    }