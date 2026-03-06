using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceProcess;

using Microsoft.Win32;

namespace Tweaker.Utilities
{
    /// <summary>
    /// VALIDADOR DE SERVICIOS CRÍTICOS
    /// 
    /// PROPÓSITO:
    /// ????????????????????????????????????????????????????????????????
    /// Verificar al INICIAR la aplicación que todos los servicios críticos
    /// están activos. Si alguno está deshabilitado, ALERTAR al usuario.
    /// 
    /// CUÁNDO EJECUTAR:
    /// ????????????????????????????????????????????????????????????????
    /// - Al iniciar la aplicación (App.xaml.cs)
    /// - Antes de aplicar cualquier tweak
    /// - Después de restaurar el sistema
    /// 
    /// SEVERIDAD: CRÍTICO
    /// </summary>
    public static class CriticalServicesValidator
    {
        /// <summary>
        /// VERIFICAR TODOS LOS SERVICIOS CRÍTICOS
        /// ????????????????????????????????????????????????????????????????
        /// Retorna una lista de servicios críticos que están DESHABILITADOS.
        /// </summary>
        public static List<string> ValidateCriticalServices()
        {
            Debug.WriteLine("???????????????????????????????????????????????????????????????");
            Debug.WriteLine("?? VALIDANDO SERVICIOS CRÍTICOS");
            Debug.WriteLine("???????????????????????????????????????????????????????????????");

            List<string> disabledServices = new List<string>();

            // Lista de servicios críticos (misma que ServiceGuard)
            string[] criticalServices = new[]
            {
                "RpcSs",
                "DcomLaunch",
                "BrokerInfrastructure",
                "LSM",
                "SamSs",
                "ProfSvc",
                "RpcEptMapper",
                "gpsvc",
                "CoreMessagingRegistrar",
                "SystemEventsBroker",
                "StateRepository",
                "Power",
                "EventLog",
                "PlugPlay",
                "CryptSvc",
                "Themes",
                "UserManager",
                "Schedule",
                "Dhcp",
                "Dnscache"
            };

            foreach (string serviceName in criticalServices)
            {
                var status = GetServiceStatus(serviceName);

                if (status.IsDisabled)
                {
                    Debug.WriteLine($"? CRÍTICO: {serviceName} está DESHABILITADO");
                    disabledServices.Add(serviceName);
                }
                else if (status.IsStopped && status.IsAutoStart)
                {
                    Debug.WriteLine($"?? ADVERTENCIA: {serviceName} está DETENIDO (pero configurado como AUTO)");
                }
                else
                {
                    Debug.WriteLine($"? {serviceName}: {status.State} ({status.StartType})");
                }
            }

            Debug.WriteLine("???????????????????????????????????????????????????????????????");
            Debug.WriteLine($"?? RESUMEN: {disabledServices.Count} servicios críticos deshabilitados");
            Debug.WriteLine("???????????????????????????????????????????????????????????????");

            return disabledServices;
        }

