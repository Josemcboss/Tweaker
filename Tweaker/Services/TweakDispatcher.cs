using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public TweakDispatcher()
        {
            _mappings = new Dictionary<string, TweakActions>(StringComparer.OrdinalIgnoreCase);
            _canonicalIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
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
            foreach (var alias in aliases)
                _mappings[alias] = actions;
        }

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
                Revert = () => KeyboardOptimization.RestoreKeyboard()
            }, "Keyboard");

            Register("visual_effects", new TweakActions
            {
                Apply = () => VisualOptimization.DisableVisualEffects(),
                Revert = () => VisualOptimization.EnableVisualEffects()
            }, "VisualEffects");

            Register("memory_optimization", new TweakActions
            {
                Apply = () => MemoryTweaks.OptimizeMemory(),
                Revert = () => MemoryTweaks.RestoreMemory()
            }, "MemoryOptimization");

            Register("transparency_effects", new TweakActions
            {
                Apply = () => VisualOptimization.DisableTransparency(),
                Revert = () => VisualOptimization.EnableTransparency()
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
                Revert = () => AdvancedLatencyTweaks.RestoreMenuShowDelay()
            }, "MenuShowDelay");

            Register("data_queue_sizes", new TweakActions
            {
                Apply = () => AdvancedLatencyTweaks.OptimizeDataQueueSizes(),
                Revert = () => AdvancedLatencyTweaks.RestoreDataQueueSizes()
            }, "DataQueueSizes");

            // 2. RED & PING
            Register("network_optimization", new TweakActions
            {
                Apply = () => NetworkOptimization.OptimizeNetwork(),
                Revert = () => NetworkOptimization.RestoreNetwork()
            }, "NetworkOptimization");

            Register("dns_cloudflare", new TweakActions
            {
                Apply = () => DnsOptimization.SetCloudflareDns().Contains("\u2705"),
                Revert = () => NetworkOptimization.RestoreNetwork()
            }, "DnsCloudflare");

            Register("dns_google", new TweakActions
            {
                Apply = () => DnsOptimization.SetGoogleDns().Contains("\u2705"),
                Revert = () => NetworkOptimization.RestoreNetwork()
            }, "DnsGoogle");

            Register("dns_cache", new TweakActions
            {
                Apply = () => DnsOptimization.EnableDnsCacheOptimization().StartsWith("\u2705"),
                Revert = () => DnsOptimization.DisableDnsCacheOptimization().StartsWith("\u2705")
            }, "DnsCache");

            Register("ecn_capability", new TweakActions
            {
                Apply = () => AntiBufferbloatTweaks.EnableECN(),
                Revert = () => AntiBufferbloatTweaks.DisableECN()
            }, "EcnCapability");

            Register("tcp_congestion", new TweakActions
            {
                Apply = () => AntiBufferbloatTweaks.SetCubicCongestion(),
                Revert = () => AntiBufferbloatTweaks.RevertCongestion()
            }, "TcpCongestion");

            Register("disable_lso", new TweakActions
            {
                Apply = () => AntiBufferbloatTweaks.DisableLSO(),
                Revert = () => AntiBufferbloatTweaks.EnableLSO()
            }, "DisableLso");

            Register("qos_prioritization", new TweakActions
            {
                Apply = () => AntiBufferbloatTweaks.EnableQoS_Prioritization(),
                Revert = () => AntiBufferbloatTweaks.DisableQoS_Prioritization()
            }, "QosPrioritization");

            Register("tcp_autotuning", new TweakActions
            {
                Apply = () => AntiBufferbloatTweaks.RestrictAutoTuning(),
                Revert = () => AntiBufferbloatTweaks.NormalAutoTuning()
            }, "TcpAutoTuning");

            Register("interrupt_moderation", new TweakActions
            {
                Apply = () => AdvancedLatencyTweaks.DisableInterruptModeration(),
                Revert = () => AdvancedLatencyTweaks.RestoreInterruptModeration()
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
                Revert = () => DnsOptimization.EnableNetBios().StartsWith("\u2705")
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
                Revert = () => GpuOptimization.DisableSystemProfileOptimization()
            }, "SystemProfile");

            Register("system_responsiveness", new TweakActions
            {
                Apply = () => CpuOptimization.EnableSystemResponsivenessOptimization(),
                Revert = () => CpuOptimization.DisableSystemResponsivenessOptimization()
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
                Revert = () => CpuOptimization.EnableCoreParking()
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
                Revert = () => KernelTweaks.RestoreHPET()
            }, "HPET");

            Register("hyperv_disable", new TweakActions
            {
                Apply = () => LatencyOptimization.DisableHyperV(),
                Revert = () => LatencyOptimization.EnableHyperV()
            }, "HyperV");

            Register("spectre_meltdown_disable", new TweakActions
            {
                Apply = () => AdvancedTweaks.DisableSpectreMeltdown(),
                Revert = () => AdvancedTweaks.EnableSpectreMeltdown()
            }, "SpectreMeltdown");

            Register("hibernation_disable", new TweakActions
            {
                Apply = () => WindowsOptimization.DisableHibernation(),
                Revert = () => WindowsOptimization.EnableHibernation(),
                GetState = () => !PowerOptimization.IsHibernationEnabled()
            }, "Hibernation", "hibernation_wipe", "hibernate_disable");

            Register("windows_search_disable", new TweakActions
            {
                Apply = () => WindowsOptimization.DisableWindowsSearch(),
                Revert = () => WindowsOptimization.EnableWindowsSearch()
            }, "WindowsSearch");

            Register("sysmain_disable", new TweakActions
            {
                Apply = () => WindowsOptimization.DisableSysMain(),
                Revert = () => WindowsOptimization.EnableSysMain()
            }, "SysMain");

            Register("telemetry_disable", new TweakActions
            {
                Apply = () => WindowsOptimization.DisableTelemetry(),
                Revert = () => WindowsOptimization.EnableTelemetry()
            }, "Telemetry");

            Register("diagtrack_disable", new TweakActions
            {
                Apply = () => ServiceOptimization.DisableDiagTrack(),
                Revert = () => ServiceOptimization.EnableDiagTrack()
            }, "DiagTrack");

            // 5. GPU & DISPLAY
            Register("nvidia_low_latency_ultra", new TweakActions
            {
                Apply = () => NvidiaOptimization.OptimizeNvidiaGPU(),
                Revert = () => NvidiaOptimization.RestoreNvidiaGPU()
            }, "NvidiaLowLatencyUltra");

            Register("shader_cache_disable", new TweakActions
            {
                Apply = () => GpuTweaks.DisableMPO(),
                Revert = () => GpuTweaks.EnableMPO()
            }, "ShaderCacheDisable");

            Register("gpu_hw_scheduling_v2", new TweakActions
            {
                Apply = () => NvidiaOptimization.OptimizeNvidiaGPU(),
                Revert = () => NvidiaOptimization.RestoreNvidiaGPU()
            }, "GpuHwSchedulingV2");

            Register("disable_ipv6", new TweakActions
            {
                Apply = () => NetworkOptimization.OptimizeNetwork(),
                Revert = () => NetworkOptimization.RestoreNetwork()
            }, "DisableIpv6");

            Register("wasapi_exclusive_mode", new TweakActions
            {
                Apply = () => AudioOptimization.OptimizeAudioLatency(),
                Revert = () => AudioOptimization.RestoreAudioSettings()
            }, "WasapiExclusiveMode");

            Register("audio_enhancements_off", new TweakActions
            {
                Apply = () => AudioOptimization.OptimizeAudioLatency(),
                Revert = () => AudioOptimization.RestoreAudioSettings()
            }, "AudioEnhancementsOff");

            // 6. STORAGE
            Register("ssd_write_cache", new TweakActions
            {
                Apply = () => DiskTweaks.EnableTrim(),
                Revert = () => DiskTweaks.DisableTrim()
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
                Revert = () => DiskTweaks.DisableTrim()
            }, "NtfsMftZone");

            // 7. CPU ADVANCED & LOW-LEVEL LATENCY
            Register("irq_network_priority", new TweakActions
            {
                Apply = () => AdvancedLatencyTweaks.DisableInterruptModeration(),
                Revert = () => AdvancedLatencyTweaks.RestoreInterruptModeration()
            }, "IrqNetworkPriority");

            Register("nagle_algorithm_off", new TweakActions
            {
                Apply = () => NetworkOptimization.OptimizeNetwork(),
                Revert = () => NetworkOptimization.RestoreNetwork()
            }, "NagleAlgorithmOff");

            Register("cpu_affinity_auto_gaming", new TweakActions
            {
                Apply = () => CpuOptimization.DisableCoreParking(),
                Revert = () => CpuOptimization.EnableCoreParking()
            }, "CpuAffinityAutoGaming");

            Register("cpu_anti_throttling", new TweakActions
            {
                Apply = () => CpuOptimization.DisablePowerThrottling(),
                Revert = () => CpuOptimization.EnablePowerThrottling()
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
                Revert = () => true
            }, "TempFilesCleanup");

            Register("win_update_cache", new TweakActions
            {
                Apply = () => AdvancedSystemTweaks.CleanWindowsUpdateCache().success,
                Revert = () => true
            }, "WinUpdateCache");

            Register("max_timer_resolution", new TweakActions
            {
                Apply = () => ProTweaks.SetMaxTimerResolutionPersistent(),
                Revert = () => ProTweaks.RemoveTimerResolutionPersistence()
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
                Revert = () => NetworkOptimization.RestoreNetwork()
            }, "NetworkThrottlingDisable");

            Register("csrss_priority", new TweakActions
            {
                Apply = () => AdvancedLatencyTweaks.OptimizeCSRSSPriority(),
                Revert = () => AdvancedLatencyTweaks.RestoreCSRSSPriority()
            }, "CsrssPriority");

            Register("gpu_irq_affinity", new TweakActions
            {
                Apply = () => GpuIRQOptimization.EnableGpuIRQOptimization(),
                Revert = () => GpuIRQOptimization.DisableGpuIRQOptimization(),
                GetState = () => GpuIRQOptimization.GetGpuIRQStatus().isOptimized
            }, "GpuIrqAffinity");
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
    }
}
