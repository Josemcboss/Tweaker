using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;

namespace Tweaker.Controls
{
    /// <summary>
    /// Ventana de Notificación Flotante In-Game / OSD con forma de píldora translúcida.
    /// Completamente no intrusiva: permite clics a través de ella y nunca roba el foco.
    /// </summary>
    public partial class InGameToastOverlay : Window
    {
        #region Win32 Click-Through and Non-Activating Window

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_NOACTIVATE = 0x08000000;
        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int WS_EX_TOOLWINDOW = 0x00000080;

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        #endregion

        public InGameToastOverlay()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var helper = new WindowInteropHelper(this);
                int exStyle = GetWindowLong(helper.Handle, GWL_EXSTYLE);
                // Hacer la ventana transparente a los clics del ratón y de tipo toolwindow para que no robe foco
                SetWindowLong(helper.Handle, GWL_EXSTYLE, exStyle | WS_EX_TRANSPARENT | WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW);
            }
            catch { }
        }

        /// <summary>
        /// Muestra el toast animado y lo cierra automáticamente tras el tiempo especificado.
        /// </summary>
        public async void ShowToast(string gameName, string details, bool isActive = true, int displayDurationMs = 3500)
        {
            TxtGameName.Text = gameName;
            TxtDetails.Text = details;

            if (isActive)
            {
                TxtIcon.Text = "🎮";
                TxtBadge.Text = "GAME MODE ACTIVO";
                RootBorder.BorderBrush = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#5865F2"));
            }
            else
            {
                TxtIcon.Text = "🛡️";
                TxtBadge.Text = "RESTABLECIDO";
                RootBorder.BorderBrush = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#00FF88"));
            }

            // Posicionar en la esquina superior derecha de la pantalla principal o activa
            var screenWidth = SystemParameters.PrimaryScreenWidth;
            Left = screenWidth - Width - 24;
            Top = 32;

            Show();

            if (Resources["ToastShowAnimation"] is Storyboard showAnim)
            {
                showAnim.Begin(this);
            }

            await Task.Delay(displayDurationMs);

            if (Resources["ToastHideAnimation"] is Storyboard hideAnim)
            {
                hideAnim.Completed += (s, e) =>
                {
                    try { Close(); } catch { }
                };
                hideAnim.Begin(this);
            }
            else
            {
                Close();
            }
        }
    }
}
