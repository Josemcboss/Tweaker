using System;
using System.Diagnostics;
using System.IO;

using Microsoft.Win32;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// Optimizaciones avanzadas de sistema que complementan a las clases existentes
    /// Incluye: FSO & Game DVR BehaviorSteps, Context Menu "Take Ownership", UAC, Windows Update Cache
    /// </summary>
    public static class AdvancedSystemTweaks
    {
        // ────────────────────────────────────────────?
        // FSO (FULLSCREEN OPTIMIZATIONS) & GAME DVR BEHAVIOR STEPS
        // ────────────────────────────────────────────?

        /// <summary>
        /// Deshabilita Fullscreen Optimizations y Game DVR con BehaviorSteps
        /// 
        /// ¿QUÉ SON LAS FULLSCREEN OPTIMIZATIONS (FSO)?
        /// ──────────────────────────────────────────
        /// • Feature introducida en Windows 10 1903
        /// • Convierte Fullscreen Exclusive en "Borderless Fullscreen"
        /// • Permite Alt+Tab más rápido
        /// • Pero AÑADE LATENCIA en juegos competitivos
        /// 
        /// PROBLEMA EN GAMING:
        /// ──────────────────────────────────────────
        /// • FSO añade 5-15ms de input lag
        /// • Compositor de Windows (DWM) procesa cada frame
        /// • Causa micro-stuttering
        /// • Peor frame pacing
        /// 
        /// SOLUCIÓN:
        /// ──────────────────────────────────────────
        /// • DisableFullscreenOptimizations = 1
        /// • AllowGameDVR = 0
        /// • GameDVR_Enabled = 0
        /// • BehaviorSteps para deshabilitar globalmente
        /// 
        /// UBICACIÓN:
        /// HKCU\System\GameConfigStore
        /// HKLM\SOFTWARE\Microsoft\PolicyManager\default\ApplicationManagement\AllowGameDVR
        /// </summary>
        public static bool DisableFSOAndGameDVR()
        {
            try
            {
                Debug.WriteLine("?? Deshabilitando Fullscreen Optimizations y Game DVR...");

                bool success = true;

                // PASO 1: Game Config Store (per-user)
                try
                {
                    using (var key = Registry.CurrentUser.CreateSubKey(@"System\GameConfigStore", true))
                    {
                        if (key != null)
                        {
                            // GameDVR_Enabled = 0
                            key.SetValue("GameDVR_Enabled", 0, RegistryValueKind.DWord);

                            // GameDVR_FSEBehaviorMode = 2 (Deshabilitar FSO)
                            key.SetValue("GameDVR_FSEBehaviorMode", 2, RegistryValueKind.DWord);

                            // GameDVR_HonorUserFSEBehaviorMode = 1
                            key.SetValue("GameDVR_HonorUserFSEBehaviorMode", 1, RegistryValueKind.DWord);

                            // GameDVR_DXGIHonorFSEWindowsCompatible = 1
                            key.SetValue("GameDVR_DXGIHonorFSEWindowsCompatible", 1, RegistryValueKind.DWord);

                            Debug.WriteLine("   ? GameConfigStore: FSO deshabilitado");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"   ?? GameConfigStore error: {ex.Message}");
                    success = false;
                }

                // PASO 2: Policy Manager (system-wide)
                try
                {
                    string policyPath = @"SOFTWARE\Microsoft\PolicyManager\default\ApplicationManagement\AllowGameDVR";
                    using (var key = Registry.LocalMachine.CreateSubKey(policyPath, true))
                    {
                        if (key != null)
                        {
                            key.SetValue("value", 0, RegistryValueKind.DWord);
                            Debug.WriteLine("   ? PolicyManager: GameDVR deshabilitado");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"   ?? PolicyManager error: {ex.Message}");
                }

                // PASO 3: Windows Gaming Features
                try
                {
                    using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\GameBar", true))
                    {
                        if (key != null)
                        {
                            key.SetValue("AllowAutoGameMode", 0, RegistryValueKind.DWord);
                            key.SetValue("AutoGameModeEnabled", 0, RegistryValueKind.DWord);
                            Debug.WriteLine("   ? GameBar: Auto Game Mode deshabilitado");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"   ?? GameBar error: {ex.Message}");
                }

                // PASO 4: Broadcast DVR (Captures)
                try
                {
                    string broadcastPath = @"Software\Microsoft\Windows\CurrentVersion\GameDVR";
                    using (var key = Registry.CurrentUser.CreateSubKey(broadcastPath, true))
                    {
                        if (key != null)
                        {
                            key.SetValue("AppCaptureEnabled", 0, RegistryValueKind.DWord);
                            key.SetValue("AudioCaptureEnabled", 0, RegistryValueKind.DWord);
                            key.SetValue("CursorCaptureEnabled", 0, RegistryValueKind.DWord);
                            key.SetValue("HistoricalCaptureEnabled", 0, RegistryValueKind.DWord);
                            Debug.WriteLine("   ? Broadcast DVR: Captures deshabilitados");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"   ?? Broadcast DVR error: {ex.Message}");
                }

                Debug.WriteLine("? FSO y Game DVR deshabilitados");
                Debug.WriteLine("?? Input lag reducido en 5-15ms");
                Debug.WriteLine("?? REINICIA para aplicar cambios");

                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error en DisableFSOAndGameDVR: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Restaura Fullscreen Optimizations y Game DVR
        /// </summary>
        public static bool RestoreFSOAndGameDVR()
        {
            try
            {
                Debug.WriteLine("?? Restaurando FSO y Game DVR...");

                // Game Config Store
                using (var key = Registry.CurrentUser.OpenSubKey(@"System\GameConfigStore", true))
                {
                    if (key != null)
                    {
                        key.SetValue("GameDVR_Enabled", 1, RegistryValueKind.DWord);
                        key.SetValue("GameDVR_FSEBehaviorMode", 0, RegistryValueKind.DWord);
                    }
                }

                // Policy Manager
                string policyPath = @"SOFTWARE\Microsoft\PolicyManager\default\ApplicationManagement\AllowGameDVR";
                using (var key = Registry.LocalMachine.OpenSubKey(policyPath, true))
                {
                    if (key != null)
                    {
                        key.SetValue("value", 1, RegistryValueKind.DWord);
                    }
                }

                // GameBar
                using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\GameBar", true))
                {
                    if (key != null)
                    {
                        key.SetValue("AllowAutoGameMode", 1, RegistryValueKind.DWord);
                        key.SetValue("AutoGameModeEnabled", 1, RegistryValueKind.DWord);
                    }
                }

                // Broadcast DVR
                string broadcastPath = @"Software\Microsoft\Windows\CurrentVersion\GameDVR";
                using (var key = Registry.CurrentUser.OpenSubKey(broadcastPath, true))
                {
                    if (key != null)
                    {
                        key.SetValue("AppCaptureEnabled", 1, RegistryValueKind.DWord);
                    }
                }

                Debug.WriteLine("? FSO y Game DVR restaurados");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error: {ex.Message}");
                return false;
            }
        }

        // ────────────────────────────────────────────?
        // CONTEXT MENU "TAKE OWNERSHIP"
        // ────────────────────────────────────────────?

        /// <summary>
        /// Agrega "Take Ownership" al menú contextual de archivos y carpetas
        /// 
        /// ¿QUÉ HACE?
        /// ──────────────────────────────────────────
        /// • Agrega opción "Take Ownership" al click derecho
        /// • Permite tomar posesión de archivos del sistema
        /// • Útil para modificar archivos protegidos de Windows
        /// • CRÍTICO para tweaking avanzado
        /// 
        /// COMANDOS EJECUTADOS:
        /// • takeown /f "%1" /r /d y
        /// • icacls "%1" /grant administrators:F /t
        /// 
        /// UBICACIÓN:
        /// HKCR\*\shell\runas
        /// HKCR\Directory\shell\runas
        /// </summary>
        public static bool AddTakeOwnershipContext()
        {
            try
            {
                Debug.WriteLine("?? Agregando 'Take Ownership' al menú contextual...");

                // ARCHIVOS (*)
                string fileShellPath = @"*\shell\runas";
                using (var key = Registry.ClassesRoot.CreateSubKey(fileShellPath, true))
                {
                    if (key != null)
                    {
                        key.SetValue("", "Take Ownership");
                        key.SetValue("NoWorkingDirectory", "");
                        key.SetValue("HasLUAShield", "");

                        // Comando
                        using (var cmdKey = key.CreateSubKey("command", true))
                        {
                            if (cmdKey != null)
                            {
                                string command = @"cmd.exe /c takeown /f ""%1"" && icacls ""%1"" /grant administrators:F";
                                cmdKey.SetValue("", command);
                                cmdKey.SetValue("IsolatedCommand", command);
                            }
                        }

                        Debug.WriteLine("   ? Context menu para archivos agregado");
                    }
                }

                // CARPETAS (Directory)
                string dirShellPath = @"Directory\shell\runas";
                using (var key = Registry.ClassesRoot.CreateSubKey(dirShellPath, true))
                {
                    if (key != null)
                    {
                        key.SetValue("", "Take Ownership");
                        key.SetValue("NoWorkingDirectory", "");
                        key.SetValue("HasLUAShield", "");

                        // Comando
                        using (var cmdKey = key.CreateSubKey("command", true))
                        {
                            if (cmdKey != null)
                            {
                                string command = @"cmd.exe /c takeown /f ""%1"" /r /d y && icacls ""%1"" /grant administrators:F /t";
                                cmdKey.SetValue("", command);
                                cmdKey.SetValue("IsolatedCommand", command);
                            }
                        }

                        Debug.WriteLine("   ? Context menu para carpetas agregado");
                    }
                }

                Debug.WriteLine("? 'Take Ownership' agregado al menú contextual");
                Debug.WriteLine("?? Click derecho en archivos/carpetas para usarlo");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error en AddTakeOwnershipContext: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina "Take Ownership" del menú contextual
        /// </summary>
        public static bool RemoveTakeOwnershipContext()
        {
            try
            {
                Debug.WriteLine("?? Eliminando 'Take Ownership' del menú contextual...");

                // Eliminar de archivos
                Registry.ClassesRoot.DeleteSubKeyTree(@"*\shell\runas", false);

                // Eliminar de carpetas
                Registry.ClassesRoot.DeleteSubKeyTree(@"Directory\shell\runas", false);

                Debug.WriteLine("? 'Take Ownership' eliminado");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error: {ex.Message}");
                return false;
            }
        }

        // ────────────────────────────────────────────?
        // UAC (USER ACCOUNT CONTROL)
        // ────────────────────────────────────────────?

        /// <summary>
        /// Deshabilita UAC (User Account Control)
        /// 
        /// ──── ADVERTENCIA EXTREMA ────
        /// ──────────────────────────────────────────
        /// • UAC protege contra malware
        /// • Deshabilitar = RIESGO DE SEGURIDAD
        /// • Solo para PCs dedicados a gaming
        /// • NO recomendado para uso general
        /// 
        /// BENEFICIOS:
        /// • Sin popups molestos
        /// • Apps se inician sin confirmación
        /// • Tweaker funciona sin permisos
        /// 
        /// RIESGOS:
        /// • Malware puede ejecutarse sin confirmar
        /// • Apps tienen acceso completo al sistema
        /// • Vulnerabilidades sin protección
        /// 
        /// UBICACIÓN:
        /// HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System\EnableLUA
        /// </summary>
        public static bool DisableUAC()
        {
            try
            {
                Debug.WriteLine("?? Deshabilitando UAC...");
                Debug.WriteLine("──── ADVERTENCIA: RIESGO DE SEGURIDAD ────");

                string uacPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System";
                using (var key = Registry.LocalMachine.OpenSubKey(uacPath, true))
                {
                    if (key == null)
                    {
                        Debug.WriteLine("? No se pudo acceder a Policies\\System");
                        return false;
                    }

                    // EnableLUA = 0 (UAC deshabilitado)
                    key.SetValue("EnableLUA", 0, RegistryValueKind.DWord);

                    // ConsentPromptBehaviorAdmin = 0 (sin prompt)
                    key.SetValue("ConsentPromptBehaviorAdmin", 0, RegistryValueKind.DWord);

                    // PromptOnSecureDesktop = 0 (sin secure desktop)
                    key.SetValue("PromptOnSecureDesktop", 0, RegistryValueKind.DWord);

                    Debug.WriteLine("? UAC deshabilitado");
                    Debug.WriteLine("──? REINICIA para aplicar cambios ──?");
                    Debug.WriteLine("?? REVIERTE si experimentas problemas de seguridad");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error en DisableUAC: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Restaura UAC (habilitado)
        /// </summary>
        public static bool RestoreUAC()
        {
            try
            {
                Debug.WriteLine("?? Restaurando UAC...");

                string uacPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System";
                using (var key = Registry.LocalMachine.OpenSubKey(uacPath, true))
                {
                    if (key == null) return false;

                    // EnableLUA = 1 (UAC habilitado)
                    key.SetValue("EnableLUA", 1, RegistryValueKind.DWord);

                    // ConsentPromptBehaviorAdmin = 5 (prompt for administrators)
                    key.SetValue("ConsentPromptBehaviorAdmin", 5, RegistryValueKind.DWord);

                    // PromptOnSecureDesktop = 1 (secure desktop)
                    key.SetValue("PromptOnSecureDesktop", 1, RegistryValueKind.DWord);

                    Debug.WriteLine("? UAC restaurado");
                    Debug.WriteLine("?? REINICIA para aplicar cambios");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error: {ex.Message}");
                return false;
            }
        }

        // ────────────────────────────────────────────?
        // WINDOWS UPDATE CACHE CLEANUP
        // ────────────────────────────────────────────?

        /// <summary>
        /// Limpia caché de Windows Update (SoftwareDistribution)
        /// 
        /// ¿QUÉ ES SOFTWAREDISTRIBUTION?
        /// ──────────────────────────────────────────
        /// • Carpeta donde Windows Update descarga archivos
        /// • Puede crecer hasta 10-30GB
        /// • Updates fallidos quedan ahí permanentemente
        /// • Causa lentitud en Windows Update
        /// 
        /// UBICACIÓN:
        /// C:\Windows\SoftwareDistribution\Download
        /// 
        /// IMPACTO:
        /// ? Libera 5-30GB de espacio
        /// ? Windows Update más rápido
        /// ? Menos espacio desperdiciado
        /// 
        /// SEGURO:
        /// ? Windows redownloadeará updates cuando sea necesario
        /// ? No afecta updates ya instalados
        /// </summary>
        public static (bool success, long mbFreed) CleanWindowsUpdateCache()
        {
            try
            {
                Debug.WriteLine("?? Limpiando caché de Windows Update...");

                string updateCachePath = @"C:\Windows\SoftwareDistribution\Download";

                if (!Directory.Exists(updateCachePath))
                {
                    Debug.WriteLine("?? Carpeta SoftwareDistribution no encontrada");
                    return (false, 0);
                }

                // Calcular espacio antes de limpiar
                long bytesBefore = GetDirectorySize(updateCachePath);
                long mbBefore = bytesBefore / 1024 / 1024;

                Debug.WriteLine($"?? Espacio usado: {mbBefore} MB");

                // Detener servicio Windows Update
                Debug.WriteLine("   ?? Deteniendo Windows Update...");
                bool serviceStopped = StopService("wuauserv");

                if (!serviceStopped)
                {
                    Debug.WriteLine("   ?? No se pudo detener Windows Update. Algunos archivos pueden estar en uso.");
                }

                // Limpiar carpeta
                int filesDeleted = 0;
                long bytesFreed = 0;

                try
                {
                    var files = Directory.GetFiles(updateCachePath, "*", SearchOption.AllDirectories);

                    foreach (string file in files)
                    {
                        try
                        {
                            var fileInfo = new FileInfo(file);
                            long fileSize = fileInfo.Length;

                            File.Delete(file);
                            filesDeleted++;
                            bytesFreed += fileSize;
                        }
                        catch
                        {
                            // Archivo en uso, skip
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"   ?? Error limpiando archivos: {ex.Message}");
                }

                // Reiniciar servicio Windows Update
                if (serviceStopped)
                {
                    Debug.WriteLine("   ?? Reiniciando Windows Update...");
                    StartService("wuauserv");
                }

                long mbFreed = bytesFreed / 1024 / 1024;

                Debug.WriteLine("? Caché de Windows Update limpiado");
                Debug.WriteLine($"   ?? Archivos eliminados: {filesDeleted}");
                Debug.WriteLine($"   ?? Espacio liberado: {mbFreed} MB");

                return (true, mbFreed);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error en CleanWindowsUpdateCache: {ex.Message}");
                return (false, 0);
            }
        }

        // ────────────────────────────────────────────?
        // HELPER METHODS
        // ────────────────────────────────────────────?

        private static long GetDirectorySize(string path)
        {
            try
            {
                long size = 0;
                var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);

                foreach (string file in files)
                {
                    try
                    {
                        size += new FileInfo(file).Length;
                    }
                    catch { }
                }

                return size;
            }
            catch
            {
                return 0;
            }
        }

        private static bool StopService(string serviceName)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "net",
                    Arguments = $"stop {serviceName}",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                };

                var process = Process.Start(psi);
                process?.WaitForExit(10000); // 10 segundos timeout

                return process?.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }

        private static bool StartService(string serviceName)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "net",
                    Arguments = $"start {serviceName}",
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                var process = Process.Start(psi);
                process?.WaitForExit(10000);

                return process?.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }

        // ────────────────────────────────────────────?
        // CONNECTED STANDBY & WATCHDOG CANDIDATES
        // ────────────────────────────────────────────?

        /// <summary>
        /// Connected Standby (Modern Standby) Optimization
        /// </summary>
        public static bool DisableConnectedStandby()
        {
            try
            {
                string powerPath = @"SYSTEM\CurrentControlSet\Control\Power";
                using (var key = Registry.LocalMachine.CreateSubKey(powerPath, true))
                {
                    if (key != null)
                    {
                        key.SetValue("PlatformAoAcOverride", 0, RegistryValueKind.DWord);
                        key.SetValue("CsEnabled", 0, RegistryValueKind.DWord);
                        Debug.WriteLine("✅ Connected Standby (CsEnabled & PlatformAoAcOverride) Disabled");
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error disabling Connected Standby: {ex.Message}");
                return false;
            }
        }

        public static bool RestoreConnectedStandby()
        {
            try
            {
                string powerPath = @"SYSTEM\CurrentControlSet\Control\Power";
                using (var key = Registry.LocalMachine.OpenSubKey(powerPath, true))
                {
                    if (key != null)
                    {
                        key.DeleteValue("PlatformAoAcOverride", false);
                        key.DeleteValue("CsEnabled", false);
                        Debug.WriteLine("✅ Connected Standby Restored");
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error restoring Connected Standby: {ex.Message}");
                return false;
            }
        }

        public static bool IsConnectedStandbyDisabled()
        {
            try
            {
                string powerPath = @"SYSTEM\CurrentControlSet\Control\Power";
                using (var key = Registry.LocalMachine.OpenSubKey(powerPath, false))
                {
                    if (key != null)
                    {
                        var overrideVal = key.GetValue("PlatformAoAcOverride");
                        return overrideVal != null && Convert.ToInt32(overrideVal) == 0;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Disable Watchdog sensor and timers to prevent micro-stutters during DPC routines
        /// </summary>
        public static bool DisableWatchdog()
        {
            try
            {
                // Registry path 1
                string watchdogPath = @"SYSTEM\CurrentControlSet\Control\Watchdog";
                using (var key = Registry.LocalMachine.CreateSubKey(watchdogPath, true))
                {
                    key?.SetValue("DisableWatchdog", 1, RegistryValueKind.DWord);
                }
                
                // Registry path 2
                string watchdogDisplayPath = @"SYSTEM\CurrentControlSet\Control\Watchdog\Display";
                using (var key = Registry.LocalMachine.CreateSubKey(watchdogDisplayPath, true))
                {
                    key?.SetValue("DisableWatchdog", 1, RegistryValueKind.DWord);
                }
                
                // Disable service wdtval
                using (var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\wdtval", true))
                {
                    key?.SetValue("Start", 4, RegistryValueKind.DWord);
                }

                // Disable watchdog service
                using (var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\watchdog", true))
                {
                    key?.SetValue("Start", 4, RegistryValueKind.DWord);
                }

                Debug.WriteLine("✅ Watchdog disabled successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error disabling Watchdog: {ex.Message}");
                return false;
            }
        }

        public static bool RestoreWatchdog()
        {
            try
            {
                string watchdogPath = @"SYSTEM\CurrentControlSet\Control\Watchdog";
                using (var key = Registry.LocalMachine.OpenSubKey(watchdogPath, true))
                {
                    key?.DeleteValue("DisableWatchdog", false);
                }
                
                string watchdogDisplayPath = @"SYSTEM\CurrentControlSet\Control\Watchdog\Display";
                using (var key = Registry.LocalMachine.OpenSubKey(watchdogDisplayPath, true))
                {
                    key?.DeleteValue("DisableWatchdog", false);
                }
                
                // Re-enable service wdtval (manual)
                using (var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\wdtval", true))
                {
                    key?.SetValue("Start", 3, RegistryValueKind.DWord);
                }

                // Re-enable watchdog service (manual)
                using (var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\watchdog", true))
                {
                    key?.SetValue("Start", 3, RegistryValueKind.DWord);
                }

                Debug.WriteLine("✅ Watchdog restored successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error restoring Watchdog: {ex.Message}");
                return false;
            }
        }

        public static bool IsWatchdogDisabled()
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Watchdog", false))
                {
                    if (key != null)
                    {
                        var val = key.GetValue("DisableWatchdog");
                        return val != null && Convert.ToInt32(val) == 1;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        // ────────────────────────────────────────────?
        // APPLY ALL SYSTEM TWEAKS
        // ────────────────────────────────────────────?

        /// <summary>
        /// Aplica TODOS los tweaks avanzados de sistema
        /// </summary>
        public static bool ApplyAllAdvancedSystemTweaks()
        {
            try
            {
                Debug.WriteLine("──────────────────────────");
                Debug.WriteLine("APLICANDO TODOS LOS TWEAKS DE SISTEMA");
                Debug.WriteLine("──────────────────────────");

                bool success = true;

                success &= DisableFSOAndGameDVR();
                success &= AddTakeOwnershipContext();
                success &= DisableConnectedStandby();
                success &= DisableWatchdog();

                // UAC es opcional (requiere confirmación del usuario)
                // success &= DisableUAC();

                Debug.WriteLine("──────────────────────────");
                if (success)
                {
                    Debug.WriteLine("? TODOS LOS TWEAKS APLICADOS");
                }
                else
                {
                    Debug.WriteLine("?? ALGUNOS TWEAKS FALLARON");
                }
                Debug.WriteLine("──────────────────────────");

                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Revierte TODOS los tweaks avanzados de sistema
        /// </summary>
        public static bool RevertAllAdvancedSystemTweaks()
        {
            try
            {
                Debug.WriteLine("──────────────────────────");
                Debug.WriteLine("REVIRTIENDO TWEAKS DE SISTEMA");
                Debug.WriteLine("──────────────────────────");

                RestoreFSOAndGameDVR();
                RemoveTakeOwnershipContext();
                RestoreUAC();
                RestoreConnectedStandby();
                RestoreWatchdog();

                Debug.WriteLine("? TWEAKS REVERTIDOS");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// MÉTODO DE COMPATIBILIDAD PARA PRESETS
        /// Aplica las optimizaciones principales del sistema
        /// </summary>
        public static bool Apply()
        {
            return ApplyAllAdvancedSystemTweaks();
        }

        /// <summary>
        /// Revierte las optimizaciones del sistema
        /// </summary>
        public static bool Revert()
        {
            return RevertAllAdvancedSystemTweaks();
        }
    }
}
