namespace PixelWallE.Core.Lexer;

public record Token(TokenType Type, string Lexeme, object? Literal, int Line)
{
    public override string ToString() => $"[Línea {Line}] {Type} '{Lexeme}' {Literal ?? "null"}";
    public bool IsType(TokenType type) => Type == type;
    public bool IsOperator()
    {
        return Type == TokenType.Plus || 
               Type == TokenType.Minus ||
               Type == TokenType.Multiply ||
               Type == TokenType.Divide ||
               Type == TokenType.Power ||
               Type == TokenType.Modulo ||
               Type == TokenType.Equal ||
               Type == TokenType.NotEqual ||
               Type == TokenType.Less ||
               Type == TokenType.LessEqual ||
               Type == TokenType.Greater ||
               Type == TokenType.GreaterEqual ||
               Type == TokenType.Not;
    }
}