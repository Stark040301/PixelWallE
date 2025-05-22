using PixelWallE.Core.Lexer;
using System.Collections.Generic;
namespace PixelWallE.Core.Parser;

public class Parser
{
    /*private readonly List<Token> _tokens;
    private int _currentPosition;

    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
        _currentPosition = 0;
    }

    private Token CurrentToken => _tokens[_currentPosition];
    private Token PeekToken => _currentPosition + 1 < _tokens.Count ? 
        _tokens[_currentPosition + 1] : null;

    private Token Advance()
    {
        if (!IsAtEnd()) _currentPosition++;
        return PreviousToken();
    }

    private Token PreviousToken() => _tokens[_currentPosition - 1];
    private bool IsAtEnd() => CurrentToken.Type == TokenType.EOF;
    private bool Check(TokenType type) => !IsAtEnd() && CurrentToken.Type == type;

    private Token Consume(TokenType type, string errorMessage)
    {
        if (Check(type)) return Advance();
        throw new SyntaxError(errorMessage, CurrentToken.Line);
    }*/
}