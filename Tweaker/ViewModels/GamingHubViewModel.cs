using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Tweaker.Optimizations;

namespace Tweaker.ViewModels
{
    public class GamingHubViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        private string _activePowerPlan = "Cargando...";
        private double _currentTimerResolution = 15.6;
        private string _statusMessage = "Listo";
        private string _cacheCleanResult = "Limpieza de shaders sin ejecutar.";
        private bool _isCleaning = false;

        public ObservableCollection<SystemMitigationItem> Mitigations { get; } = new();

        public string ActivePowerPlan
        {
            get => _activePowerPlan;
            set
            {
                if (_activePowerPlan != value)
                {
                    _activePowerPlan = value;
                    OnPropertyChanged();
                }
            }
        }

        public double CurrentTimerResolution
        {
            get => _currentTimerResolution;
            set
            {
                if (Math.Abs(_currentTimerResolution - value) > 0.001)
                {
                    _currentTimerResolution = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TimerResolutionDisplay));
                }
            }
        }

        public string TimerResolutionDisplay => $"{_currentTimerResolution:F3} ms";

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public string CacheCleanResult
        {
            get => _cacheCleanResult;
            set
            {
                if (_cacheCleanResult != value)
                {
                    _cacheCleanResult = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsCleaning
        {
            get => _isCleaning;
            set
            {
                if (_isCleaning != value)
                {
                    _isCleaning = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand SetTimerHalfMsCommand { get; }
        public ICommand RestoreTimerCommand { get; }
        public ICommand RefreshTimerCommand { get; }
        public ICommand SwitchPowerPlanCommand { get; }
        public ICommand CleanAllShadersCommand { get; }
        public ICommand RefreshMitigationsCommand { get; }

        public GamingHubViewModel()
        {
            SetTimerHalfMsCommand = new RelayCommand(_ =>
            {
                TimerResolutionOptimization.EnableHighPrecisionTimer();
                UpdateTimerResolution();
                StatusMessage = "Temporizador fijado a 0.500 ms (Mínima latencia).";
            });

            RestoreTimerCommand = new RelayCommand(_ =>
            {
                TimerResolutionOptimization.RestoreTimerResolution();
                UpdateTimerResolution();
                StatusMessage = "Temporizador restaurado al valor estándar de Windows.";
            });

            RefreshTimerCommand = new RelayCommand(_ => UpdateTimerResolution());

            SwitchPowerPlanCommand = new RelayCommand(p =>
            {
                string plan = p?.ToString() ?? "ultimate";
                StatusMessage = $"Cambiando plan de energía a {plan}...";
                Task.Run(() =>
                {
                    bool ok = PowerTweaks.SwitchPowerPlan(plan);
                    System.Windows.Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        ActivePowerPlan = PowerTweaks.GetActivePowerPlan();
                        StatusMessage = ok ? $"Plan de energía actualizado a: {ActivePowerPlan}" : "Error cambiando plan de energía.";
                    });
                });
            });

            CleanAllShadersCommand = new RelayCommand(async _ => await CleanShadersAsync());
            RefreshMitigationsCommand = new RelayCommand(_ => LoadMitigations());

            // Carga inicial
            Task.Run(() =>
            {
                UpdateTimerResolution();
                string plan = PowerTweaks.GetActivePowerPlan();
                System.Windows.Application.Current?.Dispatcher?.Invoke(() =>
                {
                    ActivePowerPlan = plan;
                });
                LoadMitigations();
            });
        }

        public void UpdateTimerResolution()
        {
            double res = TimerResolutionOptimization.GetCurrentResolutionMs();
            System.Windows.Application.Current?.Dispatcher?.Invoke(() =>
            {
                CurrentTimerResolution = res;
            });
        }

        public void LoadMitigations()
        {
            var vbs = SystemMitigationDiagnostics.CheckVbs();
            var hags = SystemMitigationDiagnostics.CheckHags();
            var gamebar = SystemMitigationDiagnostics.CheckGameBar();
            var spectre = SystemMitigationDiagnostics.CheckSpectreMeltdown();

            System.Windows.Application.Current?.Dispatcher?.Invoke(() =>
            {
                Mitigations.Clear();
                Mitigations.Add(vbs);
                Mitigations.Add(hags);
                Mitigations.Add(gamebar);
                Mitigations.Add(spectre);
            });
        }

        private async Task CleanShadersAsync()
        {
            IsCleaning = true;
            StatusMessage = "Limpiando cachés de DirectX, NVIDIA, AMD, Steam y Minecraft...";

            await Task.Run(() =>
            {
                var (success, bytesFreed, filesDeleted) = GameCacheOptimizer.CleanAllShaderCaches();
                long mb = bytesFreed / (1024 * 1024);

                System.Windows.Application.Current?.Dispatcher?.Invoke(() =>
                {
                    CacheCleanResult = $"✓ Limpieza completada: {filesDeleted} archivos eliminados ({mb} MB liberados).";
                    StatusMessage = CacheCleanResult;
                });
            });

            IsCleaning = false;
        }

        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
}

