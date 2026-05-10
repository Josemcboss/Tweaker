using System;
using System.Diagnostics;
using System.ServiceProcess;
using Microsoft.Win32;

namespace Tweaker.Utilities
{
    /// <summary>
    /// ─
    /// MICROSOFT STORE REPAIR UTILITY
    /// ─
    /// 
    /// PROBLEMA: Error 0x80004002 en Microsoft Store
    /// CAUSA: Servicios crÃ­ticos deshabilitados por optimizaciones de gaming
    /// 
    /// SERVICIOS CRÃTICOS PARA MICROSOFT STORE:
    /// ─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”
    /// 1. Windows Update (wuauserv)
    /// 2. Background Intelligent Transfer Service (BITS)
    /// 3. Cryptographic Services (CryptSvc)
    /// 4. Windows Store Install Service (WSService)
    /// 5. Microsoft Store Install Service (InstallService)
    /// 6. State Repository Service (StateRepository)
    /// 7. Storage Service (StorSvc)
    /// 8. ClipSVC (Client License Service)
    /// 9. AppXSvc (AppX Deployment Service)
    /// 10. WSAppX (Windows Store Service)
    /// 
    /// VERSIÃ“N: 2.4.1
    /// FECHA: 2024
    /// ─
    /// </summary>
    public static class MicrosoftStoreRepair
    {
        /// <summary>
        /// Servicios crÃ­ticos para Microsoft Store
        /// Si alguno estÃ¡ deshabilitado, Store mostrarÃ¡ error 0x80004002
        /// </summary>
        private static readonly (string Name, string Display)[] StoreServices = new[]
        {
            ("wuauserv", "Windows Update"),
            ("BITS", "Background Intelligent Transfer Service"),
            ("CryptSvc", "Cryptographic Services"),
            ("WSService", "Windows Store Install Service"),
            ("InstallService", "Microsoft Store Install Service"),
            ("StateRepository", "State Repository Service"),
            ("StorSvc", "Storage Service"),
            ("ClipSVC", "Client License Service"),
            ("AppXSvc", "AppX Deployment Service"),
            ("wsappx", "Windows Store Service")
        };

        /// <summary>
        /// Repara Microsoft Store habilitando todos los servicios crÃ­ticos
        /// </summary>
        public static bool RepairStore()
        {
            Debug.WriteLine("─");
            Debug.WriteLine("ðŸ› ï¸  MICROSOFT STORE REPAIR - FIX ERROR 0x80004002");
            Debug.WriteLine("─");
            Debug.WriteLine("");

            int successCount = 0;
            int totalServices = StoreServices.Length;

            foreach (var (serviceName, displayName) in StoreServices)
            {
                if (RestoreStoreService(serviceName, displayName))
                {
                    successCount++;
                }
            }

            Debug.WriteLine("");
            Debug.WriteLine("─");
            Debug.WriteLine($"✅ Servicios restaurados: {successCount}/{totalServices}");
            Debug.WriteLine("─");
            Debug.WriteLine("");
            Debug.WriteLine("ðŸ“‹ SIGUIENTES PASOS:");
            Debug.WriteLine("   1. ✅ Reinicia Windows");
            Debug.WriteLine("   2. ✅ Abre Microsoft Store");
            Debug.WriteLine("   3. ✅ El error 0x80004002 deberÃ­a estar resuelto");
            Debug.WriteLine("");
            Debug.WriteLine("⚠️  SI EL PROBLEMA PERSISTE:");
            Debug.WriteLine("   • Ejecuta: wsreset.exe (limpia cache de Store)");
            Debug.WriteLine("   • Ejecuta: sfc /scannow (repara archivos del sistema)");
            Debug.WriteLine("   • Verifica Windows Update estÃ© activo");
            Debug.WriteLine("─");

            return successCount == totalServices;
        }

