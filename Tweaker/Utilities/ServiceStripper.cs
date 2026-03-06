using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceProcess;

using Microsoft.Win32;

namespace Tweaker.Utilities
{
    /// <summary>
    /// SERVICE STRIPPER 2.0
    /// 
    /// Problema del sistema anterior: los servicios deshabilitados podían
    /// reactivarse solos (Windows Update, SFC, políticas de grupo).
    /// 
    /// Solución: Además de deshabilitar el servicio, se bloquea su DACL
    /// mediante "sc.exe sdset" para que ningún proceso sin privilegios
    /// de SYSTEM pueda cambiar su StartType ni iniciarlo.
    /// 
    /// Flujo de operación:
    /// 1. Guardar SDDL original del servicio (para restauración)
    /// 2. Detener el servicio
    /// 3. Cambiar StartType = 4 (Disabled) vía registro
    /// 4. Aplicar SDDL restrictivo con sc.exe sdset
    /// 
    /// Restauración:
    /// 1. Restaurar SDDL original con sc.exe sdset
    /// 2. Cambiar StartType al valor guardado
    /// 3. Iniciar el servicio si corresponde
    /// </summary>
    public static class ServiceStripper
    {
        // SDDL restrictivo: solo SYSTEM y Administradores pueden leer/iniciar.
        // Nadie excepto SYSTEM puede modificar StartType o permisos.
        //
        // D: = DACL entries
        //   (A;;CCLCSWRPWPDTLOCRRC;;;SY)  → SYSTEM: control total
        //   (A;;CCDCLCSWRPWPDTLOCRSDRCWDWO;;;BA) → Admins: control total
        //   (A;;CCLCSWLOCRRC;;;IU)        → Interactive Users: solo leer + iniciar
        //   (A;;CCLCSWLOCRRC;;;SU)        → Service Users: solo leer + iniciar
        // S: = SACL (audit) — vacío intencionalmente para evitar spam de eventos
        private const string RESTRICTIVE_SDDL =
            "D:(A;;CCLCSWRPWPDTLOCRRC;;;SY)" +
            "(A;;CCDCLCSWRPWPDTLOCRSDRCWDWO;;;BA)" +
            "(A;;CCLCSWLOCRRC;;;IU)" +
            "(A;;CCLCSWLOCRRC;;;SU)";

        private const string BACKUP_REG_KEY =
            @"SOFTWARE\GhostOptimizer\ServiceStripper\Backups";

        // ═══════════════════════════════════════════════════════════════════
        // STRIP — Deshabilitar + bloquear permisos
        // ═══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Deshabilita un servicio y bloquea sus permisos para que no pueda
        /// ser reactivado por procesos sin privilegios SYSTEM/Admin.
        /// </summary>
        /// <param name="serviceName">Nombre interno del servicio (ej: "DiagTrack")</param>
        /// <param name="displayName">Nombre legible para logs</param>
        public static bool Strip(string serviceName, string displayName)
        {
            Debug.WriteLine("═══════════════════════════════════════");
            Debug.WriteLine($"🔒 SERVICE STRIPPER 2.0 → {displayName}");

            if (ServiceGuard.IsProtected(serviceName))
            {
                Debug.WriteLine($"🛡️ BLOQUEADO: {serviceName} está protegido por ServiceGuard");
                return false;
            }

            try
            {
                // 1. Leer y guardar el SDDL actual
                string? currentSddl = GetCurrentSddl(serviceName);
                int currentStartType = GetCurrentStartType(serviceName);
                SaveBackup(serviceName, currentSddl, currentStartType);
                Debug.WriteLine($"   💾 Backup guardado (StartType={currentStartType})");

                // 2. Detener el servicio si está corriendo
                StopService(serviceName);

                // 3. Cambiar StartType = 4 (Disabled) vía registro
                SetStartTypeRegistry(serviceName, 4);
                Debug.WriteLine($"   ✅ StartType = 4 (Disabled)");

                // 4. Bloquear permisos con sc.exe sdset
                bool sdSet = RunScSdSet(serviceName, RESTRICTIVE_SDDL);
                if (sdSet)
                    Debug.WriteLine($"   ✅ SDDL restrictivo aplicado — no puede reactivarse solo");
                else
                    Debug.WriteLine($"   ⚠️ sdset falló — servicio deshabilitado pero sin bloqueo de permisos");

                Debug.WriteLine($"✅ {displayName} → STRIPPED");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ ERROR Strip({serviceName}): {ex.Message}");
                return false;
            }
        }

