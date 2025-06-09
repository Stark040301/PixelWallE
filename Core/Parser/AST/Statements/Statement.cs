namespace PixelWallE.Core.Parser.AST.Statements;

public abstract class Statement
{
    public abstract void Accept(IStatementVisitor statementVisitor);
}