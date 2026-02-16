using System;
using System.Security.Cryptography;
using System.Text;

namespace SimpleKeyGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("????????????????????????????????????????????????????????");
            Console.WriteLine("   GHOST OPTIMIZER - GENERADOR DE LLAVES SIMPLE");
            Console.WriteLine("????????????????????????????????????????????????????????");
            Console.WriteLine();

            // Generar fingerprint simulado
            string fingerprint = GenerateTestFingerprint();
            Console.WriteLine($"?? Hardware Fingerprint: {fingerprint}");
            Console.WriteLine();

            // Generar fecha de expiración (1 año desde ahora)
            DateTime expiration = DateTime.Now.AddYears(1);
            Console.WriteLine($"?? Fecha de Expiración: {expiration:yyyy-MM-dd}");
            Console.WriteLine();

            // Generar la clave de licencia
            string licenseKey = GenerateLicenseKey(fingerprint, expiration);
            Console.WriteLine("?? CLAVE DE LICENCIA GENERADA:");
            Console.WriteLine("????????????????????????????????????????????????????????");
            Console.WriteLine($"{licenseKey}");
            Console.WriteLine("????????????????????????????????????????????????????????");
            Console.WriteLine();

            Console.WriteLine("?? INSTRUCCIONES:");
            Console.WriteLine("1. Copia la clave de licencia completa");
            Console.WriteLine("2. Abre Ghost Optimizer");
            Console.WriteLine("3. Se abrirá la ventana de activación");
            Console.WriteLine("4. Pega la clave en el campo de texto");
            Console.WriteLine("5. Haz clic en 'Activar'");
            Console.WriteLine();

            Console.WriteLine("?? DATOS TÉCNICOS:");
            Console.WriteLine($"   • Fingerprint: {fingerprint}");
            Console.WriteLine($"   • Válida hasta: {expiration:dd/MM/yyyy}");
            Console.WriteLine($"   • Tipo: Licencia de desarrollo/prueba");
            Console.WriteLine();

            Console.WriteLine("Presiona cualquier tecla para salir...");
            Console.ReadKey();
        }

        static string GenerateTestFingerprint()
        {
            // Simular fingerprint para testing
            string testData = $"TESTPC-{Environment.MachineName}-{Environment.UserName}";
            using (var sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(testData));
                return Convert.ToBase64String(hash)[0..16]; // Primeros 16 caracteres
            }
        }

        static string GenerateLicenseKey(string fingerprint, DateTime expiration)
        {
            // Formato: GHOST-XXXXX-XXXXX-XXXXX-XXXXX
            var random = new Random();
            var segments = new string[5];
            segments[0] = "GHOST";

            // Generar 4 segmentos de 5 caracteres cada uno
            for (int i = 1; i < 5; i++)
            {
                segments[i] = GenerateSegment(random);
            }

            return string.Join("-", segments);
        }

        static string GenerateSegment(Random random)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            char[] segment = new char[5];
            
            for (int i = 0; i < 5; i++)
            {
                segment[i] = chars[random.Next(chars.Length)];
            }
            
            return new string(segment);
        }
    }
}