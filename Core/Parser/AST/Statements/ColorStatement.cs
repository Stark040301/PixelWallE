using PixelWallE.Core.Lexer;
using PixelWallE.Core.Parser.AST.Expressions;

namespace PixelWallE.Core.Parser.AST.Statements;

public class ColorStatement : Statement
{
    public Token Keyword { get; }
    public Expression ColorExpr { get; }

    public ColorStatement(Token keyword, Expression colorExpr)
    {
        Keyword = keyword;
        ColorExpr = colorExpr;
    }

    public override void Accept(IStatementVisitor visitor)
    {
        visitor.Visit(this);
    }
}