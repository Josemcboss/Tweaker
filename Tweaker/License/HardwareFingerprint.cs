using System;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;

namespace Tweaker.License
{
    /// <summary>
    /// Genera un fingerprint único del hardware de la máquina
    /// </summary>
    public static class HardwareFingerprint
    {
        /// <summary>
        /// Obtiene el fingerprint SHA-256 del hardware actual
        /// </summary>
        public static string GetFingerprint()
        {
            try
            {
                var components = new StringBuilder();

                // UUID de la placa madre
                components.Append(GetMotherboardId());
                components.Append("|");

                // Número de serie del procesador
                components.Append(GetProcessorId());
                components.Append("|");

                // Dirección MAC de la interfaz de red principal
                components.Append(GetMacAddress());
                components.Append("|");

                // Número de serie del disco duro/SSD
                components.Append(GetDiskSerialNumber());

                // Generar hash SHA-256
                using (var sha256 = SHA256.Create())
                {
                    var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(components.ToString()));
                    return Convert.ToBase64String(hash).Replace("+", "").Replace("/", "").Replace("=", "").Substring(0, 32);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al generar fingerprint del hardware: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene el ID de la placa madre
        /// </summary>
        private static string GetMotherboardId()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BaseBoard"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        var serial = obj["SerialNumber"]?.ToString();
                        if (!string.IsNullOrWhiteSpace(serial))
                            return serial;
                    }
                }
            }
            catch { }

            return "UNKNOWN_MB";
        }

        /// <summary>
        /// Obtiene el ID del procesador
        /// </summary>
        private static string GetProcessorId()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT ProcessorId FROM Win32_Processor"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        var processorId = obj["ProcessorId"]?.ToString();
                        if (!string.IsNullOrWhiteSpace(processorId))
                            return processorId;
                    }
                }
            }
            catch { }

            return "UNKNOWN_CPU";
        }

        /// <summary>
        /// Obtiene la dirección MAC de la interfaz de red principal
        /// </summary>
        private static string GetMacAddress()
        {
            try
            {
                var nics = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(nic => nic.OperationalStatus == OperationalStatus.Up &&
                                  nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    .OrderByDescending(nic => nic.Speed)
                    .ToList();

                if (nics.Any())
                {
                    var mac = nics.First().GetPhysicalAddress().ToString();
                    if (!string.IsNullOrWhiteSpace(mac))
                        return mac;
                }
            }
            catch { }

            return "UNKNOWN_MAC";
        }

        /// <summary>
        /// Obtiene el número de serie del disco duro principal
        /// </summary>
        private static string GetDiskSerialNumber()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_PhysicalMedia"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        var serial = obj["SerialNumber"]?.ToString()?.Trim();
                        if (!string.IsNullOrWhiteSpace(serial))
                            return serial;
                    }
                }
            }
            catch { }

            return "UNKNOWN_DISK";
        }

        /// <summary>
        /// Obtiene un fingerprint legible para mostrar al usuario (primeros 16 caracteres)
        /// </summary>
        public static string GetDisplayFingerprint()
        {
            var fingerprint = GetFingerprint();
            return fingerprint.Substring(0, Math.Min(16, fingerprint.Length));
        }
    }
}
