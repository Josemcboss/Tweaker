using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Win32;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// USB & XHCI Controller MSI (Message Signaled Interrupts) Optimization
    /// Habilita modo MSI y prioridad Alta en controladores USB para eliminar
    /// DPC latency spikes, jitter y pérdidas de polling en mouse/teclado gaming (1000Hz+).
    /// </summary>
    public static class UsbMsiOptimization
    {
        private const string PCI_ENUM_KEY = @"SYSTEM\CurrentControlSet\Enum\PCI";
        private const string USB_CLASS_GUID = "{36fc9e60-c465-11cf-8056-444553540000}";

        /// <summary>
        /// Habilita modo MSI y prioridad Alta en todos los controladores de host USB (xHCI/eHCI)
        /// </summary>
        public static bool EnableUsbMsiMode()
        {
            try
            {
                Debug.WriteLine("──────────────────────────────────────────");
                Debug.WriteLine("⚡ HABILITANDO MODO MSI EN CONTROLADORES USB");
                Debug.WriteLine("──────────────────────────────────────────");

                int modifiedCount = 0;

                using (var pciKey = Registry.LocalMachine.OpenSubKey(PCI_ENUM_KEY, true))
                {
                    if (pciKey == null)
                    {
                        Debug.WriteLine("❌ No se pudo acceder a HKLM\\SYSTEM\\CurrentControlSet\\Enum\\PCI");
                        return false;
                    }

                    foreach (string devSubKeyName in pciKey.GetSubKeyNames())
                    {
                        using (var devSubKey = pciKey.OpenSubKey(devSubKeyName, true))
                        {
                            if (devSubKey == null) continue;

                            foreach (string instSubKeyName in devSubKey.GetSubKeyNames())
                            {
                                try
                                {
                                    using (var instKey = devSubKey.OpenSubKey(instSubKeyName, true))
                                    {
                                        if (instKey == null) continue;

                                        string? classGuid = instKey.GetValue("ClassGUID")?.ToString();
                                        string? service = instKey.GetValue("Service")?.ToString();
                                        string? devDesc = instKey.GetValue("DeviceDesc")?.ToString();

                                        bool isUsbController = string.Equals(classGuid, USB_CLASS_GUID, StringComparison.OrdinalIgnoreCase) ||
                                                               (service != null && (service.Equals("USBXHCI", StringComparison.OrdinalIgnoreCase) ||
                                                                                    service.Equals("usbehci", StringComparison.OrdinalIgnoreCase)));

                                        if (!isUsbController) continue;

                                        // Crear o abrir subclave Device Parameters\Interrupt Management
                                        using (var msiProps = instKey.CreateSubKey(@"Device Parameters\Interrupt Management\MessageSignaledInterruptProperties", true))
                                        {
                                            if (msiProps != null)
                                            {
                                                msiProps.SetValue("MSISupported", 1, RegistryValueKind.DWord);
                                                msiProps.SetValue("MessageNumberLimit", 16, RegistryValueKind.DWord);
                                            }
                                        }

                                        using (var affPolicy = instKey.CreateSubKey(@"Device Parameters\Interrupt Management\Affinity Policy", true))
                                        {
                                            if (affPolicy != null)
                                            {
                                                // 3 = High Priority
                                                affPolicy.SetValue("DevicePriority", 3, RegistryValueKind.DWord);
                                            }
                                        }

                                        Debug.WriteLine($"  ✔ USB Controller configurado a MSI High: {devDesc ?? instSubKeyName}");
                                        modifiedCount++;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine($"  ⚠️ Error configurando dispositivo {instSubKeyName}: {ex.Message}");
                                }
                            }
                        }
                    }
                }

                Debug.WriteLine($"✔ Modo MSI USB aplicado exitosamente en {modifiedCount} controlador(es)");
                return modifiedCount > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error en EnableUsbMsiMode: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Restaura la configuración de MSI y prioridad predeterminada en controladores USB
        /// </summary>
        public static bool RestoreUsbMsiMode()
        {
            try
            {
                Debug.WriteLine("Restaurando configuración predeterminada de controladores USB...");
                int restoredCount = 0;

                using (var pciKey = Registry.LocalMachine.OpenSubKey(PCI_ENUM_KEY, true))
                {
                    if (pciKey == null) return false;

                    foreach (string devSubKeyName in pciKey.GetSubKeyNames())
                    {
                        using (var devSubKey = pciKey.OpenSubKey(devSubKeyName, true))
                        {
                            if (devSubKey == null) continue;

                            foreach (string instSubKeyName in devSubKey.GetSubKeyNames())
                            {
                                try
                                {
                                    using (var instKey = devSubKey.OpenSubKey(instSubKeyName, true))
                                    {
                                        if (instKey == null) continue;

                                        string? classGuid = instKey.GetValue("ClassGUID")?.ToString();
                                        string? service = instKey.GetValue("Service")?.ToString();

                                        bool isUsbController = string.Equals(classGuid, USB_CLASS_GUID, StringComparison.OrdinalIgnoreCase) ||
                                                               (service != null && (service.Equals("USBXHCI", StringComparison.OrdinalIgnoreCase) ||
                                                                                    service.Equals("usbehci", StringComparison.OrdinalIgnoreCase)));

                                        if (!isUsbController) continue;

                                        using (var affPolicy = instKey.OpenSubKey(@"Device Parameters\Interrupt Management\Affinity Policy", true))
                                        {
                                            affPolicy?.DeleteValue("DevicePriority", false);
                                        }

                                        restoredCount++;
                                    }
                                }
                                catch { }
                            }
                        }
                    }
                }

                Debug.WriteLine($"✔ Configuración USB restaurada en {restoredCount} dispositivo(s)");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error en RestoreUsbMsiMode: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Comprueba si los controladores USB tienen el modo MSI habilitado
        /// </summary>
        public static bool? IsUsbMsiEnabled()
        {
            try
            {
                using (var pciKey = Registry.LocalMachine.OpenSubKey(PCI_ENUM_KEY, false))
                {
                    if (pciKey == null) return null;

                    foreach (string devSubKeyName in pciKey.GetSubKeyNames())
                    {
                        using (var devSubKey = pciKey.OpenSubKey(devSubKeyName, false))
                        {
                            if (devSubKey == null) continue;

                            foreach (string instSubKeyName in devSubKey.GetSubKeyNames())
                            {
                                using (var instKey = devSubKey.OpenSubKey(instSubKeyName, false))
                                {
                                    if (instKey == null) continue;

                                    string? service = instKey.GetValue("Service")?.ToString();
                                    if (service != null && service.Equals("USBXHCI", StringComparison.OrdinalIgnoreCase))
                                    {
                                        using (var msiProps = instKey.OpenSubKey(@"Device Parameters\Interrupt Management\MessageSignaledInterruptProperties", false))
                                        {
                                            var val = msiProps?.GetValue("MSISupported");
                                            if (val is int intVal && intVal == 1) return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                return false;
            }
            catch
            {
                return null;
            }
        }
    }
}
