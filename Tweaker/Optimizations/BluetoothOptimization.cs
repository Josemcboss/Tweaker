using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.ServiceProcess;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// Bluetooth Optimization - Diagnostics and health check
    /// </summary>
    public static class BluetoothOptimization
    {
        /// <summary>
        /// Verifica si los servicios de Bluetooth están funcionando correctamente
        /// </summary>
        public static bool IsBluetoothHealthy()
        {
            try
            {
                bool bluetoothSupport = false;
                bool bluetoothUserService = false;
                bool bluetoothAudioGateway = false;

                // Verificar servicios críticos de Bluetooth
                string[] bluetoothServices = {
                    "bthserv",          // Bluetooth Support Service
                    "BluetoothUserService", // Bluetooth User Support Service  
                    "BthAvctpSvc"       // AVCTP service
                };

                foreach (string serviceName in bluetoothServices)
                {
                    try
                    {
                        using (ServiceController service = new ServiceController(serviceName))
                        {
                            if (service.Status == ServiceControllerStatus.Running)
                            {
                                switch (serviceName)
                                {
                                    case "bthserv":
                                        bluetoothSupport = true;
                                        break;
                                    case "BluetoothUserService":
                                        bluetoothUserService = true;
                                        break;
                                    case "BthAvctpSvc":
                                        bluetoothAudioGateway = true;
                                        break;
                                }
                            }
                        }
                    }
                    catch
                    {
                        // Servicio no existe o no se puede acceder
                    }
                }

                // Verificar drivers de Bluetooth en el registro
                bool driversPresent = false;
                try
                {
                    const string bluetoothKey = @"SYSTEM\CurrentControlSet\Services\BTHPORT";
                    using (RegistryKey key = Registry.LocalMachine.OpenSubKey(bluetoothKey, false))
                    {
                        driversPresent = key != null;
                    }
                }
                catch
                {
                    driversPresent = false;
                }

                // Bluetooth está "saludable" si:
                // - Al menos el servicio principal está corriendo
                // - Los drivers están presentes
                bool isHealthy = (bluetoothSupport || bluetoothUserService) && driversPresent;

                Debug.WriteLine($"?? ESTADO DE BLUETOOTH:");
                Debug.WriteLine($"   • Bluetooth Support Service: {(bluetoothSupport ? "? Corriendo" : "? Detenido")}");
                Debug.WriteLine($"   • Bluetooth User Service: {(bluetoothUserService ? "? Corriendo" : "? Detenido")}");
                Debug.WriteLine($"   • Audio Gateway Service: {(bluetoothAudioGateway ? "? Corriendo" : "? Detenido")}");
                Debug.WriteLine($"   • Drivers presentes: {(driversPresent ? "? Sí" : "? No")}");
                Debug.WriteLine($"   • Estado general: {(isHealthy ? "? SALUDABLE" : "? PROBLEMAS DETECTADOS")}");

                return isHealthy;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error verificando Bluetooth: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Obtiene información detallada sobre el estado de Bluetooth
        /// </summary>
        public static string GetBluetoothStatus()
        {
            try
            {
                string status = "?? DIAGNÓSTICO DE BLUETOOTH:\n\n";

                // Verificar servicios
                string[] bluetoothServices = {
                    "bthserv",
                    "BluetoothUserService",
                    "BthAvctpSvc",
                    "BthA2dp",
                    "BthEnum"
                };

                status += "?? SERVICIOS:\n";
                foreach (string serviceName in bluetoothServices)
                {
                    try
                    {
                        using (ServiceController service = new ServiceController(serviceName))
                        {
                            string serviceStatus = service.Status.ToString();
                            string indicator = service.Status == ServiceControllerStatus.Running ? "?" : "?";
                            status += $"   {indicator} {serviceName}: {serviceStatus}\n";
                        }
                    }
                    catch
                    {
                        status += $"   ?? {serviceName}: No disponible\n";
                    }
                }

                // Verificar drivers
                status += "\n?? DRIVERS:\n";
                string[] bluetoothDrivers = {
                    @"SYSTEM\CurrentControlSet\Services\BTHPORT",
                    @"SYSTEM\CurrentControlSet\Services\BthPan",
                    @"SYSTEM\CurrentControlSet\Services\RFCOMM"
                };

                foreach (string driverPath in bluetoothDrivers)
                {
                    try
                    {
                        using (RegistryKey key = Registry.LocalMachine.OpenSubKey(driverPath, false))
                        {
                            if (key != null)
                            {
                                string driverName = driverPath.Split('\\')[^1];
                                status += $"   ? {driverName}: Instalado\n";
                            }
                        }
                    }
                    catch
                    {
                        string driverName = driverPath.Split('\\')[^1];
                        status += $"   ? {driverName}: No encontrado\n";
                    }
                }

                // Recomendaciones
                status += "\n?? RECOMENDACIONES:\n";
                if (IsBluetoothHealthy())
                {
                    status += "   ? Bluetooth funcionando correctamente\n";
                    status += "   ? Compatible con audífonos y Discord\n";
                }
                else
                {
                    status += "   ?? Problemas detectados en Bluetooth\n";
                    status += "   ?? Revisar drivers en Device Manager\n";
                    status += "   ?? Reiniciar servicios de Bluetooth\n";
                }

                return status;
            }
            catch (Exception ex)
            {
                return $"? Error obteniendo estado de Bluetooth: {ex.Message}";
            }
        }

        /// <summary>
        /// Intenta reparar problemas comunes de Bluetooth
        /// </summary>
        public static bool RepairBluetooth()
        {
            try
            {
                Debug.WriteLine("?? INTENTANDO REPARAR BLUETOOTH...");

                bool success = false;

                // Reiniciar servicios de Bluetooth
                string[] bluetoothServices = { "bthserv", "BluetoothUserService" };

                foreach (string serviceName in bluetoothServices)
                {
                    try
                    {
                        using (ServiceController service = new ServiceController(serviceName))
                        {
                            if (service.Status == ServiceControllerStatus.Running)
                            {
                                service.Stop();
                                service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
                            }

                            service.Start();
                            service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
                            
                            Debug.WriteLine($"? Servicio {serviceName} reiniciado");
                            success = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"? Error con servicio {serviceName}: {ex.Message}");
                    }
                }

                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error reparando Bluetooth: {ex.Message}");
                return false;
            }
        }
    }
}