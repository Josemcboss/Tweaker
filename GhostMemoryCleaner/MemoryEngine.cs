using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GhostMemoryCleaner
{
    public static class MemoryEngine
    {
        #region Native Structs & Constants

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

        #region P/Invoke

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

        public static void EnablePrivileges()
        {
            EnablePrivilege(SE_INCREASE_QUOTA_NAME);
            EnablePrivilege(SE_PROFILE_SINGLE_PROCESS_NAME);
        }

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

        public static bool PurgeStandbyList()
        {
            try
            {
                EnablePrivileges();

                IntPtr p = Marshal.AllocHGlobal(sizeof(int));
                try
                {
                    Marshal.WriteInt32(p, MemoryPurgeStandbyList);
                    NtSetSystemInformation(SystemMemoryListInformation, p, sizeof(int));

                    Marshal.WriteInt32(p, MemoryEmptyWorkingSets);
                    NtSetSystemInformation(SystemMemoryListInformation, p, sizeof(int));

                    return true;
                }
                finally
                {
                    Marshal.FreeHGlobal(p);
                }
            }
            catch
            {
                return false;
            }
        }

        public static (bool success, long mbCleaned, int processesProcessed) CleanAll()
        {
            try
            {
                var before = GetMemoryStats();
                long availBeforeMB = before.AvailableMB;

                PurgeStandbyList();

                var currentProcess = Process.GetCurrentProcess();
                var currentProcessId = currentProcess.Id;
                int processesProcessed = 0;

                string[] critical =
                {
                    "system", "smss", "csrss", "lsass", "services", "wininit", "winlogon", "dwm", "explorer", "svchost", "ghostmemorycleaner"
                };

                var processes = Process.GetProcesses();
                foreach (var proc in processes)
                {
                    try
                    {
                        if (proc.Id == currentProcessId || proc.Id < 10) continue;
                        string name = proc.ProcessName.ToLower();

                        bool isCrit = false;
                        foreach (var c in critical)
                        {
                            if (name.Contains(c)) { isCrit = true; break; }
                        }
                        if (isCrit) continue;

                        EmptyWorkingSet(proc.Handle);
                        SetProcessWorkingSetSize(proc.Handle, (IntPtr)(-1), (IntPtr)(-1));
                        processesProcessed++;
                    }
                    catch { }
                    finally
                    {
                        proc.Dispose();
                    }
                }

                PurgeStandbyList();
                System.Threading.Thread.Sleep(80);

                var after = GetMemoryStats();
                long availAfterMB = after.AvailableMB;
                long freedMB = Math.Max(0, availAfterMB - availBeforeMB);

                return (true, freedMB, processesProcessed);
            }
            catch (Exception)
            {
                return (false, 0, 0);
            }
        }

        public static (long TotalMB, long AvailableMB, long UsedMB, int PercentUsed) GetMemoryStats()
        {
            try
            {
                var memStatus = new MEMORYSTATUSEX();
                if (GlobalMemoryStatusEx(memStatus))
                {
                    long total = (long)(memStatus.ullTotalPhys / (1024 * 1024));
                    long avail = (long)(memStatus.ullAvailPhys / (1024 * 1024));
                    long used = total - avail;
                    int percent = (int)memStatus.dwMemoryLoad;
                    return (total, avail, used, percent);
                }
            }
            catch { }

            return (16384, 8192, 8192, 50);
        }
    }
}
