using System;
using System.Diagnostics;
using System.Management;

namespace Tweaker.Utilities
{
    /// <summary>
    /// Utilidad para crear Puntos de Restauración del Sistema
    /// CRÍTICO antes de aplicar tweaks de registro
    /// </summary>
    public static class SystemRestore
    {
        private static DateTime _lastRestorePointCreated = DateTime.MinValue;
        private static readonly TimeSpan MinTimeBetweenRestorePoints = TimeSpan.FromHours(24);

        /// <summary>
        /// Crea un punto de restauración del sistema usando WMI o PowerShell
        /// </summary>
        /// <param name="description">Descripción del punto de restauración</param>
        /// <param name="force">Forzar creación ignorando límite de 24h</param>
        /// <returns>True si se creó exitosamente</returns>
        public static bool CreateRestorePoint(string description, bool force = true)
        {
            try
            {
                // Validar descripción
                if (string.IsNullOrWhiteSpace(description))
                {
                    description = $"GhostOptimizer Backup - {DateTime.Now:yyyy-MM-dd HH:mm}";
                }

                // Si force = true, desactivamos el límite de frecuencia en el Registro de Windows
                if (force)
                {
                    try
                    {
                        using var key = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\SystemRestore");
                        key?.SetValue("SystemRestorePointCreationFrequency", 0, Microsoft.Win32.RegistryValueKind.DWord);
                    }
                    catch { }
                }
                else if (DateTime.Now - _lastRestorePointCreated < MinTimeBetweenRestorePoints)
                {
                    Debug.WriteLine("⚠️ Ya se creó un punto de restauración en las últimas 24 horas");
                    return false;
                }

                Debug.WriteLine("──────────────────────────────────────────");
                Debug.WriteLine($"🛡️ Creando punto de restauración: {description}");
                Debug.WriteLine("──────────────────────────────────────────");

                // Método 1: Usar WMI (System.Management)
                bool success = CreateRestorePointWMI(description);

                if (success)
                {
                    _lastRestorePointCreated = DateTime.Now;
                    Debug.WriteLine("? Punto de restauración creado exitosamente");
                    Debug.WriteLine("──────────────────────────────────────────");
                    return true;
                }

                // Método 2: Fallback a PowerShell si WMI falla
                Debug.WriteLine("?? WMI falló, intentando con PowerShell...");
                success = CreateRestorePointPowerShell(description);

                if (success)
                {
                    _lastRestorePointCreated = DateTime.Now;
                    Debug.WriteLine("? Punto de restauración creado con PowerShell");
                    Debug.WriteLine("──────────────────────────────────────────");
                    return true;
                }

                Debug.WriteLine("? No se pudo crear punto de restauración");
                Debug.WriteLine("──────────────────────────────────────────");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error creando punto de restauración: {ex.Message}");
                Debug.WriteLine("──────────────────────────────────────────");
                return false;
            }
        }

        /// <summary>
        /// Crea punto de restauración usando WMI (System.Management)
        /// </summary>
        private static bool CreateRestorePointWMI(string description)
        {
            try
            {
                // Conectar a WMI namespace
                ManagementScope scope = new ManagementScope("\\\\localhost\\root\\default");
                scope.Connect();

                // Crear punto de restauración
                ManagementClass restorePoint = new ManagementClass(scope, new ManagementPath("SystemRestore"), null);

                // Parámetros del método CreateRestorePoint
                ManagementBaseObject inParams = restorePoint.GetMethodParameters("CreateRestorePoint");
                inParams["Description"] = description;
                inParams["RestorePointType"] = 12; // MODIFY_SETTINGS
                inParams["EventType"] = 100; // BEGIN_SYSTEM_CHANGE

                // Invocar método
                ManagementBaseObject outParams = restorePoint.InvokeMethod("CreateRestorePoint", inParams, null);

                // Verificar resultado
                if (outParams != null)
                {
                    uint returnValue = (uint)outParams["ReturnValue"];

                    if (returnValue == 0)
                    {
                        Debug.WriteLine("   ? WMI: Punto de restauración creado");
                        return true;
                    }
                    else
                    {
                        Debug.WriteLine($"   ?? WMI retornó código: {returnValue}");
                        return false;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"   ? Error WMI: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Crea punto de restauración usando PowerShell (fallback)
        /// </summary>
        private static bool CreateRestorePointPowerShell(string description)
        {
            try
            {
                string command = $"-NoProfile -ExecutionPolicy Bypass -Command " +
                                $"\"Checkpoint-Computer -Description '{description}' -RestorePointType 'MODIFY_SETTINGS'\"";

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = command,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    Verb = "runas" // Requiere admin
                };

                using (Process process = Process.Start(psi))
                {
                    if (process == null)
                        return false;

                    process.WaitForExit(30000); // Esperar máximo 30 segundos

                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    if (process.ExitCode == 0)
                    {
                        Debug.WriteLine("   ? PowerShell: Comando ejecutado exitosamente");
                        return true;
                    }
                    else
                    {
                        Debug.WriteLine($"   ?? PowerShell exit code: {process.ExitCode}");
                        if (!string.IsNullOrEmpty(error))
                        {
                            Debug.WriteLine($"   Error: {error}");
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"   ? Error PowerShell: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Verifica si System Restore está habilitado
        /// </summary>
        public static bool IsSystemRestoreEnabled()
        {
            try
            {
                ManagementScope scope = new ManagementScope("\\\\localhost\\root\\default");
                scope.Connect();

                ObjectQuery query = new ObjectQuery("SELECT * FROM SystemRestore");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

                // Si podemos ejecutar la query, System Restore está habilitado
                foreach (ManagementObject obj in searcher.Get())
                {
                    return true;
                }

                return false;
            }
            catch
            {
                // Si hay error, asumir que no está habilitado
                return false;
            }
        }

        /// <summary>
        /// Obtiene el tiempo desde el último punto de restauración creado
        /// </summary>
        public static TimeSpan TimeSinceLastRestorePoint()
        {
            return DateTime.Now - _lastRestorePointCreated;
        }

        /// <summary>
        /// Verifica si se puede crear un nuevo punto de restauración
        /// </summary>
        public static bool CanCreateRestorePoint()
        {
            return DateTime.Now - _lastRestorePointCreated >= MinTimeBetweenRestorePoints;
        }

        /// <summary>
        /// Muestra prompt al usuario para crear punto de restauración
        /// </summary>
        public static void PromptCreateRestorePoint()
        {
            try
            {
                // Intentar crear punto de restauración automáticamente
                bool success = CreateRestorePoint("Ghost Optimizer - Backup Automático");

                if (!success)
                {
                    Debug.WriteLine("?? No se pudo crear punto de restauración automáticamente");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error en PromptCreateRestorePoint: {ex.Message}");
            }
        }
    }
}
