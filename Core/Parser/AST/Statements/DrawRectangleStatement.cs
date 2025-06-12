using PixelWallE.Core.Lexer;
using PixelWallE.Core.Parser.AST.Expressions;
namespace PixelWallE.Core.Parser.AST.Statements;

public class DrawRectangleStatement: Statement
{
    public Token Keyword { get; }
    public Expression DirX { get; }
    public Expression DirY { get; }
    public Expression Distance { get; }
    public Expression Width { get; }
    public Expression Height { get; }

    public DrawRectangleStatement(Token keyword, Expression dirX, Expression dirY, Expression distance, Expression width, Expression height)
    {
        Keyword = keyword;
        DirX = dirX;
        DirY = dirY;
        Distance = distance;
        Width = width;
        Height = height;
    }
    public override void Accept(IStatementVisitor statementVisitor)
    {
        statementVisitor.Visit(this);
    }
}