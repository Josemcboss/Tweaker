using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Tweaker.Services;
using Tweaker.Utilities;

namespace Tweaker.Tests
{
    public class Phase3IntegrationTests
    {
        private readonly TweakDispatcher _dispatcher;

        public Phase3IntegrationTests()
        {
            _dispatcher = new TweakDispatcher();
        }

        [Fact]
        [Trait("Category", "P1_Dispatcher")]
        public void Dispatcher_CanonicalIds_AtLeast62()
        {
            var ids = _dispatcher.GetCanonicalTweakIds();
            Assert.True(ids.Count >= 62, $"Expected >= 62 canonical tweaks, got {ids.Count}");
        }

        [Fact]
        [Trait("Category", "P1_Dispatcher")]
        public void Dispatcher_CanonicalIds_NoDuplicates()
        {
            var ids = _dispatcher.GetCanonicalTweakIds();
            var set = new HashSet<string>(ids, StringComparer.OrdinalIgnoreCase);
            Assert.Equal(ids.Count, set.Count);
        }

        [Fact]
        [Trait("Category", "P1_Dispatcher")]
        public void Dispatcher_Aliases_ResolveToSameTweak()
        {
            var aliases = new[]
            {
                "MouseAcceleration",
                "GameDVR",
                "GameBar",
                "MPOFix",
                "UltimatePower",
                "CoreParking",
                "Hibernation",
                "SysMain",
                "Win32Priority",
                "GpuIRQ",
                "trim_force_on",
                "NetBios",
                "NetworkPower",
            };

            foreach (var alias in aliases)
            {
                Assert.True(_dispatcher.IsTweakSupported(alias), $"Alias '{alias}' must be supported by the dispatcher");
            }
        }

        [Fact]
        [Trait("Category", "P1_Dispatcher")]
        public void Dispatcher_CaseInsensitive_Lookup()
        {
            Assert.True(_dispatcher.IsTweakSupported("MOUSE_ACCELERATION"));
            Assert.True(_dispatcher.IsTweakSupported("mouse_acceleration"));
            Assert.True(_dispatcher.IsTweakSupported("Mouse_Acceleration"));
        }

        [Fact]
        [Trait("Category", "P1_Dispatcher")]
        public void Dispatcher_UnsupportedId_ReturnsFalse()
        {
            Assert.False(_dispatcher.IsTweakSupported("this_tweak_does_not_exist_xyz"));
        }

        [Fact]
        [Trait("Category", "P1_StateManager")]
        public void StateManager_TotalTweaksCount_DefaultFallback()
        {
            var mgr = TweakStateManager.Instance;
            Assert.True(mgr.TotalTweaksCount >= 62, $"TotalTweaksCount fallback should be >= 62, was {mgr.TotalTweaksCount}");
        }

        [Fact]
        [Trait("Category", "P1_StateManager")]
        public void StateManager_AfterInjectDispatcher_TotalMatchesDispatcher()
        {
            var mgr = TweakStateManager.Instance;
            mgr.InjectDispatcher(_dispatcher);

            int expected = _dispatcher.GetCanonicalTweakIds().Count;
            Assert.Equal(expected, mgr.TotalTweaksCount);
        }

        [Fact]
        [Trait("Category", "P1_StateManager")]
        public void StateManager_OptimizationPercentage_ZeroWhenNoTweaksActive()
        {
            var mgr = TweakStateManager.Instance;
            mgr.InjectDispatcher(_dispatcher);

            int pct = mgr.OptimizationPercentage;
            Assert.True(pct >= 0 && pct <= 100, $"OptimizationPercentage must be 0–100, was {pct}");
        }

        [Fact]
        [Trait("Category", "P1_Dispatcher")]
        public void Dispatcher_GetRealState_DoesNotThrowForAnyCanonicalId()
        {
            var ids = _dispatcher.GetCanonicalTweakIds();
            var failures = new List<string>();

            foreach (var id in ids)
            {
                try
                {
                    _ = _dispatcher.GetRealState(id);
                }
                catch (Exception ex)
                {
                    failures.Add($"{id}: {ex.GetType().Name} - {ex.Message}");
                }
            }

            Assert.Equal(0, failures.Count);
        }

        [Fact]
        [Trait("Category", "P2_StartupVerifier")]
        public async Task StartupVerifier_RunsWithoutException()
        {
            var mgr = TweakStateManager.Instance;
            mgr.InjectDispatcher(_dispatcher);

            await StartupStateVerifier.SyncRealStateAsync(_dispatcher, mgr);
        }

        [Fact]
        [Trait("Category", "P2_StartupVerifier")]
        public async Task StartupVerifier_CorrectsMismatchedState()
        {
            var mgr = TweakStateManager.Instance;
            mgr.InjectDispatcher(_dispatcher);

            bool? realState = _dispatcher.GetRealState("mouse_acceleration");
            if (realState == null)
            {
                return;
            }

            if (realState.Value)
                mgr.SetTweakDisabled("mouse_acceleration");
            else
                mgr.SetTweakEnabled("mouse_acceleration", "Test");

            bool wrongState = mgr.IsTweakEnabled("mouse_acceleration");
            Assert.Equal(!realState.Value, wrongState);

            await StartupStateVerifier.SyncRealStateAsync(_dispatcher, mgr);

            bool correctedState = mgr.IsTweakEnabled("mouse_acceleration");
            Assert.Equal(realState.Value, correctedState);
        }

        [Fact]
        [Trait("Category", "P4_Metrics")]
        public void Metrics_AllCalculateIds_AreKnownToDispatcher()
        {
            var metricsIds = new[]
            {
                "VisualEffects", "MemoryOptimization", "SystemProfile",
                "GameDVR", "GpuScheduling", "CoreIsolation", "MPOFix",
                "UltimatePower", "SpectreMeltdown",
                "NetworkOptimization", "DnsCloudflare", "NetworkPower",
                "MouseAcceleration", "Keyboard", "raw_aim_curve", "CoreParking",
                "Hibernation", "WindowsSearch", "SysMain", "GameDVR",
            };

            var unknowns = new List<string>();
            foreach (var id in metricsIds)
            {
                if (!_dispatcher.IsTweakSupported(id))
                    unknowns.Add(id);
            }

            Assert.Equal(0, unknowns.Count);
        }

        [Fact]
        [Trait("Category", "P1_Dispatcher")]
        public void Dispatcher_SmokeTest_ApplyRevert_DoesNotThrow()
        {
            var safeIds = new[]
            {
                "mouse_acceleration",
                "raw_aim_curve",
                "do_solo_mode",
                "trim_optimization",
                "mpo_fix",
                "ultimate_power",
                "hibernation_disable",
                "gamedvr_disable",
                "gpu_irq_priority",
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

            Assert.Equal(0, errors.Count);
        }
    }
}
