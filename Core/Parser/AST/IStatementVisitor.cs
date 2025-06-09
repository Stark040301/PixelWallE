using PixelWallE.Core.Parser.AST.Statements;

namespace PixelWallE.Core.Parser.AST;

public interface IStatementVisitor
{
    void Visit(ExprStatement exprStatement);
    void Visit(VarDecl varDecl);
}