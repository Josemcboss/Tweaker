using System;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.Win32;

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
        /// </summary>
        public static bool DisableTelemetry()
        {
            try
            {
                bool success = true;

                // 1. Desactivar Data Collection (HKLM)
                using (var key = Registry.LocalMachine.CreateSubKey(DATA_COLLECTION_KEY))
                {
                    if (key != null)
                    {
                        key.SetValue("AllowTelemetry", 0, RegistryValueKind.DWord);
                        key.SetValue("DoNotShowFeedbackNotifications", 1, RegistryValueKind.DWord);
                    }
                }

                // 2. Desactivar Advertising ID (HKCU)
                using (var key = Registry.CurrentUser.CreateSubKey(ADVERTISING_INFO_KEY))
                {
                    if (key != null)
                    {
                        key.SetValue("Enabled", 0, RegistryValueKind.DWord);
                    }
                }

                // 3. Configurar políticas de privacidad adicionales
                success &= ConfigurePrivacyPolicies();

                // 4. Desactivar Customer Experience Improvement Program
                success &= DisableCEIP();

                // 5. Desactivar Historial de Actividad y Clipboard Sync
                success &= DisableActivityHistory();

                // 6. Desactivar Localización y Sensores
                success &= DisableLocationAndSensors();

                // 7. Optimizar Búsqueda en Inicio (No Web Search)
                success &= DisableWebSearch();

                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en DisableTelemetry: {ex.Message}");
                return false;
            }
        }

        private static bool ConfigurePrivacyPolicies()
        {
            try
            {
                using (var key = Registry.LocalMachine.CreateSubKey(SIUF_RULES_KEY))
                {
                    if (key != null)
                    {
                        key.SetValue("DisableCloudOptimizedContent", 1, RegistryValueKind.DWord);
                        key.SetValue("DisableConsumerAccountStateContent", 1, RegistryValueKind.DWord);
                        key.SetValue("DisableSoftLanding", 1, RegistryValueKind.DWord);
                    }
                }

                using (var key = Registry.CurrentUser.CreateSubKey(PRIVACY_KEY))
                {
                    if (key != null)
                    {
                        key.SetValue("TailoredExperiencesWithDiagnosticDataEnabled", 0, RegistryValueKind.DWord);
                    }
                }

                return true;
            }
            catch { return false; }
        }

        private static bool DisableCEIP()
        {
            try
            {
                const string CEIP_KEY = @"SOFTWARE\Microsoft\SQMClient\Windows";
                using (var key = Registry.LocalMachine.OpenSubKey(CEIP_KEY, true))
                {
                    if (key != null)
                    {
                        key.SetValue("CEIPEnable", 0, RegistryValueKind.DWord);
                    }
                }
                return true;
            }
            catch { return true; }
        }

        private static bool DisableActivityHistory()
        {
            try
            {
                const string PUBLISH_USER_ACTIVITIES = @"SOFTWARE\Policies\Microsoft\Windows\System";
                using (var key = Registry.LocalMachine.CreateSubKey(PUBLISH_USER_ACTIVITIES))
                {
                    if (key != null)
                    {
                        key.SetValue("PublishUserActivities", 0, RegistryValueKind.DWord);
                        key.SetValue("UploadUserActivities", 0, RegistryValueKind.DWord);
                    }
                }
                return true;
            }
            catch { return false; }
        }

        private static bool DisableLocationAndSensors()
        {
            try
            {
                using (var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\LocationAndSensors"))
                {
                    if (key != null)
                    {
                        key.SetValue("DisableLocation", 1, RegistryValueKind.DWord);
                        key.SetValue("DisableLocationScripting", 1, RegistryValueKind.DWord);
                        key.SetValue("DisableSensors", 1, RegistryValueKind.DWord);
                    }
                }
                return true;
            }
            catch { return false; }
        }

        private static bool DisableWebSearch()
        {
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Policies\Microsoft\Windows\Explorer"))
                {
                    if (key != null)
                    {
                        key.SetValue("DisableSearchBoxSuggestions", 1, RegistryValueKind.DWord);
                    }
                }

                using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Search"))
                {
                    if (key != null)
                    {
                        key.SetValue("BingSearchEnabled", 0, RegistryValueKind.DWord);
                        key.SetValue("AllowSearchToUseLocation", 0, RegistryValueKind.DWord);
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public static bool EnableTelemetry()
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(DATA_COLLECTION_KEY, true))
                {
                    if (key != null)
                    {
                        key.DeleteValue("AllowTelemetry", false);
                        key.DeleteValue("DoNotShowFeedbackNotifications", false);
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public static bool DisableTelemetryAndTracking() => DisableTelemetry();
        public static bool EnableTelemetryAndTracking() => EnableTelemetry();
        public static bool DisableTelemetryServices() => false;
        public static bool EnableTelemetryServices() => true;
    }
}
