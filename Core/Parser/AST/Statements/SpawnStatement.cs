using PixelWallE.Core.Lexer;
using PixelWallE.Core.Parser.AST.Expressions;

namespace PixelWallE.Core.Parser.AST.Statements;

public class SpawnStatement : Statement
{
    public Expression X { get; }
    public Expression Y { get; }

    public Token Keyword { get; } 

    public SpawnStatement(Token keyword, Expression x, Expression y)
    {
        Keyword = keyword;
        X = x;
        Y = y;
    }

    public override void Accept(IStatementVisitor visitor)
    {
        visitor.Visit(this);
    }
}