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
            SizeInput.TextChanged += (s, e) => UpdateCanvasDimensions();
            PixelSizeInput.TextChanged += (s, e) => UpdateCanvasDimensions();
            _canvasService = new CanvasService(this);
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
                            PixelCanvasControl.SetPixel(x, y, canvasData[x, y]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage("Error",$"Error updating canvas: {ex}");
                }
            });
        }
        private void UpdateCanvasDimensions()
        {
            if (int.TryParse(SizeInput.Text, out int gridSize) && gridSize > 0 &&
                int.TryParse(PixelSizeInput.Text, out int pixelSize) && pixelSize > 0)
            {
                PixelCanvasControl.Columns = gridSize;
                PixelCanvasControl.Rows = gridSize;
                PixelCanvasControl.CellSize = pixelSize;
        
                // Actualiza tamaño directamente
                PixelCanvasControl.Width = gridSize * pixelSize;
                PixelCanvasControl.Height = gridSize * pixelSize;
            }
        }
        public async void ShowMessage(string title, string message)
        {
            await MessageBoxManager.GetMessageBoxStandard(title, message).ShowWindowAsync();
        }
        
        private async void OnResizeClick(object sender, RoutedEventArgs e)
        {
            // Validación de entrada
            if (!int.TryParse(SizeInput.Text, out int size) || size <= 0 || size > 1024 ||
                !int.TryParse(PixelSizeInput.Text, out int pixelSize) || pixelSize <= 0 || pixelSize > 100)
            {
                ShowMessage("Error", "Por favor ingrese valores válidos\nTamaño de canvas válido: 1 - 1024\nTamaño de píxel válido: 1 - 100");
                return;
            }

            try
            {
                // Bloquea la UI durante la operación
                ResizeButton.IsEnabled = false;
        
                // Actualiza el control de canvas
                PixelCanvasControl.Columns = size;
                PixelCanvasControl.Rows = size;
                PixelCanvasControl.CellSize = pixelSize;
        
                // Redimensiona el canvas en el servicio
                await Task.Run(() => _canvasService.ResizeCanvas(size));
        
                // Fuerza actualización visual
                PixelCanvasControl.InvalidateVisual();
            }
            catch (Exception ex)
            {
                ShowMessage("Error", $"No se pudo redimensionar: {ex.Message}");
            }
            finally
            {
                ResizeButton.IsEnabled = true;
            }
        }
        private void OnGridClick(object sender, RoutedEventArgs e)
        {
            PixelCanvasControl.ShowGrid = !PixelCanvasControl.ShowGrid;
    
            // Actualiza el texto del botón
            GridButton.Content = PixelCanvasControl.ShowGrid ? "Cuadrículas: ON" : "Cuadrículas: OFF";
        }
        private async void OnRunClick(object sender, RoutedEventArgs e)
        {
            _canvasService.ExecuteCode(CodeEditor.Text);
        }
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
                    ShowMessage("Error",$"Error al cargar archivo: {ex.Message}");
                }
            }
        }
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
                    ShowMessage("Información","Archivo guardado exitosamente");
                }
                catch (Exception ex)
                {
                    ShowMessage("Error",$"Error al guardar archivo: {ex.Message}");
                }
            }
        }
    }
    
}