using System;
using System.Diagnostics;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// Optimización de la configuración de arranque de Windows mediante bcdedit
    /// Reduce tiempos de boot y elimina características innecesarias durante el arranque
    /// </summary>
    public static class BootOptimization
    {
        /// <summary>
        /// Optimiza la configuración de arranque de Windows
        /// 
        /// Cambios aplicados:
        /// - timeout=0: Sin espera en el menú de arranque
        /// - bootuxdisabled=on: Deshabilita animación de arranque (boot UX)
        /// - bootlog=no: Deshabilita log de arranque
        /// - debug=no: Deshabilita modo debug
        /// - recoveryenabled=no: Deshabilita arranque automático en modo de recuperación
        /// - numproc=<número de procesadores>: Usa todos los procesadores disponibles
        /// 
        /// IMPACTO:
        /// - Reduce el tiempo de arranque significativamente
        /// - Elimina overhead de logging y animaciones durante el boot
        /// </summary>
        /// <returns>True si la operación fue exitosa, false en caso contrario</returns>
        public static bool OptimizeBootConfiguration()
        {
            try
            {
                Debug.WriteLine("─");
                Debug.WriteLine("BOOT OPTIMIZATION - Optimizando configuración de arranque");
                Debug.WriteLine("─");

                bool success = true;

                // timeout=0 - Sin espera en el menú de arranque
                success &= RunBcdedit("/timeout 0", "timeout=0");

                // bootuxdisabled=on - Deshabilita animación de boot
                success &= RunBcdedit("/set {current} bootuxdisabled on", "bootuxdisabled=on");

                // bootlog=no - Deshabilita log de boot
                success &= RunBcdedit("/set {current} bootlog no", "bootlog=no");

                // debug=no - Deshabilita modo debug
                success &= RunBcdedit("/set {current} debug no", "debug=no");

                // recoveryenabled=no - Deshabilita arranque automático en recuperación
                success &= RunBcdedit("/set {current} recoveryenabled no", "recoveryenabled=no");

                // numproc - Usar todos los procesadores
                int processorCount = Environment.ProcessorCount;
                success &= RunBcdedit($"/set {{current}} numproc {processorCount}", $"numproc={processorCount}");

                Debug.WriteLine("");
                if (success)
                {
                    Debug.WriteLine("✓ BOOT OPTIMIZADO:");
                    Debug.WriteLine($"   • timeout: 0 (sin menú de espera)");
                    Debug.WriteLine($"   • bootuxdisabled: on (sin animación)");
                    Debug.WriteLine($"   • bootlog: no (sin log de arranque)");
                    Debug.WriteLine($"   • debug: no (sin modo debug)");
                    Debug.WriteLine($"   • recoveryenabled: no");
                    Debug.WriteLine($"   • numproc: {processorCount} (todos los procesadores)");
                    Debug.WriteLine("⚠ REQUIERE REINICIO para aplicar cambios");
                }
                else
                {
                    Debug.WriteLine("⚠ Algunos cambios de boot no pudieron aplicarse");
                }

                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error optimizando configuración de arranque: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Restaura la configuración de arranque a los valores predeterminados de Windows
        /// </summary>
        /// <returns>True si la operación fue exitosa, false en caso contrario</returns>
        public static bool RestoreBootConfiguration()
        {
            try
            {
                Debug.WriteLine("→ Restaurando configuración de arranque a valores predeterminados...");

                bool success = true;

                // Restaurar timeout por defecto (30 segundos)
                success &= RunBcdedit("/timeout 30", "timeout=30");

                // Restaurar bootux por defecto
                success &= RunBcdedit("/deletevalue {current} bootuxdisabled", "bootuxdisabled=<eliminado>");

                // Restaurar bootlog por defecto
                success &= RunBcdedit("/deletevalue {current} bootlog", "bootlog=<eliminado>");

                // Restaurar debug por defecto
                success &= RunBcdedit("/set {current} debug no", "debug=no");

                // Restaurar recoveryenabled
                success &= RunBcdedit("/set {current} recoveryenabled yes", "recoveryenabled=yes");

                // Eliminar numproc limitado
                success &= RunBcdedit("/deletevalue {current} numproc", "numproc=<eliminado>");

                if (success)
                {
                    Debug.WriteLine("✓ Configuración de arranque restaurada a valores predeterminados");
                }

                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error restaurando configuración de arranque: {ex.Message}");
                return false;
            }
        }

        private static bool RunBcdedit(string arguments, string settingDescription)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "bcdedit.exe",
                    Arguments = arguments,
                    UseShellExecute = true,
                    Verb = "runas",
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(psi))
                {
                    process?.WaitForExit(5000);
                    bool success = process?.ExitCode == 0;
                    if (success)
                    {
                        Debug.WriteLine($"   ✓ bcdedit {settingDescription}");
                    }
                    else
                    {
                        Debug.WriteLine($"   ⚠ bcdedit {settingDescription} - código: {process?.ExitCode}");
                    }
                    return success;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"   ✗ Error ejecutando bcdedit {arguments}: {ex.Message}");
                return false;
            }
        }
    }
}
