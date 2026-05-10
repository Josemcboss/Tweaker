#nullable disable

using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Win32;

using Tweaker.Controls;
using Tweaker.Data;
using Tweaker.Models;
using Tweaker.Optimizations;
using Tweaker.Presets;
using Tweaker.Services;
using Tweaker.Utilities;

namespace Tweaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly TweakStateManager _stateManager;
        private readonly TelemetryService _telemetry;
        private readonly NotificationService _notifications;
        private readonly TweakHelper _tweakHelper;
        private readonly SmartScanService _smartScanService;
        private readonly StartupManagerService _startupManagerService;
        private List<ScanResult> _lastScanResults;
        private bool _isInitializingTweakStates = false;

        public MainWindow()
        {
            InitializeComponent();

            // Inicializar ViewModel (Cerebro real de la operación)
            var viewModel = new ViewModels.MainWindowViewModel();
            DataContext = viewModel;

            // Inicializar servicios legacy (para compatibilidad temporal)
            _stateManager = TweakStateManager.Instance;
            _telemetry = TelemetryService.Instance;
            _notifications = NotificationService.Instance;
            _tweakHelper = new TweakHelper(_stateManager, _telemetry, _notifications);
            _smartScanService = new SmartScanService();
            _startupManagerService = new StartupManagerService();

            // Inicializar notificaciones
            var rootGrid = (Grid)this.Content;
            _notifications.Initialize(rootGrid);

            // Suscribirse a cambios de estado (UI Sync)
            _stateManager.PropertyChanged += (s, e) =>
            {
                if (_isInitializingTweakStates) return;
                Dispatcher.InvokeAsync(() =>
                {
                    UpdateDashboard();
                    UpdateTweakIndicators();
                });
            };

            // Monitoreo en tiempo real para gráficas
            _stateManager.HardwareMonitor.PropertyChanged += (s, e) =>
            {
                try
                {
                    var monitor = _stateManager.HardwareMonitor;
                    if (e.PropertyName == nameof(monitor.CpuLoadPercent))
                    {
                        GraphCpu.AddValue(monitor.CpuLoadPercent);
                        TxtCpuLoad.Text = $"{monitor.CpuLoadPercent:F0}%";
                    }
                    else if (e.PropertyName == nameof(monitor.GpuLoadPercent))
                    {
                        GraphGpu.AddValue(monitor.GpuLoadPercent);
                        TxtGpuLoad.Text = $"{monitor.GpuLoadPercent:F0}%";
                    }
                    else if (e.PropertyName == nameof(monitor.RamUsagePercent))
                    {
                        GraphRam.AddValue(monitor.RamUsagePercent);
                        TxtRamLoad.Text = $"{monitor.RamUsagePercent:F0}%";
                    }
                    else if (e.PropertyName == nameof(monitor.CpuTemperature))
                    {
                        TxtCpuTemp.Text = $"{monitor.CpuTemperature:F0}°C";
                    }
                    else if (e.PropertyName == nameof(monitor.GpuTemperature))
                    {
                        TxtGpuTemp.Text = $"{monitor.GpuTemperature:F0}°C";
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"⚠️ Error en HardwareMonitor handler: {ex.Message}");
                }
            };

            // Secuencia de inicialización segura
            this.Loaded += async (s, e) =>
            {
                await InitializeAppSafeAsync();
            };
        }

        private async Task InitializeAppSafeAsync()
        {
            // 1. Dejar que la ventana se renderice completamente
            await Task.Delay(300);

            // 2. Registrar inicio y actualizar dashboard inicial
            _telemetry.TrackAppLaunch();
            UpdateDashboard();

            // 3. Verificar privilegios de administrador (No bloqueante al renderizado)
            if (!IsRunAsAdministrator())
            {
                MessageBox.Show(
                    "⚠️ ADVERTENCIA: Esta aplicación requiere permisos de Administrador para aplicar la mayoría de los tweaks.\n\n" +
                    "Por favor, reinicia como Administrador.",
                    "Permisos Requeridos",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            else
            {
                // Solo preguntar por punto de restauración si es admin
                SystemRestore.PromptCreateRestorePoint();
            }

            // 4. Sincronizar UI con el estado actual del sistema
            await Dispatcher.InvokeAsync(() =>
            {
                UpdateTweakIndicators();
                UpdateCurrentPriorityProfile();
                Debug.WriteLine("✅ UI sincronizada");
            });

            // 5. Verificaciones adicionales con delay
            await Task.Delay(1000);
            await CheckBrowserSpeedIssues();

            Debug.WriteLine("🚀 Inicio completado");
        }

        /// <summary>
        /// Verifica si la aplicación se está ejecutando con permisos de administrador
        /// </summary>
        private bool IsRunAsAdministrator()
        {
            var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            var principal = new System.Security.Principal.WindowsPrincipal(identity);
            return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }

        // ═══════════════════════════════════════════════════════════════════
        // CUSTOM TITLE BAR - WINDOW CONTROLS
        // ═══════════════════════════════════════════════════════════════════

        #region Title Bar Controls

        /// <summary>
        /// Permite mover la ventana arrastrando la Title Bar
        /// </summary>
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        /// <summary>
        /// Minimiza la ventana
        /// </summary>
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Cierra la aplicación
        /// </summary>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════════
        // DASHBOARD DINÁMICO
        // ═══════════════════════════════════════════════════════════════════

        #region Dashboard

        /// <summary>
        /// Actualiza las estadísticas del dashboard
        /// </summary>
        private void UpdateDashboard()
        {
            try
            {
                var stats = _stateManager.GetDashboardStats();

                // Actualizar contadores
                TxtActiveTweaks.Text = stats.ActiveTweaks.ToString();
                TxtOptimizationPercent.Text = $"{stats.OptimizationPercentage}%";
                TxtEstimatedFps.Text = $"+{stats.EstimatedFpsGain}";
                TxtLatencyReduction.Text = $"-{stats.EstimatedLatencyReduction}ms ping";
                TxtRamFreed.Text = stats.EstimatedRamFreed.ToString("F1");

                // Mostrar/ocultar sección de tweaks activos
                if (stats.ActiveTweaks > 0)
                {
                    ActiveTweaksSection.Visibility = Visibility.Visible;
                    NoTweaksMessage.Visibility = Visibility.Collapsed;
                    UpdateActiveTweaksList();
                }
                else
                {
                    ActiveTweaksSection.Visibility = Visibility.Collapsed;
                    NoTweaksMessage.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating dashboard: {ex.Message}");
            }
        }

        /// <summary>
        /// Actualiza la lista de tweaks activos en el dashboard
        /// </summary>
        private void UpdateActiveTweaksList()
        {
            try
            {
                ActiveTweaksList.Children.Clear();

                var recentTweaks = _stateManager.GetRecentTweaks(10);

                foreach (var tweak in recentTweaks)
                {
                    var tweakItem = CreateActiveTweakItem(tweak);
                    ActiveTweaksList.Children.Add(tweakItem);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating active tweaks list: {ex.Message}");
            }
        }

        /// <summary>
        /// Crea un elemento visual para un tweak activo
        /// </summary>
        private Border CreateActiveTweakItem(TweakState tweak)
        {
            var nameBlock = new TextBlock
            {
                Text = GetTweakFriendlyName(tweak.Id),
                FontSize = 13,
                FontWeight = FontWeights.Medium,
                Foreground = Brushes.White
            };

            var categoryBlock = new TextBlock
            {
                Text = tweak.Category,
                FontSize = 11,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7F8084"))
            };

            var timeBlock = new TextBlock
            {
                Text = GetRelativeTime(tweak.LastModified),
                FontSize = 10,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5F5F5F")),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center
            };

            var leftStack = new StackPanel();
            leftStack.Children.Add(nameBlock);
            leftStack.Children.Add(categoryBlock);

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            Grid.SetColumn(leftStack, 0);
            Grid.SetColumn(timeBlock, 1);
            grid.Children.Add(leftStack);
            grid.Children.Add(timeBlock);

            var border = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2A2D31")),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(12, 8, 12, 8),
                Margin = new Thickness(0, 0, 0, 5),
                Child = grid
            };

            return border;
        }

        /// <summary>
        /// Obtiene el nombre amigable de un tweak
        /// </summary>
        private string GetTweakFriendlyName(string tweakId)
        {
            var names = new Dictionary<string, string>
            {
                {"MouseAcceleration", "Aceleración del Mouse"},
                {"Keyboard", "Optimización de Teclado"},
                {"VisualEffects", "Efectos Visuales"},
                {"MemoryOptimization", "Optimización de RAM"},
                {"NetworkOptimization", "TCP/IP Optimization"},
                {"DnsCloudflare", "DNS Cloudflare"},
                {"DnsGoogle", "DNS Google"},
                {"DnsCache", "Caché DNS"},
                {"NetworkPower", "Ahorro de Energía de Red"},
                {"NetBios", "NetBIOS over TCP/IP"},
                {"SystemProfile", "System Profile Games"},
                {"GameDVR", "GameDVR / Xbox Game Bar"},
                {"GpuScheduling", "GPU Hardware Scheduling"},
                {"SystemResponsiveness", "System Responsiveness"},
                {"HighPerformance", "Plan Alto Rendimiento"},
                {"PowerThrottling", "Power Throttling"},
                {"CoreParking", "Core Parking"},
                {"Hibernation", "Hibernación"},
                {"WindowsSearch", "Windows Search"},
                {"SysMain", "SysMain (SuperFetch)"},
                {"DiagTrack", "Telemetry (DiagTrack)"},
                {"MPO", "MPO (Multiplane Overlay)"},
                {"UltimatePower", "Ultimate Performance"},
                {"GameBar", "Xbox Game Bar"},
                {"CoreIsolation", "Core Isolation (VBS)"},
                {"HPET", "HPET"},
                {"HyperV", "Hyper-V"},
                {"SpectreMeltdown", "Spectre & Meltdown"},
                // Game Mode Tweaks (NUEVOS)
                {"GameMode", "Windows Game Mode"},
                {"NTFSLastAccess", "NTFS Last Access Time"},
                {"GamePriority", "High Priority for Games"},
                {"Transparency", "Transparency Effects"}
            };

            return names.ContainsKey(tweakId) ? names[tweakId] : tweakId;
        }

        /// <summary>
        /// Obtiene tiempo relativo (ej: "hace 5 min")
        /// </summary>
        private string GetRelativeTime(DateTime dateTime)
        {
            var timeSpan = DateTime.Now - dateTime;

            if (timeSpan.TotalMinutes < 1)
                return "ahora";
            if (timeSpan.TotalMinutes < 60)
                return $"hace {(int)timeSpan.TotalMinutes} min";
            if (timeSpan.TotalHours < 24)
                return $"hace {(int)timeSpan.TotalHours}h";
            if (timeSpan.TotalDays < 7)
                return $"hace {(int)timeSpan.TotalDays}d";

            return dateTime.ToString("dd/MM/yyyy");
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════════
        // SMART SCAN & OPTIMIZATION
        // ═══════════════════════════════════════════════════════════════════

        #region Smart Scan

        private async void BtnStartScan_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            try
            {
                BtnStartScan.IsEnabled = false;
                ScanningState.Visibility = Visibility.Visible;
                ResultState.Visibility = Visibility.Collapsed;
                TxtScanStatus.Text = "Analizando servicios y registro...";

                _lastScanResults = await _smartScanService.ScanAsync();

                int total = _lastScanResults.Count;
                int optimized = _lastScanResults.Count(r => r.IsOptimized);
                int score = (int)((double)optimized / total * 100);

                TxtOptimizationScore.Text = $"{score}%";

                // Cambiar color basado en score
                if (score > 80) TxtOptimizationScore.Foreground = (Brush)new BrushConverter().ConvertFrom("#2ECC71");
                else if (score > 50) TxtOptimizationScore.Foreground = (Brush)new BrushConverter().ConvertFrom("#F1C40F");
                else TxtOptimizationScore.Foreground = (Brush)new BrushConverter().ConvertFrom("#E74C3C");

                int pending = total - optimized;
                if (pending > 0)
                {
                    TxtScanSummary.Text = $"Se encontraron {pending} optimizaciones recomendadas para mejorar tu rendimiento.";
                    BtnOptimizeNow.Visibility = Visibility.Visible;
                }
                else
                {
                    TxtScanSummary.Text = "¡Felicidades! Tu sistema ya está altamente optimizado.";
                    BtnOptimizeNow.Visibility = Visibility.Collapsed;
                }

                ScanningState.Visibility = Visibility.Collapsed;
                ResultState.Visibility = Visibility.Visible;
                BtnStartScan.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                _notifications.ShowError($"Error durante el análisis: {ex.Message}", "Smart Scan");
                ScanningState.Visibility = Visibility.Collapsed;
                BtnStartScan.Visibility = Visibility.Visible;
            }
            finally
            {
                BtnStartScan.IsEnabled = true;
            }
        }

        private async void BtnOptimizeNow_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            try
            {
                var pendingResults = _lastScanResults.Where(r => !r.IsOptimized).ToList();

                if (pendingResults.Count == 0) return;

                BtnOptimizeNow.IsEnabled = false;
                TxtScanSummary.Text = "Aplicando optimizaciones seleccionadas...";

                var pendingIds = pendingResults.Select(r => r.TweakId).ToList();
                await _smartScanService.ApplyRecommendedAsync(pendingIds);

                // Actualizar estados locales y UI
                foreach (var result in pendingResults)
                {
                    _stateManager.SetTweakEnabled(result.TweakId, result.Category);
                }

                _notifications.ShowSuccess("Optimización completada con éxito", "Smart Scan");

                // Mostrar éxito final
                TxtScanSummary.Text = "Tu sistema ahora está totalmente optimizado.";
                BtnOptimizeNow.Visibility = Visibility.Collapsed;
                TxtOptimizationScore.Text = "100%";
                TxtOptimizationScore.Foreground = (Brush)new BrushConverter().ConvertFrom("#2ECC71");

                UpdateDashboard();
                UpdateTweakIndicators();

                // Regresar al botón de analizar después de un tiempo
                await Task.Delay(3000);
                ResultState.Visibility = Visibility.Collapsed;
                BtnStartScan.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                _notifications.ShowError($"Error al aplicar optimizaciones: {ex.Message}", "Smart Scan");
            }
            finally
            {
                BtnOptimizeNow.IsEnabled = true;
            }
        }

        #endregion

        #region Startup Manager

        private void BtnRefreshStartup_Click(object sender, RoutedEventArgs e)
        {
            RefreshStartupList();
        }

        private void RefreshStartupList()
        {
            if (_isInitializingTweakStates) return;
            try
            {
                var items = _startupManagerService.GetStartupItems();
                StartupItemsList.ItemsSource = items;

                TxtNoStartupItems.Visibility = items.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                _notifications.ShowError($"Error al listar programas de inicio: {ex.Message}", "Startup Manager");
            }
        }

        private void BtnDisableStartupItem_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            try
            {
                if (sender is Button btn && btn.Tag is StartupItem item)
                {
                    var result = MessageBox.Show(
                        $"¿Estás seguro de que deseas deshabilitar {item.Name} del inicio de Windows?",
                        "Confirmar Deshabilitación",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        if (_startupManagerService.DisableItem(item))
                        {
                            _notifications.ShowSuccess($"{item.Name} deshabilitado con éxito.", "Startup Manager");
                            RefreshStartupList();
                        }
                        else
                        {
                            _notifications.ShowError($"No se pudo deshabilitar {item.Name}.", "Startup Manager");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _notifications.ShowError($"Error: {ex.Message}", "Startup Manager");
            }
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════════
        // INDICADORES VISUALES DE TWEAKS ACTIVOS
        // ═══════════════════════════════════════════════════════════════════

        #region Indicadores Visuales

        /// <summary>
        /// Actualiza los indicadores visuales de todos los tweaks basándose en el estado guardado
        /// </summary>
        /// <summary>
        /// Actualiza los indicadores visuales de todos los tweaks basándose en el estado guardado
        /// </summary>
        private void UpdateTweakIndicators()
        {
            try
            {
                Debug.WriteLine("═══════════════════════════════════════");
                Debug.WriteLine("🔄 ACTUALIZANDO INDICADORES VISUALES");
                Debug.WriteLine("═══════════════════════════════════════");

                // 1. ACTUALIZAR TOGGLE SWITCHES (CheckBoxes) Y STATUS TEXTS
                // Este método es más preciso para los interruptores modernos
                UpdateNamedToggles();

                // 2. ACTUALIZAR BOTONES GENÉRICOS (Búsqueda por texto)
                // Obtener lista de tweaks activos para logging
                var stats = _stateManager.GetDashboardStats();
                Debug.WriteLine($"📊 Tweaks activos: {stats.ActiveTweaks}");

                // Buscar todos los botones en la interfaz y actualizar su estado visual
                int buttonsUpdated = 0;
                UpdateAllButtonIndicators(this, ref buttonsUpdated);

                Debug.WriteLine($"✅ {buttonsUpdated} botones actualizados con indicadores");
                Debug.WriteLine("═══════════════════════════════════════");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"⚠️ Error actualizando indicadores: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Sincroniza el estado de los interruptores (CheckBox) con el StateManager
        /// </summary>
        private void UpdateNamedToggles()
        {
            _isInitializingTweakStates = true;
            try
            {
                // --- RED & PING (Incluyendo Anti-Bufferbloat) ---
                UpdateToggleState("ecn_capability", ToggleEcnCapability, TxtEcnCapabilityStatus);
                UpdateToggleState("tcp_congestion", ToggleTcpCongestion, TxtTcpCongestionStatus);
                UpdateToggleState("disable_lso", ToggleDisableLSO, TxtDisableLSOStatus);
                UpdateToggleState("qos_prioritization", ToggleQosDscp, TxtQosDscpStatus);
                UpdateToggleState("MTUOptimization", ToggleMTU, TxtMTUStatus);
                UpdateToggleState("AdapterSettings", ToggleAdapterSettings, TxtAdapterSettingsStatus);
                UpdateToggleState("dns_cache", ToggleDnsCache, TxtDnsCacheStatus);
                UpdateToggleState("NetworkOptimization", ToggleNetworkOptimization, null);
                UpdateToggleState("NetworkPower", ToggleNetworkPower, null);
                UpdateToggleState("NetBios", ToggleNetBios, TxtNetBiosStatus);

                // --- SISTEMA & GPU ---
                UpdateToggleState("SystemProfile", ToggleSystemProfile, TxtSystemProfileStatus);
                UpdateToggleState("GameDVR", ToggleGameDVR, TxtGameDVRStatus);
                UpdateToggleState("GpuScheduling", ToggleGpuScheduling, TxtGpuSchedulingStatus);
                UpdateToggleState("SystemResponsiveness", ToggleSystemResponsiveness, TxtSystemResponsivenessStatus);
                UpdateToggleState("HighPerformance", ToggleHighPerformance, TxtHighPerformanceStatus);
                UpdateToggleState("PowerThrottling", TogglePowerThrottling, TxtPowerThrottlingStatus);
                UpdateToggleState("CoreParking", ToggleCoreParking, TxtCoreParkingStatus);
                UpdateToggleState("WindowsGameMode", ToggleGameMode, null);
                UpdateToggleState("NTFSLastAccessTime", ToggleNTFSLastAccess, null);
                UpdateToggleState("GameProcessPriority", ToggleGamePriority, null);

                // --- LIMPIEZA ---
                UpdateToggleState("Hibernation", ToggleHibernation, null);
                UpdateToggleState("WindowsSearch", ToggleWindowsSearch, null);
                UpdateToggleState("SysMain", ToggleSysMain, null);

                // --- GHOST PACK ---
                UpdateToggleState("UltimatePower", ToggleUltimatePower, null);
                UpdateToggleState("GameBar", ToggleGameBar, null);
                UpdateToggleState("CoreIsolation", ToggleCoreIsolation, null);
                UpdateToggleState("HPET", ToggleHPET, null);
                UpdateToggleState("MPOFix", ToggleMPOFix, null);
                UpdateToggleState("HyperV", ToggleHyperV, null);

                // --- ADVANCED ---
                UpdateToggleState("SpectreMeltdown", ToggleSpectreMeltdown, null);
                UpdateToggleState("GpuIRQ", ToggleGpuIRQ, null);
                UpdateToggleState("USBOptimization", ToggleOptimizeUSB, null);
                UpdateToggleState("InputQueues", ToggleInputQueues, null);
                UpdateToggleState("FaultTolerantHeap", ToggleDisableFTH, null);

                // --- INPUT & VISUALS ---
                UpdateToggleState("MouseAcceleration", ToggleMouseAccel, TxtMouseAccelStatus);
                UpdateToggleState("Keyboard", ToggleKeyboard, TxtKeyboardStatus);
                UpdateToggleState("VisualEffects", ToggleVisuals, TxtVisualsStatus);
                UpdateToggleState("MemoryOptimization", ToggleMemory, null);
                UpdateToggleState("TransparencyEffects", ToggleTransparency, TxtTransparencyStatus);
                UpdateToggleState("StickyKeys", ToggleStickyKeys, TxtStickyKeysStatus);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"⚠️ Error en UpdateNamedToggles: {ex.Message}");
            }
            finally
            {
                _isInitializingTweakStates = false;
            }
        }

        /// <summary>
        /// Helper para actualizar un ToggleSwitch y su Texto de estado
        /// </summary>
        private void UpdateToggleState(string tweakId, CheckBox toggle, TextBlock statusText)
        {
            if (toggle == null) return;

            bool isEnabled = _stateManager.IsTweakEnabled(tweakId);

            if (toggle.IsChecked != isEnabled)
            {
                toggle.IsChecked = isEnabled;
            }

            if (statusText != null)
            {
                statusText.Text = isEnabled ? "ON" : "OFF";
                statusText.Foreground = isEnabled ?
                    new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0E7A0D")) : // Verde
                    new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E74C3C")); // Rojo
            }
        }

        /// <summary>
        /// Recorre recursivamente todos los botones y actualiza sus indicadores
        /// </summary>
        private void UpdateAllButtonIndicators(DependencyObject parent, ref int count)
        {
            if (parent == null) return;

            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is Button button)
                {
                    if (UpdateButtonIndicator(button))
                    {
                        count++;
                    }
                }

                // Recursión para elementos anidados
                UpdateAllButtonIndicators(child, ref count);
            }
        }

        /// <summary>
        /// Actualiza el indicador visual de un botón específico
        /// </summary>
        /// <returns>True si el botón fue actualizado</returns>
        private bool UpdateButtonIndicator(Button button)
        {
            try
            {
                var content = button.Content?.ToString();
                if (string.IsNullOrEmpty(content)) return false;

                // Limpiar el contenido de indicadores anteriores
                content = content.Replace("✅ ", "").Replace("⚪ ", "")
                               .Replace("🟢 ", "").Replace("🔴 ", "")
                               .Replace("⚫ ", "").Replace("🟡 ", "")
                               .Replace("[ON] ", "").Replace("[OFF] ", "");

                // Mapeo de textos de botones a IDs de tweaks (búsqueda flexible)
                var tweakMappings = new Dictionary<string, string>
                {
                    // INPUT & VISUALS
                    {"ACELERACIÓN", "MouseAcceleration"},
                    {"TECLADO", "Keyboard"},
                    {"EFECTOS", "VisualEffects"},
                    {"RAM", "MemoryOptimization"},
                    {"TRANSPARENCIA", "TransparencyEffects"},
                    
                    // RED & PING
                    {"TCP/IP", "NetworkOptimization"},
                    {"NAVEGADORES", "BrowserOptimization"},
                    {"MTU", "MTUOptimization"},
                    {"QOS", "QoSConfiguration"},
                    {"AUTO-TUNING", "AutoTuningLevel"},
                    {"ADAPTADOR", "AdapterSettings"},
                    {"CONGESTION", "CongestionControl"},
                    
                    // SISTEMA & GPU
                    {"SYSTEM PROFILE", "SystemProfile"},
                    {"GAMEDVR", "GameDVR"},
                    {"GPU SCHEDULING", "GpuScheduling"},
                    {"RESPONSIVENESS", "SystemResponsiveness"},
                    {"ALTO RENDIMIENTO", "HighPerformance"},
                    {"THROTTLING", "PowerThrottling"},
                    {"CORE PARKING", "CoreParking"},
                    {"GAME MODE", "WindowsGameMode"},
                    {"LAST ACCESS", "NTFSLastAccessTime"},
                    {"PRIORIDAD", "GameProcessPriority"},
                    
                    // LIMPIEZA
                    {"HIBERNACIÓN", "Hibernation"},
                    {"WINDOWS SEARCH", "WindowsSearch"},
                    {"SYSMAIN", "SysMain"},
                    {"TELEMETRÍA", "Telemetry"},
                    
                    // GHOST PACK
                    {"ULTIMATE", "UltimatePower"},
                    {"GAME BAR", "GameBar"},
                    {"CORE ISOLATION", "CoreIsolation"},
                    {"HPET", "HPET"},
                    {"HYPER-V", "HyperV"},
                    {"MPO", "MPOFix"},
                    
                    // ADVANCED
                    {"MITIGACIONES", "SpectreMeltdown"},
                    {"GPU IRQ", "GpuIRQ"},
                    {"USB", "USBOptimization"},
                    {"COLAS", "InputQueues"},
                    {"FTH", "FaultTolerantHeap"},
                    
                    // TOQUES FINALES
                    {"UPDATES", "WindowsUpdateDisable"},
                    {"P2P", "DeliveryOptimization"},
                    {"TRACKING", "TelemetryTracking"},
                    {"SERVICIOS", "TelemetryServices"}
                };

                // Buscar si el botón corresponde a algún tweak (búsqueda parcial)
                string tweakId = null;
                foreach (var mapping in tweakMappings)
                {
                    if (content.ToUpper().Contains(mapping.Key))
                    {
                        tweakId = mapping.Value;
                        break;
                    }
                }

                if (tweakId != null)
                {
                    bool isEnabled = _stateManager.IsTweakEnabled(tweakId);

                    // No modificar botones de "RESTAURAR" o "REVERTIR"
                    if (content.Contains("RESTAURAR") || content.Contains("REVERTIR") ||
                        content.Contains("OFF") || content.Contains("RESET"))
                    {
                        // Estos botones no necesitan indicador
                        return false;
                    }

                    // Aplicar el mismo estilo visual que UpdateButtonState
                    if (isEnabled)
                    {
                        // Tweak ACTIVO - Verde brillante con borde
                        button.Content = "🟢 " + content;
                        button.Opacity = 1.0;
                        button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0E7A0D"));
                        button.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#13A813"));
                        button.BorderThickness = new Thickness(2);

                        Debug.WriteLine($"   🟢 VERDE: {content} (Tweak: {tweakId})");
                        return true;
                    }
                    else
                    {
                        // Tweak DESACTIVADO - Gris oscuro
                        button.Content = "⚫ " + content;
                        button.Opacity = 0.7;
                        button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3A3A3A"));
                        button.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5A5A5A"));
                        button.BorderThickness = new Thickness(1);

                        Debug.WriteLine($"   ⚫ GRIS: {content} (Tweak: {tweakId})");
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error actualizando botón: {ex.Message}");
                return false;
            }
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════════
        // NAVEGACIÓN SIDEBAR (DASHBOARD MODERNO)
        // ═══════════════════════════════════════════════════════════════════

        #region Navegación

        private void NavigateToDashboard(object sender, RoutedEventArgs e)
        {
            _telemetry.TrackPageVisit("Dashboard");
            SwitchView(DashboardPage);
            SetActiveButton((Button)sender);
        }

        private void NavigateToInput(object sender, RoutedEventArgs e)
        {
            _telemetry.TrackPageVisit("Input & Visuals");
            SwitchView(InputPage);
            SetActiveButton((Button)sender);
        }

        private void NavigateToNetwork(object sender, RoutedEventArgs e)
        {
            _telemetry.TrackPageVisit("Red & Ping");
            SwitchView(NetworkPage);
            SetActiveButton((Button)sender);
        }

        private void NavigateToSystem(object sender, RoutedEventArgs e)
        {
            _telemetry.TrackPageVisit("Sistema & GPU");
            SwitchView(SystemPage);
            SetActiveButton((Button)sender);
        }

        private void NavigateToCleanup(object sender, RoutedEventArgs e)
        {
            _telemetry.TrackPageVisit("Limpieza");
            SwitchView(CleanupPage);
            SetActiveButton((Button)sender);
        }

        private void NavigateToStartup(object sender, RoutedEventArgs e)
        {
            _telemetry.TrackPageVisit("Startup Manager");
            SwitchView(StartupPage);
            SetActiveButton((Button)sender);
            RefreshStartupList();
        }

        private void NavigateToGhost(object sender, RoutedEventArgs e)
        {
            _telemetry.TrackPageVisit("GHOST Pack");
            SwitchView(GhostPage);
            SetActiveButton((Button)sender);
        }

        private void NavigateToAdvanced(object sender, RoutedEventArgs e)
        {
            _telemetry.TrackPageVisit("Advanced");
            SwitchView(AdvancedPage);
            SetActiveButton((Button)sender);
        }

        private void NavigateToAdvancedLatency(object sender, RoutedEventArgs e)
        {
            _telemetry.TrackPageVisit("Advanced Latency");
            SwitchView(AdvancedLatencyPage);
            SetActiveButton((Button)sender);
        }

        private void NavigateToLaptop(object sender, RoutedEventArgs e)
        {
            _telemetry.TrackPageVisit("Laptop");
            SwitchView(LaptopPage);
            SetActiveButton((Button)sender);
        }

        private void NavigateToCompetitive(object sender, RoutedEventArgs e)
        {
            _telemetry.TrackPageVisit("Competitive Gaming");
            SwitchView(CompetitivePage);
            SetActiveButton((Button)sender);
        }

        /// <summary>
        /// Navega al Debloat Wizard (ventana modal independiente)
        /// </summary>
        private void NavigateToDebloatWizard(object sender, RoutedEventArgs e)
        {
            _telemetry.TrackPageVisit("Debloat Wizard");

            // Abrir ventana modal del Debloat Wizard
            var debloatWindow = new Windows.DebloatWindow();
            debloatWindow.Owner = this;
            debloatWindow.ShowDialog();

            // Actualizar dashboard después de cerrar el wizard
            // (en caso de que se hayan eliminado apps)
            UpdateDashboard();
            UpdateTweakIndicators();
        }

        /// <summary>
        /// Cambia a una nueva vista con animación FadeIn suave
        /// </summary>
        private void SwitchView(UIElement newView)
        {
            // Ocultar todas las páginas
            DashboardPage.Visibility = Visibility.Collapsed;
            InputPage.Visibility = Visibility.Collapsed;
            NetworkPage.Visibility = Visibility.Collapsed;
            SystemPage.Visibility = Visibility.Collapsed;
            CleanupPage.Visibility = Visibility.Collapsed;
            StartupPage.Visibility = Visibility.Collapsed;
            GhostPage.Visibility = Visibility.Collapsed;
            AdvancedPage.Visibility = Visibility.Collapsed;
            LaptopPage.Visibility = Visibility.Collapsed;
            CompetitivePage.Visibility = Visibility.Collapsed;

            // Mostrar la página seleccionada
            newView.Visibility = Visibility.Visible;

            // Aplicar FadeInAnimation
            try
            {
                var animation = (System.Windows.Media.Animation.Storyboard)FindResource("FadeInAnimation");

                // Clonar el Storyboard para evitar conflictos si se llama múltiples veces rápidamente
                var storyboardCopy = animation.Clone();

                // Aplicar la animación al elemento
                storyboardCopy.Begin((FrameworkElement)newView);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"⚠️ Error al aplicar animación de navegación: {ex.Message}");
                // Si falla la animación, al menos la vista se muestra correctamente
            }
        }

        /// <summary>
        /// Marca el botón activo en el sidebar
        /// </summary>
        private void SetActiveButton(Button activeButton)
        {
            // Remover estado "Active" de todos los botones
            BtnNavDashboard.Tag = null;
            BtnNavInput.Tag = null;
            BtnNavNetwork.Tag = null;
            BtnNavSystem.Tag = null;
            BtnNavCleanup.Tag = null;
            BtnNavStartup.Tag = null;
            BtnNavGhost.Tag = null;
            BtnNavAdvanced.Tag = null;
            BtnNavLaptop.Tag = null;
            BtnNavCompetitive.Tag = null;

            // Marcar el botón activo
            activeButton.Tag = "Active";
        }

        /// <summary>
        /// Acción del dashboard: Crear punto de restauración
        /// </summary>
        private void CreateRestorePoint(object sender, RoutedEventArgs e)
        {
            SystemRestore.CreateRestorePoint($"Tweaker Backup - {System.DateTime.Now:yyyy-MM-dd HH:mm}");

            MessageBox.Show(
                "✅ Creando punto de restauración...\n\n" +
                "Esto puede tomar 1-3 minutos (background).\n\n" +
                "Verificar: Windows + R → rstrui.exe",
                "Punto de Restauración",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        /// <summary>
        /// Abre la ventana de Restauración del Sistema de Windows
        /// </summary>
        private void OpenSystemRestore(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "rstrui.exe",
                    UseShellExecute = true,
                    Verb = "runas" // Ejecutar como administrador
                };

                Process.Start(psi);

                MessageBox.Show(
                    "🔄 RESTAURACIÓN DEL SISTEMA ABIERTA\n\n" +
                    "═══════════════════════════════════════\n" +
                    "INSTRUCCIONES:\n" +
                    "═══════════════════════════════════════\n\n" +
                    "1. Selecciona un punto de restauración\n" +
                    "   (busca: \"Tweaker Backup\")\n\n" +
                    "2. Haz click en 'Siguiente'\n\n" +
                    "3. Confirma la restauración\n" +
                    "4. El sistema se reiniciará automáticamente\n\n" +
                    "⚠️ IMPORTANTE:\n" +
                    "• Cierra todas las aplicaciones abiertas\n" +
                    "• Guarda tu trabajo antes de continuar\n" +
                    "• La restauración puede tardar 10-30 minutos\n" +
                    "• No apagues el PC durante el proceso\n\n" +
                    "💡 Si algo sale mal, puedes deshacer\n" +
                    "   la restauración desde el mismo menú.",
                    "Restauración del Sistema",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"❌ Error al abrir System Restore:\n\n{ex.Message}\n\n" +
                    "Puedes abrirlo manualmente:\n" +
                    "Windows + R → rstrui.exe",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        #endregion


        // ═══════════════════════════════════════════════════════════════════
        // CATEGORÍA 1: GPU & SISTEMA
        // ═══════════════════════════════════════════════════════════════════

        #region GPU & Sistema

        private void BtnSystemProfile_On_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            _tweakHelper.ExecuteTweak(
                "SystemProfile",
                "Sistema & GPU",
                () => GpuOptimization.EnableSystemProfileOptimization(),
                "System Profile configurado para Games. GPU Priority: 8, CPU Priority: 6.",
                null,
                true,
                showNotification: false
            );

            // Actualizar estado visual del toggle
            ToggleSystemProfile.IsChecked = true;
            TxtSystemProfileStatus.Text = "ON";
            TxtSystemProfileStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(14, 122, 13)); // Verde
        }

        private void BtnSystemProfile_Off_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            _tweakHelper.ExecuteTweakRevert(
                "SystemProfile",
                "Sistema & GPU",
                () => GpuOptimization.DisableSystemProfileOptimization(),
                "System Profile restaurado a valores predeterminados.",
                null,
                showNotification: false


            );

            // Actualizar estado visual del toggle
            ToggleSystemProfile.IsChecked = false;
            TxtSystemProfileStatus.Text = "OFF";
            TxtSystemProfileStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(231, 76, 60)); // Rojo
        }

        /// <summary>
        /// Actualiza el estado visual de un botón específico con colores y estilos claros
        /// </summary>
        private void UpdateButtonState(Button button, bool isActive)
        {
            if (button == null) return;

            var content = button.Content?.ToString() ?? "";
            // Limpiar todos los indicadores anteriores
            content = content.Replace("✅ ", "").Replace("⚪ ", "")
                           .Replace("🟢 ", "").Replace("🔴 ", "")
                           .Replace("[ON] ", "").Replace("[OFF] ", "");

            if (isActive)
            {
                // ESTADO ACTIVO - Verde brillante
                button.Content = "🟢 " + content;
                button.Opacity = 1.0;
                button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0E7A0D")); // Verde oscuro
                button.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#13A813")); // Verde brillante
                button.BorderThickness = new Thickness(2);

                Debug.WriteLine($"🟢 Botón ACTIVADO: {content}");
            }
            else
            {
                // ESTADO DESACTIVADO - Gris/Rojo
                button.Content = "⚫ " + content;
                button.Opacity = 0.7;
                button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3A3A3A")); // Gris oscuro
                button.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5A5A5A")); // Gris
                button.BorderThickness = new Thickness(1);

                Debug.WriteLine($"⚫ Botón DESACTIVADO: {content}");
            }
        }

        /// <summary>
        /// Busca y actualiza el botón ON relacionado con un tweak
        /// </summary>
        private void UpdateRelatedOnButton(string searchText, bool isActive)
        {
            if (_isInitializingTweakStates) return;
            try
            {
                FindAndUpdateButton(this, searchText, isActive);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error actualizando botón relacionado: {ex.Message}");
            }
        }

        /// <summary>
        /// Busca recursivamente un botón por su contenido y lo actualiza
        /// </summary>
        private void FindAndUpdateButton(DependencyObject parent, string searchText, bool isActive)
        {
            if (parent == null) return;

            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is Button button)
                {
                    var content = button.Content?.ToString() ?? "";
                    if (content.Contains(searchText, StringComparison.OrdinalIgnoreCase) &&
                        !content.Contains("RESTAURAR") && !content.Contains("OFF"))
                    {
                        UpdateButtonState(button, isActive);
                        return;
                    }
                }

                FindAndUpdateButton(child, searchText, isActive);
            }
        }

        private void BtnGameDVR_On_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            _tweakHelper.ExecuteTweak(
                "GameDVR",
                "Sistema & GPU",
                () => GpuOptimization.DisableGameDVR(),
                "GameDVR y Xbox Game Bar deshabilitados. Input lag -5-15ms, FPS +10-30%.",
                null,
                true,
                showNotification: false
            );

            // Actualizar estado visual del toggle
            ToggleGameDVR.IsChecked = true;
            TxtGameDVRStatus.Text = "ON";
            TxtGameDVRStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(14, 122, 13)); // Verde
        }

        private void BtnGameDVR_Off_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            _tweakHelper.ExecuteTweakRevert(
                "GameDVR",
                "Sistema & GPU",
                () => GpuOptimization.EnableGameDVR(),
                "GameDVR y Xbox Game Bar restaurados.",
                null,
                true,
                showNotification: false
            );

            // Actualizar estado visual del toggle
            ToggleGameDVR.IsChecked = false;
            TxtGameDVRStatus.Text = "OFF";
            TxtGameDVRStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(231, 76, 60)); // Rojo
        }

        private void BtnGpuScheduling_On_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            _tweakHelper.ExecuteTweak(
                "GpuScheduling",
                "Sistema & GPU",
                () => GpuOptimization.EnableHardwareAcceleratedGPUScheduling(),
                "Hardware GPU Scheduling activado. Puede mejorar o empeorar latencia (probar ambos).",
                null,
                true,
                showNotification: false
            );

            // Actualizar estado visual del toggle
            ToggleGpuScheduling.IsChecked = true;
            TxtGpuSchedulingStatus.Text = "ON";
            TxtGpuSchedulingStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(14, 122, 13)); // Verde
        }

        private void BtnGpuScheduling_Off_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            _tweakHelper.ExecuteTweakRevert(
                "GpuScheduling",
                "Sistema & GPU",
                () => GpuOptimization.DisableHardwareAcceleratedGPUScheduling(),
                "Hardware GPU Scheduling desactivado.",
                null,
                true,
                showNotification: false
            );

            // Actualizar estado visual del toggle
            ToggleGpuScheduling.IsChecked = false;
            TxtGpuSchedulingStatus.Text = "OFF";
            TxtGpuSchedulingStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(231, 76, 60)); // Rojo
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════════
        // CATEGORÍA 2: CPU (RYZEN/INTEL)
        // ═══════════════════════════════════════════════════════════════════

        #region CPU Optimizations

        private void BtnSystemResponsiveness_On_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            _tweakHelper.ExecuteTweak(
                "SystemResponsiveness",
                "Sistema & GPU",
                () => CpuOptimization.EnableSystemResponsivenessOptimization(),
                "System Responsiveness optimizado. NetworkThrottling OFF, ping reducido 5-20ms.",
                null,
                true,
                showNotification: false
            );

            // Actualizar estado visual del toggle
            ToggleSystemResponsiveness.IsChecked = true;
            TxtSystemResponsivenessStatus.Text = "ON";
            TxtSystemResponsivenessStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(14, 122, 13)); // Verde
        }

        private void BtnSystemResponsiveness_Off_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            _tweakHelper.ExecuteTweakRevert(
                "SystemResponsiveness",
                "Sistema & GPU",
                () => CpuOptimization.DisableSystemResponsivenessOptimization(),
                "System Responsiveness restaurado a valores predeterminados.",
                null,
                true,
                showNotification: false
            );

            // Actualizar estado visual del toggle
            ToggleSystemResponsiveness.IsChecked = false;
            TxtSystemResponsivenessStatus.Text = "OFF";
            TxtSystemResponsivenessStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(231, 76, 60)); // Rojo
        }

        private void BtnHighPerformance_On_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            _tweakHelper.ExecuteTweak(
                "HighPerformance",
                "Sistema & GPU",
                () => CpuOptimization.EnableHighPerformancePowerPlan(),
                "Plan Alto Rendimiento activado. CPU siempre a máxima frecuencia.",
                null, // errorMessage
                false, // requiresRestart (default)
                false, // createRestorePoint (default)
                showNotification: false
            );

            // Actualizar estado visual del toggle
            ToggleHighPerformance.IsChecked = true;
            TxtHighPerformanceStatus.Text = "ON";
            TxtHighPerformanceStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(14, 122, 13)); // Verde
        }

        private void BtnHighPerformance_Off_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            _tweakHelper.ExecuteTweakRevert(
                "HighPerformance",
                "Sistema & GPU",
                () => CpuOptimization.EnableBalancedPowerPlan(),
                "Plan Balanceado activado. CPU ajustará frecuencia según uso.",
                null,
                false, // requiresRestart = false
                showNotification: false
            );

            // Actualizar estado visual del toggle
            ToggleHighPerformance.IsChecked = false;
            TxtHighPerformanceStatus.Text = "OFF";
            TxtHighPerformanceStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(231, 76, 60)); // Rojo
        }

        private void BtnPowerThrottling_On_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            _tweakHelper.ExecuteTweak(
                "PowerThrottling",
                "Sistema & GPU",
                () => CpuOptimization.DisablePowerThrottling(),
                "Power Throttling deshabilitado. Apps en background sin limitaciones.",
                null,
                true, // requiresRestart
                false, // createRestorePoint (default)
                showNotification: false
            );

            // Actualizar estado visual del toggle
            TogglePowerThrottling.IsChecked = true;
            TxtPowerThrottlingStatus.Text = "ON";
            TxtPowerThrottlingStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(14, 122, 13)); // Verde
        }

        private void BtnPowerThrottling_Off_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            _tweakHelper.ExecuteTweakRevert(
                "PowerThrottling",
                "Sistema & GPU",
                () => CpuOptimization.EnablePowerThrottling(),
                "Power Throttling restaurado.",
                null,
                true,
                showNotification: false
            );

            // Actualizar estado visual del toggle
            TogglePowerThrottling.IsChecked = false;
            TxtPowerThrottlingStatus.Text = "OFF";
            TxtPowerThrottlingStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(231, 76, 60)); // Rojo
        }

        private void BtnCoreParking_On_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            _tweakHelper.ExecuteTweak(
                "CoreParking",
                "Sistema & GPU",
                () => CpuOptimization.DisableCoreParking(),
                "Core Parking deshabilitado. Todos los cores permanecen activos (crítico en Ryzen).",
                null,
                true,
                showNotification: false
            );

            // Actualizar estado visual del toggle
            ToggleCoreParking.IsChecked = true;
            TxtCoreParkingStatus.Text = "ON";
            TxtCoreParkingStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(14, 122, 13)); // Verde
        }

        private void BtnCoreParking_Off_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            _tweakHelper.ExecuteTweakRevert(
                "CoreParking",
                "Sistema & GPU",
                () => CpuOptimization.EnableCoreParking(),
                "Core Parking restaurado. Windows puede aparcar cores no utilizados.",
                null,
                true,
                showNotification: false
            );

            // Actualizar estado visual del toggle
            ToggleCoreParking.IsChecked = false;
            TxtCoreParkingStatus.Text = "OFF";
            TxtCoreParkingStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(231, 76, 60)); // Rojo
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════════
        // CATEGORÍA 3: WINDOWS BLOATWARE
        // ═══════════════════════════════════════════════════════════════════

        #region Windows Bloatware

        private void BtnHibernation_On_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            _tweakHelper.ExecuteTweak(
                "Hibernation",
                "Limpieza",
                () => WindowsOptimization.DisableHibernation(),
                "Hibernación deshabilitada. Archivo hiberfil.sys eliminado (8-32GB liberados).",
                null,
                true,
                showNotification: false
            );

            // Actualizar estado visual del toggle
            ToggleHibernation.IsChecked = true;
            TxtHibernationStatus.Text = "ON";
            TxtHibernationStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(14, 122, 13)); // Verde
        }

        private void BtnHibernation_Off_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            _tweakHelper.ExecuteTweakRevert(
                "Hibernation",
                "Limpieza",
                () => WindowsOptimization.EnableHibernation(),
                "Hibernación restaurada.",
                null,
                true,
                showNotification: false
            );

            // Actualizar estado visual del toggle
            ToggleHibernation.IsChecked = false;
            TxtHibernationStatus.Text = "OFF";
            TxtHibernationStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(231, 76, 60)); // Rojo
        }

        private void BtnWindowsSearch_On_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            _tweakHelper.ExecuteTweak(
                "WindowsSearch",
                "Limpieza",
                () => WindowsOptimization.DisableWindowsSearch(),
                "Windows Search deshabilitado. Indexación detenida, 200-500MB RAM liberados.",
                null, // errorMessage
                false, // requiresRestart (default)
                false, // createRestorePoint (default)
                showNotification: false
            );

            // Actualizar estado visual del toggle
            ToggleWindowsSearch.IsChecked = true;
            TxtWindowsSearchStatus.Text = "ON";
            TxtWindowsSearchStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(14, 122, 13)); // Verde
        }

        private void BtnWindowsSearch_Off_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            _tweakHelper.ExecuteTweakRevert(
                "WindowsSearch",
                "Limpieza",
                () => WindowsOptimization.EnableWindowsSearch(),
                "Windows Search restaurado. Servicio de indexación activo.",
                null,
                false, // requiresRestart = false (default)
                showNotification: false
            );

            // Actualizar estado visual del toggle
            ToggleWindowsSearch.IsChecked = false;
            TxtWindowsSearchStatus.Text = "OFF";
            TxtWindowsSearchStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(231, 76, 60)); // Rojo
        }

        private void BtnSysMain_On_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            _tweakHelper.ExecuteTweak(
                "SysMain",
                "Limpieza",
                () => WindowsOptimization.DisableSysMain(),
                "SysMain (SuperFetch) deshabilitado. 1-3GB RAM liberados, uso de disco reducido.",
                null,
                true, // requiresRestart
                false, // createRestorePoint (default)
                showNotification: false
            );

            // Actualizar estado visual del toggle
            ToggleSysMain.IsChecked = true;
            TxtSysMainStatus.Text = "ON";
            TxtSysMainStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(14, 122, 13)); // Verde
        }

        private void BtnSysMain_Off_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            _tweakHelper.ExecuteTweakRevert(
                "SysMain",
                "Limpieza",
                () => WindowsOptimization.EnableSysMain(),
                "SysMain (SuperFetch) restaurado. Pre-carga de apps habilitada.",
                null,
                true,
                showNotification: false
            );

            // Actualizar estado visual del toggle
            ToggleSysMain.IsChecked = false;
            TxtSysMainStatus.Text = "OFF";
            TxtSysMainStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(231, 76, 60)); // Rojo
        }

        private void BtnTelemetry_On_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            _tweakHelper.ExecuteTweak(
                "Telemetry",
                "Limpieza",
                () => WindowsOptimization.DisableTelemetry(),
                "Telemetría deshabilitada. Windows dejará de enviar datos a Microsoft.",
                null, // errorMessage
                false, // requiresRestart (default)
                false, // createRestorePoint (default)
                showNotification: false
            );
            UpdateButtonState((Button)sender, true);
        }

        private void BtnTelemetry_Off_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            _tweakHelper.ExecuteTweakRevert(
                "Telemetry",
                "Limpieza",
                () => WindowsOptimization.EnableTelemetry(),
                "Telemetría restaurada. Servicios de telemetría activos.",
                null,
                false, // requiresRestart = false (default)
                showNotification: false
            );
            UpdateRelatedOnButton("TELEMETRÍA", false);
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════════
        // CATEGORÍA 4: RED Y PING
        // ═══════════════════════════════════════════════════════════════════

        #region Red y Ping

        private void BtnNetworkOptimization_On_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            try
            {
                bool success = NetworkOptimization.OptimizeNetwork();

                if (success)
                {
                    // Actualizar estado visual del toggle
                    ToggleNetworkOptimization.IsChecked = true;
                    TxtNetworkOptimizationStatus.Text = "ON";
                    TxtNetworkOptimizationStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(14, 122, 13)); // Verde

                    MessageBox.Show(
                        "✅ OPTIMIZACIÓN DE RED APLICADA (ADAMX TWEAKS)\n\n" +
                        "═══════════════════════════════════════\n" +
                        "TWEAKS TCP/IP EN INTERFAZ ACTIVA:\n" +
                        "═══════════════════════════════════════\n" +
                        "• TcpAckFrequency: 1\n" +
                        "  → ACK inmediato sin esperar\n" +
                        "  → REDUCE PING EN 10-40ms\n\n" +
                        "• TCPNoDelay: 1\n" +
                        "  → Deshabilita Nagle's Algorithm\n" +
                        "  → Envía paquetes inmediatamente\n\n" +
                        "• TcpDelAckTicks: 0\n" +
                        "  → Sin delay artificial de ACK\n\n" +
                        "═══════════════════════════════════════\n" +
                        "TWEAKS GLOBALES DEL SISTEMA:\n" +
                        "═══════════════════════════════════════\n" +
                        "• NetworkThrottlingIndex: FFFFFFFF\n" +
                        "  → Elimina throttling de red\n" +
                        "  → Sin límite de paquetes por segundo\n\n" +
                        "• SystemResponsiveness: 0\n" +
                        "  → TODO el CPU disponible para gaming\n\n" +
                        "═══════════════════════════════════════\n" +
                        "BENEFICIOS EN GAMING COMPETITIVO:\n" +
                        "═══════════════════════════════════════\n" +
                        "✓ Reduce ping efectivo en 5-30ms\n" +
                        "✓ Mejora hitreg (registro de disparos)\n" +
                        "✓ Elimina \"rubber banding\"\n" +
                        "✓ Elimina packet loss artificial\n" +
                        "✓ Mejora \"peeker's advantage\"\n" +
                        "✓ CRÍTICO para Valorant, CS2, COD\n\n" +
                        "⚠️ REINICIA Windows para efecto completo.\n\n" +
                        "📝 NOTA: La interfaz de red fue detectada\n" +
                        "automáticamente y optimizada.",
                        "Optimización de Red Aplicada",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(
                        "⚠️ ADVERTENCIA: Optimización parcial o fallida\n\n" +
                        "Posibles causas:\n" +
                        "• No se detectó ninguna interfaz de red activa\n" +
                        "• Permisos de administrador insuficientes\n" +
                        "• Driver de red no compatible\n\n" +
                        "SOLUCIÓN:\n" +
                        "1. Verifica que tu adaptador de red esté activo\n" +
                        "2. Ejecuta la app como Administrador\n" +
                        "3. Verifica en Regedit si se aplicaron cambios en:\n" +
                        "   HKLM\\SYSTEM\\CurrentControlSet\\Services\\Tcpip\\Parameters\\Interfaces\n\n" +
                        "Revisa el Output de Visual Studio para más detalles.",
                        "Advertencia",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"❌ ERROR AL OPTIMIZAR RED\n\n" +
                    $"Detalles técnicos:\n{ex.Message}\n\n" +
                    $"SOLUCIÓN:\n" +
                    $"• Ejecuta como Administrador\n" +
                    $"• Verifica que tu red esté activa\n" +
                    $"• Revisa el Output de Visual Studio",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void BtnNetworkOptimization_Off_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            try
            {
                bool success = NetworkOptimization.RestoreNetwork();

                if (success)
                {
                    // Actualizar estado visual del toggle
                    ToggleNetworkOptimization.IsChecked = false;
                    TxtNetworkOptimizationStatus.Text = "OFF";
                    TxtNetworkOptimizationStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(231, 76, 60)); // Rojo

                    MessageBox.Show(
                        "✅ CONFIGURACIÓN DE RED RESTAURADA\n\n" +
                        "Los tweaks de red han sido eliminados/restaurados:\n\n" +
                        "INTERFACES TCP/IP:\n" +
                        "• TcpAckFrequency: ELIMINADO (Windows usará default)\n" +
                        "• TCPNoDelay: ELIMINADO (Nagle's habilitado)\n" +
                        "• TcpDelAckTicks: ELIMINADO (delay default)\n\n" +
                        "SISTEMA:\n" +
                        "• NetworkThrottlingIndex: 10 (default)\n" +
                        "• SystemResponsiveness: 20 (default)\n\n" +
                        "⚠️ REINICIA Windows para efecto completo.\n\n" +
                        "Tu red volverá a comportarse como Windows\n" +
                        "la configuró originalmente.",
                        "Configuración Restaurada",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(
                        "⚠️ Hubo problemas al restaurar algunos valores.\n\n" +
                        "Es posible que algunos tweaks no existieran.\n" +
                        "Verifica en Regedit si lo necesitas.\n\n" +
                        "⚠️ REINICIA Windows de todas formas.",
                        "Advertencia",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"❌ Error al restaurar red: {ex.Message}\n\n" +
                    $"Ejecuta como Administrador.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════════
        // CATEGORÍA 4.5: OPTIMIZACIÓN DE NAVEGADORES
        // ═══════════════════════════════════════════════════════════════════

        #region Optimización de Navegadores

        /// <summary>
        /// Aplica tweaks balanceados para corregir lentitud en navegadores
        /// manteniendo beneficios para gaming
        /// </summary>
        private void BtnBrowserOptimization_Apply_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            try
            {
                bool success = BrowserOptimization.ApplyBrowserBalancedTweaks();

                if (success)
                {
                    // Usar TweakHelper para registrar estado
                    _stateManager.SetTweakEnabled("BrowserOptimization", "Navegadores");
                    _telemetry.TrackTweakEnabled("BrowserOptimization", "Navegadores");

                    MessageBox.Show(
                        "✅ NAVEGADORES OPTIMIZADOS\n\n" +
                        "Se aplicaron tweaks balanceados para mejorar navegación web:\n\n" +
                        "CAMBIOS APLICADOS:\n" +
                        "• NetworkThrottlingIndex: 5 (balanceado)\n" +
                        "• TcpAckFrequency: 2 (menos agresivo)\n" +
                        "• TcpDelAckTicks: 1 (reduce overhead)\n" +
                        "• DNS Cache optimizado para navegadores\n" +
                        "• TcpWindowSize configurado para mejor throughput\n\n" +
                        "BENEFICIOS:\n" +
                        "🌐 Navegadores cargan páginas más rápido\n" +
                        "🎮 Gaming performance mantenido\n" +
                        "⚡ Balance optimal entre gaming y navegación\n\n" +
                        "📋 PRÓXIMOS PASOS:\n" +
                        "1. Reinicia todos los navegadores abiertos\n" +
                        "2. Prueba cargar sitios web\n" +
                        "3. Verifica que gaming sigue funcionando bien",
                        "Navegadores Optimizados",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    _notifications.ShowSuccess(
                        "Navegadores optimizados con tweaks balanceados. NetworkThrottlingIndex=5, TCP moderado, DNS optimizado.",
                        "Tweak Activado");
                }
                else
                {
                    MessageBox.Show(
                        "⚠️ OPTIMIZACIÓN PARCIAL\n\n" +
                        "Algunos tweaks no pudieron aplicarse:\n\n" +
                        "POSIBLES CAUSAS:\n" +
                        "• No se detectó ninguna interfaz de red activa\n" +
                        "• Permisos de administrador insuficientes\n" +
                        "• Algunos valores ya estaban optimizados\n" +
                        "• Antivirus bloqueando cambios de registro\n\n" +
                        "SOLUCIÓN:\n" +
                        "1. Verifica que tu adaptador de red esté conectado\n" +
                        "2. Ejecuta la app como Administrador\n" +
                        "3. Deshabilita temporalmente el antivirus\n" +
                        "4. Revisa el Output de Visual Studio\n\n" +
                        "Algunos cambios pueden haberse aplicado correctamente.",
                        "Advertencia",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    _notifications.ShowError("Error al optimizar navegadores. Verifica que tu adaptador de red esté conectado y activo.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"❌ ERROR AL OPTIMIZAR NAVEGADORES\n\n" +
                    $"Detalles técnicos:\n{ex.Message}\n\n" +
                    $"SOLUCIÓN:\n" +
                    $"• Ejecuta como Administrador\n" +
                    $"• Verifica permisos de registro\n" +
                    $"• Revisa el Output de Visual Studio",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                _notifications.ShowError($"Excepción al optimizar navegadores: {ex.Message}");
            }
        }

        /// <summary>
        /// Restaura tweaks extremos para gaming puro
        /// </summary>
        private void BtnBrowserOptimization_Revert_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            var result = MessageBox.Show(
                "⚠️ RESTAURAR TWEAKS EXTREMOS PARA GAMING\n\n" +
                "Esta opción restaurará los tweaks extremos optimizados\n" +
                "únicamente para gaming, lo que puede causar lentitud\n" +
                "en navegadores web.\n\n" +
                "CAMBIOS:\n" +
                "• NetworkThrottlingIndex: FFFFFFFF (extremo)\n" +
                "• TcpAckFrequency: 1 (máximo gaming)\n" +
                "• TcpDelAckTicks: 0 (sin delay)\n" +
                "• DNS Cache sin limitaciones\n\n" +
                "RESULTADO:\n" +
                "🎮 Gaming: Máximo rendimiento\n" +
                "🌐 Navegadores: Posible lentitud\n\n" +
                "⚠️ REQUISITOS:\n" +
                "• Reiniciar aplicaciones abiertas\n" +
                "• Reiniciar Windows para efecto completo\n\n" +
                "¿Continuar restaurando tweaks extremos para gaming?",
                "Confirmación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
                return;

            try
            {
                bool success = BrowserOptimization.RestoreGamingOnlyTweaks();

                if (success)
                {
                    // Usar TweakHelper para registrar estado
                    _stateManager.SetTweakDisabled("BrowserOptimization");
                    _telemetry.TrackTweakDisabled("BrowserOptimization", "Navegadores");

                    MessageBox.Show(
                        "🎮 TWEAKS EXTREMOS GAMING RESTAURADOS\n\n" +
                        "Se aplicaron tweaks máximos para gaming:\n\n" +
                        "CONFIGURACIÓN APLICADA:\n" +
                        "• NetworkThrottlingIndex: FFFFFFFF\n" +
                        "• TcpAckFrequency: 1\n" +
                        "• TcpDelAckTicks: 0\n" +
                        "• DNS Cache sin limitaciones\n\n" +
                        "IMPACTO:\n" +
                        "🎮 Gaming: Latencia mínima\n" +
                        "⚠️ Navegadores: Pueden cargar más lento\n\n" +
                        "Si experimentas lentitud en navegadores,\n" +
                        "usa 'Optimizar Navegadores' para balance.",
                        "Gaming Extremo Activado",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    _notifications.ShowInfo(
                        "Tweaks extremos para gaming restaurados. NetworkThrottlingIndex=FFFFFFFF, TCP agresivo.",
                        "Tweak Revertido");
                }
                else
                {
                    _notifications.ShowError("Error al restaurar tweaks extremos. Verifica los permisos de administrador.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"❌ Error: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                _notifications.ShowError($"Excepción al restaurar tweaks: {ex.Message}");
            }
        }

        /// <summary>
        /// Diagnóstica problemas específicos de navegadores
        /// </summary>
        private void BtnBrowserDiagnose_Click(object sender, RoutedEventArgs e)
        {
            string diagnosis = BrowserOptimization.DiagnoseBrowserIssues();

            MessageBox.Show(
                $"🔍 DIAGNÓSTICO DE NAVEGADORES\n\n" +
                $"{diagnosis}\n\n" +
                $"SCRIPTS ADICIONALES:\n" +
                $"• DiagnoseBrowserSlowness.ps1 (análisis detallado)\n" +
                $"• FixBrowserSlowness.ps1 (reparación automática)\n\n" +
                $"Revisa el Output de Visual Studio para más detalles.",
                "Diagnóstico Navegadores",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════════
        // CATEGORÍA 5: SERVICIOS (DEBLOAT)
        // ═══════════════════════════════════════════════════════════════════

        #region Servicios (Debloat)

        private void BtnSysMainService_On_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            _tweakHelper.ExecuteTweak(
                "SysMainService",
                "Limpieza",
                () => ServiceOptimization.DisableSysMain(),
                "SysMain (SuperFetch) servicio deshabilitado. 1-3GB RAM liberados.",
                null, // errorMessage
                false, // requiresRestart (default)
                false, // createRestorePoint (default)
                showNotification: false
            );
        }

        private void BtnSysMainService_Off_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            _tweakHelper.ExecuteTweakRevert(
                "SysMainService",
                "Limpieza",
                () => ServiceOptimization.EnableSysMain(),
                "SysMain (SuperFetch) servicio restaurado.",
                null,
                false, // requiresRestart = false (default)
                showNotification: false
            );
        }

        private void BtnDiagTrack_On_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            _tweakHelper.ExecuteTweak(
                "DiagTrack",
                "Limpieza",
                () => ServiceOptimization.DisableDiagTrack(),
                "DiagTrack (Telemetría) deshabilitado. Sin envío de datos a Microsoft.",
                null, // errorMessage
                false, // requiresRestart (default)
                false, // createRestorePoint (default)
                showNotification: false
            );
        }

        private void BtnDiagTrack_Off_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            _tweakHelper.ExecuteTweakRevert(
                "DiagTrack",
                "Limpieza",
                () => ServiceOptimization.EnableDiagTrack(),
                "DiagTrack (Telemetría) restaurado.",
                null,
                false, // requiresRestart = false (default)
                showNotification: false
            );
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════════
        // CATEGORÍA 6: KERNEL LATENCY (BCD / HPET)
        // ═══════════════════════════════════════════════════════════════════

        #region Kernel Latency (BCD / HPET)

        private void BtnHPET_On_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            _tweakHelper.ExecuteTweak(
                "HPET",
                "GHOST Pack",
                () => LatencyOptimization.DisableHPET(),
                "HPET deshabilitado. Micro-stuttering reducido, frame times mejorados (usa TSC).",
                null,
                true
            );

            // Actualizar estado visual del toggle
            ToggleHPET.IsChecked = true;
            TxtHPETStatus.Text = "ON";
            TxtHPETStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(14, 122, 13)); // Verde
        }

        private void BtnHPET_Off_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            _tweakHelper.ExecuteTweakRevert(
                "HPET",
                "GHOST Pack",
                () => LatencyOptimization.EnableHPET(),
                "HPET restaurado. Windows decidirá automáticamente qué timer usar.",
                null,
                true,
                showNotification: false
            );

            // Actualizar estado visual del toggle
            ToggleHPET.IsChecked = false;
            TxtHPETStatus.Text = "OFF";
            TxtHPETStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(231, 76, 60)); // Rojo
        }

        private void BtnHyperV_On_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            try
            {
                // Confirmación adicional porque deshabilita Docker/WSL2
                var result = MessageBox.Show(
                    "⚠️⚠️⚠️ ADVERTENCIA CRÍTICA ⚠️⚠️⚠️\n\n" +
                    "Estás a punto de DESHABILITAR Hyper-V.\n\n" +
                    "IMPACTO:\n" +
                    "❌ Docker Desktop dejará de funcionar\n" +
                    "❌ WSL2 volverá a WSL1 (más lento)\n" +
                    "❌ Windows Sandbox no funcionará\n" +
                    "❌ VirtualBox puede tener problemas\n\n" +
                    "BENEFICIOS:\n" +
                    "✅ Reduce la latencia de GPU 2-5ms\n" +
                    "✅ Mejora la compatibilidad con anti-cheat (Vanguard)\n" +
                    "✅ Reduce la latencia DPC\n\n" +
                    "⚠️ SOLO deshabilita si NO usas virtualización.\n\n" +
                    "¿Estás COMPLETAMENTE SEGURO de que entiendes los riesgos?",
                    "Confirmación de Seguridad Requerida",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Error);

                if (result != MessageBoxResult.Yes) return;

                _tweakHelper.ExecuteTweak(
                    "HyperV",
                    "GHOST Pack",
                    () => LatencyOptimization.DisableHyperV(),
                    "Hyper-V deshabilitado. Latencia GPU -2-5ms (Docker y WSL2 NO funcionarán).",
                    null,
                    true
                );

                // Actualizar estado visual del toggle
                ToggleHyperV.IsChecked = true;
                TxtHyperVStatus.Text = "ON";
                TxtHyperVStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(14, 122, 13)); // Verde
            }
            catch (Exception ex)
            {
                _notifications.ShowError($"Error: {ex.Message}");
            }
        }

        private void BtnHyperV_Off_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            _tweakHelper.ExecuteTweakRevert(
                "HyperV",
                "GHOST Pack",
                () => LatencyOptimization.EnableHyperV(),
                "Hyper-V restaurado. Docker y WSL2 volverán a funcionar.",
                null,
                true,
                showNotification: false
            );

            // Actualizar estado visual del toggle
            ToggleHyperV.IsChecked = false;
            TxtHyperVStatus.Text = "OFF";
            TxtHyperVStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(231, 76, 60)); // Rojo
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════════
        // CATEGORÍA 7: GHOST Tweaks
        // ═══════════════════════════════════════════════════════════════════

        #region GHOST Pack

        private void BtnUltimatePower_On_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            _tweakHelper.ExecuteTweak(
                "UltimatePower",
                "GHOST Pack",
                () => PowerTweaks.EnableUltimatePerformance(),
                "Ultimate Performance activado. Latencia CPU -93%, 0.1% low FPS +20%."
            );

            // Actualizar estado visual del toggle
            ToggleUltimatePower.IsChecked = true;
            TxtUltimatePowerStatus.Text = "ON";
            TxtUltimatePowerStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(14, 122, 13)); // Verde
        }

        private void BtnUltimatePower_Off_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            _tweakHelper.ExecuteTweakRevert(
                "UltimatePower",
                "GHOST Pack",
                () => PowerTweaks.RestoreBalancedPlan(),
                "Plan Balanced restaurado. Consumo y temperaturas optimizados.",
                null, // errorMessage
                false, // requiresRestart (default)
                showNotification: false
            );

            // Actualizar estado visual del toggle
            ToggleUltimatePower.IsChecked = false;
            TxtUltimatePowerStatus.Text = "OFF";
            TxtUltimatePowerStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(231, 76, 60)); // Rojo
        }

        private void BtnGameBar_On_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            _tweakHelper.ExecuteTweak(
                "GameBar",
                "GHOST Pack",
                () => WindowsDebloat.DisableGameBar(),
                "Xbox Game Bar deshabilitada. Input lag -50%, CPU libre +8%."
            );

            // Actualizar estado visual del toggle
            ToggleGameBar.IsChecked = true;
            TxtGameBarStatus.Text = "ON";
            TxtGameBarStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(14, 122, 13)); // Verde
        }

        private void BtnGameBar_Off_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            _tweakHelper.ExecuteTweakRevert(
                "GameBar",
                "GHOST Pack",
                () => WindowsDebloat.EnableGameBar(),
                "Xbox Game Bar restaurada.",
                null, // errorMessage
                false, // requiresRestart (default)
                showNotification: false
            );

            // Actualizar estado visual del toggle
            ToggleGameBar.IsChecked = false;
            TxtGameBarStatus.Text = "OFF";
            TxtGameBarStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(231, 76, 60)); // Rojo
        }

        private void BtnCoreIsolation_On_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            _tweakHelper.ExecuteTweak(
                "CoreIsolation",
                "GHOST Pack",
                () => WindowsDebloat.DisableCoreIsolation(),
                "Core Isolation (VBS) deshabilitado. FPS +10-30%, Input lag -3ms.",
                null,
                true
            );

            // Actualizar estado visual del toggle
            ToggleCoreIsolation.IsChecked = true;
            TxtCoreIsolationStatus.Text = "ON";
            TxtCoreIsolationStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(14, 122, 13)); // Verde
        }

        private void BtnCoreIsolation_Off_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            _tweakHelper.ExecuteTweakRevert(
                "CoreIsolation",
                "GHOST Pack",
                () => WindowsDebloat.EnableCoreIsolation(),
                "Core Isolation (VBS) restaurado.",
                null,
                true,
                showNotification: false
            );

            // Actualizar estado visual del toggle
            ToggleCoreIsolation.IsChecked = false;
            TxtCoreIsolationStatus.Text = "OFF";
            TxtCoreIsolationStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(231, 76, 60)); // Rojo
        }

        // ═══════════════════════════════════════════════════════════════════
        // NUEVAS FUNCIONALIDADES AVANZADAS - MPO & CPU SCHEDULING
        // ═══════════════════════════════════════════════════════════════════

        private void BtnMPOFix_On_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            try
            {
                var result = MessageBox.Show(
                    "🚫 DESHABILITAR MPO (MULTIPLANE OVERLAY)\n\n" +
                    "⚠️ IMPORTANTE - LEE ANTES DE CONTINUAR ⚠️\n\n" +
                    "QUÉ HACE ESTE TWEAK:\n" +
                    "✅ Elimina stuttering causado por MPO\n" +
                    "✅ Sin pantallazos negros al Alt+Tab\n" +
                    "✅ Frame pacing más consistente\n" +
                    "✅ Overlays (Discord, OBS) sin problemas\n" +
                    "✅ Mejor compatibilidad G-Sync/FreeSync\n\n" +
                    "🔧 MÉTODO:\n" +
                    "Establece OverlayTestMode = 5 para forzar modo legacy\n\n" +
                    "💻 REQUERE REINICIO OBLIGATORIO\n\n" +
                    "¿Continuar deshabilitando MPO?",
                    "Deshabilitar MPO",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _tweakHelper.ExecuteTweak(
                        "MPOFix",
                        "GHOST Pack",
                        () => InputTweaks.DisableMPO(),
                        "MPO deshabilitado. Frame pacing más consistente.\n\n⚠️ REINICIA Windows para aplicar cambios.",
                        null,
                        true
                    );

                    // Actualizar estado visual del toggle
                    ToggleMPOFix.IsChecked = true;
                    TxtMPOFixStatus.Text = "ON";
                    TxtMPOFixStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(14, 122, 13)); // Verde
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnMPOFix_Off_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            try
            {
                var result = MessageBox.Show(
                    "🔄 RESTAURAR MPO (MULTIPLANE OVERLAY)\n\n" +
                    "⚠️ ADVERTENCIA ⚠️\n\n" +
                    "ESTO RESTAURARÁ MPO A CONFIGURACIÓN WINDOWS:\n" +
                    "• El stuttering puede VOLVER si tenías problemas\n" +
                    "• Pantallazos negros pueden reaparecer\n" +
                    "• Problemas con overlays pueden volver\n\n" +
                    "✅ SOLO RESTAURA SI:\n" +
                    "• Experimentas problemas después de deshabilitar MPO\n" +
                    "• Tu sistema funciona mejor con MPO habilitado\n\n" +
                    "💻 REQUIERE REINICIO OBLIGATORIO\n\n" +
                    "¿Continuar restaurando MPO?",
                    "Restaurar MPO",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    _tweakHelper.ExecuteTweakRevert(
                        "MPOFix",
                        "GHOST Pack",
                        () => InputTweaks.RestoreMPO(),
                        "MPO restaurado a configuración Windows.\n\n⚠️ REINICIA Windows para aplicar cambios.",
                        null,
                        true
                    );

                    // Actualizar estado visual del toggle
                    ToggleMPOFix.IsChecked = false;
                    TxtMPOFixStatus.Text = "OFF";
                    TxtMPOFixStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(231, 76, 60)); // Rojo
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RadioBalanced_Checked(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            try
            {
                _tweakHelper.ExecuteTweak(
                    "CpuPriorityBalanced",
                    "GHOST Pack",
                    () => InputTweaks.SetBalancedCpuProfile(),
                    "Perfil CPU 'Balanced' aplicado. Balance óptimo gaming/multitasking.\n\n⚠️ REINICIA Windows para efecto completo.",
                    null,
                    true
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RadioSmooth_Checked(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            try
            {
                _tweakHelper.ExecuteTweak(
                    "CpuPrioritySmooth",
                    "GHOST Pack",
                    () => InputTweaks.SetSmoothCpuProfile(),
                    "Perfil CPU 'Smooth' aplicado. Time slices largos para streaming.\n\n⚠️ REINICIA Windows para efecto completo.",
                    null,
                    true
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RadioAggressive_Checked(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            try
            {
                var result = MessageBox.Show(
                    "⚡ PERFIL CPU AGGRESSIVE\n\n" +
                    "⚠️ ADVERTENCIA ⚠️\n\n" +
                    "ESTE PERFIL ES MUY AGRESIVO:\n" +
                    "✅ Máximo rendimiento para gaming competitivo\n" +
                    "✅ Latencia mínima para aplicación activa\n" +
                    "✅ Ideal para esports\n\n" +
                    "⚠️ PUEDE AFECTAR:\n" +
                    "• Multitasking intensivo\n" +
                    "• Aplicaciones en segundo plano\n" +
                    "• Estabilidad en sistemas lentos\n\n" +
                    "💻 REQUERE REINICIO OBLIGATORIO\n\n" +
                    "¿Aplicar perfil Aggressive?",
                    "Perfil CPU Aggressive",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    _tweakHelper.ExecuteTweak(
                        "CpuPriorityAggressive",
                        "GHOST Pack",
                        () => InputTweaks.SetAggressiveCpuProfile(),
                        "Perfil CPU 'Aggressive' aplicado. Máximo rendimiento gaming.\n\n⚠️ REINICIA Windows para efecto completo.",
                        null,
                        true
                    );
                }
                else
                {
                    // Si cancela, revertir la selección del radio button
                    ((RadioButton)sender).IsChecked = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnWin32PriorityReset_Click(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            try
            {
                var result = MessageBox.Show(
                    "🔄 RESETEAR PERFIL CPU\n\n" +
                    "Esto restaurará Win32PrioritySeparation\n" +
                    "al valor por defecto de Windows (2).\n\n" +
                    "✅ Revierte cualquier optimización\n" +
                    "✅ Comportamiento Windows estándar\n\n" +
                    "💻 REQUIERE REINICIO OBLIGATORIO\n\n" +
                    "¿Resetear a configuración por defecto?",
                    "Reset CPU Priority",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _tweakHelper.ExecuteTweakRevert(
                        "CpuPriority",
                        "GHOST Pack",
                        () => InputTweaks.ResetCpuProfile(),
                        "Perfil CPU reseteado a configuración Windows por defecto.\n\n⚠️ REINICIA Windows para efecto completo.",
                        null,
                        true
                    );

                    // Limpiar selección de radio buttons
                    RadioBalanced.IsChecked = false;
                    RadioSmooth.IsChecked = false;
                    RadioAggressive.IsChecked = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ═══════════════════════════════════════════════════════════════════
        // TWEAK INFO POPUP SYSTEM
        // ═══════════════════════════════════════════════════════════════════

        #region Tweak Info Popups

        /// <summary>
        /// Muestra popup de información detallada para un tweak específico
        /// </summary>
        private void ShowTweakInfo(object sender, RoutedEventArgs e)
        {
            if (_isInitializingTweakStates) return;
            try
            {
                if (sender is Button infoButton && infoButton.Tag is string tweakId)
                {
                    ShowTweakPopup(tweakId, infoButton);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error mostrando información del tweak: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Crea y muestra el popup de información de un tweak
        /// </summary>
        private void ShowTweakPopup(string tweakId, FrameworkElement relativeTo)
        {
            if (_isInitializingTweakStates) return;
            try
            {
                // Obtener información del tweak
                var tweakInfo = TweaksDatabase.GetTweakInfo(tweakId);

                // Crear popup
                var popup = new Popup();
                var popupControl = new TweakInfoPopup();

                // Configurar datos del popup
                popupControl.Title = tweakInfo.Title;
                popupControl.Description = tweakInfo.Description;
                popupControl.Benefits = tweakInfo.Benefits;
                popupControl.Warnings = tweakInfo.Warnings;
                popupControl.Recommended = tweakInfo.Recommended;

                // Configurar popup
                popup.Child = popupControl;
                popup.Placement = PlacementMode.Bottom;
                popup.PlacementTarget = relativeTo;
                popup.AllowsTransparency = true;
                popup.PopupAnimation = PopupAnimation.Fade;
                popup.StaysOpen = false; // Se cierra al hacer click fuera

                // Manejar cierre del popup
                popup.Closed += (s, e) =>
                {
                    popup.Child = null;
                    popup = null;
                };

                // Cerrar popup al hacer click en el botón de cerrar o fuera del popup
                popupControl.MouseDown += (s, e) => e.Handled = true; // Prevenir que se cierre al hacer click en el contenido

                // Mostrar popup
                popup.IsOpen = true;

                Debug.WriteLine($"📋 Popup mostrado para tweak: {tweakId}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error mostrando popup para {tweakId}: {ex.Message}");

                // Fallback: mostrar MessageBox simple
                var fallbackInfo = TweaksDatabase.GetTweakInfo(tweakId);
                MessageBox.Show(
                    $"{fallbackInfo.Title}\n\n" +
                    $"{fallbackInfo.Description}\n\n" +
                    $"BENEFICIOS:\n{fallbackInfo.Benefits}\n\n" +
                    $"ADVERTENCIAS:\n{fallbackInfo.Warnings}",
                    $"Información: {fallbackInfo.Title}",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// Muestra popup de información usando coordenadas específicas
        /// </summary>
        private void ShowTweakPopupAt(string tweakId, Point position)
        {
            if (_isInitializingTweakStates) return;
            try
            {
                var tweakInfo = TweaksDatabase.GetTweakInfo(tweakId);

                var popup = new Popup();
                var popupControl = new TweakInfoPopup();

                popupControl.Title = tweakInfo.Title;
                popupControl.Description = tweakInfo.Description;
                popupControl.Benefits = tweakInfo.Benefits;
                popupControl.Warnings = tweakInfo.Warnings;
                popupControl.Recommended = tweakInfo.Recommended;

                popup.Child = popupControl;
                popup.Placement = PlacementMode.AbsolutePoint;
                popup.HorizontalOffset = position.X;
                popup.VerticalOffset = position.Y;
                popup.AllowsTransparency = true;
                popup.PopupAnimation = PopupAnimation.Fade;
                popup.StaysOpen = false;

                popup.IsOpen = true;

                Debug.WriteLine($"📋 Popup mostrado en posición ({position.X}, {position.Y}) para: {tweakId}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error mostrando popup en posición para {tweakId}: {ex.Message}");
            }
        }

        /// <summary>
        /// Helper para mostrar quick info tooltip en hover
        /// </summary>
        private void ShowQuickTweakInfo(object sender, MouseEventArgs e)
        {
            if (sender is FrameworkElement element && element.Tag is string tweakId)
            {
                var tweakInfo = TweaksDatabase.GetTweakInfo(tweakId);
                string quickInfo = tweakInfo.Recommended ?
                    $"✅ RECOMENDADO: {tweakInfo.Title}" :
                    $"⚠️ AVANZADO: {tweakInfo.Title}";

                element.ToolTip = quickInfo;
            }
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════════
        // 🌐 FIX AUTOMÁTICO NAVEGADORES - DETECCIÓN Y REPARACIÓN
        // ═══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Verifica y ofrece fix para problemas de velocidad en navegadores
        /// </summary>
        private async Task CheckBrowserSpeedIssues()
        {
            if (_isInitializingTweakStates) return;
            try
            {
                // Dar tiempo para que la interfaz cargue completamente
                await Task.Delay(3000);

                Debug.WriteLine("🔍 Verificando problemas de navegadores...");

                Tuple<bool, string> browserIssues = await BrowserSpeedFix.HasBrowserIssues();

                if (browserIssues.Item1) // Si hay problemas detectados
                {
                    Debug.WriteLine("🚨 PROBLEMAS DE NAVEGADORES DETECTADOS - Mostrando prompt para fix.");

                    // Mostrar notificación en el hilo principal
                    Dispatcher.Invoke(() =>
                    {
                        var result = _notifications.ShowPrompt(
                            $"⚠️ Problemas de navegadores detectados:\n\n{browserIssues.Item2}\n\n" +
                            "¿Deseas aplicar un fix rápido para mejorar la velocidad?",
                            "Fix Rápido para Navegadores",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Warning);

                        if (result == MessageBoxResult.Yes)
                        {
                            _ = ApplyBrowserQuickFix(); // Llamar al método para aplicar el fix
                        }
                    });
                }
                else
                {
                    Debug.WriteLine("✅ Navegadores funcionando correctamente");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error en CheckBrowserSpeedIssues: {ex.Message}");
                _notifications.ShowError($"Error al verificar/aplicar fix para navegadores: {ex.Message}", "Navegadores");
            }
        }

        /// <summary>
        /// Aplica el fix rápido de navegadores y muestra los resultados.
        /// </summary>
        private async Task ApplyBrowserQuickFix()
        {
            if (_isInitializingTweakStates) return;
            try
            {
                string fixResult = await BrowserSpeedFix.ApplyQuickBrowserFix();
                if (fixResult.StartsWith("✅"))
                {
                    _notifications.ShowSuccess(fixResult, "Fix Rápido para Navegadores");
                }
                else if (fixResult.StartsWith("⚠️"))
                {
                    _notifications.ShowWarning(fixResult, "Fix Parcial");
                }
                else
                {
                    _notifications.ShowError(fixResult, "Error en Fix");
                }
            }
            catch (Exception ex)
            {
                _notifications.ShowError($"Error al aplicar fix rápido: {ex.Message}", "Navegadores");
            }
        }

        /// <summary>
        /// Muestra el diálogo de fix de navegadores
        /// </summary>
        private async Task ShowBrowserFixDialog()
        {
            if (_isInitializingTweakStates) return;
            try
            {
                var result = _notifications.ShowPrompt(
                    "🌐 FIX NAVEGADORES ULTRA RÁPIDO\n\n" +
                    "PROBLEMA DETECTADO:\n" +
                    "• Configuraciones extremas de gaming están causando\n" +
                    "  lentitud severa en navegadores web\n\n" +
                    "SOLUCIÓN AUTOMÁTICA:\n" +
                    "• Mantiene 90% del rendimiento gaming\n" +
                    "• Mejora DRÁSTICAMENTE la velocidad de navegadores\n" +
                    "• Se aplica inmediatamente sin reiniciar\n\n" +
                    "CAMBIOS QUE SE APLICARÁN:\n" +
                    "• TcpAckFrequency: 1 → 2 (menos agresivo)\n" +
                    "• NetworkThrottling: Parcialmente restaurado\n" +
                    "• SystemResponsiveness: Mejorado para multitarea\n" +
                    "• DNS Cache: Optimizado para navegación\n\n" +
                    "¿Aplicar fix automático ahora?",
                    "Fix Navegadores - Ghost Optimizer",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    Debug.WriteLine("🔧 Aplicando fix automático de navegadores...");

                    _notifications.ShowInfo(
                        "🔧 Aplicando fix de navegadores...\n\n" +
                        "Por favor espera 10-15 segundos.",
                        "Aplicando Fix"
                    );

                    string fixResult = await BrowserSpeedFix.ApplyQuickBrowserFix();

                    if (fixResult.StartsWith("✅"))
                    {
                        _notifications.ShowSuccess(fixResult, "¡Fix Completado!");
                    }
                    else if (fixResult.StartsWith("⚠️"))
                    {
                        _notifications.ShowWarning(fixResult, "Fix Parcial");
                    }
                    else
                    {
                        _notifications.ShowError(fixResult, "Error en Fix");
                    }

                    // Opcional: Abrir navegador para probar
                    var testBrowser = _notifications.ShowPrompt(
                        "¿Abrir Google para probar la velocidad?",
                        "Probar Navegador",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (testBrowser == MessageBoxResult.Yes)
                    {
                        try
                        {
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = "https://www.google.com",
                                UseShellExecute = true
                            });
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"No se pudo abrir navegador: {ex.Message}");
                        }
                    }
                }
                else
                {
                    Debug.WriteLine("ℹ️ Usuario decidió no aplicar fix de navegadores");

                    _notifications.ShowInfo(
                        "ℹ️ Fix no aplicado\n\n" +
                        "Si experimentas navegadores lentos:\n" +
                        "• Ejecuta FIX_NAVEGADORES_ULTRA_RAPIDO_v2.3.0.bat\n" +
                        "• O revierte todos los tweaks temporalmente",
                        "Fix Manual Disponible"
                    );
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error en diálogo de fix: {ex.Message}");
                _notifications.ShowError($"Error en diálogo de fix: {ex.Message}", "Error");
            }
        }

        private void FixBrowsersResult_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "¿Quieres abrir una guía rápida de diagnóstico de navegadores?",
                    "Fix de navegadores",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "https://www.google.com",
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"No se pudo abrir navegador: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en diálogo de fix: {ex.Message}");
                _notifications.ShowError($"Error en diálogo de fix: {ex.Message}", "Error");
            }
        }

        private void RestartBadge_Click(object sender, MouseButtonEventArgs e)
        {
            var vm = DataContext as ViewModels.MainWindowViewModel;
            if (vm == null || !vm.PendingRestartTweaks.Any()) return;

            var names = string.Join("\n• ", vm.PendingRestartTweaks.Select(t => t.DisplayName));
            var result = MessageBox.Show($"Los siguientes tweaks requieren reinicio:\n\n• {names}\n\n¿Reiniciar ahora?", "Reinicio requerido", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result == MessageBoxResult.Yes)
            {
                Process.Start(new ProcessStartInfo("shutdown", "/r /t 10 /c \"Ghost Optimizer: aplicando tweaks\"") { UseShellExecute = false });
            }
        }

        private void SmartScanButton_Click(object sender, RoutedEventArgs e) => ((ViewModels.MainWindowViewModel)DataContext).StartScanCommand.Execute(null);

        private void ToggleGameMode_Click(object sender, RoutedEventArgs e)
        {
            var gameBooster = GameBoosterService.Instance;
            if (gameBooster.IsMonitoring)
            {
                gameBooster.StopMonitoring();
            }
            else
            {
                gameBooster.StartMonitoring();
            }
        }

        private void ApplyAdvancedSettings_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Ajustes avanzados disponibles desde el dashboard.", "Ajustes avanzados", MessageBoxButton.OK, MessageBoxImage.Information);

        private void QuickOptimizeButton_Click(object sender, RoutedEventArgs e) => ((ViewModels.MainWindowViewModel)DataContext).OptimizeNowCommand.Execute(null);

        private void ViewStatusReportButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = (ViewModels.MainWindowViewModel)DataContext;
            MessageBox.Show($"{vm.HardwareSummary}\n\nAnticheats: {vm.DetectedAntiCheatNames}", "Estado del sistema", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
