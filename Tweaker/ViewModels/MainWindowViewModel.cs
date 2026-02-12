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
using Tweaker.Utilities;

namespace Tweaker.ViewModels
{
    /// <summary>
    /// ViewModel principal optimizado para MainWindow
    /// Maneja la navegación y estado de tweaks de forma eficiente
    /// </summary>
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Fields

        private TweakSectionModel _currentSection;
        private string _currentPageName = "Dashboard";
        private readonly TweakStateManager _stateManager;

        #endregion

        #region Properties

        public ObservableCollection<NavigationItem> NavigationItems { get; }
        public ObservableCollection<TweakSectionModel> AllSections { get; }

        public TweakSectionModel CurrentSection
        {
            get => _currentSection;
            set => SetProperty(ref _currentSection, value);
        }

        public string CurrentPageName
        {
            get => _currentPageName;
            set => SetProperty(ref _currentPageName, value);
        }

        // Dashboard Statistics
        public int ActiveTweaksCount => _stateManager?.GetActiveTweaksCount() ?? 0;
        public int TotalTweaksCount => 58; // Total tweaks disponibles
        public double OptimizationPercentage => TotalTweaksCount > 0 ? (double)ActiveTweaksCount / TotalTweaksCount * 100 : 0;
        public int EstimatedFpsGain => CalculateEstimatedFpsGain();
        public double RamFreedGB => CalculateRamFreed();

        #endregion

        #region Constructor

        public MainWindowViewModel()
        {
            _stateManager = TweakStateManager.Instance;
            _stateManager.PropertyChanged += StateManager_PropertyChanged;

            // Inicializar colecciones
            NavigationItems = new ObservableCollection<NavigationItem>();
            AllSections = new ObservableCollection<TweakSectionModel>();

            // Cargar datos
            InitializeNavigationItems();
            InitializeSections();
            
            // Comandos
            NavigateCommand = new RelayCommand<string>(NavigateToSection);
        }

        #endregion

        #region Commands

        public ICommand NavigateCommand { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Navega a una sección específica
        /// </summary>
        public void NavigateToSection(string sectionName)
        {
            CurrentPageName = sectionName;
            
            if (sectionName == "Dashboard")
            {
                CurrentSection = null;
            }
            else
            {
                CurrentSection = AllSections.FirstOrDefault(s => s.Category == sectionName);
            }

            // Actualizar estado activo de navegación
            foreach (var item in NavigationItems)
            {
                item.IsActive = item.Name == sectionName;
            }

            OnPropertyChanged(nameof(NavigationItems));
        }

        /// <summary>
        /// Actualiza las estadísticas del dashboard
        /// </summary>
        public void UpdateDashboardStats()
        {
            OnPropertyChanged(nameof(ActiveTweaksCount));
            OnPropertyChanged(nameof(OptimizationPercentage));
            OnPropertyChanged(nameof(EstimatedFpsGain));
            OnPropertyChanged(nameof(RamFreedGB));
        }

        #endregion

        #region Private Methods

        private void InitializeNavigationItems()
        {
            NavigationItems.Add(new NavigationItem("??", "Dashboard", "Dashboard", true));
            NavigationItems.Add(new NavigationItem("??", "Input & Visuals", "Input"));
            NavigationItems.Add(new NavigationItem("??", "Red & Ping", "Network"));
            NavigationItems.Add(new NavigationItem("??", "Sistema & GPU", "System"));
            NavigationItems.Add(new NavigationItem("??", "Limpieza", "Cleanup"));
            NavigationItems.Add(new NavigationItem("??", "GHOST Pack", "Ghost"));
            NavigationItems.Add(new NavigationItem("??", "Advanced", "Advanced"));
            NavigationItems.Add(new NavigationItem("??", "Preset Laptop", "Laptop"));
            NavigationItems.Add(new NavigationItem("??", "Competitive Gaming", "Competitive"));
        }

        private void InitializeSections()
        {
            var sections = TweakFactory.GetAllSections();
            foreach (var section in sections)
            {
                AllSections.Add(section);
                
                // Configurar event handlers para cada tweak
                foreach (var tweak in section.Tweaks)
                {
                    ConfigureTweakHandlers(tweak);
                }
            }
        }

        private void ConfigureTweakHandlers(TweakModel tweak)
        {
            tweak.OnClickAction = () => HandleTweakOn(tweak.TweakId);
            tweak.OffClickAction = () => HandleTweakOff(tweak.TweakId);
            tweak.ApplyClickAction = () => HandleTweakApply(tweak.TweakId);
            tweak.InfoClickAction = () => HandleTweakInfo(tweak.TweakId);
        }

        private void HandleTweakOn(string tweakId)
        {
            // Implementar lógica de activación del tweak
            // Esto se conectará con los métodos existentes en MainWindow
        }

        private void HandleTweakOff(string tweakId)
        {
            // Implementar lógica de desactivación del tweak
        }

        private void HandleTweakApply(string tweakId)
        {
            // Implementar lógica de aplicación del tweak
        }

        private void HandleTweakInfo(string tweakId)
        {
            // Mostrar popup de información usando TweaksDatabase
            var tweakInfo = TweaksDatabase.GetTweakInfo(tweakId);
            // Implementar show popup
        }

        private int CalculateEstimatedFpsGain()
        {
            // Lógica simple para calcular ganancia estimada de FPS
            var baseFps = 5; // FPS base por optimizaciones básicas
            var multiplier = Math.Min(ActiveTweaksCount * 0.5, 25); // Máximo 25 FPS
            return (int)(baseFps + multiplier);
        }

        private double CalculateRamFreed()
        {
            // Cálculo estimado de RAM liberada
            var baseRam = 0.1; // 100MB base
            var perTweak = 0.05; // 50MB por tweak promedio
            return Math.Round(baseRam + (ActiveTweaksCount * perTweak), 1);
        }

        private void StateManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Actualizar estadísticas cuando cambie el estado
            UpdateDashboardStats();
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion
    }

    /// <summary>
    /// Modelo para items de navegación
    /// </summary>
    public class NavigationItem
    {
        public string Icon { get; set; }
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public NavigationItem(string icon, string displayName, string name, bool isActive = false)
        {
            Icon = icon;
            DisplayName = displayName;
            Name = name;
            IsActive = isActive;
        }
    }

    /// <summary>
    /// Comando simple para navegación
    /// </summary>
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;

        public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke((T)parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}