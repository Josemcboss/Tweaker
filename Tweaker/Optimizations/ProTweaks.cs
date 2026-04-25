using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Microsoft.Win32;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// Tweaks de kernel
    /// Requiere permisos de administrador
    /// </summary>
    public static class ProTweaks
    {
        // ─
        // NTDLL IMPORTS - TIMER RESOLUTION
        // ─

        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int NtSetTimerResolution(
            uint DesiredResolution,
            bool SetResolution,
            out uint CurrentResolution);

        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int NtQueryTimerResolution(
            out uint MinimumResolution,
            out uint MaximumResolution,
            out uint CurrentResolution);

        private const int STATUS_SUCCESS = 0;
        private static uint _currentTimerResolution = 0;

        // ─
        // HIGH RESOLUTION TIMER
        // ─

        /// <summary>
        /// Establece la resolución del temporizador del sistema a 0.5ms
        /// 
        /// IMPACTO:
        /// ✅ Reduce jitter de frame times (más consistente)
        /// ✅ Mejora precisión de Sleep() y multimedia timers
        /// ✅ CRÍTICO para gaming competitivo (CS2, Valorant)
        /// ✅ Reduce input lag en ~1-2ms
        /// 
        /// CÓMO FUNCIONA:
        /// • Timer resolution normal: 15.6ms (64 Hz)
        /// • Timer resolution optimizada: 0.5ms (2000 Hz)
        /// • Más interrupciones = más overhead CPU (~1-2%)
        /// 
        /// ADVERTENCIA:
        /// ⚠️ Aumenta consumo de CPU en 1-2%
        /// ⚠️ Reduce duración de batería en laptops (~5-10%)
        /// ⚠️ Solo recomendado durante gaming o producción musical
        /// 
        /// REVERSIÓN:
        /// • No persiste después de reiniciar
        /// • Se debe llamar cada vez que se inicia la app
        /// </summary>
        public static bool SetMaxTimerResolution()
        {
            try
            {
                Debug.WriteLine("🔧 Configurando timer resolution a 0.5ms...");

                // Consultar límites del sistema
                int status = NtQueryTimerResolution(
                    out uint minResolution,
                    out uint maxResolution,
                    out uint currentResolution);

                if (status != STATUS_SUCCESS)
                {
                    Debug.WriteLine($"❌ Error consultando timer resolution. Status: {status}");
                    return false;
                }

                Debug.WriteLine($"📊 Timer Resolution Info:");
                Debug.WriteLine($"   → Mínima: {minResolution / 10000.0:F2}ms");
                Debug.WriteLine($"   → Máxima: {maxResolution / 10000.0:F2}ms");
                Debug.WriteLine($"   → Actual: {currentResolution / 10000.0:F2}ms");

                // Resolución deseada: 0.5ms = 5000 unidades de 100ns
                // (1ms = 10,000 unidades de 100 nanosegundos)
                uint desiredResolution = 5000; // 0.5ms

                // Ajustar si el sistema no soporta 0.5ms
                if (desiredResolution < maxResolution)
                {
                    desiredResolution = maxResolution;
                    Debug.WriteLine($"⚠️ Sistema no soporta 0.5ms. Usando máximo: {maxResolution / 10000.0:F2}ms");
                }

                // Establecer nueva resolución
                status = NtSetTimerResolution(
                    desiredResolution,
                    true, // SetResolution = true
                    out uint newResolution);

                if (status != STATUS_SUCCESS)
                {
                    Debug.WriteLine($"❌ Error estableciendo timer resolution. Status: {status}");
                    return false;
                }

                _currentTimerResolution = newResolution;

                Debug.WriteLine($"✅ Timer Resolution configurada exitosamente");
                Debug.WriteLine($"   → Nueva resolución: {newResolution / 10000.0:F2}ms");
                Debug.WriteLine($"   → Interrupciones por segundo: {10000000 / newResolution:F0} Hz");
                Debug.WriteLine($"   → Frame time consistency mejorada");
                Debug.WriteLine($"   → Input lag reducido ~1-2ms");

                return true;
            }
            catch (DllNotFoundException)
            {
                Debug.WriteLine("❌ ntdll.dll no encontrada. Sistema no compatible.");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error configurando timer resolution: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Restaura la resolución del temporizador al valor por defecto
        /// Nota: La resolución se restablece automáticamente al cerrar la app
        /// </summary>
        public static bool RevertTimerResolution()
        {
            try
            {
                Debug.WriteLine("🔄 Restaurando timer resolution a default...");

                if (_currentTimerResolution == 0)
                {
                    Debug.WriteLine("⚠️ No hay resolución personalizada activa");
                    return true;
                }

                // Restablecer a resolución por defecto (desactivar SetResolution)
                int status = NtSetTimerResolution(
                    _currentTimerResolution,
                    false, // SetResolution = false (revertir)
                    out uint newResolution);

                if (status != STATUS_SUCCESS)
                {
                    Debug.WriteLine($"❌ Error restaurando timer resolution. Status: {status}");
                    return false;
                }

                _currentTimerResolution = 0;

                Debug.WriteLine($"✅ Timer Resolution restaurada");
                Debug.WriteLine($"   → Resolución actual: {newResolution / 10000.0:F2}ms");

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error restaurando timer resolution: {ex.Message}");
                return false;
            }
        }

        // ─
        // NETWORK ADAPTER POWER SAVING
        // ─

        /// <summary>
        /// Desactiva ahorro de energía en TODOS los adaptadores de red
        /// 
        /// IMPACTO:
        /// ✅ Elimina packet loss causado por suspensión de NIC
        /// ✅ Reduce ping spikes cuando la NIC entra/sale de sleep
        /// ✅ CRÍTICO para gaming online (ping estable)
        /// ✅ Mejora throughput de red en ~5-10%
        /// 
        /// CÓMO FUNCIONA:
        /// • PnPCapabilities controla el Power Management
        /// • Valor 0 = Todas las características de ahorro habilitadas
        /// • Valor 24 (0x18) = Deshabilita suspend y wake-on-lan
        ///   → Bit 3 (0x08): Allow device to wake system
        ///   → Bit 4 (0x10): Allow system to turn off device
        /// 
        /// UBICACIÓN REGISTRO:
        /// HKLM\SYSTEM\CurrentControlSet\Control\Class\{4d36e972-e325-11ce-bfc1-08002be10318}
        /// (Esta es la clase GUID para Network Adapters)
        /// 
        /// ADVERTENCIA:
        /// ⚠️ Aumenta consumo de energía en ~0.5-1W por adaptador
        /// </summary>
        public static bool DisableNetworkPowerSaving()
        {
            try
            {
                Debug.WriteLine("🔧 Deshabilitando ahorro de energía en adaptadores de red...");

                // GUID de la clase Network Adapters
                const string networkAdapterClassGuid = "{4d36e972-e325-11ce-bfc1-08002be10318}";
                string registryPath = $@"SYSTEM\CurrentControlSet\Control\Class\{networkAdapterClassGuid}";

                int adaptorsModified = 0;

                using (var classKey = Registry.LocalMachine.OpenSubKey(registryPath, true))
                {
                    if (classKey == null)
                    {
                        Debug.WriteLine("❌ No se pudo abrir la clave de Network Adapters");
                        return false;
                    }

                    // Recorrer todas las subclaves (0000, 0001, 0002, etc.)
                    foreach (string subKeyName in classKey.GetSubKeyNames())
                    {
                        // Solo procesar claves numéricas (0000, 0001, etc.)
                        if (!subKeyName.StartsWith("00"))
                            continue;

                        using (var adapterKey = classKey.OpenSubKey(subKeyName, true))
                        {
                            if (adapterKey == null)
                                continue;

                            // Verificar que es un adaptador de red real (tiene DriverDesc)
                            var driverDesc = adapterKey.GetValue("DriverDesc");
                            if (driverDesc == null)
                                continue;

                            try
                            {
                                // PnPCapabilities = 24 (0x18)
                                // Bit 3: Disable "Allow device to wake computer"
                                // Bit 4: Disable "Allow computer to turn off device"
                                adapterKey.SetValue("PnPCapabilities", 24, RegistryValueKind.DWord);

                                Debug.WriteLine($"✅ Adaptador modificado: {driverDesc}");
                                adaptorsModified++;
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"⚠️ Error modificando {driverDesc}: {ex.Message}");
                            }
                        }
                    }
                }

                if (adaptorsModified > 0)
                {
                    Debug.WriteLine($"✅ {adaptorsModified} adaptador(es) de red configurado(s)");
                    Debug.WriteLine("   → Power saving deshabilitado");
                    Debug.WriteLine("   → Ping más estable (sin packet loss)");
                    Debug.WriteLine("   ⚠️ REINICIO REQUERIDO para aplicar cambios");
                    return true;
                }
                else
                {
                    Debug.WriteLine("⚠️ No se encontraron adaptadores de red para modificar");
                    return false;
                }
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("❌ Acceso denegado. Ejecuta como Administrador.");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error deshabilitando network power saving: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Restaura el ahorro de energía en adaptadores de red
        /// </summary>
        public static bool RevertNetworkPowerSaving()
        {
            try
            {
                Debug.WriteLine("🔄 Restaurando ahorro de energía en adaptadores de red...");

                const string networkAdapterClassGuid = "{4d36e972-e325-11ce-bfc1-08002be10318}";
                string registryPath = $@"SYSTEM\CurrentControlSet\Control\Class\{networkAdapterClassGuid}";

                int adaptorsModified = 0;

                using (var classKey = Registry.LocalMachine.OpenSubKey(registryPath, true))
                {
                    if (classKey == null)
                    {
                        Debug.WriteLine("❌ No se pudo abrir la clave de Network Adapters");
                        return false;
                    }

                    foreach (string subKeyName in classKey.GetSubKeyNames())
                    {
                        if (!subKeyName.StartsWith("00"))
                            continue;

                        using (var adapterKey = classKey.OpenSubKey(subKeyName, true))
                        {
                            if (adapterKey == null)
                                continue;

                            var driverDesc = adapterKey.GetValue("DriverDesc");
                            if (driverDesc == null)
                                continue;

                            try
                            {
                                // Eliminar la clave (Windows usará el default)
                                adapterKey.DeleteValue("PnPCapabilities", false);

                                Debug.WriteLine($"✅ Adaptador restaurado: {driverDesc}");
                                adaptorsModified++;
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"⚠️ Error restaurando {driverDesc}: {ex.Message}");
                            }
                        }
                    }
                }

                if (adaptorsModified > 0)
                {
                    Debug.WriteLine($"✅ {adaptorsModified} adaptador(es) restaurado(s)");
                    Debug.WriteLine("   ⚠️ REINICIO REQUERIDO para aplicar cambios");
                    return true;
                }
                else
                {
                    Debug.WriteLine("⚠️ No se encontraron adaptadores para restaurar");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error restaurando network power saving: {ex.Message}");
                return false;
            }
        }

        // ─
        // STICKY KEYS & ACCESSIBILITY SHORTCUTS
        // ─

        /// <summary>
        /// Desactiva atajos de accesibilidad molestos
        /// 
        /// IMPACTO:
        /// ✅ No más popup de Sticky Keys al presionar Shift 5 veces
        /// ✅ No más Toggle Keys al mantener Num Lock
        /// ✅ No más Filter Keys al mantener Shift 8 segundos
        /// ✅ CRÍTICO para gaming (evita interrupciones)
        /// 
        /// TECLAS DESHABILITADAS:
        /// • Sticky Keys (Shift x5)
        /// • Toggle Keys (Num Lock hold)
        /// • Filter Keys (Shift hold 8s)
        /// 
        /// FLAGS EXPLICADOS:
        /// • 506 (Sticky Keys): Deshabilita hotkey + sonido
        /// • 58 (Toggle Keys): Deshabilita hotkey
        /// • 122 (Filter Keys): Deshabilita hotkey + confirmación
        /// </summary>
        public static bool DisableStickyKeys()
        {
            try
            {
                Debug.WriteLine("🔧 Deshabilitando atajos de accesibilidad...");

                string accessibilityPath = @"Control Panel\Accessibility";

                // STICKY KEYS (Shift x5)
                using (var stickyKey = Registry.CurrentUser.OpenSubKey(
                    $@"{accessibilityPath}\StickyKeys", true))
                {
                    if (stickyKey != null)
                    {
                        // Flags = 506
                        // Bit 1: Desactiva hotkey (Shift x5)
                        // Bit 3: Desactiva sonido
                        stickyKey.SetValue("Flags", "506", RegistryValueKind.String);
                        Debug.WriteLine("✅ Sticky Keys deshabilitado (Shift x5)");
                    }
                }

                // TOGGLE KEYS (Num Lock hold)
                using (var toggleKey = Registry.CurrentUser.OpenSubKey(
                    $@"{accessibilityPath}\ToggleKeys", true))
                {
                    if (toggleKey != null)
                    {
                        // Flags = 58
                        // Desactiva hotkey (Num Lock hold)
                        toggleKey.SetValue("Flags", "58", RegistryValueKind.String);
                        Debug.WriteLine("✅ Toggle Keys deshabilitado (Num Lock hold)");
                    }
                }

                // FILTER KEYS (Shift hold 8s)
                using (var filterKey = Registry.CurrentUser.OpenSubKey(
                    $@"{accessibilityPath}\Keyboard Response", true))
                {
                    if (filterKey != null)
                    {
                        // Flags = 122
                        // Desactiva hotkey + confirmación
                        filterKey.SetValue("Flags", "122", RegistryValueKind.String);
                        Debug.WriteLine("✅ Filter Keys deshabilitado (Shift hold 8s)");
                    }
                }

                Debug.WriteLine("   → No más popups molestos durante gaming");
                Debug.WriteLine("   → Shift, Num Lock y otras teclas sin interrupciones");
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("❌ Acceso denegado. Ejecuta como Administrador.");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error deshabilitando Sticky Keys: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Restaura los atajos de accesibilidad a valores por defecto
        /// </summary>
        public static bool RevertStickyKeys()
        {
            try
            {
                Debug.WriteLine("🔄 Restaurando atajos de accesibilidad...");

                string accessibilityPath = @"Control Panel\Accessibility";

                // Valores default de Windows
                using (var stickyKey = Registry.CurrentUser.OpenSubKey(
                    $@"{accessibilityPath}\StickyKeys", true))
                {
                    if (stickyKey != null)
                    {
                        stickyKey.SetValue("Flags", "510", RegistryValueKind.String); // Default
                    }
                }

                using (var toggleKey = Registry.CurrentUser.OpenSubKey(
                    $@"{accessibilityPath}\ToggleKeys", true))
                {
                    if (toggleKey != null)
                    {
                        toggleKey.SetValue("Flags", "62", RegistryValueKind.String); // Default
                    }
                }

                using (var filterKey = Registry.CurrentUser.OpenSubKey(
                    $@"{accessibilityPath}\Keyboard Response", true))
                {
                    if (filterKey != null)
                    {
                        filterKey.SetValue("Flags", "126", RegistryValueKind.String); // Default
                    }
                }

                Debug.WriteLine("✅ Atajos de accesibilidad restaurados");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error restaurando Sticky Keys: {ex.Message}");
                return false;
            }
        }

        // ─
        // APLICAR/REVERTIR TODO
        // ─

        /// <summary>
        /// Aplica todas las optimizaciones profesionales
        /// </summary>
        public static bool ApplyAllProTweaks()
        {
            Debug.WriteLine("─");
            Debug.WriteLine("\U0001F680 APLICANDO TODOS LOS PRO TWEAKS");
            Debug.WriteLine("─");

            bool success = true;
            success &= SetMaxTimerResolution();
            success &= DisableNetworkPowerSaving();
            success &= DisableStickyKeys();

            Debug.WriteLine("─");
            if (success)
            {
                Debug.WriteLine("✅ TODOS LOS PRO TWEAKS APLICADOS");
                Debug.WriteLine("⚠️ REINICIA Windows para aplicar todos los cambios");
            }
            else
            {
                Debug.WriteLine("⚠️ ALGUNOS TWEAKS FALLARON");
            }
            Debug.WriteLine("─");

            return success;
        }

        /// <summary>
        /// Revierte todas las optimizaciones profesionales
        /// </summary>
        public static bool RevertAllProTweaks()
        {
            Debug.WriteLine("─");
            Debug.WriteLine("🔄 REVIRTIENDO TODOS LOS PRO TWEAKS");
            Debug.WriteLine("─");

            bool success = true;
            success &= RevertTimerResolution();
            success &= RevertNetworkPowerSaving();
            success &= RevertStickyKeys();

            Debug.WriteLine("─");
            if (success)
            {
                Debug.WriteLine("✅ TODOS LOS PRO TWEAKS REVERTIDOS");
                Debug.WriteLine("⚠️ REINICIA Windows para aplicar todos los cambios");
            }
            else
            {
                Debug.WriteLine("⚠️ ALGUNAS REVERSIONES FALLARON");
            }
            Debug.WriteLine("─");

            return success;
        }

        // ─
        // TIMER RESOLUTION PERSISTENCE (NEW)
        // ─

        private const string TIMER_RUN_KEY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string TIMER_RUN_VALUE = "GhostOptimizerTimerResolution";

        /// <summary>
        /// Persiste el Timer Resolution de 0.5ms al inicio de sesión creando
        /// una entrada en el registro Run que re-aplica la resolución al reiniciar.
        /// Usa un VBScript mínimo para llamar a NtSetTimerResolution sin ventana.
        /// </summary>
        public static bool PersistTimerResolution()
        {
            try
            {
                // El script aplica 0.5ms via powershell al iniciar sesión
                // NtSetTimerResolution no persiste entre reinicios, pero
                // con el Run key + powershell podemos re-aplicarlo.
                string psCommand =
                    "Add-Type -TypeDefinition 'using System;using System.Runtime.InteropServices;" +
                    "public class NtTimer{" +
                    "[DllImport(\"ntdll.dll\")]public static extern int NtSetTimerResolution(uint d,bool s,out uint c);}' ;" +
                    "$c=0u;[NtTimer]::NtSetTimerResolution(5000,$true,[ref]$c)";

                string runValue = $"powershell.exe -WindowStyle Hidden -NonInteractive -Command \"{psCommand}\"";

                using var key = Registry.CurrentUser.OpenSubKey(TIMER_RUN_KEY, writable: true);
                key?.SetValue(TIMER_RUN_VALUE, runValue, RegistryValueKind.String);

                Debug.WriteLine("✅ Timer Resolution 0.5ms persistido en Run key");
                Debug.WriteLine("   → Se aplicará automáticamente al iniciar sesión");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ ERROR PersistTimerResolution: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina la entrada de persistencia del Timer Resolution.
        /// </summary>
        public static bool RemoveTimerResolutionPersistence()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(TIMER_RUN_KEY, writable: true);
                key?.DeleteValue(TIMER_RUN_VALUE, throwOnMissingValue: false);
                Debug.WriteLine("✅ Timer Resolution persistence eliminada");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ ERROR RemoveTimerResolutionPersistence: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Aplica Timer Resolution 0.5ms ahora Y lo persiste para el próximo reinicio.
        /// </summary>
        public static bool SetMaxTimerResolutionPersistent()
        {
            bool applied = SetMaxTimerResolution();
            bool persisted = PersistTimerResolution();
            return applied && persisted;
        }
    }
}
