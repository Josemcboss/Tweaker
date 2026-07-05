using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Tweaker.Models
{
    /// <summary>
    /// Nivel de riesgo de un tweak
    /// </summary>
    public enum RiskLevel
    {
        /// <summary>
        /// Seguro - No afecta funcionalidad cr�tica del sistema
        /// </summary>
        Safe = 0,

        /// <summary>
        /// Moderado - Puede afectar algunas funcionalidades no cr�ticas
        /// </summary>
        Moderate = 1,

        /// <summary>
        /// Avanzado - Puede afectar funcionalidad cr�tica, solo para usuarios experimentados
        /// </summary>
        Advanced = 2
    }

    /// <summary>
    /// Modelo de datos para representar un Tweak individual
    /// Elimina hardcoding de informaci�n en XAML
    /// </summary>
    public class TweakModel : INotifyPropertyChanged
    {
        private string _title = string.Empty;
        private string _description = string.Empty;
        private bool _isRecommended;
        private bool _isEnabled;
        private string _tweakId = string.Empty;
        private RiskLevel _risk;
        private bool _requiresRestart;
        private int _fpsGain;
        private int _pingReduction;
        private double _ramFreedGB;
        private bool _isVisible = true;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string DisplayName => Title;

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public bool IsRecommended
        {
            get => _isRecommended;
            set => SetProperty(ref _isRecommended, value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        public string TweakId
        {
            get => _tweakId;
            set => SetProperty(ref _tweakId, value);
        }

        public string Id
        {
            get => TweakId;
            set => TweakId = value;
        }

        /// <summary>
        /// Nivel de riesgo del tweak
        /// </summary>
        public RiskLevel Risk
        {
            get => _risk;
            set => SetProperty(ref _risk, value);
        }

        public bool RequiresRestart
        {
            get => _requiresRestart;
            set => SetProperty(ref _requiresRestart, value);
        }

        public int FpsGain
        {
            get => _fpsGain;
            set => SetProperty(ref _fpsGain, value);
        }

        public int PingReduction
        {
            get => _pingReduction;
            set => SetProperty(ref _pingReduction, value);
        }

        public double RamFreedGB
        {
            get => _ramFreedGB;
            set => SetProperty(ref _ramFreedGB, value);
        }

        private bool _isDisabled;
        private string? _warningMessage;

        public bool IsDisabled
        {
            get => _isDisabled;
            set => SetProperty(ref _isDisabled, value);
        }

        public string? WarningMessage
        {
            get => _warningMessage;
            set => SetProperty(ref _warningMessage, value);
        }

        public bool IsVisible
        {
            get => _isVisible;
            set => SetProperty(ref _isVisible, value);
        }

        public bool ShowInfoButton { get; set; }
        public bool UseApplyMode { get; set; }

        public string? Category { get; set; }

        // Event handlers como Action para mejor performance
        public Action? OnClickAction { get; set; }
        public Action? OffClickAction { get; set; }
        public Action? ApplyClickAction { get; set; }
        public Action? InfoClickAction { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }

    /// <summary>
    /// Modelo para secciones de tweaks
    /// </summary>
    public class TweakSectionModel
    {
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public System.Collections.ObjectModel.ObservableCollection<TweakModel> Tweaks { get; set; }

        public TweakSectionModel()
        {
            Tweaks = new System.Collections.ObjectModel.ObservableCollection<TweakModel>();
        }
    }
}
