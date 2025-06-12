using System;
using System.Collections.Generic;
using PixelWallE.WallE;

namespace PixelWallE.Core.Evaluator.Runtime.Functions;

public class IsCanvasColorFunction : INativeFunction
{
    private readonly WallEContext context;

    public IsCanvasColorFunction(WallEContext context)
    {
        this.context = context;
    }

    public int Arity => 3;

    public object Call(List<object> arguments)
    {
        if (arguments[0] is not string colorName ||
            arguments[1] is not int vertical ||
            arguments[2] is not int horizontal)
        {
            throw new RuntimeError(null, "IsCanvasColor espera (string, int, int)");
        }

        if (!Enum.TryParse<CanvasColor>(colorName, true, out var targetColor) ||
            !Enum.IsDefined(typeof(CanvasColor), targetColor))
        {
            throw new RuntimeError(null, $"Color inválido: \"{colorName}\"");
        }

        int checkX = context.PositionX + horizontal;
        int checkY = context.PositionY + vertical;

        if (!context.IsInBounds(checkX, checkY))
            return 0;

        return context.GetColorAt(checkX, checkY) == targetColor ? 1 : 0;
    }
}