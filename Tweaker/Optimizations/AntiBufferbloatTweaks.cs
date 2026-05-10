using System;
using System.Diagnostics;
using System.Management;

using Microsoft.Win32;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// Tweaks especializados para combatir el Bufferbloat y optimizar latencia.
    /// Unificado con optimizaciones avanzadas de red.
    /// </summary>
    public static class AntiBufferbloatTweaks
    {
        private const string TCP_PARAMETERS = @"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters";

        // ---------------------------------------------------------------------------------------------------
        // 1. ECN Capability (Explicit Congestion Notification)
        // ---------------------------------------------------------------------------------------------------
        public static bool EnableECN()
        {
            return ExecuteCommand("netsh int tcp set global ecncapability=enabled");
        }

        public static bool DisableECN()
        {
            return ExecuteCommand("netsh int tcp set global ecncapability=disabled");
        }

        // ---------------------------------------------------------------------------------------------------
        // 2. TCP Congestion Provider (CUBIC / BBR / CTCP)
        // ---------------------------------------------------------------------------------------------------
        public static bool SetCubicCongestion()
        {
            // Windows 11 soporta BBR2, Windows 10 soporta CUBIC.
            // CUBIC es superior a CTCP para gaming moderno.
            return ExecuteCommand("netsh int tcp set supplemental template=internet congestionprovider=cubic");
        }

        public static bool RevertCongestion()
        {
            return ExecuteCommand("netsh int tcp set supplemental template=internet congestionprovider=default");
        }

        // ---------------------------------------------------------------------------------------------------
        // 3. Disable Large Send Offload (LSO)
        // ---------------------------------------------------------------------------------------------------
        public static bool DisableLSO()
        {
            return ExecuteCommand("powershell.exe -NoProfile -Command \"Enable-NetAdapterAdvancedProperty -Name '*' -DisplayName 'Large Send Offload*' -Enabled $false\"");
        }

        public static bool EnableLSO()
        {
            return ExecuteCommand("powershell.exe -NoProfile -Command \"Enable-NetAdapterAdvancedProperty -Name '*' -DisplayName 'Large Send Offload*' -Enabled $true\"");
        }

        // ---------------------------------------------------------------------------------------------------
        // 4. QoS DSCP Tagging & Packet Scheduler
        // ---------------------------------------------------------------------------------------------------
        public static bool EnableQoS_Prioritization()
        {
            try
            {
                // Habilitar etiquetado DSCP (Anti-NLA)
                using (RegistryKey? key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\services\Tcpip\QoS", true))
                {
                    if (key != null) key.SetValue("Do not use NLA", "1", RegistryValueKind.String);
                }

                // Deshabilitar reserva de ancho de banda (20% default)
                using (RegistryKey? key = Registry.LocalMachine.CreateSubKey(TCP_PARAMETERS, true))
                {
                    if (key != null)
                    {
                        key.SetValue("NonBestEffortLimit", 0, RegistryValueKind.DWord);
                        key.SetValue("DisableUserTOSSetting", 0, RegistryValueKind.DWord);
                        key.SetValue("DefaultTOSValue", 0, RegistryValueKind.DWord);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error configurando QoS: {ex.Message}");
                return false;
            }
        }

        public static bool DisableQoS_Prioritization()
        {
            try
            {
                using (RegistryKey? key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\services\Tcpip\QoS", true))
                {
                    if (key != null) key.DeleteValue("Do not use NLA", false);
                }

                using (RegistryKey? key = Registry.LocalMachine.OpenSubKey(TCP_PARAMETERS, true))
                {
                    if (key != null)
                    {
                        key.DeleteValue("NonBestEffortLimit", false);
                        key.DeleteValue("DisableUserTOSSetting", false);
                        key.DeleteValue("DefaultTOSValue", false);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error revirtiendo QoS: {ex.Message}");
                return false;
            }
        }

        // ---------------------------------------------------------------------------------------------------
        // 5. TCP Auto-Tuning Level
        // ---------------------------------------------------------------------------------------------------
        public static bool RestrictAutoTuning()
        {
            // Restricted es mejor que Normal para Bufferbloat de bajada.
            return ExecuteCommand("netsh int tcp set global autotuninglevel=restricted");
        }

        public static bool NormalAutoTuning()
        {
            return ExecuteCommand("netsh int tcp set global autotuninglevel=normal");
        }

        // ---------------------------------------------------------------------------------------------------
        // Helper
        // ---------------------------------------------------------------------------------------------------
        private static bool ExecuteCommand(string command)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c {command}",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (Process? process = Process.Start(psi))
                {
                    if (process == null) return false;
                    process.WaitForExit();
                    return process.ExitCode == 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error ejecutando comando ({command}): {ex.Message}");
                return false;
            }
        }

        private static string? GetActiveNetworkInterface()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    "SELECT * FROM Win32_NetworkAdapter WHERE NetConnectionStatus = 2");

                foreach (ManagementObject obj in searcher.Get())
                {
                    string netConnectionID = obj["NetConnectionID"]?.ToString();
                    if (!string.IsNullOrEmpty(netConnectionID)) return netConnectionID;
                }
                return null;
            }
            catch { return null; }
        }
    }
}
