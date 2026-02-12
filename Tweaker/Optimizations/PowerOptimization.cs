using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Management;

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
    }
}