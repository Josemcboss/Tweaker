using System;

namespace Tweaker.License
{
    public interface ILicenseValidator
    {
        LicenseData? ValidateLicenseKey(string licenseKey, string currentHardwareFingerprint, DateTime currentTime, DateTime? knownCreatedDate = null);
        bool IsValidFormat(string? licenseKey);
    }
}



