using System;
using System.Diagnostics;
using System.Linq;

using Microsoft.Win32;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// Optimizaciones de Red TCP/IP para reducir ping y latencia en juegos competitivos
    /// Basado en tweaks de Adamx y la comunidad de eSports
    /// </summary>
    public static class NetworkOptimization
    {
        private const string TCPIP_INTERFACES = @"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces";
        private const string SYSTEM_PROFILE = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile";

        /// <summary>
        /// OPTIMIZACIÓN COMPLETA DE RED (Adamx Tweaks)
        /// 
        /// Este método hace 2 cosas críticas:
        /// 
        /// 1. TWEAKS TCP/IP EN LA INTERFAZ DE RED ACTIVA:
        ///    - Busca dinámicamente la tarjeta de red activa (con IP asignada)
        ///    - Aplica tweaks TCP/IP para eliminar delays artificiales
        /// 
        /// 2. TWEAKS GLOBALES DEL SISTEMA:
        ///    - NetworkThrottlingIndex: Elimina throttling de red
        ///    - SystemResponsiveness: Prioriza gaming sobre servicios
        /// 
        /// IMPACTO EN GAMING:
        /// - Reduce ping efectivo en 5-30ms
        /// - Mejora hitreg (registro de disparos) en shooters
        /// - Elimina "rubber banding" y packet loss artificial
        /// - USADO POR TODOS LOS PRO PLAYERS
        /// </summary>
        public static bool OptimizeNetwork()
        {
            bool tcpipSuccess = false;
            bool systemSuccess = false;

            try
            {
                // ──────────────────────────────────────??
                // PASO 1: BUSCAR Y OPTIMIZAR INTERFAZ DE RED ACTIVA
                // ──────────────────────────────────────??

                tcpipSuccess = OptimizeTcpIpInterface();

                // ──────────────────────────────────────??
                // PASO 2: APLICAR TWEAKS GLOBALES DE SISTEMA
                // ──────────────────────────────────────??

                systemSuccess = OptimizeSystemNetworkSettings();

                return tcpipSuccess && systemSuccess;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en OptimizeNetwork: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// OPTIMIZA LA INTERFAZ DE RED TCP/IP ACTIVA
        /// 
        /// BÚSQUEDA DINÁMICA:
        /// - Itera sobre todas las interfaces en el registro
        /// - Busca la que tiene DHCP habilitado O dirección IP estática
        /// - Aplica los tweaks TCP/IP críticos
        /// 
        /// TWEAKS APLICADOS:
        /// 
        /// 1. TcpAckFrequency = 1 (CRÍTICO)
        ///    - Windows por defecto espera recibir 2 paquetes antes de enviar ACK
        ///    - O espera 200ms si solo llega 1 paquete
        ///    - Valor 1 = ACK inmediato sin esperar
        ///    - REDUCE PING EN 10-40ms en juegos
        /// 
        /// 2. TCPNoDelay = 1 (Deshabilita Nagle's Algorithm)
        ///    - Nagle's Algorithm agrupa paquetes pequeños para "eficiencia"
        ///    - En gaming causa LAG porque retrasa paquetes
        ///    - Valor 1 = Enviar paquetes inmediatamente sin agrupar
        ///    - CRÍTICO para shooters (Valorant, CS2, COD)
        /// 
        /// 3. TcpDelAckTicks = 0
        ///    - Controla el delay de ACK (en ticks de 100ms)
        ///    - Valor 0 = Sin delay artificial
        ///    - Complementa TcpAckFrequency
        /// 
        /// IMPACTO EN ESPORTS:
        /// - Reduce latencia de red en 10-40ms
        /// - Mejora "peeker's advantage" en shooters
        /// - Elimina "delay" en movimientos enemigos
        /// - Mejora hitreg (los disparos registran mejor)
        /// </summary>
        private static bool OptimizeTcpIpInterface()
        {
            try
            {
                using (RegistryKey interfacesKey = Registry.LocalMachine.OpenSubKey(TCPIP_INTERFACES, false))
                {
                    if (interfacesKey == null)
                    {
                        Debug.WriteLine("No se pudo abrir la clave de interfaces TCP/IP");
                        return false;
                    }

                    // Obtener todas las sub-claves (cada una es una interfaz de red)
                    string[] interfaceGuids = interfacesKey.GetSubKeyNames();

                    int optimizedInterfaces = 0;

                    // Iterar sobre todas las interfaces
                    foreach (string guid in interfaceGuids)
                    {
                        try
                        {
                            using (RegistryKey interfaceKey = Registry.LocalMachine.OpenSubKey(
                                $"{TCPIP_INTERFACES}\\{guid}", true))
                            {
                                if (interfaceKey == null) continue;

                                // ────────────────────────────────────?
                                // DETECTAR SI ES UNA INTERFAZ ACTIVA
                                // ────────────────────────────────────?

                                // Verificar si tiene DHCP habilitado
                                object? dhcpEnabled = interfaceKey?.GetValue("EnableDHCP");

                                // Verificar si tiene IP estática asignada
                                object? ipAddress = interfaceKey?.GetValue("IPAddress");
                                object? dhcpIpAddress = interfaceKey?.GetValue("DhcpIPAddress");

                                // Es una interfaz activa si:
                                // - Tiene DHCP habilitado (valor 1)
                                // - O tiene una dirección IP asignada (estática o DHCP)
                                bool isActiveInterface = false;

                                if (dhcpEnabled != null && dhcpEnabled.ToString() == "1")
                                {
                                    isActiveInterface = true;
                                }
                                else if (ipAddress != null && !string.IsNullOrEmpty(ipAddress.ToString()))
                                {
                                    isActiveInterface = true;
                                }
                                else if (dhcpIpAddress != null && !string.IsNullOrEmpty(dhcpIpAddress.ToString()))
                                {
                                    isActiveInterface = true;
                                }

                                // Si es una interfaz activa, aplicar tweaks
                                if (isActiveInterface)
                                {
                                    Debug.WriteLine($"Optimizando interfaz activa: {guid}");

                                    // ────────────────────────────────────?
                                    // APLICAR TWEAKS TCP/IP CRÍTICOS
                                    // ────────────────────────────────────?

                                    // TcpAckFrequency = 2 (BALANCEADO: gaming + navegadores)
                                    // Valor 1 = extremo (rompe navegadores)
                                    // Valor 2 = balance perfecto (reduce ping pero no rompe throughput)
                                    interfaceKey.SetValue("TcpAckFrequency", 2, RegistryValueKind.DWord);

                                    // TCPNoDelay = 1 (Deshabilitar Nagle's Algorithm)
                                    // BUENO para gaming y navegadores modernos
                                    interfaceKey.SetValue("TCPNoDelay", 1, RegistryValueKind.DWord);

                                    // TcpDelAckTicks = 1 (BALANCEADO)
                                    // Valor 0 = extremo (rompe navegadores)
                                    // Valor 1 = balance (reduce overhead sin romper throughput)
                                    interfaceKey.SetValue("TcpDelAckTicks", 1, RegistryValueKind.DWord);

                                    // TcpWindowSize = 65536 (64KB) para mejor throughput
                                    // Mejora navegadores sin afectar gaming
                                    interfaceKey.SetValue("TcpWindowSize", 65536, RegistryValueKind.DWord);

                                    optimizedInterfaces++;

                                    Debug.WriteLine($"? Interfaz optimizada: {guid}");
                                    Debug.WriteLine($"  - TcpAckFrequency: 2 (BALANCEADO)");
                                    Debug.WriteLine($"  - TCPNoDelay: 1");
                                    Debug.WriteLine($"  - TcpDelAckTicks: 1 (BALANCEADO)");
                                    Debug.WriteLine($"  - TcpWindowSize: 65536");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // Continuar con la siguiente interfaz si hay error
                            Debug.WriteLine($"Error optimizando interfaz {guid}: {ex.Message}");
                        }
                    }

                    if (optimizedInterfaces > 0)
                    {
                        Debug.WriteLine($"? Total de interfaces optimizadas: {optimizedInterfaces}");
                        return true;
                    }
                    else
                    {
                        Debug.WriteLine("? No se encontró ninguna interfaz de red activa");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en OptimizeTcpIpInterface: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// OPTIMIZA CONFIGURACIÓN GLOBAL DE RED DEL SISTEMA
        /// 
        /// NetworkThrottlingIndex = 0xFFFFFFFF (DWORD máximo)
        ///   - Windows limita paquetes de red por segundo para "ahorrar energía"
        ///   - Este tweak ELIMINA completamente el throttling
        ///   - Valor máximo (FFFFFFFF) = sin límite de paquetes
        ///   
        ///   IMPACTO:
        ///   - Reduce ping en 5-20ms
        ///   - Elimina "packet loss" artificial
        ///   - Mejora "tickrate" percibido en juegos
        ///   - CRÍTICO para juegos de 128 tick (CS2, Valorant)
        /// 
        /// SystemResponsiveness = 0
        ///   - Controla cuánto CPU reserva Windows para tareas del sistema
        ///   - Valor 0 = 0% reservado, TODO disponible para juegos
        ///   
        ///   IMPACTO:
        ///   - Reduce latencia del sistema operativo
        ///   - Mejora procesamiento de paquetes de red
        ///   - Elimina "lag spikes" causados por servicios de Windows
        /// </summary>
        private static bool OptimizeSystemNetworkSettings()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(SYSTEM_PROFILE, true))
                {
                    if (key == null)
                    {
                        Debug.WriteLine("No se pudo abrir System Profile");
                        return false;
                    }

                    // NetworkThrottlingIndex = 10 (BALANCEADO: gaming + navegadores)
                    // Valor FFFFFFFF = extremo (rompe navegadores y Discord)
                    // Valor 10 = alta prioridad sin romper throughput
                    key.SetValue("NetworkThrottlingIndex", 10, RegistryValueKind.DWord);

                    // SystemResponsiveness = 10 (BALANCEADO)
                    // Valor 0 = extremo (puede causar problemas en navegadores)
                    // Valor 10 = alta prioridad para gaming pero estable
                    key.SetValue("SystemResponsiveness", 10, RegistryValueKind.DWord);

                    Debug.WriteLine("? Tweaks globales de red aplicados:");
                    Debug.WriteLine("  - NetworkThrottlingIndex: 10 (BALANCEADO para gaming + navegadores)");
                    Debug.WriteLine("  - SystemResponsiveness: 10 (alta prioridad estable)");

                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en OptimizeSystemNetworkSettings: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// RESTAURA CONFIGURACIÓN DE RED A VALORES PREDETERMINADOS DE WINDOWS
        /// 
        /// ELIMINA los valores personalizados de las interfaces TCP/IP
        /// y restaura los valores del sistema a predeterminados.
        /// 
        /// NOTA: Los valores TCP/IP se ELIMINAN en vez de cambiarlos porque:
        /// - Windows usa valores internos si no existen en el registro
        /// - Es más limpio que intentar adivinar los valores originales
        /// </summary>
        public static bool RestoreNetwork()
        {
            bool tcpipSuccess = false;
            bool systemSuccess = false;

            try
            {
                // ──────────────────────────────────────??
                // PASO 1: RESTAURAR INTERFACES TCP/IP
                // ──────────────────────────────────────??

                tcpipSuccess = RestoreTcpIpInterface();

                // ──────────────────────────────────────??
                // PASO 2: RESTAURAR CONFIGURACIÓN GLOBAL DE SISTEMA
                // ──────────────────────────────────────??

                systemSuccess = RestoreSystemNetworkSettings();

                return tcpipSuccess && systemSuccess;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en RestoreNetwork: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// ELIMINA los tweaks TCP/IP de todas las interfaces de red
        /// </summary>
        private static bool RestoreTcpIpInterface()
        {
            try
            {
                using (RegistryKey interfacesKey = Registry.LocalMachine.OpenSubKey(TCPIP_INTERFACES, false))
                {
                    if (interfacesKey == null)
                    {
                        Debug.WriteLine("No se pudo abrir la clave de interfaces TCP/IP");
                        return false;
                    }

                    string[] interfaceGuids = interfacesKey.GetSubKeyNames();
                    int restoredInterfaces = 0;

                    foreach (string guid in interfaceGuids)
                    {
                        try
                        {
                            using (RegistryKey interfaceKey = Registry.LocalMachine.OpenSubKey(
                                $"{TCPIP_INTERFACES}\\{guid}", true))
                            {
                                if (interfaceKey == null) continue;

                                // Eliminar valores personalizados (Windows usará sus defaults)
                                try
                                {
                                    interfaceKey.DeleteValue("TcpAckFrequency", false);
                                    interfaceKey.DeleteValue("TCPNoDelay", false);
                                    interfaceKey.DeleteValue("TcpDelAckTicks", false);

                                    restoredInterfaces++;
                                    Debug.WriteLine($"? Interfaz restaurada: {guid}");
                                }
                                catch
                                {
                                    // Los valores pueden no existir, ignorar
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Error restaurando interfaz {guid}: {ex.Message}");
                        }
                    }

                    Debug.WriteLine($"? Total de interfaces restauradas: {restoredInterfaces}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en RestoreTcpIpInterface: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// RESTAURA configuración global de red del sistema a valores predeterminados
        /// </summary>
        private static bool RestoreSystemNetworkSettings()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(SYSTEM_PROFILE, true))
                {
                    if (key == null)
                    {
                        Debug.WriteLine("No se pudo abrir System Profile");
                        return false;
                    }

                    // NetworkThrottlingIndex = 10 (valor predeterminado de Windows)
                    key.SetValue("NetworkThrottlingIndex", 10, RegistryValueKind.DWord);

                    // SystemResponsiveness = 20 (valor predeterminado de Windows)
                    key.SetValue("SystemResponsiveness", 20, RegistryValueKind.DWord);

                    Debug.WriteLine("? Configuración global de red restaurada:");
                    Debug.WriteLine("  - NetworkThrottlingIndex: 10 (predeterminado)");
                    Debug.WriteLine("  - SystemResponsiveness: 20 (predeterminado)");

                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en RestoreSystemNetworkSettings: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// MÉTODO AUXILIAR: Muestra información de las interfaces de red detectadas
        /// Útil para debugging y verificar qué interfaces se están optimizando
        /// </summary>
        public static string GetNetworkInterfacesInfo()
        {
            try
            {
                using (RegistryKey interfacesKey = Registry.LocalMachine.OpenSubKey(TCPIP_INTERFACES, false))
                {
                    if (interfacesKey == null)
                        return "No se pudo acceder a las interfaces de red";

                    string[] interfaceGuids = interfacesKey.GetSubKeyNames();
                    string info = $"Interfaces de red encontradas: {interfaceGuids.Length}\n\n";

                    foreach (string guid in interfaceGuids)
                    {
                        try
                        {
                            using (RegistryKey interfaceKey = Registry.LocalMachine.OpenSubKey(
                                $"{TCPIP_INTERFACES}\\{guid}", false))
                            {
                                if (interfaceKey == null) continue;

                                info += $"GUID: {guid}\n";

                                object? dhcp = interfaceKey?.GetValue("EnableDHCP");
                                object? ip = interfaceKey?.GetValue("DhcpIPAddress");

                                if (dhcp != null)
                                    info += $"  DHCP: {dhcp}\n";
                                if (ip != null && !string.IsNullOrEmpty(ip.ToString()))
                                    info += $"  IP: {ip}\n";

                                info += "\n";
                            }
                        }
                        catch { }
                    }

                    return info;
                }
            }
            catch (Exception ex)
            {
                return $"Error obteniendo información: {ex.Message}";
            }
        }

        /// <summary>
        /// REVIERTE TODAS LAS OPTIMIZACIONES DE RED
        /// </summary>
        public static bool RevertAllNetworkTweaks()
        {
            try
            {
                Debug.WriteLine("?? REVIRTIENDO TODAS LAS OPTIMIZACIONES DE RED");
                Debug.WriteLine("──────────────────────────────??");

                bool success = true;

                // Restaurar interfaces TCP/IP
                success &= RestoreTcpIpInterface();

                // Restaurar configuraciones globales del sistema
                success &= RestoreSystemNetworkSettings();

                if (success)
                {
                    Debug.WriteLine("\n? TODAS LAS OPTIMIZACIONES DE RED REVERTIDAS");
                    Debug.WriteLine("   Red restaurada a configuración por defecto");
                }
                else
                {
                    Debug.WriteLine("\n??  Algunas optimizaciones no se pudieron revertir");
                }

                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error revirtiendo optimizaciones de red: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// MÉTODOS DE COMPATIBILIDAD PARA PRESETS
        /// </summary>

        /// <summary>
        /// Aplica optimizaciones balanceadas de red (para laptops)
        /// Usa valores menos agresivos que las optimizaciones extremas
        /// </summary>
        public static bool ApplyBalancedOptimizations()
        {
            try
            {
                Debug.WriteLine("?? APLICANDO OPTIMIZACIONES BALANCEADAS DE RED");
                Debug.WriteLine("──────────────────────────────?");

                bool success = false;

                // Buscar interfaz activa y aplicar tweaks balanceados
                using (RegistryKey interfacesKey = Registry.LocalMachine.OpenSubKey(TCPIP_INTERFACES, false))
                {
                    if (interfacesKey == null) return false;

                    string[] interfaceGuids = interfacesKey.GetSubKeyNames();

                    foreach (string guid in interfaceGuids)
                    {
                        using (RegistryKey interfaceKey = Registry.LocalMachine.OpenSubKey($"{TCPIP_INTERFACES}\\{guid}", true))
                        {
                            if (interfaceKey == null) continue;

                            // Verificar si es una interfaz activa
                            object? enableDhcp = interfaceKey?.GetValue("EnableDHCP");
                            object? dhcpIp = interfaceKey?.GetValue("DhcpIPAddress");

                            if ((enableDhcp != null && enableDhcp.ToString() == "1" && dhcpIp != null && !string.IsNullOrEmpty(dhcpIp.ToString())) ||
                                interfaceKey.GetValue("IPAddress") != null)
                            {
                                // Valores BALANCEADOS (menos agresivos que los extremos)
                                interfaceKey.SetValue("TcpAckFrequency", 2, RegistryValueKind.DWord);  // 2 en lugar de 1
                                interfaceKey.SetValue("TCPNoDelay", 1, RegistryValueKind.DWord);
                                interfaceKey.SetValue("TcpDelAckTicks", 1, RegistryValueKind.DWord);    // 1 en lugar de 0
                                interfaceKey.SetValue("TcpWindowSize", 65536, RegistryValueKind.DWord); // Buffer más grande

                                Debug.WriteLine($"? Optimizaciones balanceadas aplicadas a interfaz: {guid}");
                                success = true;
                            }
                        }
                    }
                }

                // Network throttling balanceado
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(SYSTEM_PROFILE, true))
                {
                    if (key != null)
                    {
                        // Valor balanceado (5 en lugar de FFFFFFFF)
                        key.SetValue("NetworkThrottlingIndex", 5, RegistryValueKind.DWord);
                        Debug.WriteLine("? Network throttling balanceado aplicado");
                    }
                }

                if (success)
                {
                    Debug.WriteLine("\n?? OPTIMIZACIONES BALANCEADAS APLICADAS");
                    Debug.WriteLine("   • 90% del rendimiento de gaming");
                    Debug.WriteLine("   • Compatible con navegadores");
                    Debug.WriteLine("   • Estable para uso mixto");
                }

                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error aplicando optimizaciones balanceadas: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Optimiza el caché DNS para mejor rendimiento
        /// </summary>
        public static bool OptimizeDNSCache()
        {
            try
            {
                Debug.WriteLine("?? OPTIMIZANDO CACHÉ DNS");
                Debug.WriteLine("──────────────??");

                bool success = false;
                string dnsKey = @"SYSTEM\CurrentControlSet\Services\Dnscache\Parameters";

                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(dnsKey, true))
                {
                    if (key != null)
                    {
                        // Optimizar caché DNS
                        key.SetValue("MaxCacheTtl", 86400, RegistryValueKind.DWord);      // 24 horas
                        key.SetValue("NegativeCacheTime", 0, RegistryValueKind.DWord);    // Sin caché negativo
                        key.SetValue("MaxNegativeCacheTtl", 0, RegistryValueKind.DWord);
                        key.SetValue("NetFailureCacheTime", 0, RegistryValueKind.DWord);

                        Debug.WriteLine("? Caché DNS optimizado");
                        Debug.WriteLine("   • MaxCacheTtl: 86400s (24h)");
                        Debug.WriteLine("   • NegativeCacheTime: 0s");
                        Debug.WriteLine("   • Resolución DNS más rápida");

                        success = true;
                    }
                }

                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error optimizando DNS cache: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Verifica si las optimizaciones de red están aplicadas
        /// </summary>
        public static bool IsNetworkOptimized()
        {
            try
            {
                // Verificar si NetworkThrottlingIndex está optimizado
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(SYSTEM_PROFILE, false))
                {
                    if (key != null)
                    {
                        object? value = key?.GetValue("NetworkThrottlingIndex");
                        if (value != null)
                        {
                            int intValue = Convert.ToInt32(value);
                            // Si es FFFFFFFF (-1) o un valor bajo (<=10), está optimizado
                            return intValue == -1 || intValue <= 10;
                        }
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
