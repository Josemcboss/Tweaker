using System.Collections.Generic;
using System.Diagnostics;

namespace Tweaker.Utilities
{
    /// <summary>
    /// SISTEMA DE PROTECCIÓN DE SERVICIOS CRÍTICOS
    /// 
    /// ¿Por qué existe esta clase?
    /// ──────────────────────────────────────────?
    /// Un error en la optimización de servicios puede causar:
    /// - Pantallas negras al iniciar Windows
    /// - Error "Sihost.exe - Unknown Hard Error"
    /// - Sistema inestable que requiere reinstalación
    /// 
    /// SERVICIOS PROHIBIDOS (NUNCA MODIFICAR):
    /// ──────────────────────────────────────────?
    /// Esta lista contiene servicios VITALES para el funcionamiento de Windows.
    /// Si se deshabilitan, Windows NO ARRANCA o tiene errores críticos.
    /// 
    /// PROCESO DE VALIDACIÓN:
    /// ──────────────────────────────────────────?
    /// Antes de modificar cualquier servicio, se debe llamar a:
    ///   if (ServiceGuard.IsProtected(serviceName))
    ///   {
    ///       Debug.WriteLine($"?? SERVICIO PROTEGIDO: {serviceName}");
    ///       return false; // NO MODIFICAR
    ///   }
    /// 
    /// SEVERIDAD: CRÍTICO - NO MODIFICAR ESTA LISTA SIN INVESTIGACIÓN PROFUNDA
    /// </summary>
    public static class ServiceGuard
    {
        /// <summary>
        /// LISTA BLANCA DE SERVICIOS PROTEGIDOS
        /// ──────────────────────────────────────────?
        /// Servicios que NUNCA deben ser deshabilitados bajo ninguna circunstancia.
        /// 
        /// RpcSs (Remote Procedure Call):
        /// ──────────────────────────────────────────?
        /// - Sistema de comunicación entre procesos de Windows
        /// - CRÍTICO: Sin este, Windows no puede comunicar internamente
        /// - Deshabilitar = Pantalla negra / Boot loop
        /// 
        /// DcomLaunch (DCOM Server Process Launcher):
        /// ──────────────────────────────────────────?
        /// - Lanza servicios COM/DCOM
        /// - CRÍTICO: Necesario para Shell (explorer.exe, sihost.exe)
        /// - Deshabilitar = "Sihost.exe Unknown Hard Error"
        /// 
        /// BrokerInfrastructure (Background Tasks Infrastructure Service):
        /// ──────────────────────────────────────────?
        /// - Infraestructura para tareas en background
        /// - CRÍTICO: Necesario para Modern Apps y Shell
        /// - Deshabilitar = Errores de aplicaciones críticas
        /// 
        /// LSM (Local Session Manager):
        /// ──────────────────────────────────────────?
        /// - Maneja sesiones locales de usuario
        /// - CRÍTICO: Sin este, no puedes iniciar sesión
        /// - Deshabilitar = No login / Pantalla negra
        /// 
        /// SamSs (Security Accounts Manager):
        /// ──────────────────────────────────────────?
        /// - Base de datos de cuentas de usuario
        /// - CRÍTICO: Necesario para autenticación
        /// - Deshabilitar = No login / Fallo de seguridad
        /// 
        /// ProfSvc (User Profile Service):
        /// ──────────────────────────────────────────?
        /// - Carga perfil de usuario
        /// - CRÍTICO: Sin este, Windows carga perfil temporal
        /// - Deshabilitar = Login con perfil temporal / Pérdida de settings
        /// 
        /// RpcEptMapper (RPC Endpoint Mapper):
        /// ──────────────────────────────────────────?
        /// - Mapper de endpoints RPC
        /// - CRÍTICO: Necesario para comunicación RPC
        /// - Deshabilitar = Muchos servicios fallan
        /// 
        /// gpsvc (Group Policy Client):
        /// ──────────────────────────────────────────?
        /// - Aplica políticas de grupo
        /// - CRÍTICO: Necesario para configuración de sistema
        /// - Deshabilitar = Configuraciones no se aplican
        /// 
        /// CoreMessagingRegistrar (Core Messaging Broker):
        /// ──────────────────────────────────────────?
        /// - Sistema de mensajería del sistema
        /// - CRÍTICO: Necesario para notificaciones y comunicación
        /// - Deshabilitar = Shell inestable
        /// 
        /// SystemEventsBroker (System Events Broker):
        /// ──────────────────────────────────────────?
        /// - Broker de eventos del sistema
        /// - CRÍTICO: Necesario para eventos de Windows
        /// - Deshabilitar = Muchas funciones dejan de trabajar
        /// 
        /// StateRepository (State Repository Service):
        /// ──────────────────────────────────────────?
        /// - Repositorio de estado de Windows
        /// - CRÍTICO: Necesario para Modern UI
        /// - Deshabilitar = Start Menu / Notificaciones fallan
        /// 
        /// Power (Power Service):
        /// ──────────────────────────────────────────?
        /// - Gestión de energía
        /// - CRÍTICO: Necesario para gestión de energía
        /// - Deshabilitar = Problemas de apagado/suspensión
        /// 
        /// EventLog (Windows Event Log):
        /// ──────────────────────────────────────────?
        /// - Sistema de logs de Windows
        /// - CRÍTICO: Necesario para diagnóstico
        /// - Deshabilitar = No logs / Difícil diagnosticar problemas
        /// 
        /// PlugPlay (Plug and Play):
        /// ──────────────────────────────────────────?
        /// - Detecta hardware
        /// - CRÍTICO: Necesario para detectar dispositivos
        /// - Deshabilitar = Dispositivos no funcionan
        /// 
        /// CryptSvc (Cryptographic Services):
        /// ──────────────────────────────────────────?
        /// - Servicios criptográficos
        /// - CRÍTICO: Necesario para firmas digitales
        /// - Deshabilitar = Windows Update falla
        /// 
        /// Themes (Themes Service):
        /// ──────────────────────────────────────────?
        /// - Temas visuales
        /// - IMPORTANTE: Necesario para UI moderna
        /// - Deshabilitar = UI fea / Classic theme
        /// 
        /// UserManager (User Manager):
        /// ──────────────────────────────────────────?
        /// - Gestión de usuarios
        /// - CRÍTICO: Necesario para multi-usuario
        /// - Deshabilitar = Problemas de login
        /// 
        /// Schedule (Task Scheduler):
        /// ──────────────────────────────────────────?
        /// - Programador de tareas
        /// - IMPORTANTE: Muchas tareas críticas del sistema lo usan
        /// - Deshabilitar = Mantenimiento no se ejecuta
        /// 
        /// ADVERTENCIA:
        /// ──────────────────────────────────────────?
        /// NO AGREGAR SERVICIOS A ESTA LISTA SIN INVESTIGACIÓN EXHAUSTIVA.
        /// NO REMOVER SERVICIOS DE ESTA LISTA SIN PROBAR EN VM PRIMERO.
        /// </summary>
        private static readonly HashSet<string> ProtectedServices = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase)
        {
            // ──────────────────────────────────────────
            // NÚCLEO DE WINDOWS (ABSOLUTAMENTE CRÍTICO)
            // ──────────────────────────────────────────
            "RpcSs",                    // Remote Procedure Call
            "DcomLaunch",               // DCOM Server Process Launcher
            "BrokerInfrastructure",     // Background Tasks Infrastructure
            "LSM",                      // Local Session Manager
            "SamSs",                    // Security Accounts Manager
            "ProfSvc",                  // User Profile Service
            "RpcEptMapper",             // RPC Endpoint Mapper
            "gpsvc",                    // Group Policy Client
            "CoreMessagingRegistrar",   // Core Messaging Broker
            "SystemEventsBroker",       // System Events Broker
            "StateRepository",          // State Repository Service
            "Power",                    // Power Service
            "EventLog",                 // Windows Event Log
            "PlugPlay",                 // Plug and Play
            "CryptSvc",                 // Cryptographic Services
            
            // ──────────────────────────────────────────
            // SHELL Y UI (NECESARIO PARA INTERFAZ)
            // ──────────────────────────────────────────
            "Themes",                   // Themes Service
            "UserManager",              // User Manager
            "Schedule",                 // Task Scheduler
            
            // ──────────────────────────────────────────
            // SEGURIDAD (NO DESHABILITAR POR SEGURIDAD)
            // ──────────────────────────────────────────
            "SecurityHealthService",    // Windows Security Service
            "WdNisSvc",                 // Windows Defender Network Inspection
            "WinDefend",                // Windows Defender Antivirus
            
            // ──────────────────────────────────────────
            // RED (NECESARIO PARA CONECTIVIDAD)
            // ──────────────────────────────────────────
            "Dhcp",                     // DHCP Client
            "Dnscache",                 // DNS Client
            "NlaSvc",                   // Network Location Awareness
            "netprofm",                 // Network List Service
            "Wcmsvc",                   // Windows Connection Manager
            
            // ──────────────────────────────────────────
            // STORAGE (NECESARIO PARA DISCOS)
            // ──────────────────────────────────────────
            "StorSvc",                  // Storage Service
            "BDESVC",                   // BitLocker Drive Encryption (si está habilitado)
        };

