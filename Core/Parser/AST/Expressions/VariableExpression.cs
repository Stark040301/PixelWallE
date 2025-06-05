/*using PixelWallE.Core.Lexer;
namespace PixelWallE.Core.Parser.AST.Expressions;

public class VariableExpression : Expression
{
    public string Name { get; }

    public VariableExpression(Token identifier)
    {
        Name = identifier.Lexeme;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}*/