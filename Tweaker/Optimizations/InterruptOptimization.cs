using System;
using System.Diagnostics;
using System.Management;
using Microsoft.Win32;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// Interrupt Optimization - Configura el modo de interrupción Message Signaled Interrupts (MSI)
    /// Reduce la latencia de procesamiento de E/S al evitar conflictos en el bus PCIe (Legacy mode)
    /// </summary>
    public static class InterruptOptimization
    {
        private const string ENUM_PCI_PATH = @"SYSTEM\CurrentControlSet\Enum\";

        /// <summary>
        /// OPTIMIZACIÓN AVANZADA: MSI Mode GPU
        /// 
        /// Habilita el modo MSI (Message Signaled Interrupts) para la GPU primaria detectada.
        /// El modo MSI permite que el dispositivo envíe interrupciones como mensajes directos a través del bus PCIe 
        /// en lugar de usar líneas de señal físicas (Legacy), eliminando el overhead de arbitraje de interrupciones compartido.
        /// </summary>
        public static bool EnableMSIModeGPU()
        {
            try
            {
                Debug.WriteLine("[GHOST] INICIANDO MSI MODE GPU OPTIMIZATION");
                Debug.WriteLine("-------------------------------------------");

                // 1. Detectar GPU activa
                string? pnpDeviceId = GetPrimaryGpuPnpId();
                if (string.IsNullOrEmpty(pnpDeviceId))
                {
                    Debug.WriteLine("[GHOST] Error: No se pudo detectar la GPU primaria activa.");
                    return false;
                }

                Debug.WriteLine($"[GHOST] GPU detectada: {pnpDeviceId}");

                // 2. Localizar la ruta en el registro
                // La ruta final es HKLM\SYSTEM\CurrentControlSet\Enum\<PNPDeviceID>\Device Parameters\Interrupt Management\MessageSignaledInterruptProperties
                string msiKeyPath = $@"{ENUM_PCI_PATH}{pnpDeviceId}\Device Parameters\Interrupt Management\MessageSignaledInterruptProperties";

                using (RegistryKey? msiKey = Registry.LocalMachine.CreateSubKey(msiKeyPath))
                {
                    if (msiKey == null)
                    {
                        Debug.WriteLine($"[GHOST] Error: No se pudo crear/abrir la subclave de registro: {msiKeyPath}");
                        return false;
                    }

                    // 3. Establecer parámetros
                    // MSISupported = 1 habilita el modo MSI
                    msiKey.SetValue("MSISupported", 1, RegistryValueKind.DWord);
                    
                    // MessageNumberLimit = 1 asegura que se use una única interrupción de alta prioridad (Recomendado para Gaming)
                    msiKey.SetValue("MessageNumberLimit", 1, RegistryValueKind.DWord);

                    Debug.WriteLine("[GHOST] MSI Mode habilitado exitosamente para la GPU.");
                    Debug.WriteLine("[GHOST] Beneficios: Reducción de overhead en PCIe y latencia de interrupción.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[GHOST] Error en MSI Mode GPU: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Deshabilita el modo MSI (vuelve a Legacy Mode) para la GPU primaria.
        /// </summary>
        public static bool DisableMSIModeGPU()
        {
            try
            {
                Debug.WriteLine("[GHOST] DESHABILITANDO MSI MODE GPU");
                
                string? pnpDeviceId = GetPrimaryGpuPnpId();
                if (string.IsNullOrEmpty(pnpDeviceId)) return false;

                string msiKeyPath = $@"{ENUM_PCI_PATH}{pnpDeviceId}\Device Parameters\Interrupt Management\MessageSignaledInterruptProperties";

                using (RegistryKey? msiKey = Registry.LocalMachine.OpenSubKey(msiKeyPath, true))
                {
                    if (msiKey != null)
                    {
                        msiKey.SetValue("MSISupported", 0, RegistryValueKind.DWord);
                        Debug.WriteLine("[GHOST] MSI Mode deshabilitado (Restaurado a Legacy Mode).");
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[GHOST] Error al deshabilitar MSI Mode GPU: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Obtiene el PNPDeviceID de la GPU primaria que está actualmente en uso.
        /// </summary>
        private static string? GetPrimaryGpuPnpId()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT PNPDeviceID FROM Win32_VideoController WHERE Availability = 3"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        string? id = obj["PNPDeviceID"]?.ToString();
                        if (!string.IsNullOrEmpty(id)) return id;
                    }
                }

                // Fallback: intentar sin el filtro de disponibilidad si no se encuentra nada
                using (var searcher = new ManagementObjectSearcher("SELECT PNPDeviceID FROM Win32_VideoController"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        string? id = obj["PNPDeviceID"]?.ToString();
                        if (!string.IsNullOrEmpty(id)) return id;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[GHOST] Error al obtener PNPDeviceID de GPU: {ex.Message}");
            }
            return null;
        }
    }
}
