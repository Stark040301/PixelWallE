using System.Collections.Generic;
using PixelWallE.WallE;

namespace PixelWallE.Core.Evaluator.Runtime.Functions;

public class IsBrushSizeFunction : INativeFunction
{
    private readonly WallEContext context;

    public IsBrushSizeFunction(WallEContext context)
    {
        this.context = context;
    }

    public int Arity => 1;

    public object Call(List<object> arguments)
    {
        if (arguments[0] is not int size)
        {
            throw new RuntimeError(null, "IsBrushSize espera un entero como argumento.");
        }

        return context.BrushSize == size ? 1 : 0;
    }
}