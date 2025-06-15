using Avalonia.Collections;
using ReactiveUI;
using System;

namespace PixelWallE.GUI.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        private int _canvasSize = 20;
        private string _canvasSizeStr = "20";
        
        public AvaloniaList<string> RowHeaders { get; } = new AvaloniaList<string>();
        public AvaloniaList<string> ColumnHeaders { get; } = new AvaloniaList<string>();
        
        public string CanvasSizeStr
        {
            get => _canvasSizeStr;
            set
            {
                // Validación y conversión
                if (int.TryParse(value, out int newSize) && newSize > 0)
                {
                    this.RaiseAndSetIfChanged(ref _canvasSizeStr, value);
                    CanvasSize = newSize; // Esto actualizará los headers automáticamente
                }
                else
                {
                    // Mantener el valor anterior si es inválido
                    this.RaiseAndSetIfChanged(ref _canvasSizeStr, _canvasSize.ToString());
                }
            }
        }
        
        public int CanvasSize
        {
            get => _canvasSize;
            private set
            {
                this.RaiseAndSetIfChanged(ref _canvasSize, value);
                UpdateHeaders();
            }
        }

        public MainWindowViewModel()
        {
            UpdateHeaders();
        }

        public void UpdateHeaders()
        {
            // Actualizar encabezados de filas
            RowHeaders.Clear();
            for (int i = 0; i < CanvasSize; i++)
                RowHeaders.Add(i.ToString());
            
            // Actualizar encabezados de columnas
            ColumnHeaders.Clear();
            for (int i = 0; i < CanvasSize; i++)
                ColumnHeaders.Add(i.ToString());
        }
    }
}