using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Management;

namespace Tweaker.Utilities;

public sealed class HardwareInfo
{
    public string GPUName { get; set; } = string.Empty;
    public double GPUVramGB { get; set; }
    public bool IsNvidia { get; set; }
    public bool IsAMD { get; set; }
    public bool IsSSD { get; set; }
    public long TotalRAMGB { get; set; }
    public uint RAMSpeed { get; set; }
    public string RAMType { get; set; } = "DDR4";
    public int CPUCores { get; set; }
    public int CPULogicalProcessors { get; set; }
    public uint CPUMaxSpeed { get; set; }
    public string WindowsVersion { get; set; } = string.Empty;
    public bool IsWindows11 { get; set; }
    public string CPUName { get; set; } = string.Empty;
    public bool IsRyzen { get; set; }
    public bool IsIntel { get; set; }
    public string MotherboardManufacturer { get; set; } = string.Empty;
    public string MotherboardModel { get; set; } = string.Empty;
    public string BiosVersion { get; set; } = string.Empty;
    public List<string> DetectedAntiCheats { get; set; } = new();
    public List<string> StorageDisks { get; set; } = new();
    public List<string> NetworkAdapters { get; set; } = new();
}

public static class HardwareDetector
{
    public static HardwareInfo Detect()
    {
        var info = new HardwareInfo
        {
            TotalRAMGB = DetectTotalRamGb(),
            CPUCores = Environment.ProcessorCount,
            CPULogicalProcessors = Environment.ProcessorCount,
            WindowsVersion = DetectWindowsVersion(),
            IsWindows11 = IsWindows11(),
            DetectedAntiCheats = AntiCheatDetector.DetectRunningAntiCheats()
        };

        DetectGpu(info);
        DetectCpu(info);
        DetectStorage(info);
        DetectMotherboard(info);
        DetectRamDetails(info);
        DetectNetwork(info);

        return info;
    }

    public static bool HasNvidiaGpu() => Detect().IsNvidia;

    public static bool HasSsd() => Detect().IsSSD;

