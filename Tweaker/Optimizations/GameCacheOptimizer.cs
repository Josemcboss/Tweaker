using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Win32;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// GameCacheOptimizer - Inspirado en Paragon Tweaking Utility (PTU)
    /// Limpieza avanzada de Shader Cache y optimización de perfiles de juegos competitivos (Fortnite, Valorant, Apex, CS2, etc.)
    /// </summary>
    public static class GameCacheOptimizer
    {
        #region Shader Cache Cleanup

        /// <summary>
        /// Limpia todas las carpetas de caché de shaders de DirectX, NVIDIA, AMD y Unreal Engine
        /// Elimina stutters provocados por shaders corruptos o fragmentados
        /// </summary>
        public static (bool success, long bytesFreed, int filesDeleted) CleanAllShaderCaches()
        {
            long totalBytesFreed = 0;
            int totalFilesDeleted = 0;

            Debug.WriteLine("──────────────────────────────────────────");
            Debug.WriteLine("GAME CACHE OPTIMIZER - Limpieza de Shaders");
            Debug.WriteLine("──────────────────────────────────────────");

            try
            {
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string commonAppData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

                // 1. DirectX Shader Cache de Windows
                string d3dsCache = Path.Combine(localAppData, "D3DSCache");
                var res1 = CleanDirectory(d3dsCache);
                totalBytesFreed += res1.bytes;
                totalFilesDeleted += res1.files;

                // 2. NVIDIA Shader Caches
                string nvDxCache = Path.Combine(localAppData, "NVIDIA", "DXCache");
                var res2 = CleanDirectory(nvDxCache);
                totalBytesFreed += res2.bytes;
                totalFilesDeleted += res2.files;

                string nvGlCache = Path.Combine(localAppData, "NVIDIA", "GLCache");
                var res3 = CleanDirectory(nvGlCache);
                totalBytesFreed += res3.bytes;
                totalFilesDeleted += res3.files;

                string nvComputeCache = Path.Combine(appData, "NVIDIA", "ComputeCache");
                var res4 = CleanDirectory(nvComputeCache);
                totalBytesFreed += res4.bytes;
                totalFilesDeleted += res4.files;

                string nvProgramDataCache = Path.Combine(commonAppData, "NVIDIA Corporation", "NV_Cache");
                var res5 = CleanDirectory(nvProgramDataCache);
                totalBytesFreed += res5.bytes;
                totalFilesDeleted += res5.files;

                // 3. AMD Shader Caches
                string amdDxCache = Path.Combine(localAppData, "AMD", "DxCache");
                var res6 = CleanDirectory(amdDxCache);
                totalBytesFreed += res6.bytes;
                totalFilesDeleted += res6.files;

                string amdGlCache = Path.Combine(localAppData, "AMD", "GLCache");
                var res7 = CleanDirectory(amdGlCache);
                totalBytesFreed += res7.bytes;
                totalFilesDeleted += res7.files;

                // 4. Unreal Engine / Juegos Populares (Fortnite, Valorant, Apex)
                string fortniteCache = Path.Combine(localAppData, "FortniteGame", "Saved", "webcache");
                var res8 = CleanDirectory(fortniteCache);
                totalBytesFreed += res8.bytes;
                totalFilesDeleted += res8.files;

                string valorantWebCache = Path.Combine(localAppData, "VALORANT", "Saved", "webcache");
                var res9 = CleanDirectory(valorantWebCache);
                totalBytesFreed += res9.bytes;
                totalFilesDeleted += res9.files;

                long mbFreed = totalBytesFreed / (1024 * 1024);
                Debug.WriteLine($"✓ Limpieza de shaders completada: {totalFilesDeleted} archivos eliminados, {mbFreed} MB liberados");
                return (true, totalBytesFreed, totalFilesDeleted);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error en CleanAllShaderCaches: {ex.Message}");
                return (false, totalBytesFreed, totalFilesDeleted);
            }
        }

        private static (long bytes, int files) CleanDirectory(string path)
        {
            if (!Directory.Exists(path))
                return (0, 0);

            long bytesFreed = 0;
            int filesDeleted = 0;

            try
            {
                var dirInfo = new DirectoryInfo(path);
                foreach (var file in dirInfo.GetFiles("*", SearchOption.AllDirectories))
                {
                    try
                    {
                        long size = file.Length;
                        file.Attributes = FileAttributes.Normal;
                        file.Delete();
                        bytesFreed += size;
                        filesDeleted++;
                    }
                    catch
                    {
                        // Archivo bloqueado por proceso activo, omitir de forma segura
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"⚠ Aviso al limpiar directorio {path}: {ex.Message}");
            }

            return (bytesFreed, filesDeleted);
        }

        #endregion

        #region Game Profiles & Latency Optimization

        /// <summary>
        /// Optimiza perfiles de juegos competitivos a nivel de registro:
        /// - Desactiva Fullscreen Optimizations artificiales de Windows (DWM latency bypass)
        /// - Desactiva GameDVR y Game Bar Background Recording
        /// - Prioriza Alto Rendimiento en GPU para juegos
        /// </summary>
        public static bool OptimizeGameProfiles()
        {
            try
            {
                Debug.WriteLine("→ Aplicando optimizaciones de perfiles de juegos (PTU Inspired)...");

                // 1. GameConfigStore - Desactivar GameDVR y Optimizaciones que añaden input lag
                using (RegistryKey? key = Registry.CurrentUser.CreateSubKey(@"System\GameConfigStore"))
                {
                    if (key != null)
                    {
                        key.SetValue("GameDVR_Enabled", 0, RegistryValueKind.DWord);
                        key.SetValue("GameDVR_FSEBehaviorMode", 2, RegistryValueKind.DWord);
                        key.SetValue("GameDVR_HonorUserFSEBehaviorMode", 1, RegistryValueKind.DWord);
                        key.SetValue("GameDVR_DXGIHonorFSEWindowsCompatible", 1, RegistryValueKind.DWord);
                        key.SetValue("GameDVR_EFSEFeatureFlags", 0, RegistryValueKind.DWord);
                        key.SetValue("GameDVR_DSEBehavior", 2, RegistryValueKind.DWord);
                    }
                }

                // 2. Desactivar captura de GameDVR a nivel de máquina
                using (RegistryKey? key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\GameDVR"))
                {
                    if (key != null)
                    {
                        key.SetValue("AllowGameDVR", 0, RegistryValueKind.DWord);
                    }
                }

                // 3. DirectX User GPU Preferences - Forzar Alto Rendimiento
                using (RegistryKey? key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\DirectX\UserGpuPreferences"))
                {
                    if (key != null)
                    {
                        // GpuPreference=2 indica High Performance GPU
                        key.SetValue("DirectXUserGlobalSettings", "GpuPreference=2;", RegistryValueKind.String);
                    }
                }

                // 4. Multimedia System Profile - Juegos en Ultra Priority
                using (RegistryKey? key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games"))
                {
                    if (key != null)
                    {
                        key.SetValue("Affinity", 0, RegistryValueKind.DWord);
                        key.SetValue("Background Only", "False", RegistryValueKind.String);
                        key.SetValue("Clock Rate", 10000, RegistryValueKind.DWord);
                        key.SetValue("GPU Priority", 8, RegistryValueKind.DWord);
                        key.SetValue("Priority", 6, RegistryValueKind.DWord);
                        key.SetValue("Scheduling Category", "High", RegistryValueKind.String);
                        key.SetValue("SFIO Priority", "High", RegistryValueKind.String);
                    }
                }

                Debug.WriteLine("✓ Perfiles de juegos y optimizaciones de latencia aplicadas exitosamente.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error en OptimizeGameProfiles: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Restaura la configuración predeterminada de perfiles de juegos de Windows
        /// </summary>
        public static bool RestoreGameProfiles()
        {
            try
            {
                Debug.WriteLine("→ Restaurando perfiles de juegos por defecto...");

                using (RegistryKey? key = Registry.CurrentUser.CreateSubKey(@"System\GameConfigStore"))
                {
                    if (key != null)
                    {
                        key.SetValue("GameDVR_Enabled", 1, RegistryValueKind.DWord);
                        key.SetValue("GameDVR_FSEBehaviorMode", 0, RegistryValueKind.DWord);
                    }
                }

                using (RegistryKey? key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\GameDVR"))
                {
                    if (key != null)
                    {
                        key.DeleteValue("AllowGameDVR", false);
                    }
                }

                using (RegistryKey? key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\DirectX\UserGpuPreferences"))
                {
                    if (key != null)
                    {
                        key.DeleteValue("DirectXUserGlobalSettings", false);
                    }
                }

                Debug.WriteLine("✓ Perfiles de juegos restaurados.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error en RestoreGameProfiles: {ex.Message}");
                return false;
            }
        }

        #endregion
    }
}
