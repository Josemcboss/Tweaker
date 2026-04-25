using System;
using System.Diagnostics;
using System.Linq;
using System.Management;

using Microsoft.Win32;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// Optimizaciones avanzadas de red para gaming competitivo.
    /// Esta clase se ha simplificado para evitar duplicidades con AntiBufferbloatTweaks.
    /// </summary>
    public static class AdvancedNetworkTweaks
    {
        private const string TCP_PARAMETERS = @"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters";

        // ---------------------------------------------------------------------------------------------------
        // MTU OPTIMIZATION
        // ---------------------------------------------------------------------------------------------------
        public static bool OptimizeMTU()
        {
            try
            {
                string activeInterface = GetActiveNetworkInterface();
                if (string.IsNullOrEmpty(activeInterface)) return false;

                int optimalMTU = 1492;
                return ExecuteCommand("netsh", $"interface ipv4 set subinterface \"{activeInterface}\" mtu={optimalMTU} store=persistent");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error optimizando MTU: {ex.Message}");
                return false;
            }
        }

        public static bool RestoreMTU()
        {
            try
            {
                string activeInterface = GetActiveNetworkInterface();
                if (string.IsNullOrEmpty(activeInterface)) return false;

                return ExecuteCommand("netsh", $"interface ipv4 set subinterface \"{activeInterface}\" mtu=1500 store=persistent");
            }
            catch { return false; }
        }

        // ---------------------------------------------------------------------------------------------------
        // ADAPTER SETTINGS (RSS, Scaling, SACK)
        // ---------------------------------------------------------------------------------------------------
        public static bool OptimizeAdapterSettings()
        {
            try
            {
                using (RegistryKey? key = Registry.LocalMachine.CreateSubKey(TCP_PARAMETERS))
                {
                    if (key != null)
                    {
                        key.SetValue("Tcp1323Opts", 3, RegistryValueKind.DWord);
                        key.SetValue("SackOpts", 1, RegistryValueKind.DWord);
                        key.SetValue("TcpMaxDataRetransmissions", 3, RegistryValueKind.DWord);
                        key.SetValue("SynAttackProtect", 1, RegistryValueKind.DWord);
                        key.SetValue("KeepAliveTime", 300000, RegistryValueKind.DWord);
                        key.SetValue("DefaultTTL", 128, RegistryValueKind.DWord);
                        key.SetValue("TcpMaxDupAcks", 2, RegistryValueKind.DWord);
                        key.SetValue("TcpInitialRtt", 300, RegistryValueKind.DWord);
                    }
                }

                // Habilitar RSS y otras descargas por hardware
                ExecuteCommand("netsh", "interface tcp set global rss=enabled");
                ExecuteCommand("netsh", "interface tcp set global chimney=enabled");
                ExecuteCommand("netsh", "interface tcp set global netdma=enabled");
                ExecuteCommand("netsh", "interface tcp set global timestamps=enabled");

                return true;
            }
            catch { return false; }
        }

        public static bool RestoreAdapterSettings()
        {
            try
            {
                using (RegistryKey? key = Registry.LocalMachine.OpenSubKey(TCP_PARAMETERS, true))
                {
                    if (key != null)
                    {
                        key.DeleteValue("Tcp1323Opts", false);
                        key.DeleteValue("SackOpts", false);
                        key.DeleteValue("TcpMaxDataRetransmissions", false);
                        key.DeleteValue("SynAttackProtect", false);
                        key.DeleteValue("KeepAliveTime", false);
                        key.DeleteValue("DefaultTTL", false);
                        key.DeleteValue("TcpMaxDupAcks", false);
                        key.DeleteValue("TcpInitialRtt", false);
                    }
                }

                ExecuteCommand("netsh", "interface tcp set global chimney=automatic");
                ExecuteCommand("netsh", "interface tcp set global timestamps=disabled");
                return true;
            }
            catch { return false; }
        }

        // ---------------------------------------------------------------------------------------------------
        // UNIFIED CALLS (Delegating to AntiBufferbloatTweaks)
        // ---------------------------------------------------------------------------------------------------
        public static bool ConfigureQoS() => AntiBufferbloatTweaks.EnableQoS_Prioritization();
        public static bool RevertQoS() => AntiBufferbloatTweaks.DisableQoS_Prioritization();

        public static bool ConfigureAutoTuning() => AntiBufferbloatTweaks.RestrictAutoTuning();
        public static bool RevertAutoTuning() => AntiBufferbloatTweaks.NormalAutoTuning();

        public static bool ConfigureCongestionControl() => AntiBufferbloatTweaks.SetCubicCongestion();
        public static bool RevertCongestionControl() => AntiBufferbloatTweaks.RevertCongestion();

        public static bool ApplyAllAdvancedOptimizations()
        {
            bool success = true;
            success &= OptimizeMTU();
            success &= OptimizeAdapterSettings();
            success &= ConfigureQoS();
            success &= ConfigureAutoTuning();
            success &= ConfigureCongestionControl();
            return success;
        }

        public static bool RestoreAllAdvancedSettings()
        {
            RestoreMTU();
            RestoreAdapterSettings();
            RevertQoS();
            RevertAutoTuning();
            RevertCongestionControl();
            return true;
        }

        // ---------------------------------------------------------------------------------------------------
        // HELPERS
        // ---------------------------------------------------------------------------------------------------
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

        private static bool ExecuteCommand(string fileName, string arguments)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true,
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
            catch { return false; }
        }

        // Aliases
        public static bool RevertMTU() => RestoreMTU();
        public static bool RevertAdapterSettings() => RestoreAdapterSettings();
        public static bool ApplyAllOptimizations() => ApplyAllAdvancedOptimizations();
        public static bool RevertAllOptimizations() => RestoreAllAdvancedSettings();
    }
}
