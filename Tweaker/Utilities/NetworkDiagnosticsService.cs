using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

using Microsoft.Win32;

namespace Tweaker.Utilities
{
    /// <summary>
    /// Servicio de diagnóstico automático de problemas de red
    /// Detecta y repara configuraciones problemáticas para navegadores
    /// </summary>
    public class NetworkDiagnosticsService
    {
        private readonly List<NetworkSetting> _problematicSettings = new()
        {
            new("TcpAckFrequency", "1", "2", "HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\Tcpip\\Parameters\\Interfaces"),
            new("NetworkThrottlingIndex", "ffffffff", "10", "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Multimedia\\SystemProfile"),
            new("TCPNoDelay", "1", "0", "HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\Tcpip\\Parameters"),
            new("SystemResponsiveness", "0", "20", "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Multimedia\\SystemProfile")
        };

        /// <summary>
        /// Realiza diagnóstico completo de la configuración de red
        /// </summary>
        public async Task<NetworkDiagnosisResult> PerformFullDiagnosis()
        {
            var result = new NetworkDiagnosisResult();

            Debug.WriteLine("?? Analizando configuración de red...");

            // 1. Verificar configuraciones problemáticas del registro
            await CheckRegistrySettings(result);

            // 2. Test de conectividad básica
            await CheckBasicConnectivity(result);

            // 3. Test de DNS
            await CheckDnsPerformance(result);

            // 4. Análisis de latencia
            await CheckLatencyIssues(result);

            return result;
        }

        /// <summary>
        /// Verifica configuraciones problemáticas en el registro
        /// </summary>
        private async Task CheckRegistrySettings(NetworkDiagnosisResult result)
        {
            await Task.Run(() =>
            {
                foreach (var setting in _problematicSettings)
                {
                    try
                    {
                        if (setting.Key == "TcpAckFrequency")
                        {
                            // Buscar en todas las interfaces de red
                            CheckTcpAckFrequencyAcrossInterfaces(result, setting);
                        }
                        else
                        {
                            var value = RegistryHelper.GetDwordValue(setting.RegistryPath, setting.Key);
                            if (value?.ToString()?.ToLowerInvariant() == setting.ProblematicValue.ToLowerInvariant())
                            {
                                result.CriticalIssues.Add($"{setting.Key} está configurado como {setting.ProblematicValue} (problemático para navegadores)");
                                result.HasCriticalIssues = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"?? Error verificando {setting.Key}: {ex.Message}");
                    }
                }
            });
        }

        /// <summary>
        /// Verifica TcpAckFrequency en todas las interfaces de red
        /// </summary>
        private void CheckTcpAckFrequencyAcrossInterfaces(NetworkDiagnosisResult result, NetworkSetting setting)
        {
            try
            {
                string interfacesPath = @"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces";
                using var key = Registry.LocalMachine.OpenSubKey(interfacesPath);

                if (key != null)
                {
                    int problematicInterfaces = 0;

                    foreach (string subkeyName in key.GetSubKeyNames())
                    {
                        using var interfaceKey = key.OpenSubKey(subkeyName);
                        var value = interfaceKey?.GetValue("TcpAckFrequency");

                        if (value?.ToString() == "1")
                        {
                            problematicInterfaces++;
                        }
                    }

                    if (problematicInterfaces > 0)
                    {
                        result.CriticalIssues.Add($"TcpAckFrequency = 1 en {problematicInterfaces} interface(s) (muy agresivo para navegadores)");
                        result.HasCriticalIssues = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"?? Error verificando TcpAckFrequency: {ex.Message}");
            }
        }

        /// <summary>
        /// Verifica conectividad básica
        /// </summary>
        private async Task CheckBasicConnectivity(NetworkDiagnosisResult result)
        {
            try
            {
                using var ping = new Ping();
                var reply = await ping.SendPingAsync("8.8.8.8", 5000);

                if (reply.Status != IPStatus.Success)
                {
                    result.Warnings.Add("Problemas de conectividad básica detectados");
                }
                else if (reply.RoundtripTime > 200)
                {
                    result.Warnings.Add($"Latencia alta detectada: {reply.RoundtripTime}ms");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"?? Error en test de conectividad: {ex.Message}");
                result.Warnings.Add("No se pudo verificar conectividad");
            }
        }

        /// <summary>
        /// Verifica rendimiento de DNS
        /// </summary>
        private async Task CheckDnsPerformance(NetworkDiagnosisResult result)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                var hostEntry = await System.Net.Dns.GetHostEntryAsync("google.com");
                stopwatch.Stop();

                if (stopwatch.ElapsedMilliseconds > 1000)
                {
                    result.Warnings.Add($"DNS lento: {stopwatch.ElapsedMilliseconds}ms para resolver google.com");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"?? Error en test DNS: {ex.Message}");
                result.Warnings.Add("Problemas de resolución DNS detectados");
            }
        }

