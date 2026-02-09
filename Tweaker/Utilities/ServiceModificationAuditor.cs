using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Tweaker.Utilities
{
    /// <summary>
    /// AUDITOR DE MODIFICACIONES DE SERVICIOS
    /// 
    /// PROPÓSITO:
    /// ????????????????????????????????????????????????????????????????
    /// Registrar TODAS las modificaciones de servicios para auditoría.
    /// Detectar intentos de modificar servicios protegidos.
    /// Generar reportes de seguridad.
    /// 
    /// CUÁNDO SE USA:
    /// ????????????????????????????????????????????????????????????????
    /// - Cada vez que se modifica un servicio (éxito o fallo)
    /// - Al finalizar la sesión (generar reporte)
    /// - Para debugging de incidentes
    /// 
    /// SEVERIDAD: ALTA - Sistema de auditoría y seguridad
    /// </summary>
    public static class ServiceModificationAuditor
    {
        // Log de modificaciones
        private static readonly List<ServiceModification> _modifications = new List<ServiceModification>();
        
        // Servicios que NUNCA deben modificarse (hardcoded por seguridad)
        private static readonly HashSet<string> _criticalServices = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            // CRÍTICOS - CAUSA SIHOST.EXE ERROR
            "BrokerInfrastructure",
            "DcomLaunch",
            "RpcSs",
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
            "Dnscache",
            "NlaSvc",
            "netprofm",
            "Wcmsvc",
            "StorSvc"
        };

        /// <summary>
        /// REGISTRAR INTENTO DE MODIFICACIÓN
        /// </summary>
        public static void LogModificationAttempt(
            string serviceName, 
            string operationType, 
            bool wasBlocked, 
            string reason = "")
        {
            var mod = new ServiceModification
            {
                Timestamp = DateTime.Now,
                ServiceName = serviceName,
                OperationType = operationType,
                WasBlocked = wasBlocked,
                Reason = reason,
                StackTrace = Environment.StackTrace
            };

            lock (_modifications)
            {
                _modifications.Add(mod);
            }

            // Log inmediato
            if (wasBlocked)
            {
                Debug.WriteLine("???????????????????????????????????????????????????????????????");
                Debug.WriteLine($"?? MODIFICACIÓN BLOQUEADA: {serviceName}");
                Debug.WriteLine($"   Operación: {operationType}");
                Debug.WriteLine($"   Razón: {reason}");
                Debug.WriteLine($"   Timestamp: {mod.Timestamp:yyyy-MM-dd HH:mm:ss}");
                Debug.WriteLine("???????????????????????????????????????????????????????????????");
            }
            else
            {
                Debug.WriteLine($"? Modificación permitida: {serviceName} ({operationType})");
            }

            // Alertar si es un servicio crítico
            if (_criticalServices.Contains(serviceName))
            {
                Debug.WriteLine("");
                Debug.WriteLine("?????? ALERTA CRÍTICA ??????");
                Debug.WriteLine($"Intento de modificar servicio CRÍTICO: {serviceName}");
                Debug.WriteLine($"Estado: {(wasBlocked ? "BLOQUEADO ?" : "PERMITIDO ? (PELIGRO)")}");
                Debug.WriteLine("");
                
                if (!wasBlocked)
                {
                    Debug.WriteLine("?????? PELIGRO: SERVICIO CRÍTICO FUE MODIFICADO ??????");
                    Debug.WriteLine("Esto puede causar:");
                    Debug.WriteLine("  - Error 'Sihost.exe - Unknown Hard Error'");
                    Debug.WriteLine("  - Pantalla negra al reiniciar");
                    Debug.WriteLine("  - Sistema inestable");
                    Debug.WriteLine("");
                    Debug.WriteLine("REVISA INMEDIATAMENTE:");
                    Debug.WriteLine($"  Servicio: {serviceName}");
                    Debug.WriteLine($"  Operación: {operationType}");
                    Debug.WriteLine($"  Stack Trace:");
                    Debug.WriteLine(mod.StackTrace);
                    Debug.WriteLine("????????????????????????????????????????");
                }
            }
        }

        /// <summary>
        /// GENERAR REPORTE DE AUDITORÍA
        /// </summary>
        public static string GenerateAuditReport()
        {
            var report = new System.Text.StringBuilder();
            
            report.AppendLine("???????????????????????????????????????????????????????????????");
            report.AppendLine("REPORTE DE AUDITORÍA - MODIFICACIONES DE SERVICIOS");
            report.AppendLine("???????????????????????????????????????????????????????????????");
            report.AppendLine($"Generado: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            report.AppendLine("");

            lock (_modifications)
            {
                report.AppendLine($"Total de operaciones: {_modifications.Count}");
                
                int blocked = _modifications.FindAll(m => m.WasBlocked).Count;
                int allowed = _modifications.Count - blocked;
                
                report.AppendLine($"  ? Permitidas: {allowed}");
                report.AppendLine($"  ? Bloqueadas: {blocked}");
                report.AppendLine("");

                // Modificaciones críticas
                var criticalMods = _modifications.FindAll(m => 
                    _criticalServices.Contains(m.ServiceName));
                
                if (criticalMods.Count > 0)
                {
                    report.AppendLine("?? INTENTOS DE MODIFICAR SERVICIOS CRÍTICOS:");
                    report.AppendLine("");
                    
                    foreach (var mod in criticalMods)
                    {
                        report.AppendLine($"  [{mod.Timestamp:HH:mm:ss}] {mod.ServiceName}");
                        report.AppendLine($"    Operación: {mod.OperationType}");
                        report.AppendLine($"    Estado: {(mod.WasBlocked ? "BLOQUEADO ?" : "MODIFICADO ?")}");
                        report.AppendLine($"    Razón: {mod.Reason}");
                        report.AppendLine("");
                    }
                }

                // Todas las operaciones (detallado)
                report.AppendLine("???????????????????????????????????????????????????????????????");
                report.AppendLine("REGISTRO COMPLETO:");
                report.AppendLine("???????????????????????????????????????????????????????????????");
                report.AppendLine("");

                foreach (var mod in _modifications)
                {
                    string status = mod.WasBlocked ? "? BLOQUEADO" : "? PERMITIDO";
                    report.AppendLine($"[{mod.Timestamp:yyyy-MM-dd HH:mm:ss}] {status}");
                    report.AppendLine($"  Servicio: {mod.ServiceName}");
                    report.AppendLine($"  Operación: {mod.OperationType}");
                    
                    if (!string.IsNullOrEmpty(mod.Reason))
                        report.AppendLine($"  Razón: {mod.Reason}");
                    
                    report.AppendLine("");
                }
            }

            report.AppendLine("???????????????????????????????????????????????????????????????");
            
            return report.ToString();
        }

        /// <summary>
        /// OBTENER MODIFICACIONES
        /// </summary>
        public static List<ServiceModification> GetModifications()
        {
            lock (_modifications)
            {
                return new List<ServiceModification>(_modifications);
            }
        }

        /// <summary>
        /// LIMPIAR LOG (Para testing)
        /// </summary>
        public static void ClearLog()
        {
            lock (_modifications)
            {
                _modifications.Clear();
            }
            Debug.WriteLine("?? Log de auditoría limpiado");
        }

        /// <summary>
        /// GUARDAR REPORTE A ARCHIVO
        /// </summary>
        public static void SaveReportToFile(string filePath)
        {
            try
            {
                string report = GenerateAuditReport();
                System.IO.File.WriteAllText(filePath, report);
                Debug.WriteLine($"?? Reporte guardado: {filePath}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error guardando reporte: {ex.Message}");
            }
        }

        // Clase para almacenar modificaciones
        public class ServiceModification
        {
            public DateTime Timestamp { get; set; }
            public string ServiceName { get; set; }
            public string OperationType { get; set; } // "Disable", "Enable", "Stop", "Start"
            public bool WasBlocked { get; set; }
            public string Reason { get; set; }
            public string StackTrace { get; set; }
        }
    }
}
