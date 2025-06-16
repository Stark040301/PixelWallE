using System;
using System.Collections.Generic;
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

    public void DrawLine(int dx, int dy, int distance)
    {
        if (dx < -1 || dx > 1 || dy < -1 || dy > 1 || (dx == 0 && dy == 0))
            throw new RuntimeError(null, "Dirección inválida para DrawLine.");

        int x = PositionX;
        int y = PositionY;

        for (int step = 0; step < distance; step++)
        {
            PaintBrushAt(x, y);
            x += dx;
            y += dy;
        }

        PaintBrushAt(x, y); // Pinta último paso
        if (IsInBounds(x, y))
        {
            PositionX = x;
            PositionY = y;
        }
    }
    public void DrawCircle(int dx, int dy, int radius)
    {
        if (dx < -1 || dx > 1 || dy < -1 || dy > 1 )
            throw new RuntimeError(null, "Dirección inválida para DrawCircle.");

        int centerX = PositionX + dx * radius;
        int centerY = PositionY + dy * radius;

        // Pintar puntos en la circunferencia
        int rSquared = radius * radius;

        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                int distSquared = x * x + y * y;

                // Pintamos solo los puntos en el borde (con tolerancia de ±1)
                if (Math.Abs(distSquared - rSquared) <= radius)
                {
                    PaintBrushAt(centerX + x, centerY + y);
                }
            }
        }

        if (IsInBounds(centerX, centerY))
        {
            PositionX = centerX;
            PositionY = centerY;
        }
    }

    public void DrawRectangle(int dx, int dy, int distance, int width, int height)
    {
        if (dx < -1 || dx > 1 || dy < -1 || dy > 1)
            throw new RuntimeError(null, "Dirección inválida para DrawRectangle.");
        int centerX = PositionX + dx * distance;
        int centerY = PositionY + dy * distance;
        int halfW = width / 2;
        int halfH = height / 2;
        int left = centerX - halfW;
        int right = centerX + halfW;
        int top = centerY - halfH;
        int bottom = centerY + halfH;
        for (int x = left; x <= right; x++)
        {
            PaintBrushAt(x, top);
            PaintBrushAt(x, bottom);
        }

        for (int y = top + 1; y < bottom; y++)
        {
            PaintBrushAt(left, y);
            PaintBrushAt(right, y);
        }
        if (IsInBounds(centerX, centerY))
        {
            PositionX = centerX;
            PositionY = centerY;
        }
    }
    public void Fill()
    {
        CanvasColor originalColor = GetColorAt(PositionX, PositionY);
        CanvasColor targetColor = BrushColor;

        if (originalColor == targetColor || targetColor == CanvasColor.Transparent)
            return;

        Queue<(int x, int y)> queue = new();
        HashSet<(int x, int y)> visited = new();

        queue.Enqueue((PositionX, PositionY));
        visited.Add((PositionX, PositionY));

        while (queue.Count > 0)
        {
            var (x, y) = queue.Dequeue();
            PaintCell(x, y);

            foreach (var (nx, ny) in GetNeighbors(x, y))
            {
                if (IsInBounds(nx, ny) && !visited.Contains((nx, ny)) && GetColorAt(nx, ny) == originalColor)
                {
                    queue.Enqueue((nx, ny));
                    visited.Add((nx, ny));
                }
            }
        }
    }


    public CanvasColor GetColorAt(int x, int y)
    {
        return IsInBounds(x, y) ? canvas[x, y] : CanvasColor.Transparent;
    }
    private void PaintBrushAt(int cx, int cy)
    {
        int half = BrushSize / 2;

        for (int dx = -half; dx <= half; dx++)
        for (int dy = -half; dy <= half; dy++)
        {
            PaintCell(cx + dx, cy + dy);
        }
    }

    public void PaintCell(int x, int y)
    {
        if (!IsInBounds(x, y)) return;
        if (BrushColor == CanvasColor.Transparent) return;

        canvas[x, y] = BrushColor;
    }
    private IEnumerable<(int, int)> GetNeighbors(int x, int y)
    {
        yield return (x + 1, y);
        yield return (x - 1, y);
        yield return (x, y + 1);
        yield return (x, y - 1);
    }
}