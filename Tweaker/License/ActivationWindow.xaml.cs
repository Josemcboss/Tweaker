using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

using Microsoft.Extensions.DependencyInjection;

namespace Tweaker.License
{
    /// <summary>
    /// Ventana de activación de licencia
    /// </summary>
    public partial class ActivationWindow : Window
    {
        private string currentHardwareFingerprint;
        private readonly ILicenseValidator _validator;
        private readonly ILicenseStorage _storage;

        public bool ActivationSuccessful { get; private set; }

        public ActivationWindow()
        {
            InitializeComponent();
            _validator = App.ServiceProvider.GetRequiredService<ILicenseValidator>();
            _storage = App.ServiceProvider.GetRequiredService<ILicenseStorage>();

            try
            {
                currentHardwareFingerprint = HardwareFingerprint.GetFingerprint();
                TxtHardwareId.Text = currentHardwareFingerprint;
            }
            catch (Exception ex)
            {
                ShowStatus($"Error al obtener ID del hardware: {ex.Message}", false);
                currentHardwareFingerprint = string.Empty;
            }
        }

        private void TxtLicenseKey_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var text = TxtLicenseKey.Text.Replace("-", "").ToUpper();
            if (text.Length > 20) text = text.Substring(0, 20);
            var formatted = "";
            for (int i = 0; i < text.Length; i++)
            {
                if (i > 0 && i % 5 == 0) formatted += "-";
                formatted += text[i];
            }
            if (formatted != TxtLicenseKey.Text)
            {
                var cursorPos = TxtLicenseKey.SelectionStart;
                TxtLicenseKey.Text = formatted;
                TxtLicenseKey.SelectionStart = Math.Min(cursorPos + 1, formatted.Length);
            }
            BtnActivate.IsEnabled = _validator.IsValidFormat(TxtLicenseKey.Text);
            if (BorderStatus.Visibility == Visibility.Visible) BorderStatus.Visibility = Visibility.Collapsed;
        }

        private void BtnActivate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var licenseKey = TxtLicenseKey.Text.Trim();
                if (!_validator.IsValidFormat(licenseKey))
                {
                    ShowStatus("❌ Formato de llave inválido.", false);
                    return;
                }

                var licenseData = _validator.ValidateLicenseKey(licenseKey, currentHardwareFingerprint, DateTime.Now);
                if (licenseData == null)
                {
                    ShowStatus("❌ Llave de licencia inválida o no coincide.", false);
                    return;
                }

                if (licenseData.IsExpired)
                {
                    ShowStatus($"❌ La licencia ha expirado el {licenseData.ExpirationDate:yyyy-MM-dd}.", false);
                    return;
                }

                _storage.SaveLicense(licenseKey, licenseData);
                var expirationMsg = licenseData.IsPerpetual ? "Licencia Perpetua ♾️" : $"Válida hasta: {licenseData.ExpirationDate:yyyy-MM-dd}";
                MessageBox.Show($"✅ ¡Licencia activada exitosamente!\n\n{expirationMsg}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                ActivationSuccessful = true;
                DialogResult = true;
                Close();
            }
            catch (Exception ex) { ShowStatus($"❌ Error al activar: {ex.Message}", false); }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) { DialogResult = false; Close(); }

        private void BtnCopyFingerprint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(currentHardwareFingerprint))
                {
                    Clipboard.SetText(currentHardwareFingerprint);
                    ShowStatus("✅ Fingerprint copiado", true);
                }
            }
            catch (Exception ex) { ShowStatus($"❌ Error al copiar: {ex.Message}", false); }
        }

        private void ShowStatus(string message, bool isSuccess)
        {
            TxtStatus.Text = message;
            BorderStatus.Visibility = Visibility.Visible;
            var color = isSuccess ? Color.FromRgb(0, 150, 0) : Color.FromRgb(200, 0, 0);
            BorderStatus.BorderBrush = new SolidColorBrush(color);
            TxtStatus.Foreground = new SolidColorBrush(isSuccess ? Color.FromRgb(0, 200, 0) : Color.FromRgb(255, 100, 100));
        }
    }
}
