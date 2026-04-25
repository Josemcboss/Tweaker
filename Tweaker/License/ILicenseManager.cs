using System;

namespace Tweaker.License
{
    public interface ILicenseManager
    {
        bool IsLicenseValid { get; }
        LicenseData? CurrentLicense { get; }
        bool ValidateLicenseOnStartup();
        bool ShowActivationWindow(bool showCancelOption = true);
        bool EnsureValidLicense();
        void DeactivateLicense();
        string GetLicenseInfo();
    }
}



