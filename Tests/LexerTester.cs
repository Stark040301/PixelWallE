using System;
using System.Linq;
using PixelWallE.Core.Lexer;
namespace PixelWallE.Core.Tests;

public class LexerTester
{
    public static void RunTest()
    {
        Console.WriteLine("=== TESTER DEL LEXER ===");
        
        // Comandos básicos
        TestLexer("Spawn(0, 0)");
        TestLexer("Color(\"Blue\")");
        TestLexer("Size(3)");
        TestLexer("DrawLine(1, 0, 10)");
        
        // Operaciones matemáticas
        TestLexer("x <- 5 + 3 * 2");
        TestLexer("y <- (10 - 5) / 2");
        TestLexer("z <- 2 ** 3");  // Potencia
        
        // Comparaciones y lógica
        TestLexer("if x >= 5 && y <= 10 || z == -9");
        TestLexer("valid <- true || false");
        
        // Números negativos
        TestLexer("pos <- 5");
        TestLexer("neg <- -5");
        TestLexer("calc <- 5 - -3");  // Resta de negativo
        
        // Funciones incorporadas
        TestLexer("currentX <- GetActualX()");
        TestLexer("count <- GetColorCount(\"Red\", 0, 0, 10, 10)");
        
        // Saltos condicionales
        TestLexer("GoTo [start] (!x)");
        TestLexer("mi_etiqueta:");
        
        // Casos complejos
        TestLexer("Spawn(GetActualX(), GetActualY() + 5)");
        TestLexer("if IsBrushColor(\"Red\") && GetCanvasSize() > 10");
        
        // Strings con espacios
        TestLexer("Color(\"Dark Green\")");
        TestLexer("msg <- \"Este es un mensaje\"");
    }

    private static void TestLexer(string code)
    {
        Console.WriteLine($"\nAnalizando: '{code}'");
        
        try
        {
            var lexer = new Lexer.Lexer(code);
            foreach (var token in lexer.Tokenize())
            {
                if (token.Type == TokenType.EoF) continue;
                Console.WriteLine($"{token.Type,-15} '{token.Lexeme}'" + 
                                 (token.Literal != null ? $" (Literal: {token.Literal})" : ""));
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"ERROR: {ex.Message}");
            Console.ResetColor();
        }
    }
}