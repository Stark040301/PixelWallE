using PixelWallE.Core.Lexer;
namespace PixelWallE.Core.Parser.AST.Expressions;

public abstract class Expression
{
    // Método para aceptar visitantes (parte del patrón Visitor)
    public abstract T Accept<T>(IVisitor<T> visitor);
}