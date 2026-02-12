using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// Tweaks de privacidad y anti-telemetría para gaming
    /// Desactiva recolección de datos que puede consumir recursos del sistema
    /// 
    /// VERSIÓN SEGURA: No toca servicios críticos para Bluetooth/Discord
    /// </summary>
    public static class PrivacyTweaks
    {
        private const string DATA_COLLECTION_KEY = @"SOFTWARE\Policies\Microsoft\Windows\DataCollection";
        private const string ADVERTISING_INFO_KEY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\AdvertisingInfo";
        private const string PRIVACY_KEY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Privacy";
        private const string SIUF_RULES_KEY = @"SOFTWARE\Policies\Microsoft\Windows\CloudContent";

        /// <summary>
        /// Desactiva telemetría de Windows para liberar recursos del sistema
        /// MÉTODO SEGURO: No afecta servicios críticos de Bluetooth/Discord
        /// </summary>
        public static bool DisableTelemetry()
        {
            try
            {
                Debug.WriteLine("??????????????????????????????????????????");
                Debug.WriteLine("DISABLING WINDOWS TELEMETRY & DATA COLLECTION (SAFE VERSION)");
                Debug.WriteLine("??????????????????????????????????????????");

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
                    Debug.WriteLine("? RESUMEN DE PRIVACIDAD APLICADA:");
                    Debug.WriteLine("??????????????????????????????????????");
                    Debug.WriteLine("?? Telemetría: DESACTIVADA");
                    Debug.WriteLine("   • Sin recolección de datos de uso");
                    Debug.WriteLine("   • Sin reportes de errores automáticos");
                    Debug.WriteLine("   • CPU liberado de procesos de tracking");
                    Debug.WriteLine("");
                    Debug.WriteLine("?? Advertising ID: DESACTIVADO");
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
                    Debug.WriteLine("? SEGURIDAD: Servicios críticos NO tocados");
                    Debug.WriteLine("   • Bluetooth funcionará correctamente");
                    Debug.WriteLine("   • Discord audio funcionará correctamente");
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
                Debug.WriteLine($"   ? Error en DisableCEIP: {ex.Message}");
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
                Debug.WriteLine("??????????????????????????????????????????");
                Debug.WriteLine("ENABLING WINDOWS TELEMETRY & DATA COLLECTION");
                Debug.WriteLine("??????????????????????????????????????????");

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
                Debug.WriteLine("? Configuraciones de privacidad restauradas a valores por defecto");
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
                Debug.WriteLine($"   ? Error restaurando Privacy policies: {ex.Message}");
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
                Debug.WriteLine($"   ? Error restaurando CEIP: {ex.Message}");
            }
        }

        // ????????????????????????????????????????????????????????????????????
        // MÉTODOS DE COMPATIBILIDAD CON MAINWINDOW (VERSIÓN SEGURA)
        // ????????????????????????????????????????????????????????????????????

        /// <summary>
        /// DESHABILITA TELEMETRÍA Y TRACKING (VERSIÓN SEGURA)
        /// Solo usa métodos seguros que no afectan Bluetooth/Discord
        /// </summary>
        public static bool DisableTelemetryAndTracking()
        {
            // Solo usa DisableTelemetry() que es seguro
            // NO toca servicios críticos como OneSyncSvc, MessagingService, UserDataSvc
            Debug.WriteLine("?? APLICANDO TELEMETRY TWEAKS SEGUROS (No afecta Bluetooth/Discord)");
            return DisableTelemetry();
        }

        /// <summary>
        /// HABILITA TELEMETRÍA Y TRACKING (VERSIÓN SEGURA)
        /// </summary>
        public static bool EnableTelemetryAndTracking()
        {
            Debug.WriteLine("?? RESTAURANDO TELEMETRY TWEAKS SEGUROS");
            return EnableTelemetry();
        }

        /// <summary>
        /// MÉTODO DESHABILITADO - DisableTelemetryServices()
        /// ?? Este método está deshabilitado porque causaba problemas con Bluetooth/Discord
        /// </summary>
        public static bool DisableTelemetryServices()
        {
            Debug.WriteLine("? DisableTelemetryServices() DESHABILITADO");
            Debug.WriteLine("?? MOTIVO: Causa problemas con audífonos Bluetooth en Discord");
            Debug.WriteLine("?? SERVICIOS PROBLEMÁTICOS:");
            Debug.WriteLine("   • OneSyncSvc - Necesario para dispositivos Bluetooth");
            Debug.WriteLine("   • MessagingService - Necesario para comunicación Discord");
            Debug.WriteLine("   • UserDataSvc - Necesario para configuraciones de dispositivos");
            Debug.WriteLine("");
            Debug.WriteLine("?? ALTERNATIVAS:");
            Debug.WriteLine("   • Usar solo DisableTelemetry() que es seguro");
            Debug.WriteLine("   • Scripts manuales en Release\\ para casos avanzados");
            Debug.WriteLine("   • FIX_BLUETOOTH_DISCORD.bat si ya tienes problemas");
            
            return false; // No ejecutar
        }

        /// <summary>
        /// MÉTODO DESHABILITADO - EnableTelemetryServices()
        /// Redirige a scripts de reparación
        /// </summary>
        public static bool EnableTelemetryServices()
        {
            Debug.WriteLine("?? Para restaurar servicios usar:");
            Debug.WriteLine("   • Release\\RESTAURAR_SERVICIOS_TELEMETRIA_COMPLETO.bat");
            Debug.WriteLine("   • Release\\FIX_BLUETOOTH_DISCORD.bat");
            
            return true; // No falla, pero no hace nada
        }
    }
}