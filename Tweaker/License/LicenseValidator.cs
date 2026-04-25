using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Tweaker.License
{
    /// <summary>
    /// Valida llaves de licencia
    /// </summary>
    public class LicenseValidator : ILicenseValidator
    {
        private readonly IKeyVault _keyVault;
        private readonly ISecurityChecks _securityChecks;

        public LicenseValidator(IKeyVault keyVault, ISecurityChecks securityChecks)
        {
            _keyVault = keyVault;
            _securityChecks = securityChecks;
        }

        public LicenseData? ValidateLicenseKey(string licenseKey, string currentHardwareFingerprint, DateTime currentTime, DateTime? knownCreatedDate = null)
        {
            try
            {
                if (_securityChecks.IsDebuggerAttached() || _securityChecks.IsAnalysisToolDetected())
                {
                    System.Diagnostics.Debug.WriteLine("🚨 SEGURIDAD: Herramienta de análisis o debugger detectado");
                    return null;
                }

                if (!IsValidFormat(licenseKey)) return null;

                var creationDatesToTry = new List<DateTime>();
                if (knownCreatedDate.HasValue) creationDatesToTry.Add(knownCreatedDate.Value);
                creationDatesToTry.Add(currentTime);

                var testLimits = new List<int> { -1 };
                for (int i = 1; i <= 100; i++) testLimits.Add(i);

                foreach (var createdDate in creationDatesToTry)
                {
                    foreach (var maxTweaks in testLimits)
                    {
                        var perpetualHash = GenerateLicenseHash(currentHardwareFingerprint, null, createdDate, maxTweaks);
                        if (licenseKey.Replace("-", "").ToUpper() == perpetualHash.ToUpper())
                        {
                            return new LicenseData { HardwareFingerprint = currentHardwareFingerprint, ExpirationDate = null, CreatedDate = createdDate, MaxTweaks = maxTweaks };
                        }
                    }

                    for (int years = 0; years <= 10; years++)
                    {
                        for (int months = 0; months < 12; months++)
                        {
                            var testDate = createdDate.AddYears(years).AddMonths(months);
                            foreach (var maxTweaks in testLimits)
                            {
                                var dateHash = GenerateLicenseHash(currentHardwareFingerprint, testDate, createdDate, maxTweaks);
                                if (licenseKey.Replace("-", "").ToUpper() == dateHash.ToUpper())
                                {
                                    return new LicenseData { HardwareFingerprint = currentHardwareFingerprint, ExpirationDate = testDate, CreatedDate = createdDate, MaxTweaks = maxTweaks };
                                }
                            }
                        }
                    }
                }

                return null;
            }
            catch (System.Exception ex) { System.Diagnostics.Debug.WriteLine($"[Error] Excepcion capturada: {ex.Message}"); return null; }
        }

        private string GenerateLicenseHash(string hardwareFingerprint, DateTime? expirationDate, DateTime createdDate, int maxTweaks)
        {
            string secretKey = _keyVault.GetMasterSecret();
            var licenseData = $"{hardwareFingerprint}|{expirationDate?.ToString("yyyy-MM-dd") ?? "PERPETUAL"}|{createdDate.ToString("yyyy-MM-dd")}|{maxTweaks}|{secretKey}";
            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(licenseData));
                return BitConverter.ToString(hashBytes, 0, 10).Replace("-", "");
            }
        }

        public bool IsValidFormat(string? licenseKey)
        {
            if (string.IsNullOrWhiteSpace(licenseKey)) return false;
            var pattern = @"^[A-Z0-9]{5}-[A-Z0-9]{5}-[A-Z0-9]{5}-[A-Z0-9]{5}$";
            return Regex.IsMatch(licenseKey.ToUpper(), pattern);
        }
    }
}
