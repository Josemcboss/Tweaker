using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Tweaker.Services
{
    /// <summary>
    /// Detailed report produced after inspecting real PC state against saved state.
    /// </summary>
    public class SyncResult
    {
        public int SyncedCount { get; set; }
        public int ActiveCount { get; set; }
        public int NewlyDetectedCount { get; set; }
        public int DiscrepanciesCorrected { get; set; }
        public List<string> RevertedByWindows { get; set; } = new List<string>();
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public bool HasRevertedTweaks => RevertedByWindows.Count > 0;
    }

    /// <summary>
    /// Reconciles tweaks_state.json with real system state (Registry, Services, WMI, PowerCFG).
    /// Detects tweaks already enabled on the PC, as well as tweaks reverted by Windows Update.
    /// </summary>
    public static class StartupStateVerifier
    {
        public static SyncResult? LastSyncResult { get; private set; }

        /// <summary>
        /// Runs asynchronously so it does not block the UI thread.
        /// Call after InjectDispatcher() and before UpdateNamedToggles().
        /// </summary>
        public static async Task<SyncResult> SyncRealStateAsync(
            ITweakDispatcher dispatcher,
            Utilities.TweakStateManager stateManager)
        {
            return await Task.Run(() =>
            {
                var result = new SyncResult();
                var canonicalIds = dispatcher.GetCanonicalTweakIds();

                foreach (var tweakId in canonicalIds)
                {
                    try
                    {
                        bool? realState = dispatcher.GetRealState(tweakId);

                        // GetRealState returns null when no check is implemented for this tweak.
                        if (realState is null)
                            continue;

                        result.SyncedCount++;
                        bool jsonState = stateManager.IsTweakEnabled(tweakId);

                        if (realState.Value)
                        {
                            result.ActiveCount++;
                            if (!jsonState)
                            {
                                result.NewlyDetectedCount++;
                                result.DiscrepanciesCorrected++;
                                Debug.WriteLine($"[StartupSync] ✨ Found pre-existing active tweak '{tweakId}' in Windows → setting ON.");
                                stateManager.SetTweakEnabled(tweakId, "System Detected");
                            }
                        }
                        else
                        {
                            // Si el chequeo en vivo es false pero el usuario lo había activado:
                            // NO borrar ni desactivar el estado guardado del usuario (puede requerir reinicio
                            // o permisos específicos). Solo se registra para información o re-aplicación manual.
                            if (jsonState)
                            {
                                result.RevertedByWindows.Add(tweakId);
                                Debug.WriteLine($"[StartupSync] ℹ️ Tweak '{tweakId}' guardado como activo, pendiente de verificación en sistema o reinicio.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[StartupSync] Error checking '{tweakId}': {ex.Message}");
                    }
                }

                LastSyncResult = result;
                Debug.WriteLine($"[StartupSync] ✅ Synced {result.SyncedCount} tweaks. Active on PC: {result.ActiveCount}, Newly detected: {result.NewlyDetectedCount}, Reverted by Windows: {result.RevertedByWindows.Count}");
                return result;
            });
        }

        /// <summary>
        /// One-click reapplication of any tweaks that were disabled externally by Windows Update.
        /// </summary>
        public static async Task<int> ReapplyRevertedTweaksAsync(
            ITweakDispatcher dispatcher,
            Utilities.TweakStateManager stateManager)
        {
            if (LastSyncResult == null || LastSyncResult.RevertedByWindows.Count == 0)
                return 0;

            return await Task.Run(() =>
            {
                int reApplied = 0;
                var toReapply = new List<string>(LastSyncResult.RevertedByWindows);
                foreach (var tweakId in toReapply)
                {
                    try
                    {
                        if (dispatcher.ApplyTweak(tweakId))
                        {
                            stateManager.SetTweakEnabled(tweakId, "Re-applied (Protection)");
                            reApplied++;
                            LastSyncResult.RevertedByWindows.Remove(tweakId);
                            Debug.WriteLine($"[StartupSync] 🛡️ Successfully re-applied reverted tweak: {tweakId}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[StartupSync] Failed to reapply '{tweakId}': {ex.Message}");
                    }
                }
                return reApplied;
            });
        }
    }
}
