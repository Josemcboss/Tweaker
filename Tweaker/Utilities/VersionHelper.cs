using System;
using System.Reflection;

namespace Tweaker.Utilities
{
    /// <summary>
    /// Helper para manejo de versiones de la aplicación
    /// </summary>
    public static class VersionHelper
    {
        /// <summary>
        /// Versión actual de la aplicación
        /// </summary>
        public static string CurrentVersion => "2.4.0";

        /// <summary>
        /// Versión completa con build
        /// </summary>
        public static string FullVersion 
        { 
            get
            {
                try
                {
                    var assembly = Assembly.GetExecutingAssembly();
                    var version = assembly.GetName().Version;
                    return $"{version.Major}.{version.Minor}.{version.Build}";
                }
                catch
                {
                    return CurrentVersion;
                }
            }
        }

        /// <summary>
        /// Compara dos versiones semver
        /// </summary>
        /// <returns>
        /// -1 si version1 < version2
        ///  0 si version1 == version2
        ///  1 si version1 > version2
        /// </returns>
        public static int CompareVersions(string version1, string version2)
        {
            try
            {
                // Limpiar versiones
                version1 = version1?.Trim().TrimStart('v') ?? "0.0.0";
                version2 = version2?.Trim().TrimStart('v') ?? "0.0.0";

                // Parsear versiones
                var v1Parts = version1.Split('.');
                var v2Parts = version2.Split('.');

                int length = Math.Max(v1Parts.Length, v2Parts.Length);

                for (int i = 0; i < length; i++)
                {
                    int v1Part = i < v1Parts.Length && int.TryParse(v1Parts[i], out int p1) ? p1 : 0;
                    int v2Part = i < v2Parts.Length && int.TryParse(v2Parts[i], out int p2) ? p2 : 0;

                    if (v1Part < v2Part) return -1;
                    if (v1Part > v2Part) return 1;
                }

                return 0;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Verifica si hay una nueva versión disponible
        /// </summary>
        public static bool IsNewerVersion(string remoteVersion)
        {
            return CompareVersions(CurrentVersion, remoteVersion) < 0;
        }

        /// <summary>
        /// Obtiene información de versión para mostrar al usuario
        /// </summary>
        public static string GetVersionDisplayString()
        {
            return $"Ghost Optimizer v{CurrentVersion}";
        }

        /// <summary>
        /// Verifica si la versión actual cumple con el mínimo requerido
        /// </summary>
        public static bool MeetsMinimumVersion(string minimumVersion)
        {
            if (string.IsNullOrEmpty(minimumVersion))
                return true;

            return CompareVersions(CurrentVersion, minimumVersion) >= 0;
        }
    }
}
