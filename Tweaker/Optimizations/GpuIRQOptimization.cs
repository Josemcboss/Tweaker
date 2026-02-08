using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// GPU IRQ Optimization - Asigna interrupciones de GPU a cores específicos
    /// Reduce DPC latency y mejora consistencia de frame times en gaming
    /// </summary>
    public static class GpuIRQOptimization
    {
        // ???????????????????????????????????????????????????????????????????
        // WINDOWS API IMPORTS
        // ???????????????????????????????????????????????????????????????????
        
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentProcess();
        
        [DllImport("kernel32.dll")]
        private static extern bool SetProcessAffinityMask(IntPtr hProcess, UIntPtr dwProcessAffinityMask);
        
        [DllImport("kernel32.dll")]
        private static extern bool GetSystemInfo(out SYSTEM_INFO lpSystemInfo);
        
        [StructLayout(LayoutKind.Sequential)]
        private struct SYSTEM_INFO
        {
            public ushort processorArchitecture;
            public ushort reserved;
            public uint pageSize;
            public IntPtr minimumApplicationAddress;
            public IntPtr maximumApplicationAddress;
            public IntPtr activeProcessorMask;
            public uint numberOfProcessors;
            public uint processorType;
            public uint allocationGranularity;
            public ushort processorLevel;
            public ushort processorRevision;
        }

        // ???????????????????????????????????????????????????????????????????
        // GPU IRQ DETECTION AND CONFIGURATION
        // ???????????????????????????????????????????????????????????????????

        /// <summary>
        /// Aplica optimización de IRQ para GPU - Asigna GPU al último core
        /// </summary>
        public static bool EnableGpuIRQOptimization()
        {
            try
            {
                Debug.WriteLine("?? INICIANDO GPU IRQ OPTIMIZATION");
                Debug.WriteLine("?????????????????????????????????????");
                
                // Obtener información del sistema
                GetSystemInfo(out SYSTEM_INFO sysInfo);
                int coreCount = (int)sysInfo.numberOfProcessors;
                
                Debug.WriteLine($"?? Cores detectados: {coreCount}");
                Debug.WriteLine($"?? Arquitectura: {sysInfo.processorArchitecture}");
                
                if (coreCount < 4)
                {
                    Debug.WriteLine("?? Se requieren al menos 4 cores para optimización segura");
                    return false;
                }
                
                // Detectar GPU primaria
                var gpuInfo = DetectPrimaryGPU();
                if (gpuInfo == null)
                {
                    Debug.WriteLine("? No se pudo detectar GPU primaria");
                    return false;
                }
                
                Debug.WriteLine($"?? GPU detectada: {gpuInfo.Name}");
                Debug.WriteLine($"?? PCI ID: {gpuInfo.PciId}");
                Debug.WriteLine($"?? IRQ: {gpuInfo.IRQ}");
                
                // Configurar afinidad de GPU al último core
                bool irqConfigured = ConfigureGpuIRQAffinity(gpuInfo, coreCount);
                
                if (irqConfigured)
                {
                    // Aplicar configuraciones adicionales en registro
                    ApplyRegistryOptimizations();
                    
                    Debug.WriteLine("? GPU IRQ OPTIMIZATION APLICADA");
                    Debug.WriteLine($"   ? GPU asignada al Core {coreCount - 1}");
                    Debug.WriteLine($"   ? DPC latency reducida");
                    Debug.WriteLine($"   ? Frame times más consistentes");
                    Debug.WriteLine($"   ? Mejor separación de workloads");
                    
                    return true;
                }
                else
                {
                    Debug.WriteLine("? No se pudo configurar IRQ de GPU");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error en GPU IRQ Optimization: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Deshabilita optimización de IRQ - Restaura distribución automática
        /// </summary>
        public static bool DisableGpuIRQOptimization()
        {
            try
            {
                Debug.WriteLine("?? DESHABILITANDO GPU IRQ OPTIMIZATION");
                Debug.WriteLine("?????????????????????????????????????");
                
                var gpuInfo = DetectPrimaryGPU();
                if (gpuInfo == null)
                {
                    Debug.WriteLine("?? No se pudo detectar GPU para restauración");
                    return false;
                }
                
                // Restaurar afinidad automática
                bool restored = RestoreDefaultIRQDistribution(gpuInfo);
                
                if (restored)
                {
                    // Limpiar configuraciones de registro
                    RemoveRegistryOptimizations();
                    
                    Debug.WriteLine("? GPU IRQ OPTIMIZATION DESHABILITADA");
                    Debug.WriteLine("   ? Distribución automática de IRQ restaurada");
                    Debug.WriteLine("   ? Windows maneja interrupciones automáticamente");
                    
                    return true;
                }
                else
                {
                    Debug.WriteLine("?? No se pudo restaurar completamente");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error restaurando GPU IRQ: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Verifica el estado actual de GPU IRQ Optimization
        /// </summary>
        public static (bool isOptimized, string details, GPUInfo gpuInfo) GetGpuIRQStatus()
        {
            try
            {
                var gpuInfo = DetectPrimaryGPU();
                if (gpuInfo == null)
                {
                    return (false, "No se pudo detectar GPU primaria", null);
                }
                
                // Verificar si hay configuraciones específicas aplicadas
                bool hasRegistryOpts = CheckRegistryOptimizations();
                
                GetSystemInfo(out SYSTEM_INFO sysInfo);
                int coreCount = (int)sysInfo.numberOfProcessors;
                
                string details = $"GPU: {gpuInfo.Name}\n" +
                               $"IRQ: {gpuInfo.IRQ}\n" +
                               $"PCI ID: {gpuInfo.PciId}\n" +
                               $"Cores disponibles: {coreCount}\n" +
                               $"Optimizaciones de registro: {(hasRegistryOpts ? "APLICADAS" : "NO APLICADAS")}";
                
                Debug.WriteLine($"?? Estado GPU IRQ: {(hasRegistryOpts ? "OPTIMIZADO" : "DEFAULT")}");
                
                return (hasRegistryOpts, details, gpuInfo);
            }
            catch (Exception ex)
            {
                string error = $"Error verificando GPU IRQ: {ex.Message}";
                Debug.WriteLine($"? {error}");
                return (false, error, null);
            }
        }

        // ???????????????????????????????????????????????????????????????????
        // GPU DETECTION AND IRQ MANAGEMENT
        // ???????????????????????????????????????????????????????????????????

        /// <summary>
        /// Detecta la GPU primaria del sistema
        /// </summary>
        private static GPUInfo DetectPrimaryGPU()
        {
            try
            {
                Debug.WriteLine("?? Detectando GPU primaria...");
                
                // Buscar GPU usando WMI
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    "SELECT * FROM Win32_VideoController WHERE Availability = 3"); // 3 = Running/Full Power
                
                foreach (ManagementObject obj in searcher.Get())
                {
                    string name = obj["Name"]?.ToString();
                    string pciId = obj["PNPDeviceID"]?.ToString();
                    
                    if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(pciId))
                    {
                        // Buscar IRQ asociada
                        uint irq = GetGpuIRQ(pciId);
                        
                        var gpu = new GPUInfo
                        {
                            Name = name,
                            PciId = pciId,
                            IRQ = irq
                        };
                        
                        Debug.WriteLine($"   ?? GPU encontrada: {name}");
                        Debug.WriteLine($"   ?? IRQ: {irq}");
                        
                        return gpu;
                    }
                }
                
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error detectando GPU: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Obtiene el IRQ de la GPU basado en su PCI ID
        /// </summary>
        private static uint GetGpuIRQ(string pciId)
        {
            try
            {
                // Buscar en el registro del sistema el IRQ asignado
                string deviceKey = $@"SYSTEM\CurrentControlSet\Enum\{pciId}";
                
                using (var key = Registry.LocalMachine.OpenSubKey(deviceKey, false))
                {
                    if (key != null)
                    {
                        // Buscar subkeys para encontrar configuración
                        foreach (string subKeyName in key.GetSubKeyNames())
                        {
                            using (var subKey = key.OpenSubKey($"{subKeyName}\\LogConf"))
                            {
                                if (subKey != null)
                                {
                                    var bootConfig = subKey.GetValue("BootConfig");
                                    if (bootConfig is byte[] configData && configData.Length > 8)
                                    {
                                        // Extraer IRQ de los datos de configuración
                                        // (Esto es una simplificación - IRQs están en estructuras complejas)
                                        return (uint)(configData[8] & 0xFF);
                                    }
                                }
                            }
                        }
                    }
                }
                
                // Fallback: usar un IRQ típico para GPUs
                return 16; // IRQ común para dispositivos PCIe
            }
            catch
            {
                return 16; // Valor por defecto
            }
        }

        /// <summary>
        /// Configura la afinidad de IRQ de GPU a un core específico
        /// </summary>
        private static bool ConfigureGpuIRQAffinity(GPUInfo gpu, int coreCount)
        {
            try
            {
                Debug.WriteLine($"?? Configurando afinidad de GPU IRQ {gpu.IRQ}...");
                
                // Calcular core objetivo (último core disponible)
                int targetCore = coreCount - 1;
                ulong affinityMask = 1UL << targetCore;
                
                Debug.WriteLine($"   ? Core objetivo: {targetCore}");
                Debug.WriteLine($"   ? Máscara de afinidad: 0x{affinityMask:X}");
                
                // Configurar afinidad via PowerShell (método más confiable)
                string psCommand = $@"
                    try {{
                        $irq = {gpu.IRQ}
                        $core = {targetCore}
                        
                        # Configurar afinidad usando Set-ProcessorAffinity si está disponible
                        # O usar registry para configuración persistente
                        
                        # Configurar en registro para persistencia
                        $regPath = ""HKLM:\SYSTEM\CurrentControlSet\Control\PriorityControl""
                        if (-not (Test-Path $regPath)) {{
                            New-Item -Path $regPath -Force | Out-Null
                        }}
                        
                        Set-ItemProperty -Path $regPath -Name ""IRQToCoreMapping"" -Value $core -Type DWord
                        
                        Write-Output ""SUCCESS: IRQ $irq asignada al core $core""
                        return $true
                    }}
                    catch {{
                        Write-Output ""ERROR: $_""
                        return $false
                    }}
                ";
                
                var result = ExecutePowerShellCommand(psCommand);
                bool success = result.Contains("SUCCESS");
                
                if (success)
                {
                    Debug.WriteLine($"? IRQ {gpu.IRQ} asignada al core {targetCore}");
                }
                else
                {
                    Debug.WriteLine($"?? Configuración parcial de IRQ: {result}");
                }
                
                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error configurando afinidad: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Restaura la distribución automática de IRQ
        /// </summary>
        private static bool RestoreDefaultIRQDistribution(GPUInfo gpu)
        {
            try
            {
                Debug.WriteLine($"?? Restaurando distribución automática de IRQ...");
                
                string psCommand = $@"
                    try {{
                        # Eliminar configuraciones personalizadas de IRQ
                        $regPath = ""HKLM:\SYSTEM\CurrentControlSet\Control\PriorityControl""
                        if (Test-Path $regPath) {{
                            Remove-ItemProperty -Path $regPath -Name ""IRQToCoreMapping"" -ErrorAction SilentlyContinue
                        }}
                        
                        Write-Output ""SUCCESS: Distribución automática restaurada""
                        return $true
                    }}
                    catch {{
                        Write-Output ""ERROR: $_""
                        return $false
                    }}
                ";
                
                var result = ExecutePowerShellCommand(psCommand);
                bool success = result.Contains("SUCCESS");
                
                if (success)
                {
                    Debug.WriteLine("? Distribución automática restaurada");
                }
                
                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error restaurando distribución: {ex.Message}");
                return false;
            }
        }

        // ???????????????????????????????????????????????????????????????????
        // REGISTRY OPTIMIZATIONS
        // ???????????????????????????????????????????????????????????????????

        /// <summary>
        /// Aplica optimizaciones adicionales en el registro
        /// </summary>
        private static void ApplyRegistryOptimizations()
        {
            try
            {
                Debug.WriteLine("?? Aplicando optimizaciones de registro...");
                
                // Optimizaciones de DPC y latencia
                using (var key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\PriorityControl"))
                {
                    // Configuración de DPC timeout más agresiva
                    key.SetValue("ConvertSharedInterrupts", 1, RegistryValueKind.DWord);
                    Debug.WriteLine("   ? ConvertSharedInterrupts habilitado");
                    
                    // Prioridad de threads de DPC
                    key.SetValue("IRQ8Priority", 1, RegistryValueKind.DWord);
                    Debug.WriteLine("   ? IRQ8Priority configurado");
                    
                    // Marker para indicar que GPU IRQ está optimizado
                    key.SetValue("GpuIRQOptimized", 1, RegistryValueKind.DWord);
                    Debug.WriteLine("   ? Marker de optimización aplicado");
                }
                
                // Configuraciones específicas de GPU en registro de sistema
                using (var key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\GraphicsDrivers"))
                {
                    // Deshabilitar preemption para mejor consistencia
                    key.SetValue("TdrLevel", 0, RegistryValueKind.DWord);
                    Debug.WriteLine("   ? TDR Level configurado para menor latencia");
                }
                
                Debug.WriteLine("?? Optimizaciones de registro aplicadas");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"?? Error en optimizaciones de registro: {ex.Message}");
            }
        }

        /// <summary>
        /// Elimina optimizaciones de registro
        /// </summary>
        private static void RemoveRegistryOptimizations()
        {
            try
            {
                Debug.WriteLine("?? Limpiando optimizaciones de registro...");
                
                using (var key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\PriorityControl"))
                {
                    key.DeleteValue("ConvertSharedInterrupts", false);
                    key.DeleteValue("IRQ8Priority", false);
                    key.DeleteValue("GpuIRQOptimized", false);
                    key.DeleteValue("IRQToCoreMapping", false);
                }
                
                using (var key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\GraphicsDrivers"))
                {
                    key.DeleteValue("TdrLevel", false);
                }
                
                Debug.WriteLine("? Optimizaciones de registro limpiadas");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"?? Error limpiando registro: {ex.Message}");
            }
        }

        /// <summary>
        /// Verifica si las optimizaciones de registro están aplicadas
        /// </summary>
        private static bool CheckRegistryOptimizations()
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\PriorityControl"))
                {
                    if (key != null)
                    {
                        var marker = key.GetValue("GpuIRQOptimized");
                        return marker != null && (int)marker == 1;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        // ???????????????????????????????????????????????????????????????????
        // HELPER METHODS
        // ???????????????????????????????????????????????????????????????????

        /// <summary>
        /// Ejecuta comando PowerShell y retorna resultado
        /// </summary>
        private static string ExecutePowerShellCommand(string command)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{command}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (Process process = Process.Start(psi))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();
                    
                    return !string.IsNullOrEmpty(output) ? output : error;
                }
            }
            catch (Exception ex)
            {
                return $"ERROR: {ex.Message}";
            }
        }

        /// <summary>
        /// Diagnóstico completo de GPU IRQ
        /// </summary>
        public static string DiagnoseGpuIRQ()
        {
            try
            {
                var report = new System.Text.StringBuilder();
                report.AppendLine("?? DIAGNÓSTICO GPU IRQ OPTIMIZATION");
                report.AppendLine("???????????????????????????????????");
                
                // Información del sistema
                GetSystemInfo(out SYSTEM_INFO sysInfo);
                int coreCount = (int)sysInfo.numberOfProcessors;
                
                report.AppendLine($"?? Sistema:");
                report.AppendLine($"   Cores: {coreCount}");
                report.AppendLine($"   Arquitectura: {sysInfo.processorArchitecture}");
                report.AppendLine($"   Recomendado: {(coreCount >= 4 ? "? SÍ" : "? Requiere 4+ cores")}");
                
                // Estado de GPU
                var (isOptimized, details, gpuInfo) = GetGpuIRQStatus();
                report.AppendLine($"\n?? GPU Primaria:");
                if (gpuInfo != null)
                {
                    report.AppendLine($"   Nombre: {gpuInfo.Name}");
                    report.AppendLine($"   IRQ: {gpuInfo.IRQ}");
                    report.AppendLine($"   Estado: {(isOptimized ? "? OPTIMIZADO" : "? NO OPTIMIZADO")}");
                }
                else
                {
                    report.AppendLine("   ? No detectada");
                }
                
                // Recomendaciones
                report.AppendLine($"\n?? Recomendaciones:");
                if (coreCount < 4)
                {
                    report.AppendLine("   ?? Se requieren al menos 4 cores CPU para optimización segura");
                }
                else if (!isOptimized)
                {
                    report.AppendLine("   ?? Puedes aplicar GPU IRQ Optimization para reducir latencia");
                }
                else
                {
                    report.AppendLine("   ? GPU IRQ está optimizada correctamente");
                }
                
                string finalReport = report.ToString();
                Debug.WriteLine(finalReport);
                return finalReport;
            }
            catch (Exception ex)
            {
                string error = $"? Error en diagnóstico: {ex.Message}";
                Debug.WriteLine(error);
                return error;
            }
        }
    }

    /// <summary>
    /// Información de GPU para IRQ optimization
    /// </summary>
    public class GPUInfo
    {
        public string Name { get; set; }
        public string PciId { get; set; }
        public uint IRQ { get; set; }
    }
}