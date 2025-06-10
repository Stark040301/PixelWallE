using System.Collections.Generic;

namespace PixelWallE.Core.Evaluator.Runtime;

public interface INativeFunction
{
    int Arity { get; }
    object Call(List<object> arguments);
}