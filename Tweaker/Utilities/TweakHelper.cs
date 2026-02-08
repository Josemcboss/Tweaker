using System;
using System.Diagnostics;
using System.Windows;

namespace Tweaker.Utilities
{
    /// <summary>
    /// Helpers para simplificar la ejecución de tweaks con notificaciones y telemetría
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
        }

        /// <summary>
        /// Ejecuta un tweak y maneja notificaciones/telemetría automáticamente
        /// Crea punto de restauración automático si es necesario
        /// </summary>
        public void ExecuteTweak(
            string tweakId,
            string category,
            Func<bool> action,
            string successMessage,
            string errorMessage = null,
            bool requiresRestart = false,
            bool createRestorePoint = false)
        {
            try
            {
                // Crear punto de restauración si es necesario y no se ha creado uno recientemente
                if (createRestorePoint && ShouldCreateRestorePoint())
                {
                    Debug.WriteLine($"?? Creando punto de restauración antes de aplicar: {tweakId}");
                    SystemRestore.CreateRestorePoint($"Tweaker - Antes de {tweakId}");
                    _lastRestorePointCreated = DateTime.Now;
                }

                bool success = action();

                if (success)
                {
                    // Actualizar estado
                    _stateManager.SetTweakEnabled(tweakId, category);
                    
                    // Trackear telemetría
                    _telemetry.TrackTweakEnabled(tweakId, category);

                    // Mostrar notificación
                    if (requiresRestart)
                    {
                        _notifications.ShowRestartRequired(GetTweakFriendlyName(tweakId));
                    }
                    else
                    {
                        _notifications.ShowSuccess(successMessage, "Tweak Activado");
                    }
                }
                else
                {
                    string message = errorMessage ?? "Error al aplicar el tweak. Verifica los permisos de administrador.";
                    _notifications.ShowError(message);
                }
            }
            catch (Exception ex)
            {
                _notifications.ShowError($"Excepción: {ex.Message}");
            }
        }

        /// <summary>
        /// Verifica si se debe crear un punto de restauración
        /// Windows solo permite uno cada 24 horas
        /// </summary>
        private bool ShouldCreateRestorePoint()
        {
            TimeSpan timeSinceLastRestore = DateTime.Now - _lastRestorePointCreated;
            return timeSinceLastRestore >= RestorePointCooldown;
        }

        /// <summary>
        /// Ejecuta la desactivación de un tweak
        /// </summary>
        public void ExecuteTweakRevert(
            string tweakId,
            string category,
            Func<bool> action,
            string successMessage,
            string errorMessage = null,
            bool requiresRestart = false)
        {
            try
            {
                bool success = action();

                if (success)
                {
                    // Actualizar estado
                    _stateManager.SetTweakDisabled(tweakId);
                    
                    // Trackear telemetría
                    _telemetry.TrackTweakDisabled(tweakId, category);

                    // Mostrar notificación
                    if (requiresRestart)
                    {
                        _notifications.ShowRestartRequired(GetTweakFriendlyName(tweakId));
                    }
                    else
                    {
                        _notifications.ShowInfo(successMessage, "Tweak Revertido");
                    }
                }
                else
                {
                    string message = errorMessage ?? "Error al revertir el tweak. Verifica los permisos de administrador.";
                    _notifications.ShowError(message);
                }
            }
            catch (Exception ex)
            {
                _notifications.ShowError($"Excepción: {ex.Message}");
            }
        }

        /// <summary>
        /// Ejecuta una acción simple (sin estado) con notificación
        /// </summary>
        public void ExecuteAction(
            Func<bool> action,
            string successMessage,
            string errorMessage = null)
        {
            try
            {
                bool success = action();

                if (success)
                {
                    _notifications.ShowSuccess(successMessage);
                }
                else
                {
                    string message = errorMessage ?? "Error al ejecutar la acción.";
                    _notifications.ShowError(message);
                }
            }
            catch (Exception ex)
            {
                _notifications.ShowError($"Excepción: {ex.Message}");
            }
        }

        private string GetTweakFriendlyName(string tweakId)
        {
            var names = new System.Collections.Generic.Dictionary<string, string>
            {
                {"MouseAcceleration", "Aceleración del Mouse"},
                {"Keyboard", "Optimización de Teclado"},
                {"VisualEffects", "Efectos Visuales"},
                {"MemoryOptimization", "Optimización de RAM"},
                {"NetworkOptimization", "TCP/IP Optimization"},
                {"DnsCloudflare", "DNS Cloudflare"},
                {"DnsGoogle", "DNS Google"},
                {"DnsCache", "Caché DNS"},
                {"NetworkPower", "Ahorro de Energía de Red"},
                {"NetBios", "NetBIOS over TCP/IP"},
                {"SystemProfile", "System Profile Games"},
                {"GameDVR", "GameDVR / Xbox Game Bar"},
                {"GpuScheduling", "GPU Hardware Scheduling"},
                {"SystemResponsiveness", "System Responsiveness"},
                {"HighPerformance", "Plan Alto Rendimiento"},
                {"PowerThrottling", "Power Throttling"},
                {"CoreParking", "Core Parking"},
                {"Hibernation", "Hibernación"},
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
