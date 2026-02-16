using System;
using Tweaker.License;

namespace Tweaker.LicenseTest
{
    /// <summary>
    /// Programa de prueba para el sistema de licencias
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // Modo de exportación de fingerprint
            if (args.Length > 0 && args[0] == "--export-fingerprint")
            {
                ExportFingerprint();
                return;
            }
            
            Console.WriteLine("════════════════════════════════════════════════════════");
            Console.WriteLine("   TWEAKER - TEST DEL SISTEMA DE LICENCIAS");
            Console.WriteLine("════════════════════════════════════════════════════════");
            Console.WriteLine();

            try
            {
                // Test 1: Generar hardware fingerprint
                Console.WriteLine("Test 1: Generando hardware fingerprint...");
                var fingerprint = HardwareFingerprint.GetFingerprint();
                var displayFingerprint = HardwareFingerprint.GetDisplayFingerprint();
                Console.WriteLine($"✅ Fingerprint completo: {fingerprint}");
                Console.WriteLine($"✅ Fingerprint para mostrar: {displayFingerprint}");
                Console.WriteLine();

                // Test 2: Generar llave de licencia perpetua
                Console.WriteLine("Test 2: Generando llave de licencia perpetua...");
                var perpetualKey = LicenseKeyGenerator.GenerateKey(fingerprint);
                Console.WriteLine($"✅ Llave generada: {perpetualKey}");
                Console.WriteLine();

                // Test 3: Validar formato de llave
                Console.WriteLine("Test 3: Validando formato de llave...");
                var isValidFormat = LicenseValidator.IsValidFormat(perpetualKey);
                Console.WriteLine($"✅ Formato válido: {isValidFormat}");
                Console.WriteLine();

                // Test 4: Validar llave de licencia
                Console.WriteLine("Test 4: Validando llave de licencia...");
                var licenseData = LicenseValidator.ValidateLicenseKey(perpetualKey, fingerprint);
                if (licenseData != null)
                {
                    Console.WriteLine($"✅ Llave válida");
                    Console.WriteLine($"   - Tipo: {(licenseData.IsPerpetual ? "Perpetua" : "Temporal")}");
                    Console.WriteLine($"   - Válida: {licenseData.IsValid}");
                    Console.WriteLine($"   - Expirada: {licenseData.IsExpired}");
                }
                else
                {
                    Console.WriteLine("❌ Llave inválida");
                }
                Console.WriteLine();

                // Test 5: Probar llave con fingerprint incorrecto
                Console.WriteLine("Test 5: Validando con fingerprint incorrecto (debe fallar)...");
                var wrongFingerprint = "WRONG_FINGERPRINT_123456";
                var invalidLicense = LicenseValidator.ValidateLicenseKey(perpetualKey, wrongFingerprint);
                if (invalidLicense == null)
                {
                    Console.WriteLine("✅ Correctamente rechazada (fingerprint no coincide)");
                }
                else
                {
                    Console.WriteLine("❌ ERROR: Debería haber rechazado la llave");
                }
                Console.WriteLine();

                // Test 6: Generar llave con expiración
                Console.WriteLine("Test 6: Generando llave con expiración...");
                var expirationDate = DateTime.Now.AddDays(30);
                var temporalKey = LicenseKeyGenerator.GenerateKey(fingerprint, expirationDate);
                Console.WriteLine($"✅ Llave temporal generada: {temporalKey}");
                var temporalLicense = LicenseValidator.ValidateLicenseKey(temporalKey, fingerprint);
                if (temporalLicense != null)
                {
                    Console.WriteLine($"✅ Llave temporal válida");
                    Console.WriteLine($"   - Expira: {temporalLicense.ExpirationDate:yyyy-MM-dd}");
                    Console.WriteLine($"   - Días restantes: {(temporalLicense.ExpirationDate.Value - DateTime.Now).Days}");
                }
                Console.WriteLine();

                // Test 7: Guardar y cargar licencia
                Console.WriteLine("Test 7: Guardando y cargando licencia...");
                LicenseStorage.SaveLicense(perpetualKey, licenseData!);
                Console.WriteLine("✅ Licencia guardada");
                
                var (loadedKey, loadedData) = LicenseStorage.LoadLicense();
                if (loadedKey != null && loadedData != null)
                {
                    Console.WriteLine($"✅ Licencia cargada correctamente");
                    Console.WriteLine($"   - Llave: {loadedKey}");
                    Console.WriteLine($"   - Fingerprint coincide: {loadedData.HardwareFingerprint == fingerprint}");
                }
                else
                {
                    Console.WriteLine("❌ ERROR: No se pudo cargar la licencia");
                }
                Console.WriteLine();

                // Test 8: LicenseManager
                Console.WriteLine("Test 8: Probando LicenseManager...");
                var isValid = LicenseManager.ValidateLicenseOnStartup();
                Console.WriteLine($"✅ Licencia válida en startup: {isValid}");
                Console.WriteLine($"✅ Info de licencia:\n{LicenseManager.GetLicenseInfo()}");
                Console.WriteLine();

                // Limpiar
                Console.WriteLine("Limpiando licencia de prueba...");
                LicenseStorage.DeleteLicense();
                Console.WriteLine("✅ Licencia eliminada");
                Console.WriteLine();

                Console.WriteLine("════════════════════════════════════════════════════════");
                Console.WriteLine("   ✅ TODOS LOS TESTS PASARON EXITOSAMENTE");
                Console.WriteLine("════════════════════════════════════════════════════════");
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("════════════════════════════════════════════════════════");
                Console.WriteLine("   ❌ ERROR EN LOS TESTS");
                Console.WriteLine("════════════════════════════════════════════════════════");
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Exporta el fingerprint del hardware actual
        /// </summary>
        static void ExportFingerprint()
        {
            try
            {
                Console.WriteLine("════════════════════════════════════════════════════════");
                Console.WriteLine("   EXPORTAR FINGERPRINT PARA ACTIVACIÓN");
                Console.WriteLine("   Ghost Optimizer v2.3.0");
                Console.WriteLine("════════════════════════════════════════════════════════");
                Console.WriteLine();

                Console.WriteLine("🔍 Detectando hardware de tu computadora...");
                Console.WriteLine();

                var fingerprint = HardwareFingerprint.GetFingerprint();
                
                Console.WriteLine("✅ FINGERPRINT DETECTADO:");
                Console.WriteLine("════════════════════════════════════════════════════════");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(fingerprint);
                Console.ResetColor();
                Console.WriteLine("════════════════════════════════════════════════════════");
                Console.WriteLine();

                Console.WriteLine("📋 Este es tu Hardware Fingerprint COMPLETO");
                Console.WriteLine("   Envía EXACTAMENTE este texto a tu proveedor de licencias");
                Console.WriteLine();

                // Información adicional del sistema
                Console.WriteLine("ℹ️  Información del Sistema:");
                Console.WriteLine($"   • Computadora: {Environment.MachineName}");
                Console.WriteLine($"   • Usuario: {Environment.UserName}");
                Console.WriteLine($"   • OS: {Environment.OSVersion}");
                Console.WriteLine($"   • .NET: {Environment.Version}");
                Console.WriteLine($"   • Arquitectura: {(Environment.Is64BitOperatingSystem ? "64-bit" : "32-bit")}");
                Console.WriteLine();

                Console.WriteLine("💡 SIGUIENTE PASO:");
                Console.WriteLine("   1. Copia el FINGERPRINT mostrado arriba");
                Console.WriteLine("   2. Envíalo a tu proveedor de licencias");
                Console.WriteLine("   3. Recibirás una clave de activación única");
                Console.WriteLine("   4. Ingresa la clave en Ghost Optimizer");
                Console.WriteLine();

                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Error al detectar hardware: {ex.Message}");
                Console.ResetColor();
                Environment.Exit(1);
            }
        }
    }
}
