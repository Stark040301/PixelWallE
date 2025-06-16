using PixelWallE.Core;
using System;
using PixelWallE.WallE;

namespace PixelWallE.GUI.Services
{
    public class CanvasService
    {
        private readonly MainWindow _mainWindow;
        
        public CanvasService(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            MainWallE.SetCanvasSize(_mainWindow.DefaultSize); // Tamaño inicial
        }

        public void ExecuteCode(string code)
        {
            try
            {
                MainWallE.RunFromGUI(code);
                UpdateCanvasVisual();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        public void ResizeCanvas(int newSize)
        {
            MainWallE.SetCanvasSize(newSize);
            UpdateCanvasVisual();
        }

        private void UpdateCanvasVisual()
        {
            var canvasData = MainWallE.GetCanvas();
            _mainWindow.UpdateCanvas(canvasData);
        }

        private void ShowError(string message)
        {
            _mainWindow.ShowMessage(message);
        }
    }
}