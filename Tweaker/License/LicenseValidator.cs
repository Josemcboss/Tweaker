using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Tweaker.License
{
    /// <summary>
    /// Valida llaves de licencia
    /// </summary>
    public static class LicenseValidator
    {
        // Misma clave secreta que en el generador
        private const string SECRET_KEY = "TweakerLic2024SecretKey9876543";

        /// <summary>
        /// Valida una llave de licencia
        /// </summary>
        /// <param name="licenseKey">Llave de licencia a validar</param>
        /// <param name="currentHardwareFingerprint">Fingerprint del hardware actual</param>
        /// <returns>LicenseData si es válida, null si no es válida</returns>
        public static LicenseData? ValidateLicenseKey(string licenseKey, string currentHardwareFingerprint)
        {
            try
            {
                // Verificar formato
                if (!IsValidFormat(licenseKey))
                {
                    return null;
                }

                // NUEVO ENFOQUE: Generar hashes para perpetua y con fechas, y comparar
                
                // 1. Probar licencia perpetua
                var perpetualHash = GenerateLicenseHash(currentHardwareFingerprint, null);
                if (licenseKey.Replace("-", "").ToUpper() == perpetualHash.ToUpper())
                {
                    return new LicenseData
                    {
                        HardwareFingerprint = currentHardwareFingerprint,
                        ExpirationDate = null,
                        CreatedDate = DateTime.Now
                    };
                }

                // 2. Probar con fechas de expiración (próximos 10 años)
                for (int years = 0; years <= 10; years++)
                {
                    for (int months = 0; months < 12; months++)
                    {
                        var testDate = DateTime.Now.AddYears(years).AddMonths(months);
                        var dateHash = GenerateLicenseHash(currentHardwareFingerprint, testDate);
                        
                        if (licenseKey.Replace("-", "").ToUpper() == dateHash.ToUpper())
                        {
                            return new LicenseData
                            {
                                HardwareFingerprint = currentHardwareFingerprint,
                                ExpirationDate = testDate,
                                CreatedDate = DateTime.Now
                            };
                        }
                    }
                }

                // No se encontró coincidencia
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Genera el hash de una licencia (debe coincidir con el generador)
        /// </summary>
        private static string GenerateLicenseHash(string hardwareFingerprint, DateTime? expirationDate)
        {
            var licenseData = new StringBuilder();
            licenseData.Append(hardwareFingerprint);
            licenseData.Append("|");
            licenseData.Append(expirationDate?.ToString("yyyy-MM-dd") ?? "PERPETUAL");
            licenseData.Append("|");
            licenseData.Append(SECRET_KEY);
            
            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(licenseData.ToString()));
                return BitConverter.ToString(hashBytes, 0, 10).Replace("-", "");
            }
        }

        /// <summary>
        /// Verifica que el formato de la llave sea válido
        /// </summary>
        public static bool IsValidFormat(string licenseKey)
        {
            if (string.IsNullOrWhiteSpace(licenseKey))
                return false;

            // Formato: XXXXX-XXXXX-XXXXX-XXXXX (20 caracteres + 3 guiones)
            var pattern = @"^[A-Z0-9]{5}-[A-Z0-9]{5}-[A-Z0-9]{5}-[A-Z0-9]{5}$";
            return Regex.IsMatch(licenseKey.ToUpper(), pattern);
        }

        /// <summary>
        /// Desencripta una cadena usando AES
        /// </summary>
        private static string DecryptString(string encryptedText)
        {
            try
            {
                using (var aes = Aes.Create())
                {
                    // Derivar clave de 32 bytes (256 bits)
                    var key = new byte[32];
                    var keyBytes = Encoding.UTF8.GetBytes(SECRET_KEY);
                    Array.Copy(keyBytes, key, Math.Min(keyBytes.Length, key.Length));
                    aes.Key = key;

                    // Convertir HEX a bytes
                    var encryptedBytes = new byte[encryptedText.Length / 2];
                    for (int i = 0; i < encryptedBytes.Length; i++)
                    {
                        encryptedBytes[i] = Convert.ToByte(encryptedText.Substring(i * 2, 2), 16);
                    }

                    // Generar IV determinista (mismo que en el generador)
                    var iv = new byte[16];
                    using (var sha = SHA256.Create())
                    {
                        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(SECRET_KEY + "IV_SALT"));
                        Array.Copy(hash, iv, 16);
                    }
                    aes.IV = iv;

                    using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    {
                        var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                        return Encoding.UTF8.GetString(decryptedBytes);
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Calcula un checksum de 16 bits
        /// </summary>
        private static ushort CalculateChecksum(string data)
        {
            ushort checksum = 0;
            foreach (char c in data)
            {
                checksum = (ushort)((checksum << 1) ^ c);
            }
            return checksum;
        }
    }
}
