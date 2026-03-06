using System;
using System.Security.Cryptography;
using System.Text;

namespace Tweaker.License
{
    /// <summary>
    /// Genera llaves de licencia válidas (herramienta administrativa)
    /// </summary>
    public class LicenseKeyGenerator
    {
        private readonly IKeyVault _keyVault;

        public LicenseKeyGenerator(IKeyVault keyVault)
        {
            _keyVault = keyVault;
        }

        // SEGURIDAD: La clave secreta se obtiene dinámicamente desde KeyVault
        // Esto dificulta la ingeniería inversa al no tener la clave completa en el código

        /// <summary>
        /// Genera una llave de licencia válida
        /// </summary>
        /// <param name="hardwareFingerprint">Fingerprint del hardware del cliente</param>
        /// <param name="expirationDate">Fecha de expiración (null = perpetua)</param>
        /// <param name="maxTweaks">Límite máximo de tweaks activos (-1 = ilimitado)</param>
        /// <returns>Llave de licencia en formato XXXXX-XXXXX-XXXXX-XXXXX</returns>
        public string GenerateKey(string hardwareFingerprint, DateTime? expirationDate = null, int maxTweaks = -1)
        {
            try
            {
                // SEGURIDAD NIVEL 1: Verificar anti-debugging (solo en Release)
#if !DEBUG
                if (!AntiDebugger.IsDevelopmentEnvironment() && AntiDebugger.CheckDebuggerThrottled())
                {
                    System.Diagnostics.Debug.WriteLine("🚨 SEGURIDAD: Debugger detectado en KeyGenerator");
                    throw new InvalidOperationException("Security violation detected");
                }
#endif

                // SEGURIDAD NIVEL 2: Verificar herramientas de análisis (solo en Release)
#if !DEBUG
                if (!AnalysisToolDetector.IsDevelopmentEnvironment() && AnalysisToolDetector.PerformFullCheck())
                {
                    System.Diagnostics.Debug.WriteLine("🚨 SEGURIDAD: Herramienta de análisis detectada en KeyGenerator");
                    throw new InvalidOperationException("Security violation detected");
                }
#endif

                // Obtener clave secreta desde KeyVault (seguridad por oscuridad)
                string secretKey = _keyVault.GetMasterSecret();

                // NUEVO ENFOQUE SIMPLE: Usar hash directo sin encriptación compleja


                // Crear datos de la licencia
                var licenseData = new StringBuilder();
                licenseData.Append(hardwareFingerprint);
                licenseData.Append("|");
                licenseData.Append(expirationDate?.ToString("yyyy-MM-dd") ?? "PERPETUAL");
                licenseData.Append("|");
                licenseData.Append(DateTime.Now.ToString("yyyy-MM-dd")); // Fecha de creación
                licenseData.Append("|");
                licenseData.Append(maxTweaks); // Límite de tweaks
                licenseData.Append("|");
                licenseData.Append(secretKey); // Incluir clave secreta en el hash

                // Calcular hash SHA256
                using (var sha256 = SHA256.Create())
                {
                    var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(licenseData.ToString()));

                    // Tomar los primeros 10 bytes (20 caracteres hex)
                    var hashHex = BitConverter.ToString(hashBytes, 0, 10).Replace("-", "");

                    // Formatear como XXXXX-XXXXX-XXXXX-XXXXX
                    return FormatLicenseKey(hashHex);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al generar llave de licencia: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Encripta una cadena usando AES
        /// </summary>
        private string EncryptString(string plainText)
        {
            // Obtener clave secreta desde KeyVault (seguridad por oscuridad)
            string secretKey = _keyVault.GetMasterSecret();

            using (var aes = Aes.Create())
            {
                // Derivar clave de 32 bytes (256 bits)
                var key = new byte[32];
                var keyBytes = Encoding.UTF8.GetBytes(secretKey);
                Array.Copy(keyBytes, key, Math.Min(keyBytes.Length, key.Length));
                aes.Key = key;

                // Generar IV determinista basado en la clave secreta
                var iv = new byte[16];
                using (var sha = SHA256.Create())
                {
                    var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(secretKey + "IV_SALT"));
                    Array.Copy(hash, iv, 16);
                }
                aes.IV = iv;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    var plainBytes = Encoding.UTF8.GetBytes(plainText);
                    var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                    // Convertir a HEX en lugar de Base64 para evitar problemas con caracteres especiales
                    return BitConverter.ToString(encryptedBytes).Replace("-", "");
                }
            }
        }

        /// <summary>
        /// Calcula un checksum de 16 bits
        /// </summary>
        private ushort CalculateChecksum(string data)
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
        private string FormatLicenseKey(string rawKey)
        {
            // Asegurar que tenemos exactamente 20 caracteres
            if (rawKey.Length > 20)
            {
                rawKey = rawKey.Substring(0, 20);
            }
            else if (rawKey.Length < 20)
            {
                rawKey = rawKey.PadRight(20, '0');
            }

            // Convertir a mayúsculas
            rawKey = rawKey.ToUpper();

            // Formatear con guiones: XXXXX-XXXXX-XXXXX-XXXXX
            return $"{rawKey.Substring(0, 5)}-{rawKey.Substring(5, 5)}-" +
                   $"{rawKey.Substring(10, 5)}-{rawKey.Substring(15, 5)}";
        }
    }
}