        /// <summary>
        /// VERIFICAR SI UN SERVICIO ESTÁ PROTEGIDO
        /// ──────────────────────────────────────────?
        /// Método principal de validación.
        /// 
        /// USO:
        ///   if (ServiceGuard.IsProtected("RpcSs"))
        ///   {
        ///       // NO MODIFICAR - Es crítico
        ///       return false;
        ///   }
        /// 
        /// COMPARACIÓN:
        /// - Ignora mayúsculas/minúsculas (case-insensitive)
        /// - Rápido (O(1) gracias a HashSet)
        /// </summary>
        public static bool IsProtected(string serviceName)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
                return true; // Por seguridad, bloquear nombres vacíos

            bool isProtected = ProtectedServices.Contains(serviceName);

            if (isProtected)
            {
                Debug.WriteLine("──────────────────────────────────────────");
                Debug.WriteLine($"── SERVICIO PROTEGIDO DETECTADO: {serviceName}");
                Debug.WriteLine("──────────────────────────────────────────");
                Debug.WriteLine("? OPERACIÓN BLOQUEADA");
                Debug.WriteLine("Este servicio es CRÍTICO para el funcionamiento de Windows.");
                Debug.WriteLine("Modificarlo puede causar:");
                Debug.WriteLine("  - Pantalla negra al iniciar");
                Debug.WriteLine("  - Error 'Sihost.exe - Unknown Hard Error'");
                Debug.WriteLine("  - Sistema inestable");
                Debug.WriteLine("──────────────────────────────────────────");
            }

            return isProtected;
        }

        /// <summary>
        /// OBTENER LISTA DE SERVICIOS PROTEGIDOS
        /// ──────────────────────────────────────────?
        /// Para debugging o mostrar al usuario qué servicios están protegidos.
        /// </summary>
        public static IReadOnlyCollection<string> GetProtectedServices()
        {
            return ProtectedServices;
        }

        /// <summary>
        /// AGREGAR SERVICIO A PROTECCIÓN (USO AVANZADO)
        /// ──────────────────────────────────────────?
        /// Permite al usuario agregar sus propios servicios críticos.
        /// 
        /// EJEMPLO:
        ///   ServiceGuard.AddProtectedService("MyCustomCriticalService");
        /// 
        /// ADVERTENCIA:
        /// - Solo usar si sabes que un servicio es crítico para tu sistema
        /// - Cambios persisten durante la ejecución de la app
        /// </summary>
        public static void AddProtectedService(string serviceName)
        {
            if (!string.IsNullOrWhiteSpace(serviceName))
            {
                ProtectedServices.Add(serviceName);
                Debug.WriteLine($"── Servicio agregado a protección: {serviceName}");
            }
        }
    }
}
