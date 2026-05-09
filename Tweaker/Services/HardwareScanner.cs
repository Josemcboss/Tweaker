using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading.Tasks;
using Tweaker.Models;

namespace Tweaker.Services
{
    /// <summary>
    /// HardwareScanner - High-performance WMI-based hardware telemetry service.
    /// </summary>
    public sealed class HardwareScanner
    {
        private static readonly Lazy<HardwareScanner> _instance = new(() => new HardwareScanner());
        public static HardwareScanner Instance => _instance.Value;

        public HardwareInfo CurrentInfo { get; private set; } = new();

        private HardwareScanner() { }

        /// <summary>
        /// Performs a full hardware scan asynchronously using WMI queries.
        /// </summary>
        public async Task<HardwareInfo> ScanAsync()
        {
            return await Task.Run(() =>
            {
                var info = new HardwareInfo();

                try
                {
                    ScanCpu(info);
                    ScanGpu(info);
                    ScanRam(info);
                    ScanStorage(info);
                    ScanOS(info);
                    ScanAntiCheat(info);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"⚠️ Hardware scan partially failed: {ex.Message}");
                }

                CurrentInfo = info;
                return info;
            });
        }

        private void ScanAntiCheat(HardwareInfo info)
        {
            try
            {
                info.DetectedAntiCheats = AntiCheatScanner.DetectRunningAntiCheats();
            }
            catch { }
        }

        private void ScanCpu(HardwareInfo info)
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT Name, Manufacturer, NumberOfCores, NumberOfLogicalProcessors, MaxClockSpeed FROM Win32_Processor");
                foreach (ManagementObject obj in searcher.Get())
                {
                    info.CPUName = obj["Name"]?.ToString()?.Trim() ?? "Unknown CPU";
                    info.CPUVendor = obj["Manufacturer"]?.ToString() ?? "Unknown";
                    info.CPUCores = Convert.ToInt32(obj["NumberOfCores"] ?? 0);
                    info.CPUThreads = Convert.ToInt32(obj["NumberOfLogicalProcessors"] ?? 0);
                    info.CPUClockSpeed = Math.Round(Convert.ToDouble(obj["MaxClockSpeed"] ?? 0) / 1000.0, 1);

                    info.IsRyzen = info.CPUName.Contains("Ryzen", StringComparison.OrdinalIgnoreCase);
                    info.IsIntel = info.CPUVendor.Contains("Intel", StringComparison.OrdinalIgnoreCase);
                    break;
                }
            }
            catch { }
        }

        private void ScanGpu(HardwareInfo info)
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT Name, AdapterRAM FROM Win32_VideoController");
                foreach (ManagementObject obj in searcher.Get())
                {
                    string name = obj["Name"]?.ToString() ?? "Unknown GPU";
                    if (name.Contains("Virtual", StringComparison.OrdinalIgnoreCase)) continue;

                    info.GPUName = name;
                    info.GPUVRAM = Math.Round(Convert.ToDouble(obj["AdapterRAM"] ?? 0) / 1024.0 / 1024.0 / 1024.0, 1);
                    
                    // WMI AdapterRAM can be negative or weird for modern GPUs with >4GB, but we'll take what we can get
                    if (info.GPUVRAM < 0) info.GPUVRAM = 0; 

                    info.IsNvidia = name.Contains("NVIDIA", StringComparison.OrdinalIgnoreCase);
                    info.IsAMD = name.Contains("AMD", StringComparison.OrdinalIgnoreCase) || name.Contains("Radeon", StringComparison.OrdinalIgnoreCase);
                    info.GPUVendor = info.IsNvidia ? "NVIDIA" : (info.IsAMD ? "AMD" : "Other");
                    break;
                }
            }
            catch { }
        }

        private void ScanRam(HardwareInfo info)
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT Capacity FROM Win32_PhysicalMemory");
                long totalCapacity = 0;
                int sticks = 0;
                foreach (ManagementObject obj in searcher.Get())
                {
                    totalCapacity += Convert.ToInt64(obj["Capacity"] ?? 0);
                    sticks++;
                }
                info.TotalRAMGB = Math.Round(totalCapacity / 1024.0 / 1024.0 / 1024.0, 0);
                info.RAMSticks = sticks;
            }
            catch { }
        }

        private void ScanStorage(HardwareInfo info)
        {
            try
            {
                // Note: Win32_DiskDrive is safer than MSFT_PhysicalDisk for non-admin/generic WMI
                using var searcher = new ManagementObjectSearcher("SELECT Model, Size, MediaType FROM Win32_DiskDrive");
                foreach (ManagementObject obj in searcher.Get())
                {
                    string model = obj["Model"]?.ToString() ?? "";
                    string mediaType = obj["MediaType"]?.ToString() ?? "";
                    
                    info.StorageCapacity = Math.Round(Convert.ToDouble(obj["Size"] ?? 0) / 1024.0 / 1024.0 / 1024.0, 0);
                    
                    info.IsSSD = model.Contains("SSD", StringComparison.OrdinalIgnoreCase) || 
                                 model.Contains("NVMe", StringComparison.OrdinalIgnoreCase) ||
                                 mediaType.Contains("Fixed hard disk media", StringComparison.OrdinalIgnoreCase); // Modern Windows usually reports this for SSDs too
                    
                    info.StorageType = model.Contains("NVMe", StringComparison.OrdinalIgnoreCase) ? "NVMe SSD" : (info.IsSSD ? "SSD" : "HDD");
                    break; // Just get primary for now
                }
            }
            catch { }
        }

        private void ScanOS(HardwareInfo info)
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT Caption, BuildNumber FROM Win32_OperatingSystem");
                foreach (ManagementObject obj in searcher.Get())
                {
                    info.WindowsVersion = obj["Caption"]?.ToString() ?? "Windows";
                    info.OSBuild = obj["BuildNumber"]?.ToString() ?? "Unknown";
                    info.IsWindows11 = info.WindowsVersion.Contains("11", StringComparison.OrdinalIgnoreCase);
                    break;
                }
            }
            catch { }
        }
    }
}
