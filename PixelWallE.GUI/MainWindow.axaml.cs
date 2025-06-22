using Avalonia.Controls;
using AvaloniaEdit;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using AvaloniaEdit.Highlighting;
using MsBox.Avalonia;
using PixelWallE.GUI.Services;
using PixelWallE.WallE;

namespace PixelWallE.GUI
{
    public partial class MainWindow : Window
    {
        public int DefaultSize = MainWallE.GetCanvas().GetLength(0);
        private int _cellSize = 20;
        private double _thickness = 1;
        private readonly CanvasService _canvasService;
        private Dictionary<CanvasColor, IBrush> _colorMap;
        public MainWindow()
        {
            InitializeComponent();
            _canvasService = new CanvasService(this);
            InitializeColorMap();
            this.Opened += (sender, e) => InitializeCanvas();
        }
        private void InitializeColorMap()
        {
            _colorMap = new Dictionary<CanvasColor, IBrush>
            {
                { CanvasColor.White, Brushes.White },
                { CanvasColor.Black, Brushes.Black },
                { CanvasColor.Red, Brushes.Red },
                {CanvasColor.Green, Brushes.Green },
                { CanvasColor.Blue, Brushes.Blue },
                { CanvasColor.Yellow, Brushes.Yellow },
                {CanvasColor.Orange, Brushes.Orange},
                { CanvasColor.Purple, Brushes.Purple},
                {CanvasColor.Transparent, Brushes.Transparent}
            };
        }
        public void UpdateCanvas(CanvasColor[,] canvasData)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                try
                {
                    for (int y = 0; y < canvasData.GetLength(1); y++)
                    {
                        for (int x = 0; x < canvasData.GetLength(0); x++)
                        {
                            var cell = GetCell(x, y);
                            if (cell != null && _colorMap.TryGetValue(canvasData[x, y], out var brush))
                            {
                                cell.Background = brush;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage($"Error updating canvas: {ex}");
                }
            });
        }

        public async void ShowMessage(string message)
        {
            await MessageBoxManager.GetMessageBoxStandard("", $"Error: {message}").ShowWindowAsync();
        }
        
        // Manejador para redimensionar
        private async void OnResizeClick(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(SizeInput.Text, out int size) && size > 0 &&
                int.TryParse(PixelSizeInput.Text, out int pixelSize) && pixelSize > 0)
            {
                DefaultSize = size;
                _cellSize = pixelSize;
                _canvasService.ResizeCanvas(size);
                InitializeCanvas();
            }
            else
            {
                ShowMessage("Por favor ingrese valores válidos (números enteros positivos)");
            }
        }

        private async void OnGridClick(object sender, RoutedEventArgs e)
        {
            if (_thickness == 1) _thickness = 0.5;
            else _thickness = 1;
        }

        // Manejador para ejecutar código
        private async void OnRunClick(object sender, RoutedEventArgs e)
        {
            _canvasService.ExecuteCode(CodeEditor.Text);
        }

        // Manejador para cargar archivo
        private async void OnLoadClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filters.Add(new FileDialogFilter { Name = "Archivos PW", Extensions = { "pw" } });
            dialog.AllowMultiple = false;

            var result = await dialog.ShowAsync(this);
            if (result != null && result.Length > 0)
            {
                try
                {
                    CodeEditor.Text = await File.ReadAllTextAsync(result[0]);
                }
                catch (Exception ex)
                {
                    ShowMessage($"Error al cargar archivo: {ex.Message}");
                }
            }
        }

        // Manejador para guardar archivo
        private async void OnSaveClick(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            dialog.Filters.Add(new FileDialogFilter { Name = "Archivos PW", Extensions = { "pw" } });
            dialog.DefaultExtension = "pw";

            var result = await dialog.ShowAsync(this);
            if (result != null)
            {
                try
                {
                    await File.WriteAllTextAsync(result, CodeEditor.Text);
                    ShowMessage("Archivo guardado exitosamente");
                }
                catch (Exception ex)
                {
                    ShowMessage($"Error al guardar archivo: {ex.Message}");
                }
            }
        }
        private Border GetCell(int x, int y)
        {
            foreach (var child in CanvasGrid.Children)
            {
                if (child is Border border &&
                    Grid.GetRow(border) == y &&
                    Grid.GetColumn(border) == x)
                {
                    return border;
                }
            }
            return null;
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
                                Width = _cellSize,
                                Height = _cellSize,
                                Background = Brushes.White,
                                BorderBrush = Brushes.LightGray,
                                BorderThickness = new Thickness(_thickness),
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
                            FontSize = _cellSize/2,
                            Foreground = Brushes.Black,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            Width = _cellSize,
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
                            FontSize = _cellSize/2,
                            Foreground = Brushes.Black,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            Height = _cellSize,
                            Margin = new Thickness(0, 0, 0, 0.5) // Ajuste fino para alineación
                        };
    
                        Grid.SetRow(coordText, y);
                        YCoordinatesGrid.Children.Add(coordText);
                    }
        
                    // Ajustar tamaño del contenedor
                    CanvasGrid.Width = DefaultSize * _cellSize;
                    CanvasGrid.Height = DefaultSize * _cellSize;
                    XCoordinatesGrid.Width = DefaultSize * _cellSize;
                    YCoordinatesGrid.Height = DefaultSize * _cellSize;
                    
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