using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

using Microsoft.Extensions.DependencyInjection;

using Tweaker.Controls;
using Tweaker.Data;
using Tweaker.Factories;
using Tweaker.Models;
using Tweaker.Services;
using Tweaker.Utilities;
using Tweaker.Views;

namespace Tweaker.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Fields

        private TweakSectionModel? _currentSection;
        private string _currentPageName = "Dashboard";
        private readonly TweakStateManager _stateManager;
        private readonly GameBoosterService _gameBooster;
        private readonly SmartScanService _smartScanService;
        private readonly StartupManagerService _startupManagerService;
        private readonly IProfileManager _profileManager;
        private bool _isAutoBoosterEnabled = false;
        private string _currentStatus = "Modo: Escritorio";
        private Tweaker.Utilities.HardwareInfo _hardwareInfo = new();
        private ObservableCollection<HistoryEntry> _recentHistory = new();
        private ObservableCollection<TweakerProfile> _savedProfiles = new();

        // Smart Scan fields
        private bool _isScanning = false;
        private bool _showScanResults = false;
        private int _scanScore = 0;
        private string _scanSummary = "";
        private bool _showOptimizeButton = false;
        private List<ScanResult>? _lastScanResults;

        // Phase 4: Auto-Maintenance & Performance Metrics
        private readonly AutoMaintenanceService _autoMaintenance;
        private readonly PerformanceMetricsService _performanceMetrics;
        private string _maintenanceStatusText = "Inactivo";
        private string _performanceSummaryText = "";

        // System Info Modal fields
        private bool _isSystemInfoOpen = false;
        private string _selectedSystemTab = "CPU";

        // System Corruption Check fields
        private string _chkDskStatus = "Pending";
        private string _sfcStatus = "Pending";
        private string _dismStatus = "Pending";
        private bool _isCorruptionCheckRunning = false;

        #endregion

        #region Properties

        public ObservableCollection<NavigationItem> NavigationItems { get; }
        public ObservableCollection<TweakSectionModel> AllSections { get; }
        public ObservableCollection<StartupItem> StartupItems { get; } = new ObservableCollection<StartupItem>();

        public ObservableCollection<HistoryEntry> RecentHistory
        {
            get => _recentHistory;
            set { _recentHistory = value; OnPropertyChanged(); }
        }

        public ObservableCollection<TweakerProfile> SavedProfiles
        {
            get => _savedProfiles;
            set { _savedProfiles = value; OnPropertyChanged(); }
        }

        public TweakSectionModel? CurrentSection
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

        public HardwareMonitorService HardwareMonitor => _stateManager.HardwareMonitor;

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

        public Tweaker.Utilities.HardwareInfo HardwareInfo
        {
            get => _hardwareInfo;
            private set
            {
                _hardwareInfo = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HardwareSummary));
                OnPropertyChanged(nameof(DetectedAntiCheatsSummary));
                OnPropertyChanged(nameof(DetectedAntiCheatNames));
                OnPropertyChanged(nameof(HasDetectedAntiCheats));
            }
        }

        public string HardwareSummary =>
            $"{HardwareInfo.GPUName} | {HardwareInfo.CPUName} | {HardwareInfo.TotalRAMGB}GB RAM | {(HardwareInfo.IsSSD ? "SSD" : "HDD")} | {HardwareInfo.WindowsVersion}";

        public string DetectedAntiCheatsSummary => HardwareInfo.DetectedAntiCheats.Count > 0
            ? string.Join(", ", HardwareInfo.DetectedAntiCheats)
            : "Ninguno";

        public bool HasDetectedAntiCheats => HardwareInfo.DetectedAntiCheats.Count > 0;

        public string DetectedAntiCheatNames => HardwareInfo.DetectedAntiCheats.Count > 0
            ? string.Join(", ", HardwareInfo.DetectedAntiCheats)
            : string.Empty;

        public int ActiveTweaksCount => _stateManager.ActiveTweaksCount;
        public int ActiveTweakCount => ActiveTweaksCount;
        public int TotalTweaksCount => AllSections.Sum(s => s.Tweaks.Count);
        public int TotalTweakCount => TotalTweaksCount;
        public double OptimizationPercentage => TotalTweaksCount > 0 ? (double)ActiveTweaksCount / TotalTweaksCount * 100 : 0;
        public int EstimatedFpsGain => CalculateEstimatedFpsGain();
        public int TotalFpsGain => EstimatedFpsGain;
        public int EstimatedPingReduction => CalculateEstimatedLatencyReduction();
        public int TotalPingReduction => EstimatedPingReduction;
        public double RamFreedGB => CalculateRamFreed();
        public double TotalRamFreed => RamFreedGB;
        public int PendingRestartCount => GetPendingRestartTweaks().Count;
        public ObservableCollection<TweakModel> PendingRestartTweaks => new(GetPendingRestartTweaks());

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

        // Phase 4: Auto-Maintenance Properties
        public AutoMaintenanceService AutoMaintenance => _autoMaintenance;
        public PerformanceMetricsService PerformanceMetrics => _performanceMetrics;

        public string MaintenanceStatusText
        {
            get => _maintenanceStatusText;
            set { if (_maintenanceStatusText != value) { _maintenanceStatusText = value; OnPropertyChanged(); } }
        }

        public string PerformanceSummaryText
        {
            get => _performanceSummaryText;
            set { if (_performanceSummaryText != value) { _performanceSummaryText = value; OnPropertyChanged(); } }
        }

        // System Info properties
        public bool IsSystemInfoOpen
        {
            get => _isSystemInfoOpen;
            set { _isSystemInfoOpen = value; OnPropertyChanged(); }
        }

        public string SelectedSystemTab
        {
            get => _selectedSystemTab;
            set { _selectedSystemTab = value; OnPropertyChanged(); }
        }

        // System Corruption Check properties
        public string ChkDskStatus
        {
            get => _chkDskStatus;
            set { _chkDskStatus = value; OnPropertyChanged(); }
        }

        public string SfcStatus
        {
            get => _sfcStatus;
            set { _sfcStatus = value; OnPropertyChanged(); }
        }

        public string DismStatus
        {
            get => _dismStatus;
            set { _dismStatus = value; OnPropertyChanged(); }
        }

        public bool IsCorruptionCheckRunning
        {
            get => _isCorruptionCheckRunning;
            set { _isCorruptionCheckRunning = value; OnPropertyChanged(); }
        }

        #endregion

        #region Commands

        public ICommand OpenSystemInfoCommand { get; }
        public ICommand CloseSystemInfoCommand { get; }
        public ICommand SelectSystemTabCommand { get; }
        public IAsyncCommand BeginSystemCheckCommand { get; }

        public ICommand NavigateCommand { get; }
        public ICommand ApplyAllCommand { get; }
        public ICommand RevertAllCommand { get; }
        public IAsyncCommand StartScanCommand { get; }
        public IAsyncCommand OptimizeNowCommand { get; }
        public ICommand RefreshStartupCommand { get; }
        public ICommand DisableStartupCommand { get; }
        public ICommand SaveProfileCommand { get; }
        public ICommand LoadProfileCommand { get; }
        public ICommand DeleteProfileCommand { get; }
        public ICommand ExportProfileCommand { get; }
        public ICommand ImportProfileCommand { get; }
        public ICommand LoadPresetCommand { get; }
        public ICommand ExportLogCommand { get; }
        public ICommand ClearHistoryCommand { get; }
        public ICommand RollbackTweakCommand { get; }

        // Phase 4 Commands
        public IAsyncCommand RunMaintenanceNowCommand { get; }
        public ICommand TakeBaselineSnapshotCommand { get; }
        public ICommand TakeOptimizedSnapshotCommand { get; }
        public ICommand ClearMetricsHistoryCommand { get; }

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
            _profileManager = App.ServiceProvider.GetRequiredService<IProfileManager>();

            // Phase 4: Servicios avanzados
            _autoMaintenance = AutoMaintenanceService.Instance;
            _autoMaintenance.MaintenanceCompleted += AutoMaintenance_Completed;
            _performanceMetrics = PerformanceMetricsService.Instance;

            // Inicializar colecciones
            NavigationItems = new ObservableCollection<NavigationItem>
            {
                new NavigationItem { Name = "Dashboard", Icon = "Home", Page = "Dashboard" },
                new NavigationItem { Name = "Input & Visuals", Icon = "Mouse", Page = "Input" },
                new NavigationItem { Name = "Red & Ping", Icon = "Network", Page = "Network" },
                new NavigationItem { Name = "Sistema & GPU", Icon = "Cpu", Page = "System" },
                new NavigationItem { Name = "GPU & Display", Icon = "Display", Page = "GPUDisplay" },
                new NavigationItem { Name = "Almacenamiento", Icon = "Drive", Page = "Storage" },
                new NavigationItem { Name = "CPU Avanzado", Icon = "Chip", Page = "CPUAdvanced" },
                new NavigationItem { Name = "Limpieza", Icon = "Trash", Page = "Cleanup" },
                new NavigationItem { Name = "GHOST Pack", Icon = "Ghost", Page = "Ghost" },
                new NavigationItem { Name = "Advanced", Icon = "Settings", Page = "Advanced" },
                new NavigationItem { Name = "Perfiles", Icon = "Folder", Page = "Profiles" },
                new NavigationItem { Name = "Historial", Icon = "History", Page = "History" }
            };

            AllSections = new ObservableCollection<TweakSectionModel>(TweakFactory.GetAllSections());
            CurrentSection = AllSections.FirstOrDefault();
            HardwareInfo = HardwareDetector.Detect();
            RecentHistory = LoadHistoryEntries();
            SavedProfiles = new ObservableCollection<TweakerProfile>(_profileManager.GetAllProfiles());

            // Comandos
            OpenSystemInfoCommand = new RelayCommand(_ => { IsSystemInfoOpen = true; SelectedSystemTab = "CPU"; });
            CloseSystemInfoCommand = new RelayCommand(_ => IsSystemInfoOpen = false);
            SelectSystemTabCommand = new RelayCommand(p => { if (p != null) SelectedSystemTab = p.ToString(); });
            BeginSystemCheckCommand = new AsyncRelayCommand(ExecuteSystemCheckAsync);

            NavigateCommand = new RelayCommand(p => Navigate(p?.ToString()));
            ApplyAllCommand = new RelayCommand(_ => ApplyAllInCurrentSection());
            RevertAllCommand = new RelayCommand(_ => RevertAllInCurrentSection());
            StartScanCommand = new AsyncRelayCommand(StartScanAsync);
            OptimizeNowCommand = new AsyncRelayCommand(OptimizeNowAsync);
            RefreshStartupCommand = new RelayCommand(_ => RefreshStartupList());
            DisableStartupCommand = new RelayCommand(p => DisableStartupItem(p as StartupItem));
            SaveProfileCommand = new RelayCommand(_ => SaveCurrentProfile());
            LoadProfileCommand = new RelayCommand(p => LoadProfile(p as TweakerProfile));
            DeleteProfileCommand = new RelayCommand(p => DeleteProfile(p as TweakerProfile));
            ExportProfileCommand = new RelayCommand(p => ExportProfile(p as TweakerProfile));
            ImportProfileCommand = new RelayCommand(_ => ImportProfile());
            LoadPresetCommand = new RelayCommand(p => LoadPreset(p?.ToString()));
            ExportLogCommand = new RelayCommand(_ => ExportLog());
            ClearHistoryCommand = new RelayCommand(_ => ClearHistory());
            RollbackTweakCommand = new RelayCommand(p => RollbackTweak(p?.ToString()));

            // Phase 4 Commands
            RunMaintenanceNowCommand = new AsyncRelayCommand(async () => await _autoMaintenance.RunNowAsync());
            TakeBaselineSnapshotCommand = new RelayCommand(_ => _performanceMetrics.TakeBaselineSnapshot());
            TakeOptimizedSnapshotCommand = new RelayCommand(_ => _performanceMetrics.TakeOptimizedSnapshot());
            ClearMetricsHistoryCommand = new RelayCommand(_ => _performanceMetrics.ClearHistory());

            // Cargar datos iniciales
            RefreshStartupList();
        }

        private void AutoMaintenance_Completed(object? sender, Models.MaintenanceLog log)
        {
            MaintenanceStatusText = log.Success
                ? $"✅ {log.MBFreed}MB liberados, {log.FilesDeleted} archivos - {log.Timestamp:HH:mm}"
                : $"❌ Error: {log.Details}";
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
            catch (Exception)
            {
                IsScanning = false;
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
            catch (Exception)
            {
                ShowOptimizeButton = true;
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

        private void StateManager_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TweakStateManager.ActiveTweaksCount) ||
                e.PropertyName == nameof(TweakStateManager.TweaksByCategory) ||
                e.PropertyName == nameof(TweakStateManager.OptimizationPercentage) ||
                e.PropertyName == nameof(TweakStateManager.EstimatedFpsGain) ||
                e.PropertyName == nameof(TweakStateManager.EstimatedLatencyReduction) ||
                e.PropertyName == nameof(TweakStateManager.EstimatedRamFreed))
            {
                OnPropertyChanged(nameof(ActiveTweaksCount));
                OnPropertyChanged(nameof(ActiveTweakCount));
                OnPropertyChanged(nameof(TotalTweaksCount));
                OnPropertyChanged(nameof(TotalTweakCount));
                OnPropertyChanged(nameof(OptimizationPercentage));
                OnPropertyChanged(nameof(EstimatedFpsGain));
                OnPropertyChanged(nameof(TotalFpsGain));
                OnPropertyChanged(nameof(EstimatedPingReduction));
                OnPropertyChanged(nameof(TotalPingReduction));
                OnPropertyChanged(nameof(RamFreedGB));
                OnPropertyChanged(nameof(TotalRamFreed));
                OnPropertyChanged(nameof(PendingRestartCount));
                OnPropertyChanged(nameof(PendingRestartTweaks));
            }
        }

        private void GameBooster_GameModeChanged(object? sender, GameModeChangedEventArgs e)
        {
            OnPropertyChanged(nameof(IsGameModeActive));
            CurrentStatus = e.IsActive ? "Modo: Gaming (Optimizado)" : "Modo: Escritorio";
        }

        private void Navigate(string page)
        {
            if (string.IsNullOrEmpty(page)) return;

            if (page is "Dashboard" or "Profiles" or "History" or "Startup")
            {
                CurrentSection = null;
                CurrentPageName = page;
                if (page == "History")
                {
                    LoadRecentHistory();
                }
                else if (page == "Startup")
                {
                    RefreshStartupList();
                }
                return;
            }

            var section = AllSections.FirstOrDefault(s => 
                s.Category.Equals(page, StringComparison.OrdinalIgnoreCase) ||
                s.SectionName.Contains(page, StringComparison.OrdinalIgnoreCase) ||
                s.Title.Contains(page, StringComparison.OrdinalIgnoreCase));
            if (section != null)
            {
                CurrentSection = section;
                CurrentPageName = section.SectionName;
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
            return GetActiveTweakModels().Sum(t => t.FpsGain);
        }

        private int CalculateEstimatedLatencyReduction()
        {
            return GetActiveTweakModels().Sum(t => t.PingReduction);
        }

        private double CalculateRamFreed()
        {
            return Math.Round(GetActiveTweakModels().Sum(t => t.RamFreedGB), 1);
        }

        private List<TweakModel> GetActiveTweakModels()
        {
            return AllSections
                .SelectMany(s => s.Tweaks)
                .Where(t => _stateManager.IsTweakEnabled(t.TweakId))
                .ToList();
        }

        private List<TweakModel> GetPendingRestartTweaks()
        {
            return AllSections
                .SelectMany(s => s.Tweaks)
                .Where(t => t.RequiresRestart && _stateManager.IsTweakEnabled(t.TweakId))
                .ToList();
        }

        private ObservableCollection<HistoryEntry> LoadHistoryEntries()
        {
            var entries = new ObservableCollection<HistoryEntry>();
            foreach (string line in TweakHistoryLogger.GetRecent(50))
            {
                if (TryParseHistoryLine(line, out HistoryEntry? entry) && entry != null)
                {
                    entries.Add(entry);
                }
            }

            return entries;
        }

        public void LoadRecentHistory()
        {
            RecentHistory = LoadHistoryEntries();
        }

        private static bool TryParseHistoryLine(string line, out HistoryEntry? entry)
        {
            entry = null;
            try
            {
                int firstClose = line.IndexOf('>');
                int secondOpen = line.IndexOf('[', firstClose + 1);
                int secondClose = line.IndexOf(']', secondOpen + 1);
                if (firstClose < 0 || secondOpen < 0 || secondClose < 0)
                {
                    return false;
                }

                string timestampText = line[1..firstClose];
                string action = line[(secondOpen + 1)..secondClose];
                string remainder = line[(secondClose + 2)..];
                string tweakName = remainder;
                string category = string.Empty;
                int separator = remainder.LastIndexOf(" - ", StringComparison.Ordinal);
                if (separator >= 0)
                {
                    tweakName = remainder[..separator];
                    category = remainder[(separator + 3)..];
                }

                if (!DateTime.TryParse(timestampText, out DateTime timestamp))
                {
                    timestamp = DateTime.Now;
                }

                entry = new HistoryEntry
                {
                    Timestamp = timestamp,
                    Action = action,
                    TweakName = tweakName,
                    Category = category
                };
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void SaveCurrentProfile()
        {
            var dialog = new InputDialog
            {
                Owner = Application.Current?.MainWindow
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            var enabledIds = AllSections
                .SelectMany(s => s.Tweaks)
                .Where(t => _stateManager.IsTweakEnabled(t.TweakId))
                .Select(t => t.Id)
                .ToList();

            var profile = new TweakerProfile
            {
                Name = dialog.ProfileName,
                Description = dialog.Description,
                Created = DateTime.Now,
                Tweaks = enabledIds
            };

            _profileManager.SaveProfile(profile);
            SavedProfiles = new ObservableCollection<TweakerProfile>(_profileManager.GetAllProfiles());
        }

        private void LoadProfile(TweakerProfile? profile)
        {
            if (profile == null) return;
            var result = MessageBox.Show(
                $"¿Cargar perfil '{profile.Name}'?\nSe activarán {profile.TweakIds.Count} tweaks.",
                "Cargar perfil", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            var tweakIds = profile.TweakIds.ToHashSet(StringComparer.OrdinalIgnoreCase);
            foreach (var section in AllSections)
            {
                foreach (var tweak in section.Tweaks)
                {
                    if (tweakIds.Contains(tweak.Id))
                    {
                        _stateManager.SetTweakEnabled(tweak.Id, section.SectionName);
                    }
                    else if (_stateManager.IsTweakEnabled(tweak.Id))
                    {
                        _stateManager.SetTweakDisabled(tweak.Id);
                    }
                }
            }
        }

        private void DeleteProfile(TweakerProfile? profile)
        {
            if (profile == null) return;
            _profileManager.DeleteProfile(profile.Name);
            SavedProfiles.Remove(profile);
        }

        private void ExportProfile(TweakerProfile? profile)
        {
            if (profile == null) return;

            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = $"{profile.Name}.json",
                Filter = "JSON files (*.json)|*.json"
            };

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
                    string json = System.Text.Json.JsonSerializer.Serialize(profile, options);
                    System.IO.File.WriteAllText(dlg.FileName, json);

                    MessageBox.Show(
                        Tweaker.Utilities.LocalizationService.Text("Perfil exportado exitosamente.", "Profile exported successfully."),
                        Tweaker.Utilities.LocalizationService.Text("Éxito", "Success"),
                        MessageBoxButton.OK, MessageBoxImage.Information
                    );
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        Tweaker.Utilities.LocalizationService.Text($"Error al exportar perfil: {ex.Message}", $"Error exporting profile: {ex.Message}"),
                        Tweaker.Utilities.LocalizationService.Text("Error", "Error"),
                        MessageBoxButton.OK, MessageBoxImage.Error
                    );
                }
            }
        }

        private void ImportProfile()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json"
            };

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    string json = System.IO.File.ReadAllText(dlg.FileName);
                    var profile = System.Text.Json.JsonSerializer.Deserialize<TweakerProfile>(json, new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (profile == null || string.IsNullOrWhiteSpace(profile.Name))
                    {
                        MessageBox.Show(
                            Tweaker.Utilities.LocalizationService.Text("El archivo de perfil no es válido.", "Invalid profile file."),
                            Tweaker.Utilities.LocalizationService.Text("Error", "Error"),
                            MessageBoxButton.OK, MessageBoxImage.Error
                        );
                        return;
                    }

                    // Guardar en el gestor de perfiles
                    _profileManager.SaveProfile(profile);
                    
                    // Actualizar lista
                    SavedProfiles = new ObservableCollection<TweakerProfile>(_profileManager.GetAllProfiles());

                    MessageBox.Show(
                        Tweaker.Utilities.LocalizationService.Text($"Perfil '{profile.Name}' importado exitosamente.", $"Profile '{profile.Name}' imported successfully."),
                        Tweaker.Utilities.LocalizationService.Text("Éxito", "Success"),
                        MessageBoxButton.OK, MessageBoxImage.Information
                    );
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        Tweaker.Utilities.LocalizationService.Text($"Error al importar perfil: {ex.Message}", $"Error importing profile: {ex.Message}"),
                        Tweaker.Utilities.LocalizationService.Text("Error", "Error"),
                        MessageBoxButton.OK, MessageBoxImage.Error
                    );
                }
            }
        }

        private void LoadPreset(string? presetName)
        {
            if (string.IsNullOrWhiteSpace(presetName)) return;
            var preset = _profileManager.GetBuiltInProfiles().FirstOrDefault(p => p.Name.Equals(presetName, StringComparison.OrdinalIgnoreCase));
            if (preset == null) return;

            var presetIds = preset.Tweaks.ToHashSet(StringComparer.OrdinalIgnoreCase);
            foreach (var section in AllSections)
            {
                foreach (var tweak in section.Tweaks)
                {
                    if (presetIds.Contains(tweak.Id))
                    {
                        _stateManager.SetTweakEnabled(tweak.Id, section.SectionName);
                    }
                    else if (_stateManager.IsTweakEnabled(tweak.Id))
                    {
                        _stateManager.SetTweakDisabled(tweak.Id);
                    }
                }
            }
        }

        private void ExportLog()
        {
            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "ghost_optimizer_log.txt",
                Filter = "Text files|*.txt"
            };

            if (dlg.ShowDialog() == true)
            {
                string exportPath = TweakHistoryLogger.ExportToDesktop();
                if (!string.IsNullOrWhiteSpace(exportPath))
                {
                    try
                    {
                        System.IO.File.Copy(exportPath, dlg.FileName, true);
                    }
                    catch
                    {
                    }
                }
            }
        }

        private void ClearHistory()
        {
            var r = MessageBox.Show("¿Limpiar todo el historial?", "Confirmar",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (r == MessageBoxResult.Yes)
            {
                TweakHistoryLogger.Clear();
                RecentHistory.Clear();
            }
        }

        private void RollbackTweak(string? tweakId)
        {
            if (string.IsNullOrEmpty(tweakId)) return;
            var result = MessageBox.Show($"¿Deseas deshacer la optimización '{tweakId}'?", "Deshacer cambio",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                _stateManager.SetTweakDisabled(tweakId);
                LoadRecentHistory();
                MessageBox.Show($"Cambio revertido para '{tweakId}'.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async Task ExecuteSystemCheckAsync()
        {
            if (IsCorruptionCheckRunning) return;

            IsCorruptionCheckRunning = true;
            
            // Step 1: ChkDsk
            ChkDskStatus = "Running";
            bool chkResult = await RunBackgroundSystemCommandAsync("chkdsk.exe", "c:");
            ChkDskStatus = chkResult ? "Success" : "Failed";

            // Step 2: SFC
            SfcStatus = "Running";
            bool sfcResult = await RunBackgroundSystemCommandAsync("sfc.exe", "/verifyonly");
            SfcStatus = sfcResult ? "Success" : "Failed";

            // Step 3: DISM
            DismStatus = "Running";
            bool dismResult = await RunBackgroundSystemCommandAsync("dism.exe", "/online /cleanup-image /checkhealth");
            DismStatus = dismResult ? "Success" : "Failed";

            IsCorruptionCheckRunning = false;
        }

        private Task<bool> RunBackgroundSystemCommandAsync(string fileName, string arguments)
        {
            var tcs = new TaskCompletionSource<bool>();

            Task.Run(() =>
            {
                try
                {
                    string fullPath = GetSystem32Path(fileName);
                    var psi = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = fullPath,
                        Arguments = arguments,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true
                    };

                    using var process = System.Diagnostics.Process.Start(psi);
                    if (process == null)
                    {
                        tcs.SetResult(false);
                        return;
                    }

                    process.WaitForExit();
                    // sfc /verifyonly returns 0 if no corruption, but can return 1 or other codes.
                    // For safety, let's treat any clean exit (0 or 1/2 for check success) as completed.
                    tcs.SetResult(process.ExitCode == 0 || process.ExitCode == 1);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error running {fileName}: {ex.Message}");
                    tcs.SetResult(false);
                }
            });

            return tcs.Task;
        }

        private string GetSystem32Path(string exeName)
        {
            string system32 = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32");
            string sysnative = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Sysnative");
            
            string path = System.IO.Path.Combine(system32, exeName);
            if (System.IO.Directory.Exists(sysnative))
            {
                string nativePath = System.IO.Path.Combine(sysnative, exeName);
                if (System.IO.File.Exists(nativePath))
                {
                    return nativePath;
                }
            }
            return path;
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
