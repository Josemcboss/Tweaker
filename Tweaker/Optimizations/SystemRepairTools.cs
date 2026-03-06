using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// Herramientas de reparación del sistema usando SFC y DISM
    /// Permite reparar archivos corruptos del sistema operativo Windows
    /// </summary>
    public static class SystemRepairTools
    {
        /// <summary>
        /// Ejecuta una reparación completa de archivos corruptos del sistema
        /// 
        /// Pasos que ejecuta:
        /// 1. DISM RestoreHealth - Repara la imagen de Windows desde Windows Update
        /// 2. SFC /scannow - Escanea y repara archivos de sistema protegidos
        /// 
        /// NOTA: Este proceso puede tardar varios minutos
        /// REQUIERE conexión a internet para DISM (o una imagen de Windows montada)
        /// </summary>
        /// <returns>True si ambas operaciones fueron exitosas, false en caso contrario</returns>
        public static async Task<bool> RunCorruptFileCheckerAsync()
        {
            try
            {
                Debug.WriteLine("═══════════════════════════════════════════════════════════");
                Debug.WriteLine("SYSTEM REPAIR TOOLS - Reparación de archivos del sistema");
                Debug.WriteLine("═══════════════════════════════════════════════════════════");

                // Paso 1: DISM RestoreHealth
                Debug.WriteLine("→ Paso 1/2: Ejecutando DISM /Online /Cleanup-Image /RestoreHealth...");
                Debug.WriteLine("   ⏳ Esto puede tardar varios minutos...");

                bool dismSuccess = await RunCommandAsync(
                    "dism.exe",
                    "/Online /Cleanup-Image /RestoreHealth",
                    "DISM RestoreHealth");

                if (dismSuccess)
                {
                    Debug.WriteLine("✓ DISM RestoreHealth completado exitosamente");
                }
                else
                {
                    Debug.WriteLine("⚠ DISM RestoreHealth falló o devolvió código de error");
                    Debug.WriteLine("   → Continuando con SFC de todas formas...");
                }

                // Paso 2: SFC /scannow
                Debug.WriteLine("");
                Debug.WriteLine("→ Paso 2/2: Ejecutando SFC /scannow...");
                Debug.WriteLine("   ⏳ Esto puede tardar varios minutos...");

                bool sfcSuccess = await RunCommandAsync(
                    "sfc.exe",
                    "/scannow",
                    "SFC Scannow");

                if (sfcSuccess)
                {
                    Debug.WriteLine("✓ SFC /scannow completado exitosamente");
                }
                else
                {
                    Debug.WriteLine("⚠ SFC /scannow falló o encontró archivos no reparables");
                }

                bool overallSuccess = dismSuccess && sfcSuccess;

                Debug.WriteLine("");
                Debug.WriteLine("═══════════════════════════════════════════════════════════");
                if (overallSuccess)
                {
                    Debug.WriteLine("✓ REPARACIÓN COMPLETA - Sistema revisado y reparado");
                }
                else
                {
                    Debug.WriteLine("⚠ REPARACIÓN PARCIAL - Algunos pasos fallaron");
                    Debug.WriteLine("   → Revisa los logs de eventos de Windows para más detalles");
                }
                Debug.WriteLine("═══════════════════════════════════════════════════════════");

                return overallSuccess;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error en RunCorruptFileCheckerAsync: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Ejecuta una reparación completa de archivos corruptos del sistema (versión síncrona)
        /// 
        /// ADVERTENCIA: Este método bloqueará el hilo actual durante toda la operación.
        /// Prefiere usar RunCorruptFileCheckerAsync() para no bloquear la UI.
        /// </summary>
        /// <returns>True si ambas operaciones fueron exitosas, false en caso contrario</returns>
        public static bool RunCorruptFileChecker()
        {
            try
            {
                return RunCorruptFileCheckerAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error en RunCorruptFileChecker: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Ejecuta únicamente DISM RestoreHealth para reparar la imagen del sistema
        /// </summary>
        /// <returns>True si la operación fue exitosa, false en caso contrario</returns>
        public static async Task<bool> RunDISMRestoreHealthAsync()
        {
            try
            {
                Debug.WriteLine("→ Ejecutando DISM /Online /Cleanup-Image /RestoreHealth...");
                Debug.WriteLine("   ⏳ Esto puede tardar varios minutos...");

                bool success = await RunCommandAsync(
                    "dism.exe",
                    "/Online /Cleanup-Image /RestoreHealth",
                    "DISM RestoreHealth");

                if (success)
                {
                    Debug.WriteLine("✓ DISM RestoreHealth completado exitosamente");
                }
                else
                {
                    Debug.WriteLine("⚠ DISM RestoreHealth falló o devolvió código de error");
                }

                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error en RunDISMRestoreHealthAsync: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Ejecuta únicamente SFC /scannow para escanear y reparar archivos de sistema
        /// </summary>
        /// <returns>True si la operación fue exitosa, false en caso contrario</returns>
        public static async Task<bool> RunSFCScanAsync()
        {
            try
            {
                Debug.WriteLine("→ Ejecutando SFC /scannow...");
                Debug.WriteLine("   ⏳ Esto puede tardar varios minutos...");

                bool success = await RunCommandAsync(
                    "sfc.exe",
                    "/scannow",
                    "SFC Scannow");

                if (success)
                {
                    Debug.WriteLine("✓ SFC /scannow completado exitosamente");
                }
                else
                {
                    Debug.WriteLine("⚠ SFC /scannow falló o encontró archivos no reparables");
                }

                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error en RunSFCScanAsync: {ex.Message}");
                return false;
            }
        }

        private static Task<bool> RunCommandAsync(string fileName, string arguments, string operationName)
        {
            return Task.Run(() =>
            {
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = fileName,
                        Arguments = arguments,
                        UseShellExecute = true,
                        Verb = "runas",
                        CreateNoWindow = true
                    };

                    using (Process process = Process.Start(psi))
                    {
                        process?.WaitForExit();
                        int exitCode = process?.ExitCode ?? -1;
                        Debug.WriteLine($"   → {operationName} finalizó con código: {exitCode}");
                        // SFC returns 0 for success, DISM may return non-zero for warnings
                        return exitCode == 0;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"✗ Error ejecutando {operationName}: {ex.Message}");
                    return false;
                }
            });
        }
    }
}
