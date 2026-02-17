using System;
using System.Diagnostics;
using System.Windows;
using Tweaker.Models;
using Tweaker.Utilities;

namespace Tweaker.Windows
{
    public partial class UpdateWindow : Window
    {
        private readonly UpdateInfo _updateInfo;
        private UpdateDownloader _downloader;
        private bool _isDownloading = false;

        public UpdateWindow(UpdateInfo updateInfo)
        {
            InitializeComponent();
            _updateInfo = updateInfo ?? throw new ArgumentNullException(nameof(updateInfo));
            
            LoadUpdateInfo();
        }

        private void LoadUpdateInfo()
        {
            // Título y versión
            TxtVersionInfo.Text = $"Versión {_updateInfo.Version} disponible";

            // Changelog
            TxtChangelog.Text = string.IsNullOrEmpty(_updateInfo.Changelog) 
                ? "No hay changelog disponible." 
                : _updateInfo.Changelog;

            // Release Notes
            if (!string.IsNullOrEmpty(_updateInfo.ReleaseNotes))
            {
                BorderReleaseNotes.Visibility = Visibility.Visible;
                TxtReleaseNotes.Text = _updateInfo.ReleaseNotes;
            }

            // Critical warning
            if (_updateInfo.IsCritical)
            {
                BorderCriticalWarning.Visibility = Visibility.Visible;
                BtnLater.IsEnabled = false; // No permitir posponer actualizaciones críticas
            }

            // File size
            TxtFileSize.Text = FormatFileSize(_updateInfo.FileSize);

            // Release date
            TxtReleaseDate.Text = _updateInfo.ReleaseDate.ToString("dd/MM/yyyy");
        }

        private async void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (_isDownloading)
                return;

            try
            {
                _isDownloading = true;

                // Deshabilitar botones
                BtnUpdate.IsEnabled = false;
                BtnLater.IsEnabled = false;

                // Mostrar progress bar
                PanelProgress.Visibility = Visibility.Visible;

                // Crear downloader
                _downloader = new UpdateDownloader(_updateInfo);
                _downloader.DownloadProgressChanged += Downloader_ProgressChanged;
                _downloader.StatusChanged += Downloader_StatusChanged;

                Debug.WriteLine("?? Iniciando descarga de actualización...");

                // Descargar actualización
                var installerPath = await _downloader.DownloadUpdateAsync();

                Debug.WriteLine($"? Descarga completada: {installerPath}");

                // Preguntar si desea instalar ahora
                var result = MessageBox.Show(
                    "? Actualización descargada exitosamente.\n\n" +
                    "¿Deseas instalar la actualización ahora?\n\n" +
                    "La aplicación se cerrará y se ejecutará el instalador.",
                    "Actualización Lista",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Ejecutar instalador
                    if (UpdateDownloader.ExecuteInstaller(installerPath))
                    {
                        // Cerrar la aplicación
                        Application.Current.Shutdown();
                    }
                    else
                    {
                        MessageBox.Show(
                            "Error al ejecutar el instalador.\n\n" +
                            $"Puedes ejecutarlo manualmente desde:\n{installerPath}",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show(
                        $"El instalador ha sido descargado en:\n{installerPath}\n\n" +
                        "Puedes ejecutarlo más tarde para actualizar.",
                        "Actualización Descargada",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error durante la actualización: {ex.Message}");
                
                MessageBox.Show(
                    $"Error al descargar la actualización:\n\n{ex.Message}\n\n" +
                    "Por favor, intenta más tarde o descarga la actualización manualmente desde GitHub.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                // Rehabilitar botones
                BtnUpdate.IsEnabled = true;
                BtnLater.IsEnabled = !_updateInfo.IsCritical;
                PanelProgress.Visibility = Visibility.Collapsed;
                _isDownloading = false;
            }
        }

        private void Downloader_ProgressChanged(object sender, DownloadProgressEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                ProgressDownload.Value = e.ProgressPercentage;
                TxtProgressDetails.Text = e.GetProgressString();
            });
        }

        private void Downloader_StatusChanged(object sender, string status)
        {
            Dispatcher.Invoke(() =>
            {
                TxtProgressStatus.Text = status;
            });
        }

        private void BtnLater_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            if (_updateInfo.IsCritical)
            {
                var result = MessageBox.Show(
                    "Esta es una actualización crítica que corrige problemas importantes.\n\n" +
                    "¿Estás seguro de que deseas cerrar sin actualizar?",
                    "Actualización Crítica",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.No)
                    return;
            }

            Close();
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            return $"{len:0.##} {sizes[order]}";
        }
    }
}
