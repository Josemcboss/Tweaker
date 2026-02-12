using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Tweaker.Models
{
    /// <summary>
    /// Modelo de datos para representar un Tweak individual
    /// Elimina hardcoding de información en XAML
    /// </summary>
    public class TweakModel : INotifyPropertyChanged
    {
        private string _title;
        private string _description;
        private bool _isRecommended;
        private bool _isEnabled;
        private string _tweakId;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

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

        public bool ShowInfoButton { get; set; }
        public bool UseApplyMode { get; set; }
        public string Category { get; set; }

        // Event handlers como Action para mejor performance
        public Action OnClickAction { get; set; }
        public Action OffClickAction { get; set; }
        public Action ApplyClickAction { get; set; }
        public Action InfoClickAction { get; set; }

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
    }

    /// <summary>
    /// Modelo para secciones de tweaks
    /// </summary>
    public class TweakSectionModel
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Icon { get; set; }
        public string Category { get; set; }
        public System.Collections.ObjectModel.ObservableCollection<TweakModel> Tweaks { get; set; }

        public TweakSectionModel()
        {
            Tweaks = new System.Collections.ObjectModel.ObservableCollection<TweakModel>();
        }
    }
}