        // ═══════════════════════════════════════════════════════════════════
        // RESTORE — Restaurar permisos y estado original
        // ═══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Restaura el servicio a su estado original (StartType y SDDL guardados).
        /// </summary>
        public static bool Restore(string serviceName, string displayName)
        {
            Debug.WriteLine("═══════════════════════════════════════");
            Debug.WriteLine($"🔓 SERVICE STRIPPER 2.0 RESTORE → {displayName}");

            try
            {
                var backup = LoadBackup(serviceName);
                if (backup == null)
                {
                    Debug.WriteLine($"⚠️ No hay backup para {serviceName} — usando valores default");
                    SetStartTypeRegistry(serviceName, 3); // Manual
                    RunScSdSet(serviceName, GetDefaultSddl());
                    return true;
                }

                // 1. Restaurar SDDL original
                if (!string.IsNullOrEmpty(backup.Value.Sddl))
                {
                    RunScSdSet(serviceName, backup.Value.Sddl);
                    Debug.WriteLine($"   ✅ SDDL original restaurado");
                }

                // 2. Restaurar StartType
                SetStartTypeRegistry(serviceName, backup.Value.StartType);
                Debug.WriteLine($"   ✅ StartType = {backup.Value.StartType} restaurado");

                // 3. Iniciar si el StartType era Automatic
                if (backup.Value.StartType == 2)
                {
                    StartService(serviceName);
                }

                DeleteBackup(serviceName);
                Debug.WriteLine($"✅ {displayName} → RESTAURADO");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ ERROR Restore({serviceName}): {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Indica si un servicio fue procesado por ServiceStripper (tiene backup).
        /// </summary>
        public static bool IsStripped(string serviceName)
        {
            return LoadBackup(serviceName) != null;
        }

        // ═══════════════════════════════════════════════════════════════════
        // INTERNALS
        // ═══════════════════════════════════════════════════════════════════

        private static bool RunScSdSet(string serviceName, string sddl)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "sc.exe",
                    Arguments = $"sdset \"{serviceName}\" \"{sddl}\"",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using var proc = Process.Start(psi);
                if (proc == null) return false;

                proc.WaitForExit(5000);
                bool success = proc.ExitCode == 0;

                if (!success)
                    Debug.WriteLine($"   ⚠️ sc sdset exit code: {proc.ExitCode}");

                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"   ❌ sc.exe error: {ex.Message}");
                return false;
            }
        }

        private static string? GetCurrentSddl(string serviceName)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "sc.exe",
                    Arguments = $"sdshow \"{serviceName}\"",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                };

                using var proc = Process.Start(psi);
                if (proc == null) return null;

                string output = proc.StandardOutput.ReadToEnd().Trim();
                proc.WaitForExit(3000);

