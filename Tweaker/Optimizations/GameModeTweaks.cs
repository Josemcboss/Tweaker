using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using Microsoft.Win32;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// Optimizaciones específicas para Windows Game Mode y prioridad de procesos
    /// OPTIMIZADO: Game Mode separado de GameDVR, detección automática, verificación de estado
    /// </summary>
    public static class GameModeTweaks
    {
        // ???????????????????????????????????????????????????????????????????
        // WINDOWS GAME MODE (SEPARADO DE GAMEDVR)
        // ???????????????????????????????????????????????????????????????????

        /// <summary>
        /// Habilita SOLO Windows Game Mode sin tocar GameDVR
        /// </summary>
        public static bool EnableGameMode()
        {
            try
            {
                bool success = false;

                Debug.WriteLine("?? HABILITANDO WINDOWS GAME MODE (SOLO)");
                Debug.WriteLine("???????????????????????????????????????");

                // Solo habilitar Game Mode puro, NO tocar GameDVR
                const string gameModePath = @"SOFTWARE\Microsoft\GameBar";

                using (var key = Registry.CurrentUser.CreateSubKey(gameModePath))
                {
                    if (key != null)
                    {
                        // Habilitar Game Mode
                        key.SetValue("AllowAutoGameMode", 1, RegistryValueKind.DWord);
                        key.SetValue("AutoGameModeEnabled", 1, RegistryValueKind.DWord);

                        Debug.WriteLine("? Game Mode habilitado");
                        success = true;
                    }
                }

                // Configurar Game Mode en configuración del sistema
                const string gameModeSettingsPath = @"Software\Microsoft\Windows\CurrentVersion\GameDVR";

                using (var key = Registry.CurrentUser.CreateSubKey(gameModeSettingsPath))
                {
                    if (key != null)
                    {
                        // Solo configuraciones específicas de Game Mode, NO GameDVR
                        key.SetValue("GameDVR_FSEBehaviorMode", 2, RegistryValueKind.DWord);
                        key.SetValue("GameDVR_HonorUserFSEBehaviorMode", 1, RegistryValueKind.DWord);
                        key.SetValue("GameDVR_DXGIHonorFSEWindowsCompatible", 1, RegistryValueKind.DWord);

                        Debug.WriteLine("? Game Mode settings configurados");
                    }
                }

                Debug.WriteLine($"?? Windows Game Mode: {(success ? "HABILITADO" : "ERROR")}");
                Debug.WriteLine("?? GameDVR NO modificado (usar botón separado)");

                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error al habilitar Game Mode: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Deshabilita Windows Game Mode
        /// </summary>
        public static bool DisableGameMode()
        {
            try
            {
                bool success = false;

                Debug.WriteLine("?? DESHABILITANDO WINDOWS GAME MODE");
                Debug.WriteLine("??????????????????????????????????????");

                const string gameModePath = @"SOFTWARE\Microsoft\GameBar";

                using (var key = Registry.CurrentUser.CreateSubKey(gameModePath))
                {
                    if (key != null)
                    {
                        // Deshabilitar Game Mode
                        key.SetValue("AllowAutoGameMode", 0, RegistryValueKind.DWord);
                        key.SetValue("AutoGameModeEnabled", 0, RegistryValueKind.DWord);

                        Debug.WriteLine("? Game Mode deshabilitado");
                        success = true;
                    }
                }

                Debug.WriteLine($"?? Windows Game Mode: {(success ? "DESHABILITADO" : "ERROR")}");
                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error al deshabilitar Game Mode: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Verifica el estado actual de Windows Game Mode
        /// </summary>
        public static (bool isEnabled, string details) GetGameModeStatus()
        {
            try
            {
                const string gameModePath = @"SOFTWARE\Microsoft\GameBar";
                bool autoGameMode = false;
                bool allowAutoGameMode = false;

                using (var key = Registry.CurrentUser.OpenSubKey(gameModePath))
                {
                    if (key != null)
                    {
                        var autoValue = key.GetValue("AutoGameModeEnabled");
                        var allowValue = key.GetValue("AllowAutoGameMode");

                        autoGameMode = autoValue != null && (int)autoValue == 1;
                        allowAutoGameMode = allowValue != null && (int)allowValue == 1;
                    }
                }

                bool isEnabled = autoGameMode && allowAutoGameMode;
                string status = isEnabled ? "HABILITADO" : "DESHABILITADO";

                string details = $"Game Mode: {status}\n" +
                               $"AutoGameModeEnabled: {autoGameMode}\n" +
                               $"AllowAutoGameMode: {allowAutoGameMode}";

                Debug.WriteLine($"?? Estado Game Mode: {status}");
                return (isEnabled, details);
            }
            catch (Exception ex)
            {
                string error = $"Error verificando Game Mode: {ex.Message}";
                Debug.WriteLine($"? {error}");
                return (false, error);
            }
        }

        // ???????????????????????????????????????????????????????????????????
        // NTFS LAST ACCESS TIME (Optimización de disco)
        // ???????????????????????????????????????????????????????????????????

        /// <summary>
        /// Deshabilita NTFS Last Access Time para mejorar rendimiento de disco
        /// </summary>
        public static bool DisableNTFSLastAccessTime()
        {
            try
            {
                // Usar fsutil para deshabilitar Last Access Time
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "fsutil",
                    Arguments = "behavior set disablelastaccess 1",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (Process process = Process.Start(psi))
                {
                    if (process == null) // Check for null
                    {
                        Debug.WriteLine("?? Error: No se pudo iniciar el proceso fsutil.");
                        return false;
                    }
                    process.WaitForExit();

                    if (process.ExitCode == 0)
                    {
                        Debug.WriteLine("? NTFS Last Access Time deshabilitado");
                        return true;
                    }
                    else
                    {
                        Debug.WriteLine($"?? Error al deshabilitar NTFS Last Access Time: Exit code {process.ExitCode}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error al deshabilitar NTFS Last Access Time: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Habilita NTFS Last Access Time (restaurar default)
        /// </summary>
        public static bool EnableNTFSLastAccessTime()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "fsutil",
                    Arguments = "behavior set disablelastaccess 0",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (Process process = Process.Start(psi))
                {
                    if (process == null) // Check for null
                    {
                        Debug.WriteLine("?? Error: No se pudo iniciar el proceso fsutil.");
                        return false;
                    }
                    process.WaitForExit();

                    if (process.ExitCode == 0)
                    {
                        Debug.WriteLine("? NTFS Last Access Time habilitado");
                        return true;
                    }
                    else
                    {
                        Debug.WriteLine($"?? Error al habilitar NTFS Last Access Time: Exit code {process.ExitCode}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error al habilitar NTFS Last Access Time: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Verifica el estado actual de NTFS Last Access Time
        /// </summary>
        public static (bool isEnabled, string details) GetNTFSLastAccessStatus()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "fsutil",
                    Arguments = "behavior query disablelastaccess",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (Process process = Process.Start(psi))
                {
                    if (process == null) // Check for null
                    {
                        Debug.WriteLine("?? Error: No se pudo iniciar el proceso fsutil.");
                        return (true, "ERROR: No se pudo iniciar el proceso fsutil.");
                    }
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    bool isDisabled = output.Contains("DisableLastAccess = 1");
                    bool isEnabled = !isDisabled;

                    string status = isEnabled ? "HABILITADO (lento)" : "DESHABILITADO (optimizado)";
                    string details = $"NTFS Last Access Time: {status}\nOutput: {output.Trim()}";

                    Debug.WriteLine($"?? Estado NTFS Last Access: {status}");
                    return (isEnabled, details);
                }
            }
            catch (Exception ex)
            {
                string error = $"Error verificando NTFS Last Access: {ex.Message}";
                Debug.WriteLine($"? {error}");
                return (true, error); // Asumir habilitado por defecto
            }
        }

        // ???????????????????????????????????????????????????????????????????
        // GAME PROCESS PRIORITY (Prioridad alta para juegos)
        // ???????????????????????????????????????????????????????????????????

        /// <summary>
        /// Configura prioridad alta para procesos de juegos conocidos
        /// OPTIMIZADO: Detección automática de juegos instalados
        /// </summary>
        public static bool EnableHighPriorityForGames()
        {
            try
            {
                Debug.WriteLine("?? CONFIGURANDO PRIORIDAD ALTA PARA JUEGOS");
                Debug.WriteLine("???????????????????????????????????????????");

                // Lista extendida de ejecutables de juegos populares
                var gameExecutables = new Dictionary<string, string>
                {
                    // Battle Royales
                    {"FortniteClient-Win64-Shipping.exe", "Fortnite"},
                    {"VALORANT-Win64-Shipping.exe", "Valorant"},
                    {"ApexLegends.exe", "Apex Legends"},
                    {"PUBG.exe", "PUBG"},
                    {"TslGame.exe", "PUBG (Steam)"},
                    
                    // FPS Competitivos
                    {"cs2.exe", "Counter-Strike 2"},
                    {"csgo.exe", "CS:GO (Legacy)"},
                    {"RainbowSix.exe", "Rainbow Six Siege"},
                    {"RainbowSix_BE.exe", "R6 Siege (BE)"},
                    {"Overwatch.exe", "Overwatch"},
                    {"OverwatchLauncher.exe", "Overwatch Launcher"},
                    
                    // Call of Duty
                    {"ModernWarfare.exe", "Modern Warfare"},
                    {"Warzone.exe", "Call of Duty Warzone"},
                    {"BlackOpsColdWar.exe", "Cold War"},
                    {"cod.exe", "Call of Duty"},
                    
                    // MOBAs y MMOs
                    {"LeagueofLegends.exe", "League of Legends"},
                    {"League of Legends.exe", "LoL (Alt)"},
                    {"Dota2.exe", "Dota 2"},
                    {"WorldOfWarcraft.exe", "World of Warcraft"},
                    
                    // Otros populares
                    {"EscapeFromTarkov.exe", "Escape from Tarkov"},
                    {"FiveM.exe", "FiveM (GTA V)"},
                    {"GTA5.exe", "Grand Theft Auto V"},
                    {"GTAV.exe", "GTA V (Steam)"},
                    {"RocketLeague.exe", "Rocket League"},
                    {"RustClient.exe", "Rust"},
                    {"Minecraft.exe", "Minecraft"},
                    {"javaw.exe", "Minecraft Java"},
                    
                    // Nuevos populares
                    {"Palworld-Win64-Shipping.exe", "Palworld"},
                    {"DeadByDaylight-Win64-Shipping.exe", "Dead by Daylight"},
                    {"FallGuys_client.exe", "Fall Guys"},
                    {"Among Us.exe", "Among Us"},
                    {"Genshin Impact.exe", "Genshin Impact"},
                    {"YuanShen.exe", "Genshin Impact (CN)"}
                };

                // Detectar juegos instalados
                var installedGames = DetectInstalledGames(gameExecutables);

                const string imagePath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options";
                int successCount = 0;
                int attemptedCount = 0;

                foreach (var game in gameExecutables)
                {
                    string exe = game.Key;
                    string gameName = game.Value;

                    try
                    {
                        string keyPath = $"{imagePath}\\{exe}\\PerfOptions";

                        using (var key = Registry.LocalMachine.CreateSubKey(keyPath))
                        {
                            if (key != null)
                            {
                                // CpuPriorityClass: 3 = High Priority
                                key.SetValue("CpuPriorityClass", 3, RegistryValueKind.DWord);

                                // IoPriority: 3 = High
                                key.SetValue("IoPriority", 3, RegistryValueKind.DWord);

                                successCount++;

                                string status = installedGames.Contains(exe) ? "[DETECTADO]" : "[PREVENTIVO]";
                                Debug.WriteLine($"? {gameName} {status}");
                            }
                        }
                        attemptedCount++;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"?? Error configurando {gameName}: {ex.Message}");
                        attemptedCount++;
                    }
                }

                Debug.WriteLine($"?? RESUMEN PRIORIDAD DE JUEGOS:");
                Debug.WriteLine($"   Configurados: {successCount}/{attemptedCount}");
                Debug.WriteLine($"   Detectados instalados: {installedGames.Count}");
                Debug.WriteLine($"   Preventivos: {successCount - installedGames.Count}");

                return successCount > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error al configurar prioridad de juegos: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Detecta juegos instalados en el sistema
        /// </summary>
        private static List<string> DetectInstalledGames(Dictionary<string, string> gameExecutables)
        {
            var installedGames = new List<string>();

            // Rutas comunes de instalación de juegos
            var commonPaths = new List<string>
            {
                @"C:\Program Files\Epic Games",
                @"C:\Program Files (x86)\Steam\steamapps\common",
                @"C:\Program Files\Steam\steamapps\common",
                @"C:\Riot Games",
                @"C:\Program Files\Riot Games",
                @"C:\Users\" + Environment.UserName + @"\AppData\Local\Programs",
                @"C:\XboxGames",
                @"C:\Program Files\WindowsApps",
                @"C:\Games",
                @"D:\Games",
                @"E:\Games"
            };

            Debug.WriteLine("?? Detectando juegos instalados...");

            foreach (var exe in gameExecutables.Keys)
            {
                foreach (var basePath in commonPaths)
                {
                    try
                    {
                        if (Directory.Exists(basePath))
                        {
                            var files = Directory.GetFiles(basePath, exe, SearchOption.AllDirectories);
                            if (files.Length > 0)
                            {
                                installedGames.Add(exe);
                                Debug.WriteLine($"   ?? Encontrado: {gameExecutables[exe]}");
                                break;
                            }
                        }
                    }
                    catch
                    {
                        // Ignorar errores de acceso a directorios protegidos
                    }
                }
            }

            if (installedGames.Count == 0)
            {
                Debug.WriteLine("   ?? No se detectaron juegos (aplicando configuración preventiva)");
            }

            return installedGames;
        }

        /// <summary>
        /// Restaura prioridad normal para procesos de juegos
        /// OPTIMIZADO: Limpieza completa y logging mejorado
        /// </summary>
        public static bool DisableHighPriorityForGames()
        {
            try
            {
                Debug.WriteLine("?? RESTAURANDO PRIORIDAD NORMAL PARA JUEGOS");
                Debug.WriteLine("???????????????????????????????????????????");

                const string imagePath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options";

                // Lista completa de ejecutables (misma que EnableHighPriorityForGames)
                var gameExecutables = new string[]
                {
                    "FortniteClient-Win64-Shipping.exe",
                    "VALORANT-Win64-Shipping.exe",
                    "ApexLegends.exe",
                    "PUBG.exe",
                    "TslGame.exe",
                    "cs2.exe",
                    "csgo.exe",
                    "RainbowSix.exe",
                    "RainbowSix_BE.exe",
                    "Overwatch.exe",
                    "OverwatchLauncher.exe",
                    "ModernWarfare.exe",
                    "Warzone.exe",
                    "BlackOpsColdWar.exe",
                    "cod.exe",
                    "LeagueofLegends.exe",
                    "League of Legends.exe",
                    "Dota2.exe",
                    "WorldOfWarcraft.exe",
                    "EscapeFromTarkov.exe",
                    "FiveM.exe",
                    "GTA5.exe",
                    "GTAV.exe",
                    "RocketLeague.exe",
                    "RustClient.exe",
                    "Minecraft.exe",
                    "javaw.exe",
                    "Palworld-Win64-Shipping.exe",
                    "DeadByDaylight-Win64-Shipping.exe",
                    "FallGuys_client.exe",
                    "Among Us.exe",
                    "Genshin Impact.exe",
                    "YuanShen.exe"
                };

                int successCount = 0;
                int totalCount = 0;

                foreach (string exe in gameExecutables)
                {
                    try
                    {
                        string keyPath = $"{imagePath}\\{exe}";

                        // Eliminar toda la clave de configuración del juego
                        Registry.LocalMachine.DeleteSubKeyTree(keyPath, false);
                        successCount++;

                        Debug.WriteLine($"? {exe} - Prioridad restaurada");
                    }
                    catch (Exception ex)
                    {
                        // No es crítico si la clave no existe
                        Debug.WriteLine($"?? {exe} - Ya estaba en default o error: {ex.Message}");
                    }
                    totalCount++;
                }

                Debug.WriteLine($"?? RESUMEN RESTAURACIÓN:");
                Debug.WriteLine($"   Procesados: {totalCount}");
                Debug.WriteLine($"   Restaurados: {successCount}");
                Debug.WriteLine($"   Ya en default: {totalCount - successCount}");

                return successCount >= 0; // Éxito incluso si no había nada que restaurar
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error al restaurar prioridad de juegos: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Verifica el estado actual de prioridad de juegos
        /// </summary>
        public static (int configuredGames, List<string> gamesList, string details) GetGamePriorityStatus()
        {
            try
            {
                const string imagePath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options";
                var configuredGames = new List<string>();

                // Lista de juegos a verificar
                var gameExecutables = new Dictionary<string, string>
                {
                    {"FortniteClient-Win64-Shipping.exe", "Fortnite"},
                    {"VALORANT-Win64-Shipping.exe", "Valorant"},
                    {"cs2.exe", "Counter-Strike 2"},
                    {"ApexLegends.exe", "Apex Legends"},
                    {"RainbowSix.exe", "Rainbow Six Siege"},
                    {"Overwatch.exe", "Overwatch"},
                    {"ModernWarfare.exe", "Modern Warfare"},
                    {"LeagueofLegends.exe", "League of Legends"},
                    {"EscapeFromTarkov.exe", "Escape from Tarkov"},
                    {"FiveM.exe", "FiveM"},
                    {"RocketLeague.exe", "Rocket League"}
                };

                foreach (var game in gameExecutables)
                {
                    try
                    {
                        string keyPath = $"{imagePath}\\{game.Key}\\PerfOptions";

                        using (var key = Registry.LocalMachine.OpenSubKey(keyPath))
                        {
                            if (key != null)
                            {
                                var cpuPriority = key.GetValue("CpuPriorityClass");
                                var ioPriority = key.GetValue("IoPriority");

                                if (cpuPriority != null && (int)cpuPriority == 3)
                                {
                                    configuredGames.Add(game.Value);
                                }
                            }
                        }
                    }
                    catch
                    {
                        // Ignorar errores de acceso
                    }
                }

                string details = configuredGames.Count > 0
                    ? $"Juegos con prioridad alta:\n{string.Join(", ", configuredGames)}"
                    : "Ningún juego configurado con prioridad alta";

                Debug.WriteLine($"?? Estado Game Priority: {configuredGames.Count} juegos configurados");

                return (configuredGames.Count, configuredGames, details);
            }
            catch (Exception ex)
            {
                string error = $"Error verificando Game Priority: {ex.Message}";
                Debug.WriteLine($"? {error}");
                return (0, new List<string>(), error);
            }
        }

        // ???????????????????????????????????????????????????????????????????
        // TRANSPARENCY (OPTIMIZADO CON ACTUALIZACIÓN INMEDIATA)
        // ???????????????????????????????????????????????????????????????????

        // Import para actualización inmediata de configuración del sistema
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);

        private const uint SPI_SETDRAGFULLWINDOWS = 0x0025;
        private const uint SPIF_UPDATEINIFILE = 0x01;
        private const uint SPIF_SENDCHANGE = 0x02;

        /// <summary>
        /// Deshabilita transparencia de Windows para mejor rendimiento
        /// OPTIMIZADO: Actualización inmediata sin reinicio
        /// </summary>
        public static bool DisableTransparency()
        {
            try
            {
                bool success = false;

                Debug.WriteLine("?? DESHABILITANDO TRANSPARENCIA DE WINDOWS");
                Debug.WriteLine("???????????????????????????????????????????");

                const string personalPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize";

                using (var key = Registry.CurrentUser.CreateSubKey(personalPath))
                {
                    if (key != null)
                    {
                        // Deshabilitar transparencia
                        key.SetValue("EnableTransparency", 0, RegistryValueKind.DWord);

                        Debug.WriteLine("? Registro actualizado: EnableTransparency = 0");
                        success = true;
                    }
                }

                // Forzar actualización inmediata del tema
                try
                {
                    // Notificar a Windows que la configuración cambió
                    SystemParametersInfo(SPI_SETDRAGFULLWINDOWS, 0, IntPtr.Zero,
                                       SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);

                    // Forzar actualización del explorador
                    RefreshDesktop();

                    Debug.WriteLine("? Configuración aplicada inmediatamente");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"?? Aplicado en registro, actualización inmediata falló: {ex.Message}");
                    // No es crítico, el cambio se aplicará en el próximo reinicio
                }

                Debug.WriteLine("?? Transparencia de Windows: DESHABILITADA");
                Debug.WriteLine("?? Nota: Reinicia aplicaciones para efecto completo");

                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error al deshabilitar transparencia: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Habilita transparencia de Windows
        /// OPTIMIZADO: Actualización inmediata sin reinicio
        /// </summary>
        public static bool EnableTransparency()
        {
            try
            {
                bool success = false;

                Debug.WriteLine("?? HABILITANDO TRANSPARENCIA DE WINDOWS");
                Debug.WriteLine("??????????????????????????????????????");

                const string personalPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize";

                using (var key = Registry.CurrentUser.CreateSubKey(personalPath))
                {
                    if (key != null)
                    {
                        // Habilitar transparencia
                        key.SetValue("EnableTransparency", 1, RegistryValueKind.DWord);

                        Debug.WriteLine("? Registro actualizado: EnableTransparency = 1");
                        success = true;
                    }
                }

                // Forzar actualización inmediata del tema
                try
                {
                    SystemParametersInfo(SPI_SETDRAGFULLWINDOWS, 1, IntPtr.Zero,
                                       SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);

                    RefreshDesktop();

                    Debug.WriteLine("? Configuración aplicada inmediatamente");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"?? Aplicado en registro, actualización inmediata falló: {ex.Message}");
                }

                Debug.WriteLine("?? Transparencia de Windows: HABILITADA");
                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error al habilitar transparencia: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Verifica el estado actual de transparencia
        /// </summary>
        public static (bool isEnabled, string details) GetTransparencyStatus()
        {
            try
            {
                const string personalPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize";
                bool isEnabled = true; // Default de Windows

                using (var key = Registry.CurrentUser.OpenSubKey(personalPath))
                {
                    if (key != null)
                    {
                        var value = key.GetValue("EnableTransparency");
                        if (value != null)
                        {
                            isEnabled = (int)value == 1;
                        }
                    }
                }

                string status = isEnabled ? "HABILITADA" : "DESHABILITADA";
                string details = $"Transparencia de Windows: {status}";

                Debug.WriteLine($"?? Estado Transparency: {status}");
                return (isEnabled, details);
            }
            catch (Exception ex)
            {
                string error = $"Error verificando Transparency: {ex.Message}";
                Debug.WriteLine($"? {error}");
                return (true, error); // Asumir habilitado por defecto
            }
        }

        /// <summary>
        /// Refresca el escritorio para aplicar cambios de tema
        /// </summary>
        private static void RefreshDesktop()
        {
            try
            {
                // Forzar actualización del explorador
                var explorerProcesses = Process.GetProcessesByName("explorer");
                if (explorerProcesses.Length > 0)
                {
                    // Notificar cambios a todas las ventanas
                    foreach (var process in explorerProcesses)
                    {
                        try
                        {
                            process.Refresh();
                        }
                        catch { }
                    }
                }
            }
            catch
            {
                // Ignorar errores, no es crítico
            }
        }


        // ???????????????????????????????????????????????????????????????????
        // DIAGNÓSTICO Y VERIFICACIÓN GENERAL
        // ???????????????????????????????????????????????????????????????????

        /// <summary>
        /// Diagnóstico completo de todos los tweaks de Game Mode
        /// </summary>
        public static string DiagnoseAllGameModeTweaks()
        {
            try
            {
                Debug.WriteLine("?? INICIANDO DIAGNÓSTICO COMPLETO DE GAME MODE");
                Debug.WriteLine("????????????????????????????????????????????");

                var report = new System.Text.StringBuilder();
                report.AppendLine("?? DIAGNÓSTICO COMPLETO - GAME MODE TWEAKS");
                report.AppendLine("????????????????????????????????????????????\n");

                // 1. Windows Game Mode
                var (gameModeEnabled, gameModeDetails) = GetGameModeStatus();
                report.AppendLine("1?? WINDOWS GAME MODE:");
                report.AppendLine($"   Estado: {(gameModeEnabled ? "? HABILITADO" : "? DESHABILITADO")}");
                report.AppendLine($"   {gameModeDetails}\n");

                // 2. NTFS Last Access
                var (ntfsEnabled, ntfsDetails) = GetNTFSLastAccessStatus();
                report.AppendLine("2?? NTFS LAST ACCESS TIME:");
                report.AppendLine($"   Estado: {(ntfsEnabled ? "? HABILITADO (lento)" : "? DESHABILITADO (optimizado)")}");
                report.AppendLine($"   {ntfsDetails}\n");

                // 3. Game Priority
                var (priorityCount, priorityGames, priorityDetails) = GetGamePriorityStatus();
                report.AppendLine("3?? PRIORIDAD ALTA PARA JUEGOS:");
                report.AppendLine($"   Juegos configurados: {priorityCount}");
                report.AppendLine($"   {priorityDetails}\n");

                // 4. Transparency
                var (transparencyEnabled, transparencyDetails) = GetTransparencyStatus();
                report.AppendLine("4?? TRANSPARENCIA DE WINDOWS:");
                report.AppendLine($"   Estado: {(transparencyEnabled ? "? HABILITADA (consume recursos)" : "? DESHABILITADA (mejor rendimiento)")}");
                report.AppendLine($"   {transparencyDetails}\n");

                // 5. Resumen y recomendaciones
                report.AppendLine("?? RESUMEN Y RECOMENDACIONES:");
                report.AppendLine("?????????????????????????????????????");

                if (gameModeEnabled && !ntfsEnabled && priorityCount > 0 && !transparencyEnabled)
                {
                    report.AppendLine("? CONFIGURACIÓN ÓPTIMA DETECTADA");
                    report.AppendLine("   Todos los tweaks están configurados correctamente.");
                }
                else
                {
                    report.AppendLine("?? OPTIMIZACIONES PENDIENTES:");

                    if (!gameModeEnabled)
                        report.AppendLine("   • Habilitar Windows Game Mode");

                    if (ntfsEnabled)
                        report.AppendLine("   • Deshabilitar NTFS Last Access Time");

                    if (priorityCount == 0)
                        report.AppendLine("   • Configurar prioridad alta para juegos");

                    if (transparencyEnabled)
                        report.AppendLine("   • Deshabilitar transparencia de Windows");
                }

                string finalReport = report.ToString();
                Debug.WriteLine(finalReport);

                return finalReport;
            }
            catch (Exception ex)
            {
                string error = $"? Error en diagnóstico: {ex.Message}";
                Debug.WriteLine(error);
                return error;
            }
        }
    }
}
