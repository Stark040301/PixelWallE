using System;
using PixelWallE.Core.Evaluator.Runtime;
namespace PixelWallE.WallE;

public class WallEContext
{
    private CanvasColor[,] canvas;

    public int CanvasSize => canvas.GetLength(0);

    public int PositionX { get; private set; }
    public int PositionY { get; private set; }

    public CanvasColor BrushColor { get; private set; } = CanvasColor.Transparent;
    public int BrushSize { get; private set; } = 1;

    private bool spawned = false;
    public WallEContext(int initialSize)
    {
        ResizeCanvas(initialSize);
    }
    public void ResizeCanvas(int newSize)
    {
        canvas = new CanvasColor[newSize, newSize];

        for (int x = 0; x < newSize; x++)
        {
            for (int y = 0; y < newSize; y++)
            {
                canvas[x, y] = CanvasColor.White;
            }
        }

        // Reinicia estado de Wall-E
        PositionX = 0;
        PositionY = 0;
        BrushColor = CanvasColor.Transparent;
        BrushSize = 1;
        spawned = false;
    }
    public CanvasColor[,] GetCanvasSnapshot()
    {
        var clone = new CanvasColor[CanvasSize, CanvasSize];
        Array.Copy(canvas, clone, canvas.Length);
        return clone;
    }

    public bool IsInBounds(int x, int y)
    {
        return x >= 0 && y >= 0 && x < CanvasSize && y < CanvasSize;
    }
    public void Spawn(int x, int y)
    {
        if (spawned)
            throw new RuntimeError(null, "El comando Spawn solo puede usarse una vez.");

        if (!IsInBounds(x, y))
            throw new RuntimeError(null, $"Spawn fuera del canvas en posición ({x}, {y})");

        PositionX = x;
        PositionY = y;
        spawned = true;
    }
    public void SetBrushColor(string colorName)
    {
        if (!Enum.TryParse<CanvasColor>(colorName, true, out var color) ||
            !Enum.IsDefined(typeof(CanvasColor), color))
        {
            throw new RuntimeError(null, $"Color de brocha inválido: \"{colorName}\"");
        }

        BrushColor = color;
    }
    public void SetBrushSize(int size)
    {
        if (size <= 0)
            BrushSize = 1;
        else
            BrushSize = size % 2 == 0 ? size - 1 : size;
    }

    public CanvasColor GetColorAt(int x, int y)
    {
        return IsInBounds(x, y) ? canvas[x, y] : CanvasColor.Transparent;
    }
    public void PaintCell(int x, int y)
    {
        if (!IsInBounds(x, y)) return;
        if (BrushColor == CanvasColor.Transparent) return;

        canvas[x, y] = BrushColor;
    }

    public int CountColorInArea(CanvasColor color, int x1, int y1, int x2, int y2)
    {
        int minX = Math.Min(x1, x2);
        int maxX = Math.Max(x1, x2);
        int minY = Math.Min(y1, y2);
        int maxY = Math.Max(y1, y2);

        int count = 0;

        for (int x = minX; x <= maxX; x++)
        for (int y = minY; y <= maxY; y++)
        {
            if (IsInBounds(x, y) && canvas[x, y] == color)
                count++;
        }

        return count;
    }
    public bool IsCanvasColor(CanvasColor color, int offsetX, int offsetY)
    {
        int x = PositionX + offsetX;
        int y = PositionY + offsetY;

        return IsInBounds(x, y) && canvas[x, y] == color;
    }

    public bool IsBrushColor(string colorName)
    {
        if (!Enum.TryParse<CanvasColor>(colorName, true, out var color) ||
            !Enum.IsDefined(typeof(CanvasColor), color))
        {
            throw new RuntimeError(null, $"Color de brocha inválido: \"{colorName}\"");
        }
        return BrushColor == color;
    }

    public bool IsBrushSize(int size)
    {
        return BrushSize == size;
    }
}