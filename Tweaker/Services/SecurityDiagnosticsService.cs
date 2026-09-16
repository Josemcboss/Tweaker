using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Tweaker.Models;

namespace Tweaker.Services
{
    public class SecurityDiagnosticsService
    {
        #region Native Netstat (P/Invoke)
        private const int AF_INET = 2; // IPv4

        [DllImport("iphlpapi.dll", SetLastError = true)]
        private static extern uint GetExtendedTcpTable(
            IntPtr pTcpTable,
            ref int pdwSize,
            bool bOrder,
            int ulAf,
            TCP_TABLE_CLASS tableClass,
            uint reserved);

        private enum TCP_TABLE_CLASS
        {
            TCP_TABLE_BASIC_LISTENER,
            TCP_TABLE_BASIC_CONNECTIONS,
            TCP_TABLE_BASIC_ALL,
            TCP_TABLE_OWNER_PID_LISTENER,
            TCP_TABLE_OWNER_PID_CONNECTIONS,
            TCP_TABLE_OWNER_PID_ALL,
            TCP_TABLE_OWNER_MODULE_LISTENER,
            TCP_TABLE_OWNER_MODULE_CONNECTIONS,
            TCP_TABLE_OWNER_MODULE_ALL
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MIB_TCPROW_OWNER_PID
        {
            public uint state;
            public uint localAddr;
            public byte localPort1;
            public byte localPort2;
            public byte localPort3;
            public byte localPort4;
            public uint remoteAddr;
            public byte remotePort1;
            public byte remotePort2;
            public byte remotePort3;
            public byte remotePort4;
            public int owningPid;
        }

        private static string GetTcpStateString(uint state)
        {
            return state switch
            {
                1 => "CLOSED",
                2 => "LISTENING",
                3 => "SYN_SENT",
                4 => "SYN_RCVD",
                5 => "ESTABLISHED",
                6 => "FIN_WAIT1",
                7 => "FIN_WAIT2",
                8 => "CLOSE_WAIT",
                9 => "CLOSING",
                10 => "LAST_ACK",
                11 => "TIME_WAIT",
                12 => "DELETE_TCB",
                _ => $"UNKNOWN ({state})"
            };
        }
        #endregion