        /// <summary>
        /// OBTENER ESTADO DETALLADO DE UN SERVICIO
        /// </summary>
        private static ServiceStatus GetServiceStatus(string serviceName)
        {
            var status = new ServiceStatus
            {
                ServiceName = serviceName
            };

            try
            {
                // Obtener estado del servicio
                using (ServiceController sc = new ServiceController(serviceName))
                {
                    status.State = sc.Status.ToString();
                    status.IsStopped = sc.Status == ServiceControllerStatus.Stopped;
                }

                // Obtener StartType del registro
                string registryPath = $@"SYSTEM\CurrentControlSet\Services\{serviceName}";
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryPath, false))
                {
                    if (key != null)
                    {
                        object startValue = key.GetValue("Start");
                        if (startValue != null)
                        {
                            int startType = (int)startValue;
                            status.StartType = startType switch
                            {
                                2 => "AUTO",
                                3 => "MANUAL",
                                4 => "DISABLED",
                                _ => "UNKNOWN"
                            };

                            status.IsDisabled = (startType == 4);
                            status.IsAutoStart = (startType == 2);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                status.State = "ERROR";
                status.StartType = "ERROR";
                Debug.WriteLine($"?? Error verificando {serviceName}: {ex.Message}");
            }

            return status;
        }

        /// <summary>
        /// RESTAURAR SERVICIOS CRÍTICOS DESHABILITADOS
        /// ????????????????????????????????????????????????????????????????
        /// Intenta restaurar automáticamente los servicios críticos.
        /// </summary>
        public static bool RestoreCriticalServices(List<string> disabledServices)
        {
            if (disabledServices == null || disabledServices.Count == 0)
                return true;

            Debug.WriteLine("???????????????????????????????????????????????????????????????");
            Debug.WriteLine("?? RESTAURANDO SERVICIOS CRÍTICOS");
            Debug.WriteLine("???????????????????????????????????????????????????????????????");

            bool allRestored = true;

            foreach (string serviceName in disabledServices)
            {
                try
                {
                    Debug.WriteLine($"Restaurando: {serviceName}...");

                    // Configurar como AUTO_START
                    string registryPath = $@"SYSTEM\CurrentControlSet\Services\{serviceName}";
                    using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryPath, true))
                    {
                        if (key != null)
                        {
                            key.SetValue("Start", 2, RegistryValueKind.DWord); // 2 = AUTO
                            Debug.WriteLine($"  ? {serviceName} configurado como AUTO_START");
                        }
                    }

                    // Intentar iniciar el servicio
                    using (ServiceController sc = new ServiceController(serviceName))
                    {
                        if (sc.Status != ServiceControllerStatus.Running)
                        {
                            sc.Start();
                            sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                            Debug.WriteLine($"  ? {serviceName} iniciado correctamente");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"  ? Error restaurando {serviceName}: {ex.Message}");
                    allRestored = false;
                }
            }

            Debug.WriteLine("???????????????????????????????????????????????????????????????");
            return allRestored;
        }

        /// <summary>
        /// GENERAR REPORTE DETALLADO PARA DEBUGGING
        /// </summary>
        public static string GenerateDetailedReport()
        {
            var report = new System.Text.StringBuilder();

            report.AppendLine("???????????????????????????????????????????????????????????????");
            report.AppendLine("REPORTE DE SERVICIOS CRÍTICOS");
            report.AppendLine("???????????????????????????????????????????????????????????????");
            report.AppendLine();

            string[] criticalServices = new[]
            {
                "RpcSs", "DcomLaunch", "BrokerInfrastructure", "LSM", "SamSs",
                "ProfSvc", "RpcEptMapper", "gpsvc", "CoreMessagingRegistrar",
                "SystemEventsBroker", "StateRepository", "Power", "EventLog",
                "PlugPlay", "CryptSvc", "Themes", "UserManager", "Schedule",
                "Dhcp", "Dnscache"
            };

            foreach (string serviceName in criticalServices)
            {
                var status = GetServiceStatus(serviceName);

                report.AppendLine($"Servicio: {serviceName}");
                report.AppendLine($"  Estado: {status.State}");
                report.AppendLine($"  Tipo de Inicio: {status.StartType}");
                report.AppendLine($"  ¿Deshabilitado?: {(status.IsDisabled ? "SÍ ?" : "NO ?")}");
                report.AppendLine();
            }

            report.AppendLine("???????????????????????????????????????????????????????????????");

            return report.ToString();
        }

        // Clase auxiliar para almacenar estado del servicio
        private class ServiceStatus
        {
            public string? ServiceName { get; set; }
            public string? State { get; set; }
            public string? StartType { get; set; }
            public bool IsDisabled { get; set; }
            public bool IsStopped { get; set; }
            public bool IsAutoStart { get; set; }
        }
    }
}
