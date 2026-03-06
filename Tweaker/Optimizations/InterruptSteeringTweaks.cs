using System;
using System.Diagnostics;

using Microsoft.Win32;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// Gestión de la distribución de interrupciones a núcleos de CPU (Interrupt Steering)
    /// Permite distribuir las interrupciones hardware entre múltiples cores para reducir la carga en un solo core
    /// </summary>
    public static class InterruptSteeringTweaks
    {
        private const string KERNEL_KEY = @"SYSTEM\CurrentControlSet\Control\Session Manager\kernel";

        /// <summary>
        /// Habilita la distribución de interrupciones a múltiples cores (Interrupt Steering)
        /// 
        /// ¿Qué es Interrupt Steering?
        /// - Permite que Windows distribuya las interrupciones hardware entre varios núcleos de CPU
        /// - Por defecto, muchas interrupciones se concentran en el core 0
        /// - Distribuirlas reduce la carga de un solo core y mejora la estabilidad
        /// 
        /// IMPACTO EN GAMING:
        /// - Reduce la carga en el core 0, mejorando frame times
        /// - Distribuye la carga de interrupciones de red, audio y periféricos
        /// </summary>
        /// <returns>True si la operación fue exitosa, false en caso contrario</returns>
        public static bool EnableInterruptSteering()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(KERNEL_KEY))
                {
                    if (key != null)
                    {
                        key.SetValue("InterruptSteeringDisabled", 0, RegistryValueKind.DWord);
                        key.SetValue("DistributeTimers", 1, RegistryValueKind.DWord);
                        Debug.WriteLine("✓ Interrupt Steering habilitado");
                        Debug.WriteLine("   → InterruptSteeringDisabled = 0");
                        Debug.WriteLine("   → DistributeTimers = 1");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error habilitando Interrupt Steering: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Deshabilita Interrupt Steering (concentra interrupciones en un solo core)
        /// 
        /// NOTA: Algunos benchmarks muestran que deshabilitar interrupt steering puede
        /// reducir latencia en sistemas de un solo juego al minimizar la migración de contexto
        /// </summary>
        /// <returns>True si la operación fue exitosa, false en caso contrario</returns>
        public static bool DisableInterruptSteering()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(KERNEL_KEY))
                {
                    if (key != null)
                    {
                        key.SetValue("InterruptSteeringDisabled", 1, RegistryValueKind.DWord);
                        key.SetValue("DistributeTimers", 0, RegistryValueKind.DWord);
                        Debug.WriteLine("✓ Interrupt Steering deshabilitado");
                        Debug.WriteLine("   → InterruptSteeringDisabled = 1");
                        Debug.WriteLine("   → DistributeTimers = 0");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error deshabilitando Interrupt Steering: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Restaura la configuración predeterminada de Interrupt Steering
        /// </summary>
        /// <returns>True si la operación fue exitosa, false en caso contrario</returns>
        public static bool RestoreInterruptSteering()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(KERNEL_KEY, true))
                {
                    if (key != null)
                    {
                        key.DeleteValue("InterruptSteeringDisabled", false);
                        key.DeleteValue("DistributeTimers", false);
                        Debug.WriteLine("✓ Interrupt Steering restaurado a valores predeterminados");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error restaurando Interrupt Steering: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Verifica si Interrupt Steering está habilitado
        /// </summary>
        /// <returns>True si está habilitado, false si está deshabilitado</returns>
        public static bool IsInterruptSteeringEnabled()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(KERNEL_KEY, false))
                {
                    if (key != null)
                    {
                        object value = key.GetValue("InterruptSteeringDisabled");
                        return value == null || value.ToString() != "1";
                    }
                }
                return true; // Habilitado por defecto
            }
            catch
            {
                return true;
            }
        }
    }
}
