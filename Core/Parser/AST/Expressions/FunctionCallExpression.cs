using System.Collections.Generic;
using PixelWallE.Core.Lexer;
namespace PixelWallE.Core.Parser.AST.Expressions;

public class FunctionCallExpression : Expression
{
    public Token FunctionName { get; }
    public List<Expression> Arguments { get; }

    public FunctionCallExpression(Token functionName, List<Expression> arguments)
    {
        FunctionName = functionName;
        Arguments = arguments;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}