using PixelWallE.Core;
using System;
using System.Collections.Generic;
using PixelWallE.WallE;

namespace PixelWallE.GUI.Services
{
    public class CanvasService
    {
        private readonly MainWindow _mainWindow;
        
        public CanvasService(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            MainWallE.SetCanvasSize(_mainWindow.DefaultSize);
        }

        public void ExecuteCode(string code)
        {
            try
            {
                MainWallE.RunFromGUI(code);
                if (MainWallE.HadError ||  MainWallE.HadRuntimeError)
                {
                    string allErrors = "";
                    foreach (string error in MainWallE.Errors)
                    {
                        allErrors = allErrors + error + Environment.NewLine;
                    }
                    
                    ShowError(allErrors);
                    MainWallE.Errors.Clear();
                }
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

        public void UpdateCanvasVisual()
        {
            var canvasData = MainWallE.GetCanvas();
            _mainWindow.UpdateCanvas(canvasData);
        }

        private void ShowError(string message)
        {
            _mainWindow.ShowMessage("Error",message);
        }
    }
}