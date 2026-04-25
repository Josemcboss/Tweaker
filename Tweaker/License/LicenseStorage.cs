using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

using Microsoft.Win32;

namespace Tweaker.License
{
    public class LicenseStorage : ILicenseStorage
    {
        private const string BACKUP_REG_KEY = @"SOFTWARE\GhostOptimizer\License";
        private const string STORAGE_PWD = "TweakerStorageKey_v2";

        public void SaveLicense(string licenseKey, LicenseData licenseData)
        {
            try
            {
                var saveData = new SavedLicenseData
                {
                    Key = licenseKey,
                    Fingerprint = licenseData.HardwareFingerprint,
                    CreatedDate = licenseData.CreatedDate,
                    ExpirationDate = licenseData.ExpirationDate,
                    MaxTweaks = licenseData.MaxTweaks,
                    Signature = GenerateSignature(licenseKey, licenseData.HardwareFingerprint)
                };

                string json = JsonSerializer.Serialize(saveData);
                string encrypted = EncryptString(json);

                using (var key = Registry.CurrentUser.CreateSubKey(BACKUP_REG_KEY))
                {
                    key?.SetValue("Data", encrypted, RegistryValueKind.String);
                }
            }
            catch (System.Exception ex) { System.Diagnostics.Debug.WriteLine($"[Error] Excepcion capturada: {ex.Message}"); }
        }

        public (string? licenseKey, LicenseData? licenseData) LoadLicense()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(BACKUP_REG_KEY))
                {
                    var encrypted = key?.GetValue("Data") as string;
                    if (string.IsNullOrEmpty(encrypted)) return (null, null);

                    string json = DecryptString(encrypted);
                    var saveData = JsonSerializer.Deserialize<SavedLicenseData>(json);

                    if (saveData == null) return (null, null);

                    var expectedSignature = GenerateSignature(saveData.Key, saveData.Fingerprint);
                    if (saveData.Signature != expectedSignature) return (null, null);

                    var licenseData = new LicenseData
                    {
                        HardwareFingerprint = saveData.Fingerprint,
                        ExpirationDate = saveData.ExpirationDate,
                        CreatedDate = saveData.CreatedDate,
                        MaxTweaks = saveData.MaxTweaks
                    };

                    return (saveData.Key, licenseData);
                }
            }
            catch { return (null, null); }
        }

        public void DeleteLicense()
        {
            try { Registry.CurrentUser.DeleteSubKeyTree(BACKUP_REG_KEY, false); }
            catch (System.Exception ex) { System.Diagnostics.Debug.WriteLine($"[Error] Excepcion capturada: {ex.Message}"); }
        }

        private string EncryptString(string plainText)
        {
            using (var aes = Aes.Create())
            {
                var key = DeriveKey(STORAGE_PWD);
                aes.Key = key;
                aes.GenerateIV();
                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    var plainBytes = Encoding.UTF8.GetBytes(plainText);
                    var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                    var result = new byte[aes.IV.Length + encryptedBytes.Length];
                    Array.Copy(aes.IV, 0, result, 0, aes.IV.Length);
                    Array.Copy(encryptedBytes, 0, result, aes.IV.Length, encryptedBytes.Length);
                    return Convert.ToBase64String(result);
                }
            }
        }

        private string DecryptString(string encryptedText)
        {
            var allBytes = Convert.FromBase64String(encryptedText);
            using (var aes = Aes.Create())
            {
                var key = DeriveKey(STORAGE_PWD);
                aes.Key = key;
                var iv = new byte[16];
                Array.Copy(allBytes, 0, iv, 0, 16);
                aes.IV = iv;
                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    var decryptedBytes = decryptor.TransformFinalBlock(allBytes, 16, allBytes.Length - 16);
                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
        }

        private byte[] DeriveKey(string password)
        {
            using (var sha = SHA256.Create()) return sha.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private string GenerateSignature(string licenseKey, string fingerprint)
        {
            using (var sha = SHA256.Create())
            {
                var data = $"{licenseKey}|{fingerprint}|TweakerSignatureV2";
                return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(data)));
            }
        }

        private class SavedLicenseData
        {
            public string Key { get; set; } = string.Empty;
            public string Fingerprint { get; set; } = string.Empty;
            public DateTime CreatedDate { get; set; }
            public DateTime? ExpirationDate { get; set; }
            public int MaxTweaks { get; set; }
            public string Signature { get; set; } = string.Empty;
        }
    }
}
