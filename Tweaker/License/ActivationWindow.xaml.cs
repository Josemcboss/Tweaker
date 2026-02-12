using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

namespace Tweaker.License
{
    /// <summary>
    /// Ventana de activación de licencia
    /// </summary>
    public partial class ActivationWindow : Window
    {
        private string currentHardwareFingerprint;
        
        public bool ActivationSuccessful { get; private set; }

        public ActivationWindow()
        {
            InitializeComponent();
            
            // Obtener y mostrar el fingerprint del hardware
            try
            {
                currentHardwareFingerprint = HardwareFingerprint.GetFingerprint();
                TxtHardwareId.Text = HardwareFingerprint.GetDisplayFingerprint();
            }
            catch (Exception ex)
            {
                ShowStatus($"Error al obtener ID del hardware: {ex.Message}", false);
                currentHardwareFingerprint = string.Empty;
            }
        }

        /// <summary>
        /// Maneja el cambio de texto en el campo de llave de licencia
        /// </summary>
        private void TxtLicenseKey_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Auto-formatear con guiones
            var text = TxtLicenseKey.Text.Replace("-", "").ToUpper();
            
            if (text.Length > 20)
            {
                text = text.Substring(0, 20);
            }

            var formatted = "";
            for (int i = 0; i < text.Length; i++)
            {
                if (i > 0 && i % 5 == 0)
                {
                    formatted += "-";
                }
                formatted += text[i];
            }

            if (formatted != TxtLicenseKey.Text)
            {
                var cursorPos = TxtLicenseKey.SelectionStart;
                TxtLicenseKey.Text = formatted;
                TxtLicenseKey.SelectionStart = Math.Min(cursorPos + (formatted.Length - TxtLicenseKey.Text.Length + formatted.Length), formatted.Length);
            }

            // Habilitar botón de activar si el formato es válido
            BtnActivate.IsEnabled = LicenseValidator.IsValidFormat(TxtLicenseKey.Text);
            
            // Limpiar mensaje de estado al editar
            if (BorderStatus.Visibility == Visibility.Visible)
            {
                BorderStatus.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Maneja el clic en el botón de activar
        /// </summary>
        private void BtnActivate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var licenseKey = TxtLicenseKey.Text.Trim();

                // Validar formato
                if (!LicenseValidator.IsValidFormat(licenseKey))
                {
                    ShowStatus("❌ Formato de llave inválido. Use: XXXXX-XXXXX-XXXXX-XXXXX", false);
                    return;
                }

                // Validar llave
                var licenseData = LicenseValidator.ValidateLicenseKey(licenseKey, currentHardwareFingerprint);

                if (licenseData == null)
                {
                    ShowStatus("❌ Llave de licencia inválida o no coincide con este hardware.", false);
                    return;
                }

                if (licenseData.IsExpired)
                {
                    ShowStatus($"❌ La licencia ha expirado el {licenseData.ExpirationDate:yyyy-MM-dd}.", false);
                    return;
                }

                // Guardar licencia
                LicenseStorage.SaveLicense(licenseKey, licenseData);

                // Mostrar mensaje de éxito
                var expirationMsg = licenseData.IsPerpetual 
                    ? "Licencia Perpetua ♾️" 
                    : $"Válida hasta: {licenseData.ExpirationDate:yyyy-MM-dd}";

                MessageBox.Show(
                    $"✅ ¡Licencia activada exitosamente!\n\n" +
                    $"{expirationMsg}\n\n" +
                    $"Tweaker está ahora activado en este equipo.",
                    "Activación Exitosa",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                ActivationSuccessful = true;
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                ShowStatus($"❌ Error al activar: {ex.Message}", false);
            }
        }

        /// <summary>
        /// Maneja el clic en el botón de cancelar
        /// </summary>
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// Muestra un mensaje de estado
        /// </summary>
        private void ShowStatus(string message, bool isSuccess)
        {
            TxtStatus.Text = message;
            BorderStatus.Visibility = Visibility.Visible;

            if (isSuccess)
            {
                BorderStatus.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 150, 0));
                TxtStatus.Foreground = new SolidColorBrush(Color.FromRgb(0, 200, 0));
            }
            else
            {
                BorderStatus.BorderBrush = new SolidColorBrush(Color.FromRgb(200, 0, 0));
                TxtStatus.Foreground = new SolidColorBrush(Color.FromRgb(255, 100, 100));
            }
        }
    }
}
