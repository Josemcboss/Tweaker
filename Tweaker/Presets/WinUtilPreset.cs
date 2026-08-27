using System;
using System.Diagnostics;
using Tweaker.Optimizations;

namespace Tweaker.Presets
{
    /// <summary>
    /// PRESET DE OPTIMIZACIONES DE WINUTIL (ChrisTitusTech)
    /// Aplica las optimizaciones esenciales del proyecto WinUtil para un sistema limpio y rápido.
    /// </summary>
    public static class WinUtilPreset
    {
        public static bool ApplyWinUtilPreset()
        {
            try
            {
                Debug.WriteLine("────────────────────────────");
                Debug.WriteLine("WINUTIL ESSENTIAL PRESET");
                Debug.WriteLine("────────────────────────────");

                bool overallSuccess = true;
                int count = 0;

                if (WinUtilTweaks.DisableActivityFeed()) count++;
                else overallSuccess = false;

                if (WinUtilTweaks.DisableHibernation()) count++;
                else overallSuccess = false;

                if (WinUtilTweaks.EnableEndTaskOnTaskbar()) count++;
                else overallSuccess = false;

                if (WinUtilTweaks.DisableWpbt()) count++;
                else overallSuccess = false;

                if (WinUtilTweaks.DisableLocationTracking()) count++;
                else overallSuccess = false;

                if (WinUtilTweaks.DisableRdpUnsignedWarnings()) count++;
                else overallSuccess = false;

                if (WinUtilTweaks.OptimizeSvcHostSplitThreshold()) count++;
                else overallSuccess = false;

                if (WinUtilTweaks.DebloatEdgeBrowser()) count++;
                else overallSuccess = false;

                if (WinUtilTweaks.DebloatBraveBrowser()) count++;
                else overallSuccess = false;

                WinUtilTweaks.RemoveWindowsWidgets(); // Opcional / Best effort

                Debug.WriteLine($"WinUtil Preset completado: {count} tweaks aplicados.");
                return overallSuccess;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error aplicando WinUtil Preset: {ex.Message}");
                return false;
            }
        }
    }
}
