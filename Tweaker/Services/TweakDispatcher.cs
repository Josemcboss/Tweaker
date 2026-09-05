using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Win32;
using Tweaker.Optimizations;

namespace Tweaker.Services
{
    public class TweakDispatcher : ITweakDispatcher
    {
        private class TweakActions
        {
            public Func<bool> Apply { get; set; } = () => false;
            public Func<bool> Revert { get; set; } = () => false;
            public Func<bool?> GetState { get; set; } = () => null;
        }

        private readonly Dictionary<string, TweakActions> _mappings;
        // Stores ONLY the canonical (first-registered) ID for each unique tweak.
        private readonly HashSet<string> _canonicalIds;
        private readonly Dictionary<string, string> _aliasToCanonical;
        private readonly Dictionary<string, List<string>> _canonicalToAliases;

        public TweakDispatcher()
        {
            _mappings = new Dictionary<string, TweakActions>(StringComparer.OrdinalIgnoreCase);
            _canonicalIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _aliasToCanonical = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _canonicalToAliases = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            InitializeMappings();
        }

        /// <summary>
        /// Registers a tweak with its canonical ID and optional aliases.
        /// Only the canonical ID is counted in GetCanonicalTweakIds().
        /// </summary>
        private void Register(string canonicalId, TweakActions actions, params string[] aliases)
        {
            _mappings[canonicalId] = actions;
            _canonicalIds.Add(canonicalId);
            _aliasToCanonical[canonicalId] = canonicalId;
            if (!_canonicalToAliases.ContainsKey(canonicalId))
                _canonicalToAliases[canonicalId] = new List<string>();

            foreach (var alias in aliases)
            {
                _mappings[alias] = actions;
                _aliasToCanonical[alias] = canonicalId;
                if (!_canonicalToAliases[canonicalId].Contains(alias, StringComparer.OrdinalIgnoreCase))
                    _canonicalToAliases[canonicalId].Add(alias);
            }
        }

        #region System Inspection Helpers
        private static bool CheckRegistryValue(RegistryKey hive, string subKey, string valueName, object expectedValue)
        {
            try
            {
                using var key = hive.OpenSubKey(subKey, false);
                if (key == null) return false;
                var val = key.GetValue(valueName);
                if (val == null) return false;
                return val.ToString()!.Equals(expectedValue.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            catch { return false; }
        }

        private static bool CheckRegistryDWord(RegistryKey hive, string subKey, string valueName, int expectedValue)
        {
            try
            {
                using var key = hive.OpenSubKey(subKey, false);
                if (key == null) return false;
                var val = key.GetValue(valueName);
                if (val == null) return false;
                return Convert.ToInt32(val) == expectedValue;
            }
            catch { return false; }
        }

        private static bool IsServiceDisabled(string serviceName)
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey($@"SYSTEM\CurrentControlSet\Services\{serviceName}", false);
                if (key == null) return false;
                var start = key.GetValue("Start");
                if (start != null && Convert.ToInt32(start) == 4) // 4 = Disabled
                    return true;
                return false;
            }
            catch { return false; }
        }

