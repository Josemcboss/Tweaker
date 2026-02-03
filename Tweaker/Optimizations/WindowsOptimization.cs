using Microsoft.Win32;
using System;
using System.Diagnostics;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// Optimizaciones de Windows para eliminar bloatware y mejorar rendimiento
    /// </summary>
    public static class WindowsOptimization
    {
        /// <summary>
        /// DESHABILITA Hibernación (hiberfil.sys)
        /// 
        /// ¿Qué es Hibernación?
        /// - Guarda el contenido de la RAM en el disco (hiberfil.sys)
        /// - Permite "hibernar" el PC y restaurar el estado completo
        /// - El archivo hiberfil.sys ocupa el tamaño de tu RAM (8GB, 16GB, 32GB, etc.)
        /// 
        /// PROBLEMAS:
        /// - Ocupa MUCHO espacio en disco (hasta 32GB en sistemas gaming)
        /// - Causa fragmentación del SSD/HDD
        /// - En gaming NO se usa (los jugadores apagan/reinician el PC normalmente)
        /// - Puede causar "Resume from Hibernate" bugs (pantallas negras, crashes)
        /// 
        /// IMPACTO AL DESHABILITAR:
        /// - Libera espacio: 8-32GB dependiendo de tu RAM
        /// - Elimina escrituras innecesarias al SSD (mejora vida útil)
        /// - Reduce fragmentación del sistema
        /// - Elimina posibles bugs de "Fast Startup" (que usa hibernación parcial)
        /// 
        /// NOTA: Fast Startup también se desactiva (usa hibernación)
        /// Fast Startup causa problemas con dual-boot y drivers
        /// 
        /// Comando: powercfg -h off
        /// </summary>
        public static bool DisableHibernation()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powercfg.exe",
                    Arguments = "-h off", // -h off = Hibernate OFF
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    Verb = "runas" // Requiere admin
                };

                using (Process process = Process.Start(psi))
                {
                    process?.WaitForExit(5000);
                    return process?.ExitCode == 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al deshabilitar Hibernación: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// HABILITA Hibernación (crea hiberfil.sys)
        /// </summary>
        public static bool EnableHibernation()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powercfg.exe",
                    Arguments = "-h on",
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
                Debug.WriteLine($"Error al habilitar Hibernación: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// DESHABILITA Windows Search Indexing
        /// 
        /// Windows Search:
        /// - Indexa todos los archivos del sistema constantemente
        /// - Consume CPU, Disco y RAM en background
        /// 
        /// PROBLEMA EN GAMING:
        /// - Causa stuttering cuando indexa durante gameplay
        /// - Uso constante de disco (100% disk usage en HDDs)
        /// - En gaming no necesitas búsquedas instantáneas del menú inicio
        /// 
        /// IMPACTO:
        /// - Reduce uso de disco de 20-100% (especialmente HDDs)
        /// - Libera RAM (200-500MB)
        /// - Elimina stuttering durante partidas
        /// </summary>
        public static bool DisableWindowsSearch()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "sc.exe",
                    Arguments = "config WSearch start=disabled",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Verb = "runas"
                };

                using (Process process = Process.Start(psi))
                {
                    process?.WaitForExit(3000);
                }

                // Detener el servicio también
                psi.Arguments = "stop WSearch";
                using (Process process = Process.Start(psi))
                {
                    process?.WaitForExit(3000);
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al deshabilitar Windows Search: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// HABILITA Windows Search Indexing
        /// </summary>
        public static bool EnableWindowsSearch()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "sc.exe",
                    Arguments = "config WSearch start=auto",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Verb = "runas"
                };

                using (Process process = Process.Start(psi))
                {
                    process?.WaitForExit(3000);
                }

                psi.Arguments = "start WSearch";
                using (Process process = Process.Start(psi))
                {
                    process?.WaitForExit(3000);
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al habilitar Windows Search: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// DESHABILITA SysMain (SuperFetch)
        /// 
        /// SysMain/SuperFetch:
        /// - Pre-carga aplicaciones "frecuentes" en RAM
        /// - Intenta "predecir" qué vas a abrir
        /// - En teoría mejora velocidad de apertura de apps
        /// 
        /// PROBLEMA EN GAMING:
        /// - Consume RAM innecesariamente (1-3GB)
        /// - Causa uso de disco constante
        /// - En gaming quieres RAM libre para el juego, no para cache
        /// - Con 16GB+ de RAM es innecesario
        /// 
        /// IMPACTO:
        /// - Libera 1-3GB de RAM
        /// - Reduce uso de disco
        /// - Elimina stuttering en sistemas con poca RAM (8GB)
        /// </summary>
        public static bool DisableSysMain()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "sc.exe",
                    Arguments = "config SysMain start=disabled",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Verb = "runas"
                };

                using (Process process = Process.Start(psi))
                {
                    process?.WaitForExit(3000);
                }

                psi.Arguments = "stop SysMain";
                using (Process process = Process.Start(psi))
                {
                    process?.WaitForExit(3000);
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al deshabilitar SysMain: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// HABILITA SysMain (SuperFetch)
        /// </summary>
        public static bool EnableSysMain()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "sc.exe",
                    Arguments = "config SysMain start=auto",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Verb = "runas"
                };

                using (Process process = Process.Start(psi))
                {
                    process?.WaitForExit(3000);
                }

                psi.Arguments = "start SysMain";
                using (Process process = Process.Start(psi))
                {
                    process?.WaitForExit(3000);
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al habilitar SysMain: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// DESHABILITA Windows Defender Real-Time Protection (Temporal)
        /// 
        /// ADVERTENCIA: SOLO para sesiones de gaming competitivo
        /// - Reduce uso de CPU en 5-15%
        /// - Elimina stuttering causado por escaneos en background
        /// - PRO PLAYERS lo deshabilitan antes de torneos
        /// 
        /// RIESGO: Deja el sistema vulnerable
        /// Úsalo solo si sabes lo que haces
        /// </summary>
        public static bool DisableWindowsDefenderRealTime()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection"))
                {
                    key?.SetValue("DisableRealtimeMonitoring", 1, RegistryValueKind.DWord);
                    key?.SetValue("DisableBehaviorMonitoring", 1, RegistryValueKind.DWord);
                    key?.SetValue("DisableOnAccessProtection", 1, RegistryValueKind.DWord);
                    key?.SetValue("DisableScanOnRealtimeEnable", 1, RegistryValueKind.DWord);
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al modificar Windows Defender: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// HABILITA Windows Defender Real-Time Protection
        /// </summary>
        public static bool EnableWindowsDefenderRealTime()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection", true))
                {
                    key?.DeleteValue("DisableRealtimeMonitoring", false);
                    key?.DeleteValue("DisableBehaviorMonitoring", false);
                    key?.DeleteValue("DisableOnAccessProtection", false);
                    key?.DeleteValue("DisableScanOnRealtimeEnable", false);
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al restaurar Windows Defender: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// DESHABILITA Telemetry de Windows
        /// 
        /// Telemetry:
        /// - Windows envía datos de uso a Microsoft constantemente
        /// - Consume ancho de banda y CPU
        /// 
        /// IMPACTO:
        /// - Reduce uso de red (mejora ping)
        /// - Libera CPU
        /// - Mejora privacidad
        /// </summary>
        public static bool DisableTelemetry()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\DataCollection"))
                {
                    key?.SetValue("AllowTelemetry", 0, RegistryValueKind.DWord);
                }

                // Deshabilitar servicios de telemetría
                string[] services = { "DiagTrack", "dmwappushservice" };
                
                foreach (string service in services)
                {
                    try
                    {
                        ProcessStartInfo psi = new ProcessStartInfo
                        {
                            FileName = "sc.exe",
                            Arguments = $"config {service} start=disabled",
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            Verb = "runas"
                        };

                        using (Process process = Process.Start(psi))
                        {
                            process?.WaitForExit(3000);
                        }

                        psi.Arguments = $"stop {service}";
                        using (Process process = Process.Start(psi))
                        {
                            process?.WaitForExit(3000);
                        }
                    }
                    catch { /* Ignorar errores individuales */ }
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al deshabilitar Telemetry: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// HABILITA Telemetry de Windows
        /// </summary>
        public static bool EnableTelemetry()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows\DataCollection", true))
                {
                    key?.DeleteValue("AllowTelemetry", false);
                }

                string[] services = { "DiagTrack", "dmwappushservice" };
                
                foreach (string service in services)
                {
                    try
                    {
                        ProcessStartInfo psi = new ProcessStartInfo
                        {
                            FileName = "sc.exe",
                            Arguments = $"config {service} start=auto",
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            Verb = "runas"
                        };

                        using (Process process = Process.Start(psi))
                        {
                            process?.WaitForExit(3000);
                        }
                    }
                    catch { }
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al habilitar Telemetry: {ex.Message}");
                return false;
            }
        }
    }
}
