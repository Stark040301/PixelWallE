using PixelWallE.Core.Parser.AST.Statements;

namespace PixelWallE.Core.Parser.AST;

public interface IStatementVisitor
{
    void Visit(ExprStatement exprStatement);
    void Visit(VarDecl varDecl);
    void Visit(GotoStatement gotoStatement);
    void Visit(LabelStatement labelStatement);
    void Visit(SpawnStatement spawnStatement);
    void Visit(ColorStatement colorStatement);
    void Visit(SizeStatement sizeStatement);
    void Visit(DrawLineStatement drawLineStatement);
    void Visit(DrawCircleStatement drawCircleStatement);
    void Visit(DrawRectangleStatement drawRectangleStatement);
    void Visit(FillStatement fillStatement);
}