using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Tweaker.Utilities;

public static class TweakHistoryLogger
{
    private static readonly string HistoryFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "GhostOptimizer",
        "history.log");

    public static void Log(string action, string tweakName, string category)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(HistoryFilePath)!);
            string line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{action}] {tweakName} - {category}";
            File.AppendAllText(HistoryFilePath, line + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"⚠️ History log failed: {ex.Message}");
        }
    }

    public static List<string> GetRecent(int count = 50)
    {
        try
        {
            if (!File.Exists(HistoryFilePath))
            {
                return new List<string>();
            }

            return File.ReadAllLines(HistoryFilePath)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .TakeLast(count)
                .ToList();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"⚠️ History read failed: {ex.Message}");
            return new List<string>();
        }
    }

    public static void Clear()
    {
        try
        {
            if (File.Exists(HistoryFilePath))
            {
                File.WriteAllText(HistoryFilePath, string.Empty);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"⚠️ History clear failed: {ex.Message}");
        }
    }

    public static string ExportToDesktop()
    {
        try
        {
            if (!File.Exists(HistoryFilePath))
            {
                return string.Empty;
            }

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string exportPath = Path.Combine(desktopPath, $"GhostOptimizer_history_{DateTime.Now:yyyyMMdd_HHmmss}.log");
            File.Copy(HistoryFilePath, exportPath, true);
            return exportPath;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"⚠️ History export failed: {ex.Message}");
            return string.Empty;
        }
    }
}
