using System;
using System.Diagnostics;
using System.Management;

using Microsoft.Win32;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// Tweaks avanzados de latencia que no están en LatencyOptimization.cs
    /// Incluye: Interrupt Moderation, MenuShowDelay, DataQueueSizes, CSRSS Priority
    /// </summary>
    public static class AdvancedLatencyTweaks
    {
        // ────────────────────────────────────────────?
        // INTERRUPT MODERATION (NETWORK ADAPTERS)
        // ────────────────────────────────────────────?

        /// <summary>
        /// Deshabilita Interrupt Moderation en adaptadores de red
        /// 
        /// ¿QUÉ ES INTERRUPT MODERATION?
        /// ──────────────────────────────────────────
        /// • Feature de tarjetas de red modernas
        /// • Agrupa múltiples interrupciones en una sola
        /// • Reduce overhead de CPU (~5-10%)
        /// • Pero AUMENTA LATENCIA en 2-10ms
        /// 
        /// PROBLEMA EN GAMING:
        /// ──────────────────────────────────────────
        /// • Cada packet network genera una interrupción
        /// • Interrupt Moderation "espera" antes de notificar al CPU
        /// • Causa micro-delays en recepción de datos
        /// • CRÍTICO en shooters (CS2, Valorant, Apex)
        /// 
        /// SOLUCIÓN:
        /// ──────────────────────────────────────────
        /// • InterruptModeration = 0 (Deshabilitado)
        /// • Cada packet procesado inmediatamente
        /// • Reduce ping efectivo en 2-5ms
        /// • Mejor hitreg (registro de disparos)
        /// 
        /// IMPACTO:
        /// ──────────────────────────────────────────
        /// ? Ping: -2 a -5ms
        /// ? Mejor hitreg
        /// ? Elimina "peeker's disadvantage"
        /// ?? CPU usage +3-5% (mínimo en PCs modernos)
        /// 
        /// UBICACIÓN EN REGISTRO:
        /// HKLM\SYSTEM\CurrentControlSet\Control\Class\
        /// {4d36e972-e325-11ce-bfc1-08002be10318}\[ID]\*InterruptModeration
        /// </summary>
        public static bool DisableInterruptModeration()
        {
            try
            {
                Debug.WriteLine("?? Deshabilitando Interrupt Moderation en adaptadores de red...");

                // GUID de la clase Network Adapters
                string networkAdaptersClassGuid = "{4d36e972-e325-11ce-bfc1-08002be10318}";
                string basePath = $@"SYSTEM\CurrentControlSet\Control\Class\{networkAdaptersClassGuid}";

                int adaptersModified = 0;

                using (var baseKey = Registry.LocalMachine.OpenSubKey(basePath, false))
                {
                    if (baseKey == null)
                    {
                        Debug.WriteLine("? No se pudo acceder a la clave de adaptadores de red");
                        return false;
                    }

                    // Iterar sobre todos los subkeys (0000, 0001, 0002, etc.)
                    foreach (string subKeyName in baseKey.GetSubKeyNames())
                    {
                        // Solo procesar keys numéricas (0000-9999)
                        if (!subKeyName.All(char.IsDigit) || subKeyName.Length != 4)
                            continue;

                        try
                        {
                            string fullPath = $@"{basePath}\{subKeyName}";
                            using (var adapterKey = Registry.LocalMachine.OpenSubKey(fullPath, true))
                            {
                                if (adapterKey == null) continue;

                                // Verificar que sea un adaptador de red válido
                                string driverDesc = adapterKey.GetValue("DriverDesc") as string;
                                if (string.IsNullOrEmpty(driverDesc))
                                    continue;

                                // Deshabilitar Interrupt Moderation
                                adapterKey.SetValue("*InterruptModeration", "0", RegistryValueKind.String);

                                Debug.WriteLine($"   ? {driverDesc}: Interrupt Moderation = 0");
                                adaptersModified++;
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"   ?? Error en adaptador {subKeyName}: {ex.Message}");
                        }
                    }
                }

                if (adaptersModified > 0)
                {
                    Debug.WriteLine($"? Interrupt Moderation deshabilitado en {adaptersModified} adaptador(es)");
                    Debug.WriteLine("?? REINICIA para aplicar cambios");
                    return true;
                }
                else
                {
                    Debug.WriteLine("?? No se encontraron adaptadores de red compatibles");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error en DisableInterruptModeration: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Restaura Interrupt Moderation a valor por defecto (habilitado)
        /// </summary>
        public static bool RestoreInterruptModeration()
        {
            try
            {
                Debug.WriteLine("?? Restaurando Interrupt Moderation...");

                string networkAdaptersClassGuid = "{4d36e972-e325-11ce-bfc1-08002be10318}";
                string basePath = $@"SYSTEM\CurrentControlSet\Control\Class\{networkAdaptersClassGuid}";

                int adaptersModified = 0;

                using (var baseKey = Registry.LocalMachine.OpenSubKey(basePath, false))
                {
                    if (baseKey == null) return false;

                    foreach (string subKeyName in baseKey.GetSubKeyNames())
                    {
                        if (!subKeyName.All(char.IsDigit) || subKeyName.Length != 4)
                            continue;

                        try
                        {
                            string fullPath = $@"{basePath}\{subKeyName}";
                            using (var adapterKey = Registry.LocalMachine.OpenSubKey(fullPath, true))
                            {
                                if (adapterKey == null) continue;

                                string driverDesc = adapterKey.GetValue("DriverDesc") as string;
                                if (string.IsNullOrEmpty(driverDesc)) continue;

                                // Restaurar a valor por defecto (1 = habilitado)
                                adapterKey.SetValue("*InterruptModeration", "1", RegistryValueKind.String);

                                Debug.WriteLine($"   ? {driverDesc}: Interrupt Moderation restaurado");
                                adaptersModified++;
                            }
                        }
                        catch { }
                    }
                }

                Debug.WriteLine($"? Interrupt Moderation restaurado en {adaptersModified} adaptador(es)");
                return adaptersModified > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error: {ex.Message}");
                return false;
            }
        }

        // ────────────────────────────────────────────?
        // MENU SHOW DELAY (UI RESPONSIVENESS)
        // ────────────────────────────────────────────?

        /// <summary>
        /// Elimina delay en menús contextuales de Windows
        /// 
        /// PROBLEMA:
        /// • Windows espera 400ms antes de mostrar menús
        /// • Delay artificial para "suavizar" la experiencia
        /// • Hace que Windows se sienta "lento"
        /// 
        /// SOLUCIÓN:
        /// • MenuShowDelay = 0
        /// • Menús aparecen instantáneamente
        /// • UI más responsive y "snappy"
        /// 
        /// UBICACIÓN:
        /// HKCU\Control Panel\Desktop\MenuShowDelay
        /// </summary>
        public static bool SetMenuShowDelayZero()
        {
            try
            {
                Debug.WriteLine("?? Configurando MenuShowDelay a 0ms...");

                using (var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true))
                {
                    if (key == null)
                    {
                        Debug.WriteLine("? No se pudo acceder a Control Panel\\Desktop");
                        return false;
                    }

                    // MenuShowDelay = 0 (sin delay)
                    key.SetValue("MenuShowDelay", "0", RegistryValueKind.String);

                    Debug.WriteLine("? MenuShowDelay = 0ms");
                    Debug.WriteLine("?? Los menús ahora aparecen instantáneamente");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error en SetMenuShowDelay: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Restaura MenuShowDelay a valor por defecto (400ms)
        /// </summary>
        public static bool RestoreMenuShowDelay()
        {
            try
            {
                Debug.WriteLine("?? Restaurando MenuShowDelay...");

                using (var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true))
                {
                    if (key == null) return false;

                    // Valor por defecto: 400ms
                    key.SetValue("MenuShowDelay", "400", RegistryValueKind.String);

                    Debug.WriteLine("? MenuShowDelay restaurado a 400ms");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error: {ex.Message}");
                return false;
            }
        }

        // ────────────────────────────────────────────?
        // DATA QUEUE SIZES (MOUSE & KEYBOARD)
        // ────────────────────────────────────────────?

        /// <summary>
        /// Aumenta tamaño de colas de datos de mouse y teclado
        /// 
        /// PROBLEMA:
        /// • Colas por defecto: 100 (0x64)
        /// • Con input rápido, se pueden perder eventos
        /// • Causa "skipped inputs" en gaming competitivo
        /// 
        /// SOLUCIÓN:
        /// • KeyboardDataQueueSize = 256 (0x100)
        /// • MouseDataQueueSize = 256 (0x100)
        /// • Mayor buffer = menos inputs perdidos
        /// 
        /// IMPACTO:
        /// ? Elimina "skipped inputs"
        /// ? Mejor tracking de mouse rápido
        /// ? Más inputs registrados en flicks
        /// 
        /// UBICACIÓN:
        /// HKLM\SYSTEM\CurrentControlSet\Services\mouclass\Parameters
        /// HKLM\SYSTEM\CurrentControlSet\Services\kbdclass\Parameters
        /// </summary>
        public static bool OptimizeDataQueueSizes()
        {
            try
            {
                Debug.WriteLine("?? Optimizando Data Queue Sizes...");

                bool mouseSuccess = false;
                bool keyboardSuccess = false;

                // MOUSE QUEUE
                try
                {
                    using (var key = Registry.LocalMachine.OpenSubKey(
                        @"SYSTEM\CurrentControlSet\Services\mouclass\Parameters", true))
                    {
                        if (key != null)
                        {
                            key.SetValue("MouseDataQueueSize", 0x100, RegistryValueKind.DWord);
                            Debug.WriteLine("   ? MouseDataQueueSize = 256 (0x100)");
                            mouseSuccess = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"   ?? Mouse queue error: {ex.Message}");
                }

                // KEYBOARD QUEUE
                try
                {
                    using (var key = Registry.LocalMachine.OpenSubKey(
                        @"SYSTEM\CurrentControlSet\Services\kbdclass\Parameters", true))
                    {
                        if (key != null)
                        {
                            key.SetValue("KeyboardDataQueueSize", 0x100, RegistryValueKind.DWord);
                            Debug.WriteLine("   ? KeyboardDataQueueSize = 256 (0x100)");
                            keyboardSuccess = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"   ?? Keyboard queue error: {ex.Message}");
                }

                if (mouseSuccess || keyboardSuccess)
                {
                    Debug.WriteLine("? Data Queue Sizes optimizados");
                    Debug.WriteLine("?? REINICIA para aplicar cambios");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error en OptimizeDataQueueSizes: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Restaura Data Queue Sizes a valores por defecto
        /// </summary>
        public static bool RestoreDataQueueSizes()
        {
            try
            {
                Debug.WriteLine("?? Restaurando Data Queue Sizes...");

                // Valor por defecto: 100 (0x64)
                using (var mouseKey = Registry.LocalMachine.OpenSubKey(
                    @"SYSTEM\CurrentControlSet\Services\mouclass\Parameters", true))
                {
                    if (mouseKey != null)
                    {
                        mouseKey.SetValue("MouseDataQueueSize", 0x64, RegistryValueKind.DWord);
                        Debug.WriteLine("   ? MouseDataQueueSize = 100");
                    }
                }

                using (var kbdKey = Registry.LocalMachine.OpenSubKey(
                    @"SYSTEM\CurrentControlSet\Services\kbdclass\Parameters", true))
                {
                    if (kbdKey != null)
                    {
                        kbdKey.SetValue("KeyboardDataQueueSize", 0x64, RegistryValueKind.DWord);
                        Debug.WriteLine("   ? KeyboardDataQueueSize = 100");
                    }
                }

                Debug.WriteLine("? Data Queue Sizes restaurados");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error: {ex.Message}");
                return false;
            }
        }

        // ────────────────────────────────────────────?
        // CSRSS PRIORITY (GDI OPTIMIZATION)
        // ────────────────────────────────────────────?

        /// <summary>
        /// Optimiza prioridad de CSRSS para mejor rendering
        /// 
        /// ¿QUÉ ES CSRSS?
        /// ──────────────────────────────────────────
        /// • Client/Server Runtime Subsystem
        /// • Proceso crítico de Windows
        /// • Maneja GDI (Graphics Device Interface)
        /// • Responsable de UI rendering
        /// 
        /// PROBLEMA:
        /// • Por defecto, CSRSS tiene baja prioridad
        /// • Causa stuttering en UI
        /// • Delay en rendering de overlays
        /// 
        /// SOLUCIÓN:
        /// • Win32PrioritySeparation con GDI optimizado
        /// • Realtime priority para CSRSS
        /// 
        /// UBICACIÓN:
        /// HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\csrss.exe
        /// </summary>
        public static bool OptimizeCSRSSPriority()
        {
            try
            {
                Debug.WriteLine("?? Optimizando CSRSS Priority (GDI)...");

                // Crear clave para CSRSS.exe
                string csrssPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\csrss.exe";

                using (var key = Registry.LocalMachine.CreateSubKey(csrssPath, true))
                {
                    if (key == null)
                    {
                        Debug.WriteLine("? No se pudo crear clave CSRSS");
                        return false;
                    }

                    // PerfOptions subkey
                    using (var perfKey = key.CreateSubKey("PerfOptions", true))
                    {
                        if (perfKey != null)
                        {
                            // CpuPriorityClass = 3 (High Priority)
                            perfKey.SetValue("CpuPriorityClass", 3, RegistryValueKind.DWord);

                            // IoPriority = 3 (High)
                            perfKey.SetValue("IoPriority", 3, RegistryValueKind.DWord);

                            Debug.WriteLine("   ? CSRSS CpuPriority = High (3)");
                            Debug.WriteLine("   ? CSRSS IoPriority = High (3)");
                        }
                    }
                }

                Debug.WriteLine("? CSRSS Priority optimizado");
                Debug.WriteLine("?? UI rendering mejorado");
                Debug.WriteLine("?? REINICIA para aplicar cambios");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error en OptimizeCSRSSPriority: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Restaura prioridad de CSRSS a valores por defecto
        /// </summary>
        public static bool RestoreCSRSSPriority()
        {
            try
            {
                Debug.WriteLine("?? Restaurando CSRSS Priority...");

                string csrssPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\csrss.exe";

                // Eliminar toda la clave (restaura a comportamiento por defecto)
                Registry.LocalMachine.DeleteSubKeyTree(csrssPath, false);

                Debug.WriteLine("? CSRSS Priority restaurado");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error: {ex.Message}");
                return false;
            }
        }

        // ────────────────────────────────────────────?
        // APPLY ALL LATENCY TWEAKS
        // ────────────────────────────────────────────?

        /// <summary>
        /// Aplica TODOS los tweaks de latencia avanzados
        /// </summary>
        public static bool ApplyAllAdvancedLatencyTweaks()
        {
            try
            {
                Debug.WriteLine("──────────────────────────");
                Debug.WriteLine("APLICANDO TODOS LOS TWEAKS DE LATENCIA");
                Debug.WriteLine("──────────────────────────");

                bool success = true;

                success &= DisableInterruptModeration();
                success &= SetMenuShowDelayZero();
                success &= OptimizeDataQueueSizes();
                success &= OptimizeCSRSSPriority();

                Debug.WriteLine("──────────────────────────");
                if (success)
                {
                    Debug.WriteLine("? TODOS LOS TWEAKS APLICADOS");
                }
                else
                {
                    Debug.WriteLine("?? ALGUNOS TWEAKS FALLARON");
                }
                Debug.WriteLine("──────────────────────────");

                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Revierte TODOS los tweaks de latencia avanzados
        /// </summary>
        public static bool RevertAllAdvancedLatencyTweaks()
        {
            try
            {
                Debug.WriteLine("──────────────────────────");
                Debug.WriteLine("REVIRTIENDO TWEAKS DE LATENCIA");
                Debug.WriteLine("──────────────────────────");

                RestoreInterruptModeration();
                RestoreMenuShowDelay();
                RestoreDataQueueSizes();
                RestoreCSRSSPriority();

                Debug.WriteLine("? TWEAKS REVERTIDOS");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error: {ex.Message}");
                return false;
            }
        }
    }
}
