using PixelWallE.Core.Lexer;
using PixelWallE.Core.Parser.AST.Expressions;
namespace PixelWallE.Core.Parser.AST.Statements;

public class GotoStatement : Statement
{
    public string Label { get; }
    public Expression Condition { get; }

    public GotoStatement(Token labelToken, Expression condition)
    {
        Label = labelToken.Lexeme;
        Condition = condition;
    }

    public override void Accept(IStatementVisitor visitor)
    {
        visitor.Visit(this);
    }
}