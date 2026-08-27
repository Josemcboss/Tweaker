using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using Microsoft.Win32;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// Optimizaciones y Tweaks importados del proyecto WinUtil (ChrisTitusTech/winutil)
    /// </summary>
    public static class WinUtilTweaks
    {
        #region Activity Feed
        /// <summary>
        /// Desactiva el seguimiento de actividad de usuario (Activity Feed)
        /// </summary>
        public static bool DisableActivityFeed()
        {
            try
            {
                using var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\System");
                if (key != null)
                {
                    key.SetValue("EnableActivityFeed", 0, RegistryValueKind.DWord);
                    key.SetValue("PublishUserActivities", 0, RegistryValueKind.DWord);
                    key.SetValue("UploadUserActivities", 0, RegistryValueKind.DWord);
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en DisableActivityFeed: {ex.Message}");
                return false;
            }
        }

        public static bool RestoreActivityFeed()
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows\System", true);
                if (key != null)
                {
                    key.DeleteValue("EnableActivityFeed", false);
                    key.DeleteValue("PublishUserActivities", false);
                    key.DeleteValue("UploadUserActivities", false);
                }
                return true;
            }
            catch { return false; }
        }

        public static bool? IsActivityFeedDisabled()
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows\System");
                var val = key?.GetValue("EnableActivityFeed");
                return val != null && Convert.ToInt32(val) == 0;
            }
            catch { return null; }
        }
        #endregion

        #region Hibernation
        /// <summary>
        /// Desactiva la hibernación y libera espacio en disco (hiberfil.sys)
        /// </summary>
        public static bool DisableHibernation()
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "powercfg.exe",
                    Arguments = "/hibernate off",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using var proc = Process.Start(psi);
                proc?.WaitForExit();

                using (var key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Power"))
                {
                    key?.SetValue("HibernateEnabled", 0, RegistryValueKind.DWord);
                }

                using (var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FlyoutMenuSettings"))
                {
                    key?.SetValue("ShowHibernateOption", 0, RegistryValueKind.DWord);
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en DisableHibernation: {ex.Message}");
                return false;
            }
        }

        public static bool RestoreHibernation()
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "powercfg.exe",
                    Arguments = "/hibernate on",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using var proc = Process.Start(psi);
                proc?.WaitForExit();

                using (var key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Power"))
                {
                    key?.SetValue("HibernateEnabled", 1, RegistryValueKind.DWord);
                }

                using (var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FlyoutMenuSettings"))
                {
                    key?.SetValue("ShowHibernateOption", 1, RegistryValueKind.DWord);
                }
                return true;
            }
            catch { return false; }
        }

        public static bool? IsHibernationDisabled()
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Power");
                var val = key?.GetValue("HibernateEnabled");
                return val != null && Convert.ToInt32(val) == 0;
            }
            catch { return null; }
        }
        #endregion

        #region End Task on Taskbar
        /// <summary>
        /// Habilita la opción nativa "Finalizar Tarea" en el menú contextual de la barra de tareas
        /// </summary>
        public static bool EnableEndTaskOnTaskbar()
        {
            try
            {
                using var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced");
                key?.SetValue("TaskbarDeveloperSettings", 1, RegistryValueKind.DWord);
                return true;
            }
            catch { return false; }
        }

        public static bool DisableEndTaskOnTaskbar()
        {
            try
            {
                using var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced");
                key?.SetValue("TaskbarDeveloperSettings", 0, RegistryValueKind.DWord);
                return true;
            }
            catch { return false; }
        }

        public static bool? IsEndTaskOnTaskbarEnabled()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced");
                var val = key?.GetValue("TaskbarDeveloperSettings");
                return val != null && Convert.ToInt32(val) == 1;
            }
            catch { return null; }
        }
        #endregion

        #region WPBT (Windows Platform Binary Table)
        /// <summary>
        /// Desactiva la inyección automática de software OEM en el arranque mediante WPBT
        /// </summary>
        public static bool DisableWpbt()
        {
            try
            {
                using var key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager");
                key?.SetValue("DisablePlatformBinaryExecution", 1, RegistryValueKind.DWord);
                return true;
            }
            catch { return false; }
        }

        public static bool RestoreWpbt()
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager", true);
                key?.DeleteValue("DisablePlatformBinaryExecution", false);
                return true;
            }
            catch { return false; }
        }

        public static bool? IsWpbtDisabled()
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager");
                var val = key?.GetValue("DisablePlatformBinaryExecution");
                return val != null && Convert.ToInt32(val) == 1;
            }
            catch { return null; }
        }
        #endregion

        #region Location Tracking
        /// <summary>
        /// Desactiva el rastreo de ubicación y servicios relacionados
        /// </summary>
        public static bool DisableLocationTracking()
        {
            try
            {
                // Servicio lfsvc
                ExecutePowerShell("Set-Service -Name 'lfsvc' -StartupType Disabled; Stop-Service -Name 'lfsvc' -ErrorAction SilentlyContinue");

                using (var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\location"))
                {
                    key?.SetValue("Value", "Deny", RegistryValueKind.String);
                }

                using (var key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\Maps"))
                {
                    key?.SetValue("AutoUpdateEnabled", 0, RegistryValueKind.DWord);
                }
                return true;
            }
            catch { return false; }
        }

        public static bool RestoreLocationTracking()
        {
            try
            {
                ExecutePowerShell("Set-Service -Name 'lfsvc' -StartupType Manual");

                using (var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\location"))
                {
                    key?.SetValue("Value", "Allow", RegistryValueKind.String);
                }

                using (var key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\Maps"))
                {
                    key?.SetValue("AutoUpdateEnabled", 1, RegistryValueKind.DWord);
                }
                return true;
            }
            catch { return false; }
        }

        public static bool? IsLocationTrackingDisabled()
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\location");
                var val = key?.GetValue("Value")?.ToString();
                return val != null && val.Equals("Deny", StringComparison.OrdinalIgnoreCase);
            }
            catch { return null; }
        }
        #endregion

        #region RDP Unsigned File Warnings
        /// <summary>
        /// Desactiva las advertencias al ejecutar archivos RDP no firmados
        /// </summary>
        public static bool DisableRdpUnsignedWarnings()
        {
            try
            {
                using (var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows NT\Terminal Services\Client"))
                {
                    key?.SetValue("RedirectionWarningDialogVersion", 1, RegistryValueKind.DWord);
                }
                using (var key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Terminal Server Client"))
                {
                    key?.SetValue("RdpLaunchConsentAccepted", 1, RegistryValueKind.DWord);
                }
                return true;
            }
            catch { return false; }
        }

        public static bool RestoreRdpUnsignedWarnings()
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows NT\Terminal Services\Client", true))
                {
                    key?.DeleteValue("RedirectionWarningDialogVersion", false);
                }
                using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Terminal Server Client", true))
                {
                    key?.DeleteValue("RdpLaunchConsentAccepted", false);
                }
                return true;
            }
            catch { return false; }
        }
        #endregion

        #region SvcHostSplitThreshold Dynamic Tuning
        /// <summary>
        /// Ajusta SvcHostSplitThresholdInKB en base a la RAM física total instalada
        /// Evita la sobre-fragmentación de svchost.exe
        /// </summary>
        public static bool OptimizeSvcHostSplitThreshold()
        {
            try
            {
                ulong totalRamKb = GetTotalPhysicalMemoryKb();
                if (totalRamKb == 0) totalRamKb = 16 * 1024 * 1024; // Default 16GB en KB si falla WMI

                using var key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control");
                key?.SetValue("SvcHostSplitThresholdInKB", (int)Math.Min(totalRamKb, (ulong)int.MaxValue), RegistryValueKind.DWord);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en OptimizeSvcHostSplitThreshold: {ex.Message}");
                return false;
            }
        }

        public static bool RestoreSvcHostSplitThreshold()
        {
            try
            {
                using var key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control");
                key?.SetValue("SvcHostSplitThresholdInKB", 3800000, RegistryValueKind.DWord); // Valor por defecto de Windows
                return true;
            }
            catch { return false; }
        }

        public static bool? IsSvcHostSplitThresholdOptimized()
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control");
                var val = key?.GetValue("SvcHostSplitThresholdInKB");
                if (val == null) return false;
                int currentVal = Convert.ToInt32(val);
                return currentVal > 4000000;
            }
            catch { return null; }
        }

        private static ulong GetTotalPhysicalMemoryKb()
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT Capacity FROM Win32_PhysicalMemory");
                ulong totalBytes = 0;
                foreach (ManagementObject obj in searcher.Get())
                {
                    if (obj["Capacity"] != null)
                        totalBytes += Convert.ToUInt64(obj["Capacity"]);
                }
                return totalBytes / 1024;
            }
            catch { return 0; }
        }
        #endregion

        #region Browser Debloat (Brave & Edge)
        /// <summary>
        /// Debloat de Brave Browser (Desactiva Rewards, Wallet, VPN, Leo AI Chat, etc.)
        /// </summary>
        public static bool DebloatBraveBrowser()
        {
            try
            {
                using var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\BraveSoftware\Brave");
                if (key != null)
                {
                    key.SetValue("BraveRewardsDisabled", 1, RegistryValueKind.DWord);
                    key.SetValue("BraveWalletDisabled", 1, RegistryValueKind.DWord);
                    key.SetValue("BraveVPNDisabled", 1, RegistryValueKind.DWord);
                    key.SetValue("BraveAIChatEnabled", 0, RegistryValueKind.DWord);
                    key.SetValue("BraveStatsPingEnabled", 0, RegistryValueKind.DWord);
                    key.SetValue("BraveNewsDisabled", 1, RegistryValueKind.DWord);
                    key.SetValue("BraveTalkDisabled", 1, RegistryValueKind.DWord);
                    key.SetValue("TorDisabled", 1, RegistryValueKind.DWord);
                    key.SetValue("BraveP3AEnabled", 0, RegistryValueKind.DWord);
                    key.SetValue("MetricsReportingEnabled", 0, RegistryValueKind.DWord);
                }
                return true;
            }
            catch { return false; }
        }

        public static bool RestoreBraveBrowser()
        {
            try
            {
                Registry.LocalMachine.DeleteSubKeyTree(@"SOFTWARE\Policies\BraveSoftware\Brave", false);
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// Debloat de Microsoft Edge (Desactiva Shopping, Sidebar, Copilot Button, Telemetría)
        /// </summary>
        public static bool DebloatEdgeBrowser()
        {
            try
            {
                using var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Edge");
                if (key != null)
                {
                    key.SetValue("EdgeShoppingAssistantEnabled", 0, RegistryValueKind.DWord);
                    key.SetValue("HubsSidebarEnabled", 0, RegistryValueKind.DWord);
                    key.SetValue("MetricsReportingEnabled", 0, RegistryValueKind.DWord);
                    key.SetValue("PersonalizationReportingEnabled", 0, RegistryValueKind.DWord);
                    key.SetValue("ShowCopilotButton", 0, RegistryValueKind.DWord);
                    key.SetValue("CollectionsServicesEnabled", 0, RegistryValueKind.DWord);
                    key.SetValue("UserFeedbackAllowed", 0, RegistryValueKind.DWord);
                }
                return true;
            }
            catch { return false; }
        }

        public static bool RestoreEdgeBrowser()
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Edge", true);
                if (key != null)
                {
                    key.DeleteValue("EdgeShoppingAssistantEnabled", false);
                    key.DeleteValue("HubsSidebarEnabled", false);
                    key.DeleteValue("MetricsReportingEnabled", false);
                    key.DeleteValue("PersonalizationReportingEnabled", false);
                    key.DeleteValue("ShowCopilotButton", false);
                    key.DeleteValue("CollectionsServicesEnabled", false);
                    key.DeleteValue("UserFeedbackAllowed", false);
                }
                return true;
            }
            catch { return false; }
        }
        #endregion

        #region Windows Widgets Removal
        /// <summary>
        /// Elimina la plataforma de Widgets de Windows 11
        /// </summary>
        public static bool RemoveWindowsWidgets()
        {
            try
            {
                string script = @"
                    Get-Process *Widget* -ErrorAction SilentlyContinue | Stop-Process -Force
                    Get-AppxPackage Microsoft.WidgetsPlatformRuntime -AllUsers -ErrorAction SilentlyContinue | Remove-AppxPackage -AllUsers -ErrorAction SilentlyContinue
                    Get-AppxPackage MicrosoftWindows.Client.WebExperience -AllUsers -ErrorAction SilentlyContinue | Remove-AppxPackage -AllUsers -ErrorAction SilentlyContinue
                ";
                return ExecutePowerShell(script);
            }
            catch { return false; }
        }
        #endregion

        #region Helper
        private static bool ExecutePowerShell(string command)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoProfile -NonInteractive -ExecutionPolicy Bypass -Command \"{command}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using var proc = Process.Start(psi);
                proc?.WaitForExit();
                return proc?.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}
