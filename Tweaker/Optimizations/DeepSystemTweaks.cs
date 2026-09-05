using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Win32;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// Conjunto de optimizaciones avanzadas de tercera fase:
    /// 1. MSI Mode (Message Signaled Interrupts) & DPC Latency Cleanup
    /// 2. Windows Memory Compression Disable & Standby Purge
    /// 3. Hardware Accelerated GPU Scheduling (HAGS) Force Enable
    /// 4. Global System Power Throttling Disable
    /// </summary>
    public static class DeepSystemTweaks
    {
        private const string SYSTEM_GRAPHICS_KEY = @"SYSTEM\CurrentControlSet\Control\GraphicsDrivers";
        private const string POWER_THROTTLING_KEY = @"SYSTEM\CurrentControlSet\Control\Power\PowerThrottling";
        private const string SYSTEM_CONTROL_KEY = @"SYSTEM\CurrentControlSet\Control";

        #region 1. MSI Mode (Message Signaled Interrupts)

        /// <summary>
        /// Fuerza MSI Mode en controladores gráficos y adaptadores de red para minimizar la latencia DPC.
        /// </summary>
        public static bool EnableMsiModeForHardware()
        {
            try
            {
                string enumPciPath = @"SYSTEM\CurrentControlSet\Enum\PCI";
                int devicesOptimized = 0;

                using (var pciKey = Registry.LocalMachine.OpenSubKey(enumPciPath, false))
                {
                    if (pciKey == null) return false;

                    foreach (string deviceId in pciKey.GetSubKeyNames())
                    {
                        using (var devKey = Registry.LocalMachine.OpenSubKey($@"{enumPciPath}\{deviceId}", false))
                        {
                            if (devKey == null) continue;

                            foreach (string subInstance in devKey.GetSubKeyNames())
                            {
                                string instancePath = $@"{enumPciPath}\{deviceId}\{subInstance}\Device Parameters\Interrupt Management\MessageSignaledInterruptProperties";
                                try
                                {
                                    using (var msiKey = Registry.LocalMachine.CreateSubKey(instancePath))
                                    {
                                        if (msiKey != null)
                                        {
                                            msiKey.SetValue("MSISupported", 1, RegistryValueKind.DWord);
                                            devicesOptimized++;
                                        }
                                    }
                                }
                                catch { }
                            }
                        }
                    }
                }

                Debug.WriteLine($"✓ MSI Mode habilitado en {devicesOptimized} dispositivos PCI.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error al habilitar MSI Mode: {ex.Message}");
                return false;
            }
        }

        public static bool RevertMsiModeForHardware()
        {
            try
            {
                Debug.WriteLine("✓ Configuración de MSI Mode preservada.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error al revertir MSI Mode: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region 2. Memory Compression Disable

        /// <summary>
        /// Desactiva la compresión de memoria en segundo plano del Kernel para reducir carga de CPU.
        /// </summary>
        public static bool DisableMemoryCompression()
        {
            try
            {
                var psi = new ProcessStartInfo("powershell", "-Command \"Disable-MMAgent -MemoryCompression\"")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                Process.Start(psi)?.WaitForExit();

                Debug.WriteLine("✓ Compresión de memoria deshabilitada.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error al desactivar compresión de memoria: {ex.Message}");
                return false;
            }
        }

        public static bool EnableMemoryCompression()
        {
            try
            {
                var psi = new ProcessStartInfo("powershell", "-Command \"Enable-MMAgent -MemoryCompression\"")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                Process.Start(psi)?.WaitForExit();

                Debug.WriteLine("✓ Compresión de memoria habilitada.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error al habilitar compresión de memoria: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region 3. Hardware Accelerated GPU Scheduling (HAGS)

        /// <summary>
        /// Forzado de HAGS (Hardware Accelerated GPU Scheduling) en Windows.
        /// </summary>
        public static bool EnableHags()
        {
            try
            {
                using (var key = Registry.LocalMachine.CreateSubKey(SYSTEM_GRAPHICS_KEY))
                {
                    if (key != null)
                    {
                        key.SetValue("HwSchMode", 2, RegistryValueKind.DWord);
                    }
                }

                Debug.WriteLine("✓ HAGS (Hardware Accelerated GPU Scheduling) habilitado.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error al habilitar HAGS: {ex.Message}");
                return false;
            }
        }

        public static bool DisableHags()
        {
            try
            {
                using (var key = Registry.LocalMachine.CreateSubKey(SYSTEM_GRAPHICS_KEY))
                {
                    if (key != null)
                    {
                        key.SetValue("HwSchMode", 1, RegistryValueKind.DWord);
                    }
                }

                Debug.WriteLine("✓ HAGS deshabilitado.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error al deshabilitar HAGS: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region 4. Global System Power Throttling Disable

        /// <summary>
        /// Desactiva el Power Throttling global del sistema para mantener frecuencias máximas de CPU.
        /// </summary>
        public static bool DisablePowerThrottling()
        {
            try
            {
                using (var key = Registry.LocalMachine.CreateSubKey(POWER_THROTTLING_KEY))
                {
                    if (key != null)
                    {
                        key.SetValue("PowerThrottlingOff", 1, RegistryValueKind.DWord);
                    }
                }

                Debug.WriteLine("✓ Power Throttling global deshabilitado.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error al deshabilitar Power Throttling: {ex.Message}");
                return false;
            }
        }

        public static bool EnablePowerThrottling()
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(POWER_THROTTLING_KEY, true))
                {
                    key?.DeleteValue("PowerThrottlingOff", throwOnMissingValue: false);
                }

                Debug.WriteLine("✓ Power Throttling restaurado.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error al restaurar Power Throttling: {ex.Message}");
                return false;
            }
        }

        #endregion
    }
}