    private static void DetectGpu(HardwareInfo info)
    {
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT Name, AdapterRAM FROM Win32_VideoController");
            foreach (ManagementObject gpu in searcher.Get())
            {
                string name = gpu["Name"]?.ToString() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }

                info.GPUName = name;
                info.IsNvidia |= name.Contains("NVIDIA", StringComparison.OrdinalIgnoreCase) || name.Contains("GeForce", StringComparison.OrdinalIgnoreCase);
                info.IsAMD |= name.Contains("AMD", StringComparison.OrdinalIgnoreCase) || name.Contains("Radeon", StringComparison.OrdinalIgnoreCase);

                if (gpu["AdapterRAM"] != null)
                {
                    try
                    {
                        ulong vramBytes = Convert.ToUInt64(gpu["AdapterRAM"]);
                        info.GPUVramGB = Math.Round((double)vramBytes / (1024.0 * 1024.0 * 1024.0), 1);
                    }
                    catch { }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"⚠️ GPU detection failed: {ex.Message}");
        }
    }

    private static void DetectCpu(HardwareInfo info)
    {
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT Name, NumberOfCores, NumberOfLogicalProcessors, MaxClockSpeed FROM Win32_Processor");
            foreach (ManagementObject cpu in searcher.Get())
            {
                string name = cpu["Name"]?.ToString() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }

                info.CPUName = name.Trim();
                info.IsRyzen |= name.Contains("Ryzen", StringComparison.OrdinalIgnoreCase) || name.Contains("AMD", StringComparison.OrdinalIgnoreCase);
                info.IsIntel |= name.Contains("Intel", StringComparison.OrdinalIgnoreCase);

                if (cpu["NumberOfCores"] is uint coreCount)
                {
                    info.CPUCores = (int)coreCount;
                }
                if (cpu["NumberOfLogicalProcessors"] is uint logicalCount)
                {
                    info.CPULogicalProcessors = (int)logicalCount;
                }
                if (cpu["MaxClockSpeed"] is uint maxSpeed)
                {
                    info.CPUMaxSpeed = maxSpeed;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"⚠️ CPU detection failed: {ex.Message}");
        }
    }

    private static void DetectStorage(HardwareInfo info)
    {
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT MediaType, Model, Size FROM Win32_DiskDrive");
            foreach (ManagementObject disk in searcher.Get())
            {
                string mediaType = disk["MediaType"]?.ToString() ?? string.Empty;
                string model = disk["Model"]?.ToString() ?? string.Empty;

                bool isSsd = mediaType.Contains("SSD", StringComparison.OrdinalIgnoreCase)
                    || mediaType.Contains("Solid", StringComparison.OrdinalIgnoreCase)
                    || model.Contains("SSD", StringComparison.OrdinalIgnoreCase)
                    || model.Contains("NVMe", StringComparison.OrdinalIgnoreCase);

                if (isSsd)
                {
                    info.IsSSD = true;
                }

                try
                {
                    ulong sizeBytes = disk["Size"] != null ? Convert.ToUInt64(disk["Size"]) : 0;
                    double sizeGB = Math.Round((double)sizeBytes / (1024.0 * 1024.0 * 1024.0), 0);
                    info.StorageDisks.Add($"{model} ({(isSsd ? "SSD" : "HDD")} - {sizeGB} GB)");
                }
                catch { }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"⚠️ Storage detection failed: {ex.Message}");
        }
    }

    private static void DetectMotherboard(HardwareInfo info)
    {
        try
        {
            using var searcher1 = new ManagementObjectSearcher("SELECT Manufacturer, Product FROM Win32_BaseBoard");
            foreach (ManagementObject board in searcher1.Get())
            {
                info.MotherboardManufacturer = board["Manufacturer"]?.ToString()?.Trim() ?? string.Empty;
                info.MotherboardModel = board["Product"]?.ToString()?.Trim() ?? string.Empty;
            }

            using var searcher2 = new ManagementObjectSearcher("SELECT Version FROM Win32_BIOS");
            foreach (ManagementObject bios in searcher2.Get())
            {
                info.BiosVersion = bios["Version"]?.ToString()?.Trim() ?? string.Empty;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"⚠️ Motherboard detection failed: {ex.Message}");
        }
    }

    private static void DetectRamDetails(HardwareInfo info)
    {
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT Speed, SMBIOSMemoryType FROM Win32_PhysicalMemory");
            foreach (ManagementObject mem in searcher.Get())
            {
                try
                {
                    if (mem["Speed"] != null)
                    {
                        uint speed = Convert.ToUInt32(mem["Speed"]);
                        if (speed > 0) info.RAMSpeed = speed;
                    }
                }
                catch { }

                try
                {
                    if (mem["SMBIOSMemoryType"] != null)
                    {
                        ushort typeVal = Convert.ToUInt16(mem["SMBIOSMemoryType"]);
                        info.RAMType = typeVal switch
                        {
                            20 => "DDR",
                            21 => "DDR2",
                            24 => "DDR3",
                            26 => "DDR4",
                            34 => "DDR5",
                            _ => "DDR"
                        };
                    }
                }
                catch { }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"⚠️ RAM details detection failed: {ex.Message}");
        }
    }

    private static void DetectNetwork(HardwareInfo info)
    {
        try
        {
            foreach (var ni in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up && 
                    ni.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Loopback)
                {
                    try
                    {
                        double speedGb = Math.Round((double)ni.Speed / 1_000_000_000.0, 1);
                        string speedText = speedGb >= 0.1 ? $"{speedGb} Gbps" : $"{ni.Speed / 1_000_000.0} Mbps";
                        info.NetworkAdapters.Add($"{ni.Name} ({speedText})");
                    }
                    catch { }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"⚠️ Network detection failed: {ex.Message}");
        }
    }

    private static long DetectTotalRamGb()
    {
        try
        {
            ulong totalBytes = 0;
            using var searcher = new ManagementObjectSearcher("SELECT Capacity FROM Win32_PhysicalMemory");
            foreach (ManagementObject memory in searcher.Get())
            {
                if (memory["Capacity"] == null)
                {
                    continue;
                }

                totalBytes += Convert.ToUInt64(memory["Capacity"]);
            }

            return totalBytes > 0 ? (long)(totalBytes / 1024 / 1024 / 1024) : 0;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"⚠️ RAM detection failed: {ex.Message}");
            return 0;
        }
    }

    private static string DetectWindowsVersion()
    {
        try
        {
            using var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            string productName = key?.GetValue("ProductName")?.ToString() ?? Environment.OSVersion.VersionString;
            string build = key?.GetValue("CurrentBuildNumber")?.ToString() ?? string.Empty;
            return string.IsNullOrWhiteSpace(build) ? productName : $"{productName} (Build {build})";
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"⚠️ Windows version detection failed: {ex.Message}");
            return Environment.OSVersion.VersionString;
        }
    }

    private static bool IsWindows11()
    {
        try
        {
            using var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            string productName = key?.GetValue("ProductName")?.ToString() ?? string.Empty;
            if (productName.Contains("Windows 11", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            string buildText = key?.GetValue("CurrentBuildNumber")?.ToString() ?? "0";
            return int.TryParse(buildText, NumberStyles.Integer, CultureInfo.InvariantCulture, out int build) && build >= 22000;
        }
        catch
        {
            return false;
        }
    }
}

public static class AntiCheatDetector
{
    private static readonly Dictionary<string, string> AntiCheats = new(StringComparer.OrdinalIgnoreCase)
    {
        ["vgc"] = "Valorant (Vanguard)",
        ["EasyAntiCheat"] = "EAC (Fortnite/Apex/Rust)",
        ["BEService"] = "BattlEye (PUBG/R6/DayZ)",
        ["ESEA"] = "ESEA Client",
        ["faceit"] = "FACEIT AC"
    };

    public static List<string> DetectRunningAntiCheats()
    {
        var detected = new List<string>();

        try
        {
            foreach (var process in Process.GetProcesses())
            {
                using (process)
                {
                    foreach (var entry in AntiCheats)
                    {
                        if (process.ProcessName.Contains(entry.Key, StringComparison.OrdinalIgnoreCase))
                        {
                            detected.Add(entry.Value);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"⚠️ AntiCheat detection failed: {ex.Message}");
        }

        return detected.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }

    public static bool IsRunning()
    {
        return DetectRunningAntiCheats().Count > 0;
    }
}
