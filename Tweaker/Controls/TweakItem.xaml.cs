using System;
using System.Windows;
using System.Windows.Controls;

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

        #endregion

        #region Events

        public event RoutedEventHandler OnClicked;
        public event RoutedEventHandler OffClicked;
        public event RoutedEventHandler ApplyClicked;
        public event RoutedEventHandler InfoClicked;

        #endregion

        #region Event Handlers

        private void OnButton_Click(object sender, RoutedEventArgs e)
        {
            OnClicked?.Invoke(this, e);
        }

        private void OffButton_Click(object sender, RoutedEventArgs e)
        {
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

        #region Private Methods

        private void UpdateButtonVisibility()
        {
            if (UseApplyMode)
            {
                OnOffButtons.Visibility = Visibility.Collapsed;
                ApplyButton.Visibility = Visibility.Visible;
            }
            else
            {
                OnOffButtons.Visibility = Visibility.Visible;
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
        }

        #endregion
    }
}