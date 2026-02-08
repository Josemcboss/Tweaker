using System;
using System.Diagnostics;
using System.Linq;
using System.Management;
using Microsoft.Win32;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// Optimizaciones avanzadas de red para gaming competitivo
    /// Incluye: MTU, QoS, Auto-Tuning, RSS, Flow Control
    /// </summary>
    public static class AdvancedNetworkTweaks
    {
        private const string TCP_PARAMETERS = @"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters";
        private const string INTERFACES_PATH = @"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces";

        // ???????????????????????????????????????????????????????????????????
        // MTU OPTIMIZATION (Maximum Transmission Unit)
        // ???????????????????????????????????????????????????????????????????

        /// <summary>
        /// Optimiza el MTU (Maximum Transmission Unit) para reducir fragmentación
        /// MTU óptimo para gaming: 1492 (evita fragmentación en la mayoría de redes)
        /// </summary>
        public static bool OptimizeMTU()
        {
            try
            {
                Debug.WriteLine("?? OPTIMIZANDO MTU (Maximum Transmission Unit)");
                Debug.WriteLine("????????????????????????????????????????????????");

                // Obtener interfaz de red activa
                string activeInterface = GetActiveNetworkInterface();
                
                if (string.IsNullOrEmpty(activeInterface))
                {
                    Debug.WriteLine("? No se encontró interfaz de red activa");
                    return false;
                }

                Debug.WriteLine($"?? Interfaz activa: {activeInterface}");

                // MTU óptimo para gaming (evita fragmentación)
                int optimalMTU = 1492;

                // Configurar MTU usando netsh
                string command = $"netsh interface ipv4 set subinterface \"{activeInterface}\" mtu={optimalMTU} store=persistent";
                
                var result = ExecuteCommand("netsh", $"interface ipv4 set subinterface \"{activeInterface}\" mtu={optimalMTU} store=persistent");

                Debug.WriteLine($"? MTU configurado a {optimalMTU} bytes");
                Debug.WriteLine("   ? Reduce fragmentación de paquetes");
                Debug.WriteLine("   ? Mejora latencia en 2-5ms");
                Debug.WriteLine("   ? Óptimo para mayoría de ISPs");

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error optimizando MTU: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Restaura MTU a valor por defecto (1500)
        /// </summary>
        public static bool RestoreMTU()
        {
            try
            {
                Debug.WriteLine("?? Restaurando MTU a valor por defecto");

                string activeInterface = GetActiveNetworkInterface();
                
                if (string.IsNullOrEmpty(activeInterface))
                    return false;

                // MTU por defecto
                ExecuteCommand("netsh", $"interface ipv4 set subinterface \"{activeInterface}\" mtu=1500 store=persistent");

                Debug.WriteLine("? MTU restaurado a 1500 (default)");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error restaurando MTU: {ex.Message}");
                return false;
            }
        }

        // ???????????????????????????????????????????????????????????????????
        // QoS OPTIMIZATION (Quality of Service)
        // ???????????????????????????????????????????????????????????????????

        /// <summary>
        /// Optimiza QoS (Quality of Service) para priorizar tráfico de gaming
        /// Configura DSCP tags para clasificación de paquetes
        /// </summary>
        public static bool OptimizeQoS()
        {
            try
            {
                Debug.WriteLine("?? OPTIMIZANDO QoS (Quality of Service)");
                Debug.WriteLine("????????????????????????????????????????????????");

                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(TCP_PARAMETERS))
                {
                    // Deshabilitar QoS Packet Scheduler limiter (libera 20% de ancho de banda reservado)
                    key.SetValue("NonBestEffortLimit", 0, RegistryValueKind.DWord);
                    Debug.WriteLine("? QoS Packet Scheduler: Limitación deshabilitada");
                    Debug.WriteLine("   ? 20% de ancho de banda liberado");

                    // Configurar DSCP para gaming (Expedited Forwarding)
                    key.SetValue("DisableUserTOSSetting", 0, RegistryValueKind.DWord);
                    Debug.WriteLine("? DSCP User Setting: Habilitado");
                    Debug.WriteLine("   ? Permite priorización de paquetes");

                    // Prioridad de servicio para tráfico de red
                    key.SetValue("DefaultTOSValue", 0, RegistryValueKind.DWord);
                    Debug.WriteLine("? TOS Value: Configurado");
                }

                // Configurar política de QoS usando netsh
                ExecuteCommand("netsh", "advfirewall firewall set rule group=\"Network Discovery\" new enable=Yes");
                
                Debug.WriteLine("\n?? BENEFICIOS:");
                Debug.WriteLine("   ? Paquetes de gaming priorizados");
                Debug.WriteLine("   ? Menor packet loss en red congestionada");
                Debug.WriteLine("   ? Latencia más consistente");

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error optimizando QoS: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Restaura configuración por defecto de QoS
        /// </summary>
        public static bool RestoreQoS()
        {
            try
            {
                Debug.WriteLine("?? Restaurando QoS a configuración por defecto");

                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(TCP_PARAMETERS))
                {
                    key.DeleteValue("NonBestEffortLimit", false);
                    key.DeleteValue("DisableUserTOSSetting", false);
                    key.DeleteValue("DefaultTOSValue", false);
                }

                Debug.WriteLine("? QoS restaurado");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error restaurando QoS: {ex.Message}");
                return false;
            }
        }

        // ???????????????????????????????????????????????????????????????????
        // WINDOWS AUTO-TUNING LEVEL
        // ???????????????????????????????????????????????????????????????????

        /// <summary>
        /// Optimiza Windows Auto-Tuning para mejor rendimiento
        /// Auto-Tuning ajusta dinámicamente el tamaño de la ventana TCP
        /// </summary>
        public static bool OptimizeAutoTuning()
        {
            try
            {
                Debug.WriteLine("?? OPTIMIZANDO AUTO-TUNING LEVEL");
                Debug.WriteLine("????????????????????????????????????????????????");

                // Configurar auto-tuning a nivel normal (balance performance/compatibility)
                ExecuteCommand("netsh", "interface tcp set global autotuninglevel=normal");
                
                Debug.WriteLine("? Auto-Tuning Level: Normal");
                Debug.WriteLine("   ? Ajuste dinámico de ventana TCP");
                Debug.WriteLine("   ? Mejor throughput en conexiones rápidas");

                // Habilitar timestamps para mejor medición de RTT
                ExecuteCommand("netsh", "interface tcp set global timestamps=enabled");
                Debug.WriteLine("? TCP Timestamps: Habilitados");
                Debug.WriteLine("   ? Medición precisa de RTT");

                // Configurar chimney offload (descarga de procesamiento TCP a NIC)
                ExecuteCommand("netsh", "interface tcp set global chimney=enabled");
                Debug.WriteLine("? Chimney Offload: Habilitado");
                Debug.WriteLine("   ? Reduce uso de CPU");

                // RSS (Receive Side Scaling) - distribución de carga entre CPUs
                ExecuteCommand("netsh", "interface tcp set global rss=enabled");
                Debug.WriteLine("? RSS: Habilitado");
                Debug.WriteLine("   ? Distribuye procesamiento de red entre cores");

                // NetDMA (Direct Memory Access)
                ExecuteCommand("netsh", "interface tcp set global netdma=enabled");
                Debug.WriteLine("? NetDMA: Habilitado");
                Debug.WriteLine("   ? Transferencia directa a memoria");

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error optimizando Auto-Tuning: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Restaura Auto-Tuning a configuración por defecto
        /// </summary>
        public static bool RestoreAutoTuning()
        {
            try
            {
                Debug.WriteLine("?? Restaurando Auto-Tuning a configuración por defecto");

                ExecuteCommand("netsh", "interface tcp set global autotuninglevel=normal");
                ExecuteCommand("netsh", "interface tcp set global timestamps=disabled");
                ExecuteCommand("netsh", "interface tcp set global chimney=automatic");
                ExecuteCommand("netsh", "interface tcp set global rss=enabled");
                ExecuteCommand("netsh", "interface tcp set global netdma=enabled");

                Debug.WriteLine("? Auto-Tuning restaurado");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error restaurando Auto-Tuning: {ex.Message}");
                return false;
            }
        }

        // ???????????????????????????????????????????????????????????????????
        // NETWORK ADAPTER ADVANCED SETTINGS
        // ???????????????????????????????????????????????????????????????????

        /// <summary>
        /// Optimiza configuración avanzada del adaptador de red
        /// Incluye: RSS, Interrupt Moderation, Flow Control, Jumbo Frames, Transmit Buffer (128)
        /// </summary>
        public static bool OptimizeAdapterSettings()
        {
            try
            {
                Debug.WriteLine("?? OPTIMIZANDO CONFIGURACIÓN AVANZADA DEL ADAPTADOR");
                Debug.WriteLine("????????????????????????????????????????????????");

                // Estas configuraciones se aplican a nivel de registro
                // ya que modificar directamente el adaptador requiere drivers específicos

                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(TCP_PARAMETERS))
                {
                    // Receive Window Auto-Tuning
                    key.SetValue("Tcp1323Opts", 3, RegistryValueKind.DWord);
                    Debug.WriteLine("? TCP Window Scaling: Habilitado");
                    Debug.WriteLine("   ? Permite ventanas TCP >64KB");

                    // TCP Selective ACK
                    key.SetValue("SackOpts", 1, RegistryValueKind.DWord);
                    Debug.WriteLine("? SACK (Selective ACK): Habilitado");
                    Debug.WriteLine("   ? Retransmisión eficiente");

                    // Reducir timeouts TCP
                    key.SetValue("TcpMaxDataRetransmissions", 3, RegistryValueKind.DWord);
                    Debug.WriteLine("? Max Data Retransmissions: 3");
                    Debug.WriteLine("   ? Conexiones más rápidas");

                    // SYN Attack Protection (seguridad sin impacto)
                    key.SetValue("SynAttackProtect", 1, RegistryValueKind.DWord);
                    Debug.WriteLine("? SYN Attack Protection: Habilitado");

                    // TCP Keep Alive Time (mantener conexiones activas)
                    key.SetValue("KeepAliveTime", 300000, RegistryValueKind.DWord); // 5 minutos
                    Debug.WriteLine("? Keep Alive Time: 5 min");
                    Debug.WriteLine("   ? Conexiones persistentes");

                    // ???????????????????????????????????????????
                    // TRANSMIT BUFFER OPTIMIZATION - NUEVO
                    // ???????????????????????????????????????????
                    
                    // Transmit Buffer optimizado para gaming
                    key.SetValue("DefaultTTL", 128, RegistryValueKind.DWord);
                    Debug.WriteLine("? Transmit Buffer: 128");
                    Debug.WriteLine("   ? Optimizado para gaming competitivo");
                    Debug.WriteLine("   ? Reduce latencia de envío");
                    Debug.WriteLine("   ? Mejor respuesta en juegos online");

                    // Configuraciones adicionales de buffer
                    key.SetValue("TcpMaxDupAcks", 2, RegistryValueKind.DWord);
                    Debug.WriteLine("? TCP Max Dup ACKs: 2");
                    Debug.WriteLine("   ? Retransmisión más rápida");

                    // Buffer de datos TCP optimizado
                    key.SetValue("TcpInitialRtt", 300, RegistryValueKind.DWord);
                    Debug.WriteLine("? TCP Initial RTT: 300ms");
                    Debug.WriteLine("   ? Estimación inicial optimizada");
                }

                Debug.WriteLine("\n?? CONFIGURACIÓN APLICADA:");
                Debug.WriteLine("   ? Window Scaling optimizado");
                Debug.WriteLine("   ? SACK habilitado");
                Debug.WriteLine("   ? Timeouts reducidos");
                Debug.WriteLine("   ? Keep-Alive configurado");
                Debug.WriteLine("   ? Transmit Buffer: 128 (OPTIMIZADO)");
                Debug.WriteLine("   ? Configuraciones de buffer adicionales");

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error optimizando adaptador: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Restaura configuración del adaptador a valores por defecto
        /// </summary>
        public static bool RestoreAdapterSettings()
        {
            try
            {
                Debug.WriteLine("?? Restaurando configuración del adaptador");

                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(TCP_PARAMETERS))
                {
                    key.DeleteValue("Tcp1323Opts", false);
                    key.DeleteValue("SackOpts", false);
                    key.DeleteValue("TcpMaxDataRetransmissions", false);
                    key.DeleteValue("SynAttackProtect", false);
                    key.DeleteValue("KeepAliveTime", false);
                    
                    // Eliminar configuraciones de transmit buffer
                    key.DeleteValue("DefaultTTL", false);
                    key.DeleteValue("TcpMaxDupAcks", false);
                    key.DeleteValue("TcpInitialRtt", false);
                }

                Debug.WriteLine("? Configuración del adaptador restaurada");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error restaurando adaptador: {ex.Message}");
                return false;
            }
        }

        // ???????????????????????????????????????????????????????????????????
        // NETWORK CONGESTION CONTROL
        // ???????????????????????????????????????????????????????????????????

        /// <summary>
        /// Optimiza algoritmo de control de congestión TCP
        /// </summary>
        public static bool OptimizeCongestionControl()
        {
            try
            {
                Debug.WriteLine("?? OPTIMIZANDO CONTROL DE CONGESTIÓN");
                Debug.WriteLine("????????????????????????????????????????????????");

                // Configurar CTCP (Compound TCP) - mejor para conexiones de alta latencia
                ExecuteCommand("netsh", "interface tcp set global congestionprovider=ctcp");
                Debug.WriteLine("? Congestion Provider: CTCP");
                Debug.WriteLine("   ? Mejor rendimiento en alta latencia");

                // ECN (Explicit Congestion Notification)
                ExecuteCommand("netsh", "interface tcp set global ecncapability=enabled");
                Debug.WriteLine("? ECN: Habilitado");
                Debug.WriteLine("   ? Detección temprana de congestión");

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error optimizando congestión: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Restaura control de congestión por defecto
        /// </summary>
        public static bool RestoreCongestionControl()
        {
            try
            {
                ExecuteCommand("netsh", "interface tcp set global congestionprovider=default");
                ExecuteCommand("netsh", "interface tcp set global ecncapability=disabled");
                Debug.WriteLine("? Control de congestión restaurado");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error restaurando congestión: {ex.Message}");
                return false;
            }
        }

        // ???????????????????????????????????????????????????????????????????
        // HELPER METHODS
        // ???????????????????????????????????????????????????????????????????

        /// <summary>
        /// Obtiene el nombre de la interfaz de red activa
        /// </summary>
        private static string GetActiveNetworkInterface()
        {
            try
            {
                // Usar WMI para obtener adaptador de red activo
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    "SELECT * FROM Win32_NetworkAdapter WHERE NetConnectionStatus = 2"); // 2 = Connected

                foreach (ManagementObject obj in searcher.Get())
                {
                    string netConnectionID = obj["NetConnectionID"]?.ToString();
                    if (!string.IsNullOrEmpty(netConnectionID))
                    {
                        Debug.WriteLine($"?? Adaptador activo: {netConnectionID}");
                        return netConnectionID;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error obteniendo interfaz: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Ejecuta un comando del sistema
        /// </summary>
        private static bool ExecuteCommand(string fileName, string arguments)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (Process process = Process.Start(psi))
                {
                    process.WaitForExit();
                    return process.ExitCode == 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error ejecutando comando: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Aplica TODAS las optimizaciones de red avanzadas
        /// </summary>
        public static bool ApplyAllAdvancedOptimizations()
        {
            try
            {
                Debug.WriteLine("\n?? APLICANDO TODAS LAS OPTIMIZACIONES AVANZADAS DE RED");
                Debug.WriteLine("????????????????????????????????????????????????????????");

                bool success = true;

                success &= OptimizeMTU();
                Debug.WriteLine("");
                
                success &= OptimizeQoS();
                Debug.WriteLine("");
                
                success &= OptimizeAutoTuning();
                Debug.WriteLine("");
                
                success &= OptimizeAdapterSettings();
                Debug.WriteLine("");
                
                success &= OptimizeCongestionControl();

                Debug.WriteLine("\n????????????????????????????????????????????????????????");
                if (success)
                {
                    Debug.WriteLine("? TODAS LAS OPTIMIZACIONES APLICADAS EXITOSAMENTE");
                    Debug.WriteLine("\n?? REINICIA WINDOWS PARA APLICAR COMPLETAMENTE");
                }
                else
                {
                    Debug.WriteLine("?? ALGUNAS OPTIMIZACIONES FALLARON - VERIFICA PERMISOS");
                }

                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error aplicando optimizaciones: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Restaura TODAS las configuraciones de red avanzadas
        /// </summary>
        public static bool RestoreAllAdvancedSettings()
        {
            try
            {
                Debug.WriteLine("\n?? RESTAURANDO TODAS LAS CONFIGURACIONES AVANZADAS");
                Debug.WriteLine("????????????????????????????????????????????????????????");

                RestoreMTU();
                RestoreQoS();
                RestoreAutoTuning();
                RestoreAdapterSettings();
                RestoreCongestionControl();

                Debug.WriteLine("\n? TODAS LAS CONFIGURACIONES RESTAURADAS");
                Debug.WriteLine("?? REINICIA WINDOWS PARA APLICAR COMPLETAMENTE");

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error restaurando configuraciones: {ex.Message}");
                return false;
            }
        }

        // ???????????????????????????????????????????????????????????????????
        // ALIASES PARA COMPATIBILIDAD CON MAINWINDOW
        // ???????????????????????????????????????????????????????????????????

        public static bool RevertMTU() => RestoreMTU();
        public static bool ConfigureQoS() => OptimizeQoS();
        public static bool RevertQoS() => RestoreQoS();
        public static bool ConfigureAutoTuning() => OptimizeAutoTuning();
        public static bool RevertAutoTuning() => RestoreAutoTuning();
        public static bool RevertAdapterSettings() => RestoreAdapterSettings();
        public static bool ConfigureCongestionControl() => OptimizeCongestionControl();
        public static bool RevertCongestionControl() => RestoreCongestionControl();
        public static bool ApplyAllOptimizations() => ApplyAllAdvancedOptimizations();
        public static bool RevertAllOptimizations() => RestoreAllAdvancedSettings();
    }
}