        /// <summary>
        /// Analiza problemas de latencia
        /// </summary>
        private async Task CheckLatencyIssues(NetworkDiagnosisResult result)
        {
            try
            {
                // Test múltiple para detectar variaciones de latencia
                var latencies = new List<long>();
                using var ping = new Ping();

                for (int i = 0; i < 5; i++)
                {
                    var reply = await ping.SendPingAsync("1.1.1.1", 3000);
                    if (reply.Status == IPStatus.Success)
                    {
                        latencies.Add(reply.RoundtripTime);
                    }
                    await Task.Delay(500);
                }

                if (latencies.Count >= 3)
                {
                    var avgLatency = latencies.Average();
                    var maxLatency = latencies.Max();
                    var minLatency = latencies.Min();
                    var jitter = maxLatency - minLatency;

                    if (jitter > 50)
                    {
                        result.Warnings.Add($"Alta variación de latencia detectada (jitter: {jitter}ms)");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"?? Error en análisis de latencia: {ex.Message}");
            }
        }

        /// <summary>
        /// Aplica configuración balanceada para resolver problemas de navegadores
        /// </summary>
        public async Task<bool> ApplyBalancedNetworkFix()
        {
            Debug.WriteLine("?? Aplicando configuración de red balanceada...");

            try
            {
                bool allSuccess = true;

                await Task.Run(() =>
                {
                    // 1. TcpAckFrequency: 1 ? 2 (menos agresivo)
                    allSuccess &= FixTcpAckFrequency();

                    // 2. NetworkThrottlingIndex: ffffffff ? 10
                    allSuccess &= FixNetworkThrottling();

                    // 3. SystemResponsiveness: 0 ? 20
                    allSuccess &= FixSystemResponsiveness();

                    // 4. Optimizar configuración DNS para navegadores
                    allSuccess &= OptimizeDnsForBrowsers();
                });

                if (allSuccess)
                {
                    Debug.WriteLine("? Configuración balanceada aplicada exitosamente");
                }
                else
                {
                    Debug.WriteLine("?? Algunos ajustes fallaron, pero la mayoría se aplicaron");
                }

                return allSuccess;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error aplicando fix balanceado: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Corrige TcpAckFrequency en todas las interfaces
        /// </summary>
        private bool FixTcpAckFrequency()
        {
            try
            {
                string interfacesPath = @"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces";
                using var key = Registry.LocalMachine.OpenSubKey(interfacesPath, true);

                if (key != null)
                {
                    foreach (string subkeyName in key.GetSubKeyNames())
                    {
                        using var interfaceKey = key.OpenSubKey(subkeyName, true);
                        var currentValue = interfaceKey?.GetValue("TcpAckFrequency");

                        if (currentValue?.ToString() == "1")
                        {
                            interfaceKey?.SetValue("TcpAckFrequency", 2, RegistryValueKind.DWord);
                            Debug.WriteLine($"  ? TcpAckFrequency: 1 ? 2 en interface {subkeyName}");
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error corrigiendo TcpAckFrequency: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Corrige NetworkThrottlingIndex
        /// </summary>
        private bool FixNetworkThrottling()
        {
            try
            {
                string path = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile";
                using var key = Registry.LocalMachine.OpenSubKey(path, true);

                var currentValue = key?.GetValue("NetworkThrottlingIndex");
                if (currentValue != null && currentValue.ToString().ToLowerInvariant() == "ffffffff")
                {
                    key?.SetValue("NetworkThrottlingIndex", 10, RegistryValueKind.DWord);
                    Debug.WriteLine("  ? NetworkThrottlingIndex: ffffffff ? 10");
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error corrigiendo NetworkThrottlingIndex: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Corrige SystemResponsiveness
        /// </summary>
        private bool FixSystemResponsiveness()
        {
            try
            {
                string path = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile";
                using var key = Registry.LocalMachine.OpenSubKey(path, true);

                var currentValue = key?.GetValue("SystemResponsiveness");
                if (currentValue?.ToString() == "0")
                {
                    key?.SetValue("SystemResponsiveness", 20, RegistryValueKind.DWord);
                    Debug.WriteLine("  ? SystemResponsiveness: 0 ? 20");
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error corrigiendo SystemResponsiveness: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Optimiza DNS específicamente para navegadores
        /// </summary>
        private bool OptimizeDnsForBrowsers()
        {
            try
            {
                string path = @"SYSTEM\CurrentControlSet\Services\Dnscache\Parameters";
                using var key = Registry.LocalMachine.OpenSubKey(path, true);

                // Configuraciones optimizadas para navegadores
                key?.SetValue("MaxCacheTtl", 7200, RegistryValueKind.DWord);  // 2 horas
                key?.SetValue("NegativeCacheTime", 5, RegistryValueKind.DWord);  // 5 segundos
                key?.SetValue("MaxNegativeCacheTtl", 30, RegistryValueKind.DWord);  // 30 segundos

                Debug.WriteLine("  ? Cache DNS optimizado para navegadores");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error optimizando DNS: {ex.Message}");
                return false;
            }
        }
    }

    /// <summary>
    /// Resultado del diagnóstico de red
    /// </summary>
    public class NetworkDiagnosisResult
    {
        public bool HasCriticalIssues { get; set; }
        public List<string> CriticalIssues { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
        public List<string> Recommendations { get; set; } = new();
    }

    /// <summary>
    /// Configuración de red problemática
    /// </summary>
    public class NetworkSetting
    {
        public string Key { get; set; }
        public string ProblematicValue { get; set; }
        public string RecommendedValue { get; set; }
        public string RegistryPath { get; set; }

        public NetworkSetting(string key, string problematicValue, string recommendedValue, string registryPath)
        {
            Key = key;
            ProblematicValue = problematicValue;
            RecommendedValue = recommendedValue;
            RegistryPath = registryPath;
        }
    }

    /// <summary>
    /// Configuración para XAML (no usada por ahora, pero preparada para futuro)
    /// </summary>
    public class NetworkDiagnosticsConfig
    {
        public List<NetworkSetting> ProblematicSettings { get; set; } = new();
    }
}
