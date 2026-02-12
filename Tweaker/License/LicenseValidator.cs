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

                // Limpiar formato
                var cleanKey = licenseKey.Replace("-", "");

                // Extraer checksum (últimos 4 caracteres)
                if (cleanKey.Length < 4)
                    return null;

                var checksumStr = cleanKey.Substring(cleanKey.Length - 4);
                var encrypted = cleanKey.Substring(0, cleanKey.Length - 4);

                // Verificar checksum
                var expectedChecksum = CalculateChecksum(encrypted);
                var actualChecksum = Convert.ToUInt16(checksumStr, 16);

                if (expectedChecksum != actualChecksum)
                {
                    return null;
                }

                // Desencriptar
                var decrypted = DecryptString(encrypted);
                if (string.IsNullOrEmpty(decrypted))
                {
                    return null;
                }

                // Parsear datos
                var parts = decrypted.Split('|');
                if (parts.Length < 3)
                {
                    return null;
                }

                var fingerprint = parts[0];
                var expirationStr = parts[1];
                var createdStr = parts[2];

                // Verificar que el fingerprint coincida
                if (fingerprint != currentHardwareFingerprint)
                {
                    return null;
                }

                // Parsear fecha de expiración
                DateTime? expirationDate = null;
                if (expirationStr != "PERPETUAL")
                {
                    if (DateTime.TryParse(expirationStr, out var expDate))
                    {
                        expirationDate = expDate;
                    }
                }

                // Parsear fecha de creación
                DateTime createdDate = DateTime.Now;
                if (DateTime.TryParse(createdStr, out var createDate))
                {
                    createdDate = createDate;
                }

                // Crear objeto de licencia
                var licenseData = new LicenseData
                {
                    HardwareFingerprint = fingerprint,
                    ExpirationDate = expirationDate,
                    CreatedDate = createdDate
                };

                // Verificar si está expirada
                if (licenseData.IsExpired)
                {
                    return null;
                }

                return licenseData;
            }
            catch
            {
                return null;
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

                    // Convertir base64 modificado de vuelta
                    var base64 = encryptedText;
                    // Agregar padding si es necesario
                    while (base64.Length % 4 != 0)
                    {
                        base64 += "=";
                    }

                    var encryptedBytes = Convert.FromBase64String(base64);

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
