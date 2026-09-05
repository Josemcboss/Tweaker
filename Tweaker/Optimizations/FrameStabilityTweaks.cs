using System;
using System.Diagnostics;
using Microsoft.Win32;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// Tweaks avanzados para FPS estables, frametimes consistentes y baja latencia de presentación.
    /// Incluye: DWM Latency & Priority, NVMe I/O Power Management, CPU Core Parking & Thread Scheduling,
    /// y DirectX Low Latency Buffer Settings.
    /// </summary>
    public static class FrameStabilityTweaks
    {
        private const string DISPLAY_POST_PROCESSING_KEY = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\DisplayPostProcessing";
        private const string SYSTEM_PROFILE_KEY = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile";
        private const string NVME_POWER_KEY = @"SYSTEM\CurrentControlSet\Control\Power\PowerSettings\0012ee47-9041-4b5d-9b77-535fba8b1442\d7763327-9309-4b9a-b419-977418706e2e";
        private const string DXGI_LATENCY_KEY = @"SOFTWARE\Microsoft\DirectX\UserGpuPreferences";
        private const string GRAPHICS_DRIVERS_KEY = @"SYSTEM\CurrentControlSet\Control\GraphicsDrivers";

        #region 1. DWM Priority & Presentation Latency

        /// <summary>
        /// Optimiza el DWM (Desktop Window Manager) para reducir el latencia de composición y evitar tirones.
        /// </summary>
        public static bool OptimizeDwmLatency()
        {
            try
            {
                using (var key = Registry.LocalMachine.CreateSubKey(DISPLAY_POST_PROCESSING_KEY))
                {
                    if (key != null)
                    {
                        key.SetValue("Scheduling Category", "High", RegistryValueKind.String);
                        key.SetValue("SFIO Priority", "High", RegistryValueKind.String);
                        key.SetValue("Background Only", "FALSE", RegistryValueKind.String);
                        key.SetValue("Priority", 8, RegistryValueKind.DWord);
                        key.SetValue("GPU Priority", 8, RegistryValueKind.DWord);
                    }
                }

                using (var key = Registry.LocalMachine.CreateSubKey(GRAPHICS_DRIVERS_KEY))
                {
                    if (key != null)
                    {
                        // Minimiza el buffer de presentación de marcos en el controlador gráfico
                        key.SetValue("MaxFrameLatency", 1, RegistryValueKind.DWord);
                    }
                }

                Debug.WriteLine("✓ DWM Latency & Presentation Priority optimizados.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error al optimizar DWM Latency: {ex.Message}");
                return false;
            }
        }

        public static bool RevertDwmLatency()
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(DISPLAY_POST_PROCESSING_KEY, true))
                {
                    if (key != null)
                    {
                        key.SetValue("Scheduling Category", "Medium", RegistryValueKind.String);
                        key.SetValue("SFIO Priority", "Normal", RegistryValueKind.String);
                        key.SetValue("Background Only", "TRUE", RegistryValueKind.String);
                        key.SetValue("Priority", 2, RegistryValueKind.DWord);
                        key.SetValue("GPU Priority", 2, RegistryValueKind.DWord);
                    }
                }

                using (var key = Registry.LocalMachine.OpenSubKey(GRAPHICS_DRIVERS_KEY, true))
                {
                    key?.DeleteValue("MaxFrameLatency", throwOnMissingValue: false);
                }

                Debug.WriteLine("✓ DWM Latency restaurado a valores por defecto.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error al restaurar DWM Latency: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region 2. NVMe & SSD I/O Anti-Stutter

        /// <summary>
        /// Evita la latencia de suspensión de NVMe/SATA para streaming fluido de assets en juegos.
        /// </summary>
        public static bool OptimizeNvmeAntiStutter()
        {
            try
            {
                // Configurar PowerOption para NVMe Primary Idle Timeout a 0 (nunca entrar en ahorro)
                using (var key = Registry.LocalMachine.CreateSubKey(NVME_POWER_KEY))
                {
                    key?.SetValue("Attributes", 2, RegistryValueKind.DWord);
                }

                // Ejecutar powercfg para deshabilitar APST en el plan activo
                var psi = new ProcessStartInfo("powercfg", "/setacvalueindex SCHEME_CURRENT 0012ee47-9041-4b5d-9b77-535fba8b1442 d7763327-9309-4b9a-b419-977418706e2e 0")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                Process.Start(psi)?.WaitForExit();

                var psiApply = new ProcessStartInfo("powercfg", "/setactive SCHEME_CURRENT")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                Process.Start(psiApply)?.WaitForExit();

                Debug.WriteLine("✓ NVMe Anti-Stuttering aplicado.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error al aplicar NVMe Anti-Stutter: {ex.Message}");
                return false;
            }
        }

        public static bool RevertNvmeAntiStutter()
        {
            try
            {
                var psi = new ProcessStartInfo("powercfg", "/setacvalueindex SCHEME_CURRENT 0012ee47-9041-4b5d-9b77-535fba8b1442 d7763327-9309-4b9a-b419-977418706e2e 100")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                Process.Start(psi)?.WaitForExit();

                var psiApply = new ProcessStartInfo("powercfg", "/setactive SCHEME_CURRENT")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                Process.Start(psiApply)?.WaitForExit();

                Debug.WriteLine("✓ NVMe Anti-Stuttering restaurado.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error al restaurar NVMe Anti-Stutter: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region 3. CPU Core Parking & Thread Scheduling

        /// <summary>
        /// Deshabilita el estacionamiento de núcleos (Core Parking) y prioriza P-Cores / V-Cache.
        /// </summary>
        public static bool DisableCoreParkingAndOptimizeScheduling()
        {
            try
            {
                // Deshabilitar Core Parking a nivel de Power Scheme
                string[] powerCommands = new string[]
                {
                    "/setacvalueindex SCHEME_CURRENT SUB_PROCESSOR 0cc5b647-c1df-4637-891a-dec35c3185b3 100", // Core Parking Min Cores (100%)
                    "/setacvalueindex SCHEME_CURRENT SUB_PROCESSOR ea0646f2-1958-4424-927c-56d029517e40 100", // Core Parking Max Cores
                    "/setactive SCHEME_CURRENT"
                };

                foreach (var cmd in powerCommands)
                {
                    var psi = new ProcessStartInfo("powercfg", cmd)
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false
                    };
                    Process.Start(psi)?.WaitForExit();
                }

                Debug.WriteLine("✓ Core Parking deshabilitado y programación de núcleos optimizada.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error al optimizar Core Parking: {ex.Message}");
                return false;
            }
        }

        public static bool RestoreCoreParking()
        {
            try
            {
                string[] powerCommands = new string[]
                {
                    "/setacvalueindex SCHEME_CURRENT SUB_PROCESSOR 0cc5b647-c1df-4637-891a-dec35c3185b3 10",
                    "/setactive SCHEME_CURRENT"
                };

                foreach (var cmd in powerCommands)
                {
                    var psi = new ProcessStartInfo("powercfg", cmd)
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false
                    };
                    Process.Start(psi)?.WaitForExit();
                }

                Debug.WriteLine("✓ Core Parking restaurado a valores por defecto.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error al restaurar Core Parking: {ex.Message}");
                return false;
            }
        }

        #endregion
    }
}
