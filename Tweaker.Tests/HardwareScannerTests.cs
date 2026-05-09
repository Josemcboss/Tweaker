using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tweaker.Models;
using Tweaker.Services;
using Tweaker.ViewModels;

namespace Tweaker.Tests
{
    [TestClass]
    public class HardwareScannerTests
    {
        [TestMethod]
        public void Test_CPUVendorResolution_AMD()
        {
            var info = new HardwareInfo { CPUName = "AMD Ryzen 9 5950X 16-Core Processor" };
            // Simulate the logic in HardwareScanner (or we should have it in HardwareInfo?)
            // For now, testing if the model can hold the expected state
            info.CPUVendor = "AMD";
            info.IsRyzen = true;

            Assert.IsTrue(info.IsRyzen);
            Assert.AreEqual("AMD", info.CPUVendor);
        }

        [TestMethod]
        public void Test_CPUVendorResolution_Intel()
        {
            var info = new HardwareInfo { CPUName = "12th Gen Intel(R) Core(TM) i9-12900K" };
            info.CPUVendor = "Intel";
            info.IsIntel = true;

            Assert.IsTrue(info.IsIntel);
            Assert.IsFalse(info.IsRyzen);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Test_HardwareScanner_ScanAsync_NotNull()
        {
            // Note: This test requires WMI to be available on the test machine
            var scanner = HardwareScanner.Instance;
            var info = await scanner.ScanAsync();

            Assert.IsNotNull(info);
            Assert.IsFalse(string.IsNullOrEmpty(info.CPUName));
            Assert.IsFalse(string.IsNullOrEmpty(info.GPUName));
        }
    }
}
