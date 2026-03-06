using System.Collections.Generic;

using Tweaker.Data;
using Tweaker.Models;

namespace Tweaker.Factories
{
    /// <summary>
    /// Factory para crear configuraciones de tweaks din�micamente
    /// Elimina hardcoding y centraliza configuraci�n
    /// </summary>
    public static class TweakFactory
    {
        /// <summary>
        /// Helper para crear un TweakModel obteniendo Risk de TweaksDatabase
        /// </summary>
        private static TweakModel CreateTweakModel(string tweakId, string title, string description, bool useApplyMode = false)
        {
            var tweakInfo = TweaksDatabase.GetTweakInfo(tweakId);

            return new TweakModel
            {
                Title = title,
                Description = description,
                IsRecommended = tweakInfo.Recommended,
                ShowInfoButton = true,
                TweakId = tweakId,
                UseApplyMode = useApplyMode,
                Risk = tweakInfo.Risk  // ? OBTENER RISK DESDE LA BASE DE DATOS
            };
        }
        /// <summary>
        /// Crea todos los tweaks para Input & Visuals
        /// </summary>
        public static TweakSectionModel CreateInputVisualsSection()
        {
            var section = new TweakSectionModel
            {
                Title = "Input & Visuals",
                Subtitle = "Optimizaciones de Input Lag y FPS",
                Icon = "??",
                Category = "Input"
            };

            section.Tweaks.Add(CreateTweakModel(
                "mouse_acceleration",
                "Desactivar Aceleraci�n del Mouse",
                "MouseSpeed = 0, Thresholds = 0\nAim 1:1 pixel perfect, muscle memory consistente"
            ));

            section.Tweaks.Add(CreateTweakModel(
                "keyboard_optimization",
                "Optimizar Teclado",
                "KeyboardDelay = 0, Input lag -50ms"
            ));

            section.Tweaks.Add(CreateTweakModel(
                "visual_effects",
                "Efectos Visuales OFF",
                "FPS +3-8%, GPU +5-10%"
            ));

            section.Tweaks.Add(CreateTweakModel(
                "memory_optimization",
                "Optimizar RAM",
                "DisablePagingExecutive = 1, Requiere 16GB+"
            ));

            section.Tweaks.Add(CreateTweakModel(
                "transparency_effects",
                "Disable Transparency Effects",
                "Deshabilita efectos de transparencia de Windows\nGPU usage -3-8%, VRAM +50-200MB liberada"
            ));

            section.Tweaks.Add(CreateTweakModel(
                "sticky_keys",
                "Deshabilitar Sticky Keys",
                "Elimina popups molestos (Shift x5, Num Lock hold)\nGaming sin interrupciones"
            ));

            section.Tweaks.Add(CreateTweakModel(
                "menu_show_delay",
                "MenuShow Delay 0ms",
                "Reduce delay artificial de menús de 400ms → 0ms\nUI instantánea, Alt+Tab más rápido"
            ));

            section.Tweaks.Add(CreateTweakModel(
                "data_queue_sizes",
                "Optimizar Data Queue Sizes",
                "Mouse: 256 buffers | Teclado: 200 buffers\nElimina skipped inputs con polling rates 1000Hz+"
            ));

            return section;
        }

