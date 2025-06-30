using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Controls.Shapes;
using System;
using System.Collections.Generic;
using PixelWallE.WallE;

namespace PixelWallE.GUI
{
    public class PixelCanvas : Control
    {
        private CanvasColor[,] _pixels;
        private bool _showGrid = true;

        public bool ShowGrid
        {
            get => _showGrid;
            set
            {
                _showGrid = value;
                InvalidateVisual(); // Redibuja cuando cambia
            }
        }

        // Propiedades públicas para binding
        public static readonly StyledProperty<int> ColumnsProperty =
            AvaloniaProperty.Register<PixelCanvas, int>(nameof(Columns), defaultValue: 100);

        public static readonly StyledProperty<int> RowsProperty =
            AvaloniaProperty.Register<PixelCanvas, int>(nameof(Rows), defaultValue: 100);

        public static readonly StyledProperty<int> CellSizeProperty =
            AvaloniaProperty.Register<PixelCanvas, int>(nameof(CellSize), defaultValue: 10);

        public int Columns
        {
            get => GetValue(ColumnsProperty);
            set => SetValue(ColumnsProperty, value);
        }

        public int Rows
        {
            get => GetValue(RowsProperty);
            set => SetValue(RowsProperty, value);
        }

        public int CellSize
        {
            get => GetValue(CellSizeProperty);
            set => SetValue(CellSizeProperty, value);
        }

        public PixelCanvas()
        {
            _pixels = new CanvasColor[Columns, Rows];
            InitializePixels();
            // Límites razonables
            MinWidth = 100;
            MinHeight = 100;
            MaxWidth = 2000;
            MaxHeight = 2000;
        }

        private void InitializePixels()
        {
            for (int y = 0; y < Rows; y++)
                for (int x = 0; x < Columns; x++)
                    _pixels[x, y] = CanvasColor.White;
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == ColumnsProperty || 
                change.Property == RowsProperty ||
                change.Property == CellSizeProperty)
            {
                _pixels = new CanvasColor[Columns, Rows];
                InitializePixels();
                InvalidateVisual();
            }
        }

        public void SetPixel(int x, int y, CanvasColor color)
        {
            if (x >= 0 && x < Columns && y >= 0 && y < Rows)
            {
                _pixels[x, y] = color;
                InvalidateVisual();
            }
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);
    
