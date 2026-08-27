using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// Optimizaciones avanzadas integradas desde KernelOS Toolbox
    /// Incluye gestión de TextInputHost, redirección de TaskManager (ProcessExplorer),
    /// opciones de HopLimit de red y tweaks de VBS/HVCI.
    /// </summary>
    public static class KernelOSToolboxTweaks
    {
        private const string IFEO_TEXTINPUTHOST = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\TextInputHost.exe";
        private const string IFEO_TASKMGR = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\taskmgr.exe";
        private const string TCPIP_PARAMS = @"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters";
        private const string DEVICE_GUARD_HVCI = @"SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity";

        #region TextInputHost Optimization

        /// <summary>
        /// Deshabilita o mitiga interrupciones en segundo plano del TextInputHost.exe (panel táctil/teclado en pantalla).
        /// </summary>
        public static bool DisableTextInputHost()
        {
            try
            {
                using (var key = Registry.LocalMachine.CreateSubKey(IFEO_TEXTINPUTHOST))
                {
                    key?.SetValue("Debugger", "systray.exe", RegistryValueKind.String);
                }
                Debug.WriteLine("✓ TextInputHost mitigado correctamente.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error al deshabilitar TextInputHost: {ex.Message}");
                return false;
            }
        }

        public static bool EnableTextInputHost()
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(IFEO_TEXTINPUTHOST, writable: true))
                {
                    key?.DeleteValue("Debugger", throwOnMissingValue: false);
                }
                Debug.WriteLine("✓ TextInputHost restaurado.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error al restaurar TextInputHost: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Network HopLimit (DefaultTTL)

        /// <summary>
        /// Ajusta el HopLimit (DefaultTTL) en la pila TCP/IP de Windows.
        /// </summary>
        /// <param name="ttl">Valor TTL deseado (ej. 64, 128).</param>
        public static bool SetHopLimit(int ttl)
        {
            try
            {
                using (var key = Registry.LocalMachine.CreateSubKey(TCPIP_PARAMS))
                {
                    key?.SetValue("DefaultTTL", ttl, RegistryValueKind.DWord);
                }
                Debug.WriteLine($"✓ HopLimit (DefaultTTL) establecido a {ttl}.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error al ajustar HopLimit: {ex.Message}");
                return false;
            }
        }

        public static bool ResetHopLimit()
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(TCPIP_PARAMS, writable: true))
                {
                    key?.DeleteValue("DefaultTTL", throwOnMissingValue: false);
                }
                Debug.WriteLine("✓ HopLimit (DefaultTTL) restaurado a valor predeterminado.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error al restaurar HopLimit: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Task Manager Redirect (Process Explorer)

        /// <summary>
        /// Redirige la apertura de taskmgr.exe hacia Process Explorer si está instalado.
        /// </summary>
        public static bool SetTaskManagerRedirect(string processExplorerPath)
        {
            try
            {
                if (!File.Exists(processExplorerPath))
                {
                    Debug.WriteLine("❌ Process Explorer no encontrado en la ruta especificada.");
                    return false;
                }

                using (var key = Registry.LocalMachine.CreateSubKey(IFEO_TASKMGR))
                {
                    key?.SetValue("Debugger", $"\"{processExplorerPath}\"", RegistryValueKind.String);
                }
                Debug.WriteLine("✓ Redirección de TaskManager activada.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error al redirigir TaskManager: {ex.Message}");
                return false;
            }
        }

        public static bool RemoveTaskManagerRedirect()
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(IFEO_TASKMGR, writable: true))
                {
                    key?.DeleteValue("Debugger", throwOnMissingValue: false);
                }
                Debug.WriteLine("✓ Redirección de TaskManager removida.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error al remover redirección de TaskManager: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region VBS & HVCI Optimization

        /// <summary>
        /// Deshabilita HVCI (Hypervisor-Protected Code Integrity) para maximizar FPS y reducir latencia.
        /// </summary>
        public static bool DisableHVCI()
        {
            try
            {
                using (var key = Registry.LocalMachine.CreateSubKey(DEVICE_GUARD_HVCI))
                {
                    key?.SetValue("Enabled", 0, RegistryValueKind.DWord);
                }
                Debug.WriteLine("✓ HVCI deshabilitado.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error al deshabilitar HVCI: {ex.Message}");
                return false;
            }
        }

        public static bool EnableHVCI()
        {
            try
            {
                using (var key = Registry.LocalMachine.CreateSubKey(DEVICE_GUARD_HVCI))
                {
                    key?.SetValue("Enabled", 1, RegistryValueKind.DWord);
                }
                Debug.WriteLine("✓ HVCI habilitado.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error al habilitar HVCI: {ex.Message}");
                return false;
            }
        }

        #endregion
    }
}
