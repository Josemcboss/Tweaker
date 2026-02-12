using System.Configuration;
using System.Data;
using System.Windows;
using Tweaker.License;

namespace Tweaker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Validar licencia al iniciar
            if (!LicenseManager.ValidateLicenseOnStartup())
            {
                // No hay licencia válida, mostrar ventana de activación
                LicenseManager.ShowActivationWindow(showCancelOption: false);
            }
        }
    }

}
