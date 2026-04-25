using System;
using System.Diagnostics;

using Tweaker.License;

namespace Tweaker.Utilities
{
    public class TweakHelper
    {
        private readonly TweakStateManager _stateManager;
        private readonly TelemetryService _telemetry;
        private readonly NotificationService _notifications;
        private readonly ILicenseManager _licenseManager;
        private static DateTime _lastRestorePointCreated = DateTime.MinValue;
        private static readonly TimeSpan RestorePointCooldown = TimeSpan.FromHours(24);

        public TweakHelper(
            TweakStateManager stateManager,
            TelemetryService telemetry,
            NotificationService notifications,
            ILicenseManager licenseManager)
        {
            _stateManager = stateManager;
            _telemetry = telemetry;
            _notifications = notifications;
            _licenseManager = licenseManager;
            RegistryBackupService.Initialize();
        }

        public void ExecuteTweak(string tweakId, string category, Func<bool> action, string successMessage, string? errorMessage = null, bool requiresRestart = false, bool createRestorePoint = false, bool showNotification = true)
        {
            try
            {
                var currentLicense = _licenseManager.CurrentLicense;
                if (currentLicense != null && currentLicense.MaxTweaks != -1)
                {
                    int activeTweaksCount = _stateManager.ActiveTweaksCount;
                    if (!_stateManager.IsTweakEnabled(tweakId) && activeTweaksCount >= currentLicense.MaxTweaks)
                    {
                        if (showNotification) _notifications.ShowWarning("Límite de tweaks alcanzado.", "Error");
                        return;
                    }
                }

                if (createRestorePoint && ShouldCreateRestorePoint())
                {
                    SystemRestore.CreateRestorePoint($"Tweaker - {tweakId}");
                    _lastRestorePointCreated = DateTime.Now;
                }

                if (action())
                {
                    _stateManager.SetTweakEnabled(tweakId, category);
                    _telemetry.TrackTweakEnabled(tweakId, category);
                    if (showNotification)
                    {
                        if (requiresRestart) _notifications.ShowRestartRequired(tweakId);
                        else _notifications.ShowSuccess(successMessage, "Tweak Activado");
                    }
                }
                else if (showNotification)
                {
                    _notifications.ShowError(errorMessage ?? "Error al aplicar el tweak.");
                }
            }
            catch (Exception ex) { if (showNotification) _notifications.ShowError($"Excepción: {ex.Message}"); }
        }

        private bool ShouldCreateRestorePoint() => DateTime.Now - _lastRestorePointCreated >= RestorePointCooldown;

        public void ExecuteTweakRevert(string tweakId, string category, Func<bool> action, string successMessage, string? errorMessage = null, bool requiresRestart = false, bool showNotification = true)
        {
            try
            {
                if (action())
                {
                    _stateManager.SetTweakDisabled(tweakId);
                    _telemetry.TrackTweakDisabled(tweakId, category);
                    if (showNotification)
                    {
                        if (requiresRestart) _notifications.ShowRestartRequired(tweakId);
                        else _notifications.ShowInfo(successMessage, "Tweak Revertido");
                    }
                }
                else if (showNotification)
                {
                    _notifications.ShowError(errorMessage ?? "Error al revertir.");
                }
            }
            catch (Exception ex) { if (showNotification) _notifications.ShowError($"Excepción: {ex.Message}"); }
        }

        public void ExecuteAction(Func<bool> action, string successMessage, string? errorMessage = null, bool showNotification = true)
        {
            try
            {
                if (action()) { if (showNotification) _notifications.ShowSuccess(successMessage); }
                else if (showNotification) _notifications.ShowError(errorMessage ?? "Error al ejecutar.");
            }
            catch (Exception ex) { if (showNotification) _notifications.ShowError($"Excepción: {ex.Message}"); }
        }
    }
}
