using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Windows;
using Tweaker.Utilities;

namespace Tweaker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Debug.WriteLine("═══════════════════════════════════════════════════════════════");
            Debug.WriteLine("🚀 GHOST OPTIMIZER TWEAKER - INICIANDO");
            Debug.WriteLine("═══════════════════════════════════════════════════════════════");

            // ═══════════════════════════════════════════════════════════════
            // SISTEMA DE SEGURIDAD: Inicialización
            // ═══════════════════════════════════════════════════════════════

            Debug.WriteLine("🛡️ Inicializando sistema de seguridad...");

            // 1. Generar script de rescate en el escritorio
            RescueScriptGenerator.GenerateRescueScript();

            // 2. Iniciar sesión de backup
            OptimizationBackup.StartSession();

            // 3. NUEVO: Validar servicios críticos
            Debug.WriteLine("");
            Debug.WriteLine("🔍 Validando servicios críticos del sistema...");
            List<string> disabledServices = CriticalServicesValidator.ValidateCriticalServices();

            if (disabledServices.Count > 0)
            {
                Debug.WriteLine("═══════════════════════════════════════════════════════════════");
                Debug.WriteLine("⚠️ ADVERTENCIA: SERVICIOS CRÍTICOS DESHABILITADOS DETECTADOS");
                Debug.WriteLine("═══════════════════════════════════════════════════════════════");
                
                foreach (string service in disabledServices)
                {
                    Debug.WriteLine($"  ❌ {service}");
                }

                Debug.WriteLine("");
                Debug.WriteLine("Estos servicios son CRÍTICOS para el funcionamiento de Windows.");
                Debug.WriteLine("El sistema puede ser inestable o generar errores.");
                Debug.WriteLine("");

                // Mostrar advertencia al usuario
                string message = $"⚠️ ADVERTENCIA DE SEGURIDAD\n\n" +
                                $"Se detectaron {disabledServices.Count} servicio(s) crítico(s) deshabilitado(s):\n\n";
                
                foreach (string service in disabledServices)
                {
                    message += $"  • {service}\n";
                }

                message += $"\nEstos servicios son ESENCIALES para Windows.\n" +
                          $"¿Deseas restaurarlos automáticamente?\n\n" +
                          $"(Recomendado: SÍ)";

                MessageBoxResult result = MessageBox.Show(
                    message,
                    "Servicios Críticos Deshabilitados",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    Debug.WriteLine("Usuario eligió RESTAURAR servicios críticos...");
                    bool restored = CriticalServicesValidator.RestoreCriticalServices(disabledServices);
                    
                    if (restored)
                    {
                        MessageBox.Show(
                            "✅ Servicios críticos restaurados exitosamente.\n\n" +
                            "REINICIA Windows para que los cambios surtan efecto completo.",
                            "Restauración Exitosa",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show(
                            "⚠️ Algunos servicios no pudieron restaurarse automáticamente.\n\n" +
                            "Ejecuta RESCATE_TWEAKER.bat desde tu Escritorio\n" +
                            "o contacta soporte técnico.",
                            "Restauración Parcial",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                    }
                }
                else
                {
                    Debug.WriteLine("Usuario eligió NO restaurar servicios críticos.");
                    MessageBox.Show(
                        "⚠️ PROCEDE CON PRECAUCIÓN\n\n" +
                        "Si experimentas errores o inestabilidad:\n\n" +
                        "1. Ejecuta RESCATE_TWEAKER.bat desde tu Escritorio\n" +
                        "2. O usa el botón 'REVERTIR TODOS LOS TWEAKS'\n" +
                        "3. Reinicia Windows",
                        "Servicios Críticos Sin Restaurar",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }

                Debug.WriteLine("═══════════════════════════════════════════════════════════════");
            }
            else
            {
                Debug.WriteLine("✅ Todos los servicios críticos están activos y funcionando correctamente");
            }

            Debug.WriteLine("═══════════════════════════════════════════════════════════════");
            Debug.WriteLine("✅ SISTEMA DE SEGURIDAD ACTIVADO");
            Debug.WriteLine("═══════════════════════════════════════════════════════════════");
            Debug.WriteLine("");
            Debug.WriteLine("PROTECCIONES ACTIVAS:");
            Debug.WriteLine("  🛡️ ServiceGuard: Protección de servicios críticos");
            Debug.WriteLine("  💾 OptimizationBackup: Backup automático de cambios");
            Debug.WriteLine("  📸 System Restore: Punto de restauración creado");
            Debug.WriteLine("  🆘 Rescue Script: RESCATE_TWEAKER.bat en Escritorio");
            Debug.WriteLine("  🔍 Critical Services Validator: Monitoreo activo");
            Debug.WriteLine("═══════════════════════════════════════════════════════════════");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Finalizar sesión de backup al cerrar la app
            OptimizationBackup.EndSession();
            
            Debug.WriteLine("═══════════════════════════════════════════════════════════════");
            Debug.WriteLine("👋 GHOST OPTIMIZER TWEAKER - CERRANDO");
            Debug.WriteLine("═══════════════════════════════════════════════════════════════");

            base.OnExit(e);
        }
    }

}