        /// <summary>
        /// Crea todos los tweaks para Red & Ping
        /// </summary>
        public static TweakSectionModel CreateNetworkSection()
        {
            var section = new TweakSectionModel
            {
                Title = "Red & Ping",
                Subtitle = "Optimizaciones TCP/IP para reducir ping y latencia",
                Icon = "??",
                Category = "Network"
            };

            section.Tweaks.Add(CreateTweakModel(
                "network_optimization",
                "Optimizaci�n TCP/IP Completa",
                "TcpAckFrequency = 1, TCPNoDelay = 1, NetworkThrottling OFF\nReduce ping 5-30ms, mejora hitreg, elimina packet loss"
            ));

            section.Tweaks.Add(CreateTweakModel(
                "dns_cloudflare",
                "DNS Cloudflare (1.1.1.1)",
                "DNS m�s r�pido del mundo, latencia <10ms\nPrimario: 1.1.1.1 | Secundario: 1.0.0.1\nReduce ping 10-50ms, mejor resoluci�n de dominios",
                useApplyMode: true
            ));

            section.Tweaks.Add(new TweakModel
            {
                Title = "DNS Google (8.8.8.8)",
                Description = "DNS confiable y estable, latencia ~15ms\nPrimario: 8.8.8.8 | Secundario: 8.8.4.4\nAlternativa probada, ideal para juegos en l�nea",
                IsRecommended = true,
                ShowInfoButton = false,
                TweakId = "dns_google",
                UseApplyMode = true
            });

            section.Tweaks.Add(CreateTweakModel(
                "dns_cache",
                "Optimizar Caché DNS",
                "MaxCacheTtl = 86400, NegativeCacheTime = 0\nMejora velocidad de resolución, reduce consultas"
            ));

            // --- TWEAKS ANTI-BUFFERBLOAT ---

            section.Tweaks.Add(CreateTweakModel(
                "ecn_capability",
                "Activar ECN (Anti-Bufferbloat)",
                "Mitiga el bufferbloat severo y estabiliza el ping\nEvita packet loss notificando al router antes de saturarse"
            ));

            section.Tweaks.Add(CreateTweakModel(
                "tcp_congestion",
                "Algoritmo de Congestión TCP (CUBIC/BBR)",
                "Cambia el algoritmo de control de congestión de Windows\nOptimiza cómo se envían los paquetes para no saturar los buffers"
            ));

            section.Tweaks.Add(CreateTweakModel(
                "disable_lso",
                "Deshabilitar LSO (Large Send Offload)",
                "Evita micro-picos de ping (jitter)\nObliga a la tarjeta de red a enviar paquetes más pequeños y consistentes"
            ));

            section.Tweaks.Add(CreateTweakModel(
                "qos_prioritization",
                "Habilitar QoS (Priorización)",
                "Fuerza a Windows a enviar etiquetas de prioridad en los paquetes\nPermite que el tráfico de juegos salte la cola local de red"
            ));

            section.Tweaks.Add(CreateTweakModel(
                "tcp_autotuning",
                "Restringir TCP Auto-Tuning",
                "Limita el tamaño de ventana de recepción TCP\nPreviene el bufferbloat de bajada. (Puede reducir vel. máxima de descarga)",
                useApplyMode: true
            ));

            section.Tweaks.Add(CreateTweakModel(
                "interrupt_moderation",
                "Deshabilitar Interrupt Moderation",
                "Fuerza procesamiento inmediato de paquetes de red\nLatencia -2-10ms por paquete, jitter reducido"
            ));

            return section;
        }

        /// <summary>
        /// Crea todos los tweaks para Sistema & GPU
        /// </summary>
        public static TweakSectionModel CreateSystemGpuSection()
        {
            var section = new TweakSectionModel
            {
                Title = "Sistema & GPU",
                Subtitle = "GPU, CPU y Configuraci�n del Sistema",
                Icon = "??",
                Category = "System"
            };

            section.Tweaks.Add(CreateTweakModel(
                "system_profile",
                "System Profile Games Priority",
                "GPU Priority: 8, CPU Priority: 6, Scheduling: High"
            ));

            section.Tweaks.Add(CreateTweakModel(
                "gamedvr_disable",
                "Deshabilitar GameDVR (Xbox Game Bar)",
                "Elimina overlay, reduce input lag 5-15ms"
            ));

            section.Tweaks.Add(CreateTweakModel(
                "gpu_scheduling",
                "Hardware GPU Scheduling",
                "Puede mejorar o empeorar latencia (probar ambos)"
            ));

            section.Tweaks.Add(CreateTweakModel(
                "high_performance",
                "Plan de Energ�a: Alto Rendimiento",
                "CPU siempre a m�xima frecuencia"
            ));

            return section;
        }

