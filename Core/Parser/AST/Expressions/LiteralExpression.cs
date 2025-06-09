using PixelWallE.Core.Lexer;
namespace PixelWallE.Core.Parser.AST.Expressions;

public class LiteralExpression : Expression
{
    public object Value { get; }

    public LiteralExpression(object value)
    {
        Value = value;
    }

    public override T Accept<T>(IExpressionVisitor<T> expressionVisitor)
    {
        return expressionVisitor.Visit(this);
    }
}