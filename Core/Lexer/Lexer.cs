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
        _source = source;
        _position = 0;
        _currentLine = 1;
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
    
        // Funciones incorporadas
        { "getactualx", TokenType.GetActualX },
        { "getactualy", TokenType.GetActualY },
        { "getcanvassize", TokenType.GetCanvasSize },
        { "getcolorcount", TokenType.GetColorCount },
        { "isbrushcolor", TokenType.IsBrushColor },
        { "isbrushsize", TokenType.IsBrushSize },
        { "iscanvascolor", TokenType.IsCanvasColor },
        
        // Saltos Condicionales
        { "goto", TokenType.GoTo },
        
        // Valores booleanos
        { "true", TokenType.Boolean },
        { "false", TokenType.Boolean }
    };
    
    private readonly List<(Regex pattern, TokenType type)> _tokenPatterns = new()
{
    // ==============================================
    // 1. Espacios (se ignoran)
    // ==============================================
    (new Regex(@"^\s+"), TokenType.Ignore),               // Espacios (excepto \n)
    
    // ==============================================
    // 2. Saltos de línea (importantes para la gramática)
    // ==============================================
    (new Regex(@"^\n"), TokenType.NewLine),               // Salto de línea
    
    // ==============================================
    // 3. Literales y valores constantes
    // ==============================================
    (new Regex(@"^\b(true|false)\b", RegexOptions.IgnoreCase), TokenType.Boolean), // Booleanos
    (new Regex(@"^\d+"), TokenType.Number),       // Números enteros
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
    // 6. Símbolos y operadores
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
    (new Regex(@"^\+"), TokenType.Plus),                  // Suma
    (new Regex(@"^-"), TokenType.Minus),                  // Resta (y negativo)
    (new Regex(@"^\*"), TokenType.Multiply),              // Multiplicación
    (new Regex(@"^/"), TokenType.Divide),                 // División
    (new Regex(@"^\*\*"), TokenType.Power),               // Potencia
    (new Regex(@"^%"), TokenType.Modulo),                 // Módulo
    (new Regex(@"^\("), TokenType.LeftParen),             // Paréntesis izquierdo
    (new Regex(@"^\)"), TokenType.RightParen),            // Paréntesis derecho
    (new Regex(@"^\["), TokenType.LeftBracket),           // Corchete izquierdo
    (new Regex(@"^\]"), TokenType.RightBracket),          // Corchete derecho
    (new Regex(@"^,"), TokenType.Comma),                  // Coma
    // ==============================================
    // 7. Funciones incorporadas
    // ==============================================
    (new Regex(@"^\b(getactualx)\b", RegexOptions.IgnoreCase), TokenType.GetActualX),
    (new Regex(@"^\b(getactualy)\b", RegexOptions.IgnoreCase), TokenType.GetActualY),
    (new Regex(@"^\b(getcanvassize)\b", RegexOptions.IgnoreCase), TokenType.GetCanvasSize),
    (new Regex(@"^\b(getcolorcount)\b", RegexOptions.IgnoreCase), TokenType.GetColorCount),
    (new Regex(@"^\b(isbrushcolor)\b", RegexOptions.IgnoreCase), TokenType.IsBrushColor),
    (new Regex(@"^\b(isbrushsize)\b", RegexOptions.IgnoreCase), TokenType.IsBrushSize),
    (new Regex(@"^\b(iscanvascolor)\b", RegexOptions.IgnoreCase), TokenType.IsCanvasColor)
};
}