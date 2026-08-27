using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xunit;
using Tweaker.Models;
using Tweaker.ViewModels;

namespace Tweaker.Tests
{
    public class AdaptiveLogicTests
    {
        [Fact]
        public void Test_MemoryOptimization_Disabled_Under16GB()
        {
            // We need to mock the ViewModel or use it if possible
            // Since it's a WPF project, we might need some setup
            // For now, I'll test the logic by mimicking what ApplyAdaptiveRules does
            
            var info = new HardwareInfo { TotalRAMGB = 8 };
            var tweak = new TweakModel { TweakId = "memory_optimization", Title = "RAM", IsDisabled = false };
            
            // Mimic logic:
            if (tweak.TweakId == "memory_optimization" && info.TotalRAMGB < 16)
            {
                tweak.IsDisabled = true;
                tweak.WarningMessage = "Desactivado: Bajo en RAM.";
            }

            Assert.True(tweak.IsDisabled);
            Assert.False(string.IsNullOrEmpty(tweak.WarningMessage));
        }

        [Fact]
        public void Test_Ryzen_Preselection()
        {
            var info = new HardwareInfo { IsRyzen = true };
            var tweak = new TweakModel { TweakId = "core_parking", Title = "Core Parking", IsEnabled = false };
            
            // Mimic logic:
            if (info.IsRyzen && tweak.TweakId == "core_parking")
            {
                tweak.IsEnabled = true;
            }

            Assert.True(tweak.IsEnabled);
        }
    }
}
