using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// Tweaks de privacidad y anti-telemetría para gaming
    /// Desactiva recolección de datos que puede consumir recursos del sistema
    /// </summary>
    public static class PrivacyTweaks
    {
        private const string DATA_COLLECTION_KEY = @"SOFTWARE\Policies\Microsoft\Windows\DataCollection";
        private const string ADVERTISING_INFO_KEY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\AdvertisingInfo";
        private const string PRIVACY_KEY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Privacy";
        private const string SIUF_RULES_KEY = @"SOFTWARE\Policies\Microsoft\Windows\CloudContent";

        /// <summary>
        /// Desactiva telemetría de Windows para liberar recursos del sistema
        /// Reduce tráfico de red y uso de CPU en segundo plano
        /// </summary>
        public static bool DisableTelemetry()
        {
            try
            {
                Debug.WriteLine("???????????????????????????????????????????????????????????");
                Debug.WriteLine("DISABLING WINDOWS TELEMETRY & DATA COLLECTION");
                Debug.WriteLine("???????????????????????????????????????????????????????????");

                bool success = true;

                // 1. Desactivar Data Collection (HKLM)
                Debug.WriteLine("?? Desactivando Data Collection...");
                using (var key = Registry.LocalMachine.CreateSubKey(DATA_COLLECTION_KEY))
                {
                    if (key != null)
                    {
                        // AllowTelemetry = 0 (Desactivado completamente)
                        key.SetValue("AllowTelemetry", 0, RegistryValueKind.DWord);
                        Debug.WriteLine("? AllowTelemetry = 0 (Telemetry disabled)");

                        // DoNotShowFeedbackNotifications = 1
                        key.SetValue("DoNotShowFeedbackNotifications", 1, RegistryValueKind.DWord);
                        Debug.WriteLine("? Feedback notifications disabled");
                    }
                    else
                    {
                        Debug.WriteLine("? No se pudo crear clave de Data Collection");
                        success = false;
                    }
                }

                // 2. Desactivar Advertising ID (HKCU)
                Debug.WriteLine("");
                Debug.WriteLine("?? Desactivando Advertising ID...");
                using (var key = Registry.CurrentUser.CreateSubKey(ADVERTISING_INFO_KEY))
                {
                    if (key != null)
                    {
                        // Enabled = 0 (Sin Advertising ID)
                        key.SetValue("Enabled", 0, RegistryValueKind.DWord);
                        Debug.WriteLine("? Advertising ID disabled");
                    }
                    else
                    {
                        Debug.WriteLine("? No se pudo crear clave de Advertising Info");
                        success = false;
                    }
                }

                // 3. Configurar políticas de privacidad adicionales
                Debug.WriteLine("");
                Debug.WriteLine("?? Configurando Privacy policies...");
                success &= ConfigurePrivacyPolicies();

                // 4. Desactivar Customer Experience Improvement Program
                Debug.WriteLine("");
                Debug.WriteLine("?? Desactivando CEIP...");
                success &= DisableCEIP();

                if (success)
                {
                    Debug.WriteLine("");
                    Debug.WriteLine("?? RESUMEN DE PRIVACIDAD APLICADA:");
                    Debug.WriteLine("??????????????????????????????????");
                    Debug.WriteLine("? Telemetría: DESACTIVADA");
                    Debug.WriteLine("   • Sin recolección de datos de uso");
                    Debug.WriteLine("   • Sin reportes de errores automáticos");
                    Debug.WriteLine("   • CPU liberado de procesos de tracking");
                    Debug.WriteLine("");
                    Debug.WriteLine("? Advertising ID: DESACTIVADO");
                    Debug.WriteLine("   • Sin personalización de anuncios");
                    Debug.WriteLine("   • Sin seguimiento entre aplicaciones");
                    Debug.WriteLine("   • Menos tráfico de red");
                    Debug.WriteLine("");
                    Debug.WriteLine("?? BENEFICIOS GAMING:");
                    Debug.WriteLine("   • Menos procesos en segundo plano");
                    Debug.WriteLine("   • Ancho de banda dedicado al gaming");
                    Debug.WriteLine("   • CPU focus en el juego, no en tracking");
                    Debug.WriteLine("   • Mayor privacidad y control");
                    Debug.WriteLine("");
                    Debug.WriteLine("?? NOTA: Algunas funciones de Cortana pueden verse afectadas");
                }

                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? ERROR en DisableTelemetry: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Configura políticas adicionales de privacidad
        /// </summary>
        private static bool ConfigurePrivacyPolicies()
        {
            try
            {
                // Cloud Content policies
                using (var key = Registry.LocalMachine.CreateSubKey(SIUF_RULES_KEY))
                {
                    if (key != null)
                    {
                        // DisableCloudOptimizedContent = 1
                        key.SetValue("DisableCloudOptimizedContent", 1, RegistryValueKind.DWord);
                        Debug.WriteLine("   ? Cloud optimized content disabled");

                        // DisableConsumerAccountStateContent = 1
                        key.SetValue("DisableConsumerAccountStateContent", 1, RegistryValueKind.DWord);
                        Debug.WriteLine("   ? Consumer account content disabled");

                        // DisableSoftLanding = 1
                        key.SetValue("DisableSoftLanding", 1, RegistryValueKind.DWord);
                        Debug.WriteLine("   ? Soft landing disabled");
                    }
                }

                // Privacy settings (HKCU)
                using (var key = Registry.CurrentUser.CreateSubKey(PRIVACY_KEY))
                {
                    if (key != null)
                    {
                        // TailoredExperiencesWithDiagnosticDataEnabled = 0
                        key.SetValue("TailoredExperiencesWithDiagnosticDataEnabled", 0, RegistryValueKind.DWord);
                        Debug.WriteLine("   ? Tailored experiences disabled");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"   ? Error en ConfigurePrivacyPolicies: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Desactiva Customer Experience Improvement Program
        /// </summary>
        private static bool DisableCEIP()
        {
            try
            {
                const string CEIP_KEY = @"SOFTWARE\Microsoft\SQMClient\Windows";
                
                using (var key = Registry.LocalMachine.OpenSubKey(CEIP_KEY, true))
                {
                    if (key != null)
                    {
                        // CEIPEnable = 0
                        key.SetValue("CEIPEnable", 0, RegistryValueKind.DWord);
                        Debug.WriteLine("   ? Customer Experience Improvement Program disabled");
                        return true;
                    }
                    else
                    {
                        Debug.WriteLine("   ?? CEIP key not found (already disabled or different Windows version)");
                        return true; // No es crítico
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"   ?? Error en DisableCEIP: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Restaura telemetría y configuraciones de privacidad a valores por defecto
        /// </summary>
        public static bool EnableTelemetry()
        {
            try
            {
                Debug.WriteLine("???????????????????????????????????????????????????????????");
                Debug.WriteLine("ENABLING WINDOWS TELEMETRY & DATA COLLECTION");
                Debug.WriteLine("???????????????????????????????????????????????????????????");

                bool success = true;

                // Restaurar Data Collection
                Debug.WriteLine("?? Restaurando Data Collection...");
                using (var key = Registry.LocalMachine.OpenSubKey(DATA_COLLECTION_KEY, true))
                {
                    if (key != null)
                    {
                        // AllowTelemetry = 1 (Básico) o eliminar para usar default
                        key.DeleteValue("AllowTelemetry", false);
                        key.DeleteValue("DoNotShowFeedbackNotifications", false);
                        Debug.WriteLine("? Data Collection restored to default");
                    }
                }

                // Restaurar Advertising ID
                Debug.WriteLine("");
                Debug.WriteLine("?? Restaurando Advertising ID...");
                using (var key = Registry.CurrentUser.OpenSubKey(ADVERTISING_INFO_KEY, true))
                {
                    if (key != null)
                    {
                        key.DeleteValue("Enabled", false);
                        Debug.WriteLine("? Advertising ID restored to default");
                    }
                }

                // Restaurar Privacy policies
                Debug.WriteLine("");
                Debug.WriteLine("?? Restaurando Privacy policies...");
                RestorePrivacyPolicies();

                // Restaurar CEIP
                Debug.WriteLine("");
                Debug.WriteLine("?? Restaurando CEIP...");
                RestoreCEIP();

                Debug.WriteLine("");
                Debug.WriteLine("?? Configuraciones de privacidad restauradas a valores por defecto");
                Debug.WriteLine("?? Telemetría volverá a estar activa según configuración de Windows");

                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? ERROR en EnableTelemetry: {ex.Message}");
                return false;
            }
        }

        private static void RestorePrivacyPolicies()
        {
            try
            {
                // Cloud Content policies
                using (var key = Registry.LocalMachine.OpenSubKey(SIUF_RULES_KEY, true))
                {
                    if (key != null)
                    {
                        key.DeleteValue("DisableCloudOptimizedContent", false);
                        key.DeleteValue("DisableConsumerAccountStateContent", false);
                        key.DeleteValue("DisableSoftLanding", false);
                        Debug.WriteLine("   ? Cloud Content policies restored");
                    }
                }

                // Privacy settings
                using (var key = Registry.CurrentUser.OpenSubKey(PRIVACY_KEY, true))
                {
                    if (key != null)
                    {
                        key.DeleteValue("TailoredExperiencesWithDiagnosticDataEnabled", false);
                        Debug.WriteLine("   ? Privacy settings restored");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"   ?? Error restaurando Privacy policies: {ex.Message}");
            }
        }

        private static void RestoreCEIP()
        {
            try
            {
                const string CEIP_KEY = @"SOFTWARE\Microsoft\SQMClient\Windows";
                
                using (var key = Registry.LocalMachine.OpenSubKey(CEIP_KEY, true))
                {
                    if (key != null)
                    {
                        key.DeleteValue("CEIPEnable", false);
                        Debug.WriteLine("   ? CEIP restored to default");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"   ?? Error restaurando CEIP: {ex.Message}");
            }
        }

        /// <summary>
        /// Desactiva servicios relacionados con telemetría
        /// </summary>
        public static bool DisableTelemetryServices()
        {
            try
            {
                Debug.WriteLine("???????????????????????????????????????????????????????????");
                Debug.WriteLine("DISABLING TELEMETRY SERVICES");
                Debug.WriteLine("???????????????????????????????????????????????????????????");

                bool success = true;
                string[] telemetryServices = {
                    "DiagTrack",                    // Connected User Experiences and Telemetry
                    "dmwappushservice",            // WAP Push Message Routing Service
                    "WerSvc",                      // Windows Error Reporting Service
                    "OneSyncSvc",                  // Sync Host Service
                    "MessagingService",            // MessagingService
                    "PimIndexMaintenanceSvc",      // Contact Data
                    "UserDataSvc",                 // User Data Access
                    "UnistoreSvc",                 // User Data Storage
                    // "BrokerInfrastructure",     // REMOVED: CRITICAL SYSTEM SERVICE - DO NOT DISABLE
                    "DcpSvc",                      // Data Collection and Publishing Service
                };

                foreach (string serviceName in telemetryServices)
                {
                    try
                    {
                        const string SERVICE_KEY_BASE = @"SYSTEM\CurrentControlSet\Services\";
                        using (var key = Registry.LocalMachine.OpenSubKey(SERVICE_KEY_BASE + serviceName, true))
                        {
                            if (key != null)
                            {
                                // Start = 4 (Disabled)
                                key.SetValue("Start", 4, RegistryValueKind.DWord);
                                Debug.WriteLine($"? {serviceName}: Disabled");
                            }
                            else
                            {
                                Debug.WriteLine($"?? {serviceName}: Service not found (OK)");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"?? {serviceName}: Error - {ex.Message}");
                        success = false;
                    }
                }

                if (success)
                {
                    Debug.WriteLine("");
                    Debug.WriteLine("?? SERVICIOS DE TELEMETRÍA DESACTIVADOS:");
                    Debug.WriteLine("   • Connected User Experiences and Telemetry");
                    Debug.WriteLine("   • Windows Error Reporting");
                    Debug.WriteLine("   • WAP Push Message Routing");
                    Debug.WriteLine("   • User Data Collection services");
                    Debug.WriteLine("");
                    Debug.WriteLine("?? BENEFICIO GAMING:");
                    Debug.WriteLine("   • Menos procesos corriendo en segundo plano");
                    Debug.WriteLine("   • Recursos del sistema liberados para juegos");
                    Debug.WriteLine("   • Menor uso de CPU y memoria");
                    Debug.WriteLine("");
                    Debug.WriteLine("?? REQUIERE REINICIO para que los servicios se desactiven");
                }

                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? ERROR en DisableTelemetryServices: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Restaura servicios de telemetría a configuración por defecto
        /// </summary>
        public static bool EnableTelemetryServices()
        {
            try
            {
                Debug.WriteLine("???????????????????????????????????????????????????????????");
                Debug.WriteLine("ENABLING TELEMETRY SERVICES");
                Debug.WriteLine("???????????????????????????????????????????????????????????");

                var serviceDefaults = new Dictionary<string, int>
                {
                    { "DiagTrack", 2 },                    // Automatic
                    { "dmwappushservice", 3 },             // Manual
                    { "WerSvc", 3 },                       // Manual
                    { "OneSyncSvc", 3 },                   // Manual
                    { "MessagingService", 3 },             // Manual
                    { "PimIndexMaintenanceSvc", 3 },       // Manual
                    { "UserDataSvc", 3 },                  // Manual
                    { "UnistoreSvc", 3 },                  // Manual
                    // { "BrokerInfrastructure", 2 },      // REMOVED: CRITICAL SYSTEM SERVICE - DO NOT MODIFY
                    { "DcpSvc", 3 },                       // Manual
                };

                bool success = true;
                const string SERVICE_KEY_BASE = @"SYSTEM\CurrentControlSet\Services\";

                foreach (var service in serviceDefaults)
                {
                    try
                    {
                        using (var key = Registry.LocalMachine.OpenSubKey(SERVICE_KEY_BASE + service.Key, true))
                        {
                            if (key != null)
                            {
                                key.SetValue("Start", service.Value, RegistryValueKind.DWord);
                                string startType = service.Value == 2 ? "Automatic" : "Manual";
                                Debug.WriteLine($"? {service.Key}: {startType}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"?? {service.Key}: Error - {ex.Message}");
                        success = false;
                    }
                }

                if (success)
                {
                    Debug.WriteLine("");
                    Debug.WriteLine("?? Servicios de telemetría restaurados a configuración por defecto");
                    Debug.WriteLine("?? REQUIERE REINICIO para que los cambios tomen efecto");
                }

                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? ERROR en EnableTelemetryServices: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Diagnóstico completo del estado de privacidad y telemetría
        /// </summary>
        public static string DiagnosePrivacySettings()
        {
            try
            {
                var diagnosis = "???????????????????????????????????????????????????????????\n";
                diagnosis += "DIAGNÓSTICO PRIVACY & TELEMETRY SETTINGS\n";
                diagnosis += "???????????????????????????????????????????????????????????\n\n";

                // Verificar Data Collection
                diagnosis += "?? TELEMETRY & DATA COLLECTION:\n";
                using (var key = Registry.LocalMachine.OpenSubKey(DATA_COLLECTION_KEY))
                {
                    if (key != null)
                    {
                        var allowTelemetry = key.GetValue("AllowTelemetry");
                        var feedbackNotif = key.GetValue("DoNotShowFeedbackNotifications");

                        if (allowTelemetry != null)
                        {
                            int level = (int)allowTelemetry;
                            string levelDesc = level switch
                            {
                                0 => "DESACTIVADO (Security only)",
                                1 => "BÁSICO (Basic)",
                                2 => "MEJORADO (Enhanced)",
                                3 => "COMPLETO (Full)",
                                _ => $"Valor: {level}"
                            };
                            diagnosis += $"   ?? AllowTelemetry: {levelDesc}\n";

                            if (level == 0)
                            {
                                diagnosis += "   ? Telemetría optimizada para gaming\n";
                            }
                            else
                            {
                                diagnosis += "   ?? Telemetría activa (puede afectar performance)\n";
                            }
                        }
                        else
                        {
                            diagnosis += "   ?? AllowTelemetry: Configuración por defecto\n";
                        }

                        diagnosis += $"   ?? Feedback notifications: {(feedbackNotif != null && (int)feedbackNotif == 1 ? "Disabled" : "Enabled")}\n";
                    }
                    else
                    {
                        diagnosis += "   ?? Configuración por defecto (Telemetría activa)\n";
                    }
                }

                // Verificar Advertising ID
                diagnosis += "\n?? ADVERTISING & TRACKING:\n";
                using (var key = Registry.CurrentUser.OpenSubKey(ADVERTISING_INFO_KEY))
                {
                    if (key != null)
                    {
                        var enabled = key.GetValue("Enabled");
                        if (enabled != null && (int)enabled == 0)
                        {
                            diagnosis += "   ? Advertising ID: DESACTIVADO\n";
                        }
                        else
                        {
                            diagnosis += "   ?? Advertising ID: ACTIVO\n";
                        }
                    }
                    else
                    {
                        diagnosis += "   ?? Advertising ID: Configuración por defecto\n";
                    }
                }

                // Verificar servicios de telemetría
                diagnosis += "\n?? SERVICIOS DE TELEMETRÍA:\n";
                string[] keyServices = { "DiagTrack", "dmwappushservice", "WerSvc" };
                const string SERVICE_KEY_BASE = @"SYSTEM\CurrentControlSet\Services\";

                foreach (string serviceName in keyServices)
                {
                    try
                    {
                        using (var key = Registry.LocalMachine.OpenSubKey(SERVICE_KEY_BASE + serviceName))
                        {
                            if (key != null)
                            {
                                var startValue = key.GetValue("Start");
                                if (startValue != null)
                                {
                                    int startType = (int)startValue;
                                    string status = startType switch
                                    {
                                        2 => "AUTOMATIC",
                                        3 => "MANUAL",
                                        4 => "DISABLED",
                                        _ => $"Start={startType}"
                                    };
                                    
                                    diagnosis += $"   ?? {serviceName}: {status}";
                                    if (startType == 4)
                                    {
                                        diagnosis += " ? (Gaming optimized)\n";
                                    }
                                    else
                                    {
                                        diagnosis += " ?? (Active)\n";
                                    }
                                }
                            }
                            else
                            {
                                diagnosis += $"   ?? {serviceName}: No encontrado\n";
                            }
                        }
                    }
                    catch
                    {
                        diagnosis += $"   ? {serviceName}: Error leyendo estado\n";
                    }
                }

                diagnosis += "\n?? RECOMENDACIONES GAMING:\n";
                diagnosis += "   • Desactivar telemetría para liberar CPU\n";
                diagnosis += "   • Deshabilitar Advertising ID para menos tracking\n";
                diagnosis += "   • Desactivar servicios de telemetría innecesarios\n";
                diagnosis += "   • Mantener solo servicios esenciales de seguridad\n";

                diagnosis += "\n?? BALANCE PRIVACIDAD vs FUNCIONALIDAD:\n";
                diagnosis += "   • Level 0: Máxima privacidad, mínima funcionalidad\n";
                diagnosis += "   • Level 1: Básico, balance entre privacidad y función\n";
                diagnosis += "   • Level 2+: Más datos enviados, más funcionalidades\n";
                diagnosis += "   • Para gaming competitivo: Recomendado Level 0\n";

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

        // ???????????????????????????????????????????????????????????????????
        // MÉTODOS PARA COMPATIBILIDAD CON MAINWINDOW
        // ???????????????????????????????????????????????????????????????????

        /// <summary>
        /// DESHABILITA TELEMETRÍA Y TRACKING (Combina DisableTelemetry + servicios)
        /// </summary>
        public static bool DisableTelemetryAndTracking()
        {
            try
            {
                Debug.WriteLine("?? DESHABILITANDO TELEMETRÍA Y TRACKING COMPLETO");
                Debug.WriteLine("???????????????????????????????????????????????");

                bool success = true;
                
                // Deshabilitar telemetría principal
                success &= DisableTelemetry();
                
                // Deshabilitar servicios de telemetría
                success &= DisableTelemetryServices();

                if (success)
                {
                    Debug.WriteLine("\n?? TELEMETRÍA Y TRACKING COMPLETAMENTE DESHABILITADOS");
                    Debug.WriteLine("   Beneficios:");
                    Debug.WriteLine("   • CPU liberado de procesos de tracking");
                    Debug.WriteLine("   • Menos tráfico de red");
                    Debug.WriteLine("   • Mayor privacidad");
                    Debug.WriteLine("   • Recursos dedicados al gaming");
                }

                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error en DisableTelemetryAndTracking: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// HABILITA TELEMETRÍA Y TRACKING (Restaura configuración por defecto)
        /// </summary>
        public static bool EnableTelemetryAndTracking()
        {
            try
            {
                Debug.WriteLine("?? RESTAURANDO TELEMETRÍA Y TRACKING");
                Debug.WriteLine("???????????????????????????????????????????????");

                bool success = true;
                
                // Restaurar telemetría principal
                success &= EnableTelemetry();
                
                // Restaurar servicios de telemetría
                success &= EnableTelemetryServices();

                if (success)
                {
                    Debug.WriteLine("? Telemetría y tracking restaurados a configuración por defecto");
                }

                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error en EnableTelemetryAndTracking: {ex.Message}");
                return false;
            }
        }
    }
}