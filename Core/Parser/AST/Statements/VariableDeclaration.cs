/*using PixelWallE.Core.Lexer;
using PixelWallE.Core.Parser.AST.Expressions;
namespace PixelWallE.Core.Parser.AST.Statements;

public class VariableDeclaration : Statement
{
    public string VariableName { get; }
    public Expression Initializer { get; }

    public VariableDeclaration(Token identifier, Expression initializer)
    {
        VariableName = identifier.Lexeme;
        Initializer = initializer;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}*/