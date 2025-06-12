using System;
using System.Collections.Generic;
using PixelWallE.WallE;

namespace PixelWallE.Core.Evaluator.Runtime.Functions;

public class GetColorCountFunction : INativeFunction
{
    private readonly WallEContext context;

    public GetColorCountFunction(WallEContext context)
    {
        this.context = context;
    }

    public int Arity => 5;

    public object Call(List<object> arguments)
    {
        if (arguments[0] is not string colorName ||
            arguments[1] is not int x1 ||
            arguments[2] is not int y1 ||
            arguments[3] is not int x2 ||
            arguments[4] is not int y2)
        {
            throw new RuntimeError(null, "GetColorCount espera (string, int, int, int, int)");
        }

        if (!Enum.TryParse<CanvasColor>(colorName, true, out var color) ||
            !Enum.IsDefined(typeof(CanvasColor), color))
        {
            throw new RuntimeError(null, $"Color inválido: \"{colorName}\"");
        }

        int minX = Math.Min(x1, x2);
        int maxX = Math.Max(x1, x2);
        int minY = Math.Min(y1, y2);
        int maxY = Math.Max(y1, y2);

        if (!context.IsInBounds(minX, minY) || !context.IsInBounds(maxX, maxY))
            return 0;

        int count = 0;
        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                if (context.GetColorAt(x, y) == color)
                    count++;
            }
        }

        return count;
    }
}