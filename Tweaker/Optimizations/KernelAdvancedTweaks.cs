using System;
using System.Diagnostics;
using Tweaker.Utilities;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// Kernel Advanced Latency Tweaks
    /// Configura sincronización TSC mejorada (Enhanced TSC Sync), elimina dynamic ticks
    /// y optimiza los timers de kernel de Windows para latencia de entrada sub-milisegundo.
    /// </summary>
    public static class KernelAdvancedTweaks
    {
        private static bool RunBcdEdit(string arguments)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "bcdedit",
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using var proc = Process.Start(psi);
                if (proc == null) return false;
                proc.WaitForExit();
                return proc.ExitCode == 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error ejecutando bcdedit {arguments}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Aplica optimizaciones de kernel de ultra-baja latencia (Enhanced TSC, no dynamic tick, platform tick)
        /// </summary>
        public static bool OptimizeKernelLatency()
        {
            try
            {
                Debug.WriteLine("──────────────────────────────────────────");
                Debug.WriteLine("⚡ APLICANDO OPTIMIZACIONES DE KERNEL BCD (TSC SYNC / LOW TICK)");
                Debug.WriteLine("──────────────────────────────────────────");

                bool success = true;

                // 1. Enhanced TSC Sync Policy (Sincronización de reloj TSC entre cores ultra-rápida)
                success &= RunBcdEdit("/set tscsyncpolicy Enhanced");
                Debug.WriteLine("  ✔ tscsyncpolicy -> Enhanced");

                // 2. Deshabilitar Dynamic Tick (Elimina latencia de cambio de estado de timers)
                success &= RunBcdEdit("/set disabledynamictick yes");
                Debug.WriteLine("  ✔ disabledynamictick -> yes");

                // 3. Forzar useplatformtick yes
                success &= RunBcdEdit("/set useplatformtick yes");
                Debug.WriteLine("  ✔ useplatformtick -> yes");

                // 4. Eliminar useplatformclock (evitar HPET lento)
                RunBcdEdit("/deletevalue useplatformclock");
                Debug.WriteLine("  ✔ useplatformclock -> Eliminado");

                // 5. Optimización de arranque silencioso y sin demoras
                RunBcdEdit("/set bootstatuspolicy ignore");
                RunBcdEdit("/set quietboot yes");

                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error en OptimizeKernelLatency: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Restaura la configuración de BCD a los valores por defecto de Windows
        /// </summary>
        public static bool RestoreKernelLatency()
        {
            try
            {
                Debug.WriteLine("Restaurando configuración predeterminada de Kernel BCD...");

                RunBcdEdit("/deletevalue tscsyncpolicy");
                RunBcdEdit("/deletevalue disabledynamictick");
                RunBcdEdit("/deletevalue useplatformtick");
                RunBcdEdit("/deletevalue quietboot");
                RunBcdEdit("/set bootstatuspolicy displayallfailures");

                Debug.WriteLine("✔ Kernel BCD restaurado a valores por defecto");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error en RestoreKernelLatency: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Comprueba si la política de TSC Sync está configurada como Enhanced
        /// </summary>
        public static bool? IsKernelOptimized()
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "bcdedit",
                    Arguments = "/enum {current}",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                };

                using var proc = Process.Start(psi);
                if (proc == null) return null;
                string output = proc.StandardOutput.ReadToEnd();
                proc.WaitForExit();

                if (output.Contains("tscsyncpolicy", StringComparison.OrdinalIgnoreCase) &&
                    output.Contains("Enhanced", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return null;
            }
        }
    }
}
