using PixelWallE.Core.Lexer;
using PixelWallE.Core.Parser.AST.Expressions;

namespace PixelWallE.Core.Parser.AST.Statements;

public class DrawCircleStatement : Statement
{
    public Token Keyword { get; }
    public Expression DirX { get; }
    public Expression DirY { get; }
    public Expression Radius { get; }

    public DrawCircleStatement(Token keyword, Expression dirX, Expression dirY, Expression radius)
    {
        Keyword = keyword;
        DirX = dirX;
        DirY = dirY;
        Radius = radius;
    }

    public override void Accept(IStatementVisitor visitor)
    {
        visitor.Visit(this);
    }
}