using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// BrowserOptimization - Tweaks balanceados para navegadores y gaming
    /// Corrige lentitud en navegadores manteniendo beneficios para gaming
    /// </summary>
    public static class BrowserOptimization
    {
        private const string TCPIP_INTERFACES = @"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces";
        private const string MULTIMEDIA_PROFILE = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile";
        private const string DNS_CACHE_PARAMS = @"SYSTEM\CurrentControlSet\Services\Dnscache\Parameters";

        /// <summary>
        /// Aplica configuración balanceada para navegadores y gaming
        /// Corrige tweaks extremos que causan lentitud en navegación web
        /// </summary>
        public static bool ApplyBrowserBalancedTweaks()
        {
            try
            {
                bool success = false;
                int interfacesOptimized = 0;

                Debug.WriteLine("?? APLICANDO TWEAKS BALANCEADOS PARA NAVEGADORES");
                Debug.WriteLine("===============================================");

                // 1. Ajustar Network Throttling Index (no completamente deshabilitado)
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(MULTIMEDIA_PROFILE, true))
                {
                    if (key != null)
                    {
                        // Valor 5 = prioridad alta pero balanceada (no extrema como FFFFFFFF)
                        key.SetValue("NetworkThrottlingIndex", 5, RegistryValueKind.DWord);
                        Debug.WriteLine("? NetworkThrottlingIndex ajustado a 5 (balanceado)");
                        success = true;
                    }
                }

                // 2. Optimizar interfaces TCP para navegadores
                using (RegistryKey interfacesKey = Registry.LocalMachine.OpenSubKey(TCPIP_INTERFACES, false))
                {
                    if (interfacesKey != null)
                    {
                        string[] guids = interfacesKey.GetSubKeyNames();

                        foreach (string guid in guids)
                        {
                            using (RegistryKey interfaceKey = Registry.LocalMachine.OpenSubKey(
                                $"{TCPIP_INTERFACES}\\{guid}", true))
                            {
                                if (interfaceKey == null) continue;

                                // Verificar si la interfaz está activa
                                var dhcpIp = interfaceKey.GetValue("DhcpIPAddress")?.ToString();
                                var staticIp = interfaceKey.GetValue("IPAddress");

                                bool isActive = !string.IsNullOrEmpty(dhcpIp) ||
                                              (staticIp != null && staticIp.ToString() != "0.0.0.0");

                                if (isActive)
                                {
                                    // TcpAckFrequency: 2 en lugar de 1 (menos agresivo para navegadores)
                                    interfaceKey.SetValue("TcpAckFrequency", 2, RegistryValueKind.DWord);

                                    // TcpDelAckTicks: 1 en lugar de 0 (reduce overhead)
                                    interfaceKey.SetValue("TcpDelAckTicks", 1, RegistryValueKind.DWord);

                                    // Mantener TCPNoDelay=1 para gaming (Nagle deshabilitado)
                                    interfaceKey.SetValue("TCPNoDelay", 1, RegistryValueKind.DWord);

                                    // TcpWindowSize para mejor throughput en navegadores
                                    interfaceKey.SetValue("TcpWindowSize", 65536, RegistryValueKind.DWord);

                                    // GlobalMaxTcpWindowSize para descargas web
                                    interfaceKey.SetValue("GlobalMaxTcpWindowSize", 65536, RegistryValueKind.DWord);

                                    interfacesOptimized++;
                                    Debug.WriteLine($"? Interfaz {guid.Substring(0, 8)}... optimizada para navegadores");
                                }
                            }
                        }
                    }
                }

                // 3. Optimizar DNS Cache para navegadores (balance entre velocidad y recursos)
                using (RegistryKey dnsKey = Registry.LocalMachine.OpenSubKey(DNS_CACHE_PARAMS, true))
                {
                    if (dnsKey != null)
                    {
                        // Cache positivo alto (bueno para navegadores)
                        dnsKey.SetValue("MaxCacheTtl", 86400, RegistryValueKind.DWord); // 1 día

                        // Cache negativo corto (evita consultas excesivas pero permite retry rápido)
                        dnsKey.SetValue("MaxNegativeCacheTtl", 30, RegistryValueKind.DWord); // 30 segundos

                        // Failure cache corto (retry rápido para sitios caídos temporalmente)
                        dnsKey.SetValue("NetFailureCacheTime", 10, RegistryValueKind.DWord); // 10 segundos

                        // SOA cache corto
                        dnsKey.SetValue("NegativeSOACacheTime", 60, RegistryValueKind.DWord); // 1 minuto

                        Debug.WriteLine("? DNS Cache optimizado para navegadores");
                        success = true;
                    }
                }

                // 4. Flush DNS para aplicar cambios
                bool dnsFlushSuccess = FlushDnsCache();
                if (!dnsFlushSuccess)
                {
                    Debug.WriteLine("? Advertencia: DNS flush falló, pero tweaks se aplicaron");
                }

                Debug.WriteLine($"? {interfacesOptimized} interfaces optimizadas");
                
                if (interfacesOptimized == 0)
                {
                    Debug.WriteLine("? No se encontró ninguna interfaz de red activa para optimizar.");
                    Debug.WriteLine("? Verifica que tu adaptador de red esté conectado y activo.");
                    return false; // No se optimizó nada
                }
                
                Debug.WriteLine("?? Tweaks balanceados aplicados exitosamente");
                Debug.WriteLine("?? Navegadores: Velocidad mejorada");
                Debug.WriteLine("?? Gaming: Rendimiento mantenido");

                return success && interfacesOptimized > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error aplicando tweaks balanceados: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Restaura tweaks extremos para gaming puro (puede causar lentitud en navegadores)
        /// </summary>
        public static bool RestoreGamingOnlyTweaks()
        {
            try
            {
                bool success = false;

                Debug.WriteLine("?? RESTAURANDO TWEAKS EXTREMOS PARA GAMING");
                Debug.WriteLine("==========================================");

                // Network Throttling completamente deshabilitado
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(MULTIMEDIA_PROFILE, true))
                {
                    if (key != null)
                    {
                        key.SetValue("NetworkThrottlingIndex", unchecked((int)0xFFFFFFFF), RegistryValueKind.DWord);
                        Debug.WriteLine("? NetworkThrottlingIndex: FFFFFFFF (extremo)");
                        success = true;
                    }
                }

                // TCP settings extremos para gaming
                using (RegistryKey interfacesKey = Registry.LocalMachine.OpenSubKey(TCPIP_INTERFACES, false))
                {
                    if (interfacesKey != null)
                    {
                        string[] guids = interfacesKey.GetSubKeyNames();

                        foreach (string guid in guids)
                        {
                            using (RegistryKey interfaceKey = Registry.LocalMachine.OpenSubKey(
                                $"{TCPIP_INTERFACES}\\{guid}", true))
                            {
                                if (interfaceKey == null) continue;

                                var dhcpIp = interfaceKey.GetValue("DhcpIPAddress")?.ToString();
                                var staticIp = interfaceKey.GetValue("IPAddress");

                                bool isActive = !string.IsNullOrEmpty(dhcpIp) ||
                                              (staticIp != null && staticIp.ToString() != "0.0.0.0");

                                if (isActive)
                                {
                                    // Valores extremos para gaming
                                    interfaceKey.SetValue("TcpAckFrequency", 1, RegistryValueKind.DWord);
                                    interfaceKey.SetValue("TcpDelAckTicks", 0, RegistryValueKind.DWord);
                                    interfaceKey.SetValue("TCPNoDelay", 1, RegistryValueKind.DWord);

                                    // Remover configuraciones de navegador
                                    try
                                    {
                                        interfaceKey.DeleteValue("TcpWindowSize", false);
                                        interfaceKey.DeleteValue("GlobalMaxTcpWindowSize", false);
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                }

                // DNS Cache sin limitaciones (extremo para gaming)
                using (RegistryKey dnsKey = Registry.LocalMachine.OpenSubKey(DNS_CACHE_PARAMS, true))
                {
                    if (dnsKey != null)
                    {
                        dnsKey.SetValue("MaxNegativeCacheTtl", 0, RegistryValueKind.DWord);
                        dnsKey.SetValue("NetFailureCacheTime", 0, RegistryValueKind.DWord);
                        dnsKey.SetValue("NegativeSOACacheTime", 0, RegistryValueKind.DWord);
                        Debug.WriteLine("? DNS Cache: configuración extrema para gaming");
                    }
                }

                Debug.WriteLine("?? Tweaks extremos para gaming restaurados");
                Debug.WriteLine("?? Navegadores pueden experimentar lentitud");

                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Diagnóstica problemas de navegador
        /// </summary>
        public static string DiagnoseBrowserIssues()
        {
            var issues = new System.Collections.Generic.List<string>();

            try
            {
                // Verificar Network Throttling
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(MULTIMEDIA_PROFILE))
                {
                    if (key != null)
                    {
                        var throttling = key.GetValue("NetworkThrottlingIndex");
                        if (throttling != null && ((int)throttling == -1 || (uint)(int)throttling == 0xFFFFFFFF))
                        {
                            issues.Add("? NetworkThrottlingIndex completamente deshabilitado - causa lentitud en navegadores");
                        }
                    }
                }

                // Verificar TCP settings agresivos
                int aggressiveInterfaces = 0;
                using (RegistryKey interfacesKey = Registry.LocalMachine.OpenSubKey(TCPIP_INTERFACES))
                {
                    if (interfacesKey != null)
                    {
                        foreach (string guid in interfacesKey.GetSubKeyNames())
                        {
                            using (RegistryKey interfaceKey = Registry.LocalMachine.OpenSubKey($"{TCPIP_INTERFACES}\\{guid}"))
                            {
                                if (interfaceKey == null) continue;

                                var ackFreq = interfaceKey.GetValue("TcpAckFrequency");
                                var delTicks = interfaceKey.GetValue("TcpDelAckTicks");

                                if (ackFreq != null && (int)ackFreq == 1 && delTicks != null && (int)delTicks == 0)
                                {
                                    aggressiveInterfaces++;
                                }
                            }
                        }
                    }
                }

                if (aggressiveInterfaces > 0)
                {
                    issues.Add($"? {aggressiveInterfaces} interfaces con configuración TCP muy agresiva");
                }

                // Verificar DNS Cache
                using (RegistryKey dnsKey = Registry.LocalMachine.OpenSubKey(DNS_CACHE_PARAMS))
                {
                    if (dnsKey != null)
                    {
                        var negativeCache = dnsKey.GetValue("MaxNegativeCacheTtl");
                        if (negativeCache != null && (int)negativeCache == 0)
                        {
                            issues.Add("? DNS Negative Cache deshabilitado - consultas DNS excesivas");
                        }
                    }
                }

                if (issues.Count == 0)
                {
                    return "? Configuración balanceada detectada - navegadores deberían funcionar bien";
                }

                return string.Join("\n", issues);
            }
            catch (Exception ex)
            {
                return $"? Error en diagnóstico: {ex.Message}";
            }
        }

        /// <summary>
        /// Limpia DNS Cache
        /// </summary>
        public static bool FlushDnsCache()
        {
            try
            {
                var process = Process.Start(new ProcessStartInfo
                {
                    FileName = "ipconfig",
                    Arguments = "/flushdns",
                    UseShellExecute = false,
                    CreateNoWindow = true
                });

                if (process == null)
                {
                    Debug.WriteLine("? No se pudo iniciar ipconfig /flushdns");
                    return false;
                }

                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    Debug.WriteLine($"? ipconfig /flushdns terminó con código {process.ExitCode}");
                    return false;
                }

                Debug.WriteLine("? DNS Cache limpiado exitosamente");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error limpiando DNS: {ex.Message}");
                return false;
            }
        }
    }
}