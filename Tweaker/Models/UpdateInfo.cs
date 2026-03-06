using System;

namespace Tweaker.Models
{
    /// <summary>
    /// Información sobre una actualización disponible
    /// </summary>
    public class UpdateInfo
    {
        /// <summary>
        /// Versión de la actualización (ej: "2.4.0")
        /// </summary>
        public string? Version { get; set; }

        /// <summary>
        /// URL de descarga del instalador
        /// </summary>
        public string? DownloadUrl { get; set; }

        /// <summary>
        /// Tamaño del archivo en bytes
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// Hash SHA256 del archivo para verificar integridad
        /// </summary>
        public string? Sha256Hash { get; set; }

        /// <summary>
        /// Changelog de la actualización
        /// </summary>
        public string? Changelog { get; set; }

        /// <summary>
        /// Fecha de publicación
        /// </summary>
        public DateTime ReleaseDate { get; set; }

        /// <summary>
        /// Si es una actualización crítica (obligatoria)
        /// </summary>
        public bool IsCritical { get; set; }

        /// <summary>
        /// Versión mínima requerida para actualizar
        /// </summary>
        public string? MinimumVersion { get; set; }

        /// <summary>
        /// Notas adicionales
        /// </summary>
        public string? ReleaseNotes { get; set; }
    }
}
