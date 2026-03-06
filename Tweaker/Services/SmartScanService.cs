using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Win32;

using Tweaker.Data;
using Tweaker.Optimizations;

namespace Tweaker.Services
{
    public class ScanResult
    {
        public string? TweakId { get; set; }
        public string? Title { get; set; }
        public bool IsOptimized { get; set; }
        public string? Category { get; set; }
    }

    public class SmartScanService
    {
        private readonly List<string> _scannableTweakIds = new List<string>
        {
            "mouse_acceleration",
            "keyboard_optimization",
            "visual_effects",
            "transparency_effects",
            "sticky_keys",
            "gamedvr_disable",
            "high_performance",
            "system_profile"
        };

        public async Task<List<ScanResult>> ScanAsync()
        {
            // Simular un delay para que parezca que está analizando
            await Task.Delay(1500);

            var results = new List<ScanResult>();

            foreach (var id in _scannableTweakIds)
            {
                var info = TweaksDatabase.GetTweakInfo(id)!;
                bool isOptimized = CheckTweakStatus(id);

                results.Add(new ScanResult
                {
                    TweakId = id,
                    Title = info.Title,
                    IsOptimized = isOptimized,
                    Category = info.Category
                });
            }

            return results;
        }

        private bool CheckTweakStatus(string tweakId)
        {
            try
            {
                switch (tweakId)
                {
                    case "mouse_acceleration":
                        return IsRegistryValueEqual(Registry.CurrentUser, @"Control Panel\Mouse", "MouseSpeed", "0");

                    case "keyboard_optimization":
                        return IsRegistryValueEqual(Registry.CurrentUser, @"Control Panel\Keyboard", "KeyboardDelay", "0");

                    case "visual_effects":
                        return IsRegistryValueEqual(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects", "VisualFXSetting", 2);

                    case "transparency_effects":
                        return IsRegistryValueEqual(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "EnableTransparency", 0);

                    case "sticky_keys":
                        // Simplificado para el ejemplo
                        return IsRegistryValueEqual(Registry.CurrentUser, @"Control Panel\Accessibility\StickyKeys", "Flags", "506");

                    case "gamedvr_disable":
                        return IsRegistryValueEqual(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\GameDVR", "AppCaptureEnabled", 0);

                    case "high_performance":
                        return CpuOptimization.IsHighPerformanceActive();

                    case "system_profile":
                        return IsRegistryValueEqual(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile", "NetworkThrottlingIndex", -1);

                    default:
                        return false;
                }
            }
            catch
            {
                return false;
            }
        }

        private bool IsRegistryValueEqual(RegistryKey hive, string keyPath, string valueName, object expectedValue)
        {
            try
            {
                using (var key = hive.OpenSubKey(keyPath))
                {
                    if (key == null) return false;
                    var value = key.GetValue(valueName);
                    if (value == null) return false;
                    return value.ToString().Equals(expectedValue.ToString(), StringComparison.OrdinalIgnoreCase);
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task ApplyRecommendedAsync(List<string> tweakIds)
        {
            foreach (var id in tweakIds)
            {
                // Aplicar cada tweak usando las clases existentes
                switch (id)
                {
                    case "mouse_acceleration": MouseTweaks.Apply(); break;
                    case "keyboard_optimization": KeyboardOptimization.OptimizeKeyboard(); break;
                    case "visual_effects": VisualOptimization.OptimizeVisuals(); break;
                    case "transparency_effects": VisualOptimization.DisableTransparency(); break;
                    case "sticky_keys": InputTweaks.DisableStickyKeys(); break;
                    case "gamedvr_disable": GpuOptimization.DisableGameDVR(); break;
                    case "high_performance": CpuOptimization.EnableHighPerformancePowerPlan(); break;
                    case "system_profile": GpuOptimization.EnableSystemProfileOptimization(); break;
                }
                await Task.Delay(200);
            }
        }
    }
}
