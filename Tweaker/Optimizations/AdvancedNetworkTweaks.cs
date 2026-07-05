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
            success &= DisableAdapterPowerSaving();
            return success;
        }

        public static bool RestoreAllAdvancedSettings()
        {
            RestoreMTU();
            RestoreAdapterSettings();
            RevertQoS();
            RevertAutoTuning();
            RevertCongestionControl();
            RestoreAdapterPowerSaving();
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

        // ---------------------------------------------------------------------------------------------------
        // PER-ADAPTER POWER SAVING OPTIMIZATIONS
        // ---------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Disables power-saving features on all physical network adapters
        /// </summary>
        public static bool DisableAdapterPowerSaving()
        {
            try
            {
                const string adapterClassKeyPath = @"SYSTEM\CurrentControlSet\Control\Class\{4d36e972-e325-11ce-bfc1-08002be10318}";
                using (RegistryKey? classKey = Registry.LocalMachine.OpenSubKey(adapterClassKeyPath, true))
                {
                    if (classKey == null) return false;

                    foreach (string subkeyName in classKey.GetSubKeyNames())
                    {
                        if (subkeyName.Length == 4 && int.TryParse(subkeyName, out _))
                        {
                            using (RegistryKey? adapterKey = classKey.OpenSubKey(subkeyName, true))
                            {
                                if (adapterKey == null) continue;

                                object? characteristics = adapterKey.GetValue("Characteristics");
                                if (characteristics == null) continue;
                                
                                int charVal = Convert.ToInt32(characteristics);
                                if ((charVal & 0x4) == 0) continue; // Must be NCF_PHYSICAL

                                // Disable "Allow the computer to turn off this device to save power"
                                adapterKey.SetValue("*PnPCapabilities", 24, RegistryValueKind.DWord);

                                // Disable Energy Efficient Ethernet (EEE)
                                SafeSetRegistryValue(adapterKey, "*EEE", 0);
                                SafeSetRegistryValue(adapterKey, "EEELinkAdvertisement", 0);

                                // Disable Green Ethernet / Power Saving Modes
                                SafeSetRegistryValue(adapterKey, "*GreenInternet", 0);
                                SafeSetRegistryValue(adapterKey, "AutoPowerSaveModeEnabled", 0);
                                SafeSetRegistryValue(adapterKey, "ReduceSpeedOnPowerDown", 0);
                                SafeSetRegistryValue(adapterKey, "UltraLowPowerMode", 0);
                                SafeSetRegistryValue(adapterKey, "*AoAcPacketCoalescing", 0);

                                Debug.WriteLine($"✅ Power saving disabled for adapter: {adapterKey.GetValue("DriverDesc")}");
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error disabling adapter power saving: {ex.Message}");
                return false;
            }
        }

        public static bool RestoreAdapterPowerSaving()
        {
            try
            {
                const string adapterClassKeyPath = @"SYSTEM\CurrentControlSet\Control\Class\{4d36e972-e325-11ce-bfc1-08002be10318}";
                using (RegistryKey? classKey = Registry.LocalMachine.OpenSubKey(adapterClassKeyPath, true))
                {
                    if (classKey == null) return false;

                    foreach (string subkeyName in classKey.GetSubKeyNames())
                    {
                        if (subkeyName.Length == 4 && int.TryParse(subkeyName, out _))
                        {
                            using (RegistryKey? adapterKey = classKey.OpenSubKey(subkeyName, true))
                            {
                                if (adapterKey == null) continue;

                                object? characteristics = adapterKey.GetValue("Characteristics");
                                if (characteristics == null) continue;
                                
                                int charVal = Convert.ToInt32(characteristics);
                                if ((charVal & 0x4) == 0) continue;

                                // Restore standard settings
                                adapterKey.DeleteValue("*PnPCapabilities", false);

                                SafeSetRegistryValue(adapterKey, "*EEE", 1);
                                SafeSetRegistryValue(adapterKey, "EEELinkAdvertisement", 1);
                                SafeSetRegistryValue(adapterKey, "*GreenInternet", 1);
                                SafeSetRegistryValue(adapterKey, "AutoPowerSaveModeEnabled", 1);
                                SafeSetRegistryValue(adapterKey, "ReduceSpeedOnPowerDown", 1);
                                SafeSetRegistryValue(adapterKey, "UltraLowPowerMode", 1);
                                SafeSetRegistryValue(adapterKey, "*AoAcPacketCoalescing", 1);

                                Debug.WriteLine($"✅ Power saving restored for adapter: {adapterKey.GetValue("DriverDesc")}");
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error restoring adapter power saving: {ex.Message}");
                return false;
            }
        }

        public static bool IsAdapterPowerSavingDisabled()
        {
            try
            {
                const string adapterClassKeyPath = @"SYSTEM\CurrentControlSet\Control\Class\{4d36e972-e325-11ce-bfc1-08002be10318}";
                using (RegistryKey? classKey = Registry.LocalMachine.OpenSubKey(adapterClassKeyPath, false))
                {
                    if (classKey == null) return false;

                    foreach (string subkeyName in classKey.GetSubKeyNames())
                    {
                        if (subkeyName.Length == 4 && int.TryParse(subkeyName, out _))
                        {
                            using (RegistryKey? adapterKey = classKey.OpenSubKey(subkeyName, false))
                            {
                                if (adapterKey == null) continue;

                                object? characteristics = adapterKey.GetValue("Characteristics");
                                if (characteristics == null) continue;
                                
                                int charVal = Convert.ToInt32(characteristics);
                                if ((charVal & 0x4) == 0) continue;

                                object? pnp = adapterKey.GetValue("*PnPCapabilities");
                                if (pnp != null && Convert.ToInt32(pnp) == 24)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private static void SafeSetRegistryValue(RegistryKey key, string valueName, object value)
        {
            try
            {
                object? existing = key.GetValue(valueName);
                if (existing != null)
                {
                    if (existing is string)
                    {
                        key.SetValue(valueName, value.ToString() ?? "0", RegistryValueKind.String);
                    }
                    else if (existing is int)
                    {
                        key.SetValue(valueName, Convert.ToInt32(value), RegistryValueKind.DWord);
                    }
                    else
                    {
                        key.SetValue(valueName, value);
                    }
                }
                else
                {
                    // If property doesn't exist, we skip or set it to appropriate default if it's a known driver key
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error writing {valueName}: {ex.Message}");
            }
        }

        // Aliases
        public static bool RevertMTU() => RestoreMTU();
        public static bool RevertAdapterSettings() => RestoreAdapterSettings();
        public static bool ApplyAllOptimizations() => ApplyAllAdvancedOptimizations();
        public static bool RevertAllOptimizations() => RestoreAllAdvancedSettings();
    }
}
