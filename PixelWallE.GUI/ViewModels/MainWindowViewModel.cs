using System;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using System.IO;
using System.Threading.Tasks;
using PixelWallE;
using PixelWallE.WallE;

namespace PixelWallE.GUI.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        private string _code = string.Empty;
        public string Code
        {
            get => _code;
            set => this.RaiseAndSetIfChanged(ref _code, value);
        }
        private string _canvasSizeStr = "20";
        public string CanvasSizeStr
        {
            get => _canvasSizeStr;
            set => this.RaiseAndSetIfChanged(ref _canvasSizeStr, value);
        }

        public int CanvasSize => int.TryParse(CanvasSizeStr, out var n) ? n : 20;


        public ObservableCollection<Pixel> Pixels { get; } = new();

        public ReactiveCommand<Unit, Unit> RunCommand { get; }
        public ReactiveCommand<Unit, Unit> LoadCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> ResizeCommand { get; }

        public MainWindowViewModel()
        {
            RunCommand = ReactiveCommand.Create(ExecuteCode);
            LoadCommand = ReactiveCommand.CreateFromTask(LoadFileAsync);
            SaveCommand = ReactiveCommand.CreateFromTask(SaveFileAsync);
            ResizeCommand = ReactiveCommand.Create(() =>
            {
                MainWallE.SetCanvasSize(CanvasSize);
                Pixels.Clear();
            });
        }

        private void ExecuteCode()
        {
            // Redimensionar si lo deseas antes de ejecutar (opcional)
            MainWallE.SetCanvasSize(100); // O valor dinámico

            MainWallE.RunFromGUI(Code); // Ejecuta el código

            Pixels.Clear();

            var canvas = MainWallE.GetCanvas(); // Devuelve un arreglo 2D de colores
            Console.WriteLine($"Canvas size: {canvas.GetLength(0)} x {canvas.GetLength(1)}");
            for (int y = 0; y < canvas.GetLength(0); y++)
            {
                for (int x = 0; x < canvas.GetLength(1); x++)
                {
                    if (canvas[y, x] != CanvasColor.White) // Puedes ajustar la condición
                    {
                        Pixels.Add(new Pixel
                        {
                            X = x,
                            Y = y,
                            Color = canvas[y, x]
                        });
                    }
                }
            }
        }


        private async Task LoadFileAsync()
        {
            var dlg = new Avalonia.Controls.OpenFileDialog();
            dlg.Filters.Add(new Avalonia.Controls.FileDialogFilter() { Name = "PixelWallE files", Extensions = { "pw" } });
            var result = await dlg.ShowAsync(Avalonia.Application.Current.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null);
            if (result != null && result.Length > 0)
            {
                var path = result[0];
                Code = await File.ReadAllTextAsync(path);
            }
        }

        private async Task SaveFileAsync()
        {
            var dlg = new Avalonia.Controls.SaveFileDialog();
            dlg.Filters.Add(new Avalonia.Controls.FileDialogFilter() { Name = "PixelWallE files", Extensions = { "pw" } });
            var path = await dlg.ShowAsync(Avalonia.Application.Current.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null);
            if (path != null)
            {
                await File.WriteAllTextAsync(path, Code);
            }
        }
    }

    public class Pixel
    {
        public CanvasColor Color { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}
