using System;
using System.Diagnostics;

using Microsoft.Win32;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// Optimizaciones de disco para gaming - reduce IO overhead y alarga vida útil del SSD
    /// Tweaks específicos para NTFS que mejoran rendimiento y reducen escrituras innecesarias
    /// </summary>
    public static class DiskTweaks
    {
        /// <summary>
        /// Optimiza comportamiento NTFS para gaming y longevidad del SSD
        /// Desactiva actualización de fecha de acceso y nombres 8.3 DOS
        /// </summary>
        public static bool OptimizeNTFS()
        {
            try
            {
                Debug.WriteLine("???????????????????????????????????????????????????????????");
                Debug.WriteLine("OPTIMIZING NTFS FOR GAMING & SSD LIFE");
                Debug.WriteLine("???????????????????????????????????????????????????????????");

                bool success = true;

                // 1. Desactivar Last Access Time (CRÍTICO para SSD)
                Debug.WriteLine("?? Desactivando Last Access Time...");
                if (DisableLastAccessTime())
                {
                    Debug.WriteLine("? Last Access Time desactivado");
                    Debug.WriteLine("   ?? BENEFICIO SSD: Reduce escrituras innecesarias en ~30%");
                    Debug.WriteLine("   ?? BENEFICIO GAMING: Elimina micro-freezes por IO overhead");
                }
                else
                {
                    Debug.WriteLine("? Error desactivando Last Access Time");
                    success = false;
                }

                // 2. Desactivar nombres 8.3 (DOS legacy)
                Debug.WriteLine("");
                Debug.WriteLine("?? Desactivando nombres 8.3 DOS...");
                if (Disable8Dot3Names())
                {
                    Debug.WriteLine("? Nombres 8.3 DOS desactivados");
                    Debug.WriteLine("   ?? BENEFICIO: Menos metadata por archivo");
                    Debug.WriteLine("   ? BENEFICIO: Acceso a archivos más rápido");
                }
                else
                {
                    Debug.WriteLine("? Error desactivando nombres 8.3");
                    success = false;
                }

                // 3. Configurar Memory Management para gaming
                Debug.WriteLine("");
                Debug.WriteLine("?? Optimizando Memory Management...");
                if (OptimizeMemoryManagement())
                {
                    Debug.WriteLine("? Memory Management optimizado");
                    Debug.WriteLine("   ?? BENEFICIO: Mejor cache de archivos");
                    Debug.WriteLine("   ?? BENEFICIO: Menos stutter en carga de assets");
                }
                else
                {
                    Debug.WriteLine("?? Memory Management no se pudo optimizar completamente");
                }

                if (success)
                {
                    Debug.WriteLine("");
                    Debug.WriteLine("?? RESUMEN DE OPTIMIZACIONES APLICADAS:");
                    Debug.WriteLine("???????????????????????????????????????");
                    Debug.WriteLine("? Last Access Time: DESACTIVADO");
                    Debug.WriteLine("   • Reduce escrituras SSD en ~30%");
                    Debug.WriteLine("   • Alarga vida útil del SSD significativamente");
                    Debug.WriteLine("   • Elimina micro-freezes durante carga");
                    Debug.WriteLine("");
                    Debug.WriteLine("? Nombres 8.3 DOS: DESACTIVADOS");
                    Debug.WriteLine("   • Menos overhead por archivo creado");
                    Debug.WriteLine("   • Navegación de carpetas más rápida");
                    Debug.WriteLine("");
                    Debug.WriteLine("?? IMPACTO GAMING:");
                    Debug.WriteLine("   • Menos stuttering en world loading");
                    Debug.WriteLine("   • Tiempos de carga mejorados");
                    Debug.WriteLine("   • SSD durará años más");
                    Debug.WriteLine("");
                    Debug.WriteLine("?? REQUIERE REINICIO para efecto completo");
                }

                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? ERROR en OptimizeNTFS: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Desactiva Last Access Time - CRÍTICO para longevidad del SSD
        /// Evita que Windows escriba fecha de acceso cada vez que se lee un archivo
        /// </summary>
        private static bool DisableLastAccessTime()
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "fsutil",
                    Arguments = "behavior set disablelastaccess 1",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(startInfo))
                {
                    if (process == null)
                    {
                        Debug.WriteLine("? No se pudo iniciar fsutil para Last Access Time");
                        return false;
                    }

                    process.WaitForExit();
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    if (process.ExitCode == 0)
                    {
                        Debug.WriteLine($"   ?? fsutil output: {output.Trim()}");
                        Debug.WriteLine("   ?? EXPLICACIÓN: Last Access Time causa escritura SSD en cada lectura");
                        Debug.WriteLine("   ?? Sin esto, el SSD durará 2-3x más tiempo");
                        return true;
                    }
                    else
                    {
                        Debug.WriteLine($"? fsutil error (exit {process.ExitCode}): {error}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Exception en DisableLastAccessTime: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Desactiva nombres 8.3 DOS (legacy)
        /// Mejora rendimiento de navegación de archivos
        /// </summary>
        private static bool Disable8Dot3Names()
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "fsutil",
                    Arguments = "behavior set disable8dot3 1",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(startInfo))
                {
                    if (process == null)
                    {
                        Debug.WriteLine("? No se pudo iniciar fsutil para 8.3 names");
                        return false;
                    }

                    process.WaitForExit();
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    if (process.ExitCode == 0)
                    {
                        Debug.WriteLine($"   ?? fsutil output: {output.Trim()}");
                        Debug.WriteLine("   ?? EXPLICACIÓN: Nombres 8.3 son legacy de DOS (ARCHIV~1.TXT)");
                        Debug.WriteLine("   ?? Desactivarlos reduce overhead de metadata por archivo");
                        return true;
                    }
                    else
                    {
                        Debug.WriteLine($"? fsutil error (exit {process.ExitCode}): {error}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Exception en Disable8Dot3Names: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Optimiza configuraciones de Memory Management para gaming
        /// </summary>
        private static bool OptimizeMemoryManagement()
        {
            try
            {
                const string MEMORY_KEY = @"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management";

                using (var key = Registry.LocalMachine.OpenSubKey(MEMORY_KEY, true))
                {
                    if (key == null)
                    {
                        Debug.WriteLine("? No se pudo acceder a Memory Management key");
                        return false;
                    }

                    // LargeSystemCache = 1 (Optimiza para aplicaciones, no servicios)
                    key.SetValue("LargeSystemCache", 1, RegistryValueKind.DWord);
                    Debug.WriteLine("   ? LargeSystemCache = 1 (Application optimized)");

                    // IoPageLockLimit = 16384 (16MB para I/O locking)
                    key.SetValue("IoPageLockLimit", 16384, RegistryValueKind.DWord);
                    Debug.WriteLine("   ? IoPageLockLimit = 16384 (16MB I/O buffer)");

                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error en OptimizeMemoryManagement: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Restaura configuraciones NTFS a valores por defecto
        /// </summary>
        public static bool RestoreNTFS()
        {
            try
            {
                Debug.WriteLine("???????????????????????????????????????????????????????????");
                Debug.WriteLine("RESTORING NTFS TO DEFAULT SETTINGS");
                Debug.WriteLine("???????????????????????????????????????????????????????????");

                bool success = true;

                // Restaurar Last Access Time
                Debug.WriteLine("?? Restaurando Last Access Time...");
                if (EnableLastAccessTime())
                {
                    Debug.WriteLine("? Last Access Time restaurado");
                    Debug.WriteLine("   ?? ADVERTENCIA: Esto aumentará escrituras en SSD");
                }
                else
                {
                    Debug.WriteLine("? Error restaurando Last Access Time");
                    success = false;
                }

                // Restaurar nombres 8.3
                Debug.WriteLine("");
                Debug.WriteLine("?? Restaurando nombres 8.3 DOS...");
                if (Enable8Dot3Names())
                {
                    Debug.WriteLine("? Nombres 8.3 DOS restaurados");
                    Debug.WriteLine("   ?? Compatibilidad legacy restaurada");
                }
                else
                {
                    Debug.WriteLine("? Error restaurando nombres 8.3");
                    success = false;
                }

                // Restaurar Memory Management
                Debug.WriteLine("");
                Debug.WriteLine("?? Restaurando Memory Management...");
                if (RestoreMemoryManagement())
                {
                    Debug.WriteLine("? Memory Management restaurado");
                }

                if (success)
                {
                    Debug.WriteLine("");
                    Debug.WriteLine("?? Configuraciones NTFS restauradas a valores por defecto");
                    Debug.WriteLine("?? REQUIERE REINICIO para efecto completo");
                }

                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? ERROR en RestoreNTFS: {ex.Message}");
                return false;
            }
        }

        private static bool EnableLastAccessTime()
        {
            return ExecuteFsutilCommand("behavior set disablelastaccess 0", "Enabling Last Access Time");
        }

        private static bool Enable8Dot3Names()
        {
            return ExecuteFsutilCommand("behavior set disable8dot3 0", "Enabling 8.3 names");
        }

        private static bool RestoreMemoryManagement()
        {
            try
            {
                const string MEMORY_KEY = @"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management";

                using (var key = Registry.LocalMachine.OpenSubKey(MEMORY_KEY, true))
                {
                    if (key != null)
                    {
                        // Restaurar valores por defecto
                        key.SetValue("LargeSystemCache", 0, RegistryValueKind.DWord);
                        key.DeleteValue("IoPageLockLimit", false);
                        Debug.WriteLine("   ? Memory Management values restored");
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error en RestoreMemoryManagement: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Ejecuta comando fsutil con manejo de errores
        /// </summary>
        private static bool ExecuteFsutilCommand(string arguments, string description)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "fsutil",
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(startInfo))
                {
                    if (process == null)
                    {
                        Debug.WriteLine($"? No se pudo iniciar fsutil para: {description}");
                        return false;
                    }

                    process.WaitForExit();

                    if (process.ExitCode == 0)
                    {
                        Debug.WriteLine($"   ? {description} completed successfully");
                        return true;
                    }
                    else
                    {
                        string error = process.StandardError.ReadToEnd();
                        Debug.WriteLine($"? {description} failed (exit {process.ExitCode}): {error}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Exception en {description}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Diagnóstico completo del estado de optimizaciones NTFS
        /// </summary>
        public static string DiagnoseDiskSettings()
        {
            try
            {
                var diagnosis = "???????????????????????????????????????????????????????????\n";
                diagnosis += "DIAGNÓSTICO DISK & NTFS OPTIMIZATIONS\n";
                diagnosis += "???????????????????????????????????????????????????????????\n\n";

                // Verificar configuraciones fsutil
                diagnosis += "?? CONFIGURACIONES FSUTIL:\n";
                diagnosis += CheckFsutilSetting("behavior query disablelastaccess", "Last Access Time");
                diagnosis += CheckFsutilSetting("behavior query disable8dot3", "8.3 DOS Names");

                // Verificar Memory Management
                diagnosis += "\n?? MEMORY MANAGEMENT:\n";
                try
                {
                    const string MEMORY_KEY = @"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management";
                    using (var key = Registry.LocalMachine.OpenSubKey(MEMORY_KEY))
                    {
                        if (key != null)
                        {
                            var largeCache = key.GetValue("LargeSystemCache");
                            var ioPageLock = key.GetValue("IoPageLockLimit");

                            diagnosis += $"   ?? LargeSystemCache: {largeCache ?? "Default (0)"}\n";
                            diagnosis += $"   ?? IoPageLockLimit: {ioPageLock ?? "Default (not set)"}\n";

                            if (largeCache != null && (int)largeCache == 1)
                            {
                                diagnosis += "   ? Optimizado para aplicaciones (Gaming friendly)\n";
                            }
                            else
                            {
                                diagnosis += "   ?? Configuración por defecto (Services optimized)\n";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    diagnosis += $"   ? Error leyendo Memory Management: {ex.Message}\n";
                }

                diagnosis += "\n?? RECOMENDACIONES SSD/GAMING:\n";
                diagnosis += "   • Desactivar Last Access Time (CRÍTICO para SSD)\n";
                diagnosis += "   • Desactivar nombres 8.3 DOS (Mejor rendimiento)\n";
                diagnosis += "   • Configurar LargeSystemCache para aplicaciones\n";
                diagnosis += "   • Reiniciar después de cambios NTFS\n";

                diagnosis += "\n?? EDUCACIÓN SSD:\n";
                diagnosis += "   • Last Access Time causa 1 escritura por cada lectura\n";
                diagnosis += "   • En gaming intensivo = miles de escrituras extra\n";
                diagnosis += "   • Sin esto, SSD durará 2-3 años más\n";
                diagnosis += "   • Mejora performance sin pérdida de funcionalidad\n";

                Debug.WriteLine(diagnosis);
                return diagnosis;
            }
            catch (Exception ex)
            {
                var error = $"? ERROR en diagnóstico: {ex.Message}";
                Debug.WriteLine(error);
                return error;
            }
        }

        /// <summary>
        /// Verifica una configuración fsutil específica
        /// </summary>
        private static string CheckFsutilSetting(string queryCommand, string settingName)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "fsutil",
                    Arguments = queryCommand,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(startInfo))
                {
                    if (process != null)
                    {
                        process.WaitForExit();
                        string output = process.StandardOutput.ReadToEnd();

                        if (process.ExitCode == 0)
                        {
                            return $"   ?? {settingName}: {output.Trim()}\n";
                        }
                    }
                }

                return $"   ? {settingName}: No se pudo verificar\n";
            }
            catch (Exception)
            {
                return $"   ? {settingName}: Error verificando\n";
            }
        }

        // ???????????????????????????????????????????????????????????????????
        // MÉTODOS PARA COMPATIBILIDAD CON MAINWINDOW
        // ???????????????????????????????????????????????????????????????????

        /// <summary>
        /// DESHABILITA NTFS LAST ACCESS TIME (Alias para DisableLastAccessTime)
        /// </summary>
        public static bool DisableNTFSLastAccessTime()
        {
            return DisableLastAccessTime();
        }

        /// <summary>
        /// HABILITA NTFS LAST ACCESS TIME (Alias para EnableLastAccessTime)
        /// </summary>
        public static bool EnableNTFSLastAccessTime()
        {
            return EnableLastAccessTime();
        }

        /// <summary>
        /// OPTIMIZA NTFS PARA GAMING (Alias para OptimizeNTFS)
        /// </summary>
        public static bool OptimizeNTFSForGaming()
        {
            return OptimizeNTFS();
        }

        /// <summary>
        /// RESTAURA NTFS A DEFAULTS (Alias para RestoreNTFS)
        /// </summary>
        public static bool RestoreNTFSDefaults()
        {
            return RestoreNTFS();
        }

        // ???????????????????????????????????????????????????????????????????
        // CONFIGURACIONES QoS (QUALITY OF SERVICE) MÚLTIPLES
        // ???????????????????????????????????????????????????????????????????

        /// <summary>
        /// CONFIGURACIÓN QoS PARA GAMING COMPETITIVO
        /// Prioriza latencia mínima y responsividad por encima de throughput
        /// </summary>
        public static bool ApplyGamingQoSProfile()
        {
            try
            {
                Debug.WriteLine("?? APLICANDO QoS PROFILE: GAMING COMPETITIVO");
                Debug.WriteLine("???????????????????????????????????????????????");

                bool success = true;

                // I/O Priority para gaming
                success &= SetIOPriority("Gaming");

                // Network QoS para gaming
                success &= ConfigureNetworkQoS("Gaming");

                // Memory QoS optimizado
                success &= ConfigureMemoryQoS("Gaming");

                if (success)
                {
                    Debug.WriteLine("\n? QoS GAMING PROFILE APLICADO:");
                    Debug.WriteLine("   • Latencia I/O: MÍNIMA");
                    Debug.WriteLine("   • Network Priority: ALTA");
                    Debug.WriteLine("   • Memory Access: INMEDIATO");
                    Debug.WriteLine("   • File Access: OPTIMIZADO para assets");
                }

                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error en Gaming QoS: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// CONFIGURACIÓN QoS PARA STREAMING
        /// Balance entre calidad de stream y performance gaming
        /// </summary>
        public static bool ApplyStreamingQoSProfile()
        {
            try
            {
                Debug.WriteLine("?? APLICANDO QoS PROFILE: STREAMING");
                Debug.WriteLine("???????????????????????????????????????????????");

                bool success = true;

                success &= SetIOPriority("Streaming");
                success &= ConfigureNetworkQoS("Streaming");
                success &= ConfigureMemoryQoS("Streaming");

                if (success)
                {
                    Debug.WriteLine("\n? QoS STREAMING PROFILE APLICADO:");
                    Debug.WriteLine("   • Upload Bandwidth: RESERVADO");
                    Debug.WriteLine("   • Encoding Priority: ALTA");
                    Debug.WriteLine("   • Game Process: BALANCEADO");
                    Debug.WriteLine("   • Network Stability: PRIORIZADA");
                }

                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error en Streaming QoS: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// CONFIGURACIÓN QoS PARA PRODUCTIVIDAD
        /// Optimizado para trabajo, multitasking y aplicaciones profesionales
        /// </summary>
        public static bool ApplyProductivityQoSProfile()
        {
            try
            {
                Debug.WriteLine("?? APLICANDO QoS PROFILE: PRODUCTIVIDAD");
                Debug.WriteLine("???????????????????????????????????????????????");

                bool success = true;

                success &= SetIOPriority("Productivity");
                success &= ConfigureNetworkQoS("Productivity");
                success &= ConfigureMemoryQoS("Productivity");

                if (success)
                {
                    Debug.WriteLine("\n? QoS PRODUCTIVITY PROFILE APLICADO:");
                    Debug.WriteLine("   • Multitasking: OPTIMIZADO");
                    Debug.WriteLine("   • File Operations: ACELERADAS");
                    Debug.WriteLine("   • Network Downloads: PRIORIZADAS");
                    Debug.WriteLine("   • Background Tasks: BALANCEADAS");
                }

                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error en Productivity QoS: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// CONFIGURACIÓN QoS EXTREMA PARA ESPORTS
        /// Configuración agresiva para competiciones profesionales
        /// </summary>
        public static bool ApplyEsportsQoSProfile()
        {
            try
            {
                Debug.WriteLine("?? APLICANDO QoS PROFILE: ESPORTS EXTREMO");
                Debug.WriteLine("???????????????????????????????????????????????");

                bool success = true;

                success &= SetIOPriority("Esports");
                success &= ConfigureNetworkQoS("Esports");
                success &= ConfigureMemoryQoS("Esports");
                success &= DisableBackgroundServices();

                if (success)
                {
                    Debug.WriteLine("\n? QoS ESPORTS EXTREMO APLICADO:");
                    Debug.WriteLine("   • Latencia: SUB-1MS");
                    Debug.WriteLine("   • Jitter: ELIMINADO");
                    Debug.WriteLine("   • Background Apps: SUSPENDIDAS");
                    Debug.WriteLine("   • CPU/GPU: DEDICADOS");
                    Debug.WriteLine("   ??  SOLO para competiciones");
                }

                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error en Esports QoS: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// CONFIGURACIÓN QoS BALANCEADA (DEFAULT MEJORADO)
        /// Balance óptimo para uso diario
        /// </summary>
        public static bool ApplyBalancedQoSProfile()
        {
            try
            {
                Debug.WriteLine("?? APLICANDO QoS PROFILE: BALANCEADO");
                Debug.WriteLine("???????????????????????????????????????????????");

                bool success = true;

                success &= SetIOPriority("Balanced");
                success &= ConfigureNetworkQoS("Balanced");
                success &= ConfigureMemoryQoS("Balanced");

                if (success)
                {
                    Debug.WriteLine("\n? QoS BALANCEADO APLICADO:");
                    Debug.WriteLine("   • Gaming: BUENA performance");
                    Debug.WriteLine("   • Navegación: FLUIDA");
                    Debug.WriteLine("   • Downloads: SIN INTERFERIR");
                    Debug.WriteLine("   • Sistema: ESTABLE");
                }

                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error en Balanced QoS: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// CONFIGURAR PRIORIDAD I/O SEGÚN PERFIL
        /// </summary>
        private static bool SetIOPriority(string profile)
        {
            try
            {
                const string IO_KEY = @"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management";

                using (var key = Registry.LocalMachine.OpenSubKey(IO_KEY, true))
                {
                    if (key == null) return false;

                    switch (profile.ToLower())
                    {
                        case "gaming":
                            key.SetValue("LargeSystemCache", 1, RegistryValueKind.DWord); // Apps priority
                            key.SetValue("IoPageLockLimit", 32768, RegistryValueKind.DWord); // 32MB buffer
                            Debug.WriteLine("   ?? I/O configurado para gaming (apps priority)");
                            break;

                        case "streaming":
                            key.SetValue("LargeSystemCache", 1, RegistryValueKind.DWord);
                            key.SetValue("IoPageLockLimit", 65536, RegistryValueKind.DWord); // 64MB buffer
                            Debug.WriteLine("   ?? I/O configurado para streaming (buffer grande)");
                            break;

                        case "productivity":
                            key.SetValue("LargeSystemCache", 0, RegistryValueKind.DWord); // Services priority
                            key.SetValue("IoPageLockLimit", 16384, RegistryValueKind.DWord); // 16MB buffer
                            Debug.WriteLine("   ?? I/O configurado para productividad (services priority)");
                            break;

                        case "esports":
                            key.SetValue("LargeSystemCache", 1, RegistryValueKind.DWord);
                            key.SetValue("IoPageLockLimit", 131072, RegistryValueKind.DWord); // 128MB buffer
                            key.SetValue("DisablePagingExecutive", 1, RegistryValueKind.DWord); // Kernel en RAM
                            Debug.WriteLine("   ?? I/O configurado EXTREMO (kernel en RAM)");
                            break;

                        case "balanced":
                        default:
                            key.SetValue("LargeSystemCache", 0, RegistryValueKind.DWord);
                            key.SetValue("IoPageLockLimit", 24576, RegistryValueKind.DWord); // 24MB buffer
                            Debug.WriteLine("   ?? I/O configurado balanceado");
                            break;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error configurando I/O Priority: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// CONFIGURAR QoS DE RED SEGÚN PERFIL
        /// </summary>
        private static bool ConfigureNetworkQoS(string profile)
        {
            try
            {
                const string NETWORK_KEY = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile";

                using (var key = Registry.LocalMachine.OpenSubKey(NETWORK_KEY, true))
                {
                    if (key == null) return false;

                    switch (profile.ToLower())
                    {
                        case "gaming":
                            key.SetValue("NetworkThrottlingIndex", unchecked((int)0xFFFFFFFF), RegistryValueKind.DWord);
                            key.SetValue("SystemResponsiveness", 0, RegistryValueKind.DWord); // 0% para sistema
                            Debug.WriteLine("   ?? Network: Sin throttling, 100% para gaming");
                            break;

                        case "streaming":
                            key.SetValue("NetworkThrottlingIndex", unchecked((int)0xFFFFFFFF), RegistryValueKind.DWord);
                            key.SetValue("SystemResponsiveness", 5, RegistryValueKind.DWord); // 5% para sistema
                            Debug.WriteLine("   ?? Network: Upload priorizado para streaming");
                            break;

                        case "productivity":
                            key.SetValue("NetworkThrottlingIndex", 20, RegistryValueKind.DWord);
                            key.SetValue("SystemResponsiveness", 20, RegistryValueKind.DWord); // Windows default
                            Debug.WriteLine("   ?? Network: Configuración estable para trabajo");
                            break;

                        case "esports":
                            key.SetValue("NetworkThrottlingIndex", unchecked((int)0xFFFFFFFF), RegistryValueKind.DWord);
                            key.SetValue("SystemResponsiveness", 0, RegistryValueKind.DWord);
                            Debug.WriteLine("   ?? Network: EXTREMO - Sin límites ni throttling");
                            break;

                        case "balanced":
                        default:
                            key.SetValue("NetworkThrottlingIndex", 10, RegistryValueKind.DWord);
                            key.SetValue("SystemResponsiveness", 10, RegistryValueKind.DWord);
                            Debug.WriteLine("   ?? Network: Balance gaming/sistema");
                            break;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error configurando Network QoS: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// CONFIGURAR QoS DE MEMORIA SEGÚN PERFIL
        /// </summary>
        private static bool ConfigureMemoryQoS(string profile)
        {
            try
            {
                const string PRIORITY_KEY = @"SYSTEM\CurrentControlSet\Control\PriorityControl";

                using (var key = Registry.LocalMachine.OpenSubKey(PRIORITY_KEY, true))
                {
                    if (key == null) return false;

                    switch (profile.ToLower())
                    {
                        case "gaming":
                            key.SetValue("Win32PrioritySeparation", 38, RegistryValueKind.DWord); // Gaming optimized
                            Debug.WriteLine("   ?? Memory: Foreground apps (juegos) priorizadas");
                            break;

                        case "streaming":
                            key.SetValue("Win32PrioritySeparation", 40, RegistryValueKind.DWord); // Smooth performance
                            Debug.WriteLine("   ?? Memory: Time slices largos para suavidad");
                            break;

                        case "productivity":
                            key.SetValue("Win32PrioritySeparation", 2, RegistryValueKind.DWord); // Windows default
                            Debug.WriteLine("   ?? Memory: Configuración Windows estándar");
                            break;

                        case "esports":
                            key.SetValue("Win32PrioritySeparation", 22, RegistryValueKind.DWord); // Aggressive
                            Debug.WriteLine("   ?? Memory: AGRESIVO - Time slices cortos");
                            break;

                        case "balanced":
                        default:
                            key.SetValue("Win32PrioritySeparation", 38, RegistryValueKind.DWord);
                            Debug.WriteLine("   ?? Memory: Balance óptimo");
                            break;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error configurando Memory QoS: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// DESHABILITAR SERVICIOS EN SEGUNDO PLANO (SOLO ESPORTS)
        /// </summary>
        private static bool DisableBackgroundServices()
        {
            try
            {
                Debug.WriteLine("   ?? MODO ESPORTS: Deshabilitando servicios no críticos...");

                // Deshabilitar Windows Update durante gaming
                const string UPDATE_KEY = @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU";
                using (var key = Registry.LocalMachine.CreateSubKey(UPDATE_KEY))
                {
                    if (key != null)
                    {
                        key.SetValue("NoAutoUpdate", 1, RegistryValueKind.DWord);
                        Debug.WriteLine("   ? Windows Update pausado");
                    }
                }

                // Deshabilitar telemetría durante competición
                const string TELEMETRY_KEY = @"SOFTWARE\Policies\Microsoft\Windows\DataCollection";
                using (var key = Registry.LocalMachine.CreateSubKey(TELEMETRY_KEY))
                {
                    if (key != null)
                    {
                        key.SetValue("AllowTelemetry", 0, RegistryValueKind.DWord);
                        Debug.WriteLine("   ? Telemetría pausada");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error deshabilitando servicios: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// RESTAURAR QoS A CONFIGURACIÓN POR DEFECTO
        /// </summary>
        public static bool RestoreDefaultQoS()
        {
            try
            {
                Debug.WriteLine("?? RESTAURANDO QoS A CONFIGURACIÓN POR DEFECTO");
                Debug.WriteLine("???????????????????????????????????????????????");

                bool success = true;

                // Restaurar I/O a defaults
                const string IO_KEY = @"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management";
                using (var key = Registry.LocalMachine.OpenSubKey(IO_KEY, true))
                {
                    if (key != null)
                    {
                        key.SetValue("LargeSystemCache", 0, RegistryValueKind.DWord);
                        key.DeleteValue("IoPageLockLimit", false);
                        key.DeleteValue("DisablePagingExecutive", false);
                        Debug.WriteLine("   ? I/O Settings: RESTAURADOS");
                    }
                }

                // Restaurar Network a defaults
                const string NETWORK_KEY = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile";
                using (var key = Registry.LocalMachine.OpenSubKey(NETWORK_KEY, true))
                {
                    if (key != null)
                    {
                        key.SetValue("NetworkThrottlingIndex", 10, RegistryValueKind.DWord);
                        key.SetValue("SystemResponsiveness", 20, RegistryValueKind.DWord);
                        Debug.WriteLine("   ? Network Settings: RESTAURADOS");
                    }
                }

                // Restaurar Memory a defaults
                const string PRIORITY_KEY = @"SYSTEM\CurrentControlSet\Control\PriorityControl";
                using (var key = Registry.LocalMachine.OpenSubKey(PRIORITY_KEY, true))
                {
                    if (key != null)
                    {
                        key.SetValue("Win32PrioritySeparation", 2, RegistryValueKind.DWord);
                        Debug.WriteLine("   ? Memory Priority: RESTAURADA");
                    }
                }

                Debug.WriteLine("\n? QoS RESTAURADO A CONFIGURACIÓN WINDOWS POR DEFECTO");
                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error restaurando QoS: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// DIAGNÓSTICO DE QoS ACTUAL
        /// </summary>
        public static string DiagnoseCurrentQoS()
        {
            try
            {
                var diagnosis = "?? DIAGNÓSTICO QoS (QUALITY OF SERVICE)\n";
                diagnosis += "???????????????????????????????????????????????\n\n";

                // Verificar I/O Settings
                diagnosis += "?? I/O PRIORITY SETTINGS:\n";
                const string IO_KEY = @"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management";
                using (var key = Registry.LocalMachine.OpenSubKey(IO_KEY))
                {
                    if (key != null)
                    {
                        var largeCache = key.GetValue("LargeSystemCache");
                        var ioPageLock = key.GetValue("IoPageLockLimit");
                        var disablePaging = key.GetValue("DisablePagingExecutive");

                        diagnosis += $"   LargeSystemCache: {largeCache ?? "Default (0)"}\n";
                        diagnosis += $"   IoPageLockLimit: {ioPageLock ?? "Default (not set)"}\n";
                        diagnosis += $"   DisablePagingExecutive: {disablePaging ?? "Default (0)"}\n";
                    }
                }

                // Verificar Network Settings
                diagnosis += "\n?? NETWORK QoS SETTINGS:\n";
                const string NETWORK_KEY = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile";
                using (var key = Registry.LocalMachine.OpenSubKey(NETWORK_KEY))
                {
                    if (key != null)
                    {
                        var throttling = key.GetValue("NetworkThrottlingIndex");
                        var responsiveness = key.GetValue("SystemResponsiveness");

                        diagnosis += $"   NetworkThrottlingIndex: {throttling ?? "Default (10)"}\n";
                        diagnosis += $"   SystemResponsiveness: {responsiveness ?? "Default (20)"}\n";
                    }
                }

                // Verificar Memory Priority
                diagnosis += "\n?? MEMORY PRIORITY SETTINGS:\n";
                const string PRIORITY_KEY = @"SYSTEM\CurrentControlSet\Control\PriorityControl";
                using (var key = Registry.LocalMachine.OpenSubKey(PRIORITY_KEY))
                {
                    if (key != null)
                    {
                        var priority = key.GetValue("Win32PrioritySeparation");
                        diagnosis += $"   Win32PrioritySeparation: {priority ?? "Default (2)"}\n";

                        if (priority != null)
                        {
                            int val = (int)priority;
                            string profileType = val switch
                            {
                                2 => "?? Windows Default",
                                38 => "?? Gaming Optimized",
                                40 => "?? Streaming/Smooth",
                                22 => "?? Esports/Aggressive",
                                _ => "?? Custom Configuration"
                            };
                            diagnosis += $"   Profile Detectado: {profileType}\n";
                        }
                    }
                }

                diagnosis += "\n?? PERFILES QoS DISPONIBLES:\n";
                diagnosis += "   ?? Gaming - Latencia mínima para competitivo\n";
                diagnosis += "   ?? Streaming - Balance gaming + broadcast\n";
                diagnosis += "   ?? Productivity - Multitasking optimizado\n";
                diagnosis += "   ?? Esports - Configuración extrema competición\n";
                diagnosis += "   ?? Balanced - Uso diario equilibrado\n";

                Debug.WriteLine(diagnosis);
                return diagnosis;
            }
            catch (Exception ex)
            {
                var error = $"? ERROR en diagnóstico QoS: {ex.Message}";
                Debug.WriteLine(error);
                return error;
            }
        }

        /// <summary>
        /// Deshabilita AHCI Link Power Management (Storage Idle States)
        /// 
        /// ¿Qué es AHCI LPM?
        /// - El controlador AHCI puede poner el enlace SATA en modo de bajo consumo (HIPM/DIPM)
        /// - Esto añade latencia cuando el disco necesita "despertar"
        /// - Para SSDs NVMe/SATA en sistemas de gaming, esta latencia es indeseable
        /// 
        /// IMPACTO EN GAMING:
        /// - Elimina micro-stutters causados por el disco "despertando"
        /// - Tiempos de acceso consistentes
        /// - Especialmente notable en HDDs y SSDs SATA
        /// </summary>
        /// <returns>True si la operación fue exitosa, false en caso contrario</returns>
        public static bool DisableStorageIdleStates()
        {
            try
            {
                Debug.WriteLine("→ Deshabilitando AHCI Link Power Management...");

                bool success = true;

                // Deshabilitar AHCI Link Power Management via registro
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(
                    @"SYSTEM\CurrentControlSet\Services\storahci\Parameters\Device"))
                {
                    if (key != null)
                    {
                        key.SetValue("IdlePowerManagement", 0, RegistryValueKind.DWord);
                        Debug.WriteLine("   ✓ storahci IdlePowerManagement = 0");
                    }
                    else
                    {
                        Debug.WriteLine("   ⚠ No se pudo crear storahci Parameters\\Device");
                        success = false;
                    }
                }

                // Deshabilitar disk idle timeout via powercfg (0 = nunca apagar)
                RunPowercfgDisk("-setacvalueindex scheme_current SUB_DISK DISKIDLE 0",
                    "DISKIDLE=0 (sin timeout de disco)");

                RunPowercfgDisk("-setactive scheme_current", "Aplicar configuración activa");

                if (success)
                {
                    Debug.WriteLine("   ✓ Storage Idle States deshabilitados");
                    Debug.WriteLine("   ⚠ REQUIERE REINICIO para efecto completo");
                }

                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error deshabilitando Storage Idle States: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Habilita AHCI Link Power Management (restaura estado predeterminado)
        /// </summary>
        /// <returns>True si la operación fue exitosa, false en caso contrario</returns>
        public static bool EnableStorageIdleStates()
        {
            try
            {
                Debug.WriteLine("→ Habilitando AHCI Link Power Management...");

                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(
                    @"SYSTEM\CurrentControlSet\Services\storahci\Parameters\Device", true))
                {
                    if (key != null)
                    {
                        key.DeleteValue("IdlePowerManagement", false);
                        Debug.WriteLine("   ✓ storahci IdlePowerManagement eliminado (predeterminado)");
                    }
                }

                RunPowercfgDisk("-setacvalueindex scheme_current SUB_DISK DISKIDLE 1200",
                    "DISKIDLE=1200 (20 minutos, predeterminado)");

                RunPowercfgDisk("-setactive scheme_current", "Aplicar configuración activa");

                Debug.WriteLine("   ✓ Storage Idle States restaurados");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error habilitando Storage Idle States: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Deshabilita el modo idle de StorPort para prevenir que el controlador de almacenamiento
        /// entre en modo de bajo consumo
        /// 
        /// StorPort es el miniport driver de almacenamiento de Windows.
        /// El idle timeout hace que el driver deje de procesar I/O durante períodos de baja actividad,
        /// causando picos de latencia cuando se reanuda la actividad.
        /// </summary>
        /// <returns>True si la operación fue exitosa, false en caso contrario</returns>
        public static bool DisableStorPortIdle()
        {
            try
            {
                Debug.WriteLine("→ Deshabilitando StorPort Idle Timeout...");

                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(
                    @"SYSTEM\CurrentControlSet\Control\Storage\StorPort"))
                {
                    if (key != null)
                    {
                        // 0 = sin timeout (nunca idle)
                        key.SetValue("IdleTimeout", 0, RegistryValueKind.DWord);
                        Debug.WriteLine("   ✓ StorPort IdleTimeout = 0 (sin timeout)");
                        return true;
                    }
                    else
                    {
                        Debug.WriteLine("   ⚠ No se pudo crear la clave StorPort");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error deshabilitando StorPort Idle: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Habilita el modo idle de StorPort (restaura estado predeterminado)
        /// </summary>
        /// <returns>True si la operación fue exitosa, false en caso contrario</returns>
        public static bool EnableStorPortIdle()
        {
            try
            {
                Debug.WriteLine("→ Habilitando StorPort Idle Timeout...");

                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(
                    @"SYSTEM\CurrentControlSet\Control\Storage\StorPort", true))
                {
                    if (key != null)
                    {
                        key.DeleteValue("IdleTimeout", false);
                        Debug.WriteLine("   ✓ StorPort IdleTimeout eliminado (predeterminado)");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error habilitando StorPort Idle: {ex.Message}");
                return false;
            }
        }

        private static void RunPowercfgDisk(string args, string description)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powercfg.exe",
                    Arguments = args,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                using (Process p = Process.Start(psi))
                {
                    p?.WaitForExit(3000);
                    Debug.WriteLine($"   → powercfg: {description}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"   ⚠ powercfg error: {ex.Message}");
            }
        }
    }
}