        private static bool IsNetshGlobalSetting(string parameter, string expectedValue)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "netsh",
                    Arguments = "interface tcp show global",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using var proc = Process.Start(psi);
                if (proc == null) return false;
                string output = proc.StandardOutput.ReadToEnd();
                proc.WaitForExit(2000);
                foreach (var line in output.Split('\n'))
                {
                    if (line.IndexOf(parameter, StringComparison.OrdinalIgnoreCase) >= 0 &&
                        line.IndexOf(expectedValue, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch { return false; }
        }

        private static bool IsNetshSupplementalSetting(string parameter, string expectedValue)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "netsh",
                    Arguments = "interface tcp show supplemental",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using var proc = Process.Start(psi);
                if (proc == null) return false;
                string output = proc.StandardOutput.ReadToEnd();
                proc.WaitForExit(2000);
                foreach (var line in output.Split('\n'))
                {
                    if (line.IndexOf(parameter, StringComparison.OrdinalIgnoreCase) >= 0 &&
                        line.IndexOf(expectedValue, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch { return false; }
        }

        private static bool CheckBcdEditSetting(string settingName, string expectedValue)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "bcdedit",
                    Arguments = "/enum {current}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using var proc = Process.Start(psi);
                if (proc == null) return false;
                string output = proc.StandardOutput.ReadToEnd();
                proc.WaitForExit(2000);
                foreach (var line in output.Split('\n'))
                {
                    if (line.TrimStart().StartsWith(settingName, StringComparison.OrdinalIgnoreCase) &&
                        line.IndexOf(expectedValue, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch { return false; }
        }

        private static bool IsActivePowerPlanContaining(string planNameOrGuid)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "powercfg",
                    Arguments = "/getactivescheme",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using var proc = Process.Start(psi);
                if (proc == null) return false;
                string output = proc.StandardOutput.ReadToEnd();
                proc.WaitForExit(2000);
                return output.IndexOf(planNameOrGuid, StringComparison.OrdinalIgnoreCase) >= 0;
            }
            catch { return false; }
        }
        #endregion

        private void InitializeMappings()
        {
            // 1. INPUT & VISUALS
            Register("mouse_acceleration", new TweakActions
            {
                Apply = () => MouseOptimization.DisableAcceleration(),
                Revert = () => MouseOptimization.EnableAcceleration(),
                GetState = () => MouseOptimization.IsAccelerationDisabled()
            }, "MouseAcceleration");

            Register("raw_aim_curve", new TweakActions
            {
                Apply = () => InputTweaks.DisableMouseAccelerationKernelLevel(),
                Revert = () => InputTweaks.RestoreMouseAccelerationKernelLevel(),
                GetState = () => InputTweaks.IsMouseAccelerationKernelLevelDisabled()
            }, "RawAimCurve");

            Register("keyboard_optimization", new TweakActions
            {
                Apply = () => KeyboardOptimization.OptimizeKeyboard(),
                Revert = () => KeyboardOptimization.RestoreKeyboard(),
                GetState = () => CheckRegistryValue(Registry.CurrentUser, @"Control Panel\Keyboard", "KeyboardDelay", "0")
            }, "Keyboard");

            Register("visual_effects", new TweakActions
            {
                Apply = () => VisualOptimization.DisableVisualEffects(),
                Revert = () => VisualOptimization.EnableVisualEffects(),
                GetState = () => CheckRegistryDWord(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects", "VisualFXSetting", 2)
            }, "VisualEffects");

            Register("memory_optimization", new TweakActions
            {
                Apply = () => MemoryTweaks.OptimizeMemory(),
                Revert = () => MemoryTweaks.RestoreMemory(),
                GetState = () => CheckRegistryDWord(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", "ClearPageFileAtShutdown", 0)
            }, "MemoryOptimization");

            Register("transparency_effects", new TweakActions
            {
                Apply = () => VisualOptimization.DisableTransparency(),
                Revert = () => VisualOptimization.EnableTransparency(),
                GetState = () => CheckRegistryDWord(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "EnableTransparency", 0)
            }, "TransparencyEffects");

            Register("sticky_keys", new TweakActions
            {
                Apply = () => ProTweaks.DisableStickyKeys(),
                Revert = () => ProTweaks.RevertStickyKeys(),
                GetState = () => {
                    try {
                        using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Control Panel\Accessibility\StickyKeys");
                        return key?.GetValue("Flags")?.ToString() != "506";
                    } catch { return null; }
                }
            }, "StickyKeys", "stickykeys_guard");

            Register("menu_show_delay", new TweakActions
            {
                Apply = () => AdvancedLatencyTweaks.SetMenuShowDelayZero(),
                Revert = () => AdvancedLatencyTweaks.RestoreMenuShowDelay(),
                GetState = () => CheckRegistryValue(Registry.CurrentUser, @"Control Panel\Desktop", "MenuShowDelay", "0")
            }, "MenuShowDelay");

            Register("data_queue_sizes", new TweakActions
            {
                Apply = () => AdvancedLatencyTweaks.OptimizeDataQueueSizes(),
                Revert = () => AdvancedLatencyTweaks.RestoreDataQueueSizes(),
                GetState = () => CheckRegistryDWord(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Services\mouclass\Parameters", "MouseDataQueueSize", 256) || CheckRegistryDWord(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Services\mouclass\Parameters", "MouseDataQueueSize", 16)
            }, "DataQueueSizes");

            // 2. RED & PING
            Register("network_optimization", new TweakActions
            {
                Apply = () => NetworkOptimization.OptimizeNetwork(),
                Revert = () => NetworkOptimization.RestoreNetwork(),
                GetState = () => CheckRegistryDWord(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters", "TCPDelAckTicks", 0) || CheckRegistryDWord(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters", "Tcp1323Opts", 1)
            }, "NetworkOptimization");

            Register("dns_cloudflare", new TweakActions
            {
                Apply = () => DnsOptimization.SetCloudflareDns().Contains("\u2705"),
                Revert = () => NetworkOptimization.RestoreNetwork(),
                GetState = () => { try { return System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces().Any(i => i.GetIPProperties().DnsAddresses.Any(d => d.ToString() == "1.1.1.1")); } catch { return false; } }
            }, "DnsCloudflare");

            Register("dns_google", new TweakActions
            {
                Apply = () => DnsOptimization.SetGoogleDns().Contains("\u2705"),
                Revert = () => NetworkOptimization.RestoreNetwork(),
                GetState = () => { try { return System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces().Any(i => i.GetIPProperties().DnsAddresses.Any(d => d.ToString() == "8.8.8.8")); } catch { return false; } }
            }, "DnsGoogle");

            Register("dns_cache", new TweakActions
            {
                Apply = () => DnsOptimization.EnableDnsCacheOptimization().StartsWith("\u2705"),
                Revert = () => DnsOptimization.DisableDnsCacheOptimization().StartsWith("\u2705"),
                GetState = () => CheckRegistryDWord(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Services\Dnscache\Parameters", "MaxNegativeCacheTtl", 0)
            }, "DnsCache");

            Register("ecn_capability", new TweakActions
            {
                Apply = () => AntiBufferbloatTweaks.EnableECN(),
                Revert = () => AntiBufferbloatTweaks.DisableECN(),
                GetState = () => IsNetshGlobalSetting("ECN Capability", "enabled")
            }, "EcnCapability");

            Register("tcp_congestion", new TweakActions
            {
                Apply = () => AntiBufferbloatTweaks.SetCubicCongestion(),
                Revert = () => AntiBufferbloatTweaks.RevertCongestion(),
                GetState = () => IsNetshSupplementalSetting("Congestion Provider", "cubic")
            }, "TcpCongestion");

            Register("disable_lso", new TweakActions
            {
                Apply = () => AntiBufferbloatTweaks.DisableLSO(),
                Revert = () => AntiBufferbloatTweaks.EnableLSO(),
                GetState = () => { try { using var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters"); return key != null && (Convert.ToInt32(key.GetValue("DisableTaskOffload", 0)) == 1); } catch { return false; } }
            }, "DisableLso");

            Register("qos_prioritization", new TweakActions
            {
                Apply = () => AntiBufferbloatTweaks.EnableQoS_Prioritization(),
                Revert = () => AntiBufferbloatTweaks.DisableQoS_Prioritization(),
                GetState = () => CheckRegistryValue(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\services\Tcpip\QoS", "Do not use NLA", "1")
            }, "QosPrioritization");

            Register("tcp_autotuning", new TweakActions
            {
                Apply = () => AntiBufferbloatTweaks.RestrictAutoTuning(),
                Revert = () => AntiBufferbloatTweaks.NormalAutoTuning(),
                GetState = () => IsNetshGlobalSetting("Receive Window Auto-Tuning Level", "restricted")
            }, "TcpAutoTuning");

            Register("interrupt_moderation", new TweakActions
            {
                Apply = () => AdvancedLatencyTweaks.DisableInterruptModeration(),
                Revert = () => AdvancedLatencyTweaks.RestoreInterruptModeration(),
                GetState = () => { try { using var k = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Class\{4d36e972-e325-11ce-bfc1-08002be10318}"); if (k == null) return false; foreach (var n in k.GetSubKeyNames().Where(s => s.Length == 4 && s.All(char.IsDigit))) { using var sub = k.OpenSubKey(n); if (sub?.GetValue("*InterruptModeration")?.ToString() == "0") return true; } return false; } catch { return false; } }
            }, "InterruptModeration");

            Register("network_power", new TweakActions
            {
                Apply = () => AdvancedNetworkTweaks.DisableAdapterPowerSaving(),
                Revert = () => AdvancedNetworkTweaks.RestoreAdapterPowerSaving(),
                GetState = () => AdvancedNetworkTweaks.IsAdapterPowerSavingDisabled()
            }, "NetworkPower");

            Register("netbios", new TweakActions
            {
                Apply = () => DnsOptimization.DisableNetBios().StartsWith("\u2705"),
                Revert = () => DnsOptimization.EnableNetBios().StartsWith("\u2705"),
                GetState = () => { try { using var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\NetBT\Parameters\Interfaces"); if (key == null) return false; foreach (var n in key.GetSubKeyNames()) { using var sub = key.OpenSubKey(n); if (sub != null && Convert.ToInt32(sub.GetValue("NetbiosOptions", 0)) == 2) return true; } return false; } catch { return false; } }
            }, "NetBios");

            Register("do_solo_mode", new TweakActions
            {
                Apply = () => UpdateTweaks.EnableDOSoloMode(),
                Revert = () => UpdateTweaks.DisableDOSoloMode(),
                GetState = () => UpdateTweaks.IsDOSoloModeEnabled()
            }, "DOSoloMode");

            // 3. SISTEMA & GPU
            Register("system_profile", new TweakActions
            {
                Apply = () => GpuOptimization.EnableSystemProfileOptimization(),
                Revert = () => GpuOptimization.DisableSystemProfileOptimization(),
                GetState = () => CheckRegistryDWord(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile", "NetworkThrottlingIndex", -1) || CheckRegistryDWord(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile", "NetworkThrottlingIndex", unchecked((int)0xFFFFFFFF))
            }, "SystemProfile");

            Register("system_responsiveness", new TweakActions
            {
                Apply = () => CpuOptimization.EnableSystemResponsivenessOptimization(),
                Revert = () => CpuOptimization.DisableSystemResponsivenessOptimization(),
                GetState = () => CheckRegistryDWord(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile", "SystemResponsiveness", 0)
            }, "SystemResponsiveness");

            Register("connected_standby_disable", new TweakActions
            {
                Apply = () => AdvancedSystemTweaks.DisableConnectedStandby(),
                Revert = () => AdvancedSystemTweaks.RestoreConnectedStandby(),
                GetState = () => AdvancedSystemTweaks.IsConnectedStandbyDisabled()
            }, "ConnectedStandby", "cs_enabled_disable", "ModernStandby");

            Register("watchdog_disable", new TweakActions
            {
                Apply = () => AdvancedSystemTweaks.DisableWatchdog(),
                Revert = () => AdvancedSystemTweaks.RestoreWatchdog(),
                GetState = () => AdvancedSystemTweaks.IsWatchdogDisabled()
            }, "Watchdog", "watchdog_timer_disable");

            Register("gamedvr_disable", new TweakActions
            {
                Apply = () => GpuOptimization.DisableGameDVR(),
                Revert = () => GpuOptimization.EnableGameDVR(),
                GetState = () => !WindowsDebloat.IsGameDVREnabled()
            }, "GameDVR", "fso_game_dvr", "gamebar_disable", "GameBar");

            Register("gpu_scheduling", new TweakActions
            {
                Apply = () => NvidiaOptimization.OptimizeNvidiaGPU(),
                Revert = () => NvidiaOptimization.RestoreNvidiaGPU(),
                GetState = () => GpuOptimization.IsGpuSchedulingEnabled()
            }, "GpuScheduling");

            Register("mpo_fix", new TweakActions
            {
                Apply = () => GpuTweaks.DisableMPO(),
                Revert = () => GpuTweaks.EnableMPO(),
                GetState = () => GpuTweaks.IsMPODisabled()
            }, "MPOFix");

            Register("ultimate_power", new TweakActions
            {
                Apply = () => PowerTweaks.EnableUltimatePerformance(),
                Revert = () => PowerTweaks.RestoreBalancedPlan(),
                GetState = () => PowerTweaks.IsUltimatePerformanceActive()
            }, "UltimatePower", "ultimate_power_plan", "core_power_plan");

            Register("high_performance", new TweakActions
            {
                Apply = () => CpuOptimization.EnableHighPerformancePowerPlan(),
                Revert = () => PowerTweaks.RestoreBalancedPlan(),
                GetState = () => CpuOptimization.IsHighPerformanceActive()
            }, "HighPerformance", "high_performance_plan");

            Register("power_throttling_disable", new TweakActions
            {
                Apply = () => CpuOptimization.DisablePowerThrottling(),
                Revert = () => CpuOptimization.EnablePowerThrottling(),
                GetState = () => !PowerOptimization.IsPowerThrottlingEnabled()
            }, "PowerThrottling");

            Register("core_parking_disable", new TweakActions
            {
                Apply = () => CpuOptimization.DisableCoreParking(),
                Revert = () => CpuOptimization.EnableCoreParking(),
                GetState = () => CheckRegistryDWord(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Power\PowerSettings\54533251-82be-4824-96c1-47b60b740d00\0cc5b647-c1df-4637-891a-dec35c318583", "ValueMax", 0)
            }, "CoreParking");

            // 4. GHOST PACK & ADVANCED
            Register("core_isolation", new TweakActions
            {
                Apply = () => WindowsDebloat.DisableCoreIsolation(),
                Revert = () => WindowsDebloat.EnableCoreIsolation(),
                GetState = () => !WindowsDebloat.IsVBSEnabled()
            }, "CoreIsolation");

            Register("hpet_optimization", new TweakActions
            {
                Apply = () => KernelTweaks.OptimizeHPET(),
                Revert = () => KernelTweaks.RestoreHPET(),
                GetState = () => CheckBcdEditSetting("useplatformclock", "No")
            }, "HPET");

            Register("hyperv_disable", new TweakActions
            {
                Apply = () => LatencyOptimization.DisableHyperV(),
                Revert = () => LatencyOptimization.EnableHyperV(),
                GetState = () => CheckBcdEditSetting("hypervisorlaunchtype", "Off")
            }, "HyperV");

            Register("spectre_meltdown_disable", new TweakActions
            {
                Apply = () => AdvancedTweaks.DisableSpectreMeltdown(),
                Revert = () => AdvancedTweaks.EnableSpectreMeltdown(),
                GetState = () => CheckRegistryDWord(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", "FeatureSettingsOverride", 3)
            }, "SpectreMeltdown");

            Register("text_input_host_disable", new TweakActions
            {
                Apply = () => KernelOSToolboxTweaks.DisableTextInputHost(),
                Revert = () => KernelOSToolboxTweaks.EnableTextInputHost(),
                GetState = () => { try { using var k = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\TextInputHost.exe"); return k != null && k.GetValue("Debugger") != null; } catch { return false; } }
            }, "TextInputHostDisable", "text_input_host");

            Register("hvci_disable", new TweakActions
            {
                Apply = () => KernelOSToolboxTweaks.DisableHVCI(),
                Revert = () => KernelOSToolboxTweaks.EnableHVCI(),
                GetState = () => CheckRegistryDWord(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity", "Enabled", 0)
            }, "HvciDisable", "hvci");

            Register("interrupt_steering", new TweakActions
            {
                Apply = () => InterruptSteeringTweaks.EnableInterruptSteering(),
                Revert = () => InterruptSteeringTweaks.RestoreInterruptSteering(),
                GetState = () => InterruptSteeringTweaks.IsInterruptSteeringEnabled()
            }, "InterruptSteering", "interrupt_steering_enable");

            Register("gpu_irq_affinity", new TweakActions
            {
                Apply = () => GpuIRQOptimization.EnableGpuIRQOptimization(),
                Revert = () => GpuIRQOptimization.DisableGpuIRQOptimization()
            }, "GpuIRQAffinity", "gpu_irq");

            Register("hibernation_disable", new TweakActions
            {
                Apply = () => WindowsOptimization.DisableHibernation(),
                Revert = () => WindowsOptimization.EnableHibernation(),
                GetState = () => !PowerOptimization.IsHibernationEnabled()
            }, "Hibernation", "hibernation_wipe", "hibernate_disable");

            Register("windows_search_disable", new TweakActions
            {
                Apply = () => WindowsOptimization.DisableWindowsSearch(),
                Revert = () => WindowsOptimization.EnableWindowsSearch(),
                GetState = () => IsServiceDisabled("WSearch")
            }, "WindowsSearch");

            Register("sysmain_disable", new TweakActions
            {
                Apply = () => WindowsOptimization.DisableSysMain(),
                Revert = () => WindowsOptimization.EnableSysMain(),
                GetState = () => IsServiceDisabled("SysMain")
            }, "SysMain");

            Register("telemetry_disable", new TweakActions
            {
                Apply = () => WindowsOptimization.DisableTelemetry(),
                Revert = () => WindowsOptimization.EnableTelemetry(),
                GetState = () => IsServiceDisabled("DiagTrack")
            }, "Telemetry");

            Register("diagtrack_disable", new TweakActions
            {
                Apply = () => ServiceOptimization.DisableDiagTrack(),
                Revert = () => ServiceOptimization.EnableDiagTrack(),
                GetState = () => IsServiceDisabled("DiagTrack")
            }, "DiagTrack");

            // 5. GPU & DISPLAY
            Register("nvidia_low_latency_ultra", new TweakActions
            {
                Apply = () => NvidiaOptimization.OptimizeNvidiaGPU(),
                Revert = () => NvidiaOptimization.RestoreNvidiaGPU(),
                GetState = () => CheckRegistryDWord(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Control\GraphicsDrivers", "MaxFrameLatency", 1)
            }, "NvidiaLowLatencyUltra");

            Register("shader_cache_disable", new TweakActions
            {
                Apply = () => GpuTweaks.DisableMPO(),
                Revert = () => GpuTweaks.EnableMPO(),
                GetState = () => GpuTweaks.IsMPODisabled()
            }, "ShaderCacheDisable");

            Register("gpu_hw_scheduling_v2", new TweakActions
            {
                Apply = () => NvidiaOptimization.OptimizeNvidiaGPU(),
                Revert = () => NvidiaOptimization.RestoreNvidiaGPU(),
                GetState = () => GpuOptimization.IsGpuSchedulingEnabled()
            }, "GpuHwSchedulingV2");

            Register("disable_ipv6", new TweakActions
            {
                Apply = () => NetworkOptimization.OptimizeNetwork(),
                Revert = () => NetworkOptimization.RestoreNetwork(),
                GetState = () => CheckRegistryDWord(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Services\Tcpip6\Parameters", "DisabledComponents", 255) || CheckRegistryDWord(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Services\Tcpip6\Parameters", "DisabledComponents", 32)
            }, "DisableIpv6");

            Register("wasapi_exclusive_mode", new TweakActions
            {
                Apply = () => AudioOptimization.OptimizeAudioLatency(),
                Revert = () => AudioOptimization.RestoreAudioSettings(),
                GetState = () => CheckRegistryDWord(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Audio", "Priority", 10)
            }, "WasapiExclusiveMode");

            Register("audio_enhancements_off", new TweakActions
            {
                Apply = () => AudioOptimization.OptimizeAudioLatency(),
                Revert = () => AudioOptimization.RestoreAudioSettings(),
                GetState = () => CheckRegistryDWord(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Audio", "Clock Rate", 10000)
            }, "AudioEnhancementsOff");

            // 6. STORAGE
            Register("ssd_write_cache", new TweakActions
            {
                Apply = () => DiskTweaks.EnableTrim(),
                Revert = () => DiskTweaks.DisableTrim(),
                GetState = () => DiskTweaks.IsTrimEnabled()
            }, "SsdWriteCache");

            Register("trim_optimization", new TweakActions
            {
                Apply = () => DiskTweaks.EnableTrim(),
                Revert = () => DiskTweaks.DisableTrim(),
                GetState = () => DiskTweaks.IsTrimEnabled()
            }, "TrimOptimization", "trim_force_on");

            Register("ntfs_mft_zone", new TweakActions
            {
                Apply = () => DiskTweaks.EnableTrim(),
                Revert = () => DiskTweaks.DisableTrim(),
                GetState = () => CheckRegistryDWord(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Control\FileSystem", "NtfsMftZoneReservation", 2)
            }, "NtfsMftZone");

            // 7. CPU ADVANCED & LOW-LEVEL LATENCY
            Register("irq_network_priority", new TweakActions
            {
                Apply = () => AdvancedLatencyTweaks.DisableInterruptModeration(),
                Revert = () => AdvancedLatencyTweaks.RestoreInterruptModeration(),
                GetState = () => { try { using var k = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Class\{4d36e972-e325-11ce-bfc1-08002be10318}"); if (k == null) return false; foreach (var n in k.GetSubKeyNames().Where(s => s.Length == 4 && s.All(char.IsDigit))) { using var sub = k.OpenSubKey(n); if (sub?.GetValue("*InterruptModeration")?.ToString() == "0") return true; } return false; } catch { return false; } }
            }, "IrqNetworkPriority");

            Register("nagle_algorithm_off", new TweakActions
            {
                Apply = () => NetworkOptimization.OptimizeNetwork(),
                Revert = () => NetworkOptimization.RestoreNetwork(),
                GetState = () => CheckRegistryDWord(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters", "TCPDelAckTicks", 0)
            }, "NagleAlgorithmOff");

            Register("cpu_affinity_auto_gaming", new TweakActions
            {
                Apply = () => CpuOptimization.DisableCoreParking(),
                Revert = () => CpuOptimization.EnableCoreParking(),
                GetState = () => CheckRegistryDWord(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Power\PowerSettings\54533251-82be-4824-96c1-47b60b740d00\0cc5b647-c1df-4637-891a-dec35c318583", "ValueMax", 0)
            }, "CpuAffinityAutoGaming");

            Register("cpu_anti_throttling", new TweakActions
            {
                Apply = () => CpuOptimization.DisablePowerThrottling(),
                Revert = () => CpuOptimization.EnablePowerThrottling(),
                GetState = () => !PowerOptimization.IsPowerThrottlingEnabled()
            }, "CpuAntiThrottling");

            Register("usb_selective_suspend", new TweakActions
            {
                Apply = () => InputTweaks.OptimizeUSBForGaming(),
                Revert = () => InputTweaks.RevertUSBOptimization(),
                GetState = () => {
                    try {
                        using var k = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\USB");
                        return k != null;
                    } catch { return null; }
                }
            }, "usb_power_guard", "USBOptimization");

            Register("win32_priority", new TweakActions
            {
                Apply = () => InputTweaks.SetCpuPriorityValue(38),
                Revert = () => InputTweaks.SetCpuPriorityValue(2),
                GetState = () => InputTweaks.GetCurrentCpuPriorityProfile() != "Default"
            }, "Win32Priority");

            Register("temp_files_cleanup", new TweakActions
            {
                Apply = () => CleanerTweaks.DeepClean().success,
                Revert = () => true,
                GetState = () => false
            }, "TempFilesCleanup");

            Register("win_update_cache", new TweakActions
            {
                Apply = () => AdvancedSystemTweaks.CleanWindowsUpdateCache().success,
                Revert = () => true,
                GetState = () => false
            }, "WinUpdateCache");

            Register("max_timer_resolution", new TweakActions
            {
                Apply = () => ProTweaks.SetMaxTimerResolutionPersistent(),
                Revert = () => ProTweaks.RemoveTimerResolutionPersistence(),
                GetState = () => TimerResolutionOptimization.IsHighPrecisionTimerActive()
            }, "MaxTimerResolution");

            Register("gpu_irq_priority", new TweakActions
            {
                Apply = () => GpuIRQOptimization.EnableGpuIRQOptimization(),
                Revert = () => GpuIRQOptimization.DisableGpuIRQOptimization(),
                GetState = () => GpuIRQOptimization.GetGpuIRQStatus().isOptimized
            }, "GpuIrqPriority", "GpuIRQ");

            Register("network_throttling_disable", new TweakActions
            {
                Apply = () => NetworkOptimization.OptimizeNetwork(),
                Revert = () => NetworkOptimization.RestoreNetwork(),
                GetState = () => CheckRegistryDWord(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile", "NetworkThrottlingIndex", -1) || CheckRegistryDWord(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile", "NetworkThrottlingIndex", unchecked((int)0xFFFFFFFF))
            }, "NetworkThrottlingDisable");

            Register("csrss_priority", new TweakActions
            {
                Apply = () => AdvancedLatencyTweaks.OptimizeCSRSSPriority(),
                Revert = () => AdvancedLatencyTweaks.RestoreCSRSSPriority(),
                GetState = () => CheckRegistryDWord(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\csrss.exe\PerfOptions", "CpuPriorityClass", 3)
            }, "CsrssPriority");

            Register("gpu_irq_affinity", new TweakActions
            {
                Apply = () => GpuIRQOptimization.EnableGpuIRQOptimization(),
                Revert = () => GpuIRQOptimization.DisableGpuIRQOptimization(),
                GetState = () => GpuIRQOptimization.GetGpuIRQStatus().isOptimized
            }, "GpuIrqAffinity");

            // KERNELOS TOOLBOX INTEGRATION
            Register("text_input_host_disable", new TweakActions
            {
                Apply = () => KernelOSToolboxTweaks.DisableTextInputHost(),
                Revert = () => KernelOSToolboxTweaks.EnableTextInputHost()
            }, "TextInputHostDisable");

            Register("hop_limit_opt", new TweakActions
            {
                Apply = () => KernelOSToolboxTweaks.SetHopLimit(64),
                Revert = () => KernelOSToolboxTweaks.ResetHopLimit(),
                GetState = () => CheckRegistryDWord(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters", "DefaultTTL", 64)
            }, "HopLimitOpt");

            Register("hvci_disable", new TweakActions
            {
                Apply = () => KernelOSToolboxTweaks.DisableHVCI(),
                Revert = () => KernelOSToolboxTweaks.EnableHVCI()
            }, "HvciDisable");

            // FRAME STABILITY & STUTTER REDUCTION
            Register("dwm_latency_opt", new TweakActions
            {
                Apply = () => FrameStabilityTweaks.OptimizeDwmLatency(),
                Revert = () => FrameStabilityTweaks.RevertDwmLatency(),
                GetState = () => CheckRegistryDWord(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Control\GraphicsDrivers", "MaxFrameLatency", 1)
            }, "DwmLatencyOpt");

            Register("nvme_antistutter", new TweakActions
            {
                Apply = () => FrameStabilityTweaks.OptimizeNvmeAntiStutter(),
                Revert = () => FrameStabilityTweaks.RevertNvmeAntiStutter(),
                GetState = () => CheckRegistryDWord(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Power\PowerSettings\0012ee47-9041-4b5d-9b77-535fba8b1442\d7763327-9309-4b9a-b419-977418706e2e", "Attributes", 2)
            }, "NvmeAntiStutter");

            Register("core_parking_disable", new TweakActions
            {
                Apply = () => FrameStabilityTweaks.DisableCoreParkingAndOptimizeScheduling(),
                Revert = () => FrameStabilityTweaks.RestoreCoreParking()
            }, "CoreParkingDisable");

            // ADVANCED GAMING SYSTEM TWEAKS (PHASE 2)
            Register("flip_model_opt", new TweakActions
            {
                Apply = () => AdvancedGamingSystemTweaks.ForceFlipModelAndFso(),
                Revert = () => AdvancedGamingSystemTweaks.RevertFlipModelAndFso(),
                GetState = () => CheckRegistryDWord(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\GameDVR", "GameDVR_FSEBehaviorMode", 2)
            }, "FlipModelOpt");

            Register("ethernet_eee_off", new TweakActions
            {
                Apply = () => AdvancedGamingSystemTweaks.DisableEthernetPowerSaving(),
                Revert = () => AdvancedGamingSystemTweaks.RevertEthernetPowerSaving(),
                GetState = () => { try { using var k = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Class\{4d36e972-e325-11ce-bfc1-08002be10318}"); if (k == null) return false; foreach (var n in k.GetSubKeyNames().Where(s => s.Length == 4 && s.All(char.IsDigit))) { using var sub = k.OpenSubKey(n); if (sub != null && sub.GetValue("*EEE") == null && sub.GetValue("ReduceSpeedOnPowerDown") == null) return true; } return false; } catch { return false; } }
            }, "EthernetEeeOff");

            Register("kernel_paging_opt", new TweakActions
            {
                Apply = () => AdvancedGamingSystemTweaks.OptimizeKernelPagingAndMemory(),
                Revert = () => AdvancedGamingSystemTweaks.RevertKernelPagingAndMemory(),
                GetState = () => CheckRegistryDWord(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", "DisablePagingExecutive", 1)
            }, "KernelPagingOpt");

            Register("audio_low_latency", new TweakActions
            {
                Apply = () => AdvancedGamingSystemTweaks.OptimizeAudioLatencyBuffering(),
                Revert = () => AdvancedGamingSystemTweaks.RevertAudioLatencyBuffering(),
                GetState = () => CheckRegistryDWord(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Audio", "Priority", 10)
            }, "AudioLowLatency");

            // DEEP SYSTEM TWEAKS (PHASE 3)
            Register("msi_mode_enable", new TweakActions
            {
                Apply = () => DeepSystemTweaks.EnableMsiModeForHardware(),
                Revert = () => DeepSystemTweaks.RevertMsiModeForHardware(),
                GetState = () => UsbMsiOptimization.IsUsbMsiEnabled()
            }, "MsiModeEnable");

            Register("memory_compression_off", new TweakActions
            {
                Apply = () => DeepSystemTweaks.DisableMemoryCompression(),
                Revert = () => DeepSystemTweaks.EnableMemoryCompression(),
                GetState = () => { try { var psi = new ProcessStartInfo("powershell", "-NoProfile -Command \"(Get-MMAgent).MemoryCompression\"") { RedirectStandardOutput = true, UseShellExecute = false, CreateNoWindow = true }; using var p = Process.Start(psi); if (p == null) return false; string o = p.StandardOutput.ReadToEnd(); p.WaitForExit(1500); return o.Trim().Equals("False", StringComparison.OrdinalIgnoreCase); } catch { return false; } }
            }, "MemoryCompressionOff");

            Register("hags_enable", new TweakActions
            {
                Apply = () => DeepSystemTweaks.EnableHags(),
                Revert = () => DeepSystemTweaks.DisableHags(),
                GetState = () => GpuOptimization.IsGpuSchedulingEnabled()
            }, "HagsEnable");

            Register("power_throttling_off", new TweakActions
            {
                Apply = () => DeepSystemTweaks.DisablePowerThrottling(),
                Revert = () => DeepSystemTweaks.EnablePowerThrottling(),
                GetState = () => !PowerOptimization.IsPowerThrottlingEnabled()
            }, "PowerThrottlingOff");

            // WINUTIL INTEGRATED TWEAKS (ChrisTitusTech)
            Register("winutil_activity_feed", new TweakActions
            {
                Apply = () => WinUtilTweaks.DisableActivityFeed(),
                Revert = () => WinUtilTweaks.RestoreActivityFeed(),
                GetState = () => WinUtilTweaks.IsActivityFeedDisabled()
            }, "WinUtilActivityFeed");

            Register("winutil_hibernation", new TweakActions
            {
                Apply = () => WinUtilTweaks.DisableHibernation(),
                Revert = () => WinUtilTweaks.RestoreHibernation(),
                GetState = () => WinUtilTweaks.IsHibernationDisabled()
            }, "WinUtilHibernation");

            Register("winutil_end_task", new TweakActions
            {
                Apply = () => WinUtilTweaks.EnableEndTaskOnTaskbar(),
                Revert = () => WinUtilTweaks.DisableEndTaskOnTaskbar(),
                GetState = () => WinUtilTweaks.IsEndTaskOnTaskbarEnabled()
            }, "WinUtilEndTask");

            Register("winutil_wpbt", new TweakActions
            {
                Apply = () => WinUtilTweaks.DisableWpbt(),
                Revert = () => WinUtilTweaks.RestoreWpbt(),
                GetState = () => WinUtilTweaks.IsWpbtDisabled()
            }, "WinUtilWpbt");

            Register("winutil_location", new TweakActions
            {
                Apply = () => WinUtilTweaks.DisableLocationTracking(),
                Revert = () => WinUtilTweaks.RestoreLocationTracking(),
                GetState = () => WinUtilTweaks.IsLocationTrackingDisabled()
            }, "WinUtilLocation");

            Register("winutil_rdp_warnings", new TweakActions
            {
                Apply = () => WinUtilTweaks.DisableRdpUnsignedWarnings(),
                Revert = () => WinUtilTweaks.RestoreRdpUnsignedWarnings(),
                GetState = () => CheckRegistryDWord(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows NT\Terminal Services\Client", "RedirectionWarningDialogVersion", 0)
            }, "WinUtilRdpWarnings");

            Register("winutil_svchost_split", new TweakActions
            {
                Apply = () => WinUtilTweaks.OptimizeSvcHostSplitThreshold(),
                Revert = () => WinUtilTweaks.RestoreSvcHostSplitThreshold(),
                GetState = () => WinUtilTweaks.IsSvcHostSplitThresholdOptimized()
            }, "WinUtilSvcHostSplit");

            Register("winutil_brave_debloat", new TweakActions
            {
                Apply = () => WinUtilTweaks.DebloatBraveBrowser(),
                Revert = () => WinUtilTweaks.RestoreBraveBrowser(),
                GetState = () => CheckRegistryDWord(Registry.LocalMachine, @"SOFTWARE\Policies\BraveSoftware\Brave", "BraveRewardsDisabled", 1)
            }, "WinUtilBraveDebloat");

            Register("winutil_edge_debloat", new TweakActions
            {
                Apply = () => WinUtilTweaks.DebloatEdgeBrowser(),
                Revert = () => WinUtilTweaks.RestoreEdgeBrowser(),
                GetState = () => CheckRegistryDWord(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Edge", "EdgeShoppingAssistantEnabled", 0)
            }, "WinUtilEdgeDebloat");

            Register("winutil_remove_widgets", new TweakActions
            {
                Apply = () => WinUtilTweaks.RemoveWindowsWidgets(),
                Revert = () => false,
                GetState = () => CheckRegistryDWord(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Dsh", "AllowNewsAndInterests", 0)
            }, "WinUtilRemoveWidgets");

            // 14. PARAGON TWEAKING UTILITY (PTU) SUITE
            Register("game_shader_cache_clean", new TweakActions
            {
                Apply = () => GameCacheOptimizer.CleanAllShaderCaches().success,
                Revert = () => false,
                GetState = () => false
            }, "GameShaderCacheClean", "ptu_shader_clean");

            Register("game_profiles_latency", new TweakActions
            {
                Apply = () => GameCacheOptimizer.OptimizeGameProfiles(),
                Revert = () => GameCacheOptimizer.RestoreGameProfiles(),
                GetState = () => CheckRegistryDWord(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games", "GPU Priority", 8)
            }, "GameProfilesLatency", "ptu_game_profiles");

            Register("discord_gamer_optimization", new TweakActions
            {
                Apply = () => AppOptimizationTweaks.OptimizeDiscord(),
                Revert = () => AppOptimizationTweaks.RestoreDiscord(),
                GetState = () => false
            }, "DiscordGamerOptimization", "ptu_discord");

            Register("spotify_gamer_optimization", new TweakActions
            {
                Apply = () => AppOptimizationTweaks.OptimizeSpotify(),
                Revert = () => AppOptimizationTweaks.RestoreSpotify(),
                GetState = () => false
            }, "SpotifyGamerOptimization", "ptu_spotify");

            Register("browser_gamer_background", new TweakActions
            {
                Apply = () => AppOptimizationTweaks.OptimizeBrowsersGamingMode(),
                Revert = () => AppOptimizationTweaks.RestoreBrowsersGamingMode(),
                GetState = () => CheckRegistryDWord(Registry.LocalMachine, @"SOFTWARE\Policies\Google\Chrome", "BackgroundModeEnabled", 0) || CheckRegistryDWord(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Edge", "BackgroundModeEnabled", 0)
            }, "BrowserGamerBackground", "ptu_browser_background");

            Register("apex_gaming_power_plan", new TweakActions
            {
                Apply = () => ApexPowerPlanOptimization.ApplyApexPowerPlan(),
                Revert = () => ApexPowerPlanOptimization.RestoreDefaultPowerPlan(),
                GetState = () => IsActivePowerPlanContaining("Apex") || IsActivePowerPlanContaining("Ghost")
            }, "ApexGamingPowerPlan", "ptu_apex_power");

            Register("gpu_driver_telemetry_clean", new TweakActions
            {
                Apply = () => DriverMaintenanceTools.DisableGpuDriverTelemetry(),
                Revert = () => DriverMaintenanceTools.RestoreGpuDriverTelemetry(),
                GetState = () => IsServiceDisabled("NvTelemetryContainer")
            }, "GpuDriverTelemetryClean", "ptu_gpu_telemetry");

            Register("safe_mode_ddu_prep", new TweakActions
            {
                Apply = () => DriverMaintenanceTools.ConfigureSafeModeBoot(),
                Revert = () => DriverMaintenanceTools.RemoveSafeModeBoot(),
                GetState = () => CheckBcdEditSetting("safeboot", "minimal")
            }, "SafeModeDduPrep", "ptu_safe_mode");

            Register("system_file_checker", new TweakActions
            {
                Apply = () => SystemRepairTools.RunSFCScanAsync().GetAwaiter().GetResult(),
                Revert = () => false,
                GetState = () => false
            }, "SystemFileChecker", "ptu_sfc_scan");

            Register("dism_restore_health", new TweakActions
            {
                Apply = () => SystemRepairTools.RunDISMRestoreHealthAsync().GetAwaiter().GetResult(),
                Revert = () => false,
                GetState = () => false
            }, "DismRestoreHealth", "ptu_dism_restore");

            // ────────────────────────────────────────────
            // COMPETITIVE GAMING: FPS & INPUT DELAY TWEAKS
            // ────────────────────────────────────────────
            Register("usb_msi_mode", new TweakActions
            {
                Apply = () => UsbMsiOptimization.EnableUsbMsiMode(),
                Revert = () => UsbMsiOptimization.RestoreUsbMsiMode(),
                GetState = () => UsbMsiOptimization.IsUsbMsiEnabled()
            }, "UsbMsi", "usb_msi_optimization");

            Register("gamebar_presence_mitigation", new TweakActions
            {
                Apply = () => GameBarMitigationTweaks.DisableGameBarHooks(),
                Revert = () => GameBarMitigationTweaks.RestoreGameBarHooks(),
                GetState = () => GameBarMitigationTweaks.IsGameBarMitigated()
            }, "GameBarMitigation", "gamebar_hooks_disable");

            Register("kernel_tsc_sync", new TweakActions
            {
                Apply = () => KernelAdvancedTweaks.OptimizeKernelLatency(),
                Revert = () => KernelAdvancedTweaks.RestoreKernelLatency(),
                GetState = () => KernelAdvancedTweaks.IsKernelOptimized()
            }, "KernelTscSync", "kernel_advanced_latency");

            Register("timer_resolution_05ms", new TweakActions
            {
                Apply = () => TimerResolutionOptimization.EnableHighPrecisionTimer(),
                Revert = () => TimerResolutionOptimization.RestoreTimerResolution(),
                GetState = () => TimerResolutionOptimization.IsHighPrecisionTimerActive()
            }, "TimerResolution", "high_precision_timer");

            // ────────────────────────────────────────────
            // 15. COMPETITIVE ULTRA PERFORMANCE TWEAKS
            // ────────────────────────────────────────────
            Register("page_combining_off", new TweakActions
            {
                Apply = () => CompetitivePerformanceTweaks.DisablePageCombining(),
                Revert = () => CompetitivePerformanceTweaks.RevertPageCombining(),
                GetState = () => CompetitivePerformanceTweaks.IsPageCombiningDisabled()
            }, "PageCombiningOff");

            Register("tsc_sync_enhanced", new TweakActions
            {
                Apply = () => CompetitivePerformanceTweaks.EnableTscInvariantClock(),
                Revert = () => CompetitivePerformanceTweaks.RevertTscInvariantClock(),
                GetState = () => CompetitivePerformanceTweaks.IsTscInvariantClockEnabled()
            }, "TscSyncEnhanced");

            Register("quantum_gaming_priority", new TweakActions
            {
                Apply = () => CompetitivePerformanceTweaks.OptimizeGamingQuantum(),
                Revert = () => CompetitivePerformanceTweaks.RevertGamingQuantum(),
                GetState = () => CompetitivePerformanceTweaks.IsGamingQuantumOptimized()
            }, "QuantumGamingPriority");

            Register("network_rss_queues", new TweakActions
            {
                Apply = () => CompetitivePerformanceTweaks.OptimizeRssQueues(),
                Revert = () => CompetitivePerformanceTweaks.RevertRssQueues(),
                GetState = () => CompetitivePerformanceTweaks.IsRssQueuesOptimized()
            }, "NetworkRssQueues");

            Register("autologgers_diag_off", new TweakActions
            {
                Apply = () => CompetitivePerformanceTweaks.DisableDiagnosticAutoLoggers(),
                Revert = () => CompetitivePerformanceTweaks.RevertDiagnosticAutoLoggers(),
                GetState = () => CompetitivePerformanceTweaks.AreDiagnosticAutoLoggersDisabled()
            }, "AutologgersDiagOff");

            Register("shader_cache_unlimited", new TweakActions
            {
                Apply = () => CompetitivePerformanceTweaks.OptimizeShaderCacheSize(),
                Revert = () => CompetitivePerformanceTweaks.RevertShaderCacheSize(),
                GetState = () => CompetitivePerformanceTweaks.IsShaderCacheSizeOptimized()
            }, "ShaderCacheUnlimited");

            Register("fth_disable", new TweakActions
            {
                Apply = () => CompetitivePerformanceTweaks.DisableFaultTolerantHeap(),
                Revert = () => CompetitivePerformanceTweaks.RevertFaultTolerantHeap(),
                GetState = () => CompetitivePerformanceTweaks.IsFaultTolerantHeapDisabled()
            }, "FthDisable");

            Register("system_sleep_states_opt", new TweakActions
            {
                Apply = () => CompetitivePerformanceTweaks.DisableUsbSelectiveSuspend(),
                Revert = () => CompetitivePerformanceTweaks.RevertUsbSelectiveSuspend(),
                GetState = () => CompetitivePerformanceTweaks.IsUsbSelectiveSuspendDisabled()
            }, "SystemSleepStatesOpt");

            Register("tcp_timestamps_sack_opt", new TweakActions
            {
                Apply = () => CompetitivePerformanceTweaks.OptimizeTcpTimestampsAndSack(),
                Revert = () => CompetitivePerformanceTweaks.RevertTcpTimestampsAndSack(),
                GetState = () => CompetitivePerformanceTweaks.IsTcpTimestampsDisabled()
            }, "TcpTimestampsSackOpt");

            Register("system_responsiveness_extreme", new TweakActions
            {
                Apply = () => CompetitivePerformanceTweaks.OptimizeMultimediaExtreme(),
                Revert = () => CompetitivePerformanceTweaks.RevertMultimediaExtreme(),
                GetState = () => CompetitivePerformanceTweaks.IsMultimediaExtremeOptimized()
            }, "SystemResponsivenessExtreme");

            // POST-INSTALL SCRIPTS PACK (KernelOS ported tweaks)
            Register("mpo_disable", new TweakActions
            {
                Apply = () => CompetitivePerformanceTweaks.DisableMpo(),
                Revert = () => CompetitivePerformanceTweaks.RevertMpo(),
                GetState = () => CompetitivePerformanceTweaks.IsMpoDisabled()
            }, "MpoDisable");

            Register("lazy_mode_timeout", new TweakActions
            {
                Apply = () => CompetitivePerformanceTweaks.SetLazyModeTimeout(),
                Revert = () => CompetitivePerformanceTweaks.RevertLazyModeTimeout(),
                GetState = () => CompetitivePerformanceTweaks.IsLazyModeTimeoutSet()
            }, "LazyModeTimeout");

            Register("thread_dpc_disable", new TweakActions
            {
                Apply = () => CompetitivePerformanceTweaks.DisableThreadDpc(),
                Revert = () => CompetitivePerformanceTweaks.RevertThreadDpc(),
                GetState = () => CompetitivePerformanceTweaks.IsThreadDpcDisabled()
            }, "ThreadDpcDisable");

            Register("io_latency_cap", new TweakActions
            {
                Apply = () => CompetitivePerformanceTweaks.SetIoLatencyCap(),
                Revert = () => CompetitivePerformanceTweaks.RevertIoLatencyCap(),
                GetState = () => CompetitivePerformanceTweaks.IsIoLatencyCapSet()
            }, "IoLatencyCap");

            Register("driver_ppm_disable", new TweakActions
            {
                Apply = () => CompetitivePerformanceTweaks.DisableDriverPpm(),
                Revert = () => CompetitivePerformanceTweaks.RevertDriverPpm(),
                GetState = () => CompetitivePerformanceTweaks.IsDriverPpmDisabled()
            }, "DriverPpmDisable");

            Register("cpu_idle_disable", new TweakActions
            {
                Apply = () => CompetitivePerformanceTweaks.DisableCpuIdle(),
                Revert = () => CompetitivePerformanceTweaks.RevertCpuIdle(),
                GetState = () => CompetitivePerformanceTweaks.IsCpuIdleDisabled()
            }, "CpuIdleDisable");

            Register("nic_buffers_2048", new TweakActions
            {
                Apply = () => CompetitivePerformanceTweaks.SetNicBuffers2048(),
                Revert = () => CompetitivePerformanceTweaks.RevertNicBuffers(),
                GetState = () => CompetitivePerformanceTweaks.IsNicBuffers2048Set()
            }, "NicBuffers2048");

            Register("vulnerable_driver_blocklist_off", new TweakActions
            {
                Apply = () => CompetitivePerformanceTweaks.DisableVulnerableDriverBlocklist(),
                Revert = () => CompetitivePerformanceTweaks.RevertVulnerableDriverBlocklist(),
                GetState = () => CompetitivePerformanceTweaks.IsVulnerableDriverBlocklistDisabled()
            }, "VulnerableDriverBlocklistOff");
        }

        public bool ApplyTweak(string tweakId)
        {
            if (_mappings.TryGetValue(tweakId, out var actions))
            {
                try { return actions.Apply(); }
                catch (Exception ex) { Debug.WriteLine($"[Dispatcher] Error al aplicar {tweakId}: {ex.Message}"); return false; }
            }
            Debug.WriteLine($"[Dispatcher] Tweak ID no soportado: {tweakId}");
            return false;
        }

        public bool RevertTweak(string tweakId)
        {
            if (_mappings.TryGetValue(tweakId, out var actions))
            {
                try { return actions.Revert(); }
                catch (Exception ex) { Debug.WriteLine($"[Dispatcher] Error al revertir {tweakId}: {ex.Message}"); return false; }
            }
            return false;
        }

        public bool IsTweakSupported(string tweakId) => _mappings.ContainsKey(tweakId);

        public bool? GetRealState(string tweakId)
        {
            if (_mappings.TryGetValue(tweakId, out var actions))
            {
                try { return actions.GetState(); }
                catch { return null; }
            }
            return null;
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<string> GetCanonicalTweakIds() => _canonicalIds;

        public string? GetCanonicalId(string tweakId)
        {
            if (string.IsNullOrEmpty(tweakId)) return null;
            if (_aliasToCanonical.TryGetValue(tweakId, out var canonical))
                return canonical;
            return tweakId;
        }

        public IReadOnlyList<string> GetAliases(string tweakId)
        {
            var can = GetCanonicalId(tweakId);
            if (can != null && _canonicalToAliases.TryGetValue(can, out var list))
                return list;
            return Array.Empty<string>();
        }
    }
}