            // Dibuja los píxeles
            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    var brush = GetBrushForColor(_pixels[x, y]);
                    var rect = new Rect(x * CellSize, y * CellSize, CellSize, CellSize);
                    context.DrawRectangle(brush, null, rect); // Sin borde aquí
                }
            }
    
            // Dibuja las cuadrículas si están activas
            if (ShowGrid)
            {
                var gridPen = new Pen(Brushes.LightGray, 1);
        
                // Líneas verticales
                for (int x = 0; x <= Columns; x++)
                {
                    var start = new Point(x * CellSize, 0);
                    var end = new Point(x * CellSize, Rows * CellSize);
                    context.DrawLine(gridPen, start, end);
                }
        
                // Líneas horizontales
                for (int y = 0; y <= Rows; y++)
                {
                    var start = new Point(0, y * CellSize);
                    var end = new Point(Columns * CellSize, y * CellSize);
                    context.DrawLine(gridPen, start, end);
                }
            }
        }
        
        private static readonly Dictionary<CanvasColor, IBrush> _colorBrushes = new()
        {
            { CanvasColor.Transparent, Brushes.Transparent },
            { CanvasColor.AliceBlue, Brushes.AliceBlue },
            { CanvasColor.AntiqueWhite, Brushes.AntiqueWhite },
            { CanvasColor.Aqua, Brushes.Aqua },
            { CanvasColor.Aquamarine, Brushes.Aquamarine },
            { CanvasColor.Azure, Brushes.Azure },
            { CanvasColor.Beige, Brushes.Beige },
            { CanvasColor.Bisque, Brushes.Bisque },
            { CanvasColor.Black, Brushes.Black },
            { CanvasColor.BlanchedAlmond, Brushes.BlanchedAlmond },
            { CanvasColor.Blue, Brushes.Blue },
            { CanvasColor.BlueViolet, Brushes.BlueViolet },
            { CanvasColor.Brown, Brushes.Brown },
            { CanvasColor.BurlyWood, Brushes.BurlyWood },
            { CanvasColor.CadetBlue, Brushes.CadetBlue },
            { CanvasColor.Chartreuse, Brushes.Chartreuse },
            { CanvasColor.Chocolate, Brushes.Chocolate },
            { CanvasColor.Coral, Brushes.Coral },
            { CanvasColor.CornflowerBlue, Brushes.CornflowerBlue },
            { CanvasColor.Cornsilk, Brushes.Cornsilk },
            { CanvasColor.Crimson, Brushes.Crimson },
            { CanvasColor.Cyan, Brushes.Cyan },
            { CanvasColor.DarkBlue, Brushes.DarkBlue },
            { CanvasColor.DarkCyan, Brushes.DarkCyan },
            { CanvasColor.DarkGoldenrod, Brushes.DarkGoldenrod },
            { CanvasColor.DarkGray, Brushes.DarkGray },
            { CanvasColor.DarkGreen, Brushes.DarkGreen },
            { CanvasColor.DarkKhaki, Brushes.DarkKhaki },
            { CanvasColor.DarkMagenta, Brushes.DarkMagenta },
            { CanvasColor.DarkOliveGreen, Brushes.DarkOliveGreen },
            { CanvasColor.DarkOrange, Brushes.DarkOrange },
            { CanvasColor.DarkOrchid, Brushes.DarkOrchid },
            { CanvasColor.DarkRed, Brushes.DarkRed },
            { CanvasColor.DarkSalmon, Brushes.DarkSalmon },
            { CanvasColor.DarkSeaGreen, Brushes.DarkSeaGreen },
            { CanvasColor.DarkSlateBlue, Brushes.DarkSlateBlue },
            { CanvasColor.DarkSlateGray, Brushes.DarkSlateGray },
            { CanvasColor.DarkTurquoise, Brushes.DarkTurquoise },
            { CanvasColor.DarkViolet, Brushes.DarkViolet },
            { CanvasColor.DeepPink, Brushes.DeepPink },
            { CanvasColor.DeepSkyBlue, Brushes.DeepSkyBlue },
            { CanvasColor.DimGray, Brushes.DimGray },
            { CanvasColor.DodgerBlue, Brushes.DodgerBlue },
            { CanvasColor.Firebrick, Brushes.Firebrick },
            { CanvasColor.FloralWhite, Brushes.FloralWhite },
            { CanvasColor.ForestGreen, Brushes.ForestGreen },
            { CanvasColor.Fuchsia, Brushes.Fuchsia },
            { CanvasColor.Gainsboro, Brushes.Gainsboro },
            { CanvasColor.GhostWhite, Brushes.GhostWhite },
            { CanvasColor.Gold, Brushes.Gold },
            { CanvasColor.Goldenrod, Brushes.Goldenrod },
            { CanvasColor.Gray, Brushes.Gray },
            { CanvasColor.Green, Brushes.Green },
            { CanvasColor.GreenYellow, Brushes.GreenYellow },
            { CanvasColor.Honeydew, Brushes.Honeydew },
            { CanvasColor.HotPink, Brushes.HotPink },
            { CanvasColor.IndianRed, Brushes.IndianRed },
            { CanvasColor.Indigo, Brushes.Indigo },
            { CanvasColor.Ivory, Brushes.Ivory },
            { CanvasColor.Khaki, Brushes.Khaki },
            { CanvasColor.Lavender, Brushes.Lavender },
            { CanvasColor.LavenderBlush, Brushes.LavenderBlush },
            { CanvasColor.LawnGreen, Brushes.LawnGreen },
            { CanvasColor.LemonChiffon, Brushes.LemonChiffon },
            { CanvasColor.LightBlue, Brushes.LightBlue },
            { CanvasColor.LightCoral, Brushes.LightCoral },
            { CanvasColor.LightCyan, Brushes.LightCyan },
            { CanvasColor.LightGoldenrodYellow, Brushes.LightGoldenrodYellow },
            { CanvasColor.LightGray, Brushes.LightGray },
            { CanvasColor.LightGreen, Brushes.LightGreen },
            { CanvasColor.LightPink, Brushes.LightPink },
            { CanvasColor.LightSalmon, Brushes.LightSalmon },
            { CanvasColor.LightSeaGreen, Brushes.LightSeaGreen },
            { CanvasColor.LightSkyBlue, Brushes.LightSkyBlue },
            { CanvasColor.LightSlateGray, Brushes.LightSlateGray },
            { CanvasColor.LightSteelBlue, Brushes.LightSteelBlue },
            { CanvasColor.LightYellow, Brushes.LightYellow },
            { CanvasColor.Lime, Brushes.Lime },
            { CanvasColor.LimeGreen, Brushes.LimeGreen },
            { CanvasColor.Linen, Brushes.Linen },
            { CanvasColor.Magenta, Brushes.Magenta },
            { CanvasColor.Maroon, Brushes.Maroon },
            { CanvasColor.MediumAquamarine, Brushes.MediumAquamarine },
            { CanvasColor.MediumBlue, Brushes.MediumBlue },
            { CanvasColor.MediumOrchid, Brushes.MediumOrchid },
            { CanvasColor.MediumPurple, Brushes.MediumPurple },
            { CanvasColor.MediumSeaGreen, Brushes.MediumSeaGreen },
            { CanvasColor.MediumSlateBlue, Brushes.MediumSlateBlue },
            { CanvasColor.MediumSpringGreen, Brushes.MediumSpringGreen },
            { CanvasColor.MediumTurquoise, Brushes.MediumTurquoise },
            { CanvasColor.MediumVioletRed, Brushes.MediumVioletRed },
            { CanvasColor.MidnightBlue, Brushes.MidnightBlue },
            { CanvasColor.MintCream, Brushes.MintCream },
            { CanvasColor.MistyRose, Brushes.MistyRose },
            { CanvasColor.Moccasin, Brushes.Moccasin },
            { CanvasColor.NavajoWhite, Brushes.NavajoWhite },
            { CanvasColor.Navy, Brushes.Navy },
            { CanvasColor.OldLace, Brushes.OldLace },
            { CanvasColor.Olive, Brushes.Olive },
            { CanvasColor.OliveDrab, Brushes.OliveDrab },
            { CanvasColor.Orange, Brushes.Orange },
            { CanvasColor.OrangeRed, Brushes.OrangeRed },
            { CanvasColor.Orchid, Brushes.Orchid },
            { CanvasColor.PaleGoldenrod, Brushes.PaleGoldenrod },
            { CanvasColor.PaleGreen, Brushes.PaleGreen },
            { CanvasColor.PaleTurquoise, Brushes.PaleTurquoise },
            { CanvasColor.PaleVioletRed, Brushes.PaleVioletRed },
            { CanvasColor.PapayaWhip, Brushes.PapayaWhip },
            { CanvasColor.PeachPuff, Brushes.PeachPuff },
            { CanvasColor.Peru, Brushes.Peru },
            { CanvasColor.Pink, Brushes.Pink },
            { CanvasColor.Plum, Brushes.Plum },
            { CanvasColor.PowderBlue, Brushes.PowderBlue },
            { CanvasColor.Purple, Brushes.Purple },
            { CanvasColor.Red, Brushes.Red },
            { CanvasColor.RosyBrown, Brushes.RosyBrown },
            { CanvasColor.RoyalBlue, Brushes.RoyalBlue },
            { CanvasColor.SaddleBrown, Brushes.SaddleBrown },
            { CanvasColor.Salmon, Brushes.Salmon },
            { CanvasColor.SandyBrown, Brushes.SandyBrown },
            { CanvasColor.SeaGreen, Brushes.SeaGreen },
            { CanvasColor.SeaShell, Brushes.SeaShell },
            { CanvasColor.Sienna, Brushes.Sienna },
            { CanvasColor.Silver, Brushes.Silver },
            { CanvasColor.SkyBlue, Brushes.SkyBlue },
            { CanvasColor.SlateBlue, Brushes.SlateBlue },
            { CanvasColor.SlateGray, Brushes.SlateGray },
            { CanvasColor.Snow, Brushes.Snow },
            { CanvasColor.SpringGreen, Brushes.SpringGreen },
            { CanvasColor.SteelBlue, Brushes.SteelBlue },
            { CanvasColor.Tan, Brushes.Tan },
            { CanvasColor.Teal, Brushes.Teal },
            { CanvasColor.Thistle, Brushes.Thistle },
            { CanvasColor.Tomato, Brushes.Tomato },
            { CanvasColor.Turquoise, Brushes.Turquoise },
            { CanvasColor.Violet, Brushes.Violet },
            { CanvasColor.Wheat, Brushes.Wheat },
            { CanvasColor.White, Brushes.White },
            { CanvasColor.WhiteSmoke, Brushes.WhiteSmoke },
            { CanvasColor.Yellow, Brushes.Yellow },
            { CanvasColor.YellowGreen, Brushes.YellowGreen }
        };
        private IBrush GetBrushForColor(CanvasColor color)
        {
            if (_colorBrushes.TryGetValue(color, out var brush))
            {
                return brush;
            }
            return Brushes.Transparent; // Fallback para colores no definidos
        }
    }
}