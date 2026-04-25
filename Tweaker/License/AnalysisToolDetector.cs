using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Threading;

using Microsoft.Extensions.DependencyInjection;

namespace Tweaker.License
{
    /// <summary>
    /// Detecta herramientas de análisis, cracking y máquinas virtuales
    /// </summary>
    public static class AnalysisToolDetector
    {
        private static readonly string[] SuspectProcessNames = {
            "x64dbg", "x32dbg", "ollydbg", "ida64", "idaq", "ida", "wireshark",
            "fiddler", "httpdebuggerui", "processhacker", "procexp", "petools",
            "cheatengine", "scylla", "dnspy", "ilspy", "dotpeek", "ghidra"
        };

        private static readonly string[] SuspectFiles = {
            "Scylla.dll", "TitanEngine.dll", "x64dbg.exe", "dnSpy.exe"
        };

        public static bool IsAnalysisToolDetected()
        {
            try
            {
                if (CheckRunningProcesses()) return true;
                if (CheckSuspectFiles()) return true;
                if (CheckVirtualMachine()) return true;
                return false;
            }
            catch (System.Exception ex) { System.Diagnostics.Debug.WriteLine($"[Error] Excepcion capturada: {ex.Message}"); return true; }
        }

        private static bool CheckRunningProcesses()
        {
            try
            {
                var processes = Process.GetProcesses();
                foreach (var process in processes)
                {
                    try
                    {
                        var name = process.ProcessName.ToLower();
                        if (SuspectProcessNames.Any(s => name.Contains(s)))
                        {
                            Debug.WriteLine($"🚨 SEGURIDAD: Proceso sospechoso detectado: {process.ProcessName}");
                            return true;
                        }
                    }
                    catch { continue; }
                }
                return false;
            }
            catch (System.Exception ex) { System.Diagnostics.Debug.WriteLine($"[Error] Excepcion capturada: {ex.Message}"); return false; }
        }

        private static bool CheckSuspectFiles()
        {
            try
            {
                string[] paths = {
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    Path.GetTempPath()
                };

                foreach (var path in paths)
                {
                    foreach (var suspect in SuspectFiles)
                    {
                        if (File.Exists(Path.Combine(path, suspect)))
                        {
                            Debug.WriteLine($"🚨 SEGURIDAD: Archivo sospechoso detectado: {suspect}");
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (System.Exception ex) { System.Diagnostics.Debug.WriteLine($"[Error] Excepcion capturada: {ex.Message}"); return false; }
        }

        private static bool CheckVirtualMachine()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("Select * from Win32_ComputerSystem"))
                {
                    using (var items = searcher.Get())
                    {
                        foreach (var item in items)
                        {
                            string manufacturer = item["Manufacturer"]?.ToString()?.ToLower() ?? "";
                            string model = item["Model"]?.ToString()?.ToUpperInvariant() ?? "";
                            if ((manufacturer == "microsoft corporation" && model.Contains("VIRTUAL"))
                                || manufacturer.Contains("vmware")
                                || manufacturer.Contains("virtualbox"))
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
            catch (System.Exception ex) { System.Diagnostics.Debug.WriteLine($"[Error] Excepcion capturada: {ex.Message}"); return false; }
        }

        internal static void HandleDetection()
        {
            try
            {
                Debug.WriteLine("🚨 SEGURIDAD: Herramientas de análisis detectadas.");
                if (App.ServiceProvider != null)
                {
                    var storage = App.ServiceProvider.GetRequiredService<ILicenseStorage>();
                    storage.DeleteLicense();
                }
                Thread.Sleep(500);
                Environment.FailFast("Entorno de ejecución no seguro.");
            }
            catch (System.Exception ex) { System.Diagnostics.Debug.WriteLine($"[Error] Excepcion capturada: {ex.Message}"); Environment.Exit(-1); }
        }
    }
}
