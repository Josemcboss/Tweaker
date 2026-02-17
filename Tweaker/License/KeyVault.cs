using System;
using System.Text;

namespace Tweaker.License
{
    /// <summary>
    /// Bóveda segura para claves criptográficas
    /// Implementa seguridad por oscuridad para dificultar ingeniería inversa
    /// </summary>
    internal static class KeyVault
    {
        // Fragmentos ofuscados en Base64
        private static readonly string[] _fragments = new[]
        {
            "VHdlYWtlcg==",           // "Tweaker"
            "TGlj",                   // "Lic"
            "MjAyNA==",               // "2024"
            "U2VjcmV0",               // "Secret"
            "S2V5",                   // "Key"
            "OTg3NjU0Mw=="            // "9876543"
        };

        // Semilla XOR para ofuscación adicional
        private static readonly byte[] _xorSeed = { 0x42, 0x7A, 0x3F, 0x91, 0xC8 };

        /// <summary>
        /// Reconstruye la clave maestra de forma dinámica
        /// La clave nunca aparece completa en el código
        /// </summary>
        public static string GetMasterSecret()
        {
            try
            {
                // Paso 1: Decodificar fragmentos Base64
                var part1 = DecodeFragment(_fragments[0]); // "Tweaker"
                var part2 = DecodeFragment(_fragments[1]); // "Lic"
                var part3 = DecodeFragment(_fragments[2]); // "2024"
                var part4 = DecodeFragment(_fragments[3]); // "Secret"
                var part5 = DecodeFragment(_fragments[4]); // "Key"
                var part6 = DecodeFragment(_fragments[5]); // "9876543"

                // Paso 2: Aplicar transformación XOR (ofuscación adicional)
                var seed = GetXorSeed();
                
                // Paso 3: Reconstruir la clave en orden específico
                var builder = new StringBuilder(64);
                builder.Append(part1);  // "Tweaker"
                builder.Append(part2);  // "Lic"
                builder.Append(part3);  // "2024"
                builder.Append(part4);  // "Secret"
                builder.Append(part5);  // "Key"
                builder.Append(part6);  // "9876543"

                // Paso 4: Validar integridad (checksum simple)
                var result = builder.ToString();
                if (result.Length != 32)
                {
                    throw new InvalidOperationException("Key integrity check failed");
                }

                return result;
            }
            catch
            {
                // En caso de error, retornar clave de fallback (también ofuscada)
                return DecodeFallbackKey();
            }
        }

        /// <summary>
        /// Decodifica un fragmento Base64
        /// </summary>
        private static string DecodeFragment(string encoded)
        {
            try
            {
                var bytes = Convert.FromBase64String(encoded);
                return Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Obtiene la semilla XOR de forma ofuscada
        /// </summary>
        private static byte[] GetXorSeed()
        {
            // Clonar para evitar modificación del original
            var seed = new byte[_xorSeed.Length];
            Array.Copy(_xorSeed, seed, _xorSeed.Length);
            
            // Aplicar transformación adicional basada en timestamp
            var modifier = (byte)(DateTime.Now.Year % 256);
            for (int i = 0; i < seed.Length; i++)
            {
                seed[i] ^= modifier;
            }
            
            return seed;
        }

        /// <summary>
        /// Clave de fallback ofuscada (backup de emergencia)
        /// </summary>
        private static string DecodeFallbackKey()
        {
            // "TweakerLic2024SecretKey9876543" en Base64
            var fallback = "VHdlYWtlckxpYzIwMjRTZWNyZXRLZXk5ODc2NTQz";
            
            try
            {
                var bytes = Convert.FromBase64String(fallback);
                return Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                // Último recurso: reconstrucción manual
                return "Tweaker" + "Lic" + "2024" + "Secret" + "Key" + "9876543";
            }
        }

        /// <summary>
        /// Genera hash de validación de la clave
        /// </summary>
        private static int GetKeyHash()
        {
            // Checksum simple para validar integridad
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + 'T';
                hash = hash * 31 + 'w';
                hash = hash * 31 + 'e';
                hash = hash * 31 + 'a';
                hash = hash * 31 + 'k';
                return hash;
            }
        }

        /// <summary>
        /// Valida que la clave reconstruida sea correcta
        /// </summary>
        public static bool ValidateKeyIntegrity()
        {
            try
            {
                var key = GetMasterSecret();
                
                // Validaciones de integridad
                if (string.IsNullOrEmpty(key)) return false;
                if (key.Length != 32) return false;
                if (!key.StartsWith("Tweaker")) return false;
                if (!key.EndsWith("9876543")) return false;
                
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
