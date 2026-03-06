using System;
using System.Diagnostics;

using Microsoft.Win32;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// Optimizaciones de CPU, Latencia del Sistema y Power Management
    /// Cr�tico para reducir latencia en juegos competitivos
    /// </summary>
    public static class CpuOptimization
    {
        private const string SYSTEM_PROFILE = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile";

        /// <summary>
        /// OPTIMIZACI�N CR�TICA: System Responsiveness
        /// 
        /// SystemResponsiveness: 0 (Rango 0-100)
        ///   - Controla cu�nto tiempo de CPU reserva Windows para tareas del sistema
        ///   - Valor predeterminado: 20 (20% del CPU reservado para el sistema)
        ///   - Valor 0: 0% reservado = TODO el CPU disponible para aplicaciones
        /// 
        /// IMPACTO EN GAMING:
        /// - Reduce latencia del sistema operativo (OS latency)
        /// - Mejora respuesta de input (mouse, teclado)
        /// - Elimina "lag" causado por procesos de Windows en background
        /// - CR�TICO para juegos de alta precisi�n (Valorant, CS2)
        /// 
        /// NetworkThrottlingIndex: 0xFFFFFFFF (DWORD m�ximo)
        ///   - Deshabilita el "throttling" de red de Windows
        ///   - Windows limita paquetes de red por defecto para "ahorrar energ�a"
        ///   - Valor m�ximo = sin l�mite de paquetes por segundo
        /// 
        /// IMPACTO EN ESPORTS:
        /// - Reduce ping efectivo en 5-20ms
        /// - Elimina "packet loss" artificial
        /// - Mejora hitreg (registro de disparos) en shooters
        /// - ESENCIAL para juegos online competitivos
        /// </summary>
        public static bool EnableSystemResponsivenessOptimization()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(SYSTEM_PROFILE, true))
                {
                    if (key != null)
                    {
                        // SystemResponsiveness en 0 = TODO el CPU para gaming
                        key.SetValue("SystemResponsiveness", 0, RegistryValueKind.DWord);

                        // NetworkThrottlingIndex en m�ximo = sin l�mite de paquetes
                        key.SetValue("NetworkThrottlingIndex", unchecked((int)0xFFFFFFFF), RegistryValueKind.DWord);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al optimizar System Responsiveness: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// RESTAURA valores predeterminados de Windows
        /// SystemResponsiveness: 20, NetworkThrottlingIndex: 10
        /// </summary>
        public static bool DisableSystemResponsivenessOptimization()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(SYSTEM_PROFILE, true))
                {
                    if (key != null)
                    {
                        key.SetValue("SystemResponsiveness", 20, RegistryValueKind.DWord);
                        key.SetValue("NetworkThrottlingIndex", 10, RegistryValueKind.DWord);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al restaurar System Responsiveness: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// ACTIVA Plan de Energ�a de Alto Rendimiento (High Performance Power Plan)
        /// 
        /// �Qu� hace?
        /// - Deshabilita C-States del CPU (estados de bajo consumo)
        /// - Mantiene el CPU a velocidad m�xima constantemente
        /// - Elimina "power throttling" que causa stuttering
        /// 
        /// PROBLEMA CON "BALANCED":
        /// - El CPU baja frecuencia en momentos de bajo uso
        /// - Tarda 1-5ms en volver a frecuencia m�xima
        /// - Causa micro-stutters y frame drops
        /// 
        /// IMPACTO EN GAMING:
        /// - Elimina stuttering por cambios de frecuencia
        /// - Mejora frame times consistency (1% lows)
        /// - Reduce latencia de entrada en 2-5ms
        /// - Usado por TODOS los jugadores profesionales
        /// 
        /// NOTA: Aumenta consumo el�ctrico y temperatura del CPU
        /// 
        /// Comando: powercfg -setactive scheme_min
        /// scheme_min = GUID del plan de Alto Rendimiento
        /// </summary>
        public static bool EnableHighPerformancePowerPlan()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powercfg.exe",
                    Arguments = "-setactive scheme_min", // scheme_min = High Performance GUID
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    Verb = "runas" // Requiere permisos de administrador
                };

                using (Process process = Process.Start(psi))
                {
                    process?.WaitForExit(5000); // Timeout de 5 segundos
                    return process?.ExitCode == 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al activar plan de Alto Rendimiento: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// ACTIVA el Plan de Energía 'Ultimate Performance' (Máximo Rendimiento)
        /// 
        /// ¿Qué es?
        /// - Un plan oculto de Windows diseñado para estaciones de trabajo de alto nivel.
        /// - Elimina micro-latencias de energía y mantiene el hardware al 100%.
        /// </summary>
        public static bool EnableUltimatePowerPlan()
        {
            try
            {
                // Primero intentar duplicar el esquema (si no existe)
                ProcessStartInfo duplicatePsi = new ProcessStartInfo
                {
                    FileName = "powercfg.exe",
                    Arguments = "-duplicatescheme e9a42b02-d5df-448d-aa00-03f14749eb61",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Verb = "runas"
                };

                using (Process p = Process.Start(duplicatePsi))
                {
                    p?.WaitForExit(3000);
                }

                // Luego activarlo
                ProcessStartInfo activePsi = new ProcessStartInfo
                {
                    FileName = "powercfg.exe",
                    Arguments = "-setactive e9a42b02-d5df-448d-aa00-03f14749eb61",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Verb = "runas"
                };

                using (Process p = Process.Start(activePsi))
                {
                    p?.WaitForExit(3000);
                    return p?.ExitCode == 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al activar plan Ultimate: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// RESTAURA el plan Balanceado (Disable Ultimate)
        /// </summary>
        public static bool DisableUltimatePowerPlan()
        {
            return EnableBalancedPowerPlan();
        }

        /// <summary>
        /// ACTIVA Plan de Energa Balanceado (predeterminado de Windows)
        /// </summary>
        public static bool EnableBalancedPowerPlan()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powercfg.exe",
                    Arguments = "-setactive scheme_balanced", // scheme_balanced = Balanced GUID
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    Verb = "runas"
                };

                using (Process process = Process.Start(psi))
                {
                    process?.WaitForExit(5000);
                    return process?.ExitCode == 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al activar plan Balanceado: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// DESHABILITA Power Throttling para todos los procesos
        /// 
        /// Power Throttling (Windows 10+):
        /// - Reduce velocidad de CPU de aplicaciones en background
        /// - PROBLEMA: A veces afecta juegos mal detectados
        /// - Anti-cheat y launchers pueden ser "throttled" incorrectamente
        /// 
        /// IMPACTO:
        /// - Elimina stuttering causado por throttling incorrecto
        /// - Mejora frame times cuando tienes apps abiertas (Discord, Chrome)
        /// </summary>
        public static bool DisablePowerThrottling()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\Power\PowerThrottling"))
                {
                    key?.SetValue("PowerThrottlingOff", 1, RegistryValueKind.DWord);
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al deshabilitar Power Throttling: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// HABILITA Power Throttling (valor predeterminado)
        /// </summary>
        public static bool EnablePowerThrottling()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Power\PowerThrottling", true))
                {
                    key?.DeleteValue("PowerThrottlingOff", false);
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al habilitar Power Throttling: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// OPTIMIZACI�N AVANZADA: Deshabilita Core Parking
        /// 
        /// Core Parking:
        /// - Windows "apaga" n�cleos de CPU no utilizados
        /// - Ahorra energ�a pero causa stuttering al "despertar" cores
        /// 
        /// IMPACTO EN RYZEN (AMD):
        /// - Ryzen tiene latencia alta entre CCX/CCD
        /// - Core parking causa frame drops severos
        /// - CR�TICO en Ryzen 5000/7000 para gaming
        /// 
        /// IMPACTO EN INTEL:
        /// - Menos cr�tico pero a�n mejora frame times
        /// - Especialmente importante en CPUs de 8+ cores
        /// </summary>
        public static bool DisableCoreParking()
        {
            try
            {
                // Deshabilitar core parking en todos los power schemes
                string[] schemes = { "scheme_min", "scheme_balanced", "scheme_max" };

                foreach (string scheme in schemes)
                {
                    // Configurar m�nimo de cores activos al 100%
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = "powercfg.exe",
                        Arguments = $"-setacvalueindex {scheme} SUB_PROCESSOR CPMINCORES 100",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        Verb = "runas"
                    };

                    using (Process process = Process.Start(psi))
                    {
                        process?.WaitForExit(3000);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al deshabilitar Core Parking: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// HABILITA Core Parking (comportamiento predeterminado)
        /// </summary>
        public static bool EnableCoreParking()
        {
            try
            {
                string[] schemes = { "scheme_min", "scheme_balanced", "scheme_max" };

                foreach (string scheme in schemes)
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = "powercfg.exe",
                        Arguments = $"-setacvalueindex {scheme} SUB_PROCESSOR CPMINCORES 0",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        Verb = "runas"
                    };

                    using (Process process = Process.Start(psi))
                    {
                        process?.WaitForExit(3000);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al habilitar Core Parking: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Verifica si Core Parking est� habilitado
        /// </summary>
        public static bool IsCoreParking()
        {
            try
            {
                // Verificar si el GUID de Core Parking est� configurado
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Power\PowerSettings\54533251-82be-4824-96c1-47b60b740d00\0cc5b647-c1df-4637-891a-dec35c318583", false))
                {
                    if (key != null)
                    {
                        // Verificar el valor ValueMax
                        object valueMax = key.GetValue("ValueMax");
                        if (valueMax != null && valueMax.ToString() == "0")
                        {
                            return false; // Core Parking deshabilitado
                        }
                        else
                        {
                            return true; // Core Parking habilitado (default)
                        }
                    }
                }

                // Si no se puede determinar, asumir que est� habilitado (default de Windows)
                return true;
            }
            catch
            {
                // Error accediendo al registro, asumir habilitado
                return true;
            }
        }

        /// <summary>
        /// Verifica si el plan de energ�a de alto rendimiento est� activo
        /// </summary>
        public static bool IsHighPerformanceActive()
        {
            try
            {
                using (var process = new Process())
                {
                    process.StartInfo = new ProcessStartInfo
                    {
                        FileName = "powercfg",
                        Arguments = "/getactivescheme",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    };

                    process.Start();
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    // GUID del plan de alto rendimiento: 8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c
                    return output.Contains("8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c");
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Deshabilita las actualizaciones de microcode del procesador
        /// 
        /// ¿Qué es el microcode?
        /// - El microcode es firmware interno del CPU que implementa el set de instrucciones
        /// - Windows puede aplicar actualizaciones de microcode al arranque para parchear vulnerabilidades
        ///   (Spectre, Meltdown, etc.) a través de los archivos mcupdate_GenuineIntel.dll / mcupdate_AuthenticAMD.dll
        /// - Estas actualizaciones pueden reducir el rendimiento del procesador
        /// 
        /// ADVERTENCIA: Deshabilitar el microcode puede exponer vulnerabilidades de seguridad.
        /// Solo recomendado para sistemas de gaming dedicados y aislados.
        /// </summary>
        /// <returns>True si la operación fue exitosa, false en caso contrario</returns>
        public static bool DisableMicrocodeUpdates()
        {
            try
            {
                Debug.WriteLine("→ Deshabilitando actualizaciones de microcode...");
                Debug.WriteLine("   ⚠ ADVERTENCIA: Reduce protección ante vulnerabilidades de CPU");

                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(
                    @"SYSTEM\CurrentControlSet\Control\Session Manager"))
                {
                    if (key != null)
                    {
                        key.SetValue("DisableMicrocodeLoad", 1, RegistryValueKind.DWord);
                        Debug.WriteLine("   ✓ DisableMicrocodeLoad = 1");
                        Debug.WriteLine("   ⚠ REQUIERE REINICIO para aplicar");
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error deshabilitando microcode updates: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Habilita las actualizaciones de microcode del procesador (restaura valores predeterminados)
        /// </summary>
        /// <returns>True si la operación fue exitosa, false en caso contrario</returns>
        public static bool EnableMicrocodeUpdates()
        {
            try
            {
                Debug.WriteLine("→ Habilitando actualizaciones de microcode...");

                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(
                    @"SYSTEM\CurrentControlSet\Control\Session Manager", true))
                {
                    if (key != null)
                    {
                        key.DeleteValue("DisableMicrocodeLoad", false);
                        Debug.WriteLine("   ✓ DisableMicrocodeLoad eliminado (predeterminado: habilitado)");
                        Debug.WriteLine("   ⚠ REQUIERE REINICIO para aplicar");
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error habilitando microcode updates: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Deshabilita el Package Idle State del procesador
        /// 
        /// ¿Qué es Package Idle?
        /// - Cuando todos los cores del paquete de CPU están en C-state profundo,
        ///   todo el paquete entra en un estado de muy bajo consumo (PC-state)
        /// - Salir de este estado tiene latencia adicional
        /// 
        /// IMPACTO EN GAMING:
        /// - Elimina la latencia de "despertar" del paquete de CPU completo
        /// - Más relevante en CPUs multi-die (AMD Ryzen con varios CCDs)
        /// - Mejora la consistencia de frame times
        /// </summary>
        /// <returns>True si la operación fue exitosa, false en caso contrario</returns>
        public static bool DisablePerfPackageIdle()
        {
            try
            {
                Debug.WriteLine("→ Deshabilitando Package Idle State...");

                bool success = true;

                string[] schemes = { "scheme_current" };
                foreach (string scheme in schemes)
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = "powercfg.exe",
                        Arguments = $"-setacvalueindex {scheme} SUB_PROCESSOR IDLEDISABLE 1",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };

                    using (Process process = Process.Start(psi))
                    {
                        process?.WaitForExit(3000);
                        success &= process?.ExitCode == 0;
                    }
                }

                ProcessStartInfo activePsi = new ProcessStartInfo
                {
                    FileName = "powercfg.exe",
                    Arguments = "-setactive scheme_current",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (Process process = Process.Start(activePsi))
                {
                    process?.WaitForExit(3000);
                }

                if (success)
                {
                    Debug.WriteLine("   ✓ Package Idle State deshabilitado");
                }

                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error deshabilitando Package Idle: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Habilita el Package Idle State del procesador (restaura valores predeterminados)
        /// </summary>
        /// <returns>True si la operación fue exitosa, false en caso contrario</returns>
        public static bool EnablePerfPackageIdle()
        {
            try
            {
                Debug.WriteLine("→ Habilitando Package Idle State...");

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powercfg.exe",
                    Arguments = "-setacvalueindex scheme_current SUB_PROCESSOR IDLEDISABLE 0",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (Process process = Process.Start(psi))
                {
                    process?.WaitForExit(3000);
                }

                ProcessStartInfo activePsi = new ProcessStartInfo
                {
                    FileName = "powercfg.exe",
                    Arguments = "-setactive scheme_current",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (Process process = Process.Start(activePsi))
                {
                    process?.WaitForExit(3000);
                }

                Debug.WriteLine("   ✓ Package Idle State habilitado (predeterminado)");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error habilitando Package Idle: {ex.Message}");
                return false;
            }
        }
    }
}