        /// <summary>
        /// Crea todos los tweaks para Limpieza
        /// </summary>
        public static TweakSectionModel CreateCleanupSection()
        {
            var section = new TweakSectionModel
            {
                Title = "Limpieza",
                Subtitle = "Optimización de espacio y archivos temporales",
                Icon = "??",
                Category = "Cleanup"
            };

            section.Tweaks.Add(CreateTweakModel(
                "temp_files_cleanup",
                "Limpiar Archivos Temporales",
                "Elimina archivos en %TEMP%, Prefetch y caché de aplicaciones\nLibera 500MB - 5GB+ de espacio en disco",
                useApplyMode: true
            ));

            section.Tweaks.Add(CreateTweakModel(
                "win_update_cache",
                "Caché de Windows Update",
                "Limpia la carpeta SoftwareDistribution\nLibera hasta 10GB de espacio en disco",
                useApplyMode: true
            ));

            return section;
        }

        /// <summary>
        /// Crea todos los tweaks para GHOST Pack
        /// </summary>
        public static TweakSectionModel CreateGhostPackSection()
        {
            var section = new TweakSectionModel
            {
                Title = "GHOST Pack",
                Subtitle = "Optimizaciones Extremas (Requiere Reinicio)",
                Icon = "??",
                Category = "Ghost"
            };

            section.Tweaks.Add(CreateTweakModel(
                "core_isolation",
                "Deshabilitar Core Isolation (VBS)",
                "FPS +10-30% (Ryzen), Input lag -5ms\nElimina overhead de virtualización de Windows"
            ));

            section.Tweaks.Add(CreateTweakModel(
                "hpet_optimization",
                "Optimización HPET",
                "Micro-stuttering -80% (Ryzen), 0.1% low FPS +20%\nForza el uso de timers TSC ultra-precisos"
            ));

            section.Tweaks.Add(CreateTweakModel(
                "mpo_fix",
                "MPO Fix (Anti-Flicker)",
                "Elimina stuttering y pantallazos negros en navegadores/juegos\nFuerza modo legacy estable en composición de ventanas"
            ));

            return section;
        }

        /// <summary>
        /// Crea todos los tweaks para Advanced System
        /// </summary>
        public static TweakSectionModel CreateAdvancedSection()
        {
            var section = new TweakSectionModel
            {
                Title = "Advanced",
                Subtitle = "Configuraciones avanzadas de Windows",
                Icon = "??",
                Category = "Advanced"
            };

            section.Tweaks.Add(CreateTweakModel(
                "fso_game_dvr",
                "Desactivar FSO y Game DVR",
                "Input lag -5-15ms, Frame pacing consistente\nElimina latencia del compositor DWM en Fullscreen"
            ));

            section.Tweaks.Add(CreateTweakModel(
                "uac_disable",
                "Desactivar UAC (Notificaciones)",
                "Evita popups molestos de permisos\nFlujo de trabajo ininterrumpido (Reduce seguridad)"
            ));

            section.Tweaks.Add(CreateTweakModel(
                "hibernate_disable",
                "Desactivar Hibernación",
                "Libera 4GB-16GB de espacio en disco\nElimina hiberfil.sys y alarga vida de SSD"
            ));

            return section;
        }

        /// <summary>
        /// Crea todos los tweaks para Laptop & Power
        /// </summary>
        public static TweakSectionModel CreateLaptopPowerSection()
        {
            var section = new TweakSectionModel
            {
                Title = "Laptop & Power",
                Subtitle = "Planes de energía y optimización de batería",
                Icon = "??",
                Category = "Laptop"
            };

            section.Tweaks.Add(CreateTweakModel(
                "ultimate_power_plan",
                "Plan: Máximo Rendimiento",
                "Desbloquea 'Ultimate Performance'\nMáxima respuesta de CPU y mínima latencia de hardware",
                useApplyMode: true
            ));

            section.Tweaks.Add(CreateTweakModel(
                "usb_selective_suspend",
                "Desactivar Suspensión USB",
                "Evita que el mouse/teclado se 'duerman'\nEstabilidad total en periféricos gaming"
            ));

            return section;
        }

