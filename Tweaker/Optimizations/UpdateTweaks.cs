using Microsoft.Win32;
using System;
using System.Diagnostics;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// Tweaks de Windows Update para evitar lag spikes durante gaming
    /// Desactiva descargas automáticas y P2P delivery que causan stuttering
    /// </summary>
    public static class UpdateTweaks
    {
        private const string WINDOWS_UPDATE_KEY = @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU";
        private const string DELIVERY_OPTIMIZATION_KEY = @"SYSTEM\CurrentControlSet\Services\DoSvc";

        /// <summary>
        /// Desactiva Windows Update automático para evitar descargas durante gaming
        /// Previene lag spikes causados por descargas en segundo plano
        /// </summary>
        public static bool DisableAutomaticUpdates()
        {
            try
            {
                Debug.WriteLine("???????????????????????????????????????????????????????????");
                Debug.WriteLine("DISABLING AUTOMATIC WINDOWS UPDATES");
                Debug.WriteLine("???????????????????????????????????????????????????????????");

                // Crear clave de políticas si no existe
                using (var key = Registry.LocalMachine.CreateSubKey(WINDOWS_UPDATE_KEY))
                {
                    if (key == null)
                    {
                        Debug.WriteLine("? ERROR: No se pudo crear clave de Windows Update");
                        return false;
                    }

                    // NoAutoUpdate = 1 (Desactiva actualizaciones automáticas)
                    key.SetValue("NoAutoUpdate", 1, RegistryValueKind.DWord);
                    Debug.WriteLine("? NoAutoUpdate = 1 (Automatic updates disabled)");

                    // AUOptions = 2 (Notificar antes de descargar)
                    key.SetValue("AUOptions", 2, RegistryValueKind.DWord);
                    Debug.WriteLine("? AUOptions = 2 (Notify before download)");

                    // ScheduledInstallDay = 0 (Sin día programado)
                    key.SetValue("ScheduledInstallDay", 0, RegistryValueKind.DWord);
                    Debug.WriteLine("? ScheduledInstallDay = 0 (No scheduled day)");
                }

                Debug.WriteLine("");
                Debug.WriteLine("?? BENEFICIOS GAMING:");
                Debug.WriteLine("   • Eliminación de lag spikes por descargas");
                Debug.WriteLine("   • Sin interrupciones durante partidas competitivas");
                Debug.WriteLine("   • Ancho de banda dedicado al gaming");
                Debug.WriteLine("   • Control manual de cuándo actualizar");
                Debug.WriteLine("");

                return true;
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("? ERROR: Se requieren permisos de administrador para desactivar Windows Update");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? ERROR en DisableAutomaticUpdates: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Desactiva Delivery Optimization (P2P) que consume ancho de banda para compartir updates
        /// Evita que el PC actúe como servidor de updates para otros dispositivos
        /// </summary>
        public static bool DisableDeliveryOptimization()
        {
            try
            {
                Debug.WriteLine("???????????????????????????????????????????????????????????");
                Debug.WriteLine("DISABLING DELIVERY OPTIMIZATION (P2P)");
                Debug.WriteLine("???????????????????????????????????????????????????????????");

                // Desactivar servicio DoSvc (Delivery Optimization)
                using (var key = Registry.LocalMachine.OpenSubKey(DELIVERY_OPTIMIZATION_KEY, true))
                {
                    if (key == null)
                    {
                        Debug.WriteLine("?? WARNING: Delivery Optimization service key not found");
                        return false;
                    }

                    // Start = 4 (Disabled)
                    key.SetValue("Start", 4, RegistryValueKind.DWord);
                    Debug.WriteLine("? DoSvc Start = 4 (Service disabled)");
                }

                // Configuraciones adicionales de Delivery Optimization
                const string DO_CONFIG_KEY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\DeliveryOptimization\Config";
                using (var key = Registry.LocalMachine.CreateSubKey(DO_CONFIG_KEY))
                {
                    if (key != null)
                    {
                        // DODownloadMode = 0 (Solo desde Microsoft, no P2P)
                        key.SetValue("DODownloadMode", 0, RegistryValueKind.DWord);
                        Debug.WriteLine("? DODownloadMode = 0 (No P2P sharing)");

                        // DownloadMode = 0 (Desde Windows Update solamente)
                        key.SetValue("DownloadMode", 0, RegistryValueKind.DWord);
                        Debug.WriteLine("? DownloadMode = 0 (Windows Update only)");
                    }
                }

                Debug.WriteLine("");
                Debug.WriteLine("?? BENEFICIOS DE RED:");
                Debug.WriteLine("   • Ancho de banda completo para gaming");
                Debug.WriteLine("   • Sin uploads de updates a otros PCs");
                Debug.WriteLine("   • Ping más estable y consistente");
                Debug.WriteLine("   • Menor uso de CPU por P2P");
                Debug.WriteLine("");

                return true;
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("? ERROR: Se requieren permisos de administrador para desactivar Delivery Optimization");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? ERROR en DisableDeliveryOptimization: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Restaura configuración automática de Windows Update
        /// </summary>
        public static bool EnableAutomaticUpdates()
        {
            try
            {
                Debug.WriteLine("???????????????????????????????????????????????????????????");
                Debug.WriteLine("ENABLING AUTOMATIC WINDOWS UPDATES");
                Debug.WriteLine("???????????????????????????????????????????????????????????");

                using (var key = Registry.LocalMachine.OpenSubKey(WINDOWS_UPDATE_KEY, true))
                {
                    if (key != null)
                    {
                        // Eliminar configuraciones para restaurar defaults
                        key.DeleteValue("NoAutoUpdate", false);
                        key.DeleteValue("AUOptions", false);
                        key.DeleteValue("ScheduledInstallDay", false);
                        
                        Debug.WriteLine("? Configuración de Windows Update restaurada");
                    }
                }

                Debug.WriteLine("?? Windows Update volverá a descargar e instalar automáticamente");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? ERROR en EnableAutomaticUpdates: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Restaura Delivery Optimization (P2P)
        /// </summary>
        public static bool EnableDeliveryOptimization()
        {
            try
            {
                Debug.WriteLine("???????????????????????????????????????????????????????????");
                Debug.WriteLine("ENABLING DELIVERY OPTIMIZATION (P2P)");
                Debug.WriteLine("???????????????????????????????????????????????????????????");

                // Restaurar servicio DoSvc
                using (var key = Registry.LocalMachine.OpenSubKey(DELIVERY_OPTIMIZATION_KEY, true))
                {
                    if (key != null)
                    {
                        // Start = 3 (Manual) - valor por defecto
                        key.SetValue("Start", 3, RegistryValueKind.DWord);
                        Debug.WriteLine("? DoSvc Start = 3 (Service enabled)");
                    }
                }

                // Restaurar configuraciones de Delivery Optimization
                const string DO_CONFIG_KEY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\DeliveryOptimization\Config";
                using (var key = Registry.LocalMachine.OpenSubKey(DO_CONFIG_KEY, true))
                {
                    if (key != null)
                    {
                        key.DeleteValue("DODownloadMode", false);
                        key.DeleteValue("DownloadMode", false);
                        Debug.WriteLine("? Delivery Optimization configuración restaurada");
                    }
                }

                Debug.WriteLine("?? P2P sharing de updates restaurado a configuración por defecto");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? ERROR en EnableDeliveryOptimization: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Diagnóstico completo del estado de Windows Update
        /// </summary>
        public static string DiagnoseUpdateSettings()
        {
            try
            {
                var diagnosis = "???????????????????????????????????????????????????????????\n";
                diagnosis += "DIAGNÓSTICO WINDOWS UPDATE & DELIVERY OPTIMIZATION\n";
                diagnosis += "???????????????????????????????????????????????????????????\n\n";

                // Verificar configuración de Windows Update
                using (var key = Registry.LocalMachine.OpenSubKey(WINDOWS_UPDATE_KEY))
                {
                    diagnosis += "?? WINDOWS UPDATE:\n";
                    if (key != null)
                    {
                        var noAutoUpdate = key.GetValue("NoAutoUpdate");
                        var auOptions = key.GetValue("AUOptions");

                        if (noAutoUpdate != null && (int)noAutoUpdate == 1)
                        {
                            diagnosis += "   ? Updates automáticos: DESACTIVADOS (Gaming optimized)\n";
                            diagnosis += $"   ?? AUOptions: {auOptions ?? "Default"}\n";
                        }
                        else
                        {
                            diagnosis += "   ?? Updates automáticos: ACTIVOS (Puede causar lag)\n";
                        }
                    }
                    else
                    {
                        diagnosis += "   ?? Configuración por defecto (Updates automáticos activos)\n";
                    }
                }

                // Verificar Delivery Optimization
                using (var key = Registry.LocalMachine.OpenSubKey(DELIVERY_OPTIMIZATION_KEY))
                {
                    diagnosis += "\n?? DELIVERY OPTIMIZATION (P2P):\n";
                    if (key != null)
                    {
                        var startValue = key.GetValue("Start");
                        if (startValue != null)
                        {
                            int startType = (int)startValue;
                            switch (startType)
                            {
                                case 4:
                                    diagnosis += "   ? Servicio DoSvc: DESACTIVADO (No P2P sharing)\n";
                                    break;
                                case 3:
                                    diagnosis += "   ?? Servicio DoSvc: MANUAL (P2P puede activarse)\n";
                                    break;
                                case 2:
                                    diagnosis += "   ?? Servicio DoSvc: AUTOMÁTICO (P2P activo)\n";
                                    break;
                                default:
                                    diagnosis += $"   ?? Servicio DoSvc: Start={startType}\n";
                                    break;
                            }
                        }
                    }
                }

                diagnosis += "\n?? RECOMENDACIONES GAMING:\n";
                diagnosis += "   • Desactivar updates automáticos durante sesiones gaming\n";
                diagnosis += "   • Desactivar Delivery Optimization para máximo ancho de banda\n";
                diagnosis += "   • Programar updates manualmente fuera del horario gaming\n";
                diagnosis += "   • Monitorear uso de ancho de banda durante partidas\n";

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
        /// Diagnóstico completo del estado del sistema (alias para DiagnoseUpdateSettings)
        /// </summary>
        public static string DiagnoseSystemState()
        {
            return DiagnoseUpdateSettings();
        }
    }
}