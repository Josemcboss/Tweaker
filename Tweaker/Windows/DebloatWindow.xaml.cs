using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using Tweaker.ViewModels;

namespace Tweaker.Windows
{
    /// <summary>
    /// Ventana del Debloat Wizard
    /// </summary>
    public partial class DebloatWindow : Window
    {
        private readonly DebloatViewModel _viewModel;

        public DebloatWindow()
        {
            InitializeComponent();

            _viewModel = new DebloatViewModel();
            DataContext = _viewModel;
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                DragMove();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SelectRecommended_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SelectRecommended();
        }

        private async void ExecuteDebloat_Click(object sender, RoutedEventArgs e)
        {
            // Confirmación de seguridad
            var result = MessageBox.Show(
                "?? ADVERTENCIA IMPORTANTE ??\n\n" +
                "Estás a punto de eliminar aplicaciones del sistema.\n\n" +
                "Esta operación:\n" +
                "• NO se puede deshacer automáticamente\n" +
                "• Puede requerir reinstalación manual desde Microsoft Store\n" +
                "• Requiere permisos de Administrador\n\n" +
                $"Elementos seleccionados: {_viewModel.TotalItemsSelected}\n" +
                $"RAM estimada a liberar: ~{_viewModel.EstimatedRamFreed} MB\n" +
                $"Espacio en disco: ~{_viewModel.EstimatedDiskFreed} MB\n\n" +
                "¿Estás SEGURO de continuar?",
                "Confirmación Requerida",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
            {
                _viewModel.StatusMessage = "? Operación cancelada por el usuario";
                return;
            }

            // Verificar permisos de administrador
            if (!IsAdministrator())
            {
                MessageBox.Show(
                    "? PERMISOS INSUFICIENTES\n\n" +
                    "El Debloat Wizard requiere permisos de Administrador.\n\n" +
                    "Por favor:\n" +
                    "1. Cierra Ghost Optimizer\n" +
                    "2. Click derecho en el ejecutable\n" +
                    "3. Selecciona 'Ejecutar como Administrador'",
                    "Error de Permisos",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                _viewModel.StatusMessage = "? Requiere permisos de Administrador";
                return;
            }

            // Crear ventana de progreso
            var progressWindow = new ProgressWindow();
            progressWindow.Owner = this;
            progressWindow.Show();

            // Ejecutar en background
            await Task.Run(() =>
            {
                _viewModel.ExecuteDebloat((message) =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        progressWindow.AppendLog(message);
                    });
                });
            });

            progressWindow.Close();

            // Mostrar resumen final
            MessageBox.Show(
                $"? LIMPIEZA COMPLETADA\n\n" +
                $"Elementos procesados: {_viewModel.TotalItemsSelected}\n" +
                $"RAM liberada (estimado): ~{_viewModel.EstimatedRamFreed} MB\n" +
                $"Espacio en disco (estimado): ~{_viewModel.EstimatedDiskFreed} MB\n\n" +
                $"?? RECOMENDACIÓN:\n" +
                $"Reinicia Windows para liberar completamente los recursos.",
                "Limpieza Completada",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private bool IsAdministrator()
        {
            try
            {
                var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
                var principal = new System.Security.Principal.WindowsPrincipal(identity);
                return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Ventana de progreso simple
    /// </summary>
    public class ProgressWindow : Window
    {
        private System.Windows.Controls.TextBox _logTextBox;

        public ProgressWindow()
        {
            Title = "Ejecutando Limpieza...";
            Width = 600;
            Height = 400;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ResizeMode = ResizeMode.NoResize;

            _logTextBox = new System.Windows.Controls.TextBox
            {
                IsReadOnly = true,
                VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto,
                TextWrapping = TextWrapping.Wrap,
                FontFamily = new System.Windows.Media.FontFamily("Consolas"),
                FontSize = 11,
                Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(15, 15, 15)),
                Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White),
                Padding = new Thickness(10)
            };

            Content = _logTextBox;
        }

        public void AppendLog(string message)
        {
            _logTextBox.AppendText($"{DateTime.Now:HH:mm:ss} - {message}\n");
            _logTextBox.ScrollToEnd();
        }
    }
}
