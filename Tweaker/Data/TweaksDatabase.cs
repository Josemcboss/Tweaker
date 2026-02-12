using System.Collections.Generic;
using System.Linq;

namespace Tweaker.Data
{
    /// <summary>
    /// TweakInfo - Información detallada de un tweak específico
    /// </summary>
    public class TweakInfo
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Benefits { get; set; }
        public string Warnings { get; set; }
        public bool Recommended { get; set; }
        public string Category { get; set; }
    }

    /// <summary>
    /// TweaksDatabase - Base de datos con información de todos los tweaks
    /// </summary>
    public static class TweaksDatabase
    {
        private static readonly Dictionary<string, TweakInfo> _tweaks = new Dictionary<string, TweakInfo>
        {
            // ???????????????????????????????????????????????????????????????????
            // INPUT & VISUALS TWEAKS
            // ???????????????????????????????????????????????????????????????????
            
            ["mouse_acceleration"] = new TweakInfo
            {
                Id = "mouse_acceleration",
                Title = "Desactivar Aceleración del Mouse",
                Category = "Input & Visuals",
                Description = "Elimina la aceleración artificial que Windows aplica al mouse. Esto hace que el cursor se mueva a velocidad constante independientemente de qué tan rápido muevas el mouse físicamente.",
                Benefits = "• Aim 1:1 pixel perfect tracking\n• Movimientos predecibles y consistentes\n• Mejor muscle memory para gaming\n• Precisión mejorada en shooters competitivos\n• Usado por el 100% de pro players",
                Warnings = "• Puede sentirse 'lento' al principio\n• Requiere reajustar sensibilidad en juegos\n• Necesitas acostumbrarte al cambio",
                Recommended = true
            },
            
            ["keyboard_optimization"] = new TweakInfo
            {
                Id = "keyboard_optimization",
                Title = "Optimizar Teclado",
                Category = "Input & Visuals", 
                Description = "Reduce el delay de repetición de teclas y optimiza la respuesta del teclado para gaming. Configura KeyboardDelay = 0 para respuesta instantánea.",
                Benefits = "• Input lag reducido en ~50ms\n• Respuesta más rápida de teclas\n• Mejor para spam de habilidades\n• Movimiento más fluido en juegos",
                Warnings = "• Puede causar repetición accidental de teclas\n• Algunos juegos pueden no beneficiarse",
                Recommended = true
            },

            ["visual_effects"] = new TweakInfo
            {
                Id = "visual_effects", 
                Title = "Efectos Visuales OFF",
                Category = "Input & Visuals",
                Description = "Deshabilita animaciones de Windows, transparencias y efectos visuales para liberar recursos de GPU. Configura VisualFXSetting = 2 (Mejor rendimiento).",
                Benefits = "• FPS +3-8% en promedio\n• GPU usage -5-10% (disponible para el juego)\n• Alt+Tab 50% más rápido\n• RAM libre +200-500MB\n• Frame times más consistentes",
                Warnings = "• Windows se verá más 'plano'\n• Sin animaciones ni transparencias\n• Menos atractivo visualmente",
                Recommended = true
            },

            ["memory_optimization"] = new TweakInfo
            {
                Id = "memory_optimization",
                Title = "Optimizar RAM", 
                Category = "Input & Visuals",
                Description = "Evita que Windows use archivo de paginación para código ejecutable del kernel. Requiere al menos 16GB de RAM para funcionar correctamente.",
                Benefits = "• Kernel siempre en RAM física\n• Latencia del sistema reducida\n• Mejor responsividad general\n• Sin paginación de código crítico",
                Warnings = "• REQUIERE 16GB+ de RAM\n• Puede causar inestabilidad con poca RAM\n• Solo para sistemas con memoria suficiente",
                Recommended = false
            },

            ["transparency_effects"] = new TweakInfo
            {
                Id = "transparency_effects",
                Title = "Disable Transparency Effects",
                Category = "Input & Visuals", 
                Description = "Deshabilita efectos de transparencia y Acrylic de Windows. Libera recursos de GPU que se usaban para renderizar transparencias.",
                Benefits = "• GPU usage -3-8%\n• VRAM liberada +50-200MB\n• Compositor más eficiente\n• Mejor frame stability\n• Menos carga en GPU integradas",
                Warnings = "• Ventanas se ven más sólidas\n• Sin efectos de transparencia modernos\n• Interfaz menos 'premium'",
                Recommended = true
            },

            ["sticky_keys"] = new TweakInfo
            {
                Id = "sticky_keys",
                Title = "Deshabilitar Sticky Keys",
                Category = "Input & Visuals",
                Description = "Elimina los popups molestos de accesibilidad que aparecen al presionar Shift 5 veces, Num Lock mantenido, etc. durante gaming.",
                Benefits = "• Sin interrupciones durante gaming\n• Elimina popups de Shift x5\n• Sin alertas de accesibilidad\n• Gaming ininterrumpido",
                Warnings = "• Desactiva funciones de accesibilidad\n• No recomendado si usas esas funciones",
                Recommended = true
            },

            // ???????????????????????????????????????????????????????????????????
            // NETWORK TWEAKS
            // ???????????????????????????????????????????????????????????????????
            
            ["network_optimization"] = new TweakInfo
            {
                Id = "network_optimization", 
                Title = "Optimización TCP/IP Completa",
                Category = "Red & Ping",
                Description = "Aplica tweaks TCP/IP agresivos para gaming competitivo. TcpAckFrequency=1, TCPNoDelay=1, NetworkThrottling OFF. Reduce ping y mejora hitreg.",
                Benefits = "• Ping reducido 5-30ms\n• Hitreg más consistente en shooters\n• Packet loss eliminado\n• Input lag de red -10-40ms\n• Usado por pro players",
                Warnings = "• Puede ralentizar navegadores web\n• Discord puede conectar más lento\n• Mayor uso de CPU de red",
                Recommended = false
            },

            ["network_balanced"] = new TweakInfo
            {
                Id = "network_balanced",
                Title = "Optimización Balanceada (Gaming + Navegadores)", 
                Category = "Red & Ping",
                Description = "Versión balanceada de los tweaks de red. TcpAckFrequency=2, throttling moderado. 90% del beneficio gaming sin afectar navegadores.",
                Benefits = "• 90% del rendimiento gaming\n• Navegadores funcionan correctamente\n• Discord conecta sin problemas\n• Balance perfecto uso mixto",
                Warnings = "• Ligeramente menos agresivo que modo extremo",
                Recommended = true
            },

            ["dns_cloudflare"] = new TweakInfo
            {
                Id = "dns_cloudflare",
                Title = "DNS Cloudflare (1.1.1.1)",
                Category = "Red & Ping",
                Description = "Configura los servidores DNS más rápidos del mundo. Cloudflare tiene latencia <10ms globalmente y es ultra-confiable.",
                Benefits = "• Latencia DNS <10ms\n• Resolución ultra-rápida\n• Ping reducido 10-50ms\n• Más estable que DNS del ISP\n• Sin censura ni logging",
                Warnings = "• Cambio permanente hasta revertir\n• Algunos ISP pueden detectarlo",
                Recommended = true
            },

            ["dns_cache"] = new TweakInfo
            {
                Id = "dns_cache",
                Title = "Optimizar Caché DNS",
                Category = "Red & Ping", 
                Description = "Optimiza la configuración del caché DNS de Windows. MaxCacheTtl=86400, NegativeCacheTime=0. Mejora velocidad de resolución.",
                Benefits = "• Resolución DNS más rápida\n• Menos consultas a servidores\n• Navegación web más fluida\n• Conexiones más rápidas",
                Warnings = "• Cambios de DNS tardan más en aplicarse",
                Recommended = true
            },

            // ???????????????????????????????????????????????????????????????????
            // SISTEMA & GPU TWEAKS
            // ???????????????????????????????????????????????????????????????????
            
            ["system_profile"] = new TweakInfo
            {
                Id = "system_profile",
                Title = "System Profile Games Priority",
                Category = "Sistema & GPU",
                Description = "Configura prioridad máxima de GPU y CPU para gaming. GPU Priority=8, CPU Priority=6, Scheduling=High. Crítico para gaming competitivo.",
                Benefits = "• Input lag reducido 3-8ms\n• 1% y 0.1% low FPS mejorado\n• Micro-stutters eliminados\n• Prioridad total para gaming\n• Frame times más estables",
                Warnings = "• Procesos en background más lentos\n• Multitasking afectado",
                Recommended = true
            },

            ["gamedvr_disable"] = new TweakInfo
            {
                Id = "gamedvr_disable",
                Title = "Deshabilitar GameDVR (Xbox Game Bar)", 
                Category = "Sistema & GPU",
                Description = "Elimina completamente Xbox Game Bar y DVR. Libera overlay, reduce input lag y elimina grabación en background.",
                Benefits = "• Input lag -5-15ms\n• CPU liberado del overlay\n• Sin interrupciones de Game Bar\n• Recursos dedicados al juego\n• Sin capturas accidentales",
                Warnings = "• Sin screenshots/grabación de Xbox\n• Sin Game Bar overlay\n• Funciones Xbox Live afectadas",
                Recommended = true
            },

            ["gpu_scheduling"] = new TweakInfo
            {
                Id = "gpu_scheduling",
                Title = "Hardware GPU Scheduling",
                Category = "Sistema & GPU",
                Description = "Activa/desactiva GPU scheduling por hardware. El efecto varía según el sistema - puede mejorar o empeorar latencia. Probar ambos modos.",
                Benefits = "• Puede reducir latencia GPU\n• Mejor multitasking de GPU\n• Scheduling más eficiente\n• Compatible con GPUs modernas",
                Warnings = "• Efecto impredecible por sistema\n• Algunos sistemas empeoran\n• Requiere GPU compatible\n• Probar ambos modos",
                Recommended = false
            },

            ["high_performance"] = new TweakInfo
            {
                Id = "high_performance",
                Title = "Plan de Energía: Alto Rendimiento",
                Category = "Sistema & GPU", 
                Description = "Activa plan de energía de alto rendimiento. CPU siempre a máxima frecuencia, sin throttling de energía. Crítico para gaming.",
                Benefits = "• CPU siempre a máx frecuencia\n• Sin throttling de energía\n• Latencia CPU reducida\n• Frame times más consistentes\n• Sin power management delays",
                Warnings = "• Mayor consumo energético\n• Más calor generado\n• Laptops: menor duración batería\n• Ventiladores más activos",
                Recommended = true
            },

            // ???????????????????????????????????????????????????????????????????
            // GHOST PACK TWEAKS  
            // ???????????????????????????????????????????????????????????????????
            
            ["core_isolation"] = new TweakInfo
            {
                Id = "core_isolation",
                Title = "Deshabilitar Core Isolation (VBS)",
                Category = "GHOST Pack",
                Description = "Desactiva Virtualization Based Security y Memory Integrity. Elimina overhead de virtualización que causa pérdidas de FPS significativas.",
                Benefits = "• FPS +10-30% (especialmente Ryzen)\n• Input lag -3-5ms\n• Latencia de memoria reducida\n• Sin overhead de VBS\n• Mejor para gaming competitivo",
                Warnings = "• REQUIERE REINICIO\n• Reduce seguridad del sistema\n• Menor protección contra malware\n• Solo para PCs dedicados gaming",
                Recommended = false
            },

            ["hpet_optimization"] = new TweakInfo
            {
                Id = "hpet_optimization",
                Title = "Optimización HPET",
                Category = "GHOST Pack",
                Description = "Forza uso de timers TSC más rápidos en lugar de HPET lento. Especialmente beneficioso en CPUs AMD Ryzen donde HPET causa stuttering.",
                Benefits = "• Micro-stuttering -80% (Ryzen)\n• Frame times más consistentes\n• 0.1% low FPS +15-25%\n• Timer ultra-preciso\n• Elimina hiccups de timer",
                Warnings = "• REQUIERE REINICIO OBLIGATORIO\n• Comando bcdedit (requiere admin)\n• Algunos sistemas pueden no beneficiarse",
                Recommended = false
            },

            ["mpo_fix"] = new TweakInfo
            {
                Id = "mpo_fix",
                Title = "MPO Fix (Anti-Flicker)",
                Category = "GHOST Pack", 
                Description = "Deshabilita Multiplane Overlay para eliminar stuttering y pantallazos negros. Fuerza modo legacy más estable en composición de ventanas.",
                Benefits = "• Elimina stuttering por MPO\n• Sin pantallazos negros\n• Frame pacing más consistente\n• Overlays funcionan sin problemas\n• Mejor compatibilidad G-Sync/FreeSync",
                Warnings = "• REQUIERE REINICIO\n• Posible ligero aumento uso GPU\n• Algunos sistemas pueden no necesitarlo",
                Recommended = false
            }
        };

        public static TweakInfo GetTweakInfo(string tweakId)
        {
            return _tweaks.TryGetValue(tweakId, out TweakInfo info) ? info : CreateDefaultTweakInfo(tweakId);
        }

        private static TweakInfo CreateDefaultTweakInfo(string tweakId)
        {
            return new TweakInfo
            {
                Id = tweakId,
                Title = "Información no disponible",
                Description = "La información detallada para este tweak aún no está disponible.",
                Benefits = "Consulta la documentación para más detalles.",
                Warnings = "Revisa la documentación antes de aplicar.",
                Recommended = true
            };
        }

        public static IEnumerable<TweakInfo> GetTweaksByCategory(string category)
        {
            return _tweaks.Values.Where(t => t.Category == category);
        }

        public static IEnumerable<TweakInfo> GetAllTweaks()
        {
            return _tweaks.Values;
        }
    }
}