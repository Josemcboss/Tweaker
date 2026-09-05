using System;
using System.Diagnostics;
using Microsoft.Win32;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// GameBar & Background DVR Mitigation Tweaks
    /// Deshabilita GameBarPresenceWriter y la captura fantasma en segundo plano
    /// que genera input delay y microstuttering en fullscreen y borderless.
    /// </summary>
    public static class GameBarMitigationTweaks
    {
        private const string IFEO_GAMEBAR = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\GameBarPresenceWriter.exe";
        private const string GAME_CONFIG_STORE = @"System\GameConfigStore";
        private const string GAME_DVR_USER = @"Software\Microsoft\Windows\CurrentVersion\GameDVR";
        private const string GAME_DVR_POLICY = @"SOFTWARE\Policies\Microsoft\Windows\GameDVR";

        /// <summary>
        /// Desactiva GameBarPresenceWriter y el DVR fantasma para máxima fluidez y menor latencia
        /// </summary>
        public static bool DisableGameBarHooks()
        {
            try
            {
                Debug.WriteLine("──────────────────────────────────────────");
                Debug.WriteLine("⚡ DESACTIVANDO GAMEBAR PRESENCE WRITER & BACKGROUND DVR");
                Debug.WriteLine("──────────────────────────────────────────");

                // 1. Desactivar en GameConfigStore (FSE Behavior)
                using (var key = Registry.CurrentUser.CreateSubKey(GAME_CONFIG_STORE, true))
                {
                    if (key != null)
                    {
                        key.SetValue("GameDVR_Enabled", 0, RegistryValueKind.DWord);
                        key.SetValue("GameDVR_FSEBehaviorMode", 2, RegistryValueKind.DWord);
                        key.SetValue("GameDVR_HonorUserFSEBehaviorMode", 1, RegistryValueKind.DWord);
                        key.SetValue("GameDVR_DXGIHonorFSEWindowsCompatible", 1, RegistryValueKind.DWord);
                        key.SetValue("GameDVR_EFSEFeatureFlags", 0, RegistryValueKind.DWord);
                        Debug.WriteLine("  ✔ GameConfigStore optimizado para bypass de GameDVR");
                    }
                }

                // 2. Desactivar capturas en HKCU GameDVR
                using (var key = Registry.CurrentUser.CreateSubKey(GAME_DVR_USER, true))
                {
                    if (key != null)
                    {
                        key.SetValue("AppCaptureEnabled", 0, RegistryValueKind.DWord);
                        key.SetValue("HistoricalCaptureEnabled", 0, RegistryValueKind.DWord);
                        key.SetValue("AudioCaptureEnabled", 0, RegistryValueKind.DWord);
                        Debug.WriteLine("  ✔ Capturas en segundo plano deshabilitadas en HKCU");
                    }
                }

                // 3. Política de sistema HKLM GameDVR
                using (var key = Registry.LocalMachine.CreateSubKey(GAME_DVR_POLICY, true))
                {
                    if (key != null)
                    {
                        key.SetValue("AllowGameDVR", 0, RegistryValueKind.DWord);
                        Debug.WriteLine("  ✔ Política AllowGameDVR establecida en 0");
                    }
                }

                // 4. Bloquear ejecutable en IFEO (Previene ejecución de GameBarPresenceWriter)
                using (var key = Registry.LocalMachine.CreateSubKey(IFEO_GAMEBAR, true))
                {
                    if (key != null)
                    {
                        key.SetValue("Debugger", "systray.exe", RegistryValueKind.String);
                        Debug.WriteLine("  ✔ GameBarPresenceWriter.exe bloqueado vía IFEO");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error en DisableGameBarHooks: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Restaura la configuración predeterminada de GameBar y DVR
        /// </summary>
        public static bool RestoreGameBarHooks()
        {
            try
            {
                Debug.WriteLine("Restaurando configuración de GameBar...");

                using (var key = Registry.CurrentUser.CreateSubKey(GAME_CONFIG_STORE, true))
                {
                    if (key != null)
                    {
                        key.SetValue("GameDVR_Enabled", 1, RegistryValueKind.DWord);
                        key.DeleteValue("GameDVR_FSEBehaviorMode", false);
                        key.DeleteValue("GameDVR_HonorUserFSEBehaviorMode", false);
                    }
                }

                using (var key = Registry.CurrentUser.CreateSubKey(GAME_DVR_USER, true))
                {
                    if (key != null)
                    {
                        key.SetValue("AppCaptureEnabled", 1, RegistryValueKind.DWord);
                    }
                }

                using (var key = Registry.LocalMachine.CreateSubKey(GAME_DVR_POLICY, true))
                {
                    if (key != null)
                    {
                        key.DeleteValue("AllowGameDVR", false);
                    }
                }

                try
                {
                    Registry.LocalMachine.DeleteSubKeyTree(IFEO_GAMEBAR, false);
                }
                catch { }

                Debug.WriteLine("✔ Configuración de GameBar restaurada");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error en RestoreGameBarHooks: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Comprueba si GameBar Hooks están mitigados
        /// </summary>
        public static bool? IsGameBarMitigated()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(GAME_CONFIG_STORE, false);
                var val = key?.GetValue("GameDVR_Enabled");
                if (val is int intVal && intVal == 0) return true;
                return false;
            }
            catch
            {
                return null;
            }
        }
    }
}
