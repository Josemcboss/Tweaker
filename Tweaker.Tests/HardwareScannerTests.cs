using System;
using System.Collections.Generic;
using Xunit;
using Tweaker.Models;
using Tweaker.Services;
using Tweaker.ViewModels;

namespace Tweaker.Tests
{
    public class HardwareScannerTests
    {
        [Fact]
        public void Test_CPUVendorResolution_AMD()
        {
            var info = new HardwareInfo { CPUName = "AMD Ryzen 9 5950X 16-Core Processor" };
            info.CPUVendor = "AMD";
            info.IsRyzen = true;

            Assert.True(info.IsRyzen);
            Assert.Equal("AMD", info.CPUVendor);
        }

        [Fact]
        public void Test_CPUVendorResolution_Intel()
        {
            var info = new HardwareInfo { CPUName = "12th Gen Intel(R) Core(TM) i9-12900K" };
            info.CPUVendor = "Intel";
            info.IsIntel = true;

            Assert.True(info.IsIntel);
            Assert.False(info.IsRyzen);
        }

        [Fact]
        public async System.Threading.Tasks.Task Test_HardwareScanner_ScanAsync_NotNull()
        {
            var scanner = HardwareScanner.Instance;
            var info = await scanner.ScanAsync();

            Assert.NotNull(info);
            Assert.False(string.IsNullOrEmpty(info.CPUName));
            Assert.False(string.IsNullOrEmpty(info.GPUName));
        }
    }
}
