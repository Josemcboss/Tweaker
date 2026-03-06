using System;
using System.Diagnostics;

using Microsoft.Win32;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// Optimizaciones de Seguridad del Sistema que afectan el rendimiento
    /// </summary>
    public static class SystemSecurityOptimization
    {
        private const string DEVICE_GUARD_KEY = @"SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity";

        /// <summary>
        /// DESHABILITA Core Isolation (VBS / HVCI)
        /// 
        /// ¿Qué es?
        /// - Virtualization-Based Security (VBS) usa hardware de virtualización para aislar memoria.
        /// - Hypervisor-Enforced Code Integrity (HVCI) es parte de esto.
        /// - PROBLEMA: Causa un impacto de rendimiento del 10% al 30% en CPUs modernas (especialmente Ryzen).
        /// 
        /// IMPACTO:
        /// - FPS significativamente más altos.
        /// - Menor latencia de CPU.
        /// - Indispensable para gaming competitivo extremo.
        /// </summary>
        public static bool DisableCoreIsolation()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(DEVICE_GUARD_KEY))
                {
                    key?.SetValue("Enabled", 0, RegistryValueKind.DWord);
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al deshabilitar Core Isolation: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// HABILITA Core Isolation (Comportamiento por defecto / Seguro)
        /// </summary>
        public static bool EnableCoreIsolation()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(DEVICE_GUARD_KEY))
                {
                    key?.SetValue("Enabled", 1, RegistryValueKind.DWord);
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al habilitar Core Isolation: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Verifica si Core Isolation está habilitado
        /// </summary>
        public static bool IsCoreIsolationEnabled()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(DEVICE_GUARD_KEY))
                {
                    if (key != null)
                    {
                        object value = key.GetValue("Enabled");
                        return value != null && value.ToString() == "1";
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
