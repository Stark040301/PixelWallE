using Avalonia.Controls;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using AvaloniaEdit.Editing;
using System;
using System.Linq;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Media;
using Avalonia.VisualTree;
using PixelWallE.GUI.ViewModels;
using PixelWallE.WallE;
using ReactiveUI;

namespace PixelWallE.GUI
{
    public partial class MainWindow : Window
    {
        private const int DefaultCanvasSize = 20; // 20x20
        private const int CellSize = 20;
        public MainWindow()
        {
            InitializeComponent();

            var viewModel = new MainWindowViewModel();
            DataContext = viewModel;
            viewModel.RunCommand.Subscribe(_ => DrawPixelsToCanvas());
            viewModel.Pixels.CollectionChanged += (_, _) => DrawPixelsToCanvas();

            CodeEditor.GetObservable(TextBox.TextProperty).Subscribe(text =>
            {
                var lines = text?.Split('\n').Length ?? 1;
                LineNumbers.Text = string.Join('\n', Enumerable.Range(1, lines));
            });

            this.AttachedToVisualTree += (_, _) =>
            {
                var codeScroll = CodeEditor.GetVisualDescendants()
                    .OfType<ScrollViewer>()
                    .FirstOrDefault();

                if (codeScroll != null)
                {
                    codeScroll.ScrollChanged += (_, args) =>
                    {
                        LineScrollViewer.Offset = new Vector(LineScrollViewer.Offset.X, codeScroll.Offset.Y);
                    };
                }

                // ✅ Inicializar el grid SOLO cuando el control ya está renderizado
                InitializePixelGrid(DefaultCanvasSize);
            };
        }

        private void InitializePixelGrid(int size)
        {
            Console.WriteLine($"Inicializando canvas de {size}x{size}");
            CanvasGrid.Children.Clear();
            CanvasGrid.RowDefinitions.Clear();
            CanvasGrid.ColumnDefinitions.Clear();

            // Usar Auto para que las celdas mantengan un tamaño fijo
            for (int i = 0; i < size; i++)
            {
                CanvasGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
                CanvasGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
            }

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    var cell = new Border
                    {
                        Width = CellSize,
                        Height = CellSize,
                        Background = Brushes.White,
                        BorderBrush = Brushes.Gray,
                        BorderThickness = new Thickness(0.5)
                    };

                    Grid.SetRow(cell, y);
                    Grid.SetColumn(cell, x);
                    CanvasGrid.Children.Add(cell);
                }
            }
        }

        private void DrawPixelsToCanvas()
        {
            if (CanvasGrid == null || DataContext is not MainWindowViewModel vm)
                return;
            Console.WriteLine("Redibujando");
            CanvasGrid.Children.Clear();
            CanvasGrid.RowDefinitions.Clear();
            CanvasGrid.ColumnDefinitions.Clear();

            int size = vm.CanvasSize;
            const int cellSize = 20;

            for (int i = 0; i < size; i++)
                CanvasGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
    
            for (int j = 0; j < size; j++)
                CanvasGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));

            var pixels = vm.Pixels;
            
            CanvasGrid.Width = size * cellSize;
            CanvasGrid.Height = size * cellSize;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    var color = CanvasColor.White;
                    var pixel = pixels.FirstOrDefault(p => p.X == j && p.Y == i);
                    if (pixel != null) color = pixel.Color;

                    var rect = new Border
                    {
                        Width = cellSize,
                        Height = cellSize,
                        Background = new SolidColorBrush(ToAvaloniaColor(color)),
                        BorderBrush = Brushes.LightGray,
                        BorderThickness = new Thickness(0.5)
                    };

                    Grid.SetRow(rect, i);
                    Grid.SetColumn(rect, j);
                    CanvasGrid.Children.Add(rect);
                }
            }
        }


        private Color ToAvaloniaColor(CanvasColor canvasColor)
        {
            return canvasColor switch
            {
                CanvasColor.Black => Colors.Black,
                CanvasColor.Red => Colors.Red,
                CanvasColor.Green => Colors.Green,
                CanvasColor.Blue => Colors.Blue,
                CanvasColor.Yellow => Colors.Yellow,
                _ => Colors.White
            };
        }
    }
}