using System.Windows.Controls;
using Tweaker.ViewModels;

namespace Tweaker.Views
{
    public partial class CyberSecurityView : UserControl
    {
        public CyberSecurityView()
        {
            InitializeComponent();
            DataContext = new CyberSecurityViewModel();
        }
    }
}
