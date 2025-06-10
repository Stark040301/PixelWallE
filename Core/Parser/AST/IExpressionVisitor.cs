using PixelWallE.Core.Parser.AST.Expressions;
using PixelWallE.Core.Parser.AST.Statements;

namespace PixelWallE.Core.Parser.AST;

public interface IExpressionVisitor<T>
{
    T Visit(LiteralExpression expr);
    T Visit(VariableExpression expr);
    T Visit(BinaryExpression expr);
    T Visit(UnaryExpression expr);
    T Visit(GroupingExpression expr);
    T Visit(CallExpression expr);
    }