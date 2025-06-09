using PixelWallE.Core.Lexer;
namespace PixelWallE.Core.Parser.AST.Expressions;

public class UnaryExpression : Expression
{
    public Token Operator { get; }
    public Expression Right { get; }

    public UnaryExpression(Token op, Expression right)
    {
        Operator = op;
        Right = right;
    }

    public override T Accept<T>(IExpressionVisitor<T> expressionVisitor)
    {
        return expressionVisitor.Visit(this);
    }
}