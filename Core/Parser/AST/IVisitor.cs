using PixelWallE.Core.Parser.AST.Expressions;
using PixelWallE.Core.Parser.AST.Statements;

namespace PixelWallE.Core.Parser.AST;

public interface IVisitor<T>
{
    // Expresiones
    T Visit(LiteralExpression expr);
    T Visit(VariableExpression expr);
    T Visit(BinaryExpression expr);
    T Visit(UnaryExpression expr);
    T Visit(GroupingExpression expr);
    T Visit(FunctionCallExpression expr);
    
    // Statements
    T Visit(CommandStatement stmt);
    T Visit(VariableDeclaration stmt);
    T Visit(ConditionalGoto stmt);
}