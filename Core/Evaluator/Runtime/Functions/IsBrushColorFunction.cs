using System;
using System.Collections.Generic;
using PixelWallE.WallE;

namespace PixelWallE.Core.Evaluator.Runtime.Functions;

public class IsBrushColorFunction : INativeFunction
{
    private readonly WallEContext context;

    public IsBrushColorFunction(WallEContext context)
    {
        this.context = context;
    }

    public int Arity => 1;

    public object Call(List<object> arguments)
    {
        if (arguments[0] is not string colorName)
        {
            throw new RuntimeError(null, "IsBrushColor espera un string como argumento.");
        }

        if (!Enum.TryParse<CanvasColor>(colorName, true, out var color) ||
            !Enum.IsDefined(typeof(CanvasColor), color))
        {
            throw new RuntimeError(null, $"Color inválido: \"{colorName}\"");
        }

        return context.BrushColor == color ? 1 : 0;
    }
}