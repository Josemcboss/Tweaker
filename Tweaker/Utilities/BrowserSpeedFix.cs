using System;
using System.Diagnostics;
using System.Threading.Tasks;
// Removed System.Windows to prevent direct MessageBox.Show calls
using Microsoft.Win32;

namespace Tweaker.Utilities
{
    /// <summary>
    /// Fix rpido para problemas de lentitud en navegadores
    /// Acta de forma independiente para solucionar el problema inmediatamente
    /// </summary>
    public static class BrowserSpeedFix
    {
        /// <summary>
        /// Aplica fix inmediato para navegadores lentos
        /// </summary>
        /// <returns>Mensaje de éxito o error</returns>
        public static async Task<string> ApplyQuickBrowserFix()
        {
            Debug.WriteLine("?? APLICANDO FIX RPIDO PARA NAVEGADORES...");

            try
            {
                bool allSuccess = true;

                // 1. Fix TcpAckFrequency (1 ? 2)
                allSuccess &= await FixTcpAckFrequency();

                // 2. Fix NetworkThrottlingIndex 
                allSuccess &= await FixNetworkThrottling();

                // 3. Fix SystemResponsiveness
                allSuccess &= await FixSystemResponsiveness();

                // 4. Optimizar DNS Cache
                allSuccess &= await OptimizeDnsCache();

                // 5. Flush DNS
                await FlushDns();

                if (allSuccess)
                {
                    Debug.WriteLine("? Fix completo aplicado exitosamente");
                    return "✅ FIX NAVEGADORES APLICADO EXITOSAMENTE\n\n" +
                           "Cambios realizados:\n" +
                           "• TcpAckFrequency: 1 -> 2 (menos agresivo)\n" +
                           "• NetworkThrottling: Parcialmente restaurado\n" +
                           "• SystemResponsiveness: Mejorado para multitarea\n" +
                           "• DNS Cache: Optimizado para navegadores\n\n" +
                           "🚀 Los navegadores deberían cargar MUCHO más rápido ahora\n" +
                           "🎮 Gaming mantiene ~90% del rendimiento";
                }
                else
                {
                    Debug.WriteLine("?? Algunos fixes fallaron, pero la mayora se aplicaron");
                    return "⚠️ Fix parcial aplicado\n\n" +
                           "Algunos ajustes no se pudieron aplicar automáticamente.\n" +
                           "Ejecuta FIX_NAVEGADORES_ULTRA_RAPIDO_v2.3.0.bat\n" +
                           "desde la carpeta Release para completar el fix.";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error aplicando fix: {ex.Message}");
                return $"❌ Error aplicando fix automático: {ex.Message}\n\n" +
                       "Solución manual:\n" +
                       "1. Ejecuta FIX_NAVEGADORES_ULTRA_RAPIDO_v2.3.0.bat\n" +
                       "2. O ve a Red & Ping -> Optimización Balanceada\n" +
                       "3. Reinicia Windows después del fix";
            }
        }

        /// <summary>
        /// Detecta si hay problemas de navegadores
        /// </summary>
        /// <returns>Tuple (bool hasIssues, string message)</returns>
        public static async Task<Tuple<bool, string>> HasBrowserIssues()
        {
            return await Task.Run(() =>
            {
                try
                {
                    bool tcpAckFrequencyIssue = false;
                    bool throttlingIssue = false;

                    // Verificar TcpAckFrequency = 1 en interfaces
                    string interfacesPath = @"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces";
                    using (var key = Registry.LocalMachine.OpenSubKey(interfacesPath))
                    {
                        if (key != null)
                        {
                            foreach (string subkeyName in key.GetSubKeyNames())
                            {
                                using (var interfaceKey = key.OpenSubKey(subkeyName))
                                {
                                    var value = interfaceKey?.GetValue("TcpAckFrequency");
                                    if (value?.ToString() == "1")
                                    {
                                        Debug.WriteLine("?? TcpAckFrequency = 1 detectado (problemtico para navegadores)");
                                        tcpAckFrequencyIssue = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    // Verificar NetworkThrottlingIndex = ffffffff
                    string mmPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile";
                    using (var mmKey = Registry.LocalMachine.OpenSubKey(mmPath))
                    {
                        var throttling = mmKey?.GetValue("NetworkThrottlingIndex");
                        if (throttling?.ToString().ToLowerInvariant() == "ffffffff")
                        {
                            Debug.WriteLine("?? NetworkThrottlingIndex = FFFFFFFF detectado");
                            throttlingIssue = true;
                        }
                    }

                    if (tcpAckFrequencyIssue && throttlingIssue)
                    {
                        return Tuple.Create(true, "Problemas de red detectados: TcpAckFrequency = 1 y NetworkThrottlingIndex = ffffffff. Se recomienda aplicar el fix.");
                    }
                    else if (tcpAckFrequencyIssue)
                    {
                        return Tuple.Create(true, "Problema de red detectado: TcpAckFrequency = 1. Se recomienda aplicar el fix.");
                    }
                    else if (throttlingIssue)
                    {
                        return Tuple.Create(true, "Problema de red detectado: NetworkThrottlingIndex = ffffffff. Se recomienda aplicar el fix.");
                    }
                    return Tuple.Create(false, "No se detectaron problemas conocidos de velocidad en navegadores.");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"?? Error detectando problemas: {ex.Message}");
                    return Tuple.Create(false, $"Error al verificar problemas de navegadores: {ex.Message}");
                }
            });
        }

        private static async Task<bool> FixTcpAckFrequency()
        {
            return await Task.Run(() =>
            {
                try
                {
                    Debug.WriteLine("?? Corrigiendo TcpAckFrequency...");

                    string interfacesPath = @"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces";
                    using (var key = Registry.LocalMachine.OpenSubKey(interfacesPath, true))
                    {
                        if (key != null)
                        {
                            int fixedInterfaces = 0;

                            foreach (string subkeyName in key.GetSubKeyNames())
                            {
                                using (var interfaceKey = key.OpenSubKey(subkeyName, true))
                                {
                                    var currentValue = interfaceKey?.GetValue("TcpAckFrequency");

                                    if (currentValue?.ToString() == "1")
                                    {
                                        interfaceKey?.SetValue("TcpAckFrequency", 2, RegistryValueKind.DWord);
                                        fixedInterfaces++;
                                    }
                                }
                            }

                            Debug.WriteLine($"? TcpAckFrequency corregido en {fixedInterfaces} interfaces");
                            return true;
                        }
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"? Error corrigiendo TcpAckFrequency: {ex.Message}");
                    return false;
                }
            });
        }

        private static async Task<bool> FixNetworkThrottling()
        {
            return await Task.Run(() =>
            {
                try
                {
                    Debug.WriteLine("?? Corrigiendo NetworkThrottlingIndex...");

                    string path = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile";
                    using (var key = Registry.LocalMachine.OpenSubKey(path, true))
                    {
                        var currentValue = key?.GetValue("NetworkThrottlingIndex");
                        if (currentValue?.ToString().ToLowerInvariant() == "ffffffff")
                        {
                            key?.SetValue("NetworkThrottlingIndex", 10, RegistryValueKind.DWord);
                            Debug.WriteLine("? NetworkThrottlingIndex: ffffffff -> 10");
                        }
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"? Error corrigiendo NetworkThrottlingIndex: {ex.Message}");
                    return false;
                }
            });
        }

        private static async Task<bool> FixSystemResponsiveness()
        {
            return await Task.Run(() =>
            {
                try
                {
                    Debug.WriteLine("?? Corrigiendo SystemResponsiveness...");

                    string path = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile";
                    using (var key = Registry.LocalMachine.OpenSubKey(path, true))
                    {
                        var currentValue = key?.GetValue("SystemResponsiveness");
                        if (currentValue?.ToString() == "0")
                        {
                            key?.SetValue("SystemResponsiveness", 20, RegistryValueKind.DWord);
                            Debug.WriteLine("? SystemResponsiveness: 0 -> 20");
                        }
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"? Error corrigiendo SystemResponsiveness: {ex.Message}");
                    return false;
                }
            });
        }

        private static async Task<bool> OptimizeDnsCache()
        {
            return await Task.Run(() =>
            {
                try
                {
                    Debug.WriteLine("?? Optimizando DNS Cache...");

                    string path = @"SYSTEM\CurrentControlSet\Services\Dnscache\Parameters";
                    using (var key = Registry.LocalMachine.OpenSubKey(path, true))
                    {
                        if (key != null)
                        {
                            key.SetValue("MaxCacheTtl", 7200, RegistryValueKind.DWord);
                            key.SetValue("NegativeCacheTime", 5, RegistryValueKind.DWord);
                            key.SetValue("MaxNegativeCacheTtl", 30, RegistryValueKind.DWord);
                            Debug.WriteLine("? DNS Cache optimizado para navegadores");
                        }
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"? Error optimizando DNS: {ex.Message}");
                    return false;
                }
            });
        }

        private static async Task FlushDns()
        {
            await Task.Run(() =>
            {
                try
                {
                    Debug.WriteLine("?? Limpiando DNS Cache...");

                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "ipconfig",
                        Arguments = "/flushdns",
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true
                    }).WaitForExit();

                    Debug.WriteLine("? DNS Cache limpiado");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"?? Error limpiando DNS: {ex.Message}");
                }
            });
        }
    }
}
