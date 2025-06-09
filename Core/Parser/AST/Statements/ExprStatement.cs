using PixelWallE.Core.Lexer;
using PixelWallE.Core.Parser.AST.Expressions;

namespace PixelWallE.Core.Parser.AST.Statements;

public class ExprStatement: Statement
{
    public Expression Expression;
    public Token NewLine;
    public ExprStatement(Expression expression)
    {
        Expression = expression;
    }
    public override void Accept(IStatementVisitor statementVisitor)
    {
        statementVisitor.Visit(this);
    }
}