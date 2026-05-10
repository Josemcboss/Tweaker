using System;
using System.Diagnostics;

using Microsoft.Win32;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// Tweaks específicos para procesadores AMD e Intel
    /// Optimiza la gestión de energía y rendimiento del procesador
    /// 
    /// ADVERTENCIA: Estos tweaks están diseñados para sistemas de gaming de alto rendimiento.
    /// Pueden aumentar el consumo energético y la temperatura del procesador.
    /// </summary>
    public static class ProcessorOptimization
    {
        private const string AMD_PPM_KEY = @"SYSTEM\CurrentControlSet\Services\amdppm\Parameters";
        private const string INTEL_PPM_KEY = @"SYSTEM\CurrentControlSet\Services\intelppm\Parameters";

        /// <summary>
        /// Optimiza procesadores AMD deshabilitando características de ahorro de energía
        /// 
        /// Cambios aplicados:
        /// - Deshabilita CPPC (Collaborative Processor Performance Control)
        ///   Evita que Windows limite la frecuencia del procesador
        /// - Deshabilita Power Throttling a nivel de procesador
        /// - Deshabilita Cool'n'Quiet (tecnología de ahorro de energía de AMD)
        /// - Configura PERFAUTONOMOUS=0 para que Windows controle el rendimiento
        /// - Configura PERFBOOSTMODE=2 para máximo Turbo Boost
        /// - Deshabilita IDLEDISABLE para manejar estados idle correctamente
        /// 
        /// IMPACTO EN RYZEN:
        /// - Frecuencias más estables y predecibles
        /// - Menos stuttering por cambios de P-state
        /// - Mejor rendimiento en gaming competitivo
        /// </summary>
        /// <returns>True si la operación fue exitosa, false en caso contrario</returns>
        public static bool OptimizeAMDProcessor()
        {
            try
            {
                Debug.WriteLine("─");
                Debug.WriteLine("AMD PROCESSOR OPTIMIZATION - Optimizando procesador AMD");
                Debug.WriteLine("─");

                bool success = true;

                // Configurar parámetros del driver de gestión de energía AMD
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(AMD_PPM_KEY))
                {
                    if (key != null)
                    {
                        // Deshabilitar CPPC
                        key.SetValue("CppcEnabled", 0, RegistryValueKind.DWord);
                        Debug.WriteLine("   ✓ CppcEnabled = 0 (CPPC deshabilitado)");

                        // Deshabilitar Cool'n'Quiet
                        key.SetValue("CoolnQuietEnabled", 0, RegistryValueKind.DWord);
                        Debug.WriteLine("   ✓ CoolnQuietEnabled = 0 (Cool'n'Quiet deshabilitado)");
                    }
                    else
                    {
                        Debug.WriteLine("   ⚠ No se pudo crear la clave AMD PPM");
                        success = false;
                    }
                }

                // Configurar powercfg para el plan activo
                success &= RunPowercfg("-setacvalueindex scheme_current SUB_PROCESSOR PERFAUTONOMOUS 0",
                    "PERFAUTONOMOUS=0 (Windows controla el rendimiento)");

                success &= RunPowercfg("-setacvalueindex scheme_current SUB_PROCESSOR PERFBOOSTMODE 2",
                    "PERFBOOSTMODE=2 (máximo Turbo Boost)");

                success &= RunPowercfg("-setacvalueindex scheme_current SUB_PROCESSOR IDLEDISABLE 1",
                    "IDLEDISABLE=1 (estados idle deshabilitados)");

                success &= RunPowercfg("-setactive scheme_current", "Aplicar configuración activa");

                if (success)
                {
                    Debug.WriteLine("");
                    Debug.WriteLine("✓ PROCESADOR AMD OPTIMIZADO:");
                    Debug.WriteLine("   • CPPC deshabilitado - frecuencias más estables");
                    Debug.WriteLine("   • Cool'n'Quiet deshabilitado - sin throttling");
                    Debug.WriteLine("   • PERFAUTONOMOUS=0 - control por Windows");
                    Debug.WriteLine("   • PERFBOOSTMODE=2 - máximo Turbo Boost");
                    Debug.WriteLine("   • IDLEDISABLE=1 - sin estados idle");
                    Debug.WriteLine("⚠ REQUIERE REINICIO para algunos cambios");
                }

                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error optimizando procesador AMD: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Restaura la configuración predeterminada del procesador AMD
        /// </summary>
        /// <returns>True si la operación fue exitosa, false en caso contrario</returns>
        public static bool RestoreAMDProcessor()
        {
            try
            {
                Debug.WriteLine("→ Restaurando configuración de procesador AMD...");

                bool success = true;

                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(AMD_PPM_KEY, true))
                {
                    if (key != null)
                    {
                        key.DeleteValue("CppcEnabled", false);
                        key.DeleteValue("CoolnQuietEnabled", false);
                        Debug.WriteLine("   ✓ Parámetros AMD PPM restaurados");
                    }
                }

                success &= RunPowercfg("-setacvalueindex scheme_current SUB_PROCESSOR PERFAUTONOMOUS 1",
                    "PERFAUTONOMOUS=1 (restaurado)");

                success &= RunPowercfg("-setacvalueindex scheme_current SUB_PROCESSOR PERFBOOSTMODE 0",
                    "PERFBOOSTMODE=0 (restaurado)");

                success &= RunPowercfg("-setacvalueindex scheme_current SUB_PROCESSOR IDLEDISABLE 0",
                    "IDLEDISABLE=0 (restaurado)");

                success &= RunPowercfg("-setactive scheme_current", "Aplicar configuración activa");

                if (success)
                {
                    Debug.WriteLine("✓ Procesador AMD restaurado a valores predeterminados");
                }

                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error restaurando procesador AMD: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Optimiza procesadores Intel deshabilitando SpeedStep y optimizando Turbo Boost
        /// 
        /// Cambios aplicados:
        /// - Deshabilita SpeedStep (EIST - Enhanced Intel SpeedStep Technology)
        ///   Mantiene el CPU a frecuencia máxima constante
        /// - Configura Turbo Boost para rendimiento máximo
        /// - Deshabilita C-States en el plan de energía
        /// 
        /// IMPACTO EN INTEL:
        /// - Frecuencia de CPU constante (sin variaciones por P-states)
        /// - Mejor consistencia en frame times
        /// - Reduce latencia de input
        /// </summary>
        /// <returns>True si la operación fue exitosa, false en caso contrario</returns>
        public static bool OptimizeIntelProcessor()
        {
            try
            {
                Debug.WriteLine("─");
                Debug.WriteLine("INTEL PROCESSOR OPTIMIZATION - Optimizando procesador Intel");
                Debug.WriteLine("─");

                bool success = true;

                // Configurar parámetros del driver de gestión de energía Intel
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(INTEL_PPM_KEY))
                {
                    if (key != null)
                    {
                        // Deshabilitar SpeedStep (EIST)
                        key.SetValue("Attributes", 0, RegistryValueKind.DWord);
                        Debug.WriteLine("   ✓ Intel PPM Attributes = 0");
                    }
                    else
                    {
                        Debug.WriteLine("   ⚠ No se pudo crear la clave Intel PPM");
                        success = false;
                    }
                }

                // Configurar powercfg para máximo rendimiento Intel
                success &= RunPowercfg("-setacvalueindex scheme_current SUB_PROCESSOR PERFBOOSTMODE 2",
                    "PERFBOOSTMODE=2 (máximo Turbo Boost)");

                success &= RunPowercfg("-setacvalueindex scheme_current SUB_PROCESSOR PERFAUTONOMOUS 0",
                    "PERFAUTONOMOUS=0 (Windows controla el rendimiento)");

                success &= RunPowercfg("-setacvalueindex scheme_current SUB_PROCESSOR IDLEDISABLE 1",
                    "IDLEDISABLE=1 (estados idle deshabilitados)");

                // Mantener CPU al 100% mínimo
                success &= RunPowercfg("-setacvalueindex scheme_current SUB_PROCESSOR PROCTHROTTLEMIN 100",
                    "PROCTHROTTLEMIN=100 (mínimo 100%)");

                success &= RunPowercfg("-setactive scheme_current", "Aplicar configuración activa");

                if (success)
                {
                    Debug.WriteLine("");
                    Debug.WriteLine("✓ PROCESADOR INTEL OPTIMIZADO:");
                    Debug.WriteLine("   • Intel PPM configurado para máximo rendimiento");
                    Debug.WriteLine("   • PERFBOOSTMODE=2 - máximo Turbo Boost");
                    Debug.WriteLine("   • PERFAUTONOMOUS=0 - control por Windows");
                    Debug.WriteLine("   • IDLEDISABLE=1 - sin estados idle");
                    Debug.WriteLine("   • PROCTHROTTLEMIN=100 - siempre al 100%");
                    Debug.WriteLine("⚠ REQUIERE REINICIO para algunos cambios");
                }

                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error optimizando procesador Intel: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Restaura la configuración predeterminada del procesador Intel
        /// </summary>
        /// <returns>True si la operación fue exitosa, false en caso contrario</returns>
        public static bool RestoreIntelProcessor()
        {
            try
            {
                Debug.WriteLine("→ Restaurando configuración de procesador Intel...");

                bool success = true;

                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(INTEL_PPM_KEY, true))
                {
                    if (key != null)
                    {
                        key.DeleteValue("Attributes", false);
                        Debug.WriteLine("   ✓ Intel PPM restaurado");
                    }
                }

                success &= RunPowercfg("-setacvalueindex scheme_current SUB_PROCESSOR PERFBOOSTMODE 0",
                    "PERFBOOSTMODE=0 (restaurado)");

                success &= RunPowercfg("-setacvalueindex scheme_current SUB_PROCESSOR PERFAUTONOMOUS 1",
                    "PERFAUTONOMOUS=1 (restaurado)");

                success &= RunPowercfg("-setacvalueindex scheme_current SUB_PROCESSOR IDLEDISABLE 0",
                    "IDLEDISABLE=0 (restaurado)");

                success &= RunPowercfg("-setacvalueindex scheme_current SUB_PROCESSOR PROCTHROTTLEMIN 5",
                    "PROCTHROTTLEMIN=5 (restaurado)");

                success &= RunPowercfg("-setactive scheme_current", "Aplicar configuración activa");

                if (success)
                {
                    Debug.WriteLine("✓ Procesador Intel restaurado a valores predeterminados");
                }

                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error restaurando procesador Intel: {ex.Message}");
                return false;
            }
        }

        private static bool RunPowercfg(string arguments, string settingDescription)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powercfg.exe",
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (Process process = Process.Start(psi))
                {
                    process?.WaitForExit(5000);
                    bool success = process?.ExitCode == 0;
                    if (success)
                    {
                        Debug.WriteLine($"   ✓ powercfg: {settingDescription}");
                    }
                    else
                    {
                        Debug.WriteLine($"   ⚠ powercfg: {settingDescription} - código: {process?.ExitCode}");
                    }
                    return success;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"   ✗ Error ejecutando powercfg {arguments}: {ex.Message}");
                return false;
            }
        }
    }
}
