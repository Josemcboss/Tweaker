using Microsoft.Win32;
using System;
using System.Diagnostics;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// AdvancedTweaks - Input Lag & Visuals
    /// Teclado, FSO, MPO, HAGS
    /// </summary>
    public static class AdvancedTweaks
    {
        private const string KEYBOARD_KEY = @"Control Panel\Keyboard";
        private const string GAME_CONFIG_STORE = @"System\GameConfigStore";
        private const string DWM_KEY = @"SOFTWARE\Microsoft\Windows\Dwm";
        private const string GRAPHICS_DRIVERS = @"SYSTEM\CurrentControlSet\Control\GraphicsDrivers";

        /// <summary>
        /// OPTIMIZACIÓN DE TECLADO
        /// Reduce delay de repetición a 0
        /// </summary>
        public static bool OptimizeKeyboard()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(KEYBOARD_KEY))
                {
                    if (key == null) return false;

                    // KeyboardDelay: 0 = Sin delay (más responsive)
                    // KeyboardSpeed: 31 = Máxima velocidad de repetición
                    key.SetValue("KeyboardDelay", "0", RegistryValueKind.String);
                    key.SetValue("KeyboardSpeed", "31", RegistryValueKind.String);

                    Debug.WriteLine("? Keyboard Optimized");
                    Debug.WriteLine("  Delay: 0 (Sin delay)");
                    Debug.WriteLine("  Speed: 31 (Máxima)");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Keyboard Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// DESHABILITAR FULLSCREEN OPTIMIZATION (FSO)
        /// Windows fuerza Borderless Fullscreen (añade latencia)
        /// </summary>
        public static bool DisableFSO()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(GAME_CONFIG_STORE))
                {
                    if (key == null) return false;

                    // GameDVR_FSEBehavior = 2: Deshabilita FSO globalmente
                    key.SetValue("GameDVR_FSEBehavior", 2, RegistryValueKind.DWord);

                    Debug.WriteLine("? Fullscreen Optimization DISABLED");
                    Debug.WriteLine("  Permite true exclusive fullscreen");
                    Debug.WriteLine("  Input lag reducido 2-5ms");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? FSO Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// DESHABILITAR MPO (Multiplane Overlay)
        /// Causa pantallazos negros y stuttering
        /// </summary>
        public static bool DisableMPO()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(DWM_KEY))
                {
                    if (key == null) return false;

                    // OverlayTestMode = 5: Desactiva MPO
                    key.SetValue("OverlayTestMode", 5, RegistryValueKind.DWord);

                    Debug.WriteLine("? MPO (Multiplane Overlay) DISABLED");
                    Debug.WriteLine("  Fix: Pantallazos negros");
                    Debug.WriteLine("  Fix: Stuttering con overlays");
                    Debug.WriteLine("  ?? REINICIO OBLIGATORIO");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? MPO Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// HARDWARE ACCELERATED GPU SCHEDULING (HAGS)
        /// 
        /// ?? CONTROVERSIAL: Puede mejorar o empeorar latencia
        /// 
        /// QUÉ ES:
        /// - Windows 10/11 feature que delega scheduling a la GPU
        /// - Reduce overhead de CPU
        /// 
        /// VENTAJAS:
        /// ? Reduce latencia en GPUs modernas (RTX 3000+, RX 6000+)
        /// ? Mejor frame pacing en algunos casos
        /// 
        /// DESVENTAJAS:
        /// ? Puede aumentar latencia en GPUs viejas (GTX 1000)
        /// ? Drivers inmaduros causan problemas
        /// 
        /// RECOMENDACIÓN:
        /// - ACTIVAR si tienes RTX 3000+ o RX 6000+
        /// - DESACTIVAR si tienes GTX 1000/RX 500
        /// - PROBAR ambos estados y medir con FrameView
        /// </summary>
        public static bool EnableHAGS()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(GRAPHICS_DRIVERS))
                {
                    if (key == null) return false;

                    // HwSchMode = 2: Habilita HAGS
                    key.SetValue("HwSchMode", 2, RegistryValueKind.DWord);

                    Debug.WriteLine("? HAGS (Hardware GPU Scheduling) ENABLED");
                    Debug.WriteLine("  ?? Probar si mejora o empeora latencia");
                    Debug.WriteLine("  ?? REINICIO OBLIGATORIO");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? HAGS Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// DESHABILITAR HAGS
        /// </summary>
        public static bool DisableHAGS()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(GRAPHICS_DRIVERS))
                {
                    if (key == null) return false;

                    // HwSchMode = 1: Desactiva HAGS
                    key.SetValue("HwSchMode", 1, RegistryValueKind.DWord);

                    Debug.WriteLine("? HAGS DISABLED");
                    Debug.WriteLine("  CPU maneja scheduling (clásico)");
                    Debug.WriteLine("  ?? REINICIO OBLIGATORIO");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? HAGS Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// APLICAR TODOS LOS TWEAKS AVANZADOS
        /// </summary>
        public static bool Apply()
        {
            bool success = true;

            success &= OptimizeKeyboard();
            success &= DisableFSO();
            success &= DisableMPO();
            // HAGS no se incluye en Apply() porque es controversial

            Debug.WriteLine("????????????????????????????????????????");
            Debug.WriteLine("? ADVANCED TWEAKS APLICADOS");
            Debug.WriteLine("????????????????????????????????????????");

            return success;
        }

        /// <summary>
        /// DESHABILITAR MITIGACIONES SPECTRE & MELTDOWN
        /// Gana +5-15% FPS a cambio de seguridad
        /// EXPONE tu sistema a vulnerabilidades. REQUIERE REINICIO.
        /// </summary>
        public static bool DisableSpectreMeltdown()
        {
            try
            {
                const string sessionMgrKey = @"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management";
                
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(sessionMgrKey))
                {
                    if (key == null) return false;

                    // Deshabilita las mitigaciones de Spectre/Meltdown
                    key.SetValue("FeatureSettingsOverride", 3, RegistryValueKind.DWord);
                    key.SetValue("FeatureSettingsOverrideMask", 3, RegistryValueKind.DWord);

                    Debug.WriteLine("?? Spectre & Meltdown Mitigations DISABLED");
                    Debug.WriteLine("  ? Ganancia: +5-15% FPS");
                    Debug.WriteLine("  ?? SEGURIDAD REDUCIDA");
                    Debug.WriteLine("  ?? REINICIO OBLIGATORIO");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Spectre/Meltdown Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// HABILITAR MITIGACIONES SPECTRE & MELTDOWN
        /// Restaura la seguridad del sistema
        /// </summary>
        public static bool EnableSpectreMeltdown()
        {
            try
            {
                const string sessionMgrKey = @"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management";
                
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(sessionMgrKey))
                {
                    if (key == null) return false;

                    // Habilita las mitigaciones (valores por defecto)
                    key.SetValue("FeatureSettingsOverride", 0, RegistryValueKind.DWord);
                    key.SetValue("FeatureSettingsOverrideMask", 0, RegistryValueKind.DWord);

                    Debug.WriteLine("? Spectre & Meltdown Mitigations ENABLED");
                    Debug.WriteLine("  ?? Seguridad restaurada");
                    Debug.WriteLine("  ?? REINICIO OBLIGATORIO");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Spectre/Meltdown Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// RESTAURAR VALORES PREDETERMINADOS
        /// </summary>
        public static bool Revert()
        {
            try
            {
                // Keyboard defaults
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(KEYBOARD_KEY))
                {
                    if (key != null)
                    {
                        key.SetValue("KeyboardDelay", "1", RegistryValueKind.String);
                        key.SetValue("KeyboardSpeed", "31", RegistryValueKind.String);
                    }
                }

                // FSO default (enabled)
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(GAME_CONFIG_STORE))
                {
                    if (key != null)
                    {
                        key.SetValue("GameDVR_FSEBehavior", 0, RegistryValueKind.DWord);
                    }
                }

                // MPO default (enabled)
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(DWM_KEY))
                {
                    if (key != null)
                    {
                        try { key.DeleteValue("OverlayTestMode"); } catch { }
                    }
                }

                Debug.WriteLine("? Advanced Tweaks restaurados");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error: {ex.Message}");
                return false;
            }
        }
    }
}
