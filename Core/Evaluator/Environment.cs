using System.Collections.Generic;
using PixelWallE.Core.Evaluator.Runtime;
using PixelWallE.Core.Lexer;

namespace PixelWallE.Core.Evaluator;

public class Environment
{
    private readonly Dictionary<string, object> values = new Dictionary<string, object>();
    public object Get(Token name)
    {
        if (values.TryGetValue(name.Lexeme, out var value))
        {
            return value;
        }
        throw new RuntimeError(name, $"Variable indefinida '{name.Lexeme}'.");
    }
    public void Define(string name, object value)
    {
        values[name] = value;
    }
}