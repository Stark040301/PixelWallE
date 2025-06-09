using PixelWallE.Core.Lexer;
namespace PixelWallE.Core.Parser.AST.Expressions;

public class VariableExpression : Expression
{
    public string Name { get; }
    public Token Token { get; }

    public VariableExpression(Token identifier)
    {
        Name = identifier.Lexeme;
        Token = identifier;
    }

    public override T Accept<T>(IExpressionVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}