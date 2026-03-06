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

            var keyVault = new KeyVault();
            var securityChecks = new SecurityChecks();
            var licenseKeyGenerator = new LicenseKeyGenerator(keyVault);
            var licenseValidator = new LicenseValidator(keyVault, securityChecks);

            if (args.Length > 0 && args[0] == "--batch")
            {
                // Modo batch: args[1] = fingerprint, args[2] = fecha (opcional)
                BatchMode(args, licenseKeyGenerator);
                return;
            }

            // Modo interactivo
            InteractiveMode(licenseKeyGenerator, licenseValidator);
        }

        static void InteractiveMode(LicenseKeyGenerator licenseKeyGenerator, LicenseValidator licenseValidator)
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
                        
                        Console.WriteLine($"✅ Fingerprint detectado: {fingerprint}");
                        
                        // Preguntar por el límite de tweaks
                        Console.Write("Límite de Tweaks (-1 para ilimitado, o número específico): ");
                        var maxTweaksInput = Console.ReadLine()?.Trim();
                        int maxTweaksValue = -1; // Por defecto ilimitado
                        
                        if (!string.IsNullOrEmpty(maxTweaksInput) && int.TryParse(maxTweaksInput, out var parsedValue))
                        {
                            maxTweaksValue = parsedValue;
                        }
                        
                        Console.WriteLine();
                        Console.WriteLine("🔑 Generando tu clave personalizada...");
                        
                        var licenseKey = licenseKeyGenerator.GenerateKey(fingerprint, null, maxTweaksValue);
                        
                        if (string.IsNullOrEmpty(licenseKey))
                        {
                            Console.WriteLine("❌ ERROR: La clave generada está vacía.");
                        }
                        else
                        {
                            PrintLicenseKey(fingerprint, licenseKey, null, maxTweaksValue);
                            
                            // Validar
                            var result = licenseValidator.ValidateLicenseKey(licenseKey, fingerprint, DateTime.Now);
                            if (result != null)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("✅ CLAVE VÁLIDA - Lista para usar en Ghost Optimizer");
                                Console.ResetColor();
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("❌ ADVERTENCIA: La clave generada no pasó la validación inmediata.");
                                Console.ResetColor();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"❌ ERROR CRÍTICO: {ex.Message}");
                        if (ex.InnerException != null) Console.WriteLine($"   Detalle: {ex.InnerException.Message}");
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
                int maxTweaks = -1; // Por defecto ilimitado

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

                // Preguntar por el límite de tweaks
                Console.Write("Límite de Tweaks (-1 para ilimitado, o número específico): ");
                var maxTweaksStr = Console.ReadLine()?.Trim();
                
                if (!string.IsNullOrEmpty(maxTweaksStr) && int.TryParse(maxTweaksStr, out var parsedMaxTweaks))
                {
                    if (parsedMaxTweaks < -1)
                    {
                        Console.WriteLine("⚠️ Valor inválido. Usando ilimitado (-1).");
                        maxTweaks = -1;
                    }
                    else
                    {
                        maxTweaks = parsedMaxTweaks;
                    }
                }

                try
                {
                    var licenseKey = licenseKeyGenerator.GenerateKey(manualFingerprint, expirationDate, maxTweaks);
                    PrintLicenseKey(manualFingerprint, licenseKey, expirationDate, maxTweaks);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error: {ex.Message}");
                }
            }
        }

        static void PrintLicenseKey(string fingerprint, string licenseKey, DateTime? expirationDate, int maxTweaks = -1)
        {
            Console.WriteLine();
            Console.WriteLine("════════════════════════════════════════════════════════");
            Console.WriteLine("✅ LLAVE GENERADA EXITOSAMENTE");
            Console.WriteLine("════════════════════════════════════════════════════════");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"LLAVE DE LICENCIA:    {licenseKey}");
            Console.ResetColor();
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
            
            // Mostrar límite de tweaks
            if (maxTweaks == -1)
            {
                Console.WriteLine($"Límite de Tweaks:     ∞ Ilimitado");
            }
            else
            {
                Console.WriteLine($"Límite de Tweaks:     {maxTweaks} optimizaciones");
            }
            
        }

        static void BatchMode(string[] args, LicenseKeyGenerator licenseKeyGenerator)
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
                var licenseKey = licenseKeyGenerator.GenerateKey(fingerprint, expirationDate);
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
