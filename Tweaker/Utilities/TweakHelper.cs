using System;
using System.Diagnostics;
using System.Windows; // Keep this for MessageBoxResult if ShowPrompt is intended to use it. If not, remove.

namespace Tweaker.Utilities
{
    /// <summary>
    /// Helpers para simplificar la ejecucin de tweaks con notificaciones y telemetra
    /// </summary>
    public class TweakHelper
    {
        private readonly TweakStateManager _stateManager;
        private readonly TelemetryService _telemetry;
        private readonly NotificationService _notifications;
        private static DateTime _lastRestorePointCreated = DateTime.MinValue;
        private static readonly TimeSpan RestorePointCooldown = TimeSpan.FromHours(24);

        public TweakHelper(
            TweakStateManager stateManager,
            TelemetryService telemetry,
            NotificationService notifications)
        {
            _stateManager = stateManager;
            _telemetry = telemetry;
            _notifications = notifications;

            // Inicializar servicio de backup de registro
            RegistryBackupService.Initialize();
        }

        /// <summary>
        /// Ejecuta un tweak y maneja notificaciones/telemetra automticamente
        /// Crea punto de restauracin automtico si es necesario
        /// </summary>
        public void ExecuteTweak(
            string tweakId,
            string category,
            Func<bool> action,
            string successMessage,
            string? errorMessage = null,
            bool requiresRestart = false,
            bool createRestorePoint = false,
            bool showNotification = true) // Nuevo parmetro
        {
            try
            {
                // VALIDAR LMITE DE TWEAKS ACTIVOS
                var currentLicense = License.LicenseManager.CurrentLicense;
                if (currentLicense != null && currentLicense.MaxTweaks != -1)
                {
                    int activeTweaksCount = _stateManager.ActiveTweaksCount;

                    // Verificar si ya est activado (no cuenta para el lmite si es una reactivacin)
                    bool isAlreadyActive = _stateManager.IsTweakEnabled(tweakId);

                    if (!isAlreadyActive && activeTweaksCount >= currentLicense.MaxTweaks)
                    {
                        if (showNotification) // Solo mostrar advertencia si las notificaciones estn habilitadas
                        {
                            // Se alcanz el lmite
                            string limitMessage = $"⚠️ LMITE DE TWEAKS ALCANZADO\n\n" +
                                                $"Tu licencia permite un mximo de {currentLicense.MaxTweaks} optimizaciones activas.\n\n" +
                                                $"Tweaks activos actualmente: {activeTweaksCount}/{currentLicense.MaxTweaks}\n\n" +
                                                $"Para activar ms tweaks:\n" +
                                                $"• Desactiva algn tweak existente\n" +
                                                $"• O actualiza tu licencia para obtener ms optimizaciones\n\n" +
                                                $"💡 Tip: Desactiva los tweaks que no uses para liberar espacio.";

                            _notifications.ShowWarning(limitMessage, "Lmite de Tweaks Alcanzado");
                        }
                        Debug.WriteLine($"⚠️ Lmite de tweaks alcanzado: {activeTweaksCount}/{currentLicense.MaxTweaks}");
                        return; // No continuar con la activacin
                    }
                }

                // Crear punto de restauracin si es necesario y no se ha creado uno recientemente
                if (createRestorePoint && ShouldCreateRestorePoint())
                {
                    Debug.WriteLine($"ℹ️ Creando punto de restauracin antes de aplicar: {tweakId}");
                    SystemRestore.CreateRestorePoint($"Tweaker - Antes de {tweakId}");
                    _lastRestorePointCreated = DateTime.Now;
                }

                // SEGURIDAD: Crear backup del estado actual del registro antes de aplicar el tweak
                // Esto no hace backup de valores especficos aqu, sino que se hace en cada optimizacin
                // que modifica el registro. Ver BaseOptimization.SetRegistryValue()
                Debug.WriteLine($"🔧 Sistema de backup activo para: {tweakId}");

                bool success = action();

                if (success)
                {
                    // Actualizar estado
                    _stateManager.SetTweakEnabled(tweakId, category);

                    // Trackear telemetra
                    _telemetry.TrackTweakEnabled(tweakId, category);

                    // Mostrar notificacin solo si showNotification es true
                    if (showNotification)
                    {
                        if (requiresRestart)
                        {
                            _notifications.ShowRestartRequired(GetTweakFriendlyName(tweakId));
                        }
                        else
                        {
                            _notifications.ShowSuccess(successMessage, "Tweak Activado");
                        }
                    }
                }
                else // if (!success)
                {
                    if (showNotification)
                    {
                        string message = errorMessage ?? "Error al aplicar el tweak. Verifica los permisos de administrador.";
                        _notifications.ShowError(message);
                    }
                }
            }
            catch (Exception ex)
            {
                // Mostrar notificacin de excepcin solo si showNotification es true
                if (showNotification)
                {
                    _notifications.ShowError($"Excepcin: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Verifica si se debe crear un punto de restauracin
        /// Windows solo permite uno cada 24 horas
        /// </summary>
        private bool ShouldCreateRestorePoint()
        {
            TimeSpan timeSinceLastRestore = DateTime.Now - _lastRestorePointCreated;
            return timeSinceLastRestore >= RestorePointCooldown;
        }

        /// <summary>
        /// Ejecuta la desactivacin de un tweak
        /// </summary>
        public void ExecuteTweakRevert(
            string tweakId,
            string category,
            Func<bool> action,
            string successMessage,
            string? errorMessage = null,
            bool requiresRestart = false,
            bool showNotification = true) // Nuevo parmetro
        {
            try
            {
                bool success = action();

                if (success)
                {
                    // Actualizar estado
                    _stateManager.SetTweakDisabled(tweakId);

                    // Trackear telemetra
                    _telemetry.TrackTweakDisabled(tweakId, category);

                    // Mostrar notificacin solo si showNotification es true
                    if (showNotification)
                    {
                        if (requiresRestart)
                        {
                            _notifications.ShowRestartRequired(GetTweakFriendlyName(tweakId));
                        }
                        else
                        {
                            _notifications.ShowInfo(successMessage, "Tweak Revertido");
                        }
                    }
                }
                else
                {
                    // Mostrar notificación de error solo si showNotification es true
                    if (showNotification)
                    {
                        string message = errorMessage ?? "Error al revertir el tweak. Verifica los permisos de administrador.";
                        _notifications.ShowError(message);
                    }
                }
            }
            catch (Exception ex)
            {
                // Mostrar notificacin de excepcin solo si showNotification es true
                if (showNotification)
                {
                    _notifications.ShowError($"Excepcin: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Ejecuta una accin simple (sin estado) con notificacin
        /// </summary>
        public void ExecuteAction(
            Func<bool> action,
            string successMessage,
            string? errorMessage = null,
            bool showNotification = true) // Nuevo parmetro
        {
            try
            {
                bool success = action();

                if (success)
                {
                    // Mostrar notificacin solo si showNotification es true
                    if (showNotification)
                    {
                        _notifications.ShowSuccess(successMessage);
                    }
                }
                else
                {
                    // Mostrar notificacin de error solo si showNotification es true
                    if (showNotification)
                    {
                        string message = errorMessage ?? "Error al ejecutar la accin.";
                        _notifications.ShowError(message);
                    }
                }
            }
            catch (Exception ex)
            {
                // Mostrar notificacin de excepcin solo si showNotification es true
                if (showNotification)
                {
                    _notifications.ShowError($"Excepcin: {ex.Message}");
                }
            }
        }

        private string GetTweakFriendlyName(string tweakId)
        {
            var names = new System.Collections.Generic.Dictionary<string, string>
            {
                {"MouseAcceleration", "Aceleracin del Mouse"},
                {"Keyboard", "Optimizacin de Teclado"},
                {"VisualEffects", "Efectos Visuales"},
                {"MemoryOptimization", "Optimizacin de RAM"},
                {"NetworkOptimization", "TCP/IP Optimization"},
                {"DnsCloudflare", "DNS Cloudflare"},
                {"DnsGoogle", "DNS Google"},
                {"DnsCache", "Cach DNS"},
                {"NetworkPower", "Ahorro de Energa de Red"},
                {"NetBios", "NetBIOS over TCP/IP"},
                {"SystemProfile", "System Profile Games"},
                {"GameDVR", "GameDVR / Xbox Game Bar"},
                {"GpuScheduling", "GPU Hardware Scheduling"},
                {"SystemResponsiveness", "System Responsiveness"},
                {"HighPerformance", "Plan Alto Rendimiento"},
                {"PowerThrottling", "Power Throttling"},
                {"CoreParking", "Core Parking"},
                {"Hibernation", "Hibernacin"},
                {"WindowsSearch", "Windows Search"},
                {"SysMain", "SysMain (SuperFetch)"},
                {"DiagTrack", "Telemetry (DiagTrack)"},
                {"MPO", "MPO (Multiplane Overlay)"},
                {"UltimatePower", "Ultimate Performance"},
                {"GameBar", "Xbox Game Bar"},
                {"CoreIsolation", "Core Isolation (VBS)"},
                {"HPET", "HPET"},
                {"HyperV", "Hyper-V"},
                {"SpectreMeltdown", "Spectre & Meltdown"}
            };

            return names.ContainsKey(tweakId) ? names[tweakId] : tweakId;
        }
    }
}
