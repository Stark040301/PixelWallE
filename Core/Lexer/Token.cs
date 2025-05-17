namespace PixelWallE.Core.Lexer;

public record Token(TokenType Type, string Lexeme, object? Literal, int Line)
{
    public override string ToString() => $"[Línea {Line}] {Type} '{Lexeme}' {Literal ?? "null"}";
    public bool IsType(TokenType type) => Type == type;
}