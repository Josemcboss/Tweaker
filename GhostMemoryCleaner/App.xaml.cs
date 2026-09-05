using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace GhostMemoryCleaner
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Verificar si se llamó con argumento de línea de comandos (ej: --silent o --clean)
            if (e.Args.Any(a => a.Equals("--silent", StringComparison.OrdinalIgnoreCase) || 
                                a.Equals("-s", StringComparison.OrdinalIgnoreCase) ||
                                a.Equals("--clean", StringComparison.OrdinalIgnoreCase)))
            {
                var result = MemoryEngine.CleanAll();
                Debug.WriteLine($"[GhostMemoryCleaner] Silent cleanup: {result.mbCleaned} MB freed.");
                Shutdown();
                return;
            }

            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}
