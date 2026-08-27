using System;
using System.Diagnostics;
using Tweaker.Optimizations;

namespace Tweaker.Presets
{
    /// <summary>
    /// PRESET PARAGON COMPETITIVE (PTU Inspired)
    /// Aplica el conjunto completo de mejoras, limpieza de shaders y optimizaciones de bajo input lag de Paragon Tweaking Utility.
    /// </summary>
    public static class PTUPreset
    {
        public static bool ApplyPTUPreset()
        {
            try
            {
                Debug.WriteLine("────────────────────────────────────────────");
                Debug.WriteLine("PARAGON COMPETITIVE PRESET (PTU Inspired)");
                Debug.WriteLine("────────────────────────────────────────────");

                bool overallSuccess = true;
                int count = 0;

                // 1. Limpieza de Shader Caches
                var (cacheOk, bytes, files) = GameCacheOptimizer.CleanAllShaderCaches();
                if (cacheOk) count++;

                // 2. Perfiles de Juegos y Latencia
                if (GameCacheOptimizer.OptimizeGameProfiles()) count++;
                else overallSuccess = false;

                // 3. Optimización de Apps de Fondo (Discord, Spotify, Navegadores)
                if (AppOptimizationTweaks.OptimizeDiscord()) count++;
                if (AppOptimizationTweaks.OptimizeSpotify()) count++;
                if (AppOptimizationTweaks.OptimizeBrowsersGamingMode()) count++;

                // 4. Plan de Energía Apex / Ultra Gaming
                if (ApexPowerPlanOptimization.ApplyApexPowerPlan()) count++;
                else overallSuccess = false;

                // 5. Eliminación de Telemetría de Drivers GPU
                if (DriverMaintenanceTools.DisableGpuDriverTelemetry()) count++;

                Debug.WriteLine($"✓ PTU Preset completado: {count} optimizaciones aplicadas.");
                return overallSuccess;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error aplicando PTU Preset: {ex.Message}");
                return false;
            }
        }
    }
}
