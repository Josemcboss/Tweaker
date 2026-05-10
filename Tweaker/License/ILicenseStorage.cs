using System;

namespace Tweaker.License
{
    public interface ILicenseStorage
    {
        void SaveLicense(string licenseKey, LicenseData licenseData);
        (string? licenseKey, LicenseData? licenseData) LoadLicense();
        void DeleteLicense();
    }
}



