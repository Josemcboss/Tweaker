using System;
using System.Diagnostics;

using Microsoft.Win32;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// Optimizaciones específicas para GPUs AMD Radeon
    /// Mejora rendimiento en gaming deshabilitando características de ahorro de energía
    /// 
    /// ADVERTENCIA: Estos tweaks modifican claves del registro del controlador AMD.
    /// Asegúrate de tener los drivers AMD instalados antes de aplicar.
    /// </summary>
    public static class RadeonOptimization
    {
        // Clase de dispositivo para adaptadores de display (GPUs)
        private const string GPU_CLASS_KEY = @"SYSTEM\CurrentControlSet\Control\Class\{4d36e968-e325-11ce-bfc1-08002be10318}";

        /// <summary>
        /// Optimiza la GPU AMD Radeon para máximo rendimiento en gaming
        /// 
        /// Cambios aplicados:
        /// - EnableUlps = 0: Deshabilita Ultra Low Power State (ULPS)
        ///   El ULPS hace que la GPU entre en estado de muy bajo consumo,
        ///   lo que puede causar stuttering al volver a modo de rendimiento
        /// 
        /// - PP_ThermalAutoThrottlingEnable = 0: Deshabilita throttling térmico automático
        ///   Permite a la GPU mantener frecuencias máximas más tiempo
        ///   (asegúrate de tener buen sistema de refrigeración)
        /// 
        /// - KMD_EnableComputePreemption = 0: Deshabilita preemption de cómputo
        ///   Puede mejorar rendimiento en juegos que usan compute shaders intensivamente
        /// 
        /// IMPACTO EN GAMING:
        /// - Elimina micro-stutters causados por transiciones de estado de energía
        /// - Mantiene la GPU a frecuencias más estables
        /// - Reduce frame time variance
        /// </summary>
        /// <returns>True si la operación fue exitosa, false en caso contrario</returns>
        public static bool OptimizeRadeonGPU()
        {
            try
            {
                Debug.WriteLine("─");
                Debug.WriteLine("RADEON GPU OPTIMIZATION - Optimizando GPU AMD");
                Debug.WriteLine("─");

                bool anyApplied = false;

                // Iterar sobre las subclaves (0000, 0001, etc.) para encontrar la GPU AMD
                using (RegistryKey classKey = Registry.LocalMachine.OpenSubKey(GPU_CLASS_KEY, true))
                {
                    if (classKey == null)
                    {
                        Debug.WriteLine("⚠ No se encontró la clave de clase de GPU");
                        Debug.WriteLine("   → Los drivers de display pueden no estar instalados");
                        return false;
                    }

                    foreach (string subKeyName in classKey.GetSubKeyNames())
                    {
                        // Saltar claves que no son instancias de dispositivo (ej: "Properties")
                        if (!IsNumericSubKey(subKeyName))
                            continue;

                        using (RegistryKey deviceKey = classKey.OpenSubKey(subKeyName, true))
                        {
                            if (deviceKey == null)
                                continue;

                            // Verificar si es un dispositivo AMD/ATI
                            object driverDesc = deviceKey.GetValue("DriverDesc");
                            string driverDescription = driverDesc?.ToString() ?? "";

                            if (!IsAMDDevice(driverDescription))
                                continue;

                            Debug.WriteLine($"   → GPU encontrada: {driverDescription} (clave: {subKeyName})");

                            // Deshabilitar ULPS (Ultra Low Power State)
                            deviceKey.SetValue("EnableUlps", 0, RegistryValueKind.DWord);
                            Debug.WriteLine("   ✓ EnableUlps = 0 (ULPS deshabilitado)");

                            // Deshabilitar throttling térmico automático
                            deviceKey.SetValue("PP_ThermalAutoThrottlingEnable", 0, RegistryValueKind.DWord);
                            Debug.WriteLine("   ✓ PP_ThermalAutoThrottlingEnable = 0");

                            // Deshabilitar preemption de cómputo del kernel mode driver
                            deviceKey.SetValue("KMD_EnableComputePreemption", 0, RegistryValueKind.DWord);
                            Debug.WriteLine("   ✓ KMD_EnableComputePreemption = 0");

                            anyApplied = true;
                        }
                    }
                }

                if (anyApplied)
                {
                    Debug.WriteLine("");
                    Debug.WriteLine("✓ RADEON GPU OPTIMIZADA:");
                    Debug.WriteLine("   • ULPS deshabilitado - sin transiciones de estado de energía");
                    Debug.WriteLine("   • Thermal Auto Throttling deshabilitado - frecuencias estables");
                    Debug.WriteLine("   • Compute Preemption deshabilitado - mejor rendimiento compute");
                    Debug.WriteLine("⚠ REQUIERE REINICIO para aplicar cambios");
                }
                else
                {
                    Debug.WriteLine("⚠ No se encontraron GPUs AMD/ATI para optimizar");
                    Debug.WriteLine("   → Verifica que los drivers AMD estén instalados");
                }

                return anyApplied;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error optimizando Radeon GPU: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Restaura la configuración predeterminada de la GPU AMD Radeon
        /// </summary>
        /// <returns>True si la operación fue exitosa, false en caso contrario</returns>
        public static bool RestoreRadeonGPU()
        {
            try
            {
                Debug.WriteLine("→ Restaurando configuración de GPU AMD a valores predeterminados...");

                bool anyRestored = false;

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

                            if (!IsAMDDevice(driverDescription))
                                continue;

                            // Restaurar ULPS (habilitado por defecto)
                            deviceKey.SetValue("EnableUlps", 1, RegistryValueKind.DWord);
                            Debug.WriteLine($"   ✓ EnableUlps = 1 restaurado para {driverDescription}");

                            // Restaurar thermal throttling (habilitado por defecto)
                            deviceKey.DeleteValue("PP_ThermalAutoThrottlingEnable", false);
                            Debug.WriteLine($"   ✓ PP_ThermalAutoThrottlingEnable eliminado");

                            // Restaurar compute preemption (habilitado por defecto)
                            deviceKey.DeleteValue("KMD_EnableComputePreemption", false);
                            Debug.WriteLine($"   ✓ KMD_EnableComputePreemption eliminado");

                            anyRestored = true;
                        }
                    }
                }

                if (anyRestored)
                {
                    Debug.WriteLine("✓ Configuración de GPU AMD restaurada");
                    Debug.WriteLine("⚠ REQUIERE REINICIO para aplicar cambios");
                }
                else
                {
                    Debug.WriteLine("⚠ No se encontraron GPUs AMD para restaurar");
                }

                return anyRestored;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error restaurando Radeon GPU: {ex.Message}");
                return false;
            }
        }

        private static bool IsNumericSubKey(string name)
        {
            if (name.Length != 4)
                return false;

            foreach (char c in name)
            {
                if (!char.IsDigit(c))
                    return false;
            }
            return true;
        }

        private static bool IsAMDDevice(string driverDescription)
        {
            if (string.IsNullOrEmpty(driverDescription))
                return false;

            string lower = driverDescription.ToLowerInvariant();
            return lower.Contains("amd") || lower.Contains("ati") || lower.Contains("radeon");
        }
    }
}
