using System;
using PixelWallE.Core.Lexer;

namespace PixelWallE.Core.Evaluator.Runtime;

public class RuntimeError : Exception
{
    public Token Token { get; }
    public int Line => Token?.Line ?? 0;
        
    public RuntimeError(Token token, string message) : base(message)
    {
        Token = token;
    }
}