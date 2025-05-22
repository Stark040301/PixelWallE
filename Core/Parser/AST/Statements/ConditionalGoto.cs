using PixelWallE.Core.Lexer;
using PixelWallE.Core.Parser.AST.Expressions;
namespace PixelWallE.Core.Parser.AST.Statements;

public class ConditionalGoto : Statement
{
    public string Label { get; }
    public Expression Condition { get; }

    public ConditionalGoto(Token labelToken, Expression condition)
    {
        Label = labelToken.Lexeme;
        Condition = condition;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}