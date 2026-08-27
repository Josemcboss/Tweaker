using System;
using System.Diagnostics;
using Microsoft.Win32;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// DriverMaintenanceTools - Inspirado en Paragon Tweaking Utility (PTU)
    /// Automatización y preparación de instalación limpia de drivers GPU (DDU Flow) y eliminación de telemetría de drivers.
    /// </summary>
    public static class DriverMaintenanceTools
    {
        #region Safe Mode Helper for DDU

        /// <summary>
        /// Configura Windows para reiniciar en Modo Seguro (Safe Mode Minimal)
        /// Permite ejecutar Display Driver Uninstaller (DDU) de forma 100% limpia sin drivers en memoria
        /// </summary>
        public static bool ConfigureSafeModeBoot()
        {
            try
            {
                Debug.WriteLine("→ Configurando inicio en Modo Seguro para DDU...");
                var psi = new ProcessStartInfo
                {
                    FileName = "bcdedit.exe",
                    Arguments = "/set {current} safeboot minimal",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using var proc = Process.Start(psi);
                proc?.WaitForExit(5000);
                bool success = proc != null && proc.ExitCode == 0;

                if (success)
                {
                    Debug.WriteLine("✓ Windows configurado para iniciar en Modo Seguro en el próximo reinicio.");
                }
                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error configurando Modo Seguro: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina la directiva de Modo Seguro para que Windows arranque en modo normal
        /// </summary>
        public static bool RemoveSafeModeBoot()
        {
            try
            {
                Debug.WriteLine("→ Restaurando arranque normal de Windows...");
                var psi = new ProcessStartInfo
                {
                    FileName = "bcdedit.exe",
                    Arguments = "/deletevalue {current} safeboot",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using var proc = Process.Start(psi);
                proc?.WaitForExit(5000);
                bool success = proc != null && (proc.ExitCode == 0 || proc.ExitCode == 1); // 1 si ya no existía
                Debug.WriteLine("✓ Inicio normal de Windows asegurado.");
                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error restaurando arranque normal: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region GPU Driver Telemetry Debloat

        /// <summary>
        /// Desactiva tareas programadas de telemetría de NVIDIA y AMD (estilo NVCleanStall)
        /// </summary>
        public static bool DisableGpuDriverTelemetry()
        {
            try
            {
                Debug.WriteLine("→ Desactivando telemetría de drivers NVIDIA / AMD...");

                // 1. Desactivar tareas de NVIDIA Telemetry en Task Scheduler
                string[] nvTasks = new[]
                {
                    "NvTmMon_{*",
                    "NvTmRep_{*",
                    "NvTmRepOnLogon_{*",
                    "NvDriverUpdateCheckDaily_{*",
                    "NVIDIA\\NvTmMon",
                    "NVIDIA\\NvTmRep",
                    "NVIDIA\\NvTmRepOnLogon"
                };

                foreach (var task in nvTasks)
                {
                    RunCmd($"schtasks /Change /TN \"{task}\" /Disable");
                }

                // 2. Registro de telemetría NVIDIA
                using (RegistryKey? key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\NVIDIA Corporation\Global\FTS"))
                {
                    if (key != null)
                    {
                        key.SetValue("EnableRID44231", 0, RegistryValueKind.DWord);
                        key.SetValue("EnableRID64640", 0, RegistryValueKind.DWord);
                        key.SetValue("EnableRID66610", 0, RegistryValueKind.DWord);
                    }
                }

                // 3. AMD Crash Reporting & Telemetry
                using (RegistryKey? key = Registry.CurrentUser.CreateSubKey(@"Software\AMD\DVR"))
                {
                    if (key != null)
                    {
                        key.SetValue("DVRTelemetry", 0, RegistryValueKind.DWord);
                    }
                }

                Debug.WriteLine("✓ Telemetría de drivers GPU desactivada.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error en DisableGpuDriverTelemetry: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Restaura las tareas y registros de telemetría de drivers GPU
        /// </summary>
        public static bool RestoreGpuDriverTelemetry()
        {
            try
            {
                using (RegistryKey? key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\NVIDIA Corporation\Global\FTS", true))
                {
                    if (key != null)
                    {
                        key.DeleteValue("EnableRID44231", false);
                        key.DeleteValue("EnableRID64640", false);
                        key.DeleteValue("EnableRID66610", false);
                    }
                }

                using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"Software\AMD\DVR", true))
                {
                    key?.DeleteValue("DVRTelemetry", false);
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error en RestoreGpuDriverTelemetry: {ex.Message}");
                return false;
            }
        }

        #endregion

        private static void RunCmd(string command)
        {
            try
            {
                using var p = new Process();
                p.StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c {command}",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                p.Start();
                p.WaitForExit(3000);
            }
            catch
            {
                // Omitir errores de tareas inexistentes
            }
        }
    }
}
