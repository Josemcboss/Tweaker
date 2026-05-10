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

        public TweakDispatcher()
        {
            _mappings = new Dictionary<string, TweakActions>();
            InitializeMappings();
        }

        private void InitializeMappings()
        {
            // --- Input & Visuals ---
            _mappings["mouse_acceleration"] = new TweakActions { 
                Apply = () => MouseOptimization.DisableAcceleration(), 
                Revert = () => MouseOptimization.EnableAcceleration(),
                GetState = () => MouseOptimization.IsAccelerationDisabled()
            };
            _mappings["keyboard_optimization"] = new TweakActions { 
                Apply = () => KeyboardOptimization.OptimizeKeyboard(), 
                Revert = () => KeyboardOptimization.RestoreKeyboard() 
            };
            _mappings["visual_effects"] = new TweakActions { 
                Apply = () => VisualOptimization.DisableVisualEffects(), 
                Revert = () => VisualOptimization.EnableVisualEffects() 
            };
            _mappings["memory_optimization"] = new TweakActions { 
                Apply = () => MemoryTweaks.OptimizeMemory(), 
                Revert = () => MemoryTweaks.RestoreMemory() 
            };
            _mappings["transparency_effects"] = new TweakActions { 
                Apply = () => VisualOptimization.DisableTransparency(), 
                Revert = () => VisualOptimization.EnableTransparency() 
            };

            // --- Red & Ping ---
            _mappings["network_optimization"] = new TweakActions { 
                Apply = () => NetworkOptimization.OptimizeNetwork(), 
                Revert = () => NetworkOptimization.RestoreNetwork() 
            };
            _mappings["dns_cloudflare"] = new TweakActions { 
                Apply = () => DnsOptimization.SetCloudflareDns().Contains("✅"), 
                Revert = () => NetworkOptimization.RestoreNetwork() 
            };
            _mappings["dns_google"] = new TweakActions { 
                Apply = () => DnsOptimization.SetGoogleDns().Contains("✅"), 
                Revert = () => NetworkOptimization.RestoreNetwork() 
            };

            // --- Sistema & GPU ---
            _mappings["system_profile"] = new TweakActions { 
                Apply = () => GpuOptimization.EnableSystemProfileOptimization(), 
                Revert = () => GpuOptimization.DisableSystemProfileOptimization() 
            };
            _mappings["system_responsiveness"] = new TweakActions { 
                Apply = () => CpuOptimization.EnableSystemResponsivenessOptimization(), 
                Revert = () => CpuOptimization.DisableSystemResponsivenessOptimization() 
            };
            _mappings["gamedvr_disable"] = new TweakActions { 
                Apply = () => GpuOptimization.DisableGameDVR(), 
                Revert = () => GpuOptimization.EnableGameDVR(),
                GetState = () => !WindowsDebloat.IsGameDVREnabled()
            };
            _mappings["gpu_scheduling"] = new TweakActions { 
                Apply = () => NvidiaOptimization.OptimizeNvidiaGPU(), 
                Revert = () => NvidiaOptimization.RestoreNvidiaGPU(),
                GetState = () => GpuOptimization.IsGpuSchedulingEnabled()
            };
            _mappings["mpo_fix"] = new TweakActions { 
                Apply = () => GpuTweaks.DisableMPO(), 
                Revert = () => GpuTweaks.EnableMPO(),
                GetState = () => GpuTweaks.IsMPODisabled()
            };
            _mappings["ultimate_power"] = new TweakActions { 
                Apply = () => PowerTweaks.EnableUltimatePerformance(), 
                Revert = () => PowerTweaks.RestoreBalancedPlan() 
            };
            _mappings["high_performance_plan"] = new TweakActions { 
                Apply = () => CpuOptimization.EnableHighPerformancePowerPlan(), 
                Revert = () => PowerTweaks.RestoreBalancedPlan(),
                GetState = () => CpuOptimization.IsHighPerformanceActive()
            };
            _mappings["power_throttling_disable"] = new TweakActions { 
                Apply = () => CpuOptimization.DisablePowerThrottling(), 
                Revert = () => CpuOptimization.EnablePowerThrottling(),
                GetState = () => !PowerOptimization.IsPowerThrottlingEnabled()
            };
            _mappings["core_parking_disable"] = new TweakActions { 
                Apply = () => CpuOptimization.DisableCoreParking(), 
                Revert = () => CpuOptimization.EnableCoreParking() 
            };
            
            // --- Ghost Pack & Advanced ---
            _mappings["core_isolation"] = new TweakActions { 
                Apply = () => WindowsDebloat.DisableCoreIsolation(), 
                Revert = () => WindowsDebloat.EnableCoreIsolation(),
                GetState = () => !WindowsDebloat.IsVBSEnabled()
            };
            _mappings["hpet_optimization"] = new TweakActions { 
                Apply = () => KernelTweaks.OptimizeHPET(), 
                Revert = () => KernelTweaks.RestoreHPET() 
            };
            _mappings["hyperv_disable"] = new TweakActions { 
                Apply = () => LatencyOptimization.DisableHyperV(), 
                Revert = () => LatencyOptimization.EnableHyperV() 
            };
            _mappings["gamebar_disable"] = new TweakActions { 
                Apply = () => WindowsDebloat.DisableGameBar(), 
                Revert = () => WindowsDebloat.EnableGameBar(),
                GetState = () => GamingOptimization.IsGameBarDisabled()
            };
            _mappings["spectre_meltdown_disable"] = new TweakActions { 
                Apply = () => AdvancedTweaks.DisableSpectreMeltdown(), 
                Revert = () => AdvancedTweaks.EnableSpectreMeltdown() 
            };

            // --- Limpieza & Servicios ---
            _mappings["hibernation_disable"] = new TweakActions { 
                Apply = () => WindowsOptimization.DisableHibernation(), 
                Revert = () => WindowsOptimization.EnableHibernation(),
                GetState = () => !PowerOptimization.IsHibernationEnabled()
            };
            _mappings["windows_search_disable"] = new TweakActions { 
                Apply = () => WindowsOptimization.DisableWindowsSearch(), 
                Revert = () => WindowsOptimization.EnableWindowsSearch() 
            };
            _mappings["sysmain_disable"] = new TweakActions { 
                Apply = () => WindowsOptimization.DisableSysMain(), 
                Revert = () => WindowsOptimization.EnableSysMain() 
            };
            _mappings["telemetry_disable"] = new TweakActions { 
                Apply = () => WindowsOptimization.DisableTelemetry(), 
                Revert = () => WindowsOptimization.EnableTelemetry() 
            };
            _mappings["diagtrack_disable"] = new TweakActions { 
                Apply = () => ServiceOptimization.DisableDiagTrack(), 
                Revert = () => ServiceOptimization.EnableDiagTrack() 
            };
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
    }
}
