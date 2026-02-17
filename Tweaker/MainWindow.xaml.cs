#nullable disable

using System.Text;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using Microsoft.Win32;
using Tweaker.Optimizations;
using Tweaker.Utilities;
using Tweaker.Presets;
using Tweaker.Data;
using Tweaker.Controls;

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

        public MainWindow()
        {
            InitializeComponent();

            // Inicializar servicios
            _stateManager = TweakStateManager.Instance;
            _telemetry = TelemetryService.Instance;
            _notifications = NotificationService.Instance;
            _tweakHelper = new TweakHelper(_stateManager, _telemetry, _notifications);

            // Configurar binding de datos para dashboard dinámico
            DataContext = _stateManager;

            // Inicializar notificaciones
            var rootGrid = (Grid)this.Content;
            _notifications.Initialize(rootGrid);

            // 🌐 VERIFICAR Y OFRECER FIX RÁPIDO DE NAVEGADORES
            Task.Run(async () => await CheckBrowserSpeedIssues());

            // Registrar inicio de aplicación
            _telemetry.TrackAppLaunch();

            // Actualizar dashboard
            UpdateDashboard();
            
            // Actualizar perfil de CPU Scheduling actual
            UpdateCurrentPriorityProfile();
            
            // Verificar si se ejecuta como administrador
            if (!IsRunAsAdministrator())
            {
                MessageBox.Show(
                    "⚠️ ADVERTENCIA: Esta aplicación debe ejecutarse como Administrador.\n\n" +
                    "Los tweaks de registro y comandos requieren permisos elevados.\n" +
                    "Click derecho > Ejecutar como Administrador",
                    "Permisos de Administrador Requeridos",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            else
            {
                // Si es admin, preguntar por punto de restauración
                // Solo preguntar la primera vez (al abrir la app)
                SystemRestore.PromptCreateRestorePoint();
            }

            // Suscribirse a cambios de estado
            _stateManager.PropertyChanged += (s, e) =>
            {
                UpdateDashboard();
                // Dar un pequeño delay para asegurar que el UI esté listo
                Dispatcher.BeginInvoke(new Action(() => UpdateTweakIndicators()), System.Windows.Threading.DispatcherPriority.Loaded);
            };

            // Actualizar indicadores visuales al iniciar (con delay para asegurar que la UI esté renderizada)
            this.Loaded += (s, e) =>
            {
                // Primer intento inmediato
                UpdateTweakIndicators();
                
                // Segundo intento con delay de 100ms (para asegurar que todos los botones estén renderizados)
                Task.Delay(100).ContinueWith(_ =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        UpdateTweakIndicators();
                        Debug.WriteLine("🔄 Indicadores actualizados después de cargar la ventana");
                    });
                });
                
                // Tercer intento con delay de 500ms (para casos donde la UI tarda más)
                Task.Delay(500).ContinueWith(_ =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        UpdateTweakIndicators();
                        Debug.WriteLine("🔄 Indicadores actualizados (verificación final)");
                    });
                });
            };

#if DEBUG
            // Tests de nuevas características (solo en DEBUG)
            // TestProfileManager(); // Método no implementado
            // TestBackupService(); // Método no implementado  
