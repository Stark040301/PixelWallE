using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PixelWallE.Core;
using PixelWallE.Core.Evaluator;
using PixelWallE.Core.Evaluator.Runtime;
using PixelWallE.Core.Lexer;
using PixelWallE.Core.Parser;
using PixelWallE.Core.Parser.AST.Expressions;
using PixelWallE.Core.Parser.AST.Statements;
using PixelWallE.WallE;
using Environment = System.Environment;

namespace PixelWallE;

public static class MainWallE
{
    private static WallEContext _context = new WallEContext(50);
    private static Interpreter _interpreter = new Interpreter(_context);
    private static string ErrorMessage { get; set; } = "";
    public static List<string> Errors = new List<string>();
    public static bool HadError { get; set; }
    public static bool HadRuntimeError { get; set; }
    private static void RunFile(string path)
    {
        string source = File.ReadAllText(path, Encoding.Default);
        Run(source);
        if (HadError) Environment.Exit(65);
        if (HadRuntimeError) Environment.Exit(70);
    }
    public static void RunFromGUI(string source)
    {
        Reset();
        Run(source);
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
        _interpreter.Interpret(statements);
    }
    
    public static void Report(int line, string where, string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[Línea {line}] Error{where}: {message}");
        ErrorMessage = $"Syntax Error: [Línea {line}] Error{where}: {message}";
        Errors.Add(ErrorMessage);
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
        Console.WriteLine(error.Message);
        ErrorMessage = "Runtime Error: " + error.Message;
        Errors.Add(ErrorMessage);
        HadRuntimeError = true;
    }

    private static void Reset()
    {
        HadError = false;
        HadRuntimeError = false;
    }
    public static void SetCanvasSize(int newSize)
    {
        _context = new WallEContext(newSize);
        _interpreter = new Interpreter(_context);
    }
    public static CanvasColor[,] GetCanvas()
    {
        return _context.GetCanvasSnapshot();
    }

}