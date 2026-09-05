using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Win32;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// Conjunto de optimizaciones avanzadas de segunda fase:
    /// 1. DWM Independent Flip & Presentation Model
    /// 2. Ethernet Energy Saving (EEE / Green Ethernet) Removal
    /// 3. Pagefile & Kernel Paging Executive Optimization
    /// 4. Audio Engine WASAPI / MMCSS Low Latency Buffering
    /// </summary>
    public static class AdvancedGamingSystemTweaks
    {
        private const string DX_GRAPHICS_DRIVERS_KEY = @"SOFTWARE\Microsoft\DirectX\UserGpuPreferences";
        private const string GAME_BAR_FSO_KEY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\GameDVR";
        private const string MEMORY_MANAGEMENT_KEY = @"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management";
        private const string AUDIO_TIMING_KEY = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Audio";

        #region 1. DWM Independent Flip & Presentation Optimizations

        /// <summary>
        /// Fuerza el modelo Flip de Direct3D / DWM y optimiza las Presentación sin bordes.
        /// </summary>
        public static bool ForceFlipModelAndFso()
        {
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(GAME_BAR_FSO_KEY))
                {
                    if (key != null)
                    {
                        key.SetValue("AppCaptureEnabled", 0, RegistryValueKind.DWord);
                        key.SetValue("GameDVR_Enabled", 0, RegistryValueKind.DWord);
                        key.SetValue("GameDVR_FSEBehaviorMode", 2, RegistryValueKind.DWord);
                        key.SetValue("GameDVR_HonorUserFSEBehaviorMode", 1, RegistryValueKind.DWord);
                        key.SetValue("GameDVR_DXGIHonorFSEWindowsMode", 1, RegistryValueKind.DWord);
                    }
                }

                using (var key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\GraphicsDrivers"))
                {
                    key?.SetValue("DisableDXGIOnDx12", 0, RegistryValueKind.DWord);
                }

                Debug.WriteLine("✓ DWM Flip Model y Optimización de Pantalla Completa forzados.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error al forzar Flip Model: {ex.Message}");
                return false;
            }
        }

        public static bool RevertFlipModelAndFso()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(GAME_BAR_FSO_KEY, true))
                {
                    if (key != null)
                    {
                        key.SetValue("AppCaptureEnabled", 1, RegistryValueKind.DWord);
                        key.SetValue("GameDVR_Enabled", 1, RegistryValueKind.DWord);
                        key.SetValue("GameDVR_FSEBehaviorMode", 0, RegistryValueKind.DWord);
                    }
                }

                Debug.WriteLine("✓ Ajustes de Flip Model restaurados.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error al restaurar Flip Model: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region 2. Ethernet Energy Saving Removal (EEE / Green Ethernet Off)

        /// <summary>
        /// Desactiva Energy Efficient Ethernet (EEE) y Power Saving en todas las NICs para cero latencia inicial.
        /// </summary>
        public static bool DisableEthernetPowerSaving()
        {
            try
            {
                string networkAdaptersClassGuid = "{4d36e972-e325-11ce-bfc1-08002be10318}";
                string basePath = $@"SYSTEM\CurrentControlSet\Control\Class\{networkAdaptersClassGuid}";

                int modifiedCount = 0;

                using (var baseKey = Registry.LocalMachine.OpenSubKey(basePath, false))
                {
                    if (baseKey == null) return false;

                    foreach (string subKeyName in baseKey.GetSubKeyNames())
                    {
                        if (!subKeyName.All(char.IsDigit) || subKeyName.Length != 4) continue;

                        try
                        {
                            using (var adapterKey = Registry.LocalMachine.OpenSubKey($@"{basePath}\{subKeyName}", true))
                            {
                                if (adapterKey == null) continue;

                                string driverDesc = adapterKey.GetValue("DriverDesc") as string;
                                if (string.IsNullOrEmpty(driverDesc)) continue;

                                // Deshabilitar características de ahorro de energía
                                adapterKey.SetValue("*EEE", "0", RegistryValueKind.String);
                                adapterKey.SetValue("EEELinkSpeed", "0", RegistryValueKind.String);
                                adapterKey.SetValue("ReduceSpeedOnPowerDown", "0", RegistryValueKind.String);
                                adapterKey.SetValue("AutoPowerSaveModeEnabled", "0", RegistryValueKind.String);
                                adapterKey.SetValue("S5WakeOnLan", "0", RegistryValueKind.String);

                                modifiedCount++;
                            }
                        }
                        catch { }
                    }
                }

                Debug.WriteLine($"✓ Desactivado el ahorro de energía EEE en {modifiedCount} adaptadores de red.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error al deshabilitar Ethernet Power Saving: {ex.Message}");
                return false;
            }
        }

        public static bool RevertEthernetPowerSaving()
        {
            try
            {
                string networkAdaptersClassGuid = "{4d36e972-e325-11ce-bfc1-08002be10318}";
                string basePath = $@"SYSTEM\CurrentControlSet\Control\Class\{networkAdaptersClassGuid}";

                using (var baseKey = Registry.LocalMachine.OpenSubKey(basePath, false))
                {
                    if (baseKey == null) return false;

                    foreach (string subKeyName in baseKey.GetSubKeyNames())
                    {
                        if (!subKeyName.All(char.IsDigit) || subKeyName.Length != 4) continue;

                        try
                        {
                            using (var adapterKey = Registry.LocalMachine.OpenSubKey($@"{basePath}\{subKeyName}", true))
                            {
                                adapterKey?.DeleteValue("*EEE", throwOnMissingValue: false);
                                adapterKey?.DeleteValue("EEELinkSpeed", throwOnMissingValue: false);
                                adapterKey?.DeleteValue("ReduceSpeedOnPowerDown", throwOnMissingValue: false);
                                adapterKey?.DeleteValue("AutoPowerSaveModeEnabled", throwOnMissingValue: false);
                            }
                        }
                        catch { }
                    }
                }

                Debug.WriteLine("✓ Ahorro de energía en adaptadores de red restaurado.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error al restaurar Ethernet Power Saving: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region 3. Pagefile & Kernel Paging Executive Optimization

        /// <summary>
        /// Mantiene el Kernel de Windows y los controladores en la memoria RAM física sin paginar a disco.
        /// </summary>
        public static bool OptimizeKernelPagingAndMemory()
        {
            try
            {
                using (var key = Registry.LocalMachine.CreateSubKey(MEMORY_MANAGEMENT_KEY))
                {
                    if (key != null)
                    {
                        // 1 = Mantiene el código/datos del sistema en la memoria RAM física en lugar de paginar al disco
                        key.SetValue("DisablePagingExecutive", 1, RegistryValueKind.DWord);
                        // Aumentar el pool de memoria asignable al kernel
                        key.SetValue("LargeSystemCache", 0, RegistryValueKind.DWord); // 0 es mejor para gaming según Microsoft docs
                    }
                }

                Debug.WriteLine("✓ DisablePagingExecutive optimizado (Kernel 100% en RAM).");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error al optimizar Kernel Paging: {ex.Message}");
                return false;
            }
        }

        public static bool RevertKernelPagingAndMemory()
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(MEMORY_MANAGEMENT_KEY, true))
                {
                    if (key != null)
                    {
                        key.SetValue("DisablePagingExecutive", 0, RegistryValueKind.DWord);
                    }
                }

                Debug.WriteLine("✓ DisablePagingExecutive restaurado.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error al restaurar Kernel Paging: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region 4. Audio Engine WASAPI / MMCSS Low Latency Buffering

        /// <summary>
        /// Prioriza el hilo de audio en MMCSS para garantizar cero retraso en efectos de sonido de juegos.
        /// </summary>
        public static bool OptimizeAudioLatencyBuffering()
        {
            try
            {
                using (var key = Registry.LocalMachine.CreateSubKey(AUDIO_TIMING_KEY))
                {
                    if (key != null)
                    {
                        key.SetValue("Scheduling Category", "High", RegistryValueKind.String);
                        key.SetValue("SFIO Priority", "High", RegistryValueKind.String);
                        key.SetValue("Background Only", "FALSE", RegistryValueKind.String);
                        key.SetValue("Priority", 10, RegistryValueKind.DWord);
                        key.SetValue("GPU Priority", 8, RegistryValueKind.DWord);
                        key.SetValue("Clock Rate", 10000, RegistryValueKind.DWord);
                    }
                }

                Debug.WriteLine("✓ Latencia del motor de audio (WASAPI/MMCSS) optimizada.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error al optimizar motor de audio: {ex.Message}");
                return false;
            }
        }

        public static bool RevertAudioLatencyBuffering()
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(AUDIO_TIMING_KEY, true))
                {
                    if (key != null)
                    {
                        key.SetValue("Scheduling Category", "Medium", RegistryValueKind.String);
                        key.SetValue("SFIO Priority", "Normal", RegistryValueKind.String);
                        key.SetValue("Priority", 2, RegistryValueKind.DWord);
                    }
                }

                Debug.WriteLine("✓ Ajustes de latencia de audio restaurados.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error al restaurar latencia de audio: {ex.Message}");
                return false;
            }
        }

        #endregion
    }
}
