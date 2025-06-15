using Avalonia.Controls;
using AvaloniaEdit;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.Markup.Xaml;
using System;
using System.Text;
using Avalonia;
using Avalonia.VisualTree;
using AvaloniaEdit.Highlighting;

namespace PixelWallE.GUI
{
    public partial class MainWindow : Window
    {
        private const int DefaultSize = 10; // 20x20 celdas
        private const int CellSize = 25;    // 25px por celda

        public MainWindow()
        {
            InitializeComponent();
            // Esperar a que la ventana esté completamente cargada
            this.Opened += (sender, e) => InitializeCanvas();
        }
        private void CodeEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateLineNumbers();
            SyncScrollViewers();
        }
        private void UpdateLineNumbers()
        {
            var lineCount = CodeEditor.Text.Split('\n').Length;
            var sb = new StringBuilder();
        
            for (int i = 1; i <= lineCount; i++)
            {
                sb.AppendLine(i.ToString());
            }
        
            LineNumbers.Text = sb.ToString().TrimEnd();
        }

        private void SyncScrollViewers()
        {
            var editorScroll = CodeEditor.GetScrollViewer();
            if (editorScroll != null && LineNumberScroll != null)
            {
                Dispatcher.UIThread.Post(() => 
                {
                    LineNumberScroll.Offset = new Vector(0, editorScroll.Offset.Y);
                });
            }
        }
        private void InitializeCanvas()
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                try
                {
                    // Limpiar grids existentes
                    CanvasGrid.Children.Clear();
                    CanvasGrid.RowDefinitions.Clear();
                    CanvasGrid.ColumnDefinitions.Clear();
                    
                    XCoordinatesGrid.Children.Clear();
                    XCoordinatesGrid.ColumnDefinitions.Clear();
                    
                    YCoordinatesGrid.Children.Clear();
                    YCoordinatesGrid.RowDefinitions.Clear();
        
                    // Configurar filas y columnas del canvas principal
                    for (int i = 0; i < DefaultSize; i++)
                    {
                        CanvasGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
                        CanvasGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
                        
                        // Configurar columnas para coordenadas X
                        XCoordinatesGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
                        
                        // Configurar filas para coordenadas Y
                        YCoordinatesGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
                    }
        
                    // Crear celdas con bordes visibles
                    for (int y = 0; y < DefaultSize; y++)
                    {
                        for (int x = 0; x < DefaultSize; x++)
                        {
                            var cell = new Border
                            {
                                Width = CellSize,
                                Height = CellSize,
                                Background = Brushes.White,
                                BorderBrush = Brushes.LightGray,
                                BorderThickness = new Thickness(1),
                                HorizontalAlignment = HorizontalAlignment.Stretch,
                                VerticalAlignment = VerticalAlignment.Stretch
                            };
        
                            Grid.SetRow(cell, y);
                            Grid.SetColumn(cell, x);
                            CanvasGrid.Children.Add(cell);
                        }
                    }
        
                    // Añadir coordenadas X (columnas)
                    for (int x = 0; x < DefaultSize; x++)
                    {
                        var coordText = new TextBlock
                        {
                            Text = x.ToString(),
                            FontSize = 12,
                            Foreground = Brushes.Black,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            Width = CellSize,
                            Margin = new Thickness(0, 0, 0.5, 0) // Ajuste fino para alineación
                        };
    
                        Grid.SetColumn(coordText, x);
                        XCoordinatesGrid.Children.Add(coordText);
                    }

                    // Añadir coordenadas Y (filas)
                    for (int y = 0; y < DefaultSize; y++)
                    {
                        var coordText = new TextBlock
                        {
                            Text = y.ToString(),
                            FontSize = 12,
                            Foreground = Brushes.Black,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            Height = CellSize,
                            Margin = new Thickness(0, 0, 0, 0.5) // Ajuste fino para alineación
                        };
    
                        Grid.SetRow(coordText, y);
                        YCoordinatesGrid.Children.Add(coordText);
                    }
        
                    // Ajustar tamaño del contenedor
                    CanvasGrid.Width = DefaultSize * CellSize;
                    CanvasGrid.Height = DefaultSize * CellSize;
                    XCoordinatesGrid.Width = DefaultSize * CellSize;
                    YCoordinatesGrid.Height = DefaultSize * CellSize;
                    
                    // Forzar redibujado
                    CanvasGrid.InvalidateVisual();
                    XCoordinatesGrid.InvalidateVisual();
                    YCoordinatesGrid.InvalidateVisual();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error inicializando canvas: {ex}");
                }
            }, DispatcherPriority.Render);
        }
    }
}
public static class ControlExtensions
{
    public static ScrollViewer GetScrollViewer(this Control control)
    {
        if (control is ScrollViewer sv) return sv;
        
        foreach (var child in control.GetVisualChildren())
        {
            if (child is Control c)
            {
                var result = GetScrollViewer(c);
                if (result != null) return result;
            }
        }
        return null;
    }
}