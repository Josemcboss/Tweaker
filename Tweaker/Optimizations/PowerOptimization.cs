using System;
using System.Diagnostics;
using System.Management;

using Microsoft.Win32;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// Power Optimization Wrapper - Compatibilidad con presets
    /// </summary>
    public static class PowerOptimization
    {
        /// <summary>
        /// Verifica si Power Throttling está habilitado
        /// </summary>
        public static bool IsPowerThrottlingEnabled()
        {
            try
            {
                const string powerKey = @"SYSTEM\CurrentControlSet\Control\Power\PowerThrottling";

                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(powerKey, false))
                {
                    if (key != null)
                    {
                        object value = key.GetValue("PowerThrottlingOff");
                        return value == null || value.ToString() != "1";
                    }
                }
                return true; // Por defecto está habilitado
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// Verifica si la hibernación está habilitada
        /// </summary>
        public static bool IsHibernationEnabled()
        {
            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo = new ProcessStartInfo
                    {
                        FileName = "powercfg",
                        Arguments = "/a",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    };

                    process.Start();
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    // Si el output contiene "hibernar", está habilitado
                    return output.ToLower().Contains("hibernar") || output.ToLower().Contains("hibernate");
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Deshabilita Power Throttling
        /// </summary>
        public static bool DisablePowerThrottling()
        {
            try
            {
                const string powerKey = @"SYSTEM\CurrentControlSet\Control\Power\PowerThrottling";

                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(powerKey))
                {
                    if (key != null)
                    {
                        key.SetValue("PowerThrottlingOff", 1, RegistryValueKind.DWord);
                        Debug.WriteLine("? Power Throttling deshabilitado");
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error deshabilitando Power Throttling: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Habilita Power Throttling
        /// </summary>
        public static bool EnablePowerThrottling()
        {
            try
            {
                const string powerKey = @"SYSTEM\CurrentControlSet\Control\Power\PowerThrottling";

                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(powerKey))
                {
                    if (key != null)
                    {
                        key.SetValue("PowerThrottlingOff", 0, RegistryValueKind.DWord);
                        Debug.WriteLine("? Power Throttling habilitado");
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error habilitando Power Throttling: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Deshabilita hibernación
        /// </summary>
        public static bool DisableHibernation()
        {
            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo = new ProcessStartInfo
                    {
                        FileName = "powercfg",
                        Arguments = "/h off",
                        UseShellExecute = true,
                        Verb = "runas", // Requiere privilegios de administrador
                        CreateNoWindow = true
                    };

                    process.Start();
                    process.WaitForExit();

                    Debug.WriteLine("? Hibernación deshabilitada");
                    return process.ExitCode == 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error deshabilitando hibernación: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Habilita hibernación
        /// </summary>
        public static bool EnableHibernation()
        {
            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo = new ProcessStartInfo
                    {
                        FileName = "powercfg",
                        Arguments = "/h on",
                        UseShellExecute = true,
                        Verb = "runas",
                        CreateNoWindow = true
                    };

                    process.Start();
                    process.WaitForExit();

                    Debug.WriteLine("? Hibernación habilitada");
                    return process.ExitCode == 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error habilitando hibernación: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Deshabilita Sleep Study (análisis de consumo energético durante sleep)
        /// 
        /// ¿Qué es Sleep Study?
        /// - Windows registra el consumo de energía mientras el equipo está en sleep/hibernación
        /// - Genera logs detallados de actividad del sistema durante el sueño
        /// - Consume recursos al despertar y al dormir
        /// 
        /// IMPACTO:
        /// - Reduce la carga al entrar/salir de sleep
        /// - Elimina escrituras de log innecesarias
        /// </summary>
        /// <returns>True si la operación fue exitosa, false en caso contrario</returns>
        public static bool DisableSleepStudy()
        {
            try
            {
                const string powerKey = @"SYSTEM\CurrentControlSet\Control\Power";

                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(powerKey))
                {
                    if (key != null)
                    {
                        key.SetValue("SleepStudyDisabled", 1, RegistryValueKind.DWord);
                        Debug.WriteLine("✓ Sleep Study deshabilitado");
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error deshabilitando Sleep Study: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Habilita Sleep Study (restaura estado predeterminado)
        /// </summary>
        /// <returns>True si la operación fue exitosa, false en caso contrario</returns>
        public static bool EnableSleepStudy()
        {
            try
            {
                const string powerKey = @"SYSTEM\CurrentControlSet\Control\Power";

                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(powerKey, true))
                {
                    if (key != null)
                    {
                        key.DeleteValue("SleepStudyDisabled", false);
                        Debug.WriteLine("✓ Sleep Study habilitado (predeterminado)");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error habilitando Sleep Study: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Deshabilita el ahorro de energía en drivers PCI (PCI Express Link State Power Management)
        /// 
        /// ¿Qué hace?
        /// - Los dispositivos PCI/PCIe pueden reducir la velocidad del enlace para ahorrar energía
        /// - Esto añade latencia cuando el dispositivo necesita operar a velocidad completa
        /// 
        /// IMPACTO EN GAMING:
        /// - Elimina stutter causado por transiciones de energía PCIe
        /// - Afecta principalmente a GPU, NIC y controladores NVMe
        /// </summary>
        /// <returns>True si la operación fue exitosa, false en caso contrario</returns>
        public static bool DisablePowersavingDrivers()
        {
            try
            {
                Debug.WriteLine("→ Deshabilitando ahorro de energía en drivers PCI...");

                bool success = true;

                // Deshabilitar PCIe Link State Power Management via powercfg
                success &= RunPowercfgSilent(
                    "-setacvalueindex scheme_current SUB_PCIEXPRESS ASPM 0",
                    "PCIe ASPM=0 (deshabilitado)");

                success &= RunPowercfgSilent("-setactive scheme_current", "Aplicar configuración");

                if (success)
                {
                    Debug.WriteLine("✓ Ahorro de energía en drivers PCI deshabilitado");
                }

                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error deshabilitando Powersaving Drivers: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Habilita el ahorro de energía en drivers PCI (restaura estado predeterminado)
        /// </summary>
        /// <returns>True si la operación fue exitosa, false en caso contrario</returns>
        public static bool EnablePowersavingDrivers()
        {
            try
            {
                Debug.WriteLine("→ Habilitando ahorro de energía en drivers PCI...");

                bool success = RunPowercfgSilent(
                    "-setacvalueindex scheme_current SUB_PCIEXPRESS ASPM 2",
                    "PCIe ASPM=2 (moderado, predeterminado)");

                success &= RunPowercfgSilent("-setactive scheme_current", "Aplicar configuración");

                if (success)
                {
                    Debug.WriteLine("✓ Ahorro de energía en drivers PCI habilitado");
                }

                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error habilitando Powersaving Drivers: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Deshabilita el powergating de GPU (NVIDIA) y CPU
        /// 
        /// ¿Qué es Powergating?
        /// - Técnica donde partes del chip se apagan completamente cuando no se usan
        /// - NVIDIA usa PowerMizer para gestionar el estado de energía de la GPU
        /// - Reduce el consumo energético pero puede causar stuttering al escalar la energía
        /// 
        /// IMPACTO EN GAMING:
        /// - GPU siempre lista para renderizar a máximo rendimiento
        /// - Elimina stutter causado por transiciones de estado de energía
        /// </summary>
        /// <returns>True si la operación fue exitosa, false en caso contrario</returns>
        public static bool DisablePowergating()
        {
            try
            {
                Debug.WriteLine("→ Deshabilitando powergating...");

                bool success = true;

                // Deshabilitar NVIDIA PowerMizer (forzar modo rendimiento máximo)
                const string nvlddmkmKey = @"SYSTEM\CurrentControlSet\Services\nvlddmkm";

                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(nvlddmkmKey))
                {
                    if (key != null)
                    {
                        // PowerMizerEnable = 0 - Deshabilita PowerMizer (gestión de energía NVIDIA)
                        key.SetValue("PowerMizerEnable", 0, RegistryValueKind.DWord);
                        Debug.WriteLine("   ✓ NVIDIA PowerMizerEnable = 0");

                        // PowerMizerDefault = 1 - Forza modo máximo rendimiento
                        key.SetValue("PowerMizerDefault", 1, RegistryValueKind.DWord);
                        Debug.WriteLine("   ✓ NVIDIA PowerMizerDefault = 1 (máximo rendimiento)");
                    }
                    else
                    {
                        Debug.WriteLine("   ⚠ No se encontró el driver NVIDIA nvlddmkm");
                        success = false;
                    }
                }

                if (success)
                {
                    Debug.WriteLine("✓ Powergating deshabilitado");
                    Debug.WriteLine("   ⚠ REQUIERE REINICIO para aplicar");
                }

                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error deshabilitando Powergating: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Habilita el powergating de GPU (restaura estado predeterminado)
        /// </summary>
        /// <returns>True si la operación fue exitosa, false en caso contrario</returns>
        public static bool EnablePowergating()
        {
            try
            {
                Debug.WriteLine("→ Habilitando powergating...");

                const string nvlddmkmKey = @"SYSTEM\CurrentControlSet\Services\nvlddmkm";

                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(nvlddmkmKey, true))
                {
                    if (key != null)
                    {
                        key.DeleteValue("PowerMizerEnable", false);
                        key.DeleteValue("PowerMizerDefault", false);
                        Debug.WriteLine("   ✓ NVIDIA PowerMizer restaurado a predeterminados");
                    }
                }

                Debug.WriteLine("✓ Powergating habilitado (predeterminado)");
                Debug.WriteLine("   ⚠ REQUIERE REINICIO para aplicar");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error habilitando Powergating: {ex.Message}");
                return false;
            }
        }

        private static bool RunPowercfgSilent(string arguments, string description)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powercfg.exe",
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (Process process = Process.Start(psi))
                {
                    process?.WaitForExit(5000);
                    bool success = process?.ExitCode == 0;
                    Debug.WriteLine($"   → powercfg: {description} - {(success ? "✓" : "⚠")}");
                    return success;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"   ✗ Error ejecutando powercfg: {ex.Message}");
                return false;
            }
        }
    }
}