        /// <summary>
        /// Restaura un servicio especÃ­fico de Microsoft Store
        /// </summary>
        private static bool RestoreStoreService(string serviceName, string displayName)
        {
            try
            {
                Debug.WriteLine($"─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”");
                Debug.WriteLine($"ðŸ”§ Restaurando: {displayName} ({serviceName})");
                Debug.WriteLine($"─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”");

                // 1. Verificar si el servicio existe
                if (!ServiceExists(serviceName))
                {
                    Debug.WriteLine($"   ⚠️  Servicio no existe en este sistema");
                    Debug.WriteLine($"   ─„¹ï¸  Esto es normal para algunos servicios en ciertas versiones de Windows");
                    return true; // No es un error
                }

                // 2. Restaurar configuraciÃ³n en registro
                bool registryRestored = RestoreServiceRegistry(serviceName);
                
                // 3. Intentar iniciar el servicio
                bool serviceStarted = StartServiceSafe(serviceName);

                bool success = registryRestored && serviceStarted;
                
                if (success)
                {
                    Debug.WriteLine($"   ✅ {displayName} ─†’ RESTAURADO Y FUNCIONANDO");
                }
                else if (registryRestored)
                {
                    Debug.WriteLine($"   ⚠️  {displayName} ─†’ Configurado (requiere reinicio)");
                }
                else
                {
                    Debug.WriteLine($"   ❌ {displayName} ─†’ ERROR al restaurar");
                }

                return success || registryRestored;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"   ❌ ERROR: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Verifica si un servicio existe en el sistema
        /// </summary>
        private static bool ServiceExists(string serviceName)
        {
            try
            {
                using var sc = new ServiceController(serviceName);
                _ = sc.Status; // Forzar consulta
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Restaura la configuraciÃ³n de registro del servicio
        /// </summary>
        private static bool RestoreServiceRegistry(string serviceName)
        {
            try
            {
                string registryPath = $@"SYSTEM\CurrentControlSet\Services\{serviceName}";
                
                using var key = Registry.LocalMachine.OpenSubKey(registryPath, writable: true);
                if (key == null)
                {
                    Debug.WriteLine($"   ⚠️  Clave de registro no encontrada");
                    return false;
                }

                // Leer valor actual
                int currentStart = Convert.ToInt32(key.GetValue("Start", -1));
                
                if (currentStart == 4) // Disabled
                {
                    // Start = 3 (Manual) o 2 (Automatic) dependiendo del servicio
                    int newStart = GetRecommendedStartType(serviceName);
                    key.SetValue("Start", newStart, RegistryValueKind.DWord);
                    
                    string startTypeName = newStart == 2 ? "Automatic" : "Manual";
                    Debug.WriteLine($"   ✅ StartType: Disabled ─†’ {startTypeName}");
                    return true;
                }
                else if (currentStart == 2 || currentStart == 3)
                {
                    Debug.WriteLine($"   ─„¹ï¸  Ya estÃ¡ habilitado (StartType = {currentStart})");
                    return true;
                }
                else
                {
                    Debug.WriteLine($"   ⚠️  StartType actual: {currentStart}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"   ❌ Error en registro: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Intenta iniciar un servicio de forma segura
        /// </summary>
        private static bool StartServiceSafe(string serviceName)
        {
            try
            {
                using var sc = new ServiceController(serviceName);
                
                // Verificar estado actual
                sc.Refresh();
                var currentStatus = sc.Status;
                
                if (currentStatus == ServiceControllerStatus.Running)
                {
                    Debug.WriteLine($"   ─„¹ï¸  Servicio ya estÃ¡ en ejecuciÃ³n");
                    return true;
                }

                if (currentStatus == ServiceControllerStatus.Stopped)
                {
                    Debug.WriteLine($"   ─³ Iniciando servicio...");
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                    Debug.WriteLine($"   ✅ Servicio iniciado correctamente");
                    return true;
                }

                Debug.WriteLine($"   ⚠️  Estado actual: {currentStatus}");
                return false;
            }
            catch (InvalidOperationException)
            {
                Debug.WriteLine($"   ⚠️  No se puede iniciar (puede requerir dependencias)");
                return false;
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                Debug.WriteLine($"   ⚠️  Error Win32: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"   ❌ Error al iniciar: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Obtiene el tipo de inicio recomendado para cada servicio
        /// </summary>
        private static int GetRecommendedStartType(string serviceName)
        {
            // Servicios que deben ser Automatic (2)
            var automaticServices = new[] 
            { 
                "wuauserv", 
                "CryptSvc", 
                "StateRepository", 
                "StorSvc" 
            };

            return Array.Exists(automaticServices, s => 
                s.Equals(serviceName, StringComparison.OrdinalIgnoreCase)) ? 2 : 3;
        }

        /// <summary>
        /// Ejecuta wsreset.exe para limpiar cache de Microsoft Store
        /// </summary>
        public static bool ResetStoreCache()
        {
            try
            {
                Debug.WriteLine("─");
                Debug.WriteLine("ðŸ”„ LIMPIANDO CACHE DE MICROSOFT STORE (wsreset.exe)");
                Debug.WriteLine("─");

                var psi = new ProcessStartInfo
                {
                    FileName = "wsreset.exe",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(psi);
                if (process == null)
                {
                    Debug.WriteLine("❌ No se pudo iniciar wsreset.exe");
                    return false;
                }

                Debug.WriteLine("─³ Esperando a que wsreset.exe complete...");
                process.WaitForExit(30000); // 30 segundos max

                if (process.ExitCode == 0)
                {
                    Debug.WriteLine("✅ Cache de Store limpiado correctamente");
                    return true;
                }
                else
                {
                    Debug.WriteLine($"⚠️  wsreset.exe finalizÃ³ con cÃ³digo: {process.ExitCode}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error ejecutando wsreset.exe: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// DiagnÃ³stico completo de Microsoft Store
        /// </summary>
        public static void DiagnoseStore()
        {
            Debug.WriteLine("─");
            Debug.WriteLine("\U0001F50D DIAGNÃ“STICO DE MICROSOFT STORE");
            Debug.WriteLine("─");
            Debug.WriteLine("");

            foreach (var (serviceName, displayName) in StoreServices)
            {
                DiagnoseService(serviceName, displayName);
            }

            Debug.WriteLine("─");
        }

        private static void DiagnoseService(string serviceName, string displayName)
        {
            try
            {
                Debug.WriteLine($"─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”─”");
                Debug.WriteLine($"ðŸ“‹ {displayName} ({serviceName})");

                if (!ServiceExists(serviceName))
                {
                    Debug.WriteLine("   ⚠️  NO EXISTE en este sistema");
                    return;
                }

                using var sc = new ServiceController(serviceName);
                sc.Refresh();

                Debug.WriteLine($"   Estado: {GetStatusIcon(sc.Status)} {sc.Status}");
                Debug.WriteLine($"   Tipo de inicio: {GetStartTypeFromRegistry(serviceName)}");

                if (sc.Status != ServiceControllerStatus.Running)
                {
                    Debug.WriteLine("   ⚠️  PROBLEMA DETECTADO: Servicio no estÃ¡ en ejecuciÃ³n");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"   ❌ ERROR: {ex.Message}");
            }
        }

        private static string GetStatusIcon(ServiceControllerStatus status)
        {
            return status switch
            {
                ServiceControllerStatus.Running => "✅",
                ServiceControllerStatus.Stopped => "❌",
                ServiceControllerStatus.Paused => "─¸ï¸",
                ServiceControllerStatus.StartPending => "─³",
                ServiceControllerStatus.StopPending => "─³",
                _ => "⚠️"
            };
        }

        private static string GetStartTypeFromRegistry(string serviceName)
        {
            try
            {
                string registryPath = $@"SYSTEM\CurrentControlSet\Services\{serviceName}";
                using var key = Registry.LocalMachine.OpenSubKey(registryPath);
                if (key == null) return "Unknown";

                int startValue = Convert.ToInt32(key.GetValue("Start", -1));
                return startValue switch
                {
                    0 => "Boot",
                    1 => "System",
                    2 => "Automatic",
                    3 => "Manual",
                    4 => "❌ Disabled",
                    _ => "Unknown"
                };
            }
            catch
            {
                return "Error";
            }
        }
    }
}



