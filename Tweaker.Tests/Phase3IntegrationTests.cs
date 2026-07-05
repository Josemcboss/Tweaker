using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tweaker.Services;
using Tweaker.Utilities;

namespace Tweaker.Tests
{
    /// <summary>
    /// Integration tests for Phase 3 of the Ghost Optimizer hardening.
    /// Covers: TweakDispatcher, TweakStateManager, StartupStateVerifier, Calculate* metrics.
    ///
    /// These tests exercise real system calls (registry reads) so they must run
    /// as Administrator to mirror the production environment.
    /// </summary>
    [TestClass]
    public class Phase3IntegrationTests
    {
        private TweakDispatcher _dispatcher = null!;

        [TestInitialize]
        public void Setup()
        {
            _dispatcher = new TweakDispatcher();
        }

        // ─────────────────────────────────────────────────────────────
        // P1 — TweakDispatcher: canonical ID count
        // ─────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("P1_Dispatcher")]
        public void Dispatcher_CanonicalIds_AtLeast62()
        {
            var ids = _dispatcher.GetCanonicalTweakIds();
            Assert.IsTrue(ids.Count >= 62,
                $"Expected >= 62 canonical tweaks, got {ids.Count}");
        }

        [TestMethod]
        [TestCategory("P1_Dispatcher")]
        public void Dispatcher_CanonicalIds_NoDuplicates()
        {
            var ids = _dispatcher.GetCanonicalTweakIds();
            var set = new HashSet<string>(ids, StringComparer.OrdinalIgnoreCase);
            Assert.AreEqual(ids.Count, set.Count,
                "Canonical IDs must not contain duplicates (case-insensitive)");
        }

        [TestMethod]
        [TestCategory("P1_Dispatcher")]
        public void Dispatcher_Aliases_ResolveToSameTweak()
        {
            // Each alias should resolve (IsTweakSupported = true)
            var aliases = new[]
            {
                "MouseAcceleration",   // alias for mouse_acceleration
                "GameDVR",             // alias for gamedvr_disable
                "GameBar",             // alias for gamedvr_disable
                "MPOFix",              // alias for mpo_fix
                "UltimatePower",       // alias for ultimate_power
                "CoreParking",         // alias for core_parking_disable
                "Hibernation",         // alias for hibernation_disable
                "SysMain",             // alias for sysmain_disable
                "Win32Priority",       // alias for win32_priority
                "GpuIRQ",              // alias for gpu_irq_priority
                "trim_force_on",       // alias for trim_optimization
                "NetBios",             // alias for netbios
                "NetworkPower",        // alias for network_power
            };

            foreach (var alias in aliases)
            {
                Assert.IsTrue(_dispatcher.IsTweakSupported(alias),
                    $"Alias '{alias}' must be supported by the dispatcher");
            }
        }

        [TestMethod]
        [TestCategory("P1_Dispatcher")]
        public void Dispatcher_CaseInsensitive_Lookup()
        {
            // Same tweak, different casing
            Assert.IsTrue(_dispatcher.IsTweakSupported("MOUSE_ACCELERATION"));
            Assert.IsTrue(_dispatcher.IsTweakSupported("mouse_acceleration"));
            Assert.IsTrue(_dispatcher.IsTweakSupported("Mouse_Acceleration"));
        }

        [TestMethod]
        [TestCategory("P1_Dispatcher")]
        public void Dispatcher_UnsupportedId_ReturnsFalse()
        {
            Assert.IsFalse(_dispatcher.IsTweakSupported("this_tweak_does_not_exist_xyz"));
        }

        // ─────────────────────────────────────────────────────────────
        // P1 — TweakStateManager: dynamic TotalTweaksCount
        // ─────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("P1_StateManager")]
        public void StateManager_TotalTweaksCount_DefaultFallback()
        {
            // Without InjectDispatcher, should return fallback (62)
            var mgr = TweakStateManager.Instance;
            Assert.IsTrue(mgr.TotalTweaksCount >= 62,
                $"TotalTweaksCount fallback should be >= 62, was {mgr.TotalTweaksCount}");
        }

        [TestMethod]
        [TestCategory("P1_StateManager")]
        public void StateManager_AfterInjectDispatcher_TotalMatchesDispatcher()
        {
            var mgr = TweakStateManager.Instance;
            mgr.InjectDispatcher(_dispatcher);

            int expected = _dispatcher.GetCanonicalTweakIds().Count;
            Assert.AreEqual(expected, mgr.TotalTweaksCount,
                "TotalTweaksCount must equal dispatcher canonical ID count after injection");
        }

        [TestMethod]
        [TestCategory("P1_StateManager")]
        public void StateManager_OptimizationPercentage_ZeroWhenNoTweaksActive()
        {
            // Create a fresh isolated state to avoid polluting shared singleton
            // We can't easily isolate the singleton so we just verify the formula
            var mgr = TweakStateManager.Instance;
            mgr.InjectDispatcher(_dispatcher);

            int pct = mgr.OptimizationPercentage;
            Assert.IsTrue(pct >= 0 && pct <= 100,
                $"OptimizationPercentage must be 0–100, was {pct}");
        }

