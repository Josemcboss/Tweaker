using System;
using System.Diagnostics;
using System.Management;
using System.Net.NetworkInformation;

using Microsoft.Win32;
// Removed System.Windows to prevent direct MessageBox.Show calls

namespace Tweaker.Optimizations
{
    /// <summary>
    /// Optimizaciones DNS para gaming - Reduce latencia y mejora velocidad
    /// </summary>
    public static class DnsOptimization
    {
        private const string DnsClientKey = @"SYSTEM\CurrentControlSet\Services\Dnscache\Parameters";

        /// <summary>
        /// Configura DNS Cloudflare (1.1.1.1) en el adaptador de red principal
        /// </summary>
        /// <returns>Mensaje de éxito o error</returns>
        public static string SetCloudflareDns()
        {
            try
            {
                string primaryDns = "1.1.1.1";
                string secondaryDns = "1.0.0.1";

                SetDnsServers(primaryDns, secondaryDns);
                FlushDnsCache();

                return "✅ DNS Cloudflare configurado correctamente\n" +
                       "Primario: 1.1.1.1\n" +
                       "Secundario: 1.0.0.1\n" +
                       "Beneficios: Latencia ultra baja, resolución más rápida, mejor privacidad.";
            }
            catch (Exception ex)
            {
                return $"❌ Error al configurar DNS Cloudflare: {ex.Message}\n" +
                       "Intenta ejecutar como Administrador.";
            }
        }

        /// <summary>
        /// Configura DNS Google (8.8.8.8) en el adaptador de red principal
        /// </summary>
        /// <returns>Mensaje de éxito o error</returns>
        public static string SetGoogleDns()
        {
            try
            {
                string primaryDns = "8.8.8.8";
                string secondaryDns = "8.8.4.4";

                SetDnsServers(primaryDns, secondaryDns);
                FlushDnsCache();

                return "✅ DNS Google configurado correctamente\n" +
                       "Primario: 8.8.8.8\n" +
                       "Secundario: 8.8.4.4\n" +
                       "Beneficios: Confiable y estable, excelente para gaming.";
            }
            catch (Exception ex)
            {
                return $"❌ Error al configurar DNS Google: {ex.Message}\n" +
                       "Intenta ejecutar como Administrador.";
            }
        }

        /// <summary>
        /// Configura servidores DNS en el adaptador de red activo
        /// </summary>
        private static void SetDnsServers(string primaryDns, string secondaryDns)
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            NetworkInterface activeAdapter = null;

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
        /// <returns>Mensaje de éxito o error</returns>
        public static string EnableDnsCacheOptimization()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(DnsClientKey))
                {
                    if (key != null)
                    {
                        key.SetValue("MaxCacheTtl", 86400, RegistryValueKind.DWord);
                        key.SetValue("MaxNegativeCacheTtl", 0, RegistryValueKind.DWord);
                        key.SetValue("NetFailureCacheTime", 0, RegistryValueKind.DWord);
                        key.SetValue("NegativeSOACacheTime", 0, RegistryValueKind.DWord);
                    }
                }

