using PixelWallE.Core.Lexer;

namespace PixelWallE.Core.Parser.AST.Statements;

public class FillStatement : Statement
{
    public Token Keyword { get; }

    public FillStatement(Token keyword)
    {
        Keyword = keyword;
    }

    public override void Accept(IStatementVisitor visitor)
    {
        visitor.Visit(this);
    }
}