using System;
using System.Windows;
using System.Windows.Media;

namespace Tweaker.Utilities;

public static class ThemeManager
{
    public static void ApplyTheme(string themeName)
    {
        Color accent, secondary, background, sidebarBg, headerBg;
        switch (themeName.ToLower())
        {
            case "matrix":
                accent = Color.FromRgb(0, 255, 65);       // Matrix Green
                secondary = Color.FromRgb(0, 120, 0);     // Dark Green
                background = Color.FromRgb(5, 12, 5);     // Dark Green-Black
                sidebarBg = Color.FromRgb(2, 6, 2);
                headerBg = Color.FromRgb(0, 3, 0);
                break;
            case "nordic":
                accent = Color.FromRgb(136, 192, 208);    // Nord Light Blue
                secondary = Color.FromRgb(94, 129, 172);  // Nord Deep Blue
                background = Color.FromRgb(46, 52, 64);    // Nord Dark Gray
                sidebarBg = Color.FromRgb(36, 41, 51);
                headerBg = Color.FromRgb(26, 30, 38);
                break;
            case "blood":
                accent = Color.FromRgb(255, 59, 48);      // Blood Red
                secondary = Color.FromRgb(138, 0, 0);     // Deep Crimson
                background = Color.FromRgb(15, 10, 10);    // Crimson Black
                sidebarBg = Color.FromRgb(8, 5, 5);
                headerBg = Color.FromRgb(4, 2, 2);
                break;
            case "cyberpunk":
            default:
                accent = Color.FromRgb(0, 217, 255);      // Cyberpunk Cyan
                secondary = Color.FromRgb(169, 112, 255); // Cyberpunk Purple
                background = Color.FromRgb(15, 15, 15);    // Dark Gray-Black
                sidebarBg = Color.FromRgb(20, 20, 20);
                headerBg = Color.FromRgb(10, 10, 10);
                break;
        }

        // Apply dynamically to Application Resources
        Application.Current.Resources["AccentColor"] = accent;
        Application.Current.Resources["SecondaryColor"] = secondary;
        
        var accentBrush = new SolidColorBrush(accent);
        accentBrush.Freeze();
        Application.Current.Resources["AccentBrush"] = accentBrush;

        var secondaryBrush = new SolidColorBrush(secondary);
        secondaryBrush.Freeze();
        Application.Current.Resources["SecondaryBrush"] = secondaryBrush;

        var bgBrush = new SolidColorBrush(background);
        bgBrush.Freeze();
        Application.Current.Resources["ThemeBgBrush"] = bgBrush;

        var sbgBrush = new SolidColorBrush(sidebarBg);
        sbgBrush.Freeze();
        Application.Current.Resources["ThemeSidebarBgBrush"] = sbgBrush;

        var hbgBrush = new SolidColorBrush(headerBg);
        hbgBrush.Freeze();
        Application.Current.Resources["ThemeHeaderBgBrush"] = hbgBrush;

        // Save theme to registry preference
        try
        {
            using var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"Software\GhostOptimizer");
            key?.SetValue("Theme", themeName);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ThemeManager] Error saving theme: {ex.Message}");
        }
    }

    public static string LoadThemePreference()
    {
        try
        {
            using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\GhostOptimizer");
            return key?.GetValue("Theme")?.ToString() ?? "cyberpunk";
        }
        catch
        {
            return "cyberpunk";
        }
    }
}
