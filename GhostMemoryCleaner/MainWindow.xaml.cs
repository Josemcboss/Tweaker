using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace GhostMemoryCleaner
{
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer _monitorTimer;
        private readonly DispatcherTimer _autoCleanTimer;
        private bool _isCleaning = false;

        public MainWindow()
        {
            InitializeComponent();

            // Timer de monitorización en tiempo real (cada 1.5s)
            _monitorTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1500)
            };
            _monitorTimer.Tick += (s, e) => UpdateMemoryStats();
            _monitorTimer.Start();

            // Timer de auto-limpieza (cada 15 minutos)
            _autoCleanTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(15)
            };
            _autoCleanTimer.Tick += async (s, e) =>
            {
                var stats = MemoryEngine.GetMemoryStats();
                if (stats.PercentUsed >= 80 && !_isCleaning)
                {
                    await PerformCleanAsync(silent: true);
                }
            };

            UpdateMemoryStats();
        }

        private void UpdateMemoryStats()
        {
            try
            {
                var stats = MemoryEngine.GetMemoryStats();
                
                PbRamUsage.Value = stats.PercentUsed;
                TxtRamPercent.Text = $"{stats.PercentUsed}%";

                double usedGb = stats.UsedMB / 1024.0;
                double availGb = stats.AvailableMB / 1024.0;
                double totalGb = stats.TotalMB / 1024.0;

                TxtUsedMem.Text = $"{usedGb:F1} GB";
                TxtAvailMem.Text = $"{availGb:F1} GB";
                TxtTotalMem.Text = $"{totalGb:F1} GB";

                if (stats.PercentUsed > 85)
                {
                    TxtRamPercent.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3B30"));
                }
                else if (stats.PercentUsed > 70)
                {
                    TxtRamPercent.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF9500"));
                }
                else
                {
                    TxtRamPercent.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00D9FF"));
                }
            }
            catch { }
        }

        private async void BtnCleanAll_Click(object sender, RoutedEventArgs e)
        {
            await PerformCleanAsync(silent: false);
        }

        private async void BtnPurgeStandby_Click(object sender, RoutedEventArgs e)
        {
            if (_isCleaning) return;
            _isCleaning = true;
            BtnCleanAll.IsEnabled = false;
            TxtStatus.Text = "Purgando Standby List...";
            TxtStatus.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00D9FF"));

            await Task.Run(() =>
            {
                MemoryEngine.PurgeStandbyList();
                System.Threading.Thread.Sleep(300);
            });

            UpdateMemoryStats();
            TxtStatus.Text = "✅ Standby List purgada con éxito.";
            TxtStatus.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00FF88"));
            BtnCleanAll.IsEnabled = true;
            _isCleaning = false;
        }

        private async void BtnTrimWorkingSets_Click(object sender, RoutedEventArgs e)
        {
            if (_isCleaning) return;
            _isCleaning = true;
            BtnCleanAll.IsEnabled = false;
            TxtStatus.Text = "Vaciando Working Sets de procesos...";
            TxtStatus.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00D9FF"));

            long freedMB = 0;
            int count = 0;

            await Task.Run(() =>
            {
                var res = MemoryEngine.CleanAll();
                freedMB = res.mbCleaned;
                count = res.processesProcessed;
            });

            UpdateMemoryStats();
            TxtStatus.Text = $"✅ Procesos optimizados ({count} procesos). ~{freedMB} MB liberados.";
            TxtStatus.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00FF88"));
            BtnCleanAll.IsEnabled = true;
            _isCleaning = false;
        }

        private async Task PerformCleanAsync(bool silent)
        {
            if (_isCleaning) return;
            _isCleaning = true;
            BtnCleanAll.IsEnabled = false;

            if (!silent)
            {
                TxtStatus.Text = "Liberando memoria y purfando caché de sistema...";
                TxtStatus.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00D9FF"));
            }

            long freedMB = 0;
            int procs = 0;

            await Task.Run(() =>
            {
                var result = MemoryEngine.CleanAll();
                freedMB = result.mbCleaned;
                procs = result.processesProcessed;
            });

            UpdateMemoryStats();

            if (freedMB > 0)
            {
                if (freedMB >= 1024)
                {
                    double freedGb = freedMB / 1024.0;
                    TxtStatus.Text = $"🚀 ¡{freedGb:F2} GB liberados! Standby list y {procs} procesos optimizados.";
                }
                else
                {
                    TxtStatus.Text = $"🚀 ¡{freedMB} MB liberados! Standby list y {procs} procesos optimizados.";
                }
                TxtStatus.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00FF88"));
            }
            else
            {
                TxtStatus.Text = "✅ Memoria optimizada. La caché ya se encontraba limpia.";
                TxtStatus.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00FF88"));
            }

            BtnCleanAll.IsEnabled = true;
            _isCleaning = false;
        }

        private void ChkAutoClean_Changed(object sender, RoutedEventArgs e)
        {
            if (ChkAutoClean.IsChecked == true)
            {
                _autoCleanTimer.Start();
                TxtStatus.Text = "🔄 Auto-limpieza en segundo plano activada.";
                TxtStatus.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00D9FF"));
            }
            else
            {
                _autoCleanTimer.Stop();
                TxtStatus.Text = "Auto-limpieza desactivada.";
            }
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
