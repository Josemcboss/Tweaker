using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Tweaker.Models;

namespace Tweaker.Utilities
{
    /// <summary>
    /// Verifica si hay actualizaciones disponibles desde GitHub Releases
    /// </summary>
    public static class UpdateChecker
    {
        private static readonly string UpdateCheckUrl = "https://raw.githubusercontent.com/Josemcboss/Tweaker/master/Release/update.json";
        private static readonly HttpClient _httpClient = new HttpClient();

        static UpdateChecker()
        {
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "GhostOptimizer-UpdateChecker");
        }

        /// <summary>
        /// Verifica si hay una actualización disponible
        /// </summary>
        /// <returns>UpdateInfo si hay actualización, null si no hay o hay error</returns>
        public static async Task<UpdateInfo> CheckForUpdatesAsync()
        {
            try
            {
                Debug.WriteLine("?? Verificando actualizaciones...");
                Debug.WriteLine($"   Versión actual: {VersionHelper.CurrentVersion}");
                Debug.WriteLine($"   URL: {UpdateCheckUrl}");

                // Descargar archivo de información de actualización
                var response = await _httpClient.GetAsync(UpdateCheckUrl);
                
                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"   ? Error HTTP: {response.StatusCode}");
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"   ?? JSON recibido: {json.Substring(0, Math.Min(200, json.Length))}...");

                // Parsear JSON
                var updateInfo = JsonSerializer.Deserialize<UpdateInfo>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (updateInfo == null)
                {
                    Debug.WriteLine("   ? Error parseando JSON");
                    return null;
                }

                Debug.WriteLine($"   ?? Versión remota: {updateInfo.Version}");

                // Verificar si es una versión más nueva
                if (!VersionHelper.IsNewerVersion(updateInfo.Version))
                {
                    Debug.WriteLine("   ? Ya estás en la última versión");
                    return null;
                }

                // Verificar versión mínima requerida
                if (!VersionHelper.MeetsMinimumVersion(updateInfo.MinimumVersion))
                {
                    Debug.WriteLine($"   ?? Versión actual no cumple mínimo requerido: {updateInfo.MinimumVersion}");
                    updateInfo.IsCritical = true; // Forzar actualización crítica
                }

                Debug.WriteLine("   ?? Nueva actualización disponible!");
                Debug.WriteLine($"      Versión: {updateInfo.Version}");
                Debug.WriteLine($"      Crítica: {updateInfo.IsCritical}");
                Debug.WriteLine($"      Tamaño: {FormatFileSize(updateInfo.FileSize)}");

                return updateInfo;
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"   ? Error de red: {ex.Message}");
                return null;
            }
            catch (TaskCanceledException)
            {
                Debug.WriteLine("   ?? Timeout verificando actualizaciones");
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"   ? Error inesperado: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Verifica actualizaciones de forma silenciosa (sin mostrar errores)
        /// </summary>
        public static async Task<UpdateInfo> CheckForUpdatesSilentAsync()
        {
            try
            {
                return await CheckForUpdatesAsync();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Formatea el tamaño de archivo para mostrar al usuario
        /// </summary>
        private static string FormatFileSize(long bytes)
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

        /// <summary>
        /// Verifica si la aplicación debería verificar actualizaciones
        /// (basado en última verificación)
        /// </summary>
        public static bool ShouldCheckForUpdates()
        {
            try
            {
                var lastCheckKey = "LastUpdateCheck";
                var lastCheckStr = Microsoft.Win32.Registry.CurrentUser
                    .OpenSubKey(@"Software\GhostOptimizer", false)
                    ?.GetValue(lastCheckKey) as string;

                if (string.IsNullOrEmpty(lastCheckStr))
                    return true;

                if (DateTime.TryParse(lastCheckStr, out DateTime lastCheck))
                {
                    // Verificar cada 24 horas
                    return DateTime.Now - lastCheck > TimeSpan.FromHours(24);
                }

                return true;
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// Registra que se verificó actualizaciones
        /// </summary>
        public static void RecordUpdateCheck()
        {
            try
            {
                using (var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"Software\GhostOptimizer"))
                {
                    key?.SetValue("LastUpdateCheck", DateTime.Now.ToString("O"));
                }
            }
            catch
            {
                // Ignorar errores
            }
        }
    }
}
