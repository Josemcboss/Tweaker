using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// AppOptimizationTweaks - Inspirado en Paragon Tweaking Utility (PTU)
    /// Optimización de aplicaciones secundarias (Discord, Spotify, Navegadores) para evitar micro-stutters y caídas de FPS durante el juego.
    /// </summary>
    public static class AppOptimizationTweaks
    {
        #region Discord Optimization

        /// <summary>
        /// Optimiza Discord para gaming:
        /// - Desactiva aceleración de hardware en settings.json para liberar recursos de GPU/NVENC
        /// - Desactiva el overlay in-game causante de micro-stuttering
        /// </summary>
        public static bool OptimizeDiscord()
        {
            try
            {
                Debug.WriteLine("→ Optimizando Discord para Gaming...");
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string discordSettingsPath = Path.Combine(appData, "discord", "settings.json");

                if (File.Exists(discordSettingsPath))
                {
                    string content = File.ReadAllText(discordSettingsPath);
                    // Actualizar o añadir "enable_hwaccel": false
                    if (content.Contains("\"enable_hwaccel\""))
                    {
                        content = Regex.Replace(content, "\"enable_hwaccel\"\\s*:\\s*(true|false)", "\"enable_hwaccel\": false");
                    }
                    else
                    {
                        // Inserción en el JSON
                        content = content.TrimEnd();
                        if (content.EndsWith("}"))
                        {
                            content = content.Substring(0, content.Length - 1).TrimEnd();
                            if (content.Length > 1 && !content.EndsWith(","))
                                content += ",";
                            content += "\n  \"enable_hwaccel\": false\n}";
                        }
                    }
                    File.WriteAllText(discordSettingsPath, content);
                }

                // Desactivar Discord In-Game Overlay hook en registro si existe
                using (RegistryKey? key = Registry.CurrentUser.CreateSubKey(@"Software\Discord"))
                {
                    if (key != null)
                    {
                        key.SetValue("DisableOverlay", 1, RegistryValueKind.DWord);
                    }
                }

                Debug.WriteLine("✓ Discord optimizado para gaming.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error en OptimizeDiscord: {ex.Message}");
                return false;
            }
        }

        public static bool RestoreDiscord()
        {
            try
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string discordSettingsPath = Path.Combine(appData, "discord", "settings.json");

                if (File.Exists(discordSettingsPath))
                {
                    string content = File.ReadAllText(discordSettingsPath);
                    if (content.Contains("\"enable_hwaccel\""))
                    {
                        content = Regex.Replace(content, "\"enable_hwaccel\"\\s*:\\s*(true|false)", "\"enable_hwaccel\": true");
                        File.WriteAllText(discordSettingsPath, content);
                    }
                }

                using (RegistryKey? key = Registry.CurrentUser.CreateSubKey(@"Software\Discord"))
                {
                    if (key != null)
                    {
                        key.DeleteValue("DisableOverlay", false);
                    }
                }

                Debug.WriteLine("✓ Discord restaurado a configuración predeterminada.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error en RestoreDiscord: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Spotify Optimization

        /// <summary>
        /// Desactiva la aceleración por hardware en Spotify para evitar consumo innecesario de GPU
        /// </summary>
        public static bool OptimizeSpotify()
        {
            try
            {
                Debug.WriteLine("→ Optimizando Spotify para Gaming...");
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string spotifyPrefsPath = Path.Combine(appData, "Spotify", "prefs");

                if (File.Exists(spotifyPrefsPath))
                {
                    string content = File.ReadAllText(spotifyPrefsPath);
                    if (content.Contains("app.browser.enable-gpu="))
                    {
                        content = Regex.Replace(content, "app\\.browser\\.enable-gpu=(true|false)", "app.browser.enable-gpu=false");
                    }
                    else
                    {
                        content += "\napp.browser.enable-gpu=false\n";
                    }
                    File.WriteAllText(spotifyPrefsPath, content);
                    Debug.WriteLine("✓ Spotify: Aceleración por hardware desactivada en archivo prefs.");
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error en OptimizeSpotify: {ex.Message}");
                return false;
            }
        }

        public static bool RestoreSpotify()
        {
            try
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string spotifyPrefsPath = Path.Combine(appData, "Spotify", "prefs");

                if (File.Exists(spotifyPrefsPath))
                {
                    string content = File.ReadAllText(spotifyPrefsPath);
                    if (content.Contains("app.browser.enable-gpu="))
                    {
                        content = Regex.Replace(content, "app\\.browser\\.enable-gpu=(true|false)", "app.browser.enable-gpu=true");
                        File.WriteAllText(spotifyPrefsPath, content);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error en RestoreSpotify: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Gamer Browser Background Optimization

        /// <summary>
        /// Desactiva que navegadores (Chrome, Edge, Brave) continúen ejecutando aplicaciones y extensiones en segundo plano al cerrarse
        /// </summary>
        public static bool OptimizeBrowsersGamingMode()
        {
            try
            {
                Debug.WriteLine("→ Configurando políticas de navegadores en segundo plano...");

                // 1. Google Chrome
                using (RegistryKey? key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Google\Chrome"))
                {
                    if (key != null)
                    {
                        key.SetValue("BackgroundModeEnabled", 0, RegistryValueKind.DWord);
                    }
                }

                // 2. Microsoft Edge
                using (RegistryKey? key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Edge"))
                {
                    if (key != null)
                    {
                        key.SetValue("BackgroundModeEnabled", 0, RegistryValueKind.DWord);
                        key.SetValue("EfficiencyModeEnabled", 1, RegistryValueKind.DWord);
                        key.SetValue("StartupBoostEnabled", 0, RegistryValueKind.DWord);
                    }
                }

                // 3. Brave Browser
                using (RegistryKey? key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\BraveSoftware\Brave"))
                {
                    if (key != null)
                    {
                        key.SetValue("BackgroundModeEnabled", 0, RegistryValueKind.DWord);
                    }
                }

                Debug.WriteLine("✓ Navegadores configurados para liberar CPU y RAM al cerrarse.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error en OptimizeBrowsersGamingMode: {ex.Message}");
                return false;
            }
        }

        public static bool RestoreBrowsersGamingMode()
        {
            try
            {
                using (RegistryKey? key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Google\Chrome", true))
                {
                    key?.DeleteValue("BackgroundModeEnabled", false);
                }

                using (RegistryKey? key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Edge", true))
                {
                    if (key != null)
                    {
                        key.DeleteValue("BackgroundModeEnabled", false);
                        key.DeleteValue("EfficiencyModeEnabled", false);
                        key.DeleteValue("StartupBoostEnabled", false);
                    }
                }

                using (RegistryKey? key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\BraveSoftware\Brave", true))
                {
                    key?.DeleteValue("BackgroundModeEnabled", false);
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error en RestoreBrowsersGamingMode: {ex.Message}");
                return false;
            }
        }

        #endregion
    }
}
