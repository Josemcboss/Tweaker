using System.Windows.Controls;
using Tweaker.ViewModels;

namespace Tweaker.Views
{
    public partial class GamingHubView : UserControl
    {
        public GamingHubView()
        {
            InitializeComponent();
            DataContext = new GamingHubViewModel();
        }
    }
}
