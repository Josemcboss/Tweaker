using System;
using System.Text;

namespace Tweaker.License
{
    /// <summary>
    /// Bóveda segura para claves criptográficas
    /// Implementa seguridad por oscuridad para dificultar ingeniería inversa
    /// </summary>
    public class KeyVault : IKeyVault
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

        /// <summary>
        /// Reconstruye la clave maestra de forma dinámica
        /// </summary>
        public string GetMasterSecret()
        {
            try
            {
                // Reconstruir la clave de forma menos predecible
                var p1 = DecodeFragment(_fragments[0]);
                var p2 = DecodeFragment(_fragments[1]);
                var p3 = DecodeFragment(_fragments[2]);
                var p4 = DecodeFragment(_fragments[3]);
                var p5 = DecodeFragment(_fragments[4]);
                var p6 = DecodeFragment(_fragments[5]);

                return $"{p1}{p2}{p3}{p4}{p5}{p6}";
            }
            catch
            {
                return "TweakerLic2024SecretKey9876543";
            }
        }

        private string DecodeFragment(string encoded)
        {
            try
            {
                var bytes = Convert.FromBase64String(encoded);
                return Encoding.UTF8.GetString(bytes);
            }
            catch { return string.Empty; }
        }

        public bool ValidateKeyIntegrity()
        {
            var key = GetMasterSecret();
            return !string.IsNullOrEmpty(key) && key.Length == 30 && key.StartsWith("Tweaker") && key.EndsWith("9876543");
        }
    }
}
