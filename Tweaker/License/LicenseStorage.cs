using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Tweaker.License
{
    /// <summary>
    /// Almacena y recupera licencias de forma segura
    /// </summary>
    public static class LicenseStorage
    {
        private const string LICENSE_FILENAME = ".tweaker.lic";
        private static readonly string LicenseFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Tweaker",
            LICENSE_FILENAME
        );

        /// <summary>
        /// Guarda una licencia activada
        /// </summary>
        public static void SaveLicense(string licenseKey, LicenseData licenseData)
        {
            try
            {
                // Crear directorio si no existe
                var directory = Path.GetDirectoryName(LicenseFilePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Crear objeto para guardar
                var saveData = new
                {
                    Key = licenseKey,
                    Fingerprint = licenseData.HardwareFingerprint,
                    ExpirationDate = licenseData.ExpirationDate?.ToString("o"),
                    CreatedDate = licenseData.CreatedDate.ToString("o"),
                    Signature = GenerateSignature(licenseKey, licenseData.HardwareFingerprint)
                };

                // Serializar a JSON
                var json = JsonSerializer.Serialize(saveData, new JsonSerializerOptions
                {
                    WriteIndented = false
                });

                // Encriptar y guardar
                var encrypted = EncryptData(json);
                File.WriteAllText(LicenseFilePath, encrypted);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al guardar licencia: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Carga la licencia guardada
        /// </summary>
        public static (string? licenseKey, LicenseData? licenseData) LoadLicense()
        {
            try
            {
                if (!File.Exists(LicenseFilePath))
                {
                    return (null, null);
                }

                // Leer y desencriptar
                var encrypted = File.ReadAllText(LicenseFilePath);
                var json = DecryptData(encrypted);

                if (string.IsNullOrEmpty(json))
                {
                    return (null, null);
                }

                // Deserializar
                var saveData = JsonSerializer.Deserialize<SavedLicenseData>(json);
                if (saveData == null)
                {
                    return (null, null);
                }

                // Verificar firma
                var expectedSignature = GenerateSignature(saveData.Key, saveData.Fingerprint);
                if (saveData.Signature != expectedSignature)
                {
                    // Archivo ha sido modificado
                    return (null, null);
                }

                // Crear objeto LicenseData
                var licenseData = new LicenseData
                {
                    HardwareFingerprint = saveData.Fingerprint,
                    ExpirationDate = string.IsNullOrEmpty(saveData.ExpirationDate)
                        ? null
                        : DateTime.Parse(saveData.ExpirationDate),
                    CreatedDate = DateTime.Parse(saveData.CreatedDate)
                };

                return (saveData.Key, licenseData);
            }
            catch
            {
                return (null, null);
            }
        }

        /// <summary>
        /// Elimina la licencia guardada
        /// </summary>
        public static void DeleteLicense()
        {
            try
            {
                if (File.Exists(LicenseFilePath))
                {
                    File.Delete(LicenseFilePath);
                }
            }
            catch
            {
                // Ignorar errores al eliminar
            }
        }

        /// <summary>
        /// Verifica si existe una licencia guardada
        /// </summary>
        public static bool HasSavedLicense()
        {
            return File.Exists(LicenseFilePath);
        }

        /// <summary>
        /// Encripta datos
        /// </summary>
        private static string EncryptData(string data)
        {
            using (var aes = Aes.Create())
            {
                var key = DeriveKey("TweakerStorage2024");
                aes.Key = key;
                aes.GenerateIV();

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    var dataBytes = Encoding.UTF8.GetBytes(data);
                    var encryptedBytes = encryptor.TransformFinalBlock(dataBytes, 0, dataBytes.Length);

                    // Combinar IV + datos encriptados
                    var result = new byte[aes.IV.Length + encryptedBytes.Length];
                    Array.Copy(aes.IV, 0, result, 0, aes.IV.Length);
                    Array.Copy(encryptedBytes, 0, result, aes.IV.Length, encryptedBytes.Length);

                    return Convert.ToBase64String(result);
                }
            }
        }

        /// <summary>
        /// Desencripta datos
        /// </summary>
        private static string DecryptData(string encryptedData)
        {
            try
            {
                var allBytes = Convert.FromBase64String(encryptedData);

                using (var aes = Aes.Create())
                {
                    var key = DeriveKey("TweakerStorage2024");
                    aes.Key = key;

                    // Extraer IV
                    var iv = new byte[16];
                    Array.Copy(allBytes, 0, iv, 0, 16);
                    aes.IV = iv;

                    // Extraer datos encriptados
                    var encryptedBytes = new byte[allBytes.Length - 16];
                    Array.Copy(allBytes, 16, encryptedBytes, 0, encryptedBytes.Length);

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
        /// Deriva una clave de 256 bits desde una contraseña
        /// </summary>
        private static byte[] DeriveKey(string password)
        {
            using (var sha = SHA256.Create())
            {
                return sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        /// <summary>
        /// Genera una firma para detectar modificaciones
        /// </summary>
        private static string GenerateSignature(string licenseKey, string fingerprint)
        {
            using (var sha = SHA256.Create())
            {
                var data = $"{licenseKey}|{fingerprint}|TweakerSignature";
                var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(data));
                return Convert.ToBase64String(hash);
            }
        }

        /// <summary>
        /// Clase auxiliar para deserialización
        /// </summary>
        private class SavedLicenseData
        {
            public string Key { get; set; } = string.Empty;
            public string Fingerprint { get; set; } = string.Empty;
            public string ExpirationDate { get; set; } = string.Empty;
            public string CreatedDate { get; set; } = string.Empty;
            public string Signature { get; set; } = string.Empty;
        }
    }
}
