using PixelWallE.Core.Lexer;
using PixelWallE.Core.Parser.AST.Expressions;

namespace PixelWallE.Core.Parser.AST.Statements;

public class SizeStatement: Statement
{
    public Token Keyword { get; }
    public Expression SizeExpr { get; }

    public SizeStatement(Token keyword, Expression sizeExpr)
    {
        Keyword = keyword;
        SizeExpr = sizeExpr;
    }

    public override void Accept(IStatementVisitor visitor)
    {
        visitor.Visit(this);
    }
}