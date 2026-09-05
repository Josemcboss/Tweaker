using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Win32;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// Pack de 10 optimizaciones avanzadas de rendimiento competitivo para gaming y baja latencia.
    /// 1. Page Combining Disable (Kernel RAM Deduplication Off)
    /// 2. TSC Invariant Hardware Clock (Reloj Hardware Invariante)
    /// 3. Quantum de CPU para Juegos (Win32PrioritySeparation 0x28)
    /// 4. Distribución de Colas RSS de Red (Separación de DPC del Core 0)
    /// 5. Desactivación de Autologgers ETW de Diagnóstico
    /// 6. Caché de Sombreadores DirectX Sin Límite (10GB Shader Cache)
    /// 7. Fault Tolerant Heap (FTH) Disable
    /// 8. USB Selective Suspend Disable (Zero Latency Polling)
    /// 9. TCP Timestamps Off & SACK Optimization
    /// 10. Multimedia Scheduler NoLazyMode & Extreme Responsiveness
    /// </summary>
    public static class CompetitivePerformanceTweaks
    {
        private const string MEMORY_MANAGEMENT_KEY = @"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management";
        private const string PRIORITY_CONTROL_KEY = @"SYSTEM\CurrentControlSet\Control\PriorityControl";
        private const string AUTOLOGGER_DIAG_KEY = @"SYSTEM\CurrentControlSet\Control\WMI\Autologger\AutoLogger-Diagtrack-Listener";
        private const string AUTOLOGGER_SQM_KEY = @"SYSTEM\CurrentControlSet\Control\WMI\Autologger\SQMLogger";
        private const string AUTOLOGGER_AIT_KEY = @"SYSTEM\CurrentControlSet\Control\WMI\Autologger\AitAgent";
        private const string FTH_KEY = @"SOFTWARE\Microsoft\FTH";
        private const string USB_KEY = @"SYSTEM\CurrentControlSet\Services\USB";
        private const string MULTIMEDIA_SYSTEM_PROFILE_KEY = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile";
        private const string MULTIMEDIA_GAMES_KEY = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games";
        private const string DIRECTX_KEY = @"SOFTWARE\Microsoft\DirectX";
        private const string DIRECTX_USER_KEY = @"Software\Microsoft\DirectX";

        #region 1. Page Combining (RAM Deduplication Off)
        public static bool DisablePageCombining()
        {
            try
            {
                using (var key = Registry.LocalMachine.CreateSubKey(MEMORY_MANAGEMENT_KEY))
                {
                    key?.SetValue("PageCombining", 0, RegistryValueKind.DWord);
                }

                RunProcess("powershell", "-NoProfile -Command \"Disable-MMAgent -PageCombining -ErrorAction SilentlyContinue\"");
                Debug.WriteLine("✓ Page Combining deshabilitado (Cero ciclos de CPU desperdiciados en escanear RAM).");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error al deshabilitar Page Combining: {ex.Message}");
                return false;
            }
        }

        public static bool RevertPageCombining()
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(MEMORY_MANAGEMENT_KEY, true))
                {
                    key?.DeleteValue("PageCombining", false);
                }

                RunProcess("powershell", "-NoProfile -Command \"Enable-MMAgent -PageCombining -ErrorAction SilentlyContinue\"");
                return true;
            }
            catch { return false; }
        }

        public static bool IsPageCombiningDisabled()
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(MEMORY_MANAGEMENT_KEY);
                var val = key?.GetValue("PageCombining");
                if (val != null && Convert.ToInt32(val) == 0) return true;

                return false;
            }
            catch { return false; }
        }
        #endregion

        #region 2. TSC Invariant Hardware Clock
        public static bool EnableTscInvariantClock()
        {
            try
            {
                RunProcess("bcdedit", "/set tscsyncpolicy Enhanced");
                RunProcess("bcdedit", "/set useplatformtick yes");
                Debug.WriteLine("✓ Reloj TSC Invariante activado (Cero jitter de reloj en DirectX 12).");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error al activar TSC Invariant: {ex.Message}");
                return false;
            }
        }

        public static bool RevertTscInvariantClock()
        {
            try
            {
                RunProcess("bcdedit", "/deletevalue tscsyncpolicy");
                RunProcess("bcdedit", "/deletevalue useplatformtick");
                return true;
            }
            catch { return false; }
        }

        public static bool IsTscInvariantClockEnabled()
        {
            try
            {
                var psi = new ProcessStartInfo("bcdedit", "/enum {current}")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using var p = Process.Start(psi);
                if (p == null) return false;
                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit(1500);

                return output.IndexOf("tscsyncpolicy", StringComparison.OrdinalIgnoreCase) >= 0 &&
                       output.IndexOf("Enhanced", StringComparison.OrdinalIgnoreCase) >= 0;
            }
            catch { return false; }
        }
        #endregion

        #region 3. Win32PrioritySeparation Quantum 0x28 (Foreground Max Boost)
        public static bool OptimizeGamingQuantum()
        {
            try
            {
                using (var key = Registry.LocalMachine.CreateSubKey(PRIORITY_CONTROL_KEY))
                {
                    // 0x28 (40 dec) = Short fixed quantum with 3:1 foreground boost
                    key?.SetValue("Win32PrioritySeparation", 40, RegistryValueKind.DWord);
                }
                Debug.WriteLine("✓ Quantum de CPU para gaming optimizado (0x28).");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error en OptimizeGamingQuantum: {ex.Message}");
                return false;
            }
        }

        public static bool RevertGamingQuantum()
        {
            try
            {
                using (var key = Registry.LocalMachine.CreateSubKey(PRIORITY_CONTROL_KEY))
                {
                    key?.SetValue("Win32PrioritySeparation", 2, RegistryValueKind.DWord);
                }
                return true;
            }
            catch { return false; }
        }

        public static bool IsGamingQuantumOptimized()
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(PRIORITY_CONTROL_KEY);
                var val = key?.GetValue("Win32PrioritySeparation");
                return val != null && (Convert.ToInt32(val) == 40 || Convert.ToInt32(val) == 38);
            }
            catch { return false; }
        }
        #endregion

        #region 4. Receive Side Scaling (RSS) Queues Optimization
        public static bool OptimizeRssQueues()
        {
            try
            {
                string cmd = "Get-NetAdapter -Physical -ErrorAction SilentlyContinue | ForEach-Object { Enable-NetAdapterRss -Name $_.Name -ErrorAction SilentlyContinue; Set-NetAdapterRss -Name $_.Name -NumberOfReceiveQueues 4 -BaseProcessorGroup 0 -BaseProcessorNumber 2 -MaxProcessorNumber 16 -ErrorAction SilentlyContinue }";
                RunProcess("powershell", $"-NoProfile -Command \"{cmd}\"");
                Debug.WriteLine("✓ Colas RSS de Red optimizadas (DPCs de red asignados fuera del Core 0).");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error al optimizar RSS: {ex.Message}");
                return false;
            }
        }

        public static bool RevertRssQueues()
        {
            try
            {
                string cmd = "Get-NetAdapter -Physical -ErrorAction SilentlyContinue | ForEach-Object { Set-NetAdapterRss -Name $_.Name -NumberOfReceiveQueues 2 -BaseProcessorNumber 0 -ErrorAction SilentlyContinue }";
                RunProcess("powershell", $"-NoProfile -Command \"{cmd}\"");
                return true;
            }
            catch { return false; }
        }

        public static bool IsRssQueuesOptimized()
        {
            try
            {
                var psi = new ProcessStartInfo("powershell", "-NoProfile -Command \"(Get-NetAdapterRss -ErrorAction SilentlyContinue | Select-Object -First 1).BaseProcessorNumber\"")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using var p = Process.Start(psi);
                if (p == null) return false;
                string o = p.StandardOutput.ReadToEnd().Trim();
                p.WaitForExit(1500);

                if (int.TryParse(o, out int baseNum))
                {
                    return baseNum >= 2;
                }
                return false;
            }
            catch { return false; }
        }
        #endregion

        #region 5. Disable Diagnostic AutoLoggers (ETW Background Writers)
        public static bool DisableDiagnosticAutoLoggers()
        {
            try
            {
                SetDWordKey(Registry.LocalMachine, AUTOLOGGER_DIAG_KEY, "Start", 0);
                SetDWordKey(Registry.LocalMachine, AUTOLOGGER_SQM_KEY, "Start", 0);
                SetDWordKey(Registry.LocalMachine, AUTOLOGGER_AIT_KEY, "Start", 0);
                Debug.WriteLine("✓ AutoLoggers de Diagnóstico ETW deshabilitados (Cero escrituras de telemetría a disco).");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error al deshabilitar AutoLoggers: {ex.Message}");
                return false;
            }
        }

        public static bool RevertDiagnosticAutoLoggers()
        {
            try
            {
                SetDWordKey(Registry.LocalMachine, AUTOLOGGER_DIAG_KEY, "Start", 1);
                SetDWordKey(Registry.LocalMachine, AUTOLOGGER_SQM_KEY, "Start", 1);
                SetDWordKey(Registry.LocalMachine, AUTOLOGGER_AIT_KEY, "Start", 1);
                return true;
            }
            catch { return false; }
        }

        public static bool AreDiagnosticAutoLoggersDisabled()
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(AUTOLOGGER_DIAG_KEY);
                var val = key?.GetValue("Start");
                return val != null && Convert.ToInt32(val) == 0;
            }
            catch { return false; }
        }
        #endregion

        #region 6. DirectX Shader Cache Expansion (10GB)
        public static bool OptimizeShaderCacheSize()
        {
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(DIRECTX_USER_KEY))
                {
                    key?.SetValue("D3D12_SHADER_CACHE_MAX_SIZE", 10240, RegistryValueKind.DWord);
                }
                using (var key = Registry.LocalMachine.CreateSubKey(DIRECTX_KEY))
                {
                    key?.SetValue("D3D12_SHADER_CACHE_MAX_SIZE", 10240, RegistryValueKind.DWord);
                }
                Debug.WriteLine("✓ Shader Cache ampliado a 10GB (Elimina stutters de recompilación).");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error al optimizar Shader Cache: {ex.Message}");
                return false;
            }
        }

        public static bool RevertShaderCacheSize()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(DIRECTX_USER_KEY, true))
                {
                    key?.DeleteValue("D3D12_SHADER_CACHE_MAX_SIZE", false);
                }
                using (var key = Registry.LocalMachine.OpenSubKey(DIRECTX_KEY, true))
                {
                    key?.DeleteValue("D3D12_SHADER_CACHE_MAX_SIZE", false);
                }
                return true;
            }
            catch { return false; }
        }

        public static bool IsShaderCacheSizeOptimized()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(DIRECTX_USER_KEY);
                var val = key?.GetValue("D3D12_SHADER_CACHE_MAX_SIZE");
                return val != null && Convert.ToInt32(val) >= 5120;
            }
            catch { return false; }
        }
        #endregion

        #region 7. Fault Tolerant Heap (FTH) Disable
        public static bool DisableFaultTolerantHeap()
        {
            try
            {
                using (var key = Registry.LocalMachine.CreateSubKey(FTH_KEY))
                {
                    key?.SetValue("Enabled", 0, RegistryValueKind.DWord);
                }
                RunProcess("powershell", "-NoProfile -Command \"Rundll32.exe fthsvc.dll,FthSysprepSpecialize -ErrorAction SilentlyContinue\"");
                Debug.WriteLine("✓ Fault Tolerant Heap deshabilitado (Cero overhead de intercepción en ejecutables de juegos).");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error al deshabilitar FTH: {ex.Message}");
                return false;
            }
        }

        public static bool RevertFaultTolerantHeap()
        {
            try
            {
                using (var key = Registry.LocalMachine.CreateSubKey(FTH_KEY))
                {
                    key?.SetValue("Enabled", 1, RegistryValueKind.DWord);
                }
                return true;
            }
            catch { return false; }
        }

        public static bool IsFaultTolerantHeapDisabled()
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(FTH_KEY);
                var val = key?.GetValue("Enabled");
                return val != null && Convert.ToInt32(val) == 0;
            }
            catch { return false; }
        }
        #endregion

        #region 8. USB Selective Suspend Disable (Zero Latency Polling)
        public static bool DisableUsbSelectiveSuspend()
        {
            try
            {
                using (var key = Registry.LocalMachine.CreateSubKey(USB_KEY))
                {
                    key?.SetValue("DisableSelectiveSuspend", 1, RegistryValueKind.DWord);
                }
                RunProcess("powercfg", "/setacvalueindex SCHEME_CURRENT 2a737441-1930-4402-8d77-b2bebba308a3 48e6b7a6-50f5-4760-a561-492c8c2b7f5e 0");
                RunProcess("powercfg", "/setactive SCHEME_CURRENT");
                Debug.WriteLine("✓ USB Selective Suspend deshabilitado (Latencia estable 1000Hz+ sin ahorro de energía).");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error al deshabilitar USB Selective Suspend: {ex.Message}");
                return false;
            }
        }

        public static bool RevertUsbSelectiveSuspend()
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(USB_KEY, true))
                {
                    key?.DeleteValue("DisableSelectiveSuspend", false);
                }
                RunProcess("powercfg", "/setacvalueindex SCHEME_CURRENT 2a737441-1930-4402-8d77-b2bebba308a3 48e6b7a6-50f5-4760-a561-492c8c2b7f5e 1");
                RunProcess("powercfg", "/setactive SCHEME_CURRENT");
                return true;
            }
            catch { return false; }
        }

        public static bool IsUsbSelectiveSuspendDisabled()
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(USB_KEY);
                var val = key?.GetValue("DisableSelectiveSuspend");
                return val != null && Convert.ToInt32(val) == 1;
            }
            catch { return false; }
        }
        #endregion

        #region 9. TCP Timestamps Off & SACK Optimization
        public static bool OptimizeTcpTimestampsAndSack()
        {
            try
            {
                RunProcess("netsh", "int tcp set global timestamps=disabled");
                RunProcess("netsh", "int tcp set global sacksegments=enabled");
                RunProcess("netsh", "int tcp set global initialRto=2000");
                Debug.WriteLine("✓ TCP Timestamps deshabilitado y SACK optimizado (12 bytes extra por paquete liberados).");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error en TCP Timestamps/SACK: {ex.Message}");
                return false;
            }
        }

        public static bool RevertTcpTimestampsAndSack()
        {
            try
            {
                RunProcess("netsh", "int tcp set global timestamps=allowed");
                RunProcess("netsh", "int tcp set global sacksegments=default");
                RunProcess("netsh", "int tcp set global initialRto=3000");
                return true;
            }
            catch { return false; }
        }

        public static bool IsTcpTimestampsDisabled()
        {
            try
            {
                var psi = new ProcessStartInfo("netsh", "int tcp show global")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using var p = Process.Start(psi);
                if (p == null) return false;
                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit(1500);

                foreach (var line in output.Split('\n'))
                {
                    if (line.IndexOf("Timestamps", StringComparison.OrdinalIgnoreCase) >= 0 &&
                        line.IndexOf("disabled", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch { return false; }
        }
        #endregion

        #region 10. Multimedia Scheduler NoLazyMode & Extreme Responsiveness
        public static bool OptimizeMultimediaExtreme()
        {
            try
            {
                using (var key = Registry.LocalMachine.CreateSubKey(MULTIMEDIA_SYSTEM_PROFILE_KEY))
                {
                    key?.SetValue("NoLazyMode", 1, RegistryValueKind.DWord);
                    key?.SetValue("NetworkThrottlingIndex", -1, RegistryValueKind.DWord);
                    key?.SetValue("SystemResponsiveness", 0, RegistryValueKind.DWord);
                }

                using (var gamesKey = Registry.LocalMachine.CreateSubKey(MULTIMEDIA_GAMES_KEY))
                {
                    gamesKey?.SetValue("Affinity", 0, RegistryValueKind.DWord);
                    gamesKey?.SetValue("Background Only", "FALSE", RegistryValueKind.String);
                    gamesKey?.SetValue("Clock Rate", 10000, RegistryValueKind.DWord);
                    gamesKey?.SetValue("GPU Priority", 8, RegistryValueKind.DWord);
                    gamesKey?.SetValue("Priority", 6, RegistryValueKind.DWord);
                    gamesKey?.SetValue("Scheduling Category", "High", RegistryValueKind.String);
                    gamesKey?.SetValue("SFIO Priority", "High", RegistryValueKind.String);
                }

                Debug.WriteLine("✓ Multimedia Scheduler configurado en modo Extreme NoLazyMode.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error al optimizar Multimedia Extreme: {ex.Message}");
                return false;
            }
        }

        public static bool RevertMultimediaExtreme()
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(MULTIMEDIA_SYSTEM_PROFILE_KEY, true))
                {
                    key?.DeleteValue("NoLazyMode", false);
                }
                return true;
            }
            catch { return false; }
        }

        public static bool IsMultimediaExtremeOptimized()
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(MULTIMEDIA_SYSTEM_PROFILE_KEY);
                var val = key?.GetValue("NoLazyMode");
                return val != null && Convert.ToInt32(val) == 1;
            }
            catch { return false; }
        }
        #endregion

        #region 11. MPO Disable (Multi-Plane Overlay — Elimina Flickering Nvidia)
        public static bool DisableMpo()
        {
            try
            {
                using var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\Dwm");
                key?.SetValue("OverlayTestMode", 5, RegistryValueKind.DWord);
                Debug.WriteLine("✓ MPO deshabilitado (OverlayTestMode=5) — Sin flickering en NVIDIA 461+.");
                return true;
            }
            catch (Exception ex) { Debug.WriteLine($"❌ Error MPO Disable: {ex.Message}"); return false; }
        }

        public static bool RevertMpo()
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\Dwm", true);
                key?.DeleteValue("OverlayTestMode", false);
                Debug.WriteLine("✓ MPO restaurado (OverlayTestMode eliminado).");
                return true;
            }
            catch (Exception ex) { Debug.WriteLine($"❌ Error MPO Revert: {ex.Message}"); return false; }
        }

        public static bool IsMpoDisabled()
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\Dwm");
                var val = key?.GetValue("OverlayTestMode");
                return val != null && Convert.ToInt32(val) == 5;
            }
            catch { return false; }
        }
        #endregion

        #region 12. LazyModeTimeout 10000 (MMCSS Cycle Suspension Optimization)
        public static bool SetLazyModeTimeout()
        {
            try
            {
                using var key = Registry.LocalMachine.CreateSubKey(MULTIMEDIA_SYSTEM_PROFILE_KEY);
                key?.SetValue("LazyModeTimeout", 10000, RegistryValueKind.DWord);
                Debug.WriteLine("✓ LazyModeTimeout = 10000 — Ciclos MMCSS idle reducidos al mínimo.");
                return true;
            }
            catch (Exception ex) { Debug.WriteLine($"❌ Error LazyModeTimeout: {ex.Message}"); return false; }
        }

        public static bool RevertLazyModeTimeout()
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(MULTIMEDIA_SYSTEM_PROFILE_KEY, true);
                key?.DeleteValue("LazyModeTimeout", false);
                Debug.WriteLine("✓ LazyModeTimeout restaurado (eliminado — default Windows).");
                return true;
            }
            catch (Exception ex) { Debug.WriteLine($"❌ Error LazyModeTimeout Revert: {ex.Message}"); return false; }
        }

        public static bool IsLazyModeTimeoutSet()
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(MULTIMEDIA_SYSTEM_PROFILE_KEY);
                var val = key?.GetValue("LazyModeTimeout");
                return val != null && Convert.ToInt32(val) == 10000;
            }
            catch { return false; }
        }
        #endregion

        #region 13. ThreadDPC Disable (Elimina Latencia de Contexto DPC)
        private const string KERNEL_SESSION_KEY = @"SYSTEM\CurrentControlSet\Control\Session Manager\kernel";

        public static bool DisableThreadDpc()
        {
            try
            {
                using var key = Registry.LocalMachine.CreateSubKey(KERNEL_SESSION_KEY);
                key?.SetValue("ThreadDpcEnable", 0, RegistryValueKind.DWord);
                Debug.WriteLine("✓ ThreadDpcEnable=0 — DPCs en modo non-threaded, latencia reducida.");
                return true;
            }
            catch (Exception ex) { Debug.WriteLine($"❌ Error ThreadDPC Disable: {ex.Message}"); return false; }
        }

        public static bool RevertThreadDpc()
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(KERNEL_SESSION_KEY, true);
                key?.DeleteValue("ThreadDpcEnable", false);
                Debug.WriteLine("✓ ThreadDpcEnable restaurado (default Windows).");
                return true;
            }
            catch (Exception ex) { Debug.WriteLine($"❌ Error ThreadDPC Revert: {ex.Message}"); return false; }
        }

        public static bool IsThreadDpcDisabled()
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(KERNEL_SESSION_KEY);
                var val = key?.GetValue("ThreadDpcEnable");
                return val != null && Convert.ToInt32(val) == 0;
            }
            catch { return false; }
        }
        #endregion

        #region 14. IoLatencyCap = 80 (StorPort I/O Queue Limit)
        private const string IO_LATENCY_KEY = @"SYSTEM\CurrentControlSet\Services\stornvme\Parameters";

        public static bool SetIoLatencyCap()
        {
            try
            {
                // StorNVMe IoLatencyCap — limita la cola de I/O a 80 ticks
                using var key = Registry.LocalMachine.CreateSubKey(IO_LATENCY_KEY);
                key?.SetValue("IoLatencyCap", 80, RegistryValueKind.DWord);
                // También aplicar a storport genérico
                using var key2 = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Services\storport\Parameters");
                key2?.SetValue("IoLatencyCap", 80, RegistryValueKind.DWord);
                Debug.WriteLine("✓ IoLatencyCap = 80 — Cola StorPort limitada para SSD gaming sin congelamiento.");
                return true;
            }
            catch (Exception ex) { Debug.WriteLine($"❌ Error IoLatencyCap: {ex.Message}"); return false; }
        }

        public static bool RevertIoLatencyCap()
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(IO_LATENCY_KEY, true);
                key?.DeleteValue("IoLatencyCap", false);
                using var key2 = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\storport\Parameters", true);
                key2?.DeleteValue("IoLatencyCap", false);
                Debug.WriteLine("✓ IoLatencyCap restaurado (eliminado).");
                return true;
            }
            catch (Exception ex) { Debug.WriteLine($"❌ Error IoLatencyCap Revert: {ex.Message}"); return false; }
        }

        public static bool IsIoLatencyCapSet()
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\storport\Parameters");
                var val = key?.GetValue("IoLatencyCap");
                return val != null && Convert.ToInt32(val) == 80;
            }
            catch { return false; }
        }
        #endregion

        #region 15. Driver PPM Disable (IntelPPM / AmdPPM — CPU Turbo Estable)
        public static bool DisableDriverPpm()
        {
            try
            {
                // Deshabilitar IntelPPM (Start=4 = disabled)
                using (var key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Services\intelppm"))
                    key?.SetValue("Start", 4, RegistryValueKind.DWord);
                // Deshabilitar AmdPPM (Start=4 = disabled)
                using (var key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Services\amdppm"))
                    key?.SetValue("Start", 4, RegistryValueKind.DWord);
                Debug.WriteLine("✓ IntelPPM/AmdPPM deshabilitados — Gestión de energía del CPU por software eliminada.");
                return true;
            }
            catch (Exception ex) { Debug.WriteLine($"❌ Error DriverPPM Disable: {ex.Message}"); return false; }
        }

        public static bool RevertDriverPpm()
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\intelppm", true))
                    key?.SetValue("Start", 1, RegistryValueKind.DWord);
                using (var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\amdppm", true))
                    key?.SetValue("Start", 1, RegistryValueKind.DWord);
                Debug.WriteLine("✓ IntelPPM/AmdPPM restaurados (Start=1).");
                return true;
            }
            catch (Exception ex) { Debug.WriteLine($"❌ Error DriverPPM Revert: {ex.Message}"); return false; }
        }

        public static bool IsDriverPpmDisabled()
        {
            try
            {
                // Chequear cualquiera de los dos
                using var intel = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\intelppm");
                var val = intel?.GetValue("Start");
                if (val != null && Convert.ToInt32(val) == 4) return true;
                using var amd = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\amdppm");
                var val2 = amd?.GetValue("Start");
                return val2 != null && Convert.ToInt32(val2) == 4;
            }
            catch { return false; }
        }
        #endregion

        #region 16. CPU Idle Disable (C-State 0 Forzado — Latencia Mínima)
        // GUID del power plan activo + sub-setting de idle
        private const string IDLE_SUBGROUP  = "54533251-82be-4824-96c1-47b60b740d00";
        private const string IDLE_SETTING   = "5d76a2ca-e8c0-402f-a133-2158492d58ad";

        public static bool DisableCpuIdle()
        {
            try
            {
                // 1. Obtener GUID del plan de energía activo
                var psi = new ProcessStartInfo("powercfg", "/getactivescheme")
                {
                    RedirectStandardOutput = true, UseShellExecute = false, CreateNoWindow = true
                };
                using var proc = Process.Start(psi);
                string output = proc?.StandardOutput.ReadToEnd() ?? "";
                proc?.WaitForExit(3000);

                // Extraer GUID (formato: "... : {guid} ...")
                var match = System.Text.RegularExpressions.Regex.Match(output, @"\{?([0-9a-fA-F\-]{36})\}?");
                string planGuid = match.Success ? match.Groups[1].Value : "8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c"; // High Performance fallback

                // 2. Establecer Processor Idle Disable = 1 (0 = enabled, 1 = disabled)
                RunProcess("powercfg", $"/setacvalueindex {planGuid} {IDLE_SUBGROUP} {IDLE_SETTING} 1");
                RunProcess("powercfg", $"/setdcvalueindex {planGuid} {IDLE_SUBGROUP} {IDLE_SETTING} 1");
                RunProcess("powercfg", "/setactive SCHEME_CURRENT");
                Debug.WriteLine("✓ CPU Idle deshabilitado — C-State 0 forzado. Latencia de respuesta mínima.");
                return true;
            }
            catch (Exception ex) { Debug.WriteLine($"❌ Error CpuIdle Disable: {ex.Message}"); return false; }
        }

        public static bool RevertCpuIdle()
        {
            try
            {
                var psi = new ProcessStartInfo("powercfg", "/getactivescheme")
                {
                    RedirectStandardOutput = true, UseShellExecute = false, CreateNoWindow = true
                };
                using var proc = Process.Start(psi);
                string output = proc?.StandardOutput.ReadToEnd() ?? "";
                proc?.WaitForExit(3000);
                var match = System.Text.RegularExpressions.Regex.Match(output, @"\{?([0-9a-fA-F\-]{36})\}?");
                string planGuid = match.Success ? match.Groups[1].Value : "8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c";

                RunProcess("powercfg", $"/setacvalueindex {planGuid} {IDLE_SUBGROUP} {IDLE_SETTING} 0");
                RunProcess("powercfg", $"/setdcvalueindex {planGuid} {IDLE_SUBGROUP} {IDLE_SETTING} 0");
                RunProcess("powercfg", "/setactive SCHEME_CURRENT");
                Debug.WriteLine("✓ CPU Idle restaurado (C-States habilitados).");
                return true;
            }
            catch (Exception ex) { Debug.WriteLine($"❌ Error CpuIdle Revert: {ex.Message}"); return false; }
        }

        public static bool IsCpuIdleDisabled()
        {
            try
            {
                var psi = new ProcessStartInfo("powercfg", $"/query SCHEME_CURRENT {IDLE_SUBGROUP} {IDLE_SETTING}")
                {
                    RedirectStandardOutput = true, UseShellExecute = false, CreateNoWindow = true
                };
                using var proc = Process.Start(psi);
                string output = proc?.StandardOutput.ReadToEnd() ?? "";
                proc?.WaitForExit(3000);
                // Si contiene "0x00000001" en Current AC Power Setting Index => deshabilitado
                return output.Contains("0x00000001");
            }
            catch { return false; }
        }
        #endregion

        #region 17. NIC Receive/Transmit Buffers 2048 (Zero Packet Loss under Load)
        public static bool SetNicBuffers2048()
        {
            try
            {
                const string NIC_CLASS = @"SYSTEM\CurrentControlSet\Control\Class\{4d36e972-e325-11ce-bfc1-08002be10318}";
                using var nicClass = Registry.LocalMachine.OpenSubKey(NIC_CLASS, true);
                if (nicClass == null) return false;
                int modified = 0;
                foreach (string subKeyName in nicClass.GetSubKeyNames())
                {
                    if (subKeyName.Length != 4 || !subKeyName.All(char.IsDigit)) continue;
                    using var sub = nicClass.OpenSubKey(subKeyName, true);
                    if (sub == null) continue;
                    var desc = sub.GetValue("DriverDesc")?.ToString() ?? "";
                    if (string.IsNullOrEmpty(desc)) continue;
                    // Solo adaptadores físicos (no virtuales/loopback)
                    if (desc.Contains("WAN Miniport") || desc.Contains("Microsoft Kernel") || 
                        desc.Contains("Hyper-V") || desc.Contains("Loopback")) continue;
                    sub.SetValue("*ReceiveBuffers", "2048", RegistryValueKind.String);
                    sub.SetValue("*TransmitBuffers", "2048", RegistryValueKind.String);
                    modified++;
                    Debug.WriteLine($"  ✓ NIC Buffers 2048 en: {desc}");
                }
                Debug.WriteLine($"✓ NIC Buffers 2048 aplicado a {modified} adaptadores.");
                return modified > 0;
            }
            catch (Exception ex) { Debug.WriteLine($"❌ Error NIC Buffers: {ex.Message}"); return false; }
        }

        public static bool RevertNicBuffers()
        {
            try
            {
                const string NIC_CLASS = @"SYSTEM\CurrentControlSet\Control\Class\{4d36e972-e325-11ce-bfc1-08002be10318}";
                using var nicClass = Registry.LocalMachine.OpenSubKey(NIC_CLASS, true);
                if (nicClass == null) return false;
                foreach (string subKeyName in nicClass.GetSubKeyNames())
                {
                    if (subKeyName.Length != 4 || !subKeyName.All(char.IsDigit)) continue;
                    using var sub = nicClass.OpenSubKey(subKeyName, true);
                    sub?.DeleteValue("*ReceiveBuffers", false);
                    sub?.DeleteValue("*TransmitBuffers", false);
                }
                Debug.WriteLine("✓ NIC Buffers restaurados (valores default del driver).");
                return true;
            }
            catch (Exception ex) { Debug.WriteLine($"❌ Error NIC Buffers Revert: {ex.Message}"); return false; }
        }

        public static bool IsNicBuffers2048Set()
        {
            try
            {
                const string NIC_CLASS = @"SYSTEM\CurrentControlSet\Control\Class\{4d36e972-e325-11ce-bfc1-08002be10318}";
                using var nicClass = Registry.LocalMachine.OpenSubKey(NIC_CLASS);
                if (nicClass == null) return false;
                foreach (string subKeyName in nicClass.GetSubKeyNames())
                {
                    if (subKeyName.Length != 4 || !subKeyName.All(char.IsDigit)) continue;
                    using var sub = nicClass.OpenSubKey(subKeyName);
                    if (sub == null) continue;
                    var val = sub.GetValue("*ReceiveBuffers")?.ToString();
                    if (val == "2048") return true;
                }
                return false;
            }
            catch { return false; }
        }
        #endregion

        #region 18. Vulnerable Driver Blocklist Disable (Tools y OC sin lag)
        private const string CI_CONFIG_KEY = @"SYSTEM\CurrentControlSet\Control\CI\Config";

        public static bool DisableVulnerableDriverBlocklist()
        {
            try
            {
                using var key = Registry.LocalMachine.CreateSubKey(CI_CONFIG_KEY);
                key?.SetValue("VulnerableDriverBlocklistEnable", 0, RegistryValueKind.DWord);
                Debug.WriteLine("✓ Vulnerable Driver Blocklist deshabilitado — OC tools y drivers custom sin bloqueo.");
                return true;
            }
            catch (Exception ex) { Debug.WriteLine($"❌ Error VulnerableDriverBlocklist: {ex.Message}"); return false; }
        }

        public static bool RevertVulnerableDriverBlocklist()
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(CI_CONFIG_KEY, true);
                key?.DeleteValue("VulnerableDriverBlocklistEnable", false);
                Debug.WriteLine("✓ Vulnerable Driver Blocklist restaurado (default Windows).");
                return true;
            }
            catch (Exception ex) { Debug.WriteLine($"❌ Error VulnerableDriverBlocklist Revert: {ex.Message}"); return false; }
        }

        public static bool IsVulnerableDriverBlocklistDisabled()
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(CI_CONFIG_KEY);
                var val = key?.GetValue("VulnerableDriverBlocklistEnable");
                return val != null && Convert.ToInt32(val) == 0;
            }
            catch { return false; }
        }
        #endregion

        #region Helper Methods
        private static void RunProcess(string fileName, string arguments)
        {
            try
            {
                var psi = new ProcessStartInfo(fileName, arguments)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden
                };
                using var proc = Process.Start(psi);
                proc?.WaitForExit(3000);
            }
            catch { }
        }

        private static void SetDWordKey(RegistryKey hive, string subKey, string valueName, int value)
        {
            try
            {
                using var key = hive.CreateSubKey(subKey);
                key?.SetValue(valueName, value, RegistryValueKind.DWord);
            }
            catch { }
        }
        #endregion
    }
}
