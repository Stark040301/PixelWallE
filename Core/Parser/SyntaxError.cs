using System;
using PixelWallE.Core.Lexer;
namespace PixelWallE.Core.Parser;

public class SyntaxError
{
    public static void Report(int line, string where, string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[Línea {line}] Error{where}: {message}");
        Console.ResetColor();
    }
}