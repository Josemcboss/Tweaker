using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Tweaker.Optimizations;
using Tweaker.Utilities;
using Application = System.Windows.Application;

namespace Tweaker.Services
{
    /// <summary>
    /// Servicio de Bandeja del Sistema (System Tray) nativo Win32/WPF para Ghost Optimizer.
    /// Proporciona minimizado sigiloso y menú contextual nativo WPF con tema oscuro gaming.
    /// </summary>
    public sealed class SystemTrayService : IDisposable
    {
        private static readonly Lazy<SystemTrayService> _instance = new(() => new SystemTrayService());
        public static SystemTrayService Instance => _instance.Value;

        #region Win32 Native Shell_NotifyIcon

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct NOTIFYICONDATA
        {
            public int cbSize;
            public IntPtr hWnd;
            public int uID;
            public int uFlags;
            public int uCallbackMessage;
            public IntPtr hIcon;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szTip;
            public int dwState;
            public int dwStateMask;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szInfo;
            public int uTimeoutOrVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string szInfoTitle;
            public int dwInfoFlags;
            public Guid guidItem;
            public IntPtr hBalloonIcon;
        }

        private const int NIM_ADD = 0x00000000;
        private const int NIM_MODIFY = 0x00000001;
        private const int NIM_DELETE = 0x00000002;
        private const int NIM_SETVERSION = 0x00000004;

        private const int NIF_MESSAGE = 0x00000001;
        private const int NIF_ICON = 0x00000002;
        private const int NIF_TIP = 0x00000004;
        private const int NIF_INFO = 0x00000010;

        private const int NIIF_NONE = 0x00000000;
        private const int NIIF_INFO = 0x00000001;
        private const int NIIF_WARNING = 0x00000002;
        private const int NIIF_ERROR = 0x00000003;

        private const int WM_USER = 0x0400;
        private const int WM_TRAYICON = WM_USER + 1024;

        private const int WM_LBUTTONUP = 0x0202;
        private const int WM_LBUTTONDBLCLK = 0x0203;
        private const int WM_RBUTTONUP = 0x0205;

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern bool Shell_NotifyIcon(int dwMessage, ref NOTIFYICONDATA lpData);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }

        #endregion

        private HwndSource? _hwndSource;
        private IntPtr _hIcon = IntPtr.Zero;
        private bool _isIconAdded = false;
        private bool _isDisposed = false;
        private bool _hasShownFirstMinimizeTip = false;
        private ContextMenu? _trayContextMenu;

        private SystemTrayService()
        {
        }

