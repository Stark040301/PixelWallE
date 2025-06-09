using Avalonia;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PixelWallE.Core;
//using PixelWallE.Core.Common;
using PixelWallE.Core.Evaluator;
using PixelWallE.Core.Evaluator.Runtime;
using PixelWallE.Core.Lexer;
using PixelWallE.Core.Parser;
using PixelWallE.Core.Parser.AST.Expressions;
using PixelWallE.Core.Parser.AST.Statements;
using PixelWallE.Core.Tests;
using Environment = System.Environment;

namespace PixelWallE;

public static class MainWallE
{
    private static readonly Interpreter Interpreter = new Interpreter();
    private static bool HadError { get; set; }
    private static bool HadRuntimeError { get; set; }
    public static void Main(string[] args)
    {
        Reset();
        try
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: pw [script]");
                Environment.Exit(64);
            }
            else if (args.Length == 1)
            {
                RunFile(args[0]);
            }
            else
            {
                RunPrompt();
            }
        }
        catch (IOException ex)
        {
            Console.Error.WriteLine($"IO Error: {ex.Message}");
            Environment.Exit(74); // Standard error code for IO errors
        }
    }
    private static void RunFile(string path)
    {
        string source = File.ReadAllText(path, Encoding.Default);
        Run(source);
        if (HadError) Environment.Exit(65);
        if (HadRuntimeError) Environment.Exit(70);
    }
    private static void RunPrompt()
    {
        Console.WriteLine("PixelWallE REPL (Escriba 'salir' para terminar)");
        Console.WriteLine("Ingrese su código y presione Enter dos veces para ejecutar:");
    
        var inputBuffer = new StringBuilder();
        int emptyLineCount = 0;

        while (true)
        {
            Console.Write("> ");
            string line = Console.ReadLine();

            // Salir con comando explícito
            if (line?.Trim().Equals("salir", StringComparison.OrdinalIgnoreCase) == true)
                break;

            // Detectar fin de entrada (Ctrl+Z/D)
            if (line == null)
            {
                Console.WriteLine();
                break;
            }

            // Contar líneas vacías para ejecución
            if (string.IsNullOrWhiteSpace(line))
            {
                emptyLineCount++;
                if (emptyLineCount >= 1) // Cambia a 2 si quieres requerir dos Enters
                {
                    if (inputBuffer.Length > 0)
                    {
                        Run(inputBuffer.ToString());
                        inputBuffer.Clear();
                    }
                    emptyLineCount = 0;
                    HadError = false;
                    continue;
                }
            }
            else
            {
                emptyLineCount = 0;
            }

            // Agregar línea al buffer con salto de línea
            inputBuffer.AppendLine(line);
        }
    }

    private static void Run(string source)
    {
        Lexer lexer = new Lexer(source);
        List<Token> tokenList = new List<Token>();
        foreach (var token in lexer.Tokenize())
        {
            tokenList.Add(token);
        }
        Parser parser = new Parser(tokenList);
        List<Statement> statements = parser.Parse();
        if (HadError) return;
        Interpreter.Interpret(statements);
    }
    
    public static void Report(int line, string where, string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[Línea {line}] Error{where}: {message}");
        Console.ResetColor();
        HadError = true;
    }

    public static void SyntaxError(Token token, string message)
    {
        if (token.Type == TokenType.EoF)
        {
            Report(token.Line, " at end", message);
        }
        else
        {
            Report(token.Line, " at " + token.Lexeme + "'", message);
        }
    }

    public static void RuntimeError(RuntimeError error)
    {
        Console.WriteLine(error.Message + "\n[line " + error.Line + "]");
        HadRuntimeError = true;
    }
    /*public static class ExitCodes
    {
        public const int Success = 0;
        public const int SyntaxErrorCode = 65;
        public const int RuntimeErrorCode = 70;
    }
    public static int GetExitCode()
    {
        if (HadError) return ExitCodes.SyntaxErrorCode;
        if (HadRuntimeError) return ExitCodes.RuntimeErrorCode;
        return ExitCodes.Success;
    }*/

    private static void Reset()
    {
        HadError = false;
        HadRuntimeError = false;
    }
}