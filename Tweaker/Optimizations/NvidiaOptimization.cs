using System;
using System.Diagnostics;

using Microsoft.Win32;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// Optimizaciones específicas para GPUs NVIDIA GeForce
    /// Mejora rendimiento en gaming ajustando valores del registro del driver NVIDIA
    ///
    /// ADVERTENCIA: Estos tweaks modifican claves del registro del controlador NVIDIA.
    /// Asegúrate de tener los drivers NVIDIA instalados antes de aplicar.
    /// </summary>
    public static class NvidiaOptimization
    {
        // Clase de dispositivo para adaptadores de display (GPUs)
        private const string GPU_CLASS_KEY = @"SYSTEM\CurrentControlSet\Control\Class\{4d36e968-e325-11ce-bfc1-08002be10318}";

        // Ruta del panel de control NVIDIA (perfil global)
        private const string NVIDIA_PROFILE_KEY = @"SOFTWARE\NVIDIA Corporation\Global\NVTweak";

        // Ruta de configuración de threading NVIDIA
        private const string NVIDIA_THREADING_KEY = @"SYSTEM\CurrentControlSet\Services\nvlddmkm\Global\NVTweak";

        /// <summary>
        /// Aplica optimizaciones de rendimiento para GPU NVIDIA GeForce:
        ///
        /// - RMAEnable = 0:         Deshabilita Remote Memory Architecture (overhead innecesario)
        /// - PerfLevelSrc = 0x2222: Fuerza modo High Performance constante
        /// - DisableDynamicPstate = 1: Deshabilita P-States dinámicos (evita downclock)
        /// - PowerMizerEnable = 1:  Habilita PowerMizer en modo máximo rendimiento
        /// - PowerMizerLevel = 1:   Fuerza nivel de rendimiento máximo
        /// - PowerMizerLevelAC = 1: Idem cuando está conectado a la corriente
        ///
        /// IMPACTO EN GAMING:
        /// - Elimina micro-stutters por cambios de clock dinámico
        /// - GPU siempre a máxima frecuencia
        /// - Reduce frame time variance en escenas intensas
        /// </summary>
        public static bool OptimizeNvidiaGPU()
        {
            try
            {
                Debug.WriteLine("─");
                Debug.WriteLine("NVIDIA GPU OPTIMIZATION - Optimizando GPU NVIDIA GeForce");
                Debug.WriteLine("─");

                bool anyApplied = false;

                using (RegistryKey classKey = Registry.LocalMachine.OpenSubKey(GPU_CLASS_KEY, true))
                {
                    if (classKey == null)
                    {
                        Debug.WriteLine("⚠ No se encontró la clave de clase de GPU");
                        return false;
                    }

                    foreach (string subKeyName in classKey.GetSubKeyNames())
                    {
                        if (!IsNumericSubKey(subKeyName))
                            continue;

                        using (RegistryKey deviceKey = classKey.OpenSubKey(subKeyName, true))
                        {
                            if (deviceKey == null)
                                continue;

                            object driverDesc = deviceKey.GetValue("DriverDesc");
                            string driverDescription = driverDesc?.ToString() ?? "";

                            if (!IsNvidiaDevice(driverDescription))
                                continue;

                            Debug.WriteLine($"   → GPU encontrada: {driverDescription} (clave: {subKeyName})");

                            // Deshabilitar Remote Memory Architecture - reduce overhead
                            deviceKey.SetValue("RMAEnable", 0, RegistryValueKind.DWord);
                            Debug.WriteLine("   ✓ RMAEnable = 0 (RMA deshabilitado)");

                            // Forzar High Performance P-State
                            deviceKey.SetValue("PerfLevelSrc", 0x2222, RegistryValueKind.DWord);
                            Debug.WriteLine("   ✓ PerfLevelSrc = 0x2222 (High Performance forzado)");

                            // Deshabilitar P-States dinámicos (evita downclocking en gaming)
                            deviceKey.SetValue("DisableDynamicPstate", 1, RegistryValueKind.DWord);
                            Debug.WriteLine("   ✓ DisableDynamicPstate = 1 (P-States dinámicos OFF)");

                            // PowerMizer: forzar máximo rendimiento
                            deviceKey.SetValue("PowerMizerEnable", 1, RegistryValueKind.DWord);
                            deviceKey.SetValue("PowerMizerLevel", 1, RegistryValueKind.DWord);
                            deviceKey.SetValue("PowerMizerLevelAC", 1, RegistryValueKind.DWord);
                            Debug.WriteLine("   ✓ PowerMizer = Máximo rendimiento (AC + Battery)");

                            // Ported from NVIDIA Profile Inspector:
                            // Texture Filtering Quality: High Performance
                            deviceKey.SetValue("OGL_TextureFilteringQuality", 2, RegistryValueKind.DWord);
                            Debug.WriteLine("   ✓ OGL_TextureFilteringQuality = 2 (High Performance)");

                            // Vertical Sync: Force Off
                            deviceKey.SetValue("OGL_VSyncMode", 0, RegistryValueKind.DWord);
                            deviceKey.SetValue("ForceVSync", 0, RegistryValueKind.DWord);
                            deviceKey.SetValue("VSyncMode", 0, RegistryValueKind.DWord);
                            Debug.WriteLine("   ✓ Vertical Sync = Force Off (OGL & DX)");

                            // Preferred Refresh Rate: Highest Available
                            deviceKey.SetValue("PreferredRefreshRate", 1, RegistryValueKind.DWord);
                            Debug.WriteLine("   ✓ PreferredRefreshRate = 1 (Highest Available)");

                            // Maximum Pre-rendered Frames: 1
                            deviceKey.SetValue("OGL_MaxFramesAllowed", 1, RegistryValueKind.DWord);
                            deviceKey.SetValue("MaxPreRenderedFrames", 1, RegistryValueKind.DWord);
                            Debug.WriteLine("   ✓ Pre-rendered Frames / Low Latency = 1");

                            // Multi-Threaded Shader Optimization (OpenGL)
                            deviceKey.SetValue("OGL_ThreadControl", 2, RegistryValueKind.DWord);
                            Debug.WriteLine("   ✓ OGL_ThreadControl = 2 (Force Multi-Threaded)");

                            anyApplied = true;
                        }
                    }
                }

                if (anyApplied)
                {
                    Debug.WriteLine("");
                    Debug.WriteLine("✓ NVIDIA GPU OPTIMIZADA:");
                    Debug.WriteLine("   • RMA deshabilitado - sin overhead de memoria remota");
                    Debug.WriteLine("   • P-States forzados a High Performance");
                    Debug.WriteLine("   • Dynamic P-States OFF - sin downclocking en juegos");
                    Debug.WriteLine("   • PowerMizer en máximo rendimiento");
                    Debug.WriteLine("   • Calidad de filtrado de textura en Alto Rendimiento");
                    Debug.WriteLine("   • Sincronización vertical forzada a Desactivado");
                    Debug.WriteLine("   • Tasa de refresco preferida establecida al Máximo");
                    Debug.WriteLine("   • Latencia ultra baja (Pre-rendered frames = 1)");
                    Debug.WriteLine("⚠ REQUIERE REINICIO para aplicar cambios");
                }
                else
                {
                    Debug.WriteLine("⚠ No se encontraron GPUs NVIDIA para optimizar");
                    Debug.WriteLine("   → Verifica que los drivers NVIDIA estén instalados");
                }

                return anyApplied;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error optimizando NVIDIA GPU: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Deshabilita NVIDIA Shader Cache para reducir stutters en la primera carga de shaders.
        /// Útil en juegos que compilan shaders en tiempo real.
        ///
        /// NOTA: En la mayoría de juegos modernos conviene HABILITARLO para evitar recompilaciones.
        /// Usar solo si experimentas stutters en juegos específicos.
        /// </summary>
        public static bool DisableNvidiaShaderCache()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(NVIDIA_THREADING_KEY))
                {
                    if (key == null) return false;
                    key.SetValue("DisableShaderCache", 1, RegistryValueKind.DWord);
                    Debug.WriteLine("   ✓ DisableShaderCache = 1 (Shader Cache deshabilitado)");
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Habilita NVIDIA Shader Cache (valor recomendado por defecto).
        /// </summary>
        public static bool EnableNvidiaShaderCache()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(NVIDIA_THREADING_KEY))
                {
                    if (key == null) return false;
                    key.DeleteValue("DisableShaderCache", false);
                    Debug.WriteLine("   ✓ DisableShaderCache eliminado (Shader Cache habilitado)");
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Restaura la configuración predeterminada de la GPU NVIDIA
        /// </summary>
        public static bool RestoreNvidiaGPU()
        {
            try
            {
                Debug.WriteLine("→ Restaurando configuración de GPU NVIDIA a valores predeterminados...");

                bool anyRestored = false;

                using (RegistryKey classKey = Registry.LocalMachine.OpenSubKey(GPU_CLASS_KEY, true))
                {
                    if (classKey == null) return false;

                    foreach (string subKeyName in classKey.GetSubKeyNames())
                    {
                        if (!IsNumericSubKey(subKeyName))
                            continue;

                        using (RegistryKey deviceKey = classKey.OpenSubKey(subKeyName, true))
                        {
                            if (deviceKey == null) continue;

                            object driverDesc = deviceKey.GetValue("DriverDesc");
                            string driverDescription = driverDesc?.ToString() ?? "";

                            if (!IsNvidiaDevice(driverDescription))
                                continue;

                            Debug.WriteLine($"   → Restaurando: {driverDescription}");

                            deviceKey.DeleteValue("RMAEnable", false);
                            deviceKey.DeleteValue("PerfLevelSrc", false);
                            deviceKey.DeleteValue("DisableDynamicPstate", false);
                            deviceKey.DeleteValue("PowerMizerEnable", false);
                            deviceKey.DeleteValue("PowerMizerLevel", false);
                            deviceKey.DeleteValue("PowerMizerLevelAC", false);
                            
                            deviceKey.DeleteValue("OGL_TextureFilteringQuality", false);
                            deviceKey.DeleteValue("OGL_VSyncMode", false);
                            deviceKey.DeleteValue("ForceVSync", false);
                            deviceKey.DeleteValue("VSyncMode", false);
                            deviceKey.DeleteValue("PreferredRefreshRate", false);
                            deviceKey.DeleteValue("OGL_MaxFramesAllowed", false);
                            deviceKey.DeleteValue("MaxPreRenderedFrames", false);
                            deviceKey.DeleteValue("OGL_ThreadControl", false);

                            Debug.WriteLine("   ✓ Valores NVIDIA restaurados a default");
                            anyRestored = true;
                        }
                    }
                }

                return anyRestored;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error restaurando NVIDIA GPU: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Detecta si hay alguna GPU NVIDIA instalada en el sistema
        /// </summary>
        public static bool IsNvidiaGPUPresent()
        {
            try
            {
                using (RegistryKey classKey = Registry.LocalMachine.OpenSubKey(GPU_CLASS_KEY))
                {
                    if (classKey == null) return false;

                    foreach (string subKeyName in classKey.GetSubKeyNames())
                    {
                        if (!IsNumericSubKey(subKeyName)) continue;

                        using (RegistryKey deviceKey = classKey.OpenSubKey(subKeyName))
                        {
                            if (deviceKey == null) continue;
                            object driverDesc = deviceKey.GetValue("DriverDesc");
                            if (IsNvidiaDevice(driverDesc?.ToString() ?? ""))
                                return true;
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

        private static bool IsNvidiaDevice(string driverDescription)
        {
            string desc = driverDescription.ToUpperInvariant();
            return desc.Contains("NVIDIA") || desc.Contains("GEFORCE") || desc.Contains("QUADRO") || desc.Contains("RTX") || desc.Contains("GTX");
        }

        private static bool IsNumericSubKey(string name)
        {
            return name.Length == 4 && int.TryParse(name, out _);
        }
    }
}
