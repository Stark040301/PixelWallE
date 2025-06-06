using Avalonia;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PixelWallE.Core;
using PixelWallE.Core.Common;
using PixelWallE.Core.Evaluator;
using PixelWallE.Core.Evaluator.Runtime;
using PixelWallE.Core.Lexer;
using PixelWallE.Core.Parser;
using PixelWallE.Core.Parser.AST.Expressions;
using PixelWallE.Core.Tests;

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
        while(true)
        {
            Console.Write("> ");
            string line = Console.ReadLine();
            if (line == null) break; // Ctrl+Z/Ctrl+D in Windows/Linux
            Run(line);
            HadError = false;
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
        Expression expression = parser.Parse();
        if (HadError) return;
        //Console.WriteLine(new AstPrinter().Print(expression));
        Interpreter.Interpret(expression);
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
        if (token.Type == TokenType.EOF)
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