                return "✅ Caché DNS optimizado\n" +
                       "Beneficios: Resolución DNS más rápida, menos consultas a servidores, reducción de latencia.";
            }
            catch (Exception ex)
            {
                return $"❌ Error al optimizar caché DNS: {ex.Message}";
            }
        }

        /// <summary>
        /// Desactiva optimizaciones de caché DNS (restaura valores predeterminados)
        /// </summary>
        /// <returns>Mensaje de éxito o error</returns>
        public static string DisableDnsCacheOptimization()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(DnsClientKey))
                {
                    if (key != null)
                    {
                        key.SetValue("MaxCacheTtl", 86400, RegistryValueKind.DWord);
                        key.SetValue("MaxNegativeCacheTtl", 900, RegistryValueKind.DWord);
                        key.SetValue("NetFailureCacheTime", 30, RegistryValueKind.DWord);
                        key.SetValue("NegativeSOACacheTime", 300, RegistryValueKind.DWord);
                    }
                }

                return "✅ Caché DNS restaurado a valores predeterminados";
            }
            catch (Exception ex)
            {
                return $"❌ Error al restaurar caché DNS: {ex.Message}";
            }
        }

        /// <summary>
        /// Limpia el caché DNS del sistema
        /// </summary>
        public static bool FlushDnsCache()
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
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al limpiar caché DNS: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Desactiva ahorro de energía en adaptadores de red
        /// </summary>
        /// <returns>Mensaje de éxito o error</returns>
        public static string DisableNetworkAdapterPowerSaving()
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

                                subKey.SetValue("*WakeOnMagicPacket", "0", RegistryValueKind.String);
                                subKey.SetValue("*WakeOnPattern", "0", RegistryValueKind.String);
                                subKey.SetValue("EnablePME", "0", RegistryValueKind.String);
                                subKey.SetValue("PnPCapabilities", 24, RegistryValueKind.DWord);

                                count++;
                            }
                        }
                    }
                }

                return $"✅ Ahorro de energía desactivado en {count} adaptador(es)\n" +
                       "Beneficios: Sin micro-desconexiones, latencia más estable, adaptador siempre a máximo rendimiento.";
            }
            catch (Exception ex)
            {
                return $"❌ Error al desactivar ahorro de energía: {ex.Message}";
            }
        }

        /// <summary>
        /// Reactiva ahorro de energía en adaptadores de red
        /// </summary>
        /// <returns>Mensaje de éxito o error</returns>
        public static string EnableNetworkAdapterPowerSaving()
        {
            try
            {
                string regPath = $@"SYSTEM\CurrentControlSet\Control\Class\{{4d36e972-e325-11ce-bfc1-08002be10318}}";

                using (RegistryKey classKey = Registry.LocalMachine.OpenSubKey(regPath))
                {
                    if (classKey == null) return "❌ Error: No se encontró la clave de clase de red.";

                    foreach (string subKeyName in classKey.GetSubKeyNames())
                    {
                        using (RegistryKey subKey = classKey.OpenSubKey(subKeyName, true))
                        {
                            if (subKey == null) continue;

                            subKey.SetValue("*WakeOnMagicPacket", "1", RegistryValueKind.String);
                            subKey.SetValue("*WakeOnPattern", "1", RegistryValueKind.String);
                            subKey.SetValue("EnablePME", "1", RegistryValueKind.String);
                            subKey.SetValue("PnPCapabilities", 0, RegistryValueKind.DWord);
                        }
                    }
                }

                return "✅ Ahorro de energía restaurado. Reinicia el PC.";
            }
            catch (Exception ex)
            {
                return $"❌ Error al restaurar ahorro de energía: {ex.Message}";
            }
        }

        /// <summary>
        /// Deshabilita NetBIOS over TCP/IP en todos los adaptadores
        /// </summary>
        /// <returns>Mensaje de éxito o error</returns>
        public static string DisableNetBios()
        {
            try
            {
                string regPath = @"SYSTEM\CurrentControlSet\Services\NetBT\Parameters\Interfaces";

                using (RegistryKey interfacesKey = Registry.LocalMachine.OpenSubKey(regPath))
                {
                    if (interfacesKey == null)
                    {
                        return "❌ Error: No se encontró la clave de NetBT.";
                    }

                    int count = 0;
                    foreach (string interfaceName in interfacesKey.GetSubKeyNames())
                    {
                        using (RegistryKey interfaceKey = Registry.LocalMachine.OpenSubKey(
                            $@"{regPath}\{interfaceName}", true))
                        {
                            if (interfaceKey != null)
                            {
                                interfaceKey.SetValue("NetbiosOptions", 2, RegistryValueKind.DWord);
                                count++;
                            }
                        }
                    }

                    return $"✅ NetBIOS deshabilitado en {count} adaptador(es)\n" +
                           "Beneficios: Reduce overhead de red, mejora seguridad, libera ancho de banda.";
                }
            }
            catch (Exception ex)
            {
                return $"❌ Error al deshabilitar NetBIOS: {ex.Message}";
            }
        }

        /// <summary>
        /// Habilita NetBIOS over TCP/IP (restaura configuración predeterminada)
        /// </summary>
        /// <returns>Mensaje de éxito o error</returns>
        public static string EnableNetBios()
        {
            try
            {
                string regPath = @"SYSTEM\CurrentControlSet\Services\NetBT\Parameters\Interfaces";

                using (RegistryKey interfacesKey = Registry.LocalMachine.OpenSubKey(regPath))
                {
                    if (interfacesKey == null) return "❌ Error: No se encontró la clave de NetBT.";

                    foreach (string interfaceName in interfacesKey.GetSubKeyNames())
                    {
                        using (RegistryKey interfaceKey = Registry.LocalMachine.OpenSubKey(
                            $@"{regPath}\{interfaceName}", true))
                        {
                            if (interfaceKey != null)
                            {
                                interfaceKey.SetValue("NetbiosOptions", 0, RegistryValueKind.DWord);
                            }
                        }
                    }

                    return "✅ NetBIOS restaurado. Reinicia el PC.";
                }
            }
            catch (Exception ex)
            {
                return $"❌ Error al restaurar NetBIOS: {ex.Message}";
            }
        }
    }
}
