using System;
using System.Windows;

namespace Tweaker.License
{
    public class LicenseManager : ILicenseManager
    {
        private readonly ILicenseValidator _validator;
        private readonly ILicenseStorage _storage;
        private LicenseData? _currentLicense;
        private bool _isValidated = false;

        public LicenseManager(ILicenseValidator validator, ILicenseStorage storage)
        {
            _validator = validator;
            _storage = storage;
        }

        public bool IsLicenseValid => _isValidated && _currentLicense != null && _currentLicense.IsValid;
        public LicenseData? CurrentLicense => _currentLicense;

        public bool ValidateLicenseOnStartup()
        {
            try
            {
                var currentFingerprint = HardwareFingerprint.GetFingerprint();
                var (licenseKey, savedLicense) = _storage.LoadLicense();

                if (licenseKey == null || savedLicense == null) return false;
                if (savedLicense.HardwareFingerprint != currentFingerprint) return false;
                if (savedLicense.IsExpired) return false;

                var validatedLicense = _validator.ValidateLicenseKey(licenseKey, currentFingerprint, DateTime.Now, savedLicense.CreatedDate);
                if (validatedLicense == null) return false;

                _currentLicense = validatedLicense;
                _isValidated = true;
                return true;
            }
            catch { return false; }
        }

        public bool ShowActivationWindow(bool showCancelOption = true)
        {
            var activationWindow = new ActivationWindow(); 
            var result = activationWindow.ShowDialog();

            if (result == true && activationWindow.ActivationSuccessful)
            {
                return ValidateLicenseOnStartup();
            }

            if (!showCancelOption && result != true)
            {
                MessageBox.Show("Tweaker requiere una licencia válida.", "Licencia Requerida", MessageBoxButton.OK, MessageBoxImage.Warning);
                Application.Current.Shutdown();
            }

            return result == true;
        }

        public bool EnsureValidLicense() => IsLicenseValid || ShowActivationWindow(false);

        public void DeactivateLicense()
        {
            _storage.DeleteLicense();
            _currentLicense = null;
            _isValidated = false;
        }

        public string GetLicenseInfo()
        {
            if (!IsLicenseValid || _currentLicense == null) return "Sin licencia activa";
            var info = $"Licencia Activa\nID: {HardwareFingerprint.GetDisplayFingerprint()}\n";
            info += _currentLicense.IsPerpetual ? "Tipo: Perpetua ♾️" : $"Expira: {_currentLicense.ExpirationDate:yyyy-MM-dd}";
            return info;
        }
    }
}
