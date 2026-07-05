using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Tweaker.Services
{
    /// <summary>
    /// On app startup, reconciles tweaks_state.json with real system state.
    /// For every tweak that has a GetRealState() implementation, it queries
    /// the live registry/WMI and corrects any discrepancy silently.
    /// </summary>
    public static class StartupStateVerifier
    {
        /// <summary>
        /// Runs asynchronously so it does not block the UI thread.
        /// Call after InjectDispatcher() and before UpdateNamedToggles().
        /// </summary>
        public static async Task SyncRealStateAsync(
            ITweakDispatcher dispatcher,
            Utilities.TweakStateManager stateManager)
        {
            await Task.Run(() =>
            {
                int synced = 0;
                int mismatches = 0;

                var canonicalIds = dispatcher.GetCanonicalTweakIds();

                foreach (var tweakId in canonicalIds)
                {
                    try
                    {
                        bool? realState = dispatcher.GetRealState(tweakId);

                        // GetRealState returns null when no check is implemented for this tweak.
                        if (realState is null)
                            continue;

                        bool jsonState = stateManager.IsTweakEnabled(tweakId);

                        if (realState.Value != jsonState)
                        {
                            mismatches++;
                            Debug.WriteLine($"[StartupSync] Discrepancy on '{tweakId}': JSON={jsonState}, System={realState.Value} → correcting.");

                            // Silently correct the JSON to match reality.
                            if (realState.Value)
                                stateManager.SetTweakEnabled(tweakId, "System Sync");
                            else
                                stateManager.SetTweakDisabled(tweakId);
                        }

                        synced++;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[StartupSync] Error checking '{tweakId}': {ex.Message}");
                    }
                }

                Debug.WriteLine($"[StartupSync] ✅ Synced {synced} tweaks, corrected {mismatches} discrepancies.");
            });
        }
    }
}
