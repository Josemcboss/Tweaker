using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Tweaker.Controls
{
    /// <summary>
    /// UserControl para representar una sección completa de tweaks
    /// Reduce la duplicación de headers y estructura de páginas
    /// </summary>
    public partial class TweakSection : UserControl
    {
        public TweakSection()
        {
            InitializeComponent();
            TweakItems = new ObservableCollection<TweakItem>();
        }

        #region Dependency Properties

        public static readonly DependencyProperty SectionTitleProperty =
            DependencyProperty.Register("SectionTitle", typeof(string), typeof(TweakSection), new PropertyMetadata(""));

        public static readonly DependencyProperty SectionSubtitleProperty =
            DependencyProperty.Register("SectionSubtitle", typeof(string), typeof(TweakSection), new PropertyMetadata(""));

        public static readonly DependencyProperty SectionIconProperty =
            DependencyProperty.Register("SectionIcon", typeof(string), typeof(TweakSection), new PropertyMetadata(""));

        public static readonly DependencyProperty TweakItemsProperty =
            DependencyProperty.Register("TweakItems", typeof(ObservableCollection<TweakItem>), typeof(TweakSection), new PropertyMetadata(null));

        public static readonly DependencyProperty AdditionalContentProperty =
            DependencyProperty.Register("AdditionalContent", typeof(object), typeof(TweakSection), new PropertyMetadata(null));

        public string SectionTitle
        {
            get { return (string)GetValue(SectionTitleProperty); }
            set { SetValue(SectionTitleProperty, value); }
        }

        public string SectionSubtitle
        {
            get { return (string)GetValue(SectionSubtitleProperty); }
            set { SetValue(SectionSubtitleProperty, value); }
        }

        public string SectionIcon
        {
            get { return (string)GetValue(SectionIconProperty); }
            set { SetValue(SectionIconProperty, value); }
        }

        public ObservableCollection<TweakItem> TweakItems
        {
            get { return (ObservableCollection<TweakItem>)GetValue(TweakItemsProperty); }
            set { SetValue(TweakItemsProperty, value); }
        }

        public object AdditionalContent
        {
            get { return GetValue(AdditionalContentProperty); }
            set { SetValue(AdditionalContentProperty, value); }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Agrega un TweakItem a la sección
        /// </summary>
        public void AddTweakItem(TweakItem tweakItem)
        {
            TweakItems.Add(tweakItem);
        }

        /// <summary>
        /// Limpia todos los TweakItems de la sección
        /// </summary>
        public void ClearTweakItems()
        {
            TweakItems.Clear();
        }

        #endregion
    }
}