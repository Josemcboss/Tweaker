using System;
using System.Windows;

namespace Tweaker.License
{
    /// <summary>
    /// Gestor central del sistema de licencias
    /// </summary>
    public static class LicenseManager
    {
        private static LicenseData? currentLicense;
        private static bool isValidated = false;

        /// <summary>
        /// Indica si la aplicación tiene una licencia válida
        /// </summary>
        public static bool IsLicenseValid => isValidated && currentLicense != null && currentLicense.IsValid;

        /// <summary>
        /// Obtiene los datos de la licencia actual
        /// </summary>
        public static LicenseData? CurrentLicense => currentLicense;

        /// <summary>
        /// Valida la licencia al inicio de la aplicación
        /// </summary>
        /// <returns>True si la licencia es válida, False si no lo es</returns>
        public static bool ValidateLicenseOnStartup()
        {
            try
            {
                // Obtener fingerprint actual
                var currentFingerprint = HardwareFingerprint.GetFingerprint();

                // Cargar licencia guardada
                var (licenseKey, savedLicense) = LicenseStorage.LoadLicense();

                if (licenseKey == null || savedLicense == null)
                {
                    // No hay licencia guardada
                    isValidated = false;
                    return false;
                }

                // Verificar que el fingerprint coincida
                if (savedLicense.HardwareFingerprint != currentFingerprint)
                {
                    // Hardware cambió, licencia inválida
                    isValidated = false;
                    return false;
                }

                // Verificar que no esté expirada
                if (savedLicense.IsExpired)
                {
                    // Licencia expirada
                    isValidated = false;
                    return false;
                }

                // Re-validar la llave completa
                var validatedLicense = LicenseValidator.ValidateLicenseKey(licenseKey, currentFingerprint);
                if (validatedLicense == null)
                {
                    // Llave no válida (archivo corrupto/modificado)
                    isValidated = false;
                    return false;
                }

                // Licencia válida
                currentLicense = validatedLicense;
                isValidated = true;
                return true;
            }
            catch
            {
                isValidated = false;
                return false;
            }
        }

        /// <summary>
        /// Muestra la ventana de activación
        /// </summary>
        /// <param name="showCancelOption">Si se permite cancelar (false = cerrar app si se cancela)</param>
        /// <returns>True si se activó correctamente</returns>
        public static bool ShowActivationWindow(bool showCancelOption = true)
        {
            var activationWindow = new ActivationWindow();
            var result = activationWindow.ShowDialog();

            if (result == true && activationWindow.ActivationSuccessful)
            {
                // Re-validar después de la activación
                return ValidateLicenseOnStartup();
            }

            if (!showCancelOption && result != true)
            {
                // Si no se permite cancelar y el usuario canceló, cerrar la aplicación
                MessageBox.Show(
                    "⚠️ Tweaker requiere una licencia válida para funcionar.\n\n" +
                    "La aplicación se cerrará.",
                    "Licencia Requerida",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                
                Application.Current.Shutdown();
            }

            return result == true;
        }

        /// <summary>
        /// Verifica y solicita activación si es necesario
        /// </summary>
        /// <returns>True si hay licencia válida, False si no</returns>
        public static bool EnsureValidLicense()
        {
            if (!IsLicenseValid)
            {
                return ShowActivationWindow(showCancelOption: false);
            }
            return true;
        }

        /// <summary>
        /// Desactiva la licencia actual (elimina archivo de licencia)
        /// </summary>
        public static void DeactivateLicense()
        {
            LicenseStorage.DeleteLicense();
            currentLicense = null;
            isValidated = false;
        }

        /// <summary>
        /// Obtiene información de la licencia para mostrar
        /// </summary>
        public static string GetLicenseInfo()
        {
            if (!IsLicenseValid || currentLicense == null)
            {
                return "Sin licencia activa";
            }

            var info = "Licencia Activa\n";
            info += $"Hardware ID: {HardwareFingerprint.GetDisplayFingerprint()}\n";
            
            if (currentLicense.IsPerpetual)
            {
                info += "Tipo: Perpetua ♾️\n";
            }
            else
            {
                info += $"Expira: {currentLicense.ExpirationDate:yyyy-MM-dd}\n";
                var daysRemaining = (currentLicense.ExpirationDate!.Value - DateTime.Now).Days;
                info += $"Días restantes: {daysRemaining}\n";
            }

            info += $"Activada: {currentLicense.CreatedDate:yyyy-MM-dd}";

            return info;
        }
    }
}
