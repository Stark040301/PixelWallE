using PixelWallE.Core.Lexer;
using PixelWallE.Core.Parser.AST.Expressions;

namespace PixelWallE.Core.Parser.AST.Statements;

public class DrawLineStatement : Statement
{
    public Token Keyword { get; }
    public Expression DirX { get; }
    public Expression DirY { get; }
    public Expression Distance { get; }

    public DrawLineStatement(Token keyword, Expression dirX, Expression dirY, Expression distance)
    {
        Keyword = keyword;
        DirX = dirX;
        DirY = dirY;
        Distance = distance;
    }

    public override void Accept(IStatementVisitor visitor)
    {
        visitor.Visit(this);
    }
}