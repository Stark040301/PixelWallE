using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace PixelWallE.Core.Lexer;

public class Lexer
{
    private readonly string _source; 
    private int _position; 
    private int _currentLine; 

    public Lexer(string source)
    {
        _source = source; // Almacena el codigo fuente
        _position = 0; // Puntero al caracter actual
        _currentLine = 1; // Contador de lineas
    }
    private char CurrentChar
    {
        get
        {
            if (_position >= _source.Length)
            {
                return '\0'; // Carácter nulo para fin de archivo
            }
            return _source[_position];
        }
    }
    private void Advance() => _position++;
    private bool IsAtEnd() => _position >= _source.Length;
    public IEnumerable<Token> Tokenize()
    {
        while (!IsAtEnd()) // Mientras no estemos en el final del codigo
        {
            var token = GetNextToken(); 
            if (token != null && token.Type != TokenType.Ignore)
            {
                yield return token; //devuelve el token valido
            }
        }
        yield return new Token(TokenType.EoF, "", null, _currentLine); // token final
    }
    private Token GetNextToken()
    {
        foreach (var (pattern, type) in _tokenPatterns) //prueba cada patron
        {
            var match = pattern.Match(_source.Substring(_position)); //intenta hacer coincidir el patron con el inicio del substring
            if (match.Success)
            {
                string value = match.Value; //almacena el texto que coincidió
                _position += value.Length; // mueve el puntero al final del token

                // Manejo especial para saltos de línea
                if (type == TokenType.NewLine)
                {
                    // Ignorar múltiples saltos de línea consecutivos
                    while (!IsAtEnd() && (CurrentChar == '\n' || CurrentChar == '\r'))
                    {
                        Advance();
                        _currentLine++;
                    }
                    return new Token(type, "\\n", null, _currentLine - 1);
                }

                // Manejo de palabras reservadas
                if (type == TokenType.Identifier && Keywords.TryGetValue(value.ToLower(), out var keywordType))
                {
                    return new Token(keywordType, value, null, _currentLine);
                }

                // Crear el token correspondiente
                return CreateToken(type, value);
            }
        }

        throw new Exception($"Syntax Error: Carácter inesperado '{CurrentChar}' en línea {_currentLine}");
    }
    private Token CreateToken(TokenType type, string value)
    {
        object literal = null;

        switch (type)
        {
            case TokenType.Number:
                literal = int.Parse(value); // Solo enteros
                break;
            case TokenType.String:
                literal = value.Substring(1, value.Length - 2); // Quitar comillas
                break;
            case TokenType.Boolean:
                literal = bool.Parse(value);
                break;
        }

        return new Token(type, value, literal, _currentLine);
    }
    
    private static readonly Dictionary<string, TokenType> Keywords = new(StringComparer.OrdinalIgnoreCase)
    {
        // Comandos básicos (instrucciones)
        { "spawn", TokenType.Spawn },
        { "color", TokenType.Color },
        { "size", TokenType.Size },
        { "drawline", TokenType.DrawLine },
        { "drawcircle", TokenType.DrawCircle },
        { "drawrectangle", TokenType.DrawRectangle },
        { "fill", TokenType.Fill },
        
        // Saltos Condicionales
        { "goto", TokenType.GoTo },
        
        // Valores booleanos
        { "true", TokenType.Boolean },
        { "false", TokenType.Boolean }
    };
    
    private readonly List<(Regex pattern, TokenType type)> _tokenPatterns = new()
    {
        // ==============================================
        // 2=1. Saltos de línea (importantes para la gramática)
        // ==============================================
        (new Regex(@"^(\r\n|\n|\r)"), TokenType.NewLine),               // Salto de línea
        
        
        // ==============================================
        // 2. Espacios, comentarios y tabs (se ignoran)
        // ==============================================
        (new Regex(@"^[ \t]+"), TokenType.Ignore),               // Espacios (excepto \n)
        
        (new Regex(@"^//[^\r\n]*"), TokenType.Ignore),        // Comentarios de línea (// ...)
        
        
        // ==============================================
        // 3. Operadores, literales y valores constantes
        // ==============================================
        (new Regex(@"^<-"), TokenType.Arrow),                 // Asignación
        (new Regex(@"^=="), TokenType.Equal),                 // Igualdad
        (new Regex(@"^!="), TokenType.NotEqual),              // Desigualdad
        (new Regex(@"^>="), TokenType.GreaterEqual),          // Mayor o igual que
        (new Regex(@"^>"), TokenType.Greater),                // Mayor que 
        (new Regex(@"^<="), TokenType.LessEqual),             // Menor o igual que
        (new Regex(@"^<"), TokenType.Less),                   // Menor que
        (new Regex(@"^&&"), TokenType.And),                   // AND lógico
        (new Regex(@"^\|\|"), TokenType.Or),                  // OR lógico
        (new Regex(@"^!"), TokenType.Not),                    // Negaciòn
        (new Regex(@"^\+"), TokenType.Plus),                  // Suma
        (new Regex(@"^-"), TokenType.Minus),                  // Resta
        (new Regex(@"^\*\*"), TokenType.Power),               // Potencia
        (new Regex(@"^\*"), TokenType.Multiply),              // Multiplicación
        (new Regex(@"^/"), TokenType.Divide),                 // División
        (new Regex(@"^%"), TokenType.Modulo),                 // Módulo
        (new Regex(@"^(?<=\s|^|\(|\[|,)-?\d+"), TokenType.Number), // Números con manejo contextual de negativos
        (new Regex(@"^\d+"), TokenType.Number), // Números positivos
        (new Regex(@"^\b(true|false)\b", RegexOptions.IgnoreCase), TokenType.Boolean), // Booleanos
        (new Regex(@"^""[^""]*"""), TokenType.String), // Strings
        
        // ==============================================
        // 4. Palabras reservadas (case-insensitive)
        // ==============================================
        (new Regex(@"^\b(spawn)\b", RegexOptions.IgnoreCase), TokenType.Spawn),
        (new Regex(@"^\b(color)\b", RegexOptions.IgnoreCase), TokenType.Color),
        (new Regex(@"^\b(size)\b", RegexOptions.IgnoreCase), TokenType.Size),
        (new Regex(@"^\b(drawline)\b", RegexOptions.IgnoreCase), TokenType.DrawLine),
        (new Regex(@"^\b(drawcircle)\b", RegexOptions.IgnoreCase), TokenType.DrawCircle),
        (new Regex(@"^\b(drawrectangle)\b", RegexOptions.IgnoreCase), TokenType.DrawRectangle),
        (new Regex(@"^\b(fill)\b", RegexOptions.IgnoreCase), TokenType.Fill),
        (new Regex(@"^\b(goto)\b", RegexOptions.IgnoreCase), TokenType.GoTo),
        
        // ==============================================
        // 5. Identificadores (variables y labels)
        // ==============================================
        (new Regex(@"^[a-zA-Z][a-zA-Z0-9_]*"), TokenType.Identifier), // No empieza con _ ni número
        
        // ==============================================
        // 6. Símbolos
        // ==============================================
        (new Regex(@"^\("), TokenType.LeftParen),             // Paréntesis izquierdo
        (new Regex(@"^\)"), TokenType.RightParen),            // Paréntesis derecho
        (new Regex(@"^\["), TokenType.LeftBracket),           // Corchete izquierdo
        (new Regex(@"^\]"), TokenType.RightBracket),          // Corchete derecho
        (new Regex(@"^,"), TokenType.Comma),                  // Coma
    };
}