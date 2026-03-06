using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

using Tweaker.Controls;
using Tweaker.Data;
using Tweaker.Factories;
using Tweaker.Models;
using Tweaker.Services;
using Tweaker.Utilities;

namespace Tweaker.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Fields

        private TweakSectionModel _currentSection;
        private string _currentPageName = "Dashboard";
        private readonly TweakStateManager _stateManager;
        private readonly GameBoosterService _gameBooster;
        private readonly SmartScanService _smartScanService;
        private readonly StartupManagerService _startupManagerService;
        private bool _isAutoBoosterEnabled = false;
        private string _currentStatus = "Modo: Escritorio";

        // Smart Scan fields
        private bool _isScanning = false;
        private bool _showScanResults = false;
        private int _scanScore = 0;
        private string _scanSummary = "";
        private bool _showOptimizeButton = false;
        private List<ScanResult> _lastScanResults;

        #endregion

        #region Properties

        public ObservableCollection<NavigationItem> NavigationItems { get; }
        public ObservableCollection<TweakSectionModel> AllSections { get; }
        public ObservableCollection<StartupItem> StartupItems { get; } = new ObservableCollection<StartupItem>();

        public TweakSectionModel CurrentSection
        {
            get => _currentSection;
            set
            {
                if (_currentSection != value)
                {
                    _currentSection = value;
                    OnPropertyChanged();
                    CurrentPageName = value?.SectionName ?? "Dashboard";
                }
            }
        }

        public string CurrentPageName
        {
            get => _currentPageName;
            set
            {
                if (_currentPageName != value)
                {
                    _currentPageName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string CurrentStatus
        {
            get => _currentStatus;
            set
            {
                if (_currentStatus != value)
                {
                    _currentStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        public int ActiveTweaksCount => _stateManager.ActiveTweaksCount;
        public int TotalTweaksCount => 58; // Total tweaks disponibles
        public double OptimizationPercentage => TotalTweaksCount > 0 ? (double)ActiveTweaksCount / TotalTweaksCount * 100 : 0;
        public int EstimatedFpsGain => CalculateEstimatedFpsGain();
        public double RamFreedGB => CalculateRamFreed();

        // Game Booster Properties
        public bool IsGameModeActive => _gameBooster.IsGameModeActive;
        public bool IsAutoBoosterEnabled
        {
            get => _isAutoBoosterEnabled;
            set
            {
                if (_isAutoBoosterEnabled != value)
                {
                    _isAutoBoosterEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        // Smart Scan Properties
        public bool IsScanning
        {
            get => _isScanning;
            set { _isScanning = value; OnPropertyChanged(); }
        }

        public bool ShowScanResults
        {
            get => _showScanResults;
            set { _showScanResults = value; OnPropertyChanged(); }
        }

        public int ScanScore
        {
            get => _scanScore;
            set { _scanScore = value; OnPropertyChanged(); }
        }

        public string ScanSummary
        {
            get => _scanSummary;
            set { _scanSummary = value; OnPropertyChanged(); }
        }

        public bool ShowOptimizeButton
        {
            get => _showOptimizeButton;
            set { _showOptimizeButton = value; OnPropertyChanged(); }
        }

        #endregion

        #region Commands

        public ICommand NavigateCommand { get; }
        public ICommand ApplyAllCommand { get; }
        public ICommand RevertAllCommand { get; }
        public IAsyncCommand StartScanCommand { get; }
        public IAsyncCommand OptimizeNowCommand { get; }
        public ICommand RefreshStartupCommand { get; }
        public ICommand DisableStartupCommand { get; }

        #endregion

        public MainWindowViewModel()
        {
            _stateManager = TweakStateManager.Instance;
            _stateManager.PropertyChanged += StateManager_PropertyChanged;

            // Inicializar Servicios
            _gameBooster = GameBoosterService.Instance;
            _gameBooster.GameModeChanged += GameBooster_GameModeChanged;
            _smartScanService = new SmartScanService();
            _startupManagerService = new StartupManagerService();

            // Inicializar colecciones
            NavigationItems = new ObservableCollection<NavigationItem>
            {
                new NavigationItem { Name = "Dashboard", Icon = "Home", Page = "Dashboard" },
                new NavigationItem { Name = "Input & Visuals", Icon = "Mouse", Page = "Input" },
                new NavigationItem { Name = "Red & Ping", Icon = "Network", Page = "Network" },
                new NavigationItem { Name = "Sistema & GPU", Icon = "Cpu", Page = "System" },
                new NavigationItem { Name = "Limpieza", Icon = "Trash", Page = "Cleanup" },
                new NavigationItem { Name = "GHOST Pack", Icon = "Ghost", Page = "Ghost" },
                new NavigationItem { Name = "Advanced", Icon = "Settings", Page = "Advanced" }
            };

            AllSections = new ObservableCollection<TweakSectionModel>(TweakFactory.GetAllSections());
            CurrentSection = AllSections.FirstOrDefault();

            // Comandos
            NavigateCommand = new RelayCommand(p => Navigate(p?.ToString()));
            ApplyAllCommand = new RelayCommand(_ => ApplyAllInCurrentSection());
            RevertAllCommand = new RelayCommand(_ => RevertAllInCurrentSection());

            StartScanCommand = new AsyncRelayCommand(StartScanAsync);
            OptimizeNowCommand = new AsyncRelayCommand(OptimizeNowAsync);
            RefreshStartupCommand = new RelayCommand(_ => RefreshStartupList());
            DisableStartupCommand = new RelayCommand(p => DisableStartupItem(p as StartupItem));

            // Cargar datos iniciales
            RefreshStartupList();
        }

        private async Task StartScanAsync()
        {
            try
            {
                IsScanning = true;
                ShowScanResults = false;

                _lastScanResults = await _smartScanService.ScanAsync();

                int total = _lastScanResults.Count;
                int optimized = _lastScanResults.Count(r => r.IsOptimized);
                ScanScore = total > 0 ? (int)((double)optimized / total * 100) : 100;

                int pending = total - optimized;
                if (pending > 0)
                {
                    ScanSummary = $"Se encontraron {pending} optimizaciones recomendadas para mejorar tu rendimiento.";
                    ShowOptimizeButton = true;
                }
                else
                {
                    ScanSummary = "¡Felicidades! Tu sistema ya está altamente optimizado.";
                    ShowOptimizeButton = false;
                }

                IsScanning = false;
                ShowScanResults = true;
            }
            catch (Exception ex)
            {
                IsScanning = false;
                // Log error
            }
        }

        private async Task OptimizeNowAsync()
        {
            if (_lastScanResults == null) return;

            try
            {
                var pendingResults = _lastScanResults.Where(r => !r.IsOptimized).ToList();
                if (pendingResults.Count == 0) return;

                ShowOptimizeButton = false;
                ScanSummary = "Aplicando optimizaciones seleccionadas...";

                var pendingIds = pendingResults.Select(r => r.TweakId).ToList();
                await _smartScanService.ApplyRecommendedAsync(pendingIds);

                foreach (var result in pendingResults)
                {
                    _stateManager.SetTweakEnabled(result.TweakId, result.Category);
                }

                ScanSummary = "Tu sistema ahora está totalmente optimizado.";
                ScanScore = 100;

                await Task.Delay(3000);
                ShowScanResults = false;
            }
            catch (Exception ex)
            {
                ShowOptimizeButton = true;
                // Log error
            }
        }

        private void RefreshStartupList()
        {
            try
            {
                var items = _startupManagerService.GetStartupItems();
                StartupItems.Clear();
                foreach (var item in items)
                {
                    StartupItems.Add(item);
                }
            }
            catch
            {
                // Log error
            }
        }

        private void DisableStartupItem(StartupItem item)
        {
            if (item == null) return;
            if (_startupManagerService.DisableItem(item))
            {
                RefreshStartupList();
            }
        }

        private void StateManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TweakStateManager.ActiveTweaksCount))
            {
                OnPropertyChanged(nameof(ActiveTweaksCount));
                OnPropertyChanged(nameof(OptimizationPercentage));
                OnPropertyChanged(nameof(EstimatedFpsGain));
                OnPropertyChanged(nameof(RamFreedGB));
            }
        }

        private void GameBooster_GameModeChanged(object sender, GameModeChangedEventArgs e)
        {
            OnPropertyChanged(nameof(IsGameModeActive));
            CurrentStatus = e.IsActive ? "Modo: Gaming (Optimizado)" : "Modo: Escritorio";
        }

        private void Navigate(string page)
        {
            if (string.IsNullOrEmpty(page)) return;
            var section = AllSections.FirstOrDefault(s => s.SectionName.Contains(page, StringComparison.OrdinalIgnoreCase));
            if (section != null)
            {
                CurrentSection = section;
            }
            else if (page == "Dashboard")
            {
                CurrentPageName = "Dashboard";
                CurrentSection = null;
            }
        }

        private void ApplyAllInCurrentSection()
        {
            if (CurrentSection == null) return;
            foreach (var tweak in CurrentSection.Tweaks)
            {
                _stateManager.SetTweakEnabled(tweak.Id, CurrentSection.SectionName);
            }
        }

        private void RevertAllInCurrentSection()
        {
            if (CurrentSection == null) return;
            foreach (var tweak in CurrentSection.Tweaks)
            {
                _stateManager.SetTweakDisabled(tweak.Id);
            }
        }

        private int CalculateEstimatedFpsGain()
        {
            return _stateManager.EstimatedFpsGain;
        }

        private double CalculateRamFreed()
        {
            return _stateManager.EstimatedRamFreed;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    public class NavigationItem
    {
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Page { get; set; } = string.Empty;
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Predicate<object?>? _canExecute;

        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute(parameter);

        public void Execute(object? parameter) => _execute(parameter);

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
