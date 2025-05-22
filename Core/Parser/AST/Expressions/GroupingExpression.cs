using PixelWallE.Core.Lexer;
namespace PixelWallE.Core.Parser.AST.Expressions;

public class GroupingExpression : Expression
{
    public Expression Expression { get; }

    public GroupingExpression(Expression expression)
    {
        Expression = expression;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}