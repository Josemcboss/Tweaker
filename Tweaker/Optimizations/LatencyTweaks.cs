using System;
using System.Diagnostics;

using Microsoft.Win32;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// Optimizaciones de latencia del sistema y UI
    /// Reduce delays en la interfaz y mejora responsiveness
    /// </summary>
    public static class LatencyTweaks
    {
        // ────────────────────────────────────────────?
        // CPU SCHEDULING OPTIMIZATION
        // ────────────────────────────────────────────?

        /// <summary>
        /// Optimiza el CPU scheduling para priorizar ventanas activas
        /// Win32PrioritySeparation = 38 (0x26)
        /// 
        /// IMPACTO:
        /// ? Ventanas activas (juegos) reciben más tiempo de CPU
        /// ? Reduce input lag en aplicaciones foreground
        /// ? Mejora frame consistency en gaming
        /// 
        /// DECODIFICACIÓN DEL VALOR 38 (0x26 en hex):
        /// - Bits 0-1: Short quantum (más context switches)
        /// - Bits 2-3: Variable quantum length
        /// - Bits 4-5: Foreground boost de 2:1
        /// 
        /// Resultado: Foreground apps reciben el doble de CPU time slices
        /// </summary>
        public static bool OptimizeCpuScheduling()
        {
            try
            {
                Debug.WriteLine("?? Optimizando CPU Scheduling...");

                using (var key = Registry.LocalMachine.OpenSubKey(
                    @"SYSTEM\CurrentControlSet\Control\PriorityControl", true))
                {
                    if (key == null)
                    {
                        Debug.WriteLine("? No se pudo abrir la clave de PriorityControl");
                        return false;
                    }

                    // Valor 38 (decimal) = 0x26 (hex)
                    // Optimiza para foreground applications (gaming)
                    key.SetValue("Win32PrioritySeparation", 38, RegistryValueKind.DWord);

                    Debug.WriteLine("? Win32PrioritySeparation configurado a 38");
                    Debug.WriteLine("   ? Foreground apps ahora tienen prioridad 2:1 sobre background");
                    return true;
                }
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("? Acceso denegado. Ejecuta como Administrador.");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error optimizando CPU Scheduling: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Restaura el CPU scheduling a valores por defecto de Windows
        /// Win32PrioritySeparation = 2 (default)
        /// </summary>
        public static bool RevertCpuScheduling()
        {
            try
            {
                Debug.WriteLine("?? Restaurando CPU Scheduling a default...");

                using (var key = Registry.LocalMachine.OpenSubKey(
                    @"SYSTEM\CurrentControlSet\Control\PriorityControl", true))
                {
                    if (key == null)
                    {
                        Debug.WriteLine("? No se pudo abrir la clave de PriorityControl");
                        return false;
                    }

                    // Valor 2 (default de Windows)
                    key.SetValue("Win32PrioritySeparation", 2, RegistryValueKind.DWord);

                    Debug.WriteLine("? Win32PrioritySeparation restaurado a 2 (default)");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error restaurando CPU Scheduling: {ex.Message}");
                return false;
            }
        }

        // ────────────────────────────────────────────?
        // USB POWER SAVING DISABLE
        // ────────────────────────────────────────────?

        /// <summary>
        /// Desactiva el ahorro de energía en USB
        /// 
        /// IMPACTO:
        /// ? Evita que mouse/teclado entren en suspensión
        /// ? Elimina micro-delays al reactivar dispositivos USB
        /// ? CRÍTICO para gaming (mouse sin lag)
        /// 
        /// IMPORTANTE:
        /// ?? Aumenta consumo de energía en ~0.5W por puerto USB
        /// ?? Solo recomendado para PCs de escritorio o gaming laptops
        /// </summary>
        public static bool DisableUsbPowerSaving()
        {
            try
            {
                Debug.WriteLine("?? Deshabilitando ahorro de energía USB...");

                using (var key = Registry.LocalMachine.OpenSubKey(
                    @"SYSTEM\CurrentControlSet\Services\USB", true))
                {
                    if (key == null)
                    {
                        Debug.WriteLine("? No se pudo abrir la clave de USB");
                        return false;
                    }

                    // 1 = Deshabilitar Selective Suspend
                    key.SetValue("DisableSelectiveSuspend", 1, RegistryValueKind.DWord);

                    Debug.WriteLine("? USB Selective Suspend deshabilitado");
                    Debug.WriteLine("   ? Mouse y teclado sin delays de reactivación");
                    return true;
                }
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("? Acceso denegado. Ejecuta como Administrador.");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error deshabilitando USB Power Saving: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Restaura el ahorro de energía USB
        /// </summary>
        public static bool RevertUsbPowerSaving()
        {
            try
            {
                Debug.WriteLine("?? Restaurando ahorro de energía USB...");

                using (var key = Registry.LocalMachine.OpenSubKey(
                    @"SYSTEM\CurrentControlSet\Services\USB", true))
                {
                    if (key == null)
                    {
                        Debug.WriteLine("? No se pudo abrir la clave de USB");
                        return false;
                    }

                    // 0 = Habilitar Selective Suspend (default)
                    key.SetValue("DisableSelectiveSuspend", 0, RegistryValueKind.DWord);

                    Debug.WriteLine("? USB Selective Suspend restaurado");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error restaurando USB Power Saving: {ex.Message}");
                return false;
            }
        }

        // ────────────────────────────────────────────?
        // USER INTERFACE OPTIMIZATION
        // ────────────────────────────────────────────?

        /// <summary>
        /// Optimiza delays de la interfaz de usuario
        /// 
        /// CAMBIOS:
        /// • MenuShowDelay = 0 (menús instantáneos)
        /// • WaitToKillAppTimeout = 2000 (cierre rápido de apps)
        /// • WaitToKillServiceTimeout = 2000 (cierre rápido de servicios)
        /// 
        /// IMPACTO:
        /// ? Menús contextuales aparecen al instante
        /// ? Apps cierran en 2s en vez de 5s al hacer Alt+F4
        /// ? Windows se apaga/reinicia más rápido
        /// ? UI más responsive y "snappy"
        /// 
        /// SEGURIDAD:
        /// ?? Apps que no responden se cerrarán más rápido (posible pérdida de datos)
        /// </summary>
        public static bool OptimizeUserInterface()
        {
            try
            {
                Debug.WriteLine("?? Optimizando UI delays...");

                using (var key = Registry.CurrentUser.OpenSubKey(
                    @"Control Panel\Desktop", true))
                {
                    if (key == null)
                    {
                        Debug.WriteLine("? No se pudo abrir la clave de Desktop");
                        return false;
                    }

                    // MenuShowDelay: Delay antes de mostrar menús contextuales
                    // 0 = Instantáneo (default es 400ms)
                    key.SetValue("MenuShowDelay", "0", RegistryValueKind.String);
                    Debug.WriteLine("? MenuShowDelay configurado a 0 (instantáneo)");

                    // WaitToKillAppTimeout: Tiempo de espera antes de forzar cierre de app
                    // 2000ms = 2 segundos (default es 5000ms)
                    key.SetValue("WaitToKillAppTimeout", "2000", RegistryValueKind.String);
                    Debug.WriteLine("? WaitToKillAppTimeout configurado a 2000ms");

                    // WaitToKillServiceTimeout: Tiempo de espera antes de forzar cierre de servicio
                    // 2000ms = 2 segundos (default es 5000ms)
                    key.SetValue("WaitToKillServiceTimeout", "2000", RegistryValueKind.String);
                    Debug.WriteLine("? WaitToKillServiceTimeout configurado a 2000ms");

                    Debug.WriteLine("   ? Menús contextuales instantáneos");
                    Debug.WriteLine("   ? Apps cierran en 2s en vez de 5s");
                    Debug.WriteLine("   ? UI más responsive");
                    return true;
                }
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("? Acceso denegado. Ejecuta como Administrador.");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error optimizando UI: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Restaura delays de UI a valores por defecto
        /// </summary>
        public static bool RevertUserInterface()
        {
            try
            {
                Debug.WriteLine("?? Restaurando UI delays a default...");

                using (var key = Registry.CurrentUser.OpenSubKey(
                    @"Control Panel\Desktop", true))
                {
                    if (key == null)
                    {
                        Debug.WriteLine("? No se pudo abrir la clave de Desktop");
                        return false;
                    }

                    // Valores default de Windows
                    key.SetValue("MenuShowDelay", "400", RegistryValueKind.String);
                    key.SetValue("WaitToKillAppTimeout", "5000", RegistryValueKind.String);
                    key.SetValue("WaitToKillServiceTimeout", "5000", RegistryValueKind.String);

                    Debug.WriteLine("? UI delays restaurados a valores default");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error restaurando UI: {ex.Message}");
                return false;
            }
        }

        // ────────────────────────────────────────────?
        // APLICAR/REVERTIR TODO
        // ────────────────────────────────────────────?

        /// <summary>
        /// Aplica todas las optimizaciones de latencia
        /// </summary>
        public static bool ApplyAllLatencyOptimizations()
        {
            Debug.WriteLine("──────────────────────────");
            Debug.WriteLine("?? APLICANDO TODAS LAS OPTIMIZACIONES DE LATENCIA");
            Debug.WriteLine("──────────────────────────");

            bool success = true;
            success &= OptimizeCpuScheduling();
            success &= DisableUsbPowerSaving();
            success &= OptimizeUserInterface();

            Debug.WriteLine("──────────────────────────");
            if (success)
            {
                Debug.WriteLine("? TODAS LAS OPTIMIZACIONES DE LATENCIA APLICADAS");
            }
            else
            {
                Debug.WriteLine("?? ALGUNAS OPTIMIZACIONES FALLARON");
            }
            Debug.WriteLine("──────────────────────────");

            return success;
        }

        /// <summary>
        /// Revierte todas las optimizaciones de latencia
        /// </summary>
        public static bool RevertAllLatencyOptimizations()
        {
            Debug.WriteLine("──────────────────────────");
            Debug.WriteLine("?? REVIRTIENDO TODAS LAS OPTIMIZACIONES DE LATENCIA");
            Debug.WriteLine("──────────────────────────");

            bool success = true;
            success &= RevertCpuScheduling();
            success &= RevertUsbPowerSaving();
            success &= RevertUserInterface();

            Debug.WriteLine("──────────────────────────");
            if (success)
            {
                Debug.WriteLine("? TODAS LAS OPTIMIZACIONES REVERTIDAS");
            }
            else
            {
                Debug.WriteLine("?? ALGUNAS REVERSIONES FALLARON");
            }
            Debug.WriteLine("──────────────────────────");

            return success;
        }
    }
}
