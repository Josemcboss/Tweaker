using System;
using Tweaker.License;

namespace Tweaker.KeyGenerator
{
    /// <summary>
    /// Herramienta administrativa para generar llaves de licencia
    /// IMPORTANTE: Esta herramienta debe mantenerse privada y no distribuirse
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("════════════════════════════════════════════════════════");
            Console.WriteLine("   TWEAKER - GENERADOR DE LLAVES DE LICENCIA");
            Console.WriteLine("   HERRAMIENTA ADMINISTRATIVA - CONFIDENCIAL");
            Console.WriteLine("════════════════════════════════════════════════════════");
            Console.WriteLine();

            if (args.Length > 0 && args[0] == "--batch")
            {
                // Modo batch: args[1] = fingerprint, args[2] = fecha (opcional)
                BatchMode(args);
                return;
            }

            // Modo interactivo
            InteractiveMode();
        }

        static void InteractiveMode()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Opciones:");
                Console.WriteLine("0. 🎯 Generar MI clave (autodetectar hardware)");
                Console.WriteLine("1. Generar llave perpetua");
                Console.WriteLine("2. Generar llave con fecha de expiración");
                Console.WriteLine("3. Salir");
                Console.Write("\nSeleccione una opción: ");

                var option = Console.ReadLine();

                if (option == "3")
                {
                    break;
                }

                // OPCIÓN 0: Autodetectar hardware y generar clave
                if (option == "0")
                {
                    Console.WriteLine();
                    Console.WriteLine("🔍 Detectando tu hardware...");
                    try
                    {
                        var fingerprint = HardwareFingerprint.GetFingerprint();
                        var displayFingerprint = HardwareFingerprint.GetDisplayFingerprint();
                        
                        Console.WriteLine($"✅ Fingerprint detectado: {displayFingerprint}");
                        Console.WriteLine();
                        Console.WriteLine("🔑 Generando tu clave personalizada...");
                        
                        var licenseKey = LicenseKeyGenerator.GenerateKey(fingerprint, null);
                        
                        PrintLicenseKey(fingerprint, licenseKey, null);
                        
                        // Validar
                        var result = LicenseValidator.ValidateLicenseKey(licenseKey, fingerprint);
                        if (result != null && result.IsValid)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("✅ CLAVE VÁLIDA - Lista para usar en Ghost Optimizer");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("❌ ADVERTENCIA: Error al validar la clave");
                            Console.ResetColor();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"❌ Error al detectar hardware: {ex.Message}");
                        Console.ResetColor();
                    }
                    
                    continue;
                }

                Console.WriteLine();
                Console.Write("Ingrese el Hardware Fingerprint del cliente: ");
                var manualFingerprint = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(manualFingerprint))
                {
                    Console.WriteLine("❌ Fingerprint inválido.");
                    continue;
                }

                DateTime? expirationDate = null;

                if (option == "2")
                {
                    Console.Write("Ingrese fecha de expiración (yyyy-MM-dd): ");
                    var dateStr = Console.ReadLine();

                    if (DateTime.TryParse(dateStr, out var date))
                    {
                        expirationDate = date;
                    }
                    else
                    {
                        Console.WriteLine("❌ Fecha inválida.");
                        continue;
                    }
                }

                try
                {
                    var licenseKey = LicenseKeyGenerator.GenerateKey(manualFingerprint, expirationDate);
                    PrintLicenseKey(manualFingerprint, licenseKey, expirationDate);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error: {ex.Message}");
                }
            }
        }

        static void PrintLicenseKey(string fingerprint, string licenseKey, DateTime? expirationDate)
        {
            Console.WriteLine();
            Console.WriteLine("════════════════════════════════════════════════════════");
            Console.WriteLine("✅ LLAVE GENERADA EXITOSAMENTE");
            Console.WriteLine("════════════════════════════════════════════════════════");
            Console.WriteLine();
            Console.WriteLine($"Hardware Fingerprint: {fingerprint}");
            
            if (expirationDate.HasValue)
            {
                Console.WriteLine($"Fecha de Expiración:  {expirationDate.Value:yyyy-MM-dd}");
            }
            else
            {
                Console.WriteLine($"Tipo:                 Licencia Perpetua ♾️");
            }
            
            Console.WriteLine();
            Console.WriteLine($"LLAVE DE LICENCIA:");
            Console.WriteLine($"╔══════════════════════════════════╗");
            Console.WriteLine($"║  {licenseKey}  ║");
            Console.WriteLine($"╚══════════════════════════════════╝");
            Console.WriteLine();
        }

        static void BatchMode(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Uso: KeyGenerator --batch <fingerprint> [fecha-expiracion]");
                Console.WriteLine("Ejemplo: KeyGenerator --batch ABC123DEF456 2025-12-31");
                return;
            }

            var fingerprint = args[1];
            DateTime? expirationDate = null;

            if (args.Length >= 3)
            {
                if (DateTime.TryParse(args[2], out var date))
                {
                    expirationDate = date;
                }
                else
                {
                    Console.WriteLine($"❌ Fecha inválida: {args[2]}");
                    return;
                }
            }

            try
            {
                var licenseKey = LicenseKeyGenerator.GenerateKey(fingerprint, expirationDate);
                Console.WriteLine(licenseKey);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                Environment.Exit(1);
            }
        }
    }
}
