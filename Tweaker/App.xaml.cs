using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Win32;

using Tweaker.License;
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

            // EVITAR QUE LA APP SE CIERRE AL CERRAR LA VENTANA DE ACTIVACIÓN
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            Debug.WriteLine("═══════════════════════════════════════════════════════════════");
            Debug.WriteLine("🚀 GHOST OPTIMIZER v2.3.1 - SEGURIDAD MEJORADA");
            Debug.WriteLine("═══════════════════════════════════════════════════════════════");

            // SEGURIDAD: Iniciar verificación continua de anti-debugging (solo en Release)
#if !DEBUG
            if (!AntiDebugger.IsDevelopmentEnvironment())
            {
                Debug.WriteLine("🛡️ Iniciando protección anti-debugging...");
                AntiDebugger.StartContinuousCheck();
            }
#endif

            // PRIMERO: Validar licencia al iniciar
            if (!LicenseManager.ValidateLicenseOnStartup())
            {
                // No hay licencia válida, mostrar ventana de activación
                bool? activated = LicenseManager.ShowActivationWindow(showCancelOption: false);

                // Si el usuario no activó (cerró la ventana), cerrar la app
                if (activated != true)
                {
                    Debug.WriteLine("❌ Activación cancelada o fallida. Cerrando aplicación.");
                    Shutdown();
                    return;
                }

                Debug.WriteLine("✅ Aplicación activada exitosamente. Continuando inicio...");
            }

            // Crear y mostrar la ventana principal explícitamente
            Debug.WriteLine("🏠 Mostrando ventana principal...");
            MainWindow mainWin;
            try
            {
                mainWin = new MainWindow();
                mainWin.Show();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ ERROR FATAL al crear MainWindow: {ex}");
                MessageBox.Show(
                    $"❌ Error al iniciar la ventana principal:\n\n{ex.Message}\n\n" +
                    $"Detalles: {ex.InnerException?.Message}\n\n" +
                    "Intenta ejecutar como Administrador o reinstalar la aplicación.",
                    "Error de Inicio",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Shutdown();
                return;
            }

            // 🌐 DIAGNÓSTICO Y FIX AUTOMÁTICO DE NAVEGADORES AL INICIO
            Task.Run(async () => await CheckAndFixBrowserIssues());

            // 🔄 VERIFICAR ACTUALIZACIONES (en background)
            Task.Run(async () => await CheckForUpdatesAsync());

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
            Debug.WriteLine("  🌐 Network Diagnostics: Detección de problemas de navegación");
            Debug.WriteLine("═══════════════════════════════════════════════════════════════");
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Este evento se ejecuta después de OnStartup
            // Aquí podemos hacer diagnósticos adicionales que requieran la UI cargada
        }

        /// <summary>
        /// Verifica y aplica fix automático para problemas de navegadores
        /// </summary>
        private async Task CheckAndFixBrowserIssues()
        {
            try
            {
                Debug.WriteLine("🔍 Verificando problemas de navegadores...");

                var scanResult = await BrowserSpeedFix.HasBrowserIssues();
                bool hasBrowserIssues = scanResult.Item1;

                if (hasBrowserIssues)
                {
                    Debug.WriteLine("🚨 PROBLEMAS DE NAVEGADORES DETECTADOS");

                    // Mostrar prompt al usuario
                    bool shouldFix = false;
                    Dispatcher.Invoke(() =>
                    {
                        var result = MessageBox.Show(
                            "🌐 PROBLEMAS DE NAVEGACIÓN DETECTADOS\n\n" +
                            "Ghost Optimizer ha detectado configuraciones extremas\n" +
                            "que están causando lentitud en navegadores web.\n\n" +
                            "💡 SOLUCIÓN AUTOMÁTICA DISPONIBLE:\n" +
                            "• Mantiene 90% del rendimiento gaming\n" +
                            "• Mejora SIGNIFICATIVAMENTE la velocidad de navegadores\n" +
                            "• Fix se aplica inmediatamente\n\n" +
                            "¿Aplicar fix automático ahora?",
                            "Fix Navegadores Disponible",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question);

                        shouldFix = (result == MessageBoxResult.Yes);
                    });

                    if (shouldFix)
                    {
                        Debug.WriteLine("🔧 Usuario autorizó fix automático, aplicando...");
                        await BrowserSpeedFix.ApplyQuickBrowserFix();
                    }
                    else
                    {
                        Debug.WriteLine("ℹ️ Usuario decidió no aplicar fix automático");
                        Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show(
                                "ℹ️ Fix no aplicado\n\n" +
                                "Si experimentas navegadores lentos, puedes:\n\n" +
                                "1. Ejecutar FIX_NAVEGADORES_ULTRA_RAPIDO_v2.3.0.bat\n" +
                                "2. Ir a Red & Ping → Optimización Balanceada\n" +
                                "3. Usar el botón 'REVERTIR TODOS' si el problema persiste",
                                "Fix Manual Disponible",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                        });
                    }
                }
                else
                {
                    Debug.WriteLine("✅ No se detectaron problemas de navegadores");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error verificando navegadores: {ex.Message}");
            }
        }

        /// <summary>
        /// Verifica si hay actualizaciones disponibles
        /// </summary>
        private async Task CheckForUpdatesAsync()
        {
            try
            {
                // Esperar 3 segundos después del inicio para no interferir con la carga
                await Task.Delay(3000);

                // Verificar si debe revisar actualizaciones (evitar verificar muy seguido)
                if (!UpdateChecker.ShouldCheckForUpdates())
                {
                    Debug.WriteLine("⏭️ Saltando verificación de actualizaciones (verificado recientemente)");
                    return;
                }

                Debug.WriteLine("🔍 Verificando actualizaciones disponibles...");

                // Verificar actualizaciones
                var updateInfo = await UpdateChecker.CheckForUpdatesSilentAsync();

                // Registrar que se verificó
                UpdateChecker.RecordUpdateCheck();

                if (updateInfo != null)
                {
                    Debug.WriteLine($"🎉 Nueva actualización disponible: v{updateInfo.Version}");

                    // Mostrar ventana de actualización en el thread de UI
                    Dispatcher.Invoke(() =>
                    {
                        var updateWindow = new Windows.UpdateWindow(updateInfo);
                        updateWindow.Show();
                    });
                }
                else
                {
                    Debug.WriteLine("✅ No hay actualizaciones disponibles");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error verificando actualizaciones: {ex.Message}");
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Finalizar sesión de backup al cerrar la app
            OptimizationBackup.EndSession();

            // Liberar recursos de servicios
            TweakStateManager.Instance.Dispose();

            Debug.WriteLine("═══════════════════════════════════════════════════════════════");
            Debug.WriteLine("👋 GHOST OPTIMIZER TWEAKER - CERRANDO");
            Debug.WriteLine("═══════════════════════════════════════════════════════════════");

            base.OnExit(e);
        }
    }
}