        public List<NetworkConnectionItem> GetActiveConnections()
        {
            var results = new List<NetworkConnectionItem>();
            int bufferSize = 0;

            GetExtendedTcpTable(IntPtr.Zero, ref bufferSize, true, AF_INET, TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL, 0);
            IntPtr tcpTablePtr = Marshal.AllocHGlobal(bufferSize);

            try
            {
                uint ret = GetExtendedTcpTable(tcpTablePtr, ref bufferSize, true, AF_INET, TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL, 0);
                if (ret == 0)
                {
                    int rowCount = Marshal.ReadInt32(tcpTablePtr);
                    IntPtr rowPtr = (IntPtr)((long)tcpTablePtr + 4);

                    for (int i = 0; i < rowCount; i++)
                    {
                        var row = Marshal.PtrToStructure<MIB_TCPROW_OWNER_PID>(rowPtr);
                        rowPtr = (IntPtr)((long)rowPtr + Marshal.SizeOf<MIB_TCPROW_OWNER_PID>());

                        ushort localPort = (ushort)((row.localPort1 << 8) + row.localPort2);
                        ushort remotePort = (ushort)((row.remotePort1 << 8) + row.remotePort2);

                        string localIp = new IPAddress(row.localAddr).ToString();
                        string remoteIp = new IPAddress(row.remoteAddr).ToString();

                        string processName = "Unknown";
                        try
                        {
                            if (row.owningPid > 0)
                            {
                                using var p = Process.GetProcessById(row.owningPid);
                                processName = p.ProcessName;
                            }
                            else if (row.owningPid == 0)
                            {
                                processName = "System Idle";
                            }
                        }
                        catch { }

                        results.Add(new NetworkConnectionItem
                        {
                            ProcessId = row.owningPid,
                            ProcessName = processName,
                            Protocol = "TCP",
                            LocalAddress = $"{localIp}:{localPort}",
                            RemoteAddress = $"{remoteIp}:{remotePort}",
                            State = GetTcpStateString(row.state),
                            IsSuspicious = row.state == 5 && (remoteIp.StartsWith("0.") || remotePort == 4444 || remotePort == 1337)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error obteniendo conexiones de red: {ex.Message}");
            }
            finally
            {
                Marshal.FreeHGlobal(tcpTablePtr);
            }

            return results;
        }

        public List<PersistenceItem> GetPersistenceItems()
        {
            var list = new List<PersistenceItem>();

            // HKCU Run
            ScanRegistryRunKey(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Run", "HKCU Run", list);
            // HKLM Run
            ScanRegistryRunKey(Registry.LocalMachine, @"Software\Microsoft\Windows\CurrentVersion\Run", "HKLM Run", list);
            // HKLM RunOnce
            ScanRegistryRunKey(Registry.LocalMachine, @"Software\Microsoft\Windows\CurrentVersion\RunOnce", "HKLM RunOnce", list);

            return list;
        }

        private void ScanRegistryRunKey(RegistryKey root, string subKeyPath, string location, List<PersistenceItem> list)
        {
            try
            {
                using var key = root.OpenSubKey(subKeyPath, false);
                if (key == null) return;

                foreach (var valueName in key.GetValueNames())
                {
                    string rawValue = key.GetValue(valueName)?.ToString() ?? string.Empty;
                    string filePath = CleanExecutablePath(rawValue);

                    bool isMicrosoft = false;
                    if (File.Exists(filePath))
                    {
                        try
                        {
                            var versionInfo = FileVersionInfo.GetVersionInfo(filePath);
                            if (versionInfo.CompanyName != null && versionInfo.CompanyName.Contains("Microsoft", StringComparison.OrdinalIgnoreCase))
                            {
                                isMicrosoft = true;
                            }
                        }
                        catch { }
                    }

                    list.Add(new PersistenceItem
                    {
                        Name = string.IsNullOrEmpty(valueName) ? "(Predeterminado)" : valueName,
                        Path = rawValue,
                        Location = location,
                        IsSignedByMicrosoft = isMicrosoft
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error escaneando registro {subKeyPath}: {ex.Message}");
            }
        }

        private string CleanExecutablePath(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
            string trimmed = raw.Trim();
            if (trimmed.StartsWith("\""))
            {
                int endQuote = trimmed.IndexOf('"', 1);
                if (endQuote > 1) return trimmed.Substring(1, endQuote - 1);
            }
            int spaceIdx = trimmed.IndexOf(' ');
            return spaceIdx > 0 ? trimmed.Substring(0, spaceIdx) : trimmed;
        }

        public (bool IsClean, List<HostsEntry> Entries, string Summary) AnalyzeHostsFile()
        {
            string hostsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), @"drivers\etc\hosts");
            var entries = new List<HostsEntry>();

            if (!File.Exists(hostsPath))
            {
                return (true, entries, "Archivo hosts no encontrado (predeterminado de Windows).");
            }

            try
            {
                string[] lines = File.ReadAllLines(hostsPath);
                int suspiciousCount = 0;

                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i].Trim();
                    if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                        continue;

                    var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2)
                    {
                        string ip = parts[0];
                        string domain = parts[1];

                        // Detección de posibles bloqueos o desvíos sospechosos
                        bool suspicious = domain.Contains("microsoft", StringComparison.OrdinalIgnoreCase) ||
                                          domain.Contains("steam", StringComparison.OrdinalIgnoreCase) ||
                                          domain.Contains("discord", StringComparison.OrdinalIgnoreCase) ||
                                          domain.Contains("antivirus", StringComparison.OrdinalIgnoreCase);

                        if (suspicious) suspiciousCount++;

                        entries.Add(new HostsEntry
                        {
                            LineNumber = i + 1,
                            IP = ip,
                            Domain = domain,
                            IsSuspicious = suspicious,
                            RawLine = line
                        });
                    }
                }

                bool isClean = entries.Count == 0 || (entries.Count <= 2 && suspiciousCount == 0);
                string summary = isClean
                    ? $"Archivo hosts íntegro y seguro ({entries.Count} entradas detectadas)."
                    : $"⚠️ Atención: Se encontraron {entries.Count} entradas personalizadas ({suspiciousCount} potencialmente críticas).";

                return (isClean, entries, summary);
            }
            catch (Exception ex)
            {
                return (false, entries, $"Error al leer hosts: {ex.Message}");
            }
        }

        public bool RestoreCleanHostsFile()
        {
            string hostsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), @"drivers\etc\hosts");
            try
            {
                // Backup previo
                if (File.Exists(hostsPath))
                {
                    string backupPath = hostsPath + $".ghost_backup_{DateTime.Now:yyyyMMddHHmmss}";
                    File.Copy(hostsPath, backupPath, true);
                }

                // Escribir plantilla limpia estándar de Windows
                string cleanTemplate = @"# Copyright (c) 1993-2009 Microsoft Corp.
#
# This is a sample HOSTS file used by Microsoft TCP/IP for Windows.
#
# This file contains the mappings of IP addresses to host names. Each
# entry should be kept on an individual line. The IP address should
# be placed in the first column followed by the corresponding host name.
# The IP address and the host name should be separated by at least one
# space.
#
# localhost name resolution is handled within DNS itself.
#	127.0.0.1       localhost
#	::1             localhost
";
                File.WriteAllText(hostsPath, cleanTemplate);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error restaurando archivo hosts: {ex.Message}");
                return false;
            }
        }
    }
}