        /// <summary>
        /// Crea todos los tweaks para Competitive Gaming
        /// </summary>
        public static TweakSectionModel CreateCompetitiveSection()
        {
            var section = new TweakSectionModel
            {
                Title = "Competitive",
                Subtitle = "Latencia ultra-baja y priorización de juegos",
                Icon = "??",
                Category = "Competitive"
            };

            section.Tweaks.Add(CreateTweakModel(
                "max_timer_resolution",
                "Timer Resolution 0.5ms",
                "Reduce jitter de frame times y mejora precisión de mira\nInput lag -1-2ms (Solo mientras la app está abierta)",
                useApplyMode: true
            ));

            section.Tweaks.Add(CreateTweakModel(
                "gpu_irq_priority",
                "GPU IRQ Priority (High)",
                "Forza al CPU a priorizar interrupciones de video\nEstabilidad de FPS mejorada y 1% lows más altos"
            ));

            section.Tweaks.Add(CreateTweakModel(
                "network_throttling_disable",
                "Disable Network Throttling",
                "Evita que Windows limite la red para tareas de fondo\nSin picos de lag repentinos durante gaming"
            ));

            section.Tweaks.Add(CreateTweakModel(
                "system_responsiveness",
                "System Responsiveness (Gaming)",
                "Prioridad absoluta al juego activo sobre servicios de fondo\nRespuesta del sistema inmediata y Alt+Tab más rápido"
            ));

            section.Tweaks.Add(CreateTweakModel(
                "csrss_priority",
                "CSRSS High Priority",
                "Prioriza el proceso de rendering Win32 del sistema\nMenos micro-stutters en UI durante gaming intensivo"
            ));

            section.Tweaks.Add(CreateTweakModel(
                "gpu_irq_affinity",
                "GPU IRQ Affinity (Último Core)",
                "Asigna interrupciones GPU a un core dedicado\nDPC latency reducida, frame times más consistentes\n⚠️ REQUIERE REINICIO OBLIGATORIO"
            ));

            return section;
        }

        /// <summary>
        /// Crea la sección de Advanced Latency Tweaks
        /// </summary>
        public static TweakSectionModel CreateAdvancedLatencySection()
        {
            var section = new TweakSectionModel
            {
                Title = "Advanced Latency",
                Subtitle = "Tweaks de latencia de bajo nivel del sistema",
                Icon = "⚡",
                Category = "AdvancedLatency"
            };

            section.Tweaks.Add(CreateTweakModel(
                "interrupt_moderation",
                "Deshabilitar Interrupt Moderation",
                "Procesamiento inmediato de paquetes de red\nLatencia -2-10ms, jitter reducido"
            ));

            section.Tweaks.Add(CreateTweakModel(
                "menu_show_delay",
                "MenuShow Delay 0ms",
                "Delay de menús de 400ms → 0ms\nUI responde instantáneamente"
            ));

            section.Tweaks.Add(CreateTweakModel(
                "data_queue_sizes",
                "Optimizar Data Queue Sizes",
                "Mouse: 256 buffers | Teclado: 200 buffers\nCero inputs perdidos con 1000Hz+"
            ));

            section.Tweaks.Add(CreateTweakModel(
                "csrss_priority",
                "CSRSS High Priority",
                "Prioriza el proceso de rendering Win32\nMenos micro-stutters en UI durante gaming"
            ));

            section.Tweaks.Add(CreateTweakModel(
                "gpu_irq_affinity",
                "GPU IRQ Affinity (Último Core)",
                "Interrupciones GPU en core dedicado\nDPC latency reducida, frame times consistentes\n⚠️ REQUIERE REINICIO"
            ));

            return section;
        }

        /// <summary>
        /// Obtiene todas las secciones disponibles
        /// </summary>
        public static List<TweakSectionModel> GetAllSections()
        {
            return new List<TweakSectionModel>
            {
                CreateInputVisualsSection(),
                CreateNetworkSection(),
                CreateSystemGpuSection(),
                CreateCleanupSection(),
                CreateGhostPackSection(),
                CreateAdvancedSection(),
                CreateLaptopPowerSection(),
                CreateCompetitiveSection(),
                CreateAdvancedLatencySection()
            };
        }
    }
}
