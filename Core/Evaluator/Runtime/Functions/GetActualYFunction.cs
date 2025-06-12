using System.Collections.Generic;
using PixelWallE.WallE;

namespace PixelWallE.Core.Evaluator.Runtime.Functions;

public class GetActualYFunction : INativeFunction
{
    private readonly WallEContext context;

    public GetActualYFunction(WallEContext context)
    {
        this.context = context;
    }

    public int Arity => 0;

    public object Call(List<object> arguments)
    {
        return context.PositionY;
    }
}