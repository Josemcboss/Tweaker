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
        // SEGURIDAD: La clave secreta se obtiene dinámicamente desde KeyVault
        // Esto dificulta la ingeniería inversa al no tener la clave completa en el código

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
                // SEGURIDAD NIVEL 1: Verificar anti-debugging (solo en Release)
#if !DEBUG
                if (!AntiDebugger.IsDevelopmentEnvironment() && AntiDebugger.IsDebuggerAttached())
                {
                    System.Diagnostics.Debug.WriteLine("🚨 SEGURIDAD: Debugger detectado durante validación de licencia");
                    AntiDebugger.HandleDebuggerDetection();
                    return null; // No se llegará aquí si HandleDebuggerDetection termina el proceso
                }
#endif

                // SEGURIDAD NIVEL 2: Verificar herramientas de análisis (solo en Release)
#if !DEBUG
                if (!AnalysisToolDetector.IsDevelopmentEnvironment() && AnalysisToolDetector.PerformFullCheck())
                {
                    System.Diagnostics.Debug.WriteLine("🚨 SEGURIDAD: Herramienta de análisis detectada");
                    AnalysisToolDetector.HandleToolDetection();
                    return null; // No se llegará aquí si HandleToolDetection termina el proceso
                }
#endif

                // Verificar formato
                if (!IsValidFormat(licenseKey))
                {
                    return null;
                }


                // NUEVO ENFOQUE CON MAXTWEAK: Probar diferentes combinaciones
                
                // 1. Intentar con diferentes combinaciones de maxTweaks
                // Primero probar valores comunes: -1 (ilimitado), 10, 20, 30, 50, 100
                int[] commonLimits = { -1, 10, 20, 30, 50, 100 };
                
                // Probar licencia perpetua con diferentes límites
                foreach (var maxTweaks in commonLimits)
                {
                    var perpetualHash = GenerateLicenseHash(currentHardwareFingerprint, null, DateTime.Now, maxTweaks);
                    if (licenseKey.Replace("-", "").ToUpper() == perpetualHash.ToUpper())
                    {
                        return new LicenseData
                        {
                            HardwareFingerprint = currentHardwareFingerprint,
                            ExpirationDate = null,
                            CreatedDate = DateTime.Now,
                            MaxTweaks = maxTweaks
                        };
                    }
                }

                // 2. Probar con fechas de expiración (próximos 10 años) y diferentes límites
                for (int years = 0; years <= 10; years++)
                {
                    for (int months = 0; months < 12; months++)
                    {
                        var testDate = DateTime.Now.AddYears(years).AddMonths(months);
                        
                        foreach (var maxTweaks in commonLimits)
                        {
                            var dateHash = GenerateLicenseHash(currentHardwareFingerprint, testDate, DateTime.Now, maxTweaks);
                            
                            if (licenseKey.Replace("-", "").ToUpper() == dateHash.ToUpper())
                            {
                                return new LicenseData
                                {
                                    HardwareFingerprint = currentHardwareFingerprint,
                                    ExpirationDate = testDate,
                                    CreatedDate = DateTime.Now,
                                    MaxTweaks = maxTweaks
                                };
                            }
                        }
                    }
                }

                // 3. RETROCOMPATIBILIDAD: Probar con el formato antiguo (sin maxTweaks)
                var legacyHashPerpetual = GenerateLegacyHash(currentHardwareFingerprint, null);
                if (licenseKey.Replace("-", "").ToUpper() == legacyHashPerpetual.ToUpper())
                {
                    return new LicenseData
                    {
                        HardwareFingerprint = currentHardwareFingerprint,
                        ExpirationDate = null,
                        CreatedDate = DateTime.Now,
                        MaxTweaks = -1 // Licencias antiguas son ilimitadas
                    };
                }

                // Probar fechas con formato legacy
                for (int years = 0; years <= 10; years++)
                {
                    for (int months = 0; months < 12; months++)
                    {
                        var testDate = DateTime.Now.AddYears(years).AddMonths(months);
                        var legacyHash = GenerateLegacyHash(currentHardwareFingerprint, testDate);
                        
                        if (licenseKey.Replace("-", "").ToUpper() == legacyHash.ToUpper())
                        {
                            return new LicenseData
                            {
                                HardwareFingerprint = currentHardwareFingerprint,
                                ExpirationDate = testDate,
                                CreatedDate = DateTime.Now,
                                MaxTweaks = -1 // Licencias antiguas son ilimitadas
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
        private static string GenerateLicenseHash(string hardwareFingerprint, DateTime? expirationDate, DateTime createdDate, int maxTweaks)
        {
            // Obtener clave secreta desde KeyVault (seguridad por oscuridad)
            string secretKey = KeyVault.GetMasterSecret();
            
            var licenseData = new StringBuilder();
            licenseData.Append(hardwareFingerprint);
            licenseData.Append("|");
            licenseData.Append(expirationDate?.ToString("yyyy-MM-dd") ?? "PERPETUAL");
            licenseData.Append("|");
            licenseData.Append(createdDate.ToString("yyyy-MM-dd"));
            licenseData.Append("|");
            licenseData.Append(maxTweaks);
            licenseData.Append("|");
            licenseData.Append(secretKey);
            
            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(licenseData.ToString()));
                return BitConverter.ToString(hashBytes, 0, 10).Replace("-", "");
            }
        }

        /// <summary>
        /// Genera hash con formato legacy (sin maxTweaks) para retrocompatibilidad
        /// </summary>
        private static string GenerateLegacyHash(string hardwareFingerprint, DateTime? expirationDate)
        {
            // Obtener clave secreta desde KeyVault (seguridad por oscuridad)
            string secretKey = KeyVault.GetMasterSecret();
            
            var licenseData = new StringBuilder();
            licenseData.Append(hardwareFingerprint);
            licenseData.Append("|");
            licenseData.Append(expirationDate?.ToString("yyyy-MM-dd") ?? "PERPETUAL");
            licenseData.Append("|");
            licenseData.Append(secretKey);
            
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
                // Obtener clave secreta desde KeyVault (seguridad por oscuridad)
                string secretKey = KeyVault.GetMasterSecret();
                
                using (var aes = Aes.Create())
                {
                    // Derivar clave de 32 bytes (256 bits)
                    var key = new byte[32];
                    var keyBytes = Encoding.UTF8.GetBytes(secretKey);
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
                        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(secretKey + "IV_SALT"));
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
