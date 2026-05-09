using System;
using System.Collections.Generic;

namespace Tweaker.Models
{
    public sealed class HardwareInfo
    {
        // CPU
        public string CPUName { get; set; } = string.Empty;
        public string CPUVendor { get; set; } = string.Empty;
        public int CPUCores { get; set; }
        public int CPUThreads { get; set; }
        public double CPUClockSpeed { get; set; } // GHz
        public bool IsRyzen { get; set; }
        public bool IsIntel { get; set; }

        // GPU
        public string GPUName { get; set; } = string.Empty;
        public string GPUVendor { get; set; } = string.Empty;
        public double GPUVRAM { get; set; } // GB
        public bool IsNvidia { get; set; }
        public bool IsAMD { get; set; }

        // RAM
        public double TotalRAMGB { get; set; }
        public int RAMSticks { get; set; }

        // Storage
        public string StorageType { get; set; } = string.Empty; // SSD/HDD
        public double StorageCapacity { get; set; } // GB
        public bool IsSSD { get; set; }

        // OS
        public string WindowsVersion { get; set; } = string.Empty;
        public string OSBuild { get; set; } = string.Empty;
        public bool IsWindows11 { get; set; }

        // Misc
        public List<string> DetectedAntiCheats { get; set; } = new();

        // Compatibility properties for older logic
        public string GPUNameShort => GPUName;
        public string CPUNameShort => CPUName;
    }
}