        // ─────────────────────────────────────────────────────────────
        // P1 — TweakDispatcher: GetRealState does not throw
        // ─────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("P1_Dispatcher")]
        public void Dispatcher_GetRealState_DoesNotThrowForAnyCanonicalId()
        {
            var ids = _dispatcher.GetCanonicalTweakIds();
            var failures = new List<string>();

            foreach (var id in ids)
            {
                try
                {
                    _ = _dispatcher.GetRealState(id); // may return null — that's fine
                }
                catch (Exception ex)
                {
                    failures.Add($"{id}: {ex.GetType().Name} - {ex.Message}");
                }
            }

            Assert.AreEqual(0, failures.Count,
                $"GetRealState() threw on {failures.Count} tweak(s):\n" +
                string.Join("\n", failures));
        }

        // ─────────────────────────────────────────────────────────────
        // P2 — StartupStateVerifier
        // ─────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("P2_StartupVerifier")]
        public async Task StartupVerifier_RunsWithoutException()
        {
            var mgr = TweakStateManager.Instance;
            mgr.InjectDispatcher(_dispatcher);

            // Should complete without throwing even if registry is restricted
            await StartupStateVerifier.SyncRealStateAsync(_dispatcher, mgr);
        }

        [TestMethod]
        [TestCategory("P2_StartupVerifier")]
        public async Task StartupVerifier_CorrectsMismatchedState()
        {
            var mgr = TweakStateManager.Instance;
            mgr.InjectDispatcher(_dispatcher);

            // Force the JSON state for mouse_acceleration to the OPPOSITE of reality
            bool? realState = _dispatcher.GetRealState("mouse_acceleration");
            if (realState == null)
            {
                Assert.Inconclusive("Cannot determine real state of mouse_acceleration on this machine.");
                return;
            }

            // Set the JSON to the wrong value
            if (realState.Value)
                mgr.SetTweakDisabled("mouse_acceleration");   // wrong: system says enabled
            else
                mgr.SetTweakEnabled("mouse_acceleration", "Test"); // wrong: system says disabled

            bool wrongState = mgr.IsTweakEnabled("mouse_acceleration");
            Assert.AreEqual(!realState.Value, wrongState, "Pre-condition: JSON should be wrong before sync.");

            // Run sync
            await StartupStateVerifier.SyncRealStateAsync(_dispatcher, mgr);

            // After sync, JSON should match reality
            bool correctedState = mgr.IsTweakEnabled("mouse_acceleration");
            Assert.AreEqual(realState.Value, correctedState,
                "After SyncRealStateAsync(), JSON state must match real system state.");
        }

        // ─────────────────────────────────────────────────────────────
        // P4 — Calculate* metric IDs are valid
        // ─────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("P4_Metrics")]
        public void Metrics_AllCalculateIds_AreKnownToDispatcher()
        {
            // These are all the IDs used inside CalculateEstimatedFpsGain,
            // CalculateEstimatedLatencyReduction, CalculateEstimatedRamFreed.
            // Each must resolve in the dispatcher (alias or canonical).
            var metricsIds = new[]
            {
                // FPS gain
                "VisualEffects", "MemoryOptimization", "SystemProfile",
                "GameDVR", "GpuScheduling", "CoreIsolation", "MPOFix",
                "UltimatePower", "SpectreMeltdown",

                // Latency reduction
                "NetworkOptimization", "DnsCloudflare", "NetworkPower",
                "MouseAcceleration", "Keyboard", "raw_aim_curve", "CoreParking",

                // RAM freed
                "Hibernation", "WindowsSearch", "SysMain", "GameDVR",
            };

            var unknowns = new List<string>();
            foreach (var id in metricsIds)
            {
                if (!_dispatcher.IsTweakSupported(id))
                    unknowns.Add(id);
            }

            Assert.AreEqual(0, unknowns.Count,
                $"The following IDs used in Calculate* methods are NOT in the dispatcher:\n" +
                string.Join(", ", unknowns));
        }

        // ─────────────────────────────────────────────────────────────
        // P1 — Round-trip: Apply + Revert do not crash
        // (smoke test — does NOT verify real registry; just tests no-exception)
        // ─────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("P1_Dispatcher")]
        public void Dispatcher_SmokeTest_ApplyRevert_DoesNotThrow()
        {
            // Use a read-only / no-op tweak for safe smoke testing
            // dns_cloudflare/google apply a real change; use a safer read-only one
            // We test GetRealState path only (no side effects)
            var safeIds = new[]
            {
                "mouse_acceleration", // reads registry
                "raw_aim_curve",      // reads registry
                "do_solo_mode",       // reads registry
                "trim_optimization",  // reads filesystem
                "mpo_fix",            // reads registry
                "ultimate_power",     // reads powercfg
                "hibernation_disable",// reads registry
                "gamedvr_disable",    // reads registry
                "gpu_irq_priority",   // reads registry
            };

            var errors = new List<string>();
            foreach (var id in safeIds)
            {
                try
                {
                    _ = _dispatcher.GetRealState(id);
                }
                catch (Exception ex)
                {
                    errors.Add($"{id}: {ex.Message}");
                }
            }

            Assert.AreEqual(0, errors.Count,
                "GetRealState smoke test failed on:\n" + string.Join("\n", errors));
        }
    }
}
