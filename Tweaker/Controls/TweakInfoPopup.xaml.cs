using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Tweaker.Controls
{
    /// <summary>
    /// TweakInfoPopup - Control para mostrar información detallada de cada tweak
    /// </summary>
    public partial class TweakInfoPopup : UserControl
    {
        public TweakInfoPopup()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(TweakInfoPopup), new PropertyMetadata(""));

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(TweakInfoPopup), new PropertyMetadata(""));

        public static readonly DependencyProperty BenefitsProperty =
            DependencyProperty.Register("Benefits", typeof(string), typeof(TweakInfoPopup), new PropertyMetadata(""));

        public static readonly DependencyProperty WarningsProperty =
            DependencyProperty.Register("Warnings", typeof(string), typeof(TweakInfoPopup), new PropertyMetadata(""));

        public static readonly DependencyProperty RecommendedProperty =
            DependencyProperty.Register("Recommended", typeof(bool), typeof(TweakInfoPopup), new PropertyMetadata(true));

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

        public string Benefits
        {
            get { return (string)GetValue(BenefitsProperty); }
            set { SetValue(BenefitsProperty, value); }
        }

        public string Warnings
        {
            get { return (string)GetValue(WarningsProperty); }
            set { SetValue(WarningsProperty, value); }
        }

        public bool Recommended
        {
            get { return (bool)GetValue(RecommendedProperty); }
            set { SetValue(RecommendedProperty, value); }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (Parent is Popup popup)
            {
                popup.IsOpen = false;
            }
        }
    }
}