        /// <summary>
        /// Inicializa el icono en el System Tray
        /// </summary>
        public void Initialize()
        {
            if (_isIconAdded) return;

            Application.Current?.Dispatcher.Invoke(() =>
            {
                try
                {
                    // Crear un mensaje receptor oculto usando HwndSource
                    var parameters = new HwndSourceParameters("GhostOptimizerTrayListener")
                    {
                        WindowStyle = 0,
                        ParentWindow = IntPtr.Zero
                    };

                    _hwndSource = new HwndSource(parameters);
                    _hwndSource.AddHook(WndProcHook);

                    _hIcon = ExtractAppIcon();
                    _trayContextMenu = CreateWpfContextMenu();

                    var nid = new NOTIFYICONDATA
                    {
                        cbSize = Marshal.SizeOf(typeof(NOTIFYICONDATA)),
                        hWnd = _hwndSource.Handle,
                        uID = 1001,
                        uFlags = NIF_MESSAGE | NIF_ICON | NIF_TIP,
                        uCallbackMessage = WM_TRAYICON,
                        hIcon = _hIcon,
                        szTip = "Ghost Optimizer - Gaming Tweaker"
                    };

                    _isIconAdded = Shell_NotifyIcon(NIM_ADD, ref nid);
                    Debug.WriteLine(_isIconAdded ? "✅ System Tray Icon agregado exitosamente" : "⚠️ Error agregando System Tray Icon");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"❌ Error inicializando SystemTrayService: {ex.Message}");
                }
            });
        }

        private IntPtr ExtractAppIcon()
        {
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string iconPath = Path.Combine(baseDir, "Resources", "GhostOptimizer.ico");

                if (File.Exists(iconPath))
                {
                    using var fs = File.OpenRead(iconPath);
                    var decoder = BitmapDecoder.Create(fs, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    if (decoder.Frames.Count > 0)
                    {
                        // Extraer handle de icono de Windows
                        return System.Drawing.Icon.ExtractAssociatedIcon(Process.GetCurrentProcess().MainModule?.FileName ?? "")?.Handle ?? IntPtr.Zero;
                    }
                }

                var mainModule = Process.GetCurrentProcess().MainModule;
                if (mainModule?.FileName != null)
                {
                    using var icon = System.Drawing.Icon.ExtractAssociatedIcon(mainModule.FileName);
                    if (icon != null)
                    {
                        return icon.Handle;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"⚠️ ExtractAppIcon: {ex.Message}");
            }

            return IntPtr.Zero;
        }

        private IntPtr WndProcHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_TRAYICON)
            {
                int eventMsg = lParam.ToInt32();

                switch (eventMsg)
                {
                    case WM_LBUTTONDBLCLK:
                    case WM_LBUTTONUP:
                        RestoreMainWindow();
                        handled = true;
                        break;

                    case WM_RBUTTONUP:
                        ShowContextMenu();
                        handled = true;
                        break;
                }
            }

            return IntPtr.Zero;
        }

        private ContextMenu CreateWpfContextMenu()
        {
            var menu = new ContextMenu
            {
                Background = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#18191C")),
                Foreground = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#F2F3F5")),
                BorderBrush = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#2E3035")),
                BorderThickness = new Thickness(1),
                FontSize = 12.5
            };

            // 1. Abrir Ghost Optimizer
            var itemOpen = new MenuItem
            {
                Header = "⚡ Abrir Ghost Optimizer",
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FFFFFF"))
            };
            itemOpen.Click += (s, e) => RestoreMainWindow();
            menu.Items.Add(itemOpen);

            menu.Items.Add(new Separator { Background = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#2E3035")) });

            // 2. Activar / Desactivar Modo Juego
            var itemGameMode = new MenuItem
            {
                Header = "🎮 Modo Juego (Activar / Desactivar)",
                Foreground = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#5865F2"))
            };
            itemGameMode.Click += (s, e) => ToggleGameMode();
            menu.Items.Add(itemGameMode);

            // 3. Limpiar RAM en 1 segundo
            var itemCleanRam = new MenuItem
            {
                Header = "🧹 Limpiar RAM ahora (1s)",
                Foreground = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#00D9FF"))
            };
            itemCleanRam.Click += async (s, e) => await CleanRamQuickAsync();
            menu.Items.Add(itemCleanRam);

            // 4. Forzar Timer Resolution a 0.5 ms
            var itemTimer = new MenuItem
            {
                Header = "⏱️ Forzar Timer Resolution (0.5 ms)",
                Foreground = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FFB800"))
            };
            itemTimer.Click += (s, e) => ToggleTimerResolution();
            menu.Items.Add(itemTimer);

            // 5. Planes de Energía al vuelo
            var itemPower = new MenuItem
            {
                Header = "⚡ Planes de Energía",
                Foreground = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#00FF88"))
            };

            var planUltimate = new MenuItem { Header = "⚡ Ultimate Performance", Foreground = Brushes.White };
            planUltimate.Click += (s, e) => SwitchPlan("ultimate");

            var planHigh = new MenuItem { Header = "🚀 Alto Rendimiento", Foreground = Brushes.White };
            planHigh.Click += (s, e) => SwitchPlan("high");

            var planBalanced = new MenuItem { Header = "⚖️ Equilibrado", Foreground = Brushes.White };
            planBalanced.Click += (s, e) => SwitchPlan("balanced");

            itemPower.Items.Add(planUltimate);
            itemPower.Items.Add(planHigh);
            itemPower.Items.Add(planBalanced);
            menu.Items.Add(itemPower);

            menu.Items.Add(new Separator { Background = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#2E3035")) });

            // 6. Salir
            var itemExit = new MenuItem
            {
                Header = "❌ Salir de Ghost Optimizer",
                Foreground = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#ED4245"))
            };
            itemExit.Click += (s, e) => ExitApplication();
            menu.Items.Add(itemExit);

            return menu;
        }

        private void ShowContextMenu()
        {
            if (_trayContextMenu == null) return;

            Application.Current?.Dispatcher.Invoke(() =>
            {
                UpdateDynamicMenuLabels();

                _trayContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
                _trayContextMenu.IsOpen = false; // reset in case
                _trayContextMenu.IsOpen = true;

                if (_hwndSource != null)
                {
                    SetForegroundWindow(_hwndSource.Handle);
                }
            });
        }

        private void UpdateDynamicMenuLabels()
        {
            if (_trayContextMenu == null) return;

            try
            {
                bool isGameActive = GameBoosterService.Instance.IsGameModeActive;
                bool isTimerHigh = TimerResolutionOptimization.IsHighPrecisionTimerActive() ?? false;
                double currMs = TimerResolutionOptimization.GetCurrentResolutionMs();

                foreach (var item in _trayContextMenu.Items)
                {
                    if (item is MenuItem mi)
                    {
                        if (mi.Header.ToString()!.Contains("Modo Juego"))
                        {
                            mi.Header = isGameActive
                                ? "🎮 Modo Juego: ACTIVO (Desactivar)"
                                : "🎮 Activar Modo Juego";
                        }
                        else if (mi.Header.ToString()!.Contains("Timer"))
                        {
                            mi.Header = isTimerHigh
                                ? $"⏱️ Timer: {currMs:F3} ms (Restaurar)"
                                : "⏱️ Forzar Timer Resolution (0.5 ms)";
                        }
                    }
                }
            }
            catch { }
        }

        private void ToggleGameMode()
        {
            var booster = GameBoosterService.Instance;
            if (booster.IsGameModeActive)
            {
                booster.DisableGameMode();
                ShowNotification("Ghost Optimizer", "🎮 Modo Juego Desactivado. Recursos restaurados.");
            }
            else
            {
                booster.EnableManualGameMode();
                ShowNotification("Ghost Optimizer", "⚡ Modo Juego Activado. Máxima prioridad y latencia reducida.");
            }
        }

        private async Task CleanRamQuickAsync()
        {
            ShowNotification("Ghost Optimizer", "🧹 Purgando Standby List y Working Sets...");

            var (success, mbCleaned, processesProcessed) = await Task.Run(() => MemoryCleaner.FlushMemory());

            string msg = success
                ? $"✅ Limpieza completada: ~{mbCleaned} MB liberados en {processesProcessed} procesos."
                : "⚠️ No se pudo completar la purga de memoria.";

            ShowNotification("Ghost Optimizer - Memoria", msg);
        }

        private void ToggleTimerResolution()
        {
            bool isHigh = TimerResolutionOptimization.IsHighPrecisionTimerActive() ?? false;

            if (isHigh)
            {
                TimerResolutionOptimization.RestoreTimerResolution();
                ShowNotification("Ghost Optimizer", "⏱️ Timer Resolution restaurado al valor estándar de Windows.");
            }
            else
            {
                TimerResolutionOptimization.EnableHighPrecisionTimer();
                double curr = TimerResolutionOptimization.GetCurrentResolutionMs();
                ShowNotification("Ghost Optimizer", $"⚡ Timer Resolution forzado a {curr:F3} ms (Máxima precisión).");
            }
        }

        private void SwitchPlan(string plan)
        {
            Task.Run(() =>
            {
                bool success = PowerTweaks.SwitchPowerPlan(plan);
                string planName = plan switch
                {
                    "ultimate" => "Ultimate Performance",
                    "high" => "Alto Rendimiento",
                    "balanced" => "Equilibrado",
                    _ => plan
                };

                ShowNotification(
                    "Ghost Optimizer - Plan de Energía",
                    success ? $"Plan cambiado a: {planName}" : "No se pudo cambiar el plan de energía");
            });
        }

        /// <summary>
        /// Muestra un globo informativo del sistema en el System Tray
        /// </summary>
        public void ShowNotification(string title, string text)
        {
            if (!_isIconAdded || _hwndSource == null) return;

            try
            {
                var nid = new NOTIFYICONDATA
                {
                    cbSize = Marshal.SizeOf(typeof(NOTIFYICONDATA)),
                    hWnd = _hwndSource.Handle,
                    uID = 1001,
                    uFlags = NIF_INFO,
                    szInfo = text.Length > 250 ? text.Substring(0, 250) : text,
                    szInfoTitle = title.Length > 60 ? title.Substring(0, 60) : title,
                    dwInfoFlags = NIIF_INFO,
                    uTimeoutOrVersion = 3000
                };

                Shell_NotifyIcon(NIM_MODIFY, ref nid);
            }
            catch { }
        }

        /// <summary>
        /// Oculta la ventana principal hacia el System Tray
        /// </summary>
        public void MinimizeToTray(Window window)
        {
            if (window == null) return;

            window.Hide();

            if (!_hasShownFirstMinimizeTip)
            {
                _hasShownFirstMinimizeTip = true;
                ShowNotification(
                    "Ghost Optimizer",
                    "Ghost Optimizer se está ejecutando en segundo plano.\nHaz clic en el icono para abrirlo.");
            }
        }

        /// <summary>
        /// Restaura la ventana principal al frente
        /// </summary>
        public void RestoreMainWindow()
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                var mainWindow = Application.Current.MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.Show();
                    if (mainWindow.WindowState == WindowState.Minimized)
                    {
                        mainWindow.WindowState = WindowState.Normal;
                    }
                    mainWindow.Activate();
                    mainWindow.Focus();
                }
            });
        }

        private void ExitApplication()
        {
            Dispose();
            Application.Current?.Dispatcher.Invoke(() =>
            {
                Application.Current.Shutdown();
            });
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;

            if (_isIconAdded && _hwndSource != null)
            {
                try
                {
                    var nid = new NOTIFYICONDATA
                    {
                        cbSize = Marshal.SizeOf(typeof(NOTIFYICONDATA)),
                        hWnd = _hwndSource.Handle,
                        uID = 1001
                    };
                    Shell_NotifyIcon(NIM_DELETE, ref nid);
                    _isIconAdded = false;
                }
                catch { }
            }

            if (_hwndSource != null)
            {
                _hwndSource.RemoveHook(WndProcHook);
                _hwndSource.Dispose();
                _hwndSource = null;
            }

            if (_hIcon != IntPtr.Zero)
            {
                try { DestroyIcon(_hIcon); } catch { }
                _hIcon = IntPtr.Zero;
            }
        }
    }
}
