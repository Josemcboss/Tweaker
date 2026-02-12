using System;
using System.Security.Cryptography;
using System.Text;

namespace Tweaker.License
{
    /// <summary>
    /// Genera llaves de licencia válidas (herramienta administrativa)
    /// </summary>
    public static class LicenseKeyGenerator
    {
        // Clave secreta para encriptación AES (EN PRODUCCIÓN, USAR OFUSCACIÓN O KEY DERIVATION)
        private const string SECRET_KEY = "TweakerLic2024SecretKey9876543";
        
        /// <summary>
        /// Genera una llave de licencia válida
        /// </summary>
        /// <param name="hardwareFingerprint">Fingerprint del hardware del cliente</param>
        /// <param name="expirationDate">Fecha de expiración (null = perpetua)</param>
        /// <returns>Llave de licencia en formato XXXXX-XXXXX-XXXXX-XXXXX</returns>
        public static string GenerateKey(string hardwareFingerprint, DateTime? expirationDate = null)
        {
            try
            {
                // Crear datos de la licencia
                var licenseData = new StringBuilder();
                licenseData.Append(hardwareFingerprint);
                licenseData.Append("|");
                licenseData.Append(expirationDate?.ToString("yyyy-MM-dd") ?? "PERPETUAL");
                licenseData.Append("|");
                
                // Agregar fecha de creación
                licenseData.Append(DateTime.Now.ToString("yyyy-MM-dd"));

                // Encriptar con AES
                var encrypted = EncryptString(licenseData.ToString());
                
                // Calcular checksum
                var checksum = CalculateChecksum(encrypted);
                
                // Combinar encrypted + checksum
                var combined = encrypted + checksum.ToString("X4");
                
                // Formatear como XXXXX-XXXXX-XXXXX-XXXXX
                return FormatLicenseKey(combined);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al generar llave de licencia: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Encripta una cadena usando AES
        /// </summary>
        private static string EncryptString(string plainText)
        {
            using (var aes = Aes.Create())
            {
                // Derivar clave de 32 bytes (256 bits)
                var key = new byte[32];
                var keyBytes = Encoding.UTF8.GetBytes(SECRET_KEY);
                Array.Copy(keyBytes, key, Math.Min(keyBytes.Length, key.Length));
                aes.Key = key;
                
                // Generar IV determinista basado en el contenido
                var iv = new byte[16];
                using (var sha = SHA256.Create())
                {
                    var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(plainText + SECRET_KEY));
                    Array.Copy(hash, iv, 16);
                }
                aes.IV = iv;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    var plainBytes = Encoding.UTF8.GetBytes(plainText);
                    var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                    return Convert.ToBase64String(encryptedBytes).Replace("+", "").Replace("/", "").Replace("=", "");
                }
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

        /// <summary>
        /// Formatea una cadena en formato de llave XXXXX-XXXXX-XXXXX-XXXXX
        /// </summary>
        private static string FormatLicenseKey(string rawKey)
        {
            // Asegurar que tenemos suficientes caracteres
            if (rawKey.Length < 20)
            {
                rawKey = rawKey.PadRight(20, '0');
            }
            
            // Tomar solo los primeros 20 caracteres válidos
            var cleaned = new StringBuilder();
            foreach (char c in rawKey.ToUpper())
            {
                if (char.IsLetterOrDigit(c))
                {
                    cleaned.Append(c);
                    if (cleaned.Length >= 20)
                        break;
                }
            }

            if (cleaned.Length < 20)
            {
                cleaned.Append('0', 20 - cleaned.Length);
            }

            // Formatear con guiones
            return $"{cleaned.ToString().Substring(0, 5)}-{cleaned.ToString().Substring(5, 5)}-" +
                   $"{cleaned.ToString().Substring(10, 5)}-{cleaned.ToString().Substring(15, 5)}";
        }
    }
}
