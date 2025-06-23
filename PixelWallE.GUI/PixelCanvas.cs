using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Controls.Shapes;
using System;
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

        private IBrush GetBrushForColor(CanvasColor color)
        {
            return color switch
            {
                CanvasColor.White => Brushes.White,
                CanvasColor.Black => Brushes.Black,
                CanvasColor.Red => Brushes.Red,
                CanvasColor.Green => Brushes.Green,
                CanvasColor.Blue => Brushes.Blue,
                CanvasColor.Yellow => Brushes.Yellow,
                CanvasColor.Orange => Brushes.Orange,
                CanvasColor.Purple => Brushes.Purple,
                _ => Brushes.Transparent
            };
        }
    }
}