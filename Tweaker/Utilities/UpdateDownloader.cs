using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;

using Tweaker.Models;

namespace Tweaker.Utilities
{
    /// <summary>
    /// Descarga actualizaciones y verifica su integridad
    /// </summary>
    public class UpdateDownloader
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly UpdateInfo _updateInfo;

        public event EventHandler<DownloadProgressEventArgs> DownloadProgressChanged;
        public event EventHandler<string> StatusChanged;

        static UpdateDownloader()
        {
            _httpClient.Timeout = TimeSpan.FromMinutes(10);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "GhostOptimizer-Downloader");
        }

        public UpdateDownloader(UpdateInfo updateInfo)
        {
            _updateInfo = updateInfo ?? throw new ArgumentNullException(nameof(updateInfo));
        }

        /// <summary>
        /// Descarga la actualización
        /// </summary>
        /// <returns>Ruta del archivo descargado</returns>
        public async Task<string> DownloadUpdateAsync()
        {
            try
            {
                RaiseStatusChanged("Preparando descarga...");
                Debug.WriteLine($"?? Descargando actualización desde: {_updateInfo.DownloadUrl}");

                // Crear directorio temporal
                var tempDir = Path.Combine(Path.GetTempPath(), "GhostOptimizer_Update");
                Directory.CreateDirectory(tempDir);

                var fileName = $"GhostOptimizer_v{_updateInfo.Version}_Setup.exe";
                var filePath = Path.Combine(tempDir, fileName);

                // Eliminar archivo anterior si existe
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                RaiseStatusChanged("Descargando actualización...");

                // Descargar con progress
                using (var response = await _httpClient.GetAsync(_updateInfo.DownloadUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();

                    var totalBytes = response.Content.Headers.ContentLength ?? _updateInfo.FileSize;
                    var buffer = new byte[8192];
                    var totalRead = 0L;

                    using (var contentStream = await response.Content.ReadAsStreamAsync())
                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, buffer.Length, true))
                    {
                        int read;
                        while ((read = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, read);
                            totalRead += read;

                            var progressPercentage = totalBytes > 0 ? (int)((totalRead * 100) / totalBytes) : 0;
                            RaiseDownloadProgress(progressPercentage, totalRead, totalBytes);
                        }
                    }
                }

                Debug.WriteLine($"   ? Descarga completada: {filePath}");
                RaiseStatusChanged("Verificando integridad del archivo...");

                // Verificar integridad con SHA256
                if (!string.IsNullOrEmpty(_updateInfo.Sha256Hash))
                {
                    if (!VerifyFileIntegrity(filePath, _updateInfo.Sha256Hash))
                    {
                        File.Delete(filePath);
                        throw new InvalidDataException("El archivo descargado está corrupto o ha sido modificado.");
                    }

                    Debug.WriteLine("   ? Integridad verificada");
                }

                RaiseStatusChanged("Descarga completada exitosamente");
                return filePath;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"   ? Error descargando actualización: {ex.Message}");
                RaiseStatusChanged($"Error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Verifica la integridad del archivo descargado usando SHA256
        /// </summary>
        private bool VerifyFileIntegrity(string filePath, string expectedHash)
        {
            try
            {
                using (var sha256 = SHA256.Create())
                using (var stream = File.OpenRead(filePath))
                {
                    var hash = sha256.ComputeHash(stream);
                    var hashString = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

                    Debug.WriteLine($"   ?? Hash calculado: {hashString}");
                    Debug.WriteLine($"   ?? Hash esperado:  {expectedHash.ToLowerInvariant()}");

                    return hashString == expectedHash.ToLowerInvariant();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"   ? Error verificando integridad: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Ejecuta el instalador descargado
        /// </summary>
        public static bool ExecuteInstaller(string installerPath)
        {
            try
            {
                if (!File.Exists(installerPath))
                {
                    Debug.WriteLine($"? Instalador no encontrado: {installerPath}");
                    return false;
                }

                Debug.WriteLine($"?? Ejecutando instalador: {installerPath}");

                var psi = new ProcessStartInfo
                {
                    FileName = installerPath,
                    UseShellExecute = true,
                    Verb = "runas" // Ejecutar como administrador
                };

                Process.Start(psi);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error ejecutando instalador: {ex.Message}");
                return false;
            }
        }

        private void RaiseDownloadProgress(int percentage, long bytesReceived, long totalBytes)
        {
            DownloadProgressChanged?.Invoke(this, new DownloadProgressEventArgs
            {
                ProgressPercentage = percentage,
                BytesReceived = bytesReceived,
                TotalBytes = totalBytes
            });
        }

        private void RaiseStatusChanged(string status)
        {
            StatusChanged?.Invoke(this, status);
        }
    }

    /// <summary>
    /// Evento de progreso de descarga
    /// </summary>
    public class DownloadProgressEventArgs : EventArgs
    {
        public int ProgressPercentage { get; set; }
        public long BytesReceived { get; set; }
        public long TotalBytes { get; set; }

        public string GetProgressString()
        {
            return $"{FormatBytes(BytesReceived)} / {FormatBytes(TotalBytes)} ({ProgressPercentage}%)";
        }

        private string FormatBytes(long bytes)
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
