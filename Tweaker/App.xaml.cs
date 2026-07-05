using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;

using Tweaker.License;
using Tweaker.Utilities;
using Tweaker.Services;

namespace Tweaker
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            // Inicializar idioma y tema
            LocalizationService.Initialize();
            ThemeManager.ApplyTheme(ThemeManager.LoadThemePreference());

            _ = Task.Run(() => HardwareDetector.Detect());

            // EVITAR QUE LA APP SE CIERRE AL CERRAR LA VENTANA DE ACTIVACIÓN
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            Debug.WriteLine("────────────────────────────────────────────────────────────────");
            Debug.WriteLine("\U0001F680 GHOST OPTIMIZER v2.3.1 - DI ARCHITECTURE");
            Debug.WriteLine("────────────────────────────────────────────────────────────────");

#if !DEBUG
            if (!AntiDebugger.IsDevelopmentEnvironment())
            {
                Debug.WriteLine("🛡️ Iniciando protección anti-debugging...");
                AntiDebugger.StartContinuousCheck();
            }
#endif

            var licenseManager = ServiceProvider.GetRequiredService<ILicenseManager>();
            if (!licenseManager.ValidateLicenseOnStartup())
            {
                if (licenseManager.ShowActivationWindow(showCancelOption: false) != true)
                {
                    Debug.WriteLine("❌ Activación cancelada o fallida. Cerrando aplicación.");
                    Shutdown();
                    return;
                }
                Debug.WriteLine("✅ Aplicación activada exitosamente. Continuando inicio...");
            }

            Debug.WriteLine("🏠 Mostrando ventana principal...");
            try
            {
                // P1: Wire dispatcher → TweakStateManager so TotalTweaksCount is live
                var dispatcher = ServiceProvider.GetRequiredService<ITweakDispatcher>();
                TweakStateManager.Instance.InjectDispatcher(dispatcher);

                // P2: Sync real system state → correct any JSON ↔ reality discrepancies
                Debug.WriteLine("🔍 Sincronizando estado real del sistema...");
                await StartupStateVerifier.SyncRealStateAsync(dispatcher, TweakStateManager.Instance);
                Debug.WriteLine("✅ State sync completo.");

                MainWindow mainWin = new MainWindow();
                mainWin.Show();

                ScheduleDelayedLocalization(mainWin);
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

            Task.Run(async () => await CheckAndFixBrowserIssues());
            Task.Run(async () => await CheckForUpdatesAsync());

            Debug.WriteLine("🛡️ Inicializando sistema de seguridad...");
            RescueScriptGenerator.GenerateRescueScript();
            OptimizationBackup.StartSession();
            
            Debug.WriteLine("");
            Debug.WriteLine("\U0001F50D Validando servicios críticos del sistema...");
            List<string> disabledServices = CriticalServicesValidator.ValidateCriticalServices();

            if (disabledServices.Count > 0)
            {
                HandleDisabledServices(disabledServices);
            }
            else
            {
                Debug.WriteLine("✅ Todos los servicios críticos están activos y funcionando correctamente");
            }

            Debug.WriteLine("────────────────────────────────────────────────────────────────");
            Debug.WriteLine("✅ SISTEMA DE SEGURIDAD ACTIVADO");
            Debug.WriteLine("────────────────────────────────────────────────────────────────");
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // License Module
            services.AddSingleton<IKeyVault, KeyVault>();
            services.AddSingleton<ISecurityChecks, SecurityChecks>();
            services.AddSingleton<ILicenseValidator, LicenseValidator>();
            services.AddSingleton<ILicenseStorage, LicenseStorage>();
            services.AddSingleton<ILicenseManager, LicenseManager>();

            // core Services
            services.AddSingleton<ITweakDispatcher, TweakDispatcher>();
            services.AddSingleton<IProfileManager, ProfileManager>();
            services.AddSingleton<IBackupService, BackupService>();
        }

        private void HandleDisabledServices(List<string> disabledServices)
        {
            Debug.WriteLine("⚠️ ADVERTENCIA: SERVICIOS CRÍTICOS DESHABILITADOS DETECTADOS");
            foreach (string service in disabledServices) Debug.WriteLine($"  ❌ {service}");

            string message = $"⚠️ ADVERTENCIA DE SEGURIDAD\n\nSe detectaron {disabledServices.Count} servicio(s) crítico(s) deshabilitado(s):\n\n";
            foreach (string service in disabledServices) message += $"  • {service}\n";
            message += $"\n¿Deseas restaurarlos automáticamente?\n\n(Recomendado: SÍ)";

            MessageBoxResult result = MessageBox.Show(message, "Servicios Críticos Deshabilitados", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                if (CriticalServicesValidator.RestoreCriticalServices(disabledServices))
                {
                    MessageBox.Show("✅ Servicios críticos restaurados exitosamente.\n\nREINICIA Windows para efecto completo.", "Restauración Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private async Task CheckAndFixBrowserIssues()
        {
            try
            {
                var scanResult = await BrowserSpeedFix.HasBrowserIssues();
                if (scanResult.Item1)
                {
                    bool shouldFix = false;
                    Dispatcher.Invoke(() =>
                    {
                        var result = MessageBox.Show("🌐 PROBLEMAS DE NAVEGACIÓN DETECTADOS\n\n¿Aplicar fix automático ahora?", "Fix Navegadores Disponible", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        shouldFix = (result == MessageBoxResult.Yes);
                    });
                    if (shouldFix) await BrowserSpeedFix.ApplyQuickBrowserFix();
                }
            }
            catch (Exception ex) { Debug.WriteLine($"❌ Error verificando navegadores: {ex.Message}"); }
        }

        private async Task CheckForUpdatesAsync()
        {
            try
            {
                await Task.Delay(3000);
                if (!UpdateChecker.ShouldCheckForUpdates()) return;
                var updateInfo = await UpdateChecker.CheckForUpdatesSilentAsync();
                UpdateChecker.RecordUpdateCheck();
                if (updateInfo != null)
                {
                    Dispatcher.Invoke(() => { new Windows.UpdateWindow(updateInfo).Show(); });
                }
            }
            catch (Exception ex) { Debug.WriteLine($"❌ Error verificando actualizaciones: {ex.Message}"); }
        }

        private static void ScheduleDelayedLocalization(Window mainWin)
        {
            // Temporalmente deshabilitado: la app permanece en español.
            // Se deja como no-op para evitar dependencias adicionales.
        }

        private void ShowLanguageSelectionIfNeeded()
        {
            // Temporalmente deshabilitado: la app permanece en español.
        }

        private AppLanguage? ShowLanguageSelectionDialog()
        {
            // Temporalmente deshabilitado: no se muestra selector de idioma.
            return AppLanguage.Spanish;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            OptimizationBackup.EndSession();
            TweakStateManager.Instance.Dispose();
            base.OnExit(e);
        }
    }
}