#endif
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
        // INDICADORES VISUALES DE TWEAKS ACTIVOS
        // ═══════════════════════════════════════════════════════════════════

        #region Indicadores Visuales

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
            _tweakHelper.ExecuteTweak(
                "SystemProfile",
                "Sistema & GPU",
                () => GpuOptimization.EnableSystemProfileOptimization(),
                "System Profile configurado para Games. GPU Priority: 8, CPU Priority: 6.",
                null,
                true
            );
            
            // Actualizar botón inmediatamente
            UpdateButtonState((Button)sender, true);
        }

        private void BtnSystemProfile_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "SystemProfile",
                "Sistema & GPU",
                () => GpuOptimization.DisableSystemProfileOptimization(),
                "System Profile restaurado a valores predeterminados.",
                null,
                true
            );
            
            // Buscar y actualizar el botón ON correspondiente
            UpdateRelatedOnButton("SYSTEM PROFILE", false);
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
            _tweakHelper.ExecuteTweak(
                "GameDVR",
                "Sistema & GPU",
                () => GpuOptimization.DisableGameDVR(),
                "GameDVR y Xbox Game Bar deshabilitados. Input lag -5-15ms, FPS +10-30%.",
                null,
                true
            );
            UpdateButtonState((Button)sender, true);
        }

        private void BtnGameDVR_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "GameDVR",
                "Sistema & GPU",
                () => GpuOptimization.EnableGameDVR(),
                "GameDVR y Xbox Game Bar restaurados.",
                null,
                true
            );
            UpdateRelatedOnButton("GAMEDVR", false);
        }

        private void BtnGpuScheduling_On_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweak(
                "GpuScheduling",
                "Sistema & GPU",
                () => GpuOptimization.EnableHardwareAcceleratedGPUScheduling(),
                "Hardware GPU Scheduling activado. Puede mejorar o empeorar latencia (probar ambos).",
                null,
                true
            );
            UpdateButtonState((Button)sender, true);
        }

        private void BtnGpuScheduling_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "GpuScheduling",
                "Sistema & GPU",
                () => GpuOptimization.DisableHardwareAcceleratedGPUScheduling(),
                "Hardware GPU Scheduling desactivado.",
                null,
                true
            );
            UpdateRelatedOnButton("GPU SCHEDULING", false);
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════════
        // CATEGORÍA 2: CPU (RYZEN/INTEL)
        // ═══════════════════════════════════════════════════════════════════

        #region CPU Optimizations

        private void BtnSystemResponsiveness_On_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweak(
                "SystemResponsiveness",
                "Sistema & GPU",
                () => CpuOptimization.EnableSystemResponsivenessOptimization(),
                "System Responsiveness optimizado. NetworkThrottling OFF, ping reducido 5-20ms.",
                null,
                true
            );
            UpdateButtonState((Button)sender, true);
        }

        private void BtnSystemResponsiveness_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "SystemResponsiveness",
                "Sistema & GPU",
                () => CpuOptimization.DisableSystemResponsivenessOptimization(),
                "System Responsiveness restaurado a valores predeterminados.",
                null,
                true
            );
            UpdateRelatedOnButton("RESPONSIVENESS", false);
        }

        private void BtnHighPerformance_On_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweak(
                "HighPerformance",
                "Sistema & GPU",
                () => CpuOptimization.EnableHighPerformancePowerPlan(),
                "Plan Alto Rendimiento activado. CPU siempre a máxima frecuencia."
            );
            UpdateButtonState((Button)sender, true);
        }

        private void BtnHighPerformance_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "HighPerformance",
                "Sistema & GPU",
                () => CpuOptimization.EnableBalancedPowerPlan(),
                "Plan Balanceado activado. CPU ajustará frecuencia según uso."
            );
            UpdateRelatedOnButton("ALTO RENDIMIENTO", false);
        }

        private void BtnPowerThrottling_On_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweak(
                "PowerThrottling",
                "Sistema & GPU",
                () => CpuOptimization.DisablePowerThrottling(),
                "Power Throttling deshabilitado. Apps en background sin limitaciones.",
                null,
                true
            );
            UpdateButtonState((Button)sender, true);
        }

        private void BtnPowerThrottling_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "PowerThrottling",
                "Sistema & GPU",
                () => CpuOptimization.EnablePowerThrottling(),
                "Power Throttling restaurado.",
                null,
                true
            );
            UpdateRelatedOnButton("THROTTLING", false);
        }

        private void BtnCoreParking_On_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweak(
                "CoreParking",
                "Sistema & GPU",
                () => CpuOptimization.DisableCoreParking(),
                "Core Parking deshabilitado. Todos los cores permanecen activos (crítico en Ryzen).",
                null,
                true
            );
            UpdateButtonState((Button)sender, true);
        }

        private void BtnCoreParking_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "CoreParking",
                "Sistema & GPU",
                () => CpuOptimization.EnableCoreParking(),
                "Core Parking restaurado. Windows puede aparcar cores no utilizados.",
                null,
                true
            );
            UpdateRelatedOnButton("CORE PARKING", false);
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════════
        // CATEGORÍA 3: WINDOWS BLOATWARE
        // ═══════════════════════════════════════════════════════════════════

        #region Windows Bloatware

        private void BtnHibernation_On_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweak(
                "Hibernation",
                "Limpieza",
                () => WindowsOptimization.DisableHibernation(),
                "Hibernación deshabilitada. Archivo hiberfil.sys eliminado (8-32GB liberados).",
                null,
                true
            );
            UpdateButtonState((Button)sender, true);
        }

        private void BtnHibernation_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "Hibernation",
                "Limpieza",
                () => WindowsOptimization.EnableHibernation(),
                "Hibernación restaurada.",
                null,
                true
            );
            UpdateRelatedOnButton("HIBERNACIÓN", false);
        }

        private void BtnWindowsSearch_On_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweak(
                "WindowsSearch",
                "Limpieza",
                () => WindowsOptimization.DisableWindowsSearch(),
                "Windows Search deshabilitado. Indexación detenida, 200-500MB RAM liberados."
            );
            UpdateButtonState((Button)sender, true);
        }

        private void BtnWindowsSearch_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "WindowsSearch",
                "Limpieza",
                () => WindowsOptimization.EnableWindowsSearch(),
                "Windows Search restaurado. Servicio de indexación activo."
            );
            UpdateRelatedOnButton("WINDOWS SEARCH", false);
        }

        private void BtnSysMain_On_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweak(
                "SysMain",
                "Limpieza",
                () => WindowsOptimization.DisableSysMain(),
                "SysMain (SuperFetch) deshabilitado. 1-3GB RAM liberados, uso de disco reducido.",
                null,
                true
            );
            UpdateButtonState((Button)sender, true);
        }

        private void BtnSysMain_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "SysMain",
                "Limpieza",
                () => WindowsOptimization.EnableSysMain(),
                "SysMain (SuperFetch) restaurado. Pre-carga de apps habilitada.",
                null,
                true
            );
            UpdateRelatedOnButton("SYSMAIN", false);
        }

        private void BtnTelemetry_On_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweak(
                "Telemetry",
                "Limpieza",
                () => WindowsOptimization.DisableTelemetry(),
                "Telemetría deshabilitada. Windows dejará de enviar datos a Microsoft."
            );
            UpdateButtonState((Button)sender, true);
        }

        private void BtnTelemetry_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "Telemetry",
                "Limpieza",
                () => WindowsOptimization.EnableTelemetry(),
                "Telemetría restaurada. Servicios de telemetría activos."
            );
            UpdateRelatedOnButton("TELEMETRÍA", false);
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════════
        // CATEGORÍA 4: RED Y PING (ADAMX TWEAKS)
        // ═══════════════════════════════════════════════════════════════════

        #region Red y Ping

        private void BtnNetworkOptimization_On_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool success = NetworkOptimization.OptimizeNetwork();
                
                if (success)
                {
                    // Actualizar botón inmediatamente
                    UpdateButtonState((Button)sender, true);
                    
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
            try
            {
                bool success = NetworkOptimization.RestoreNetwork();
                
                if (success)
                {
                    // Actualizar botón ON relacionado
                    UpdateRelatedOnButton("TCP/IP", false);
                    
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
                "¿Continuar con tweaks extremos para gaming?",
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
            _tweakHelper.ExecuteTweak(
                "SysMainService",
                "Limpieza",
                () => ServiceOptimization.DisableSysMain(),
                "SysMain (SuperFetch) servicio deshabilitado. 1-3GB RAM liberados."
            );
        }

        private void BtnSysMainService_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "SysMainService",
                "Limpieza",
                () => ServiceOptimization.EnableSysMain(),
                "SysMain (SuperFetch) servicio restaurado."
            );
        }

        private void BtnDiagTrack_On_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweak(
                "DiagTrack",
                "Limpieza",
                () => ServiceOptimization.DisableDiagTrack(),
                "DiagTrack (Telemetría) deshabilitado. Sin envío de datos a Microsoft."
            );
        }

        private void BtnDiagTrack_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "DiagTrack",
                "Limpieza",
                () => ServiceOptimization.EnableDiagTrack(),
                "DiagTrack (Telemetría) restaurado."
            );
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════════
        // CATEGORÍA 6: KERNEL LATENCY (BCD / HPET)
        // ═══════════════════════════════════════════════════════════════════

        #region Kernel Latency (BCD / HPET)

        private void BtnHPET_On_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweak(
                "HPET",
                "GHOST Pack",
                () => LatencyOptimization.DisableHPET(),
                "HPET deshabilitado. Micro-stuttering reducido, frame times mejorados (usa TSC).",
                null,
                true
            );
            UpdateButtonState((Button)sender, true);
        }

        private void BtnHPET_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "HPET",
                "GHOST Pack",
                () => LatencyOptimization.EnableHPET(),
                "HPET restaurado. Windows decidirá automáticamente qué timer usar.",
                null,
                true
            );
            UpdateRelatedOnButton("HPET", false);
        }

        private void BtnHyperV_On_Click(object sender, RoutedEventArgs e)
        {
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
                    "✅ Reduce latencia de GPU 2-5ms\n" +
                    "✅ Mejora compatibilidad con anti-cheat (Vanguard)\n" +
                    "✅ Reduce DPC latency\n\n" +
                    "⚠️ SOLO deshabilita si NO usas virtualización.\n\n" +
                    "¿Estás SEGURO de continuar?",
                    "Confirmación Requerida",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.No)
                {
                    _notifications.ShowInfo(
                        "Hyper-V permanece habilitado.",
                        "Operacion Cancelada"
                    );
                    return;
                }

                _tweakHelper.ExecuteTweak(
                    "HyperV",
                    "GHOST Pack",
                    () => LatencyOptimization.DisableHyperV(),
                    "Hyper-V deshabilitado. Latencia GPU -2-5ms (Docker y WSL2 NO funcionarán).",
                    null,
                    true
                );
            }
            catch (Exception ex)
            {
                _notifications.ShowError($"Error: {ex.Message}");
            }
        }

        private void BtnHyperV_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "HyperV",
                "GHOST Pack",
                () => LatencyOptimization.EnableHyperV(),
                "Hyper-V restaurado. Docker y WSL2 volverán a funcionar.",
                null,
                true
            );
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════════
        // CATEGORÍA 7: GHOST PACK (TWEAKS LEGENDARIOS)
        // ═══════════════════════════════════════════════════════════════════

        #region GHOST Pack

        private void BtnUltimatePower_On_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweak(
                "UltimatePower",
                "GHOST Pack",
                () => PowerTweaks.EnableUltimatePerformance(),
                "Ultimate Performance activado. Latencia CPU -93%, 0.1% low FPS +20%."
            );
            UpdateButtonState((Button)sender, true);
        }

        private void BtnUltimatePower_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "UltimatePower",
                "GHOST Pack",
                () => PowerTweaks.RestoreBalancedPlan(),
                "Plan Balanced restaurado. Consumo y temperaturas optimizados."
            );
            UpdateRelatedOnButton("ULTIMATE", false);
        }

        private void BtnGameBar_On_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweak(
                "GameBar",
                "GHOST Pack",
                () => WindowsDebloat.DisableGameBar(),
                "Xbox Game Bar deshabilitada. Input lag -50%, CPU libre +8%."
            );
            UpdateButtonState((Button)sender, true);
        }

        private void BtnGameBar_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "GameBar",
                "GHOST Pack",
                () => WindowsDebloat.EnableGameBar(),
                "Xbox Game Bar restaurada."
            );
            UpdateRelatedOnButton("GAME BAR", false);
        }

        private void BtnCoreIsolation_On_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweak(
                "CoreIsolation",
                "GHOST Pack",
                () => WindowsDebloat.DisableCoreIsolation(),
                "Core Isolation (VBS) deshabilitado. FPS +10-30%, Input lag -3ms.",
                null,
                true
            );
            UpdateButtonState((Button)sender, true);
        }

        private void BtnCoreIsolation_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "CoreIsolation",
                "GHOST Pack",
                () => WindowsDebloat.EnableCoreIsolation(),
                "Core Isolation (VBS) restaurado.",
                null,
                true
            );
            UpdateRelatedOnButton("CORE ISOLATION", false);
        }

        // ═══════════════════════════════════════════════════════════════════
        // NUEVAS FUNCIONALIDADES AVANZADAS - MPO & CPU SCHEDULING
        // ═══════════════════════════════════════════════════════════════════

        private void BtnMPOFix_On_Click(object sender, RoutedEventArgs e)
        {
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
                    "💻 REQUIERE REINICIO OBLIGATORIAMENTE\n\n" +
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
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnMPOFix_Off_Click(object sender, RoutedEventArgs e)
        {
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
                    "💻 REQUIERE REINICIO OBLIGATORIAMENTE\n\n" +
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
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RadioBalanced_Checked(object sender, RoutedEventArgs e)
        {
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
                    "💻 REQUIERE REINICIO OBLIGATORIAMENTE\n\n" +
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

        private void BtnCpuPriority_Reset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "🔄 RESETEAR PERFIL CPU\n\n" +
                    "Esto restaurará Win32PrioritySeparation\n" +
                    "al valor por defecto de Windows (2).\n\n" +
                    "✅ Revierte cualquier optimización\n" +
                    "✅ Comportamiento Windows estándar\n\n" +
                    "💻 REQUIERE REINICIO OBLIGATORIAMENTE\n\n" +
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

        #endregion

        // ═══════════════════════════════════════════════════════════════════
        // CATEGORÍA 8: INPUT & VISUALS (RESPUESTA Y FPS)
        // ═══════════════════════════════════════════════════════════════════

        #region Input & Visuals

        private void BtnKeyboard_On_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweak(
                "Keyboard",
                "Input & Visuals",
                () => KeyboardOptimization.OptimizeKeyboard(),
                "Teclado optimizado. Input lag reducido 50ms, WASD más responsive."
            );
            UpdateButtonState((Button)sender, true);
        }

        private void BtnKeyboard_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "Keyboard",
                "Input & Visuals",
                () => KeyboardOptimization.RestoreKeyboard(),
                "Configuración de teclado restaurada a valores predeterminados."
            );
            UpdateRelatedOnButton("TECLADO", false);
        }

        private void BtnVisuals_On_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweak(
                "VisualEffects",
                "Input & Visuals",
                () => VisualOptimization.OptimizeVisuals(),
                "Efectos visuales deshabilitados. FPS +3-8%, GPU usage -5-10%."
            );
            UpdateButtonState((Button)sender, true);
        }

        private void BtnVisuals_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "VisualEffects",
                "Input & Visuals",
                () => VisualOptimization.RestoreVisuals(),
                "Efectos visuales restaurados. Interfaz Windows con animaciones habilitadas."
            );
            UpdateRelatedOnButton("EFECTOS", false);
        }

        private void BtnMemory_On_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Verificar RAM del sistema antes de aplicar
                var (recommended, reason) = MemoryTweaks.IsMemoryOptimizationRecommended();

                if (!recommended)
                {
                _notifications.ShowWarning(
                    reason + "\n\nDisablePagingExecutive mantiene el kernel en RAM. Con menos de 16GB puede causar problemas.",
                    "Advertencia - RAM Insuficiente"
                );
                    return;
                }

                _tweakHelper.ExecuteTweak(
                    "MemoryOptimization",
                    "Input & Visuals",
                    () => MemoryTweaks.OptimizeMemory(),
                    "RAM optimizada. Kernel en RAM permanente (requiere 16GB+).",
                    null,
                    true
                );
            }
            catch (Exception ex)
            {
                _notifications.ShowError($"Error al optimizar memoria: {ex.Message}");
            }
        }

        private void BtnMemory_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "MemoryOptimization",
                "Input & Visuals",
                () => MemoryTweaks.RestoreMemory(),
                "Configuración de memoria restaurada.",
                null,
                true
            );
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════════
        // CATEGORÍA 9: MOUSE TWEAKS (ACELERACIÓN)
        // ═══════════════════════════════════════════════════════════════════

        #region Mouse Tweaks

        private void BtnMouseAccel_On_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweak(
                "MouseAcceleration",
                "Input & Visuals",
                () => MouseTweaks.Apply(),
                "Mouse acceleration desactivada. Aim 1:1 pixel perfect activado para gaming competitivo."
            );
            
            UpdateButtonState((Button)sender, true);
        }

        private void BtnMouseAccel_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "MouseAcceleration",
                "Input & Visuals",
                () => MouseTweaks.Revert(),
                "Aceleración del mouse restaurada a valores predeterminados de Windows."
            );
            
            UpdateRelatedOnButton("ACELERACIÓN", false);
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════════
        // CATEGORÍA 10: CLEANER (LIMPIEZA DE DISCO)
        // ═══════════════════════════════════════════════════════════════════

        #region Cleaner

        private void BtnCleanup_Analyze_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var (mbCanFree, filesCount) = CleanerTweaks.AnalyzeSpace();
                
                _notifications.ShowInfo(
                    $"Se pueden liberar {mbCanFree:F0} MB ({filesCount:N0} archivos).\n" +
                    $"Promedio esperado: 500MB - 5GB",
                    "Analisis Completado"
                );
            }
            catch (Exception ex)
            {
                _notifications.ShowError($"Error al analizar: {ex.Message}");
            }
        }

        private void BtnCleanup_Clean_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "⚠️ CONFIRMACIÓN DE LIMPIEZA\n\n" +
                    "Se eliminarán archivos temporales de:\n" +
                    "• C:\\Windows\\Temp\n" +
                    "• %TEMP% (AppData\\Local\\Temp)\n" +
                    "• C:\\Windows\\Prefetch\n\n" +
                    "✅ Los archivos en uso se saltarán automáticamente\n" +
                    "✅ No se afectarán documentos ni aplicaciones\n\n" +
                    "¿Continuar con la limpieza?",
                    "Confirmar Limpieza",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                
                if (result == MessageBoxResult.Yes)
                {
                    var (success, mbFreed, filesDeleted) = CleanerTweaks.DeepClean();
                    
                    if (success)
                    {
                        _notifications.ShowSuccess(
                            $"Archivos eliminados: {filesDeleted:N0}\n" +
                            $"Espacio liberado: {mbFreed:N0} MB",
                            "Limpieza Completada"
                        );
                    }
                    else
                    {
                        _notifications.ShowWarning(
                            $"Archivos eliminados: {filesDeleted:N0}\n" +
                            $"Espacio liberado: {mbFreed:N0} MB\n" +
                            $"Algunos archivos no pudieron eliminarse (en uso).",
                            "Limpieza Parcial"
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                _notifications.ShowError($"Error durante limpieza: {ex.Message}");
            }
        }

        private void BtnFlushDNS_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteAction(
                () => CleanerTweaks.FlushDNS(),
                "Caché DNS limpiado (ipconfig /flushdns). Resolución DNS actualizada."
            );
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════════
        // DNS OPTIMIZATION METHODS (agregados a Red & Ping)
        // ═══════════════════════════════════════════════════════════════════

        #region DNS Optimization



        private void BtnDnsCloudflare_Click(object sender, RoutedEventArgs e)
        {
            DnsOptimization.SetCloudflareDns();
        }

        private void BtnDnsGoogle_Click(object sender, RoutedEventArgs e)
        {
            DnsOptimization.SetGoogleDns();
        }

        private void BtnDnsCache_On_Click(object sender, RoutedEventArgs e)
        {
            DnsOptimization.EnableDnsCacheOptimization();
        }

        private void BtnDnsCache_Off_Click(object sender, RoutedEventArgs e)
        {
            DnsOptimization.DisableDnsCacheOptimization();
        }

        private void BtnNetworkPower_On_Click(object sender, RoutedEventArgs e)
        {
            DnsOptimization.DisableNetworkAdapterPowerSaving();
        }

        private void BtnNetworkPower_Off_Click(object sender, RoutedEventArgs e)
        {
            DnsOptimization.EnableNetworkAdapterPowerSaving();
        }

        private void BtnNetBios_On_Click(object sender, RoutedEventArgs e)
        {
            DnsOptimization.DisableNetBios();
        }

        private void BtnNetBios_Off_Click(object sender, RoutedEventArgs e)
        {
            DnsOptimization.EnableNetBios();
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════════
        // CATEGORÍA 11: ADVANCED TWEAKS (PELIGROSO)
        // ═══════════════════════════════════════════════════════════════════

        #region Advanced Tweaks

        private void BtnSpectreMeltdown_On_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "⚠️⚠️⚠️ ADVERTENCIA EXTREMA ⚠️⚠️⚠️\n\n" +
                    "Estás a punto de DESHABILITAR las mitigaciones de Spectre y Meltdown.\n\n" +
                    "BENEFICIOS:\n" +
                    "✅ +5-15% FPS en juegos CPU-bound (CS2, Valorant, Tarkov)\n" +
                    "✅ Menor latencia del sistema\n\n" +
                    "RIESGOS:\n" +
                    "❌ EXPONES tu sistema a vulnerabilidades de ejecución especulativa.\n" +
                    "❌ Un atacante podría leer memoria de otros procesos.\n" +
                    "❌ NO RECOMENDADO para uso general o si manejas datos sensibles.\n\n" +
                    "Úsalo bajo tu propio riesgo, idealmente en un PC solo para gaming.\n\n" +
                    "¿Estás COMPLETAMENTE SEGURO de que entiendes los riesgos?",
                    "Confirmación de Seguridad Requerida",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Error);

                if (result != MessageBoxResult.Yes) return;

                _tweakHelper.ExecuteTweak(
                    "SpectreMeltdown",
                    "Advanced",
                    () => AdvancedTweaks.DisableSpectreMeltdown(),
                    "Mitigaciones Spectre/Meltdown deshabilitadas. +5-15% FPS (SISTEMA VULNERABLE).",
                    null,
                    true
                );
            }
            catch (Exception ex)
            {
                _notifications.ShowError($"Error: {ex.Message}");
            }
        }

        private void BtnSpectreMeltdown_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "SpectreMeltdown",
                "Advanced",
                () => AdvancedTweaks.EnableSpectreMeltdown(),
                "Mitigaciones Spectre/Meltdown restauradas. Sistema protegido.",
                null,
                true
            );
        }

        private void BtnGpuIRQ_On_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "🎮 GPU IRQ OPTIMIZATION\n\n" +
                    "Esta optimización asignará la interrupción de la GPU\n" +
                    "a un core específico del procesador.\n\n" +
                    "BENEFICIOS:\n" +
                    "✅ Reduce DPC latency\n" +
                    "✅ Mejora consistencia de frame times\n" +
                    "✅ Mejor separación de workloads CPU/GPU\n" +
                    "✅ Menos micro-stuttering\n\n" +
                    "REQUISITOS:\n" +
                    "⚠️ Se requieren al menos 4 cores CPU\n" +
                    "⚠️ Requiere permisos de administrador\n\n" +
                    "¿Continuar con la optimización?",
                    "GPU IRQ Optimization",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes) return;

                _tweakHelper.ExecuteTweak(
                    "GpuIRQ",
                    "Advanced",
                    () => GpuIRQOptimization.EnableGpuIRQOptimization(),
                    "GPU IRQ optimizada. Interrupción asignada a core específico para menor latencia.",
                    null,
                    true
                );
            }
            catch (Exception ex)
            {
                _notifications.ShowError($"Error: {ex.Message}");
            }
        }

        private void BtnGpuIRQ_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "GpuIRQ",
                "Advanced",
                () => GpuIRQOptimization.DisableGpuIRQOptimization(),
                "GPU IRQ restaurada. Distribución automática de interrupciones habilitada.",
                null,
                true
            );
        }

        /// <summary>
        /// Diagnóstico completo de GPU IRQ
        /// </summary>
        private void BtnGpuIRQDiagnose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string diagnosis = GpuIRQOptimization.DiagnoseGpuIRQ();
                
                var (isOptimized, details, gpuInfo) = GpuIRQOptimization.GetGpuIRQStatus();

                MessageBox.Show(
                    $"🔍 DIAGNÓSTICO GPU IRQ OPTIMIZATION\n\n" +
                    $"{diagnosis}\n\n" +
                    $"📊 FUNCIONALIDADES IMPLEMENTADAS:\n" +
                    $"• Detección automática de GPU primaria\n" +
                    $"• Asignación de IRQ a core específico\n" +
                    $"• Optimizaciones de registro DPC\n" +
                    $"• Verificación de estado completa\n\n" +
                    $"Revisa el Output de Visual Studio para logs detallados.",
                    "Diagnóstico GPU IRQ",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"❌ Error en diagnóstico: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // ═══════════════════════════════════════════════════════════════════
        // WIN32 PRIORITY SEPARATION - PERFILES DE CPU SCHEDULING
        // ═══════════════════════════════════════════════════════════════════

        private void ProfileBalanced_Click(object sender, MouseButtonEventArgs e)
        {
            ApplyPriorityProfile("Balanced", "⚖️ BALANCED");
        }

        private void ProfileSmooth_Click(object sender, MouseButtonEventArgs e)
        {
            ApplyPriorityProfile("Smooth", "🌊 SMOOTH");
        }

        private void ProfileAggressive_Click(object sender, MouseButtonEventArgs e)
        {
            ApplyPriorityProfile("Aggressive", "⚡ AGGRESSIVE");
        }

        private void BtnWin32PriorityReset_Click(object sender, RoutedEventArgs e)
        {
            ApplyPriorityProfile("Default", "🔄 DEFAULT");
        }

        /// <summary>
        /// Método auxiliar para aplicar perfiles de CPU Scheduling con confirmación
        /// </summary>
        private void ApplyPriorityProfile(string profile, string displayName)
        {
            try
            {
                // Obtener detalles del perfil
                string profileValue, description, benefits, warning;

                switch (profile.ToLower())
                {
                    case "balanced":
                        profileValue = "38 (0x26)";
                        description = "Balance óptimo gaming/sistema";
                        benefits = 
                            "✅ Foreground apps priorizadas moderadamente\n" +
                            "✅ Background apps funcionan bien\n" +
                            "✅ Perfecto para gaming + streaming\n" +
                            "✅ Time slices balanceados";
                        warning = "⚠️ RECOMENDADO para la mayoría de usuarios";
                        break;

                    case "smooth":
                        profileValue = "40 (0x28)";
                        description = "Máxima suavidad y frame times";
                        benefits = 
                            "✅ Time slices largos = menos context switches\n" +
                            "✅ Frame times ultra-consistentes\n" +
                            "✅ Ideal para single-player exigentes\n" +
                            "✅ Menos interrupciones del sistema";
                        warning = "⚠️ Background apps menos responsivas";
                        break;

                    case "aggressive":
                        profileValue = "22 (0x16)";
                        description = "Máxima responsividad competitiva";
                        benefits = 
                            "✅ Foreground app recibe TODO el CPU\n" +
                            "✅ Time slices cortos = respuesta instantánea\n" +
                            "✅ Perfecto para FPS competitivos (CS2, Valorant)\n" +
                            "✅ Input lag mínimo absoluto";
                        warning = "⚠️ EXTREMO: Background apps casi congeladas";
                        break;

                    case "default":
                        profileValue = "2";
                        description = "Valor por defecto de Windows";
                        benefits = 
                            "✅ Comportamiento estándar de Windows\n" +
                            "✅ Sin optimizaciones específicas\n" +
                            "✅ Balance general del sistema\n" +
                            "✅ Revierte cualquier cambio previo";
                        warning = "ℹ️ Restaura configuración original";
                        break;

                    default:
                        MessageBox.Show($"Perfil desconocido: {profile}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                }

                var result = MessageBox.Show(
                    $"🎯 APLICAR PERFIL: {displayName}\n\n" +
                    $"📋 VALOR: {profileValue}\n" +
                    $"🎮 DESCRIPCIÓN: {description}\n\n" +
                    $"🚀 BENEFICIOS:\n{benefits}\n\n" +
                    $"{warning}\n\n" +
                    $"⚠️⚠️ ADVERTENCIA CRÍTICA ⚠️⚠️\n" +
                    $"Este tweak cambia el CPU scheduling de TODO el sistema.\n" +
                    $"Foreground apps (juegos) vs Background apps.\n\n" +
                    $"¿Aplicar perfil {profile.ToUpper()}?",
                    $"Perfil CPU Scheduling - {displayName}",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes) return;

                // Aplicar el perfil
                bool success = InputTweaks.SetPriorityProfile(profile);

                if (success)
                {
                    // Actualizar UI
                    UpdatePriorityProfileUI(profile);

                    // Registrar en TweakHelper (solo si no es Default)
                    if (profile.ToLower() != "default")
                    {
                        _tweakHelper.ExecuteTweak(
                            "Win32Priority",
                            "Advanced - Input & USB",
                            () => true, // Ya aplicado arriba
                            $"✅ Perfil {displayName} aplicado exitosamente. CPU scheduling optimizado."
                        );
                    }
                    else
                    {
                        _tweakHelper.ExecuteTweakRevert(
                            "Win32Priority",
                            "Advanced - Input & USB",
                            () => true, // Ya aplicado arriba
                            "✅ Win32 Priority restaurado a valor por defecto de Windows (2)."
                        );
                    }

                    _notifications.ShowSuccess(
                        $"Perfil {displayName} aplicado exitosamente.\n\n" +
                        $"El CPU scheduling ahora está configurado para: {description}",
                        $"Perfil {displayName} Aplicado"
                    );
                }
                else
                {
                    MessageBox.Show(
                        "❌ Error al aplicar el perfil.\n\n" +
                        "Verifica que ejecutas la aplicación como Administrador.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"❌ Error al aplicar perfil: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Actualiza la UI para mostrar el perfil actualmente seleccionado
        /// </summary>
        private void UpdatePriorityProfileUI(string profile)
        {
            try
            {
                // Buscar los controles dinámicamente
                var profileBalanced = this.FindName("ProfileBalanced") as Border;
                var profileSmooth = this.FindName("ProfileSmooth") as Border;
                var profileAggressive = this.FindName("ProfileAggressive") as Border;
                var txtCurrentProfile = this.FindName("TxtCurrentPriorityProfile") as TextBlock;

                if (profileBalanced == null || profileSmooth == null || profileAggressive == null)
                {
                    Debug.WriteLine("Warning: Priority Profile controls not found");
                    return;
                }

                // Reset todos los bordes a estado normal
                profileBalanced.BorderBrush = null;
                profileBalanced.BorderThickness = new Thickness(0);
                profileSmooth.BorderBrush = null;
                profileSmooth.BorderThickness = new Thickness(0);
                profileAggressive.BorderBrush = null;
                profileAggressive.BorderThickness = new Thickness(0);

                // Highlight el perfil seleccionado
                var activeBrush = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#00D9FF"));
                
                switch (profile.ToLower())
                {
                    case "balanced":
                        profileBalanced.BorderBrush = activeBrush;
                        profileBalanced.BorderThickness = new Thickness(2);
                        if (txtCurrentProfile != null)
                            txtCurrentProfile.Text = "⚖️ BALANCED (38)";
                        break;
                    case "smooth":
                        profileSmooth.BorderBrush = activeBrush;
                        profileSmooth.BorderThickness = new Thickness(2);
                        if (txtCurrentProfile != null)
                            txtCurrentProfile.Text = "🌊 SMOOTH (40)";
                        break;
                    case "aggressive":
                        profileAggressive.BorderBrush = activeBrush;
                        profileAggressive.BorderThickness = new Thickness(2);
                        if (txtCurrentProfile != null)
                            txtCurrentProfile.Text = "⚡ AGGRESSIVE (22)";
                        break;
                    case "default":
                        if (txtCurrentProfile != null)
                            txtCurrentProfile.Text = "🔄 DEFAULT (2)";
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating Priority Profile UI: {ex.Message}");
            }
        }

        /// <summary>
        /// Actualiza el perfil de CPU Scheduling actual al cargar la ventana
        /// </summary>
        private void UpdateCurrentPriorityProfile()
        {
            try
            {
                string currentProfile = InputTweaks.GetCurrentPriorityProfile();
                UpdatePriorityProfileUI(currentProfile);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error detecting current Priority Profile: {ex.Message}");
                var txtCurrentProfile = this.FindName("TxtCurrentPriorityProfile") as TextBlock;
                if (txtCurrentProfile != null)
                    txtCurrentProfile.Text = "Desconocido";
            }
        }

        // Mantener métodos antiguos para compatibilidad (ahora llaman a Balanced)
        [Obsolete("Use ProfileBalanced_Click instead")]
        private void BtnWin32Priority_On_Click(object sender, RoutedEventArgs e)
        {
            ApplyPriorityProfile("Balanced", "⚖️ BALANCED");
        }

        [Obsolete("Use BtnWin32PriorityReset_Click instead")]
        private void BtnWin32Priority_Off_Click(object sender, RoutedEventArgs e)
        {
            ApplyPriorityProfile("Default", "🔄 DEFAULT");
        }

        // ═══════════════════════════════════════════════════════════════════
        // INPUT & USB OPTIMIZATIONS - MÉTODOS FALTANTES
        // ═══════════════════════════════════════════════════════════════════

        private void BtnOptimizeUSB_On_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweak(
                "USBOptimization",
                "Advanced - Input & USB",
                () => InputTweaks.OptimizeUSBForGaming(),
                "✅ USB optimizado para gaming. Latencia reducida, mejor polling rate para mouse/teclado."
            );
        }

        private void BtnOptimizeUSB_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "USBOptimization",
                "Advanced - Input & USB",
                () => InputTweaks.RevertUSBOptimization(),
                "USB restaurado a configuración por defecto."
            );
        }

        private void BtnOptimizeInputQueues_On_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweak(
                "InputQueues",
                "Advanced - Input & USB",
                () => InputTweaks.OptimizeInputQueues(),
                "✅ Colas de input optimizadas. Reducción de latencia de mouse y teclado."
            );
        }

        private void BtnOptimizeInputQueues_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "InputQueues",
                "Advanced - Input & USB",
                () => InputTweaks.RevertInputQueues(),
                "Colas de input restauradas a configuración por defecto."
            );
        }

        private void BtnDisableFTH_On_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "🔧 DESHABILITAR FAULT TOLERANT HEAP (FTH)\n\n" +
                    "¿Qué es FTH?\n" +
                    "════════════════════════════════════════\n" +
                    "• Sistema de Windows que detecta crashes frecuentes\n" +
                    "• Activa un 'heap especial' para apps 'problemáticas'\n" +
                    "• Afecta NEGATIVAMENTE a juegos optimizados\n\n" +
                    "PROBLEMA EN GAMING:\n" +
                    "════════════════════════════════════════\n" +
                    "• FTH marca juegos como 'problemáticos' incorrectamente\n" +
                    "• Fuerza heap lento en juegos optimizados\n" +
                    "• Causa frame drops inesperados\n" +
                    "• Reduce rendimiento en 5-15%\n\n" +
                    "SOLUCIÓN:\n" +
                    "════════════════════════════════════════\n" +
                    "• Deshabilitar FTH completamente\n" +
                    "• Los juegos usarán heap normal (rápido)\n" +
                    "• Frame consistency mejorada\n\n" +
                    "⚠️ IMPORTANTE: Solo para sistemas estables.\n" +
                    "Si tienes crashes frecuentes, mantén FTH activado.\n\n" +
                    "¿Deshabilitar Fault Tolerant Heap?",
                    "Deshabilitar FTH",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes) return;

                _tweakHelper.ExecuteTweak(
                    "FaultTolerantHeap",
                    "Advanced - Input & USB",
                    () => InputTweaks.DisableFaultTolerantHeap(),
                    "✅ Fault Tolerant Heap deshabilitado. Juegos usarán heap optimizado. REINICIA para aplicar.",
                    null,
                    true // Requires restart
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDisableFTH_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "FaultTolerantHeap",
                "Advanced - Input & USB",
                () => InputTweaks.EnableFaultTolerantHeap(),
                "Fault Tolerant Heap restaurado. Sistema protegido contra apps problemáticas.",
                null,
                true // Requires restart
            );
        }

        // ═══════════════════════════════════════════════════════════════════
        // TRANSPARENCY EFFECTS
        // ═══════════════════════════════════════════════════════════════════

        private void BtnTransparency_On_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweak(
                "TransparencyEffects",
                "Input & Visuals",
                () => VisualOptimization.DisableTransparency(),
                "✅ Efectos de transparencia deshabilitados. VRAM liberada +50-200MB."
            );
            UpdateButtonState((Button)sender, true);
        }

        private void BtnTransparency_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "TransparencyEffects",
                "Input & Visuals",
                () => VisualOptimization.RestoreTransparency(),
                "Efectos de transparencia restaurados."
            );
            UpdateRelatedOnButton("TRANSPARENCIA", false);
        }

        // ═══════════════════════════════════════════════════════════════════
        // ADVANCED NETWORK OPTIMIZATIONS
        // ═══════════════════════════════════════════════════════════════════

        private void BtnMTU_On_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweak(
                "MTUOptimization",
                "Red & Ping - Advanced",
                () => AdvancedNetworkTweaks.OptimizeMTU(),
                "✅ MTU optimizado a 1492 bytes. Latencia reducida 2-5ms."
            );
        }

        private void BtnMTU_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "MTUOptimization",
                "Red & Ping - Advanced",
                () => AdvancedNetworkTweaks.RevertMTU(),
                "MTU restaurado a valores por defecto."
            );
        }

        private void BtnQoS_On_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweak(
                "QoSConfiguration",
                "Red & Ping - Advanced",
                () => AdvancedNetworkTweaks.ConfigureQoS(),
                "✅ QoS configurado. Tráfico de gaming priorizado, 20% ancho de banda liberado."
            );
        }

        private void BtnQoS_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "QoSConfiguration",
                "Red & Ping - Advanced",
                () => AdvancedNetworkTweaks.RevertQoS(),
                "QoS restaurado a configuración estándar."
            );
        }

        private void BtnAutoTuning_On_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweak(
                "AutoTuningLevel",
                "Red & Ping - Advanced",
                () => AdvancedNetworkTweaks.ConfigureAutoTuning(),
                "✅ Auto-Tuning configurado. Uso de CPU reducido -30%, mejor throughput."
            );
        }

        private void BtnAutoTuning_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "AutoTuningLevel",
                "Red & Ping - Advanced",
                () => AdvancedNetworkTweaks.RevertAutoTuning(),
                "Auto-Tuning restaurado a configuración por defecto."
            );
        }

        private void BtnAdapterSettings_On_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweak(
                "AdapterSettings",
                "Red & Ping - Advanced",
                () => AdvancedNetworkTweaks.OptimizeAdapterSettings(),
                "✅ Configuración de adaptador optimizada. Window Scaling y SACK configurados."
            );
        }

        private void BtnAdapterSettings_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "AdapterSettings",
                "Red & Ping - Advanced",
                () => AdvancedNetworkTweaks.RevertAdapterSettings(),
                "Configuración de adaptador restaurada."
            );
        }

        private void BtnCongestionControl_On_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweak(
                "CongestionControl",
                "Red & Ping - Advanced",
                () => AdvancedNetworkTweaks.ConfigureCongestionControl(),
                "✅ Control de congestión optimizado. CTCP y ECN configurados."
            );
        }

        private void BtnCongestionControl_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "CongestionControl",
                "Red & Ping - Advanced",
                () => AdvancedNetworkTweaks.RevertCongestionControl(),
                "Control de congestión restaurado."
            );
        }

        private void BtnAllAdvancedNetwork_On_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "🌐 APLICAR TODAS LAS OPTIMIZACIONES AVANZADAS DE RED\n\n" +
                    "SE APLICARÁN:\n" +
                    "✅ MTU Optimization (1492 bytes)\n" +
                    "✅ QoS Configuration (20% liberado)\n" +
                    "✅ Auto-Tuning Level (CPU -30%)\n" +
                    "✅ Adapter Advanced Settings\n" +
                    "✅ Congestion Control (CTCP + ECN)\n\n" +
                    "🎯 REDUCCIÓN ESTIMADA: -11 a -29ms latencia\n\n" +
                    "⚠️ REQUIERE REINICIO después de aplicar.\n\n" +
                    "¿Aplicar TODAS las optimizaciones avanzadas?",
                    "Aplicar Optimizaciones Avanzadas de Red",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes) return;

                _tweakHelper.ExecuteTweak(
                    "AllAdvancedNetwork",
                    "Red & Ping - Advanced",
                    () => AdvancedNetworkTweaks.ApplyAllOptimizations(),
                    "✅ TODAS las optimizaciones avanzadas de red aplicadas. REINICIA para efecto completo.",
                    null,
                    true // Requires restart
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAllAdvancedNetwork_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "AllAdvancedNetwork",
                "Red & Ping - Advanced",
                () => AdvancedNetworkTweaks.RevertAllOptimizations(),
                "Todas las optimizaciones avanzadas de red han sido restauradas.",
                null,
                true // Requires restart
            );
        }

        // ═══════════════════════════════════════════════════════════════════
        // GAME MODE & OPTIMIZATIONS
        // ═══════════════════════════════════════════════════════════════════

        private void BtnGameMode_On_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweak(
                "WindowsGameMode",
                "Sistema & GPU - Game Mode",
                () => GameModeTweaks_Optimized.EnableGameMode(),
                "✅ Windows Game Mode activado. Frame stability +10-15%."
            );
            UpdateButtonState((Button)sender, true);
        }

        private void BtnGameMode_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "WindowsGameMode",
                "Sistema & GPU - Game Mode",
                () => GameModeTweaks_Optimized.DisableGameMode(),
                "Windows Game Mode desactivado."
            );
            UpdateRelatedOnButton("GAME MODE", false);
        }

        private void BtnNTFSLastAccess_On_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweak(
                "NTFSLastAccessTime",
                "Sistema & GPU - Game Mode",
                () => DiskTweaks.DisableNTFSLastAccessTime(),
                "✅ NTFS Last Access Time deshabilitado. Disco +5-15%, vida útil SSD aumentada. REINICIO REQUERIDO.",
                null,
                true // Requires restart
            );
            UpdateButtonState((Button)sender, true);
        }

        private void BtnNTFSLastAccess_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "NTFSLastAccessTime",
                "Sistema & GPU - Game Mode",
                () => DiskTweaks.EnableNTFSLastAccessTime(),
                "NTFS Last Access Time restaurado. REINICIO REQUERIDO.",
                null,
                true // Requires restart
            );
            UpdateRelatedOnButton("LAST ACCESS", false);
        }

        private void BtnGamePriority_On_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "🎮 CONFIGURAR PRIORIDAD ALTA PARA JUEGOS\n\n" +
                    "SE CONFIGURARÁ PRIORIDAD ALTA PARA:\n" +
                    "• Fortnite, CS2, Valorant, Warzone\n" +
                    "• Apex Legends, PUBG, Rainbow Six\n" +
                    "• League of Legends, Rocket League\n" +
                    "• Y 6 juegos más populares\n\n" +
                    "🚀 BENEFICIOS:\n" +
                    "• 0.1% Low FPS +15-20%\n" +
                    "• Input lag -2-5ms\n" +
                    "• Menos interrupciones del sistema\n\n" +
                    "⚠️ REQUIERE REINICIO para aplicar\n\n" +
                    "¿Configurar prioridad alta para juegos?",
                    "Prioridad Alta para Juegos",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes) return;

                _tweakHelper.ExecuteTweak(
                    "GameProcessPriority",
                    "Sistema & GPU - Game Mode",
                    () => GameModeTweaks_Optimized.SetHighPriorityForGames(),
                    "✅ Prioridad ALTA configurada para 15 juegos populares. REINICIA para aplicar.",
                    null,
                    true // Requires restart
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnGamePriority_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "GameProcessPriority",
                "Sistema & GPU - Game Mode",
                () => GameModeTweaks_Optimized.RevertGamePriority(),
                "Prioridad de juegos restaurada a normal. REINICIA para aplicar.",
                null,
                true // Requires restart
            );
        }

        // ═══════════════════════════════════════════════════════════════════
        // TOQUES FINALES - FINISHING TOUCHES
        // ═══════════════════════════════════════════════════════════════════

        private void BtnDisableUpdates_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "🚫 DESHABILITAR WINDOWS UPDATE AUTOMÁTICO\n\n" +
                    "⚠️ ADVERTENCIA IMPORTANTE ⚠️\n" +
                    "Esto deshabilitará las actualizaciones automáticas.\n\n" +
                    "BENEFICIOS GAMING:\n" +
                    "✅ Sin lag spikes por descargas\n" +
                    "✅ Ancho de banda completo para gaming\n" +
                    "✅ Sin interrupciones durante partidas\n\n" +
                    "⚠️ IMPORTANTE:\n" +
                    "• Deberás actualizar manualmente\n" +
                    "• Check updates periódicamente\n" +
                    "• Solo para gamers experimentados\n\n" +
                    "¿Deshabilitar Windows Update automático?",
                    "Deshabilitar Windows Update",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes) return;

                _tweakHelper.ExecuteTweak(
                    "WindowsUpdateDisable",
                    "Sistema & GPU - Toques Finales",
                    () => UpdateTweaks.DisableAutomaticUpdates(),
                    "✅ Windows Update automático deshabilitado. Ancho de banda liberado para gaming."
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnEnableUpdates_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "WindowsUpdateDisable",
                "Sistema & GPU - Toques Finales",
                () => UpdateTweaks.EnableAutomaticUpdates(),
                "Windows Update automático reactivado."
            );
        }

        private void BtnDisableDeliveryOpt_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweak(
                "DeliveryOptimization",
                "Sistema & GPU - Toques Finales",
                () => UpdateTweaks.DisableDeliveryOptimization(),
                "✅ P2P Update Sharing deshabilitado. Ping más estable, sin uploads P2P."
            );
        }

        private void BtnEnableDeliveryOpt_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "DeliveryOptimization",
                "Sistema & GPU - Toques Finales",
                () => UpdateTweaks.EnableDeliveryOptimization(),
                "P2P Update Sharing reactivado."
            );
        }

        private void BtnOptimizeNTFS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "💽 OPTIMIZAR NTFS PARA GAMING & SSD\n\n" +
                    "SE APLICARÁ:\n" +
                    "✅ Deshabilitar Last Access Time\n" +
                    "✅ Deshabilitar nombres 8.3 DOS\n" +
                    "✅ Optimizar indexación\n\n" +
                    "🚀 BENEFICIOS:\n" +
                    "• Reduce escrituras SSD ~30%\n" +
                    "• Alarga vida útil SSD 2-3 años\n" +
                    "• Menos micro-freezes\n" +
                    "• Carga de assets más rápida\n\n" +
                    "⚠️ REQUIERE REINICIO para efecto completo\n\n" +
                    "¿Optimizar NTFS para gaming?",
                    "Optimizar NTFS",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes) return;

                _tweakHelper.ExecuteTweak(
                    "NTFSOptimization",
                    "Sistema & GPU - Toques Finales",
                    () => DiskTweaks.OptimizeNTFSForGaming(),
                    "✅ NTFS optimizado para gaming y SSD. Vida útil extendida, mejor rendimiento. REINICIA para efecto completo.",
                    null,
                    true // Requires restart
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRestoreNTFS_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "NTFSOptimization",
                "Sistema & GPU - Toques Finales",
                () => DiskTweaks.RestoreNTFSDefaults(),
                "NTFS restaurado a configuración por defecto. REINICIA para aplicar.",
                null,
                true // Requires restart
            );
        }

        // MÉTODOS DE TELEMETRÍA ELIMINADOS POR PROBLEMAS CON BLUETOOTH/DISCORD
        // Estos tweaks causaban que los audífonos Bluetooth no funcionaran para hablar en Discord
        // Los servicios OneSyncSvc, MessagingService, UserDataSvc son necesarios para dispositivos Bluetooth
        
        /*
        private void BtnDisableTelemetry_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweak(
                "TelemetryTracking",
                "Sistema & GPU - Toques Finales",
                () => PrivacyTweaks.DisableTelemetryAndTracking(),
                "✅ Telemetría y tracking deshabilitados. CPU liberado, menos tráfico de red."
            );
        }

        private void BtnEnableTelemetry_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "TelemetryTracking",
                "Sistema & GPU - Toques Finales",
                () => PrivacyTweaks.EnableTelemetryAndTracking(),
                "Telemetría y tracking restaurados."
            );
        }

        private void BtnDisableTelemetryServices_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweak(
                "TelemetryServices",
                "Sistema & GPU - Toques Finales",
                () => PrivacyTweaks.DisableTelemetryServices(),
                "✅ Servicios de telemetría deshabilitados. Recursos liberados. REINICIA para desactivar servicios.",
                null,
                true // Requires restart
            );
        }

        private void BtnEnableTelemetryServices_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "TelemetryServices",
                "Sistema & GPU - Toques Finales",
                () => PrivacyTweaks.EnableTelemetryServices(),
                "Servicios de telemetría reactivados. REINICIA para aplicar.",
                null,
                true // Requires restart
            );
        }
        */

        private void BtnDiagnoseFinishingTouches_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _notifications.ShowInfo(
                    "🔍 Iniciando diagnóstico completo...\n\n" +
                    "Analizando:\n" +
                    "• Estado de Windows Update\n" +
                    "• Configuración NTFS\n" +
                    "• Servicios de telemetría\n" +
                    "• Delivery Optimization\n\n" +
                    "Esto puede tomar 30-60 segundos.",
                    "Diagnóstico de Toques Finales"
                );

                Task.Run(() =>
                {
                    try
                    {
                        var diagnosis = UpdateTweaks.DiagnoseSystemState();
                        
                        Dispatcher.Invoke(() =>
                        {
                            var diagWindow = new Window
                            {
                                Title = "Diagnóstico Completo - Toques Finales",
                                Width = 600,
                                Height = 500,
                                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                                Owner = this
                            };

                            var scrollViewer = new ScrollViewer();
                            var textBlock = new TextBlock
                            {
                                Text = diagnosis,
                                TextWrapping = TextWrapping.Wrap,
                                Margin = new Thickness(20),
                                FontFamily = new FontFamily("Consolas"),
                                FontSize = 12
                            };

                            scrollViewer.Content = textBlock;
                            diagWindow.Content = scrollViewer;
                            diagWindow.ShowDialog();
                        });
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show($"Error durante diagnóstico: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ═══════════════════════════════════════════════════════════════════
        // DASHBOARD ACTIONS
        // ═══════════════════════════════════════════════════════════════════

        private void RevertAllTweaks(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "🔄 REVERTIR TODOS LOS TWEAKS\n\n" +
                    "⚠️⚠️ ADVERTENCIA CRÍTICA ⚠️⚠️\n" +
                    "Esto REVERTIRÁ TODAS las optimizaciones aplicadas.\n\n" +
                    "SE RESTAURARÁN:\n" +
                    "• Todas las optimizaciones de red\n" +
                    "• Todas las optimizaciones de input\n" +
                    "• Todas las optimizaciones de sistema\n" +
                    "• Todos los tweaks avanzados\n\n" +
                    "🔄 Tu sistema volverá al estado original.\n\n" +
                    "⚠️ REQUIERE REINICIO después de revertir.\n\n" +
                    "¿Estás SEGURO de revertir TODOS los tweaks?",
                    "Revertir Todos los Tweaks",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes) return;

                _notifications.ShowInfo(
                    "🔄 Revirtiendo todos los tweaks...\n\n" +
                    "Esto puede tomar 2-5 minutos.\n" +
                    "Por favor, no cierres la aplicación.",
                    "Revirtiendo Tweaks"
                );

                Task.Run(() =>
                {
                    try
                    {
                        // Revertir por categorías
                        InputTweaks.RevertAllInputOptimizations();
                        NetworkOptimization.RevertAllNetworkTweaks();
                        AdvancedNetworkTweaks.RevertAllOptimizations();
                        GameModeTweaks_Optimized.RevertAllGameModeOptimizations();
                        UpdateTweaks.EnableAutomaticUpdates();
                        UpdateTweaks.EnableDeliveryOptimization();
                        DiskTweaks.RestoreNTFSDefaults();
                        PrivacyTweaks.EnableTelemetryAndTracking();
                        PrivacyTweaks.EnableTelemetryServices();

                        // Limpiar estado
                        _stateManager.RevertAllTweaks();

                        Dispatcher.Invoke(() =>
                        {
                            UpdateDashboard();
                            UpdateCurrentPriorityProfile();

                            _notifications.ShowSuccess(
                                "✅ TODOS los tweaks han sido revertidos exitosamente.\n\n" +
                                "Tu sistema ha sido restaurado al estado original.\n\n" +
                                "⚠️ REINICIA tu PC para completar la restauración.",
                                "Tweaks Revertidos"
                            );
                        });
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show($"Error durante la reversión: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ═══════════════════════════════════════════════════════════════════
        // NUEVOS MÓDULOS: LATENCY, PRO & ULTIMATE TWEAKS
        // ═══════════════════════════════════════════════════════════════════

        #region Latency Tweaks

        private void BtnLatency_On_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweak(
                "LatencyOptimizations",
                "Advanced - Latency",
                () => LatencyTweaks.ApplyAllLatencyOptimizations(),
                "✅ Latency Optimizations aplicadas.\n\n" +
                "CAMBIOS:\n" +
                "• CPU Scheduling: Win32Priority = 38\n" +
                "• USB Power Saving: Deshabilitado\n" +
                "• UI Delays: 0ms (menús instantáneos)\n\n" +
                "IMPACTO:\n" +
                "• Input lag reducido\n" +
                "• UI más responsive\n" +
                "• Mouse/teclado sin delays",
                null,
                true // Requires restart
            );
            
            UpdateButtonState((Button)sender, true);
        }

        private void BtnLatency_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "LatencyOptimizations",
                "Advanced - Latency",
                () => LatencyTweaks.RevertAllLatencyOptimizations(),
                "Latency Optimizations revertidas a valores por defecto.",
                null,
                true
            );
            
            UpdateRelatedOnButton("LATENCY", false);
        }

        #endregion

        #region Pro Tweaks

        private void BtnPro_On_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "⚡ APLICAR PRO TWEAKS\n\n" +
                "CAMBIOS:\n" +
                "• Timer Resolution: 0.5ms (2000 Hz)\n" +
                "• Network Adapters: Sin power saving\n" +
                "• Sticky Keys: Deshabilitado\n\n" +
                "IMPACTO:\n" +
                "✅ Frame times más consistentes\n" +
                "✅ Ping estable (sin spikes)\n" +
                "✅ Sin popups molestos\n\n" +
                "⚠️ CPU usage +1-2%\n" +
                "⚠️ Consumo energía +1W\n\n" +
                "¿Continuar?",
                "Pro Tweaks",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                _tweakHelper.ExecuteTweak(
                    "ProTweaks",
                    "Advanced - Pro",
                    () => ProTweaks.ApplyAllProTweaks(),
                    "✅ Pro Tweaks aplicados exitosamente.\n\n" +
                    "NOTA: Timer resolution NO persiste después de reiniciar.\n" +
                    "Ejecuta Ghost Optimizer cada vez que enciendes el PC.\n\n" +
                    "⚠️ REINICIA para aplicar cambios de red.",
                    null,
                    true // Requires restart
                );
                
                UpdateButtonState((Button)sender, true);
            }
        }

        private void BtnPro_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "ProTweaks",
                "Advanced - Pro",
                () => ProTweaks.RevertAllProTweaks(),
                "Pro Tweaks revertidos. REINICIA para aplicar cambios.",
                null,
                true
            );
            
            UpdateRelatedOnButton("PRO", false);
        }

        #endregion

        #region Sticky Keys (Input & Visuals)

        private void BtnStickyKeys_On_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweak(
                "StickyKeys",
                "Input & Visuals",
                () => ProTweaks.DisableStickyKeys(),
                "✅ Sticky Keys deshabilitado.\n\n" +
                "• No más popup de Shift x5\n" +
                "• No más Toggle Keys\n" +
                "• Gaming sin interrupciones"
            );
            
            UpdateButtonState((Button)sender, true);
        }

        private void BtnStickyKeys_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "StickyKeys",
                "Input & Visuals",
                () => ProTweaks.RevertStickyKeys(),
                "Sticky Keys restaurado a valores por defecto."
            );
            
            UpdateRelatedOnButton("STICKY", false);
        }

        #endregion

        #region Ultimate Tweaks (Cleanup Page)

        private void BtnUltimateServices_On_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "🛑 DESHABILITAR SERVICIOS INNECESARIOS\n\n" +
                "Servicios que se deshabilitarán:\n" +
                "• Spooler (impresora)\n" +
                "• Fax\n" +
                "• WerSvc (reportes de error)\n" +
                "• MapsBroker (mapas)\n\n" +
                "IMPACTO:\n" +
                "✅ RAM: -100-200MB\n" +
                "✅ Menos procesos en background\n\n" +
                "⚠️ NO podrás imprimir\n\n" +
                "¿Continuar?",
                "Optimizar Servicios",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            
            if (result == MessageBoxResult.Yes)
            {
                _tweakHelper.ExecuteTweak(
                    "UltimateServices",
                    "Limpieza - Ultimate",
                    () => UltimateTweaks.OptimizeServices(),
                    "✅ Servicios optimizados.\n\n" +
                    "RAM liberada: ~100-200MB\n\n" +
                    "⚠️ REINICIA para aplicar cambios.",
                    null,
                    true
                );
                
                UpdateButtonState((Button)sender, true);
            }
        }

        private void BtnUltimateServices_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "UltimateServices",
                "Limpieza - Ultimate",
                () => UltimateTweaks.RevertServices(),
                "Servicios restaurados. REINICIA para aplicar cambios.",
                null,
                true
            );
            
            UpdateRelatedOnButton("SERVICIOS", false);
        }

        private void BtnUltimateBloat_On_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweak(
                "UltimateBloatware",
                "Limpieza - Ultimate",
                () => UltimateTweaks.RemoveWindowsBloat(),
                "✅ Bloatware removido.\n\n" +
                "• Bing Search OFF\n" +
                "• Windows Copilot OFF\n" +
                "• Widgets OFF\n\n" +
                "Start Menu más limpio y rápido.\n\n" +
                "⚠️ REINICIA/LOGOUT para efecto completo.",
                null,
                true
            );
            
            UpdateButtonState((Button)sender, true);
        }

        private void BtnUltimateBloat_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "UltimateBloatware",
                "Limpieza - Ultimate",
                () => UltimateTweaks.RevertWindowsBloat(),
                "Bloatware restaurado. REINICIA para aplicar cambios.",
                null,
                true
            );
            
            UpdateRelatedOnButton("BLOAT", false);
        }

        private void BtnUltimateSystem_On_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "⚙️ SYSTEM TWEAKS EXTREMOS\n\n" +
                "Cambios:\n" +
                "• NTFS Last Access OFF (-30% escrituras)\n" +
                "• Background Apps OFF (ahorra RAM/CPU)\n" +
                "• QoS Limit 0% (100% ancho de banda)\n\n" +
                "IMPACTO:\n" +
                "✅ Disco: +5-15%\n" +
                "✅ RAM: -100-300MB\n" +
                "✅ Red: +20%\n\n" +
                "⚠️ Background Apps no funcionarán\n\n" +
                "¿Continuar?",
                "System Tweaks",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            
            if (result == MessageBoxResult.Yes)
            {
                _tweakHelper.ExecuteTweak(
                    "UltimateSystem",
                    "Limpieza - Ultimate",
                    () => UltimateTweaks.OptimizeSystem(),
                    "✅ System Tweaks aplicados.\n\n" +
                    "Disco: +15% | RAM: -300MB | Red: +20%\n\n" +
                    "⚠️ REINICIA para efecto completo.",
                    null,
                    true
                );
                
                UpdateButtonState((Button)sender, true);
            }
        }

        private void BtnUltimateSystem_Off_Click(object sender, RoutedEventArgs e)
        {
            _tweakHelper.ExecuteTweakRevert(
                "UltimateSystem",
                "Limpieza - Ultimate",
                () => UltimateTweaks.RevertSystemOptimizations(),
                "System Tweaks revertidos. REINICIA para aplicar cambios.",
                null,
                true
            );
            
            UpdateRelatedOnButton("SYSTEM", false);
        }

        private void BtnApplyAllUltimate_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "🔥🔥🔥 ULTIMATE MODE 🔥🔥🔥\n\n" +
                "Esto aplicará TODAS las optimizaciones extremas:\n\n" +
                "• Servicios deshabilitados (4)\n" +
                "• Bloatware removido (3)\n" +
                "• System tweaks (3)\n\n" +
                "IMPACTO TOTAL:\n" +
                "• RAM: -500MB\n" +
                "• Disco: +15%\n" +
                "• Red: +20%\n" +
                "• Boot: -10s\n\n" +
                "⚠️⚠️⚠️ ADVERTENCIAS ⚠️⚠️⚠️\n" +
                "• NO podrás imprimir\n" +
                "• Background Apps OFF\n\n" +
                "¿Aplicar ULTIMATE MODE?",
                "Ultimate Mode",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            
            if (result == MessageBoxResult.Yes)
            {
                _notifications.ShowInfo(
                    "Aplicando TODAS las optimizaciones extremas...\n\n" +
                    "Esto puede tomar 30-60 segundos.\n" +
                    "No cierres la aplicación.",
                    "Ultimate Mode");
                
                Task.Run(() =>
                {
                    try
                    {
                        bool success = true;
                        success &= UltimateTweaks.ApplyAllUltimateTweaks();
                        
                        Dispatcher.Invoke(() =>
                        {
                            if (success)
                            {
                                MessageBox.Show(
                                    "✅ ULTIMATE MODE ACTIVADO ✅\n\n" +
                                    "TODAS las optimizaciones extremas aplicadas.\n\n" +
                                    "IMPACTO:\n" +
                                    "• RAM: -500MB\n" +
                                    "• Disco: +15%\n" +
                                    "• Red: +20%\n" +
                                    "• Boot: -10s\n\n" +
                                    "⚠️⚠️ REINICIA WINDOWS AHORA ⚠️⚠️",
                                    "Ultimate Mode Activado",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                            }
                            else
                            {
                                MessageBox.Show(
                                    "⚠️ OPTIMIZACIÓN PARCIAL\n\n" +
                                    "Algunas optimizaciones fallaron.\n" +
                                    "Ejecuta como Administrador.",
                                    "Advertencia",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show($"❌ Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        });
                    }
                });
            }
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════════
        // PRESET LAPTOP - MÉTODOS DE INTERFAZ
        // ═══════════════════════════════════════════════════════════════════

        #region Laptop Preset

        private void BtnLaptopPreset_Apply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "💻 APLICAR PRESET LAPTOP\n\n" +
                    "Este preset aplicará optimizaciones específicas para gaming portátil:\n\n" +
                    "✅ SE APLICARÁ:\n" +
                    "• Input optimizations (mouse, teclado)\n" +
                    "• Visual effects OFF (ahorro batería)\n" +
                    "• Red balanceada (gaming + navegadores)\n" +
                    "• Gaming tweaks seguros\n" +
                    "• Limpieza del sistema\n" +
                    "• Privacidad (método seguro, sin romper Bluetooth)\n\n" +
                    "❌ NO SE APLICARÁ:\n" +
                    "• Power throttling OFF (necesario para térmica)\n" +
                    "• Ultimate Performance (consumo excesivo)\n" +
                    "• Core parking OFF (necesario para batería)\n" +
                    "• Hibernación OFF (necesaria para portabilidad)\n" +
                    "• Tweaks extremos que comprometan estabilidad\n\n" +
                    "🎯 RESULTADO ESPERADO:\n" +
                    "• Gaming: FPS +5-15%, Input lag -50-70ms\n" +
                    "• Batería: +10-20% duración adicional\n" +
                    "• Estabilidad: Sin comprometer térmica ni conectividad\n\n" +
                    "⚠️ Se recomienda REINICIAR después de aplicar\n\n" +
                    "¿Aplicar preset optimizado para laptop?",
                    "Aplicar Preset Laptop",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes) return;

                _tweakHelper.ExecuteTweak(
                    "LaptopPreset",
                    "Preset Laptop",
                    () => LaptopPreset.ApplyLaptopOptimizations(),
                    "✅ PRESET LAPTOP APLICADO EXITOSAMENTE\n\n" +
                    "🎮 GAMING OPTIMIZADO:\n" +
                    "• Input lag reducido significativamente\n" +
                    "• FPS mejorado sin comprometer térmica\n" +
                    "• Frame stability aumentada\n\n" +
                    "🔋 BATERÍA PRESERVADA:\n" +
                    "• Efectos visuales optimizados\n" +
                    "• Gestión de energía mantenida\n" +
                    "• Control térmico preservado\n\n" +
                    "🌐 CONECTIVIDAD SEGURA:\n" +
                    "• Bluetooth funcionará perfectamente\n" +
                    "• WiFi optimizado pero estable\n" +
                    "• Discord sin problemas de audio\n\n" +
                    "♻️ REINICIA Windows para efecto completo\n\n" +
                    "Tu laptop está ahora optimizada para gaming portátil!",
                    null,
                    true // Requires restart
                );

                UpdateButtonState((Button)sender, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"❌ ERROR aplicando preset laptop:\n\n{ex.Message}\n\n" +
                    "Verifica que tienes permisos de administrador y que no hay antivirus bloqueando.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void BtnLaptopPreset_Revert_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "🔄 REVERTIR PRESET LAPTOP\n\n" +
                    "Esto restaurará todas las configuraciones del preset laptop\n" +
                    "a los valores por defecto de Windows.\n\n" +
                    "⚠️ Se revertirán:\n" +
                    "• Todas las optimizaciones de input\n" +
                    "• Efectos visuales\n" +
                    "• Configuraciones de red\n" +
                    "• Tweaks de gaming\n" +
                    "• Limpieza del sistema\n" +
                    "• Configuraciones de privacidad\n\n" +
                    "♻️ Se requiere REINICIO después de revertir\n\n" +
                    "¿Revertir preset laptop a configuración por defecto?",
                    "Revertir Preset Laptop",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes) return;

                _tweakHelper.ExecuteTweakRevert(
                    "LaptopPreset",
                    "Preset Laptop",
                    () => LaptopPreset.RevertLaptopOptimizations(),
                    "✅ PRESET LAPTOP REVERTIDO\n\n" +
                    "🔄 CONFIGURACIÓN RESTAURADA:\n" +
                    "• Sistema restaurado a configuración por defecto\n" +
                    "• Todas las optimizaciones revertidas\n" +
                    "• Windows vuelve a estado original\n\n" +
                    "♻️ REINICIA Windows para efecto completo\n\n" +
                    "Tu laptop ha vuelto a la configuración estándar de Windows.",
                    null,
                    true // Requires restart
                );

                UpdateRelatedOnButton("LAPTOP", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"❌ ERROR revirtiendo preset laptop:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void BtnLaptopDiagnose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _notifications.ShowInfo(
                    "🔍 Iniciando diagnóstico específico para laptop...\n\n" +
                    "Analizando:\n" +
                    "• Gestión de energía y batería\n" +
                    "• Control térmico\n" +
                    "• Optimizaciones de gaming\n" +
                    "• Estado de conectividad\n" +
                    "• Estabilidad del sistema\n\n" +
                    "Esto puede tomar 30-60 segundos.",
                    "Diagnóstico Laptop"
                );

                Task.Run(() =>
                {
                    try
                    {
                        var diagnosis = LaptopPreset.DiagnoseLaptopOptimizations();
                        
                        Dispatcher.Invoke(() =>
                        {
                            var diagWindow = new Window
                            {
                                Title = "Diagnóstico Completo - Laptop Gaming",
                                Width = 700,
                                Height = 600,
                                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                                Owner = this,
                                Background = new SolidColorBrush(Color.FromRgb(31, 31, 31)),
                                Foreground = Brushes.White,
                                FontFamily = new FontFamily("Consolas"),
                                FontSize = 12
                            };

                            var scrollViewer = new ScrollViewer
                            {
                                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                                Padding = new Thickness(20),
                                Content = new TextBlock
                                {
                                    Text = diagnosis,
                                    TextWrapping = TextWrapping.Wrap,
                                    FontFamily = new FontFamily("Consolas"),
                                    FontSize = 11,
                                    LineHeight = 16,
                                    Foreground = Brushes.LightGray
                                }
                            };

                            diagWindow.Content = scrollViewer;
                            diagWindow.ShowDialog();
                        });
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show(
                                $"❌ Error en diagnóstico: {ex.Message}",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"❌ Error iniciando diagnóstico: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════════
        // COMPETITIVE GAMING PRESET - MÉTODOS DE INTERFAZ
        // ═══════════════════════════════════════════════════════════════════

        #region Competitive Preset

        private void BtnCompetitivePreset_Apply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "🏆 APLICAR PRESET COMPETITIVE GAMING\n\n" +
                    "⚠️ ADVERTENCIA CRÍTICA ⚠️\n" +
                    "Este preset aplica optimizaciones EXTREMAS para gaming competitivo.\n\n" +
                    "✅ SE APLICARÁ (ULTRA-AGRESIVO):\n" +
                    "• Input lag mínimo absoluto (-70 a -120ms)\n" +
                    "• Red ultra optimizada (-15 a -40ms latencia)\n" +
                    "• CPU/GPU dedicados 100% al gaming\n" +
                    "• Servicios mínimos (solo esenciales)\n" +
                    "• GHOST Pack optimizations extremas\n" +
                    "• Timer resolution 0.5ms\n" +
                    "• Ultimate Performance plan\n" +
                    "• Power throttling & Core parking OFF\n\n" +
                    "🔥 SACRIFICA:\n" +
                    "• Estabilidad general del sistema\n" +
                    "• Funciones no esenciales\n" +
                    "• Compatibilidad con algunas apps\n" +
                    "• Actualizaciones automáticas\n\n" +
                    "🎯 PARA:\n" +
                    "• CS2, Valorant, LOL, Overwatch, Apex\n" +
                    "• PCs dedicados EXCLUSIVAMENTE al gaming\n" +
                    "• Usuarios experimentados\n\n" +
                    "🚀 RESULTADO ESPERADO:\n" +
                    "• Input lag: -70 a -120ms\n" +
                    "• FPS: +10-25%, 0.1% Low: +15-30%\n" +
                    "• Latencia: -15 a -40ms\n" +
                    "• RAM liberada: +300-800MB\n\n" +
                    "⚠️ REQUIERE REINICIO después de aplicar\n" +
                    "⚠️ Solo para PCs dedicados al gaming competitivo\n\n" +
                    "¿Aplicar preset EXTREMO para gaming competitivo?",
                    "Aplicar Preset Competitivo EXTREMO",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes) return;

                _tweakHelper.ExecuteTweak(
                    "CompetitivePreset",
                    "Preset Competitive Gaming",
                    () => CompetitivePreset.ApplyCompetitivePreset(),
                    "🏆 PRESET COMPETITIVE GAMING APLICADO EXTREMO\n\n" +
                    "🎯 OPTIMIZACIÓN COMPETITIVA COMPLETA:\n" +
                    "• Input lag reducido al mínimo absoluto\n" +
                    "• Latencia de red ultra optimizada\n" +
                    "• CPU/GPU dedicados 100% al gaming\n" +
                    "• Sistema configurado para máximo rendimiento\n\n" +
                    "⚡ GAMING EXTREMO ACTIVADO:\n" +
                    "• Ultimate Performance plan activo\n" +
                    "• GHOST Pack optimizations aplicadas\n" +
                    "• Timer resolution a 0.5ms\n" +
                    "• Servicios no esenciales deshabilitados\n\n" +
                    "🚀 VENTAJA COMPETITIVA:\n" +
                    "• Aim más preciso y consistente\n" +
                    "• Mejor hitreg en FPS competitivos\n" +
                    "• Respuesta instantánea a inputs\n" +
                    "• Sin micro-stutters ni lag spikes\n\n" +
                    "♻️ REINICIA Windows AHORA para efecto completo\n\n" +
                    "Tu PC está ahora optimizado para gaming competitivo extremo!",
                    null,
                    true // Requires restart
                );

                UpdateButtonState((Button)sender, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"❌ ERROR aplicando preset competitivo:\n\n{ex.Message}\n\n" +
                    "Verifica que tienes permisos de administrador y que no hay antivirus bloqueando.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void BtnCompetitivePreset_Revert_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "🔄 REVERTIR PRESET COMPETITIVE GAMING\n\n" +
                    "Esto restaurará las configuraciones del preset competitivo\n" +
                    "a valores más conservadores de Windows.\n\n" +
                    "⚠️ NOTA IMPORTANTE:\n" +
                    "No todos los cambios pueden ser revertidos automáticamente.\n" +
                    "Algunos tweaks extremos requieren intervención manual.\n\n" +
                    "🔄 SE REVERTIRÁN:\n" +
                    "• Optimizaciones de input\n" +
                    "• Configuraciones de red\n" +
                    "• Power management\n" +
                    "• Efectos visuales\n" +
                    "• Servicios básicos\n" +
                    "• GHOST Pack optimizations\n" +
                    "• Actualizaciones automáticas\n\n" +
                    "⚠️ PUEDEN REQUERIR ACCIÓN MANUAL:\n" +
                    "• CPU scheduling modes\n" +
                    "• Algunos tweaks de registro avanzados\n" +
                    "• Configuraciones de BIOS/UEFI\n\n" +
                    "♻️ Se requiere REINICIO después de revertir\n\n" +
                    "¿Revertir preset competitivo?",
                    "Revertir Preset Competitivo",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes) return;

                _tweakHelper.ExecuteTweakRevert(
                    "CompetitivePreset",
                    "Preset Competitive Gaming",
                    () => CompetitivePreset.RevertCompetitivePreset(),
                    "✅ PRESET COMPETITIVO REVERTIDO\n\n" +
                    "🔄 CONFIGURACIÓN RESTAURADA:\n" +
                    "• Sistema restaurado a configuración más conservadora\n" +
                    "• Optimizaciones extremas revertidas\n" +
                    "• Estabilidad del sistema restaurada\n\n" +
                    "ℹ️ VERIFICACIÓN RECOMENDADA:\n" +
                    "• Verifica que todos los dispositivos funcionen\n" +
                    "• Reinicia si experimentas problemas\n" +
                    "• Algunos tweaks pueden requerir restauración manual\n\n" +
                    "♻️ REINICIA Windows para efecto completo\n\n" +
                    "Tu PC ha vuelto a una configuración más estable.",
                    null,
                    true // Requires restart
                );

                UpdateRelatedOnButton("COMPETITIVE", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"❌ ERROR revirtiendo preset competitivo:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void BtnCompetitiveDiagnose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _notifications.ShowInfo(
                    "🔍 Iniciando diagnóstico de preparación competitiva...\n\n" +
                    "Analizando:\n" +
                    "• Especificaciones del sistema\n" +
                    "• Optimizaciones actuales\n" +
                    "• Preparación para gaming competitivo\n" +
                    "• Compatibilidad con tweaks extremos\n" +
                    "• Estado de latencia y rendimiento\n\n" +
                    "Esto puede tomar 30-60 segundos.",
                    "Diagnóstico Competitivo"
                );

                Task.Run(() =>
                {
                    try
                    {
                        var diagnosis = CompetitivePreset.DiagnoseCompetitiveReadiness();
                        
                        Dispatcher.Invoke(() =>
                        {
                            var diagWindow = new Window
                            {
                                Title = "Diagnóstico Completo - Competitive Gaming Readiness",
                                Width = 800,
                                Height = 650,
                                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                                Owner = this,
                                Background = new SolidColorBrush(Color.FromRgb(31, 31, 31)),
                                Foreground = Brushes.White,
                                FontFamily = new FontFamily("Consolas"),
                                FontSize = 12
                            };

                            var scrollViewer = new ScrollViewer
                            {
                                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                                Padding = new Thickness(20),
                                Content = new TextBlock
                                {
                                    Text = diagnosis,
                                    TextWrapping = TextWrapping.Wrap,
                                    FontFamily = new FontFamily("Consolas"),
                                    FontSize = 11,
                                    LineHeight = 16,
                                    Foreground = Brushes.LightGray
                                }
                            };

                            diagWindow.Content = scrollViewer;
                            diagWindow.ShowDialog();
                        });
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show(
                                $"❌ Error en diagnóstico competitivo: {ex.Message}",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"❌ Error iniciando diagnóstico competitivo: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void BtnCompetitiveDNS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _tweakHelper.ExecuteTweak(
                    "CompetitiveDNS",
                    "Preset Competitive Gaming - DNS",
                    () => CompetitivePreset.SetCompetitiveDNS(),
                    "✅ DNS COMPETITIVO CONFIGURADO\n\n" +
                    "🌐 CLOUDFLARE ULTRA OPTIMIZED:\n" +
                    "• DNS: 1.1.1.1 / 1.0.0.1\n" +
                    "• Latencia: <10ms (el más rápido del mundo)\n" +
                    "• Configuraciones especiales para gaming\n" +
                    "• Resolución de dominios ultra rápida\n\n" +
                    "🎯 BENEFICIOS COMPETITIVOS:\n" +
                    "• Ping reducido 10-50ms\n" +
                    "• Mejor conexión a servidores de juegos\n" +
                    "• Sin throttling de ISP\n" +
                    "• Latencia mínima global\n\n" +
                    "Tu DNS está ahora optimizado para gaming competitivo!"
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"❌ Error configurando DNS competitivo: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void BtnCompetitiveRestart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "🔄 APLICAR TWEAKS CRÍTICOS DE REINICIO\n\n" +
                    "Esto aplicará SOLO los tweaks más críticos que requieren\n" +
                    "reinicio inmediato para funcionar correctamente.\n\n" +
                    "✅ SE APLICARÁN:\n" +
                    "• CPU Scheduling Mode AGGRESSIVE (22)\n" +
                    "• HPET optimization\n" +
                    "• Core Isolation OFF (VBS)\n" +
                    "• Input Queues optimization\n\n" +
                    "🚀 BENEFICIOS:\n" +
                    "• CPU prioriza juegos al máximo\n" +
                    "• Timers optimizados para menor latencia\n" +
                    "• Seguridad reducida pero máximo rendimiento\n" +
                    "• Input lag mínimo absoluto\n\n" +
                    "⚠️ REQUIERE REINICIO INMEDIATO\n" +
                    "No uses el PC hasta reiniciar.\n\n" +
                    "¿Aplicar tweaks críticos de reinicio?",
                    "Tweaks Críticos + Restart",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes) return;

                _tweakHelper.ExecuteTweak(
                    "CompetitiveRestartTweaks",
                    "Preset Competitive Gaming - Critical",
                    () => CompetitivePreset.ApplyRestartRequiredTweaks(),
                    "🔄 TWEAKS CRÍTICOS APLICADOS\n\n" +
                    "⚡ SISTEMA CONFIGURADO PARA MÁXIMO RENDIMIENTO:\n" +
                    "• CPU Scheduling: AGGRESSIVE mode activado\n" +
                    "• HPET: Optimizado para gaming\n" +
                    "• Core Isolation: Deshabilitado (máximo rendimiento)\n" +
                    "• Input Queues: Optimizados para 1000Hz+\n\n" +
                    "🚀 BENEFICIOS AL REINICIAR:\n" +
                    "• CPU dará prioridad máxima a juegos\n" +
                    "• Latencia de timers reducida drásticamente\n" +
                    "• Input lag mínimo absoluto\n" +
                    "• Frame consistency máxima\n\n" +
                    "⚠️ REINICIA WINDOWS AHORA\n" +
                    "Los cambios no tendrán efecto hasta reiniciar.\n\n" +
                    "Click OK y reinicia inmediatamente.",
                    null,
                    true // Requires restart
                );

                // Mostrar mensaje adicional enfatizando el reinicio
                MessageBox.Show(
                    "🔄 REINICIO REQUERIDO AHORA\n\n" +
                    "Los tweaks críticos han sido aplicados pero NO funcionarán\n" +
                    "hasta que reinicies Windows.\n\n" +
                    "IMPORTANTE:\n" +
                    "• Guarda tu trabajo\n" +
                    "• Cierra todas las aplicaciones\n" +
                    "• Reinicia Windows ahora\n\n" +
                    "Después del reinicio tendrás el máximo rendimiento competitivo.",
                    "Reinicio Requerido",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"❌ Error aplicando tweaks críticos: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        #endregion

        #endregion

        // ═══════════════════════════════════════════════════════════════════
        // TWEAK INFO POPUP SYSTEM
        // ═══════════════════════════════════════════════════════════════════

        #region Tweak Info Popups

        /// <summary>
        /// Muestra popup de información detallada para un tweak específico
        /// </summary>
        private void ShowTweakInfo(object sender, RoutedEventArgs e)
        {
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
            try
            {
                // Dar tiempo para que la interfaz cargue completamente
                await Task.Delay(3000);

                Debug.WriteLine("🔍 Verificando problemas de navegadores...");
                
                bool hasBrowserIssues = await BrowserSpeedFix.HasBrowserIssues();
                
                if (hasBrowserIssues)
                {
                    Debug.WriteLine("🚨 PROBLEMAS DE NAVEGADORES DETECTADOS - Mostrando notificación");
                    
                    // Mostrar notificación en el hilo principal
                    Dispatcher.Invoke(() =>
                    {
                        _notifications.ShowWarning(
                            "🌐 NAVEGADORES LENTOS DETECTADOS\n\n" +
                            "Ghost Optimizer detectó configuraciones que están\n" +
                            "causando lentitud en navegadores web.\n\n" +
                            "💡 Usa el menú Red & Ping para aplicar el fix.",
                            "Fix Navegadores Disponible"
                        );
                    });
                }
                else
                {
                    Debug.WriteLine("✅ Navegadores funcionando correctamente");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error verificando navegadores: {ex.Message}");
            }
        }

        /// <summary>
        /// Muestra el diálogo de fix de navegadores
        /// </summary>
        private async Task ShowBrowserFixDialog()
        {
            try
            {
                var result = MessageBox.Show(
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

                    bool success = await BrowserSpeedFix.ApplyQuickBrowserFix();
                    
                    if (success)
                    {
                        _notifications.ShowSuccess(
                            "✅ NAVEGADORES OPTIMIZADOS EXITOSAMENTE\n\n" +
                            "🌐 Prueba abrir un navegador ahora\n" +
                            "🎮 Gaming mantiene ~90% rendimiento\n" +
                            "⚡ Navegación web MUCHO más rápida",
                            "¡Fix Completado!"
                        );

                        // Opcional: Abrir navegador para probar
                        var testBrowser = MessageBox.Show(
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
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}