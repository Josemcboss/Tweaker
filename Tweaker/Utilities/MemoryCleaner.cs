using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Tweaker.Utilities
{
    /// <summary>
    /// Limpieza avanzada de memoria RAM y caché del sistema.
    /// Purga la Standby List del Kernel, vacía working sets y libera memoria física.
    /// </summary>
    public static class MemoryCleaner
    {
        #region Native Structs & Enums

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;

            public MEMORYSTATUSEX()
            {
                dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct LUID
        {
            public uint LowPart;
            public int HighPart;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct TOKEN_PRIVILEGES
        {
            public uint PrivilegeCount;
            public LUID Luid;
            public uint Attributes;
        }

        private const uint TOKEN_ADJUST_PRIVILEGES = 0x0020;
        private const uint TOKEN_QUERY = 0x0008;
        private const uint SE_PRIVILEGE_ENABLED = 0x00000002;

        private const string SE_INCREASE_QUOTA_NAME = "SeIncreaseQuotaPrivilege";
        private const string SE_PROFILE_SINGLE_PROCESS_NAME = "SeProfileSingleProcessPrivilege";

        private const int SystemMemoryListInformation = 80;
        private const int MemoryPurgeStandbyList = 4;
        private const int MemoryEmptyWorkingSets = 2;

        #endregion

        #region P/Invoke Declarations

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);

        [DllImport("psapi.dll", SetLastError = true)]
        private static extern bool EmptyWorkingSet(IntPtr hProcess);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetProcessWorkingSetSize(IntPtr hProcess, IntPtr dwMinimumWorkingSetSize, IntPtr dwMaximumWorkingSetSize);

        [DllImport("ntdll.dll")]
        private static extern uint NtSetSystemInformation(int SystemInformationClass, IntPtr SystemInformation, int SystemInformationLength);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess, out IntPtr TokenHandle);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool LookupPrivilegeValue(string? lpSystemName, string lpName, out LUID lpLuid);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AdjustTokenPrivileges(IntPtr TokenHandle, [MarshalAs(UnmanagedType.Bool)] bool DisableAllPrivileges, ref TOKEN_PRIVILEGES NewState, uint BufferLength, IntPtr PreviousState, IntPtr ReturnLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);

        #endregion

        #region Privileges Helper

        private static void EnablePrivilege(string privilegeName)
        {
            IntPtr tokenHandle = IntPtr.Zero;
            try
            {
                if (OpenProcessToken(Process.GetCurrentProcess().Handle, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, out tokenHandle))
                {
                    if (LookupPrivilegeValue(null, privilegeName, out LUID luid))
                    {
                        TOKEN_PRIVILEGES tp = new TOKEN_PRIVILEGES
                        {
                            PrivilegeCount = 1,
                            Luid = luid,
                            Attributes = SE_PRIVILEGE_ENABLED
                        };

                        AdjustTokenPrivileges(tokenHandle, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);
                    }
                }
            }
            catch { }
            finally
            {
                if (tokenHandle != IntPtr.Zero)
                {
                    CloseHandle(tokenHandle);
                }
            }
        }

        #endregion

        /// <summary>
        /// Purga la Standby List del Kernel y vacía los Working Sets de sistema.
        /// </summary>
        public static bool PurgeStandbyListAndSystemCache()
        {
            try
            {
                EnablePrivilege(SE_INCREASE_QUOTA_NAME);
                EnablePrivilege(SE_PROFILE_SINGLE_PROCESS_NAME);

                IntPtr p = Marshal.AllocHGlobal(sizeof(int));
                try
                {
                    // 1. Purgar Standby List (Caché de archivos en RAM retenida por Windows)
                    Marshal.WriteInt32(p, MemoryPurgeStandbyList);
                    NtSetSystemInformation(SystemMemoryListInformation, p, sizeof(int));

                    // 2. Vaciar Working Sets del sistema
                    Marshal.WriteInt32(p, MemoryEmptyWorkingSets);
                    NtSetSystemInformation(SystemMemoryListInformation, p, sizeof(int));

                    return true;
                }
                finally
                {
                    Marshal.FreeHGlobal(p);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MemoryCleaner] Error purgando standby: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Limpia memoria de todos los procesos del usuario (excepto críticos y el actual),
        /// y purga la memoria caché y standby del kernel.
        /// </summary>
        /// <returns>Tuple con (éxito, MB liberados, procesos procesados)</returns>
        public static (bool success, long mbCleaned, int processesProcessed) FlushMemory()
        {
            try
            {
                var memBefore = GetSystemMemoryInfo();
                long availBeforeMB = memBefore.availableMB;

                // 1. Purgar Standby List y Cache del Kernel
                PurgeStandbyListAndSystemCache();

                var currentProcess = Process.GetCurrentProcess();
                var currentProcessId = currentProcess.Id;

                int processesProcessed = 0;

                Debug.WriteLine("🧹 [MemoryCleaner] Iniciando optimización profunda...");

                var processes = Process.GetProcesses();

                // Lista de procesos críticos que no se deben interrumpir
                string[] criticalProcesses =
                {
                    "system",
                    "csrss",
                    "smss",
                    "lsass",
                    "services",
                    "winlogon",
                    "dwm",
                    "explorer",
                    "svchost",
                    "audiodg",
                    "conhost",
                    "fontdrvhost",
                    "wininit",
                    "ghost optimizer",
                    "tweaker",
                    "memorycleaner"
                };

                foreach (var process in processes)
                {
                    try
                    {
                        if (process.Id == currentProcessId || process.Id < 10)
                            continue;

                        var processName = process.ProcessName.ToLower();

                        bool isCritical = false;
                        foreach (var critical in criticalProcesses)
                        {
                            if (processName.Contains(critical))
                            {
                                isCritical = true;
                                break;
                            }
                        }

                        if (isCritical)
                            continue;

                        // Intentar vaciar Working Set con EmptyWorkingSet y SetProcessWorkingSetSize
                        bool success = EmptyWorkingSet(process.Handle);
                        SetProcessWorkingSetSize(process.Handle, (IntPtr)(-1), (IntPtr)(-1));

                        if (success)
                        {
                            processesProcessed++;
                        }
                    }
                    catch
                    {
                        // Sin acceso o terminado
                    }
                    finally
                    {
                        process.Dispose();
                    }
                }

                // 2. Segunda pasada a la Standby List para consolidar la memoria liberada por los procesos
                PurgeStandbyListAndSystemCache();

                System.Threading.Thread.Sleep(100);

                var memAfter = GetSystemMemoryInfo();
                long availAfterMB = memAfter.availableMB;

                long totalFreed = Math.Max(0, availAfterMB - availBeforeMB);

                Debug.WriteLine($"✅ [MemoryCleaner] Limpieza completada: {totalFreed} MB liberados en {processesProcessed} procesos.");

                return (true, totalFreed, processesProcessed);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ [MemoryCleaner] Error general: {ex.Message}");
                return (false, 0, 0);
            }
        }

        /// <summary>
        /// Limpia memoria de un proceso específico
        /// </summary>
        public static bool FlushProcessMemory(Process process)
        {
            try
            {
                if (process == null || process.HasExited)
                    return false;

                long memoryBefore = process.WorkingSet64 / 1024 / 1024;

                bool success = EmptyWorkingSet(process.Handle);
                SetProcessWorkingSetSize(process.Handle, (IntPtr)(-1), (IntPtr)(-1));

                if (success)
                {
                    System.Threading.Thread.Sleep(10);
                    process.Refresh();
                    long memoryAfter = process.WorkingSet64 / 1024 / 1024;
                    long freed = memoryBefore - memoryAfter;

                    Debug.WriteLine($"🧹 Proceso {process.ProcessName}: {freed}MB liberados");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error limpiando memoria del proceso: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Obtiene información precisa de memoria del sistema usando GlobalMemoryStatusEx
        /// </summary>
        public static (long totalMB, long availableMB, int percentUsed) GetSystemMemoryInfo()
        {
            try
            {
                var memStatus = new MEMORYSTATUSEX();
                if (GlobalMemoryStatusEx(memStatus))
                {
                    long totalMB = (long)(memStatus.ullTotalPhys / (1024 * 1024));
                    long availableMB = (long)(memStatus.ullAvailPhys / (1024 * 1024));
                    int percentUsed = (int)memStatus.dwMemoryLoad;

                    return (totalMB, availableMB, percentUsed);
                }
            }
            catch { }

            // Fallback
            try
            {
                var pc = new PerformanceCounter("Memory", "Available MBytes");
                long availableMB = (long)pc.NextValue();
                var totalMemory = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes / 1024 / 1024;
                long totalMB = totalMemory > 0 ? totalMemory : 16384;
                int percentUsed = totalMB > 0 ? (int)((totalMB - availableMB) * 100 / totalMB) : 0;
                return (totalMB, availableMB, percentUsed);
            }
            catch
            {
                return (16384, 8192, 50);
            }
        }

        /// <summary>
        /// Limpia memoria agresivamente usando SetProcessWorkingSetSize
        /// </summary>
        public static bool AggressiveMemoryFlush(Process process)
        {
            try
            {
                if (process == null || process.HasExited)
                    return false;

                IntPtr minWorkingSet = new IntPtr(-1);
                IntPtr maxWorkingSet = new IntPtr(-1);

                return SetProcessWorkingSetSize(process.Handle, minWorkingSet, maxWorkingSet);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error en flush agresivo: {ex.Message}");
                return false;
            }
        }
    }
}
