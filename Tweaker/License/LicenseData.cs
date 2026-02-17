using System;

namespace Tweaker.License
{
    /// <summary>
    /// Representa los datos de una licencia
    /// </summary>
    public class LicenseData
    {
        /// <summary>
        /// Fingerprint del hardware autorizado
        /// </summary>
        public string HardwareFingerprint { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de expiración de la licencia (null = perpetua)
        /// </summary>
        public DateTime? ExpirationDate { get; set; }

        /// <summary>
        /// Fecha de creación de la licencia
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Límite máximo de tweaks activos (-1 = ilimitado)
        /// </summary>
        public int MaxTweaks { get; set; } = -1;

        /// <summary>
        /// Indica si la licencia es perpetua
        /// </summary>
        public bool IsPerpetual => ExpirationDate == null;

        /// <summary>
        /// Indica si la licencia ha expirado
        /// </summary>
        public bool IsExpired => ExpirationDate.HasValue && DateTime.Now > ExpirationDate.Value;

        /// <summary>
        /// Indica si la licencia es válida (no expirada)
        /// </summary>
        public bool IsValid => !IsExpired;

        /// <summary>
        /// Indica si la licencia tiene tweaks ilimitados
        /// </summary>
        public bool IsUnlimitedTweaks => MaxTweaks == -1;
    }
}
