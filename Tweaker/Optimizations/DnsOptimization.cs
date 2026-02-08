using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Management;
using System.Net.NetworkInformation;
using System.Windows;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// Optimizaciones DNS para gaming - Reduce latencia y mejora velocidad
    /// DaddyGhost Optimizer v2.0
    /// </summary>
    public static class DnsOptimization
    {
        private const string DnsClientKey = @"SYSTEM\CurrentControlSet\Services\Dnscache\Parameters";

        /// <summary>
        /// Configura DNS Cloudflare (1.1.1.1) en el adaptador de red principal
        /// </summary>
        public static void SetCloudflareDns()
        {
            try
            {
                string primaryDns = "1.1.1.1";
                string secondaryDns = "1.0.0.1";

                SetDnsServers(primaryDns, secondaryDns);

                MessageBox.Show(
                    "? DNS Cloudflare configurado correctamente\n\n" +
                    "Primario: 1.1.1.1\n" +
                    "Secundario: 1.0.0.1\n\n" +
                    "Beneficios:\n" +
                    "• Latencia ultra baja (<10ms)\n" +
                    "• Resolución más rápida\n" +
                    "• Mejor privacidad\n\n" +
                    "?? Ejecuta 'ipconfig /flushdns' para aplicar cambios",
                    "DNS Cloudflare",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );

                FlushDnsCache();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"? Error al configurar DNS Cloudflare:\n\n{ex.Message}\n\n" +
                    "Intenta ejecutar como Administrador",
                    "Error DNS",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        /// <summary>
        /// Configura DNS Google (8.8.8.8) en el adaptador de red principal
        /// </summary>
        public static void SetGoogleDns()
        {
            try
            {
                string primaryDns = "8.8.8.8";
                string secondaryDns = "8.8.4.4";

                SetDnsServers(primaryDns, secondaryDns);

                MessageBox.Show(
                    "? DNS Google configurado correctamente\n\n" +
                    "Primario: 8.8.8.8\n" +
                    "Secundario: 8.8.4.4\n\n" +
                    "Beneficios:\n" +
                    "• Confiable y estable\n" +
                    "• Latencia ~15ms\n" +
                    "• Excelente para gaming\n\n" +
                    "?? Ejecuta 'ipconfig /flushdns' para aplicar cambios",
                    "DNS Google",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );

                FlushDnsCache();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"? Error al configurar DNS Google:\n\n{ex.Message}\n\n" +
                    "Intenta ejecutar como Administrador",
                    "Error DNS",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        /// <summary>
        /// Configura servidores DNS en el adaptador de red activo
        /// </summary>
        private static void SetDnsServers(string primaryDns, string secondaryDns)
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            NetworkInterface activeAdapter = null;

            // Buscar adaptador activo (Ethernet o Wi-Fi)
            foreach (NetworkInterface adapter in adapters)
            {
                if (adapter.OperationalStatus == OperationalStatus.Up &&
                    (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                     adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211))
                {
                    activeAdapter = adapter;
                    break;
                }
            }

            if (activeAdapter == null)
            {
                throw new Exception("No se encontró un adaptador de red activo");
            }

            // Configurar DNS usando netsh
            string adapterName = activeAdapter.Name;
            
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "netsh",
                Arguments = $"interface ip set dns \"{adapterName}\" static {primaryDns} primary",
                Verb = "runas",
                UseShellExecute = true,
                CreateNoWindow = true
            };

            Process.Start(psi)?.WaitForExit();

            psi.Arguments = $"interface ip add dns \"{adapterName}\" {secondaryDns} index=2";
            Process.Start(psi)?.WaitForExit();
        }

        /// <summary>
        /// Activa optimizaciones de caché DNS
        /// </summary>
        public static void EnableDnsCacheOptimization()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(DnsClientKey))
                {
                    if (key != null)
                    {
                        // MaxCacheTtl: Tiempo máximo de cache (1 día)
                        key.SetValue("MaxCacheTtl", 86400, RegistryValueKind.DWord);
                        
                        // MaxNegativeCacheTtl: No cachear errores
                        key.SetValue("MaxNegativeCacheTtl", 0, RegistryValueKind.DWord);
                        
                        // NetFailureCacheTime: No cachear fallos de red
                        key.SetValue("NetFailureCacheTime", 0, RegistryValueKind.DWord);
                        
                        // NegativeSOACacheTime: No cachear respuestas negativas SOA
                        key.SetValue("NegativeSOACacheTime", 0, RegistryValueKind.DWord);
                    }
                }

                MessageBox.Show(
                    "? Caché DNS optimizado\n\n" +
                    "Cambios aplicados:\n" +
                    "• MaxCacheTtl: 86400 (1 día)\n" +
                    "• NegativeCacheTime: 0 (sin cache de errores)\n" +
                    "• NetFailureCacheTime: 0\n\n" +
                    "Beneficios:\n" +
                    "• Resolución DNS más rápida\n" +
                    "• Menos consultas a servidores\n" +
                    "• Reducción de latencia\n\n" +
                    "?? Reinicia el servicio DNS Client o el PC",
                    "Caché DNS Optimizado",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"? Error al optimizar caché DNS:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        /// <summary>
        /// Desactiva optimizaciones de caché DNS (restaura valores predeterminados)
        /// </summary>
        public static void DisableDnsCacheOptimization()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(DnsClientKey))
                {
                    if (key != null)
                    {
                        // Restaurar valores predeterminados de Windows
                        key.SetValue("MaxCacheTtl", 86400, RegistryValueKind.DWord);
                        key.SetValue("MaxNegativeCacheTtl", 900, RegistryValueKind.DWord);
                        key.SetValue("NetFailureCacheTime", 30, RegistryValueKind.DWord);
                        key.SetValue("NegativeSOACacheTime", 300, RegistryValueKind.DWord);
                    }
                }

                MessageBox.Show(
                    "? Caché DNS restaurado a valores predeterminados",
                    "Caché DNS",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"? Error al restaurar caché DNS:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        /// <summary>
        /// Limpia el caché DNS del sistema
        /// </summary>
        public static void FlushDnsCache()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "ipconfig",
                    Arguments = "/flushdns",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                };

                Process process = Process.Start(psi);
                process?.WaitForExit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"? Error al limpiar caché DNS:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        /// <summary>
        /// Desactiva ahorro de energía en adaptadores de red
        /// </summary>
        public static void DisableNetworkAdapterPowerSaving()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    "SELECT * FROM Win32_NetworkAdapter WHERE NetEnabled=TRUE"
                );

                int count = 0;
                foreach (ManagementObject adapter in searcher.Get())
                {
                    string deviceId = adapter["PNPDeviceID"]?.ToString();
                    if (string.IsNullOrEmpty(deviceId)) continue;

                    // Buscar en el registro la configuración de ahorro de energía
                    string regPath = $@"SYSTEM\CurrentControlSet\Control\Class\{{4d36e972-e325-11ce-bfc1-08002be10318}}";
                    
                    using (RegistryKey classKey = Registry.LocalMachine.OpenSubKey(regPath))
                    {
                        if (classKey == null) continue;

                        foreach (string subKeyName in classKey.GetSubKeyNames())
                        {
                            using (RegistryKey subKey = classKey.OpenSubKey(subKeyName, true))
                            {
                                if (subKey == null) continue;

                                string matchingDeviceId = subKey.GetValue("MatchingDeviceId")?.ToString();
                                if (matchingDeviceId == null) continue;

                                // Desactivar ahorro de energía
                                subKey.SetValue("*WakeOnMagicPacket", "0", RegistryValueKind.String);
                                subKey.SetValue("*WakeOnPattern", "0", RegistryValueKind.String);
                                subKey.SetValue("EnablePME", "0", RegistryValueKind.String);
                                subKey.SetValue("PnPCapabilities", 24, RegistryValueKind.DWord);
                                
                                count++;
                            }
                        }
                    }
                }

                MessageBox.Show(
                    $"? Ahorro de energía desactivado en {count} adaptador(es)\n\n" +
                    "Beneficios:\n" +
                    "• Sin micro-desconexiones\n" +
                    "• Latencia más estable\n" +
                    "• Sin spikes de ping\n" +
                    "• Adaptador siempre a máximo rendimiento\n\n" +
                    "?? Reinicia el PC para aplicar cambios",
                    "Ahorro de Energía de Red",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"? Error al desactivar ahorro de energía:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        /// <summary>
        /// Reactiva ahorro de energía en adaptadores de red
        /// </summary>
        public static void EnableNetworkAdapterPowerSaving()
        {
            try
            {
                string regPath = $@"SYSTEM\CurrentControlSet\Control\Class\{{4d36e972-e325-11ce-bfc1-08002be10318}}";

                using (RegistryKey classKey = Registry.LocalMachine.OpenSubKey(regPath))
                {
                    if (classKey == null) return;

                    foreach (string subKeyName in classKey.GetSubKeyNames())
                    {
                        using (RegistryKey subKey = classKey.OpenSubKey(subKeyName, true))
                        {
                            if (subKey == null) continue;

                            // Restaurar valores predeterminados
                            subKey.SetValue("*WakeOnMagicPacket", "1", RegistryValueKind.String);
                            subKey.SetValue("*WakeOnPattern", "1", RegistryValueKind.String);
                            subKey.SetValue("EnablePME", "1", RegistryValueKind.String);
                            subKey.SetValue("PnPCapabilities", 0, RegistryValueKind.DWord);
                        }
                    }
                }

                MessageBox.Show(
                    "? Ahorro de energía restaurado\n\n?? Reinicia el PC",
                    "Ahorro de Energía de Red",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"? Error al restaurar ahorro de energía:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        /// <summary>
        /// Deshabilita NetBIOS over TCP/IP en todos los adaptadores
        /// </summary>
        public static void DisableNetBios()
        {
            try
            {
                string regPath = @"SYSTEM\CurrentControlSet\Services\NetBT\Parameters\Interfaces";

                using (RegistryKey interfacesKey = Registry.LocalMachine.OpenSubKey(regPath))
                {
                    if (interfacesKey == null)
                    {
                        throw new Exception("No se encontró la clave de NetBT");
                    }

                    int count = 0;
                    foreach (string interfaceName in interfacesKey.GetSubKeyNames())
                    {
                        using (RegistryKey interfaceKey = Registry.LocalMachine.OpenSubKey(
                            $@"{regPath}\{interfaceName}", true))
                        {
                            if (interfaceKey != null)
                            {
                                // NetbiosOptions: 2 = Disabled
                                interfaceKey.SetValue("NetbiosOptions", 2, RegistryValueKind.DWord);
                                count++;
                            }
                        }
                    }

                    MessageBox.Show(
                        $"? NetBIOS deshabilitado en {count} adaptador(es)\n\n" +
                        "Beneficios:\n" +
                        "• Reduce overhead de red\n" +
                        "• Mejora seguridad\n" +
                        "• Libera ancho de banda\n" +
                        "• Elimina broadcasts innecesarios\n\n" +
                        "?? Reinicia el PC para aplicar cambios",
                        "NetBIOS Deshabilitado",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"? Error al deshabilitar NetBIOS:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        /// <summary>
        /// Habilita NetBIOS over TCP/IP (restaura configuración predeterminada)
        /// </summary>
        public static void EnableNetBios()
        {
            try
            {
                string regPath = @"SYSTEM\CurrentControlSet\Services\NetBT\Parameters\Interfaces";

                using (RegistryKey interfacesKey = Registry.LocalMachine.OpenSubKey(regPath))
                {
                    if (interfacesKey == null) return;

                    foreach (string interfaceName in interfacesKey.GetSubKeyNames())
                    {
                        using (RegistryKey interfaceKey = Registry.LocalMachine.OpenSubKey(
                            $@"{regPath}\{interfaceName}", true))
                        {
                            if (interfaceKey != null)
                            {
                                // NetbiosOptions: 0 = Default (habilitado)
                                interfaceKey.SetValue("NetbiosOptions", 0, RegistryValueKind.DWord);
                            }
                        }
                    }

                    MessageBox.Show(
                        "? NetBIOS restaurado\n\n?? Reinicia el PC",
                        "NetBIOS",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"? Error al restaurar NetBIOS:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
    }
}
