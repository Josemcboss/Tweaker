using System;
using System.Windows;
using System.Windows.Controls;
using Tweaker.Models;

namespace Tweaker.Controls
{
    /// <summary>
    /// UserControl reutilizable para representar un tweak individual
    /// Elimina la duplicación de código en MainWindow.xaml
    /// </summary>
    public partial class TweakItem : UserControl
    {
        public TweakItem()
        {
            InitializeComponent();
            Loaded += TweakItem_Loaded;
        }

        #region Dependency Properties

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(TweakItem), new PropertyMetadata(""));

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(TweakItem), new PropertyMetadata(""));

        public static readonly DependencyProperty IsRecommendedProperty =
            DependencyProperty.Register("IsRecommended", typeof(bool), typeof(TweakItem), new PropertyMetadata(true));

        public static readonly DependencyProperty ShowInfoButtonProperty =
            DependencyProperty.Register("ShowInfoButton", typeof(bool), typeof(TweakItem), new PropertyMetadata(false));

        public static readonly DependencyProperty TweakIdProperty =
            DependencyProperty.Register("TweakId", typeof(string), typeof(TweakItem), new PropertyMetadata(""));

        public static readonly DependencyProperty UseApplyModeProperty =
            DependencyProperty.Register("UseApplyMode", typeof(bool), typeof(TweakItem), new PropertyMetadata(false));

        public static readonly DependencyProperty RiskProperty =
            DependencyProperty.Register("Risk", typeof(RiskLevel), typeof(TweakItem), new PropertyMetadata(RiskLevel.Safe));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public bool IsRecommended
        {
            get { return (bool)GetValue(IsRecommendedProperty); }
            set { SetValue(IsRecommendedProperty, value); }
        }

        public bool ShowInfoButton
        {
            get { return (bool)GetValue(ShowInfoButtonProperty); }
            set { SetValue(ShowInfoButtonProperty, value); }
        }

        public string TweakId
        {
            get { return (string)GetValue(TweakIdProperty); }
            set { SetValue(TweakIdProperty, value); }
        }

        public bool UseApplyMode
        {
            get { return (bool)GetValue(UseApplyModeProperty); }
            set { 
                SetValue(UseApplyModeProperty, value);
                UpdateButtonVisibility();
            }
        }

        public RiskLevel Risk
        {
            get { return (RiskLevel)GetValue(RiskProperty); }
            set { SetValue(RiskProperty, value); }
        }

        #endregion

        #region Events

        public event RoutedEventHandler OnClicked;
        public event RoutedEventHandler OffClicked;
        public event RoutedEventHandler ApplyClicked;
        public event RoutedEventHandler InfoClicked;

        #endregion

        #region Event Handlers

        /// <summary>
        /// Se ejecuta cuando el control termina de cargar
        /// </summary>
        private void TweakItem_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshToggleState();
        }

        /// <summary>
        /// Maneja el evento Checked del ModernToggleSwitch
        /// </summary>
        private void TweakToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded) return;
            OnClicked?.Invoke(this, e);
        }

        /// <summary>
        /// Maneja el evento Unchecked del ModernToggleSwitch
        /// </summary>
        private void TweakToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded) return;
            OffClicked?.Invoke(this, e);
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyClicked?.Invoke(this, e);
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            InfoClicked?.Invoke(this, e);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Refresca el estado del toggle basado en TweakHelper
        /// </summary>
        public void RefreshToggleState()
        {
            if (string.IsNullOrEmpty(TweakId)) return;

            try
            {
                bool isActive = Utilities.TweakHelper.IsTweakActive(TweakId);
                
                TweakToggleSwitch.Checked -= TweakToggleSwitch_Checked;
                TweakToggleSwitch.Unchecked -= TweakToggleSwitch_Unchecked;

                TweakToggleSwitch.IsChecked = isActive;

                TweakToggleSwitch.Checked += TweakToggleSwitch_Checked;
                TweakToggleSwitch.Unchecked += TweakToggleSwitch_Unchecked;
            }
            catch { }
        }

        /// <summary>
        /// Establece el estado sin disparar eventos
        /// </summary>
        public void SetToggleState(bool isActive)
        {
            TweakToggleSwitch.Checked -= TweakToggleSwitch_Checked;
            TweakToggleSwitch.Unchecked -= TweakToggleSwitch_Unchecked;

            TweakToggleSwitch.IsChecked = isActive;

            TweakToggleSwitch.Checked += TweakToggleSwitch_Checked;
            TweakToggleSwitch.Unchecked += TweakToggleSwitch_Unchecked;
        }

        #endregion

        #region Private Methods

        private void UpdateButtonVisibility()
        {
            if (UseApplyMode)
            {
                TweakToggleSwitch.Visibility = Visibility.Collapsed;
                ApplyButton.Visibility = Visibility.Visible;
            }
            else
            {
                TweakToggleSwitch.Visibility = Visibility.Visible;
                ApplyButton.Visibility = Visibility.Collapsed;
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            
            if (e.Property == UseApplyModeProperty)
            {
                UpdateButtonVisibility();
            }
            else if (e.Property == TweakIdProperty && IsLoaded)
            {
                RefreshToggleState();
            }
        }

        #endregion
    }
}