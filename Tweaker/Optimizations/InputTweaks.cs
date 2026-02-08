using Microsoft.Win32;
using System;
using System.Diagnostics;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// INPUT & USB OPTIMIZATION for COMPETITIVE GAMING
    /// Tweaks avanzados para reducir latencia de input y optimizar dispositivos USB
    /// 
    /// ADVERTENCIAS:
    /// - Win32PrioritySeparation cambia cómo el CPU prioriza las ventanas
    /// - Estos tweaks son para gaming competitivo extremo
    /// - Algunos cambios requieren REINICIO
    /// </summary>
    public static class InputTweaks
    {
        #region Registry Keys Constants
        
        private const string USB_SERVICE_KEY = @"SYSTEM\CurrentControlSet\Services\USB";
        private const string MOUSE_CLASS_KEY = @"SYSTEM\CurrentControlSet\Services\mouclass\Parameters";
        private const string KEYBOARD_CLASS_KEY = @"SYSTEM\CurrentControlSet\Services\kbdclass\Parameters";
        private const string FTH_KEY = @"SOFTWARE\Microsoft\FTH";
        private const string PRIORITY_CONTROL_KEY = @"SYSTEM\CurrentControlSet\Control\PriorityControl";
        
        #endregion

        /// <summary>
        /// OPTIMIZAR USB para GAMING COMPETITIVO
        /// Deshabilita suspensión selectiva USB para eliminar micro-interrupciones
        /// </summary>
        public static bool OptimizeUSB()
        {
            try
            {
                Debug.WriteLine("???????????????????????????????????????????????????????????");
                Debug.WriteLine("OPTIMIZING USB FOR COMPETITIVE GAMING");
                Debug.WriteLine("???????????????????????????????????????????????????????????");

                // 1. Deshabilitar USB Selective Suspend
                using (var key = Registry.LocalMachine.CreateSubKey(USB_SERVICE_KEY))
                {
                    if (key != null)
                    {
                        // DisableSelectiveSuspend = 1 (Deshabilitado)
                        key.SetValue("DisableSelectiveSuspend", 1, RegistryValueKind.DWord);
                        Debug.WriteLine("? USB Selective Suspend DESHABILITADO");
                        Debug.WriteLine("   ?? DisableSelectiveSuspend = 1");
                        Debug.WriteLine("   ?? Elimina micro-interrupciones USB");
                        Debug.WriteLine("   ?? Mouse/Teclado 1000Hz sin pérdidas");
                    }
                }

                Debug.WriteLine("");
                Debug.WriteLine("?? BENEFICIOS USB OPTIMIZATION:");
                Debug.WriteLine("   • Elimina micro-interrupciones de dispositivos USB");
                Debug.WriteLine("   • Mouse/Teclado gaming sin pérdida de polling");
                Debug.WriteLine("   • Reduce DPC latency en controladores USB");
                Debug.WriteLine("   • Consistencia en devices de alta frecuencia (1000Hz+)");
                Debug.WriteLine("   • Elimina 'dormidas' de dispositivos durante gaming");
                Debug.WriteLine("");
                Debug.WriteLine("?? NOTA: Pequeño aumento en consumo energético USB");
                Debug.WriteLine("?? NOTA: Para gaming competitivo vale la pena");

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? ERROR en OptimizeUSB: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// REVERTIR optimizaciones USB a valores por defecto
        /// </summary>
        public static bool RevertUSB()
        {
            try
            {
                Debug.WriteLine("Revirtiendo optimizaciones USB...");

                using (var key = Registry.LocalMachine.CreateSubKey(USB_SERVICE_KEY))
                {
                    if (key != null)
                    {
                        // Eliminar valor personalizado (vuelve al default del sistema)
                        key.DeleteValue("DisableSelectiveSuspend", false);
                        Debug.WriteLine("? USB Selective Suspend restaurado a default");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? ERROR en RevertUSB: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// OPTIMIZAR INPUT QUEUES (Mouse/Teclado Buffers)
        /// Aumenta buffers para evitar pérdida de inputs en polling rates altos
        /// </summary>
        public static bool OptimizeInputQueues()
        {
            try
            {
                Debug.WriteLine("???????????????????????????????????????????????????????????");
                Debug.WriteLine("OPTIMIZING INPUT QUEUES FOR HIGH POLLING RATES");
                Debug.WriteLine("???????????????????????????????????????????????????????????");

                // 1. Optimizar Mouse Data Queue
                using (var key = Registry.LocalMachine.CreateSubKey(MOUSE_CLASS_KEY))
                {
                    if (key != null)
                    {
                        // MouseDataQueueSize = 1000 (default es ~100)
                        key.SetValue("MouseDataQueueSize", 1000, RegistryValueKind.DWord);
                        Debug.WriteLine("? Mouse Data Queue Size OPTIMIZADO");
                        Debug.WriteLine("   ?? MouseDataQueueSize = 1000 (era ~100)");
                        Debug.WriteLine("   ?? Soporta 1000Hz+ sin pérdida de datos");
                        Debug.WriteLine("   ?? Perfecto para gaming competitivo");
                    }
                }

                // 2. Optimizar Keyboard Data Queue
                using (var key = Registry.LocalMachine.CreateSubKey(KEYBOARD_CLASS_KEY))
                {
                    if (key != null)
                    {
                        // KeyboardDataQueueSize = 200 (default es ~100)
                        key.SetValue("KeyboardDataQueueSize", 200, RegistryValueKind.DWord);
                        Debug.WriteLine("? Keyboard Data Queue Size OPTIMIZADO");
                        Debug.WriteLine("   ?? KeyboardDataQueueSize = 200 (era ~100)");
                        Debug.WriteLine("   ?? Evita pérdida de keystrokes rápidos");
                        Debug.WriteLine("   ? Mejor para spam de teclas");
                    }
                }

                Debug.WriteLine("");
                Debug.WriteLine("?? BENEFICIOS INPUT QUEUES:");
                Debug.WriteLine("   • Cero pérdida de inputs con devices 1000Hz+");
                Debug.WriteLine("   • Mejor handling de spam de teclas (WASD, etc.)");
                Debug.WriteLine("   • Reduce micro-stutters en movement");
                Debug.WriteLine("   • Elimina 'input loss' en moments intensos");
                Debug.WriteLine("   • Crítico para FPS competitivos (CS2, Valorant)");
                Debug.WriteLine("");
                Debug.WriteLine("?? REQUIERE REINICIO para efecto completo");

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? ERROR en OptimizeInputQueues: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// REVERTIR Input Queues a valores por defecto
        /// </summary>
        public static bool RevertInputQueues()
        {
            try
            {
                Debug.WriteLine("Revirtiendo Input Queues...");

                // Revertir Mouse Queue
                using (var key = Registry.LocalMachine.CreateSubKey(MOUSE_CLASS_KEY))
                {
                    if (key != null)
                    {
                        key.DeleteValue("MouseDataQueueSize", false);
                        Debug.WriteLine("? Mouse Data Queue Size restaurado");
                    }
                }

                // Revertir Keyboard Queue
                using (var key = Registry.LocalMachine.CreateSubKey(KEYBOARD_CLASS_KEY))
                {
                    if (key != null)
                    {
                        key.DeleteValue("KeyboardDataQueueSize", false);
                        Debug.WriteLine("? Keyboard Data Queue Size restaurado");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? ERROR en RevertInputQueues: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// DESHABILITAR FAULT TOLERANT HEAP (FTH)
        /// Evita que Windows intercepte memoria de juegos para "protegerlos"
        /// </summary>
        public static bool DisableFTH()
        {
            try
            {
                Debug.WriteLine("???????????????????????????????????????????????????????????");
                Debug.WriteLine("DISABLING FAULT TOLERANT HEAP (FTH)");
                Debug.WriteLine("???????????????????????????????????????????????????????????");

                using (var key = Registry.LocalMachine.CreateSubKey(FTH_KEY))
                {
                    if (key != null)
                    {
                        // Enabled = 0 (Deshabilitado)
                        key.SetValue("Enabled", 0, RegistryValueKind.DWord);
                        Debug.WriteLine("? Fault Tolerant Heap (FTH) DESHABILITADO");
                        Debug.WriteLine("   ?? Enabled = 0");
                        Debug.WriteLine("   ?? Windows NO interceptará memoria de juegos");
                        Debug.WriteLine("   ?? Elimina overhead de 'protección' automática");
                        Debug.WriteLine("   ? Memory allocation más rápido en games");
                    }
                }

                Debug.WriteLine("");
                Debug.WriteLine("?? BENEFICIOS DISABLE FTH:");
                Debug.WriteLine("   • Elimina overhead de memory protection automática");
                Debug.WriteLine("   • Memory allocation más directa para juegos");
                Debug.WriteLine("   • Reduce micro-stutters por interceptación de memoria");
                Debug.WriteLine("   • Mejor frame times consistency");
                Debug.WriteLine("   • Windows no 'ayuda' los juegos innecesariamente");
                Debug.WriteLine("");
                Debug.WriteLine("?? ADVERTENCIA: Menos protección automática de crashes");
                Debug.WriteLine("?? NOTA: Para juegos estables, el beneficio vale la pena");

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? ERROR en DisableFTH: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// REVERTIR FTH a configuración por defecto
        /// </summary>
        public static bool RevertFTH()
        {
            try
            {
                Debug.WriteLine("Revirtiendo FTH...");

                using (var key = Registry.LocalMachine.CreateSubKey(FTH_KEY))
                {
                    if (key != null)
                    {
                        // Restaurar a habilitado (default Windows)
                        key.SetValue("Enabled", 1, RegistryValueKind.DWord);
                        Debug.WriteLine("? Fault Tolerant Heap restaurado (habilitado)");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? ERROR en RevertFTH: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// CONFIGURAR WIN32 PRIORITY SEPARATION
        /// Optimiza cómo el CPU prioriza las ventanas de foreground vs background
        /// </summary>
        public static bool SetWin32Priority()
        {
            try
            {
                Debug.WriteLine("???????????????????????????????????????????????????????????");
                Debug.WriteLine("SETTING WIN32 PRIORITY SEPARATION FOR GAMING");
                Debug.WriteLine("???????????????????????????????????????????????????????????");

                using (var key = Registry.LocalMachine.CreateSubKey(PRIORITY_CONTROL_KEY))
                {
                    if (key != null)
                    {
                        // Win32PrioritySeparation = 38 (0x26 hex)
                        // Valor balanceado para gaming competitivo
                        key.SetValue("Win32PrioritySeparation", 38, RegistryValueKind.DWord);
                        Debug.WriteLine("? Win32 Priority Separation OPTIMIZADO");
                        Debug.WriteLine("   ?? Win32PrioritySeparation = 38 (0x26)");
                        Debug.WriteLine("   ?? Foreground apps (juegos) priorizadas");
                        Debug.WriteLine("   ?? Background apps menos agresivas");
                        Debug.WriteLine("   ? CPU time slices optimizadas para gaming");
                    }
                }

                Debug.WriteLine("");
                Debug.WriteLine("?? EXPLICACIÓN WIN32 PRIORITY:");
                Debug.WriteLine("   • Controla cómo CPU divide tiempo entre procesos");
                Debug.WriteLine("   • Valor 38 = Balance gaming óptimo");
                Debug.WriteLine("   • Foreground (juego) recibe más CPU time");
                Debug.WriteLine("   • Background apps son menos intrusivas");
                Debug.WriteLine("   • Reduce interrupciones durante gaming");
                Debug.WriteLine("");
                Debug.WriteLine("?? BENEFICIOS:");
                Debug.WriteLine("   • Juegos reciben prioridad CPU consistente");
                Debug.WriteLine("   • Menos micro-stutters por context switching");
                Debug.WriteLine("   • Background tasks menos disruptivas");
                Debug.WriteLine("   • Frame times más estables");
                Debug.WriteLine("");
                Debug.WriteLine("?? CRÍTICO: Este valor cambia scheduling de TODO el sistema");
                Debug.WriteLine("?? NOTA: 38 es balanceado, no extremo");

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? ERROR en SetWin32Priority: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// CONFIGURAR WIN32 PRIORITY SEPARATION con valor personalizado
        /// Optimiza cómo el CPU prioriza las ventanas de foreground vs background
        /// </summary>
        /// <param name="value">Valor DWORD para Win32PrioritySeparation</param>
        public static bool SetWin32Priority(int value)
        {
            try
            {
                Debug.WriteLine("???????????????????????????????????????????????????????????");
                Debug.WriteLine($"SETTING WIN32 PRIORITY SEPARATION = {value}");
                Debug.WriteLine("???????????????????????????????????????????????????????????");

                using (var key = Registry.LocalMachine.CreateSubKey(PRIORITY_CONTROL_KEY))
                {
                    if (key != null)
                    {
                        key.SetValue("Win32PrioritySeparation", value, RegistryValueKind.DWord);
                        Debug.WriteLine($"? Win32 Priority Separation = {value} (0x{value:X})");
                        Debug.WriteLine("   ?? CPU scheduling mode actualizado");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? ERROR en SetWin32Priority: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// ESTABLECER PERFIL DE CPU SCHEDULING
        /// Perfiles optimizados para diferentes tipos de gaming
        /// </summary>
        /// <param name="profile">Nombre del perfil: "Balanced", "Smooth", "Aggressive", "Default"</param>
        public static bool SetPriorityProfile(string profile)
        {
            try
            {
                Debug.WriteLine("????????????????????????????????????????????????????????????");
                Debug.WriteLine($"?? APLICANDO PERFIL CPU SCHEDULING: {profile.ToUpper()}");
                Debug.WriteLine("????????????????????????????????????????????????????????????");

                int priorityValue;
                string description;
                string benefits;

                switch (profile.ToLower())
                {
                    case "balanced":
                        priorityValue = 38; // 0x26
                        description = "Balance óptimo gaming/sistema";
                        benefits = "• Foreground apps priorizadas moderadamente\n" +
                                  "   • Background apps siguen funcionando bien\n" +
                                  "   • Perfecto para gaming + streaming";
                        break;

                    case "smooth":
                        priorityValue = 40; // 0x28
                        description = "Máxima suavidad y frame times";
                        benefits = "• Time slices más largos = menos context switches\n" +
                                  "   • Frame times ultra-consistentes\n" +
                                  "   • Ideal para juegos single-player exigentes";
                        break;

                    case "aggressive":
                        priorityValue = 22; // 0x16
                        description = "Máxima responsividad competitiva";
                        benefits = "• Foreground app recibe TODO el CPU\n" +
                                  "   • Time slices cortos = respuesta instantánea\n" +
                                  "   • Perfecto para FPS competitivos (CS2, Valorant)";
                        break;

                    case "default":
                        priorityValue = 2;
                        description = "Valor por defecto de Windows";
                        benefits = "• Comportamiento estándar de Windows\n" +
                                  "   • Sin optimizaciones específicas\n" +
                                  "   • Balance general del sistema";
                        break;

                    default:
                        Debug.WriteLine($"? Perfil desconocido: {profile}");
                        return false;
                }

                using (var key = Registry.LocalMachine.CreateSubKey(PRIORITY_CONTROL_KEY))
                {
                    if (key != null)
                    {
                        key.SetValue("Win32PrioritySeparation", priorityValue, RegistryValueKind.DWord);
                        
                        Debug.WriteLine($"? PERFIL APLICADO: {profile.ToUpper()}");
                        Debug.WriteLine($"   ?? Win32PrioritySeparation = {priorityValue} (0x{priorityValue:X})");
                        Debug.WriteLine($"   ?? {description}");
                        Debug.WriteLine("");
                        Debug.WriteLine("?? CARACTERÍSTICAS:");
                        Debug.WriteLine($"{benefits}");
                        Debug.WriteLine("");
                        
                        // Explicación técnica según el perfil
                        switch (profile.ToLower())
                        {
                            case "balanced":
                                Debug.WriteLine("?? PERFIL BALANCED (38):");
                                Debug.WriteLine("   • Foreground boost: MEDIO");
                                Debug.WriteLine("   • Time slice: VARIABLE (balanceado)");
                                Debug.WriteLine("   • Background penalty: MODERADO");
                                break;
                            case "smooth":
                                Debug.WriteLine("?? PERFIL SMOOTH (40):");
                                Debug.WriteLine("   • Foreground boost: ALTO");
                                Debug.WriteLine("   • Time slice: LARGO (menos switches)");
                                Debug.WriteLine("   • Background penalty: ALTO");
                                break;
                            case "aggressive":
                                Debug.WriteLine("?? PERFIL AGGRESSIVE (22):");
                                Debug.WriteLine("   • Foreground boost: EXTREMO");
                                Debug.WriteLine("   • Time slice: CORTO (respuesta rápida)");
                                Debug.WriteLine("   • Background penalty: MÁXIMO");
                                break;
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? ERROR en SetPriorityProfile: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// OBTENER PERFIL ACTUAL DE CPU SCHEDULING
        /// Detecta qué perfil está actualmente configurado
        /// </summary>
        public static string GetCurrentPriorityProfile()
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(PRIORITY_CONTROL_KEY))
                {
                    var currentValue = key?.GetValue("Win32PrioritySeparation") as int? ?? 2;

                    return currentValue switch
                    {
                        38 => "Balanced",
                        40 => "Smooth",
                        22 => "Aggressive",
                        2 => "Default",
                        _ => $"Custom ({currentValue})"
                    };
                }
            }
            catch
            {
                return "Unknown";
            }
        }

        /// <summary>
        /// REVERTIR Win32 Priority a valor por defecto de Windows
        /// </summary>
        public static bool RevertWin32Priority()
        {
            return SetPriorityProfile("Default");
        }

        /// <summary>
        /// APLICAR TODAS LAS OPTIMIZACIONES INPUT & USB
        /// </summary>
        public static bool ApplyAllInputOptimizations()
        {
            try
            {
                Debug.WriteLine("????????????????????????????????????????????????????????????");
                Debug.WriteLine("?? APLICANDO TODAS LAS OPTIMIZACIONES INPUT & USB ??");
                Debug.WriteLine("????????????????????????????????????????????????????????????");

                bool usbResult = OptimizeUSB();
                bool queueResult = OptimizeInputQueues();
                bool fthResult = DisableFTH();
                bool priorityResult = SetWin32Priority();

                bool allSuccess = usbResult && queueResult && fthResult && priorityResult;

                Debug.WriteLine("");
                Debug.WriteLine("?? RESUMEN INPUT & USB OPTIMIZATIONS:");
                Debug.WriteLine($"   USB Optimization: {(usbResult ? "? ÉXITO" : "? FALLO")}");
                Debug.WriteLine($"   Input Queues: {(queueResult ? "? ÉXITO" : "? FALLO")}");
                Debug.WriteLine($"   FTH Disabled: {(fthResult ? "? ÉXITO" : "? FALLO")}");
                Debug.WriteLine($"   Win32 Priority: {(priorityResult ? "? ÉXITO" : "? FALLO")}");
                Debug.WriteLine("");

                if (allSuccess)
                {
                    Debug.WriteLine("?? TODAS LAS OPTIMIZACIONES INPUT APLICADAS EXITOSAMENTE!");
                    Debug.WriteLine("?? REINICIA Windows para efecto completo en Input Queues");
                }
                else
                {
                    Debug.WriteLine("?? ALGUNAS optimizaciones fallaron. Revisar logs arriba.");
                }

                return allSuccess;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? ERROR CRÍTICO en ApplyAllInputOptimizations: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// REVERTIR TODAS LAS OPTIMIZACIONES INPUT & USB
        /// </summary>
        public static bool RevertAllInputOptimizations()
        {
            try
            {
                Debug.WriteLine("????????????????????????????????????????????????????????????");
                Debug.WriteLine("?? REVIRTIENDO TODAS LAS OPTIMIZACIONES INPUT & USB ??");
                Debug.WriteLine("????????????????????????????????????????????????????????????");

                bool usbResult = RevertUSB();
                bool queueResult = RevertInputQueues();
                bool fthResult = RevertFTH();
                bool priorityResult = RevertWin32Priority();

                bool allSuccess = usbResult && queueResult && fthResult && priorityResult;

                Debug.WriteLine("");
                Debug.WriteLine("?? RESUMEN REVERSIÓN INPUT & USB:");
                Debug.WriteLine($"   USB Revertido: {(usbResult ? "? ÉXITO" : "? FALLO")}");
                Debug.WriteLine($"   Input Queues Revertidos: {(queueResult ? "? ÉXITO" : "? FALLO")}");
                Debug.WriteLine($"   FTH Habilitado: {(fthResult ? "? ÉXITO" : "? FALLO")}");
                Debug.WriteLine($"   Win32 Priority Revertido: {(priorityResult ? "? ÉXITO" : "? FALLO")}");

                if (allSuccess)
                {
                    Debug.WriteLine("? TODAS las optimizaciones INPUT revertidas exitosamente");
                }
                else
                {
                    Debug.WriteLine("?? ALGUNAS reversiones fallaron. Revisar logs.");
                }

                return allSuccess;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? ERROR CRÍTICO en RevertAllInputOptimizations: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// DIAGNÓSTICO COMPLETO - INPUT & USB STATUS
        /// </summary>
        public static string DiagnoseInputOptimizations()
        {
            try
            {
                var diagnosis = "???????????????????????????????????????????????????????????\n" +
                               "?? DIAGNÓSTICO INPUT & USB OPTIMIZATIONS\n" +
                               "???????????????????????????????????????????????????????????\n\n";

                // 1. USB Selective Suspend Status
                try
                {
                    using (var key = Registry.LocalMachine.OpenSubKey(USB_SERVICE_KEY))
                    {
                        if (key?.GetValue("DisableSelectiveSuspend") is int usbValue && usbValue == 1)
                        {
                            diagnosis += "?? USB Selective Suspend: ? DESHABILITADO (Óptimo)\n";
                            diagnosis += "   ?? Sin interrupciones USB durante gaming\n";
                        }
                        else
                        {
                            diagnosis += "?? USB Selective Suspend: ? HABILITADO (Default Windows)\n";
                            diagnosis += "   ?? Puede causar micro-interrupciones en devices gaming\n";
                        }
                    }
                }
                catch
                {
                    diagnosis += "?? USB Selective Suspend: ? ERROR al leer configuración\n";
                }

                diagnosis += "\n";

                // 2. Mouse Queue Status
                try
                {
                    using (var key = Registry.LocalMachine.OpenSubKey(MOUSE_CLASS_KEY))
                    {
                        var mouseQueue = key?.GetValue("MouseDataQueueSize") as int? ?? 100;
                        if (mouseQueue >= 1000)
                        {
                            diagnosis += $"??? Mouse Data Queue: ? OPTIMIZADO ({mouseQueue})\n";
                            diagnosis += "   ?? Soporta 1000Hz+ sin pérdida de datos\n";
                        }
                        else
                        {
                            diagnosis += $"??? Mouse Data Queue: ? DEFAULT ({mouseQueue})\n";
                            diagnosis += "   ?? Puede perder inputs con polling rates altos\n";
                        }
                    }
                }
                catch
                {
                    diagnosis += "??? Mouse Data Queue: ? ERROR al leer configuración\n";
                }

                // 3. Keyboard Queue Status
                try
                {
                    using (var key = Registry.LocalMachine.OpenSubKey(KEYBOARD_CLASS_KEY))
                    {
                        var kbdQueue = key?.GetValue("KeyboardDataQueueSize") as int? ?? 100;
                        if (kbdQueue >= 200)
                        {
                            diagnosis += $"?? Keyboard Data Queue: ? OPTIMIZADO ({kbdQueue})\n";
                            diagnosis += "   ?? Maneja spam de teclas sin pérdida\n";
                        }
                        else
                        {
                            diagnosis += $"?? Keyboard Data Queue: ? DEFAULT ({kbdQueue})\n";
                            diagnosis += "   ?? Puede perder keystrokes en spam intenso\n";
                        }
                    }
                }
                catch
                {
                    diagnosis += "?? Keyboard Data Queue: ? ERROR al leer configuración\n";
                }

                diagnosis += "\n";

                // 4. FTH Status
                try
                {
                    using (var key = Registry.LocalMachine.OpenSubKey(FTH_KEY))
                    {
                        var fthEnabled = key?.GetValue("Enabled") as int? ?? 1;
                        if (fthEnabled == 0)
                        {
                            diagnosis += "??? Fault Tolerant Heap: ? DESHABILITADO (Óptimo gaming)\n";
                            diagnosis += "   ?? Memory allocation directa para juegos\n";
                        }
                        else
                        {
                            diagnosis += "??? Fault Tolerant Heap: ? HABILITADO (Default Windows)\n";
                            diagnosis += "   ?? Overhead de protección automática en memoria\n";
                        }
                    }
                }
                catch
                {
                    diagnosis += "??? Fault Tolerant Heap: ? ERROR al leer configuración\n";
                }

                // 5. Win32 Priority Status
                try
                {
                    using (var key = Registry.LocalMachine.OpenSubKey(PRIORITY_CONTROL_KEY))
                    {
                        var priority = key?.GetValue("Win32PrioritySeparation") as int? ?? 2;
                        if (priority == 38)
                        {
                            diagnosis += "?? Win32 Priority Separation: ? OPTIMIZADO (38)\n";
                            diagnosis += "   ?? Foreground apps priorizadas para gaming\n";
                        }
                        else if (priority == 2)
                        {
                            diagnosis += "?? Win32 Priority Separation: ? DEFAULT WINDOWS (2)\n";
                            diagnosis += "   ?? Sin priorización especial para foreground\n";
                        }
                        else
                        {
                            diagnosis += $"?? Win32 Priority Separation: ? CUSTOM ({priority})\n";
                            diagnosis += "   ?? Valor personalizado diferente al default\n";
                        }
                    }
                }
                catch
                {
                    diagnosis += "?? Win32 Priority Separation: ? ERROR al leer configuración\n";
                }

                diagnosis += "\n????????????????????????????????????????????????????????????\n";
                diagnosis += "?? RECOMENDACIONES PARA GAMING COMPETITIVO:\n";
                diagnosis += "????????????????????????????????????????????????????????????\n";
                diagnosis += "• USB: DESHABILITADO = Elimina micro-lag en devices\n";
                diagnosis += "• Mouse Queue: 1000+ = Soporta 1000Hz sin pérdidas\n";
                diagnosis += "• Keyboard Queue: 200+ = Spam de teclas sin pérdida\n";
                diagnosis += "• FTH: DESHABILITADO = Memory allocation más rápida\n";
                diagnosis += "• Win32 Priority: 38 = Balance óptimo gaming/sistema\n\n";
                diagnosis += "?? GHOST OPTIMIZER - Input & USB Analysis Complete!";

                Debug.WriteLine(diagnosis);
                return diagnosis;
            }
            catch (Exception ex)
            {
                var errorDiag = $"? ERROR durante diagnóstico Input & USB: {ex.Message}";
                Debug.WriteLine(errorDiag);
                return errorDiag;
            }
        }

        // ???????????????????????????????????????????????????????????????????
        // NUEVAS FUNCIONALIDADES AVANZADAS - MPO & CPU SCHEDULING
        // ???????????????????????????????????????????????????????????????????

        #region MPO (Multiplane Overlay) Fix

        private const string DWM_KEY = @"SOFTWARE\Microsoft\Windows\Dwm";

        /// <summary>
        /// DESHABILITAR MPO (MULTIPLANE OVERLAY) - FIX ANTI-FLICKER
        /// 
        /// QUÉ ES MPO:
        /// - Multiplane Overlay es una tecnología de composición de Windows 10/11
        /// - Permite que diferentes capas de video se rendericen directamente en hardware
        /// - Diseñado para mejorar eficiencia energética y rendimiento
        /// 
        /// PROBLEMA EN GAMING:
        /// - Causa stuttering y pantallazos negros en muchos sistemas
        /// - Problemas con overlays (Discord, Steam, OBS)
        /// - Frame pacing inconsistente
        /// - Incompatibilidad con G-Sync/FreeSync en algunos casos
        /// 
        /// SOLUCIÓN:
        /// - Establecer OverlayTestMode = 5 fuerza modo legacy
        /// - Elimina problemas de composición MPO
        /// - Mejora consistencia de frame times
        /// 
        /// IMPACTO:
        /// ? Elimina stuttering causado por MPO
        /// ? Sin pantallazos negros al cambiar ventanas
        /// ? Overlays funcionan sin problemas
        /// ? Frame pacing más consistente
        /// </summary>
        public static bool DisableMPO()
        {
            try
            {
                Debug.WriteLine("???????????????????????????????????????????");
                Debug.WriteLine("DISABLING MPO (MULTIPLANE OVERLAY) - ANTI-FLICKER FIX");
                Debug.WriteLine("???????????????????????????????????????????");

                using (var key = Registry.LocalMachine.CreateSubKey(DWM_KEY))
                {
                    if (key != null)
                    {
                        // OverlayTestMode = 5 (Legacy mode, MPO disabled)
                        key.SetValue("OverlayTestMode", 5, RegistryValueKind.DWord);
                        
                        Debug.WriteLine("? MPO (Multiplane Overlay) DESHABILITADO");
                        Debug.WriteLine($"   ?? Ruta: HKLM\\{DWM_KEY}");
                        Debug.WriteLine("   ?? OverlayTestMode = 5 (Legacy mode)");
                        Debug.WriteLine("   ?? Modo legacy forzado para compatibilidad");
                        Debug.WriteLine("");
                        Debug.WriteLine("?? BENEFICIOS ESPERADOS:");
                        Debug.WriteLine("   ? Eliminación de stuttering por MPO");
                        Debug.WriteLine("   ? Sin pantallazos negros al Alt+Tab");
                        Debug.WriteLine("   ? Overlays (Discord, OBS) sin problemas");
                        Debug.WriteLine("   ? Frame pacing más consistente");
                        Debug.WriteLine("   ? Mejor compatibilidad G-Sync/FreeSync");
                        Debug.WriteLine("");
                        Debug.WriteLine("?? REQUIERE REINICIO para efecto completo");
                        
                        return true;
                    }
                }
                
                Debug.WriteLine("? ERROR: No se pudo acceder a la clave DWM");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? ERROR en DisableMPO: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// RESTAURAR MPO (MULTIPLANE OVERLAY) - VOLVER A DEFAULT
        /// 
        /// Elimina la configuración OverlayTestMode para restaurar
        /// el comportamiento predeterminado de Windows (MPO habilitado)
        /// 
        /// CUÁNDO USAR:
        /// - Si experimentas problemas después de deshabilitar MPO
        /// - Si tu sistema funciona mejor con MPO habilitado
        /// - Para volver al comportamiento original de Windows
        /// </summary>
        public static bool RestoreMPO()
        {
            try
            {
                Debug.WriteLine("???????????????????????????????????????????");
                Debug.WriteLine("RESTORING MPO (MULTIPLANE OVERLAY) - BACK TO DEFAULT");
                Debug.WriteLine("???????????????????????????????????????????");

                using (var key = Registry.LocalMachine.OpenSubKey(DWM_KEY, true))
                {
                    if (key != null)
                    {
                        try
                        {
                            // Eliminar el valor OverlayTestMode (restaura default de Windows)
                            key.DeleteValue("OverlayTestMode", false);
                            
                            Debug.WriteLine("? MPO (Multiplane Overlay) RESTAURADO");
                            Debug.WriteLine($"   ?? Ruta: HKLM\\{DWM_KEY}");
                            Debug.WriteLine("   ?? OverlayTestMode eliminado (Windows default)");
                            Debug.WriteLine("   ?? Windows manejará MPO automáticamente");
                            Debug.WriteLine("");
                            Debug.WriteLine("?? RESTAURADO A COMPORTAMIENTO ORIGINAL:");
                            Debug.WriteLine("   • MPO habilitado por defecto");
                            Debug.WriteLine("   • Windows decide cuándo usar MPO");
                            Debug.WriteLine("   • Posible retorno de stuttering si había problemas");
                            Debug.WriteLine("");
                            Debug.WriteLine("?? REQUIERE REINICIO para efecto completo");
                        }
                        catch (ArgumentException)
                        {
                            // El valor ya no existe, está bien
                            Debug.WriteLine("? MPO ya está en configuración por defecto");
                            Debug.WriteLine("   OverlayTestMode no existe (comportamiento normal)");
                        }
                        
                        return true;
                    }
                }
                
                Debug.WriteLine("? ERROR: No se pudo acceder a la clave DWM");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? ERROR en RestoreMPO: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// VERIFICAR ESTADO ACTUAL DEL MPO
        /// </summary>
        public static string GetMPOStatus()
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(DWM_KEY, false))
                {
                    if (key != null)
                    {
                        object value = key.GetValue("OverlayTestMode");
                        if (value == null)
                        {
                            return "Windows Default (MPO Habilitado)";
                        }
                        else
                        {
                            int mpoValue = Convert.ToInt32(value);
                            return mpoValue == 5 ? "Deshabilitado (Legacy Mode)" : $"Configuración Custom ({mpoValue})";
                        }
                    }
                }
                return "No se pudo determinar";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        #endregion

        #region CPU Scheduling Profiles (Win32PrioritySeparation)

        /// <summary>
        /// CONFIGURAR PERFIL DE PRIORIDAD CPU (Win32PrioritySeparation)
        /// 
        /// Este valor controla cómo Windows distribuye tiempo de CPU entre procesos:
        /// - Foreground vs Background processes
        /// - Quantum length (duración de time slices)
        /// - Thread priority boost
        /// 
        /// PERFILES DISPONIBLES:
        /// 
        /// BALANCED (38/0x26):
        /// - Balance óptimo para gaming
        /// - Foreground apps priorizadas moderadamente
        /// - Good responsiveness + multitasking
        /// 
        /// SMOOTH (40/0x28):
        /// - Time slices más largos
        /// - Mejor para streaming/recording
        /// - Menos cambios de contexto
        /// 
        /// AGGRESSIVE (22/0x16):
        /// - Máxima prioridad a foreground
        /// - Gaming competitivo extremo
        /// - Puede afectar multitasking
        /// 
        /// DEFAULT (2):
        /// - Configuración original de Windows
        /// - Balanced general del sistema
        /// </summary>
        public static bool SetCpuPriorityProfile(string profile)
        {
            try
            {
                Debug.WriteLine("???????????????????????????????????????????");
                Debug.WriteLine($"SETTING CPU PRIORITY PROFILE: {profile.ToUpper()}");
                Debug.WriteLine("???????????????????????????????????????????");

                int profileValue;
                string description;
                string benefits;
                string warning = "";

                switch (profile.ToLower())
                {
                    case "balanced":
                        profileValue = 38; // 0x26
                        description = "Balance óptimo para gaming";
                        benefits = 
                            "? Excelente para gaming en general\n" +
                            "? Buen balance rendimiento/multitasking\n" +
                            "? Foreground apps priorizadas moderadamente\n" +
                            "? Responsividad mejorada sin sacrificar estabilidad";
                        break;

                    case "smooth":
                        profileValue = 40; // 0x28
                        description = "Time slices largos para suavidad";
                        benefits = 
                            "? Ideal para streaming y recording\n" +
                            "? Menos cambios de contexto\n" +
                            "? Frame pacing más suave\n" +
                            "? Mejor para cargas de trabajo sostenidas";
                        break;

                    case "aggressive":
                        profileValue = 22; // 0x16
                        description = "Máxima prioridad a procesos foreground";
                        benefits = 
                            "? Máximo rendimiento para gaming competitivo\n" +
                            "? Latencia mínima para aplicación activa\n" +
                            "? Time slices cortos y agresivos\n" +
                            "? Ideal para esports";
                        warning = "?? Puede afectar multitasking intensivo";
                        break;

                    case "default":
                        profileValue = 2;
                        description = "Valor por defecto de Windows";
                        benefits = 
                            "? Comportamiento estándar de Windows\n" +
                            "? Sin optimizaciones específicas\n" +
                            "? Balance general del sistema\n" +
                            "? Revierte cualquier cambio previo";
                        warning = "?? Restaura configuración original";
                        break;

                    default:
                        Debug.WriteLine($"? ERROR: Perfil '{profile}' no válido");
                        Debug.WriteLine("   Perfiles válidos: balanced, smooth, aggressive, default");
                        return false;
                }

                // Aplicar la configuración
                using (var key = Registry.LocalMachine.CreateSubKey(PRIORITY_CONTROL_KEY))
                {
                    if (key != null)
                    {
                        key.SetValue("Win32PrioritySeparation", profileValue, RegistryValueKind.DWord);
                        
                        Debug.WriteLine($"? PERFIL CPU '{profile.ToUpper()}' APLICADO");
                        Debug.WriteLine($"   ?? Ruta: HKLM\\{PRIORITY_CONTROL_KEY}");
                        Debug.WriteLine($"   ?? Win32PrioritySeparation = {profileValue} (0x{profileValue:X2})");
                        Debug.WriteLine($"   ?? {description}");
                        Debug.WriteLine("");
                        Debug.WriteLine("?? BENEFICIOS:");
                        foreach (var line in benefits.Split('\n'))
                        {
                            if (!string.IsNullOrWhiteSpace(line))
                                Debug.WriteLine($"   {line}");
                        }
                        
                        if (!string.IsNullOrEmpty(warning))
                        {
                            Debug.WriteLine("");
                            Debug.WriteLine($"   {warning}");
                        }
                        
                        Debug.WriteLine("");
                        Debug.WriteLine("?? REQUIERE REINICIO para efecto completo");
                        
                        return true;
                    }
                }
                
                Debug.WriteLine("? ERROR: No se pudo acceder a la clave PriorityControl");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? ERROR en SetCpuPriorityProfile: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// OBTENER PERFIL CPU ACTUAL
        /// </summary>
        public static string GetCurrentCpuPriorityProfile()
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(PRIORITY_CONTROL_KEY, false))
                {
                    if (key != null)
                    {
                        object value = key.GetValue("Win32PrioritySeparation");
                        if (value != null)
                        {
                            int currentValue = Convert.ToInt32(value);
                            return currentValue switch
                            {
                                2 => "Default",
                                38 => "Balanced",
                                40 => "Smooth", 
                                22 => "Aggressive",
                                _ => $"Custom ({currentValue})"
                            };
                        }
                    }
                }
                return "Unknown";
            }
            catch (Exception)
            {
                return "Error";
            }
        }

        /// <summary>
        /// MÉTODOS DE CONVENIENCIA PARA PERFILES ESPECÍFICOS
        /// </summary>
        public static bool SetBalancedCpuProfile() => SetCpuPriorityProfile("balanced");
        public static bool SetSmoothCpuProfile() => SetCpuPriorityProfile("smooth");
        public static bool SetAggressiveCpuProfile() => SetCpuPriorityProfile("aggressive");
        public static bool ResetCpuProfile() => SetCpuPriorityProfile("default");

        #endregion

        // ???????????????????????????????????????????????????????????????????
        // MÉTODOS PARA COMPATIBILIDAD CON MAINWINDOW
        // ???????????????????????????????????????????????????????????????????

        /// <summary>
        /// OPTIMIZAR USB PARA GAMING (Alias para OptimizeUSB)
        /// </summary>
        public static bool OptimizeUSBForGaming()
        {
            return OptimizeUSB();
        }

        /// <summary>
        /// REVERTIR OPTIMIZACIÓN USB (Alias para RevertUSB)
        /// </summary>
        public static bool RevertUSBOptimization()
        {
            return RevertUSB();
        }

        /// <summary>
        /// DESHABILITAR FAULT TOLERANT HEAP (Alias para DisableFTH)
        /// </summary>
        public static bool DisableFaultTolerantHeap()
        {
            return DisableFTH();
        }

        /// <summary>
        /// HABILITAR FAULT TOLERANT HEAP (Alias para RevertFTH)
        /// </summary>
        public static bool EnableFaultTolerantHeap()
        {
            return RevertFTH();
        }
    }
}