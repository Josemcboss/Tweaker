using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Tweaker.Services
{
    public sealed class AntiCheatScanner
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
                    try
                    {
                        foreach (var entry in AntiCheats)
                        {
                            if (process.ProcessName.Contains(entry.Key, StringComparison.OrdinalIgnoreCase))
                            {
                                detected.Add(entry.Value);
                            }
                        }
                    }
                    catch
                    {
                        // Some processes might be protected, skip them
                    }
                    finally
                    {
                        process.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"⚠️ AntiCheat detection failed: {ex.Message}");
            }

            return detected.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        }

        public static bool IsAnyRunning()
        {
            return DetectRunningAntiCheats().Count > 0;
        }
    }
}