                // sc sdshow outputs an empty line + the SDDL
                var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                return lines.Length > 0 ? lines[^1].Trim() : null;
            }
            catch
            {
                return null;
            }
        }

        private static int GetCurrentStartType(string serviceName)
        {
            try
            {
                const string servicesKey = @"SYSTEM\CurrentControlSet\Services";
                using var key = Registry.LocalMachine.OpenSubKey($@"{servicesKey}\{serviceName}");
                return key != null ? Convert.ToInt32(key.GetValue("Start", 3)) : 3;
            }
            catch { return 3; }
        }

        private static void SetStartTypeRegistry(string serviceName, int startType)
        {
            const string servicesKey = @"SYSTEM\CurrentControlSet\Services";
            using var key = Registry.LocalMachine.OpenSubKey($@"{servicesKey}\{serviceName}", writable: true);
            key?.SetValue("Start", startType, RegistryValueKind.DWord);
        }

        private static void StopService(string serviceName)
        {
            try
            {
                using var sc = new ServiceController(serviceName);
                if (sc.Status == ServiceControllerStatus.Running)
                {
                    sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                    Debug.WriteLine($"   ✅ Servicio detenido");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"   ⚠️ No se pudo detener {serviceName}: {ex.Message}");
            }
        }

        private static void StartService(string serviceName)
        {
            try
            {
                using var sc = new ServiceController(serviceName);
                if (sc.Status == ServiceControllerStatus.Stopped)
                {
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                    Debug.WriteLine($"   ✅ Servicio iniciado");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"   ⚠️ No se pudo iniciar {serviceName}: {ex.Message}");
            }
        }

        private static string GetDefaultSddl() =>
            "D:(A;;CCLCSWRPWPDTLOCRRC;;;SY)" +
            "(A;;CCDCLCSWRPWPDTLOCRSDRCWDWO;;;BA)" +
            "(A;;CCLCSWLOCRRC;;;IU)" +
            "(A;;CCLCSWLOCRRC;;;SU)" +
            "S:(AU;FA;CCDCLCSWRPWPDTLOSDRCWDWO;;;WD)";

        // ── Backup en registro ───────────────────────────────────────────

        private static void SaveBackup(string serviceName, string? sddl, int startType)
        {
            using var key = Registry.LocalMachine.CreateSubKey(
                $@"{BACKUP_REG_KEY}\{serviceName}", writable: true);
            if (key == null) return;
            if (sddl != null) key.SetValue("OriginalSddl", sddl, RegistryValueKind.String);
            key.SetValue("OriginalStartType", startType, RegistryValueKind.DWord);
            key.SetValue("StrippedAt", DateTime.Now.ToString("o"), RegistryValueKind.String);
        }

        private static (string? Sddl, int StartType)? LoadBackup(string serviceName)
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(
                    $@"{BACKUP_REG_KEY}\{serviceName}");
                if (key == null) return null;

                string? sddl = key.GetValue("OriginalSddl") as string;
                int startType = Convert.ToInt32(key.GetValue("OriginalStartType", 3));
                return (sddl, startType);
            }
            catch { return null; }
        }

        private static void DeleteBackup(string serviceName)
        {
            try
            {
                using var parent = Registry.LocalMachine.OpenSubKey(BACKUP_REG_KEY, writable: true);
                parent?.DeleteSubKeyTree(serviceName, throwOnMissingSubKey: false);
            }
            catch { }
        }

        // ═══════════════════════════════════════════════════════════════════
        // BATCH OPERATIONS — listas predefinidas
        // ═══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Lista de servicios de telemetría/bloatware seguros para Strip.
        /// </summary>
        public static readonly IReadOnlyList<(string Name, string Display)> TelemetryServices =
            new List<(string, string)>
            {
                ("DiagTrack",            "Connected User Experiences & Telemetry"),
                ("dmwappushservice",     "WAP Push Message Routing Service"),
                ("MapsBroker",           "Downloaded Maps Manager"),
                ("RetailDemo",           "Retail Demo Service"),
                ("WbioSrvc",             "Windows Biometric Service"),
            };

        /// <summary>
        /// Aplica Strip a todos los servicios de TelemetryServices.
        /// </summary>
        public static int StripAllTelemetry()
        {
            int count = 0;
            foreach (var (name, display) in TelemetryServices)
                if (Strip(name, display)) count++;
            return count;
        }

        /// <summary>
        /// Restaura todos los servicios que tienen backup de ServiceStripper.
        /// </summary>
        public static int RestoreAll()
        {
            int count = 0;
            try
            {
                using var parent = Registry.LocalMachine.OpenSubKey(BACKUP_REG_KEY);
                if (parent == null) return 0;

                foreach (string name in parent.GetSubKeyNames())
                {
                    if (Restore(name, name)) count++;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ ERROR RestoreAll: {ex.Message}");
            }
            return count;
        }
    }
}
