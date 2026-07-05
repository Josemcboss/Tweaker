using System.Collections.Generic;
using System.Linq;

using Tweaker.Models;

namespace Tweaker.Data
{
    /// <summary>
    /// TweakInfo - Informaci�n detallada de un tweak espec�fico
    /// </summary>
    public class TweakInfo
    {
        public string? Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Benefits { get; set; }
        public string? Warnings { get; set; }
        public bool Recommended { get; set; }
        public string? Category { get; set; }
        public RiskLevel Risk { get; set; } = RiskLevel.Safe; // Por defecto Safe
        public bool RequiresRestart { get; set; }
        public int FpsGain { get; set; }
        public int PingReduction { get; set; }
        public double RamFreedGB { get; set; }
        public bool IsCompatible { get; set; } = true;
    }

    /// <summary>
    /// TweaksDatabase - Base de datos con informaci�n de todos los tweaks
    /// </summary>
    public static class TweaksDatabase
    {
        private static readonly Dictionary<string, TweakInfo> _tweaks = new Dictionary<string, TweakInfo>
        {
            // ────────────────────────────────────────────?
            // INPUT & VISUALS TWEAKS
            // ────────────────────────────────────────────?

            ["mouse_acceleration"] = new TweakInfo
            {
                Id = "mouse_acceleration",
                Title = "Desactivar Aceleraci�n del Mouse",
                Category = "Input & Visuals",
                Description = "Elimina la aceleraci�n artificial que Windows aplica al mouse. Esto hace que el cursor se mueva a velocidad constante independientemente de qu� tan r�pido muevas el mouse f�sicamente.",
                Benefits = "� Aim 1:1 pixel perfect tracking\n� Movimientos predecibles y consistentes\n� Mejor muscle memory para gaming\n� Precisi�n mejorada en shooters competitivos\n� Usado por el 100% de pro players",
                Warnings = "� Puede sentirse 'lento' al principio\n� Requiere reajustar sensibilidad en juegos\n� Necesitas acostumbrarte al cambio",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            ["keyboard_optimization"] = new TweakInfo
            {
                Id = "keyboard_optimization",
                Title = "Optimizar Teclado",
                Category = "Input & Visuals",
                Description = "Reduce el delay de repetici�n de teclas y optimiza la respuesta del teclado para gaming. Configura KeyboardDelay = 0 para respuesta instant�nea.",
                Benefits = "� Input lag reducido en ~50ms\n� Respuesta m�s r�pida de teclas\n� Mejor para spam de habilidades\n� Movimiento m�s fluido en juegos",
                Warnings = "� Puede causar repetici�n accidental de teclas\n� Algunos juegos pueden no beneficiarse",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            ["visual_effects"] = new TweakInfo
            {
                Id = "visual_effects",
                Title = "Efectos Visuales OFF",
                Category = "Input & Visuals",
                Description = "Deshabilita animaciones de Windows, transparencias y efectos visuales para liberar recursos de GPU. Configura VisualFXSetting = 2 (Mejor rendimiento).",
                Benefits = "� FPS +3-8% en promedio\n� GPU usage -5-10% (disponible para el juego)\n� Alt+Tab 50% m�s r�pido\n� RAM libre +200-500MB\n� Frame times m�s consistentes",
                Warnings = "� Windows se ver� m�s 'plano'\n� Sin animaciones ni transparencias\n� Menos atractivo visualmente",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            ["memory_optimization"] = new TweakInfo
            {
                Id = "memory_optimization",
                Title = "Optimizar RAM",
                Category = "Input & Visuals",
                Description = "Evita que Windows use archivo de paginaci�n para c�digo ejecutable del kernel. Requiere al menos 16GB de RAM para funcionar correctamente.",
                Benefits = "� Kernel siempre en RAM f�sica\n� Latencia del sistema reducida\n� Mejor responsividad general\n� Sin paginaci�n de c�digo cr�tico",
                Warnings = "� REQUIERE 16GB+ de RAM\n� Puede causar inestabilidad con poca RAM\n� Solo para sistemas con memoria suficiente",
                Recommended = false,
                Risk = RiskLevel.Moderate
            },

            ["transparency_effects"] = new TweakInfo
            {
                Id = "transparency_effects",
                Title = "Disable Transparency Effects",
                Category = "Input & Visuals",
                Description = "Deshabilita efectos de transparencia y Acrylic de Windows. Libera recursos de GPU que se usaban para renderizar transparencias.",
                Benefits = "� GPU usage -3-8%\n� VRAM liberada +50-200MB\n� Compositor m�s eficiente\n� Mejor frame stability\n� Menos carga en GPU integradas",
                Warnings = "� Ventanas se ven m�s s�lidas\n� Sin efectos de transparencia modernos\n� Interfaz menos 'premium'",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            ["sticky_keys"] = new TweakInfo
            {
                Id = "sticky_keys",
                Title = "Deshabilitar Sticky Keys",
                Category = "Input & Visuals",
                Description = "Elimina los popups molestos de accesibilidad que aparecen al presionar Shift 5 veces, Num Lock mantenido, etc. durante gaming.",
                Benefits = "� Sin interrupciones durante gaming\n� Elimina popups de Shift x5\n� Sin alertas de accesibilidad\n� Gaming ininterrumpido",
                Warnings = "� Desactiva funciones de accesibilidad\n� No recomendado si usas esas funciones",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            // ────────────────────────────────────────────?
            // NETWORK TWEAKS
            // ────────────────────────────────────────────?

            ["network_optimization"] = new TweakInfo
            {
                Id = "network_optimization",
                Title = "Optimizaci�n TCP/IP Completa",
                Category = "Red & Ping",
                Description = "Aplica tweaks TCP/IP agresivos para gaming competitivo. TcpAckFrequency=1, TCPNoDelay=1, NetworkThrottling OFF. Reduce ping y mejora hitreg.",
                Benefits = "� Ping reducido 5-30ms\n� Hitreg m�s consistente en shooters\n� Packet loss eliminado\n� Input lag de red -10-40ms\n� Usado por pro players",
                Warnings = "� Puede ralentizar navegadores web\n� Discord puede conectar m�s lento\n� Mayor uso de CPU de red",
                Recommended = false,
                Risk = RiskLevel.Moderate
            },

            ["network_balanced"] = new TweakInfo
            {
                Id = "network_balanced",
                Title = "Optimizaci�n Balanceada (Gaming + Navegadores)",
                Category = "Red & Ping",
                Description = "Versi�n balanceada de los tweaks de red. TcpAckFrequency=2, throttling moderado. 90% del beneficio gaming sin afectar navegadores.",
                Benefits = "� 90% del rendimiento gaming\n� Navegadores funcionan correctamente\n� Discord conecta sin problemas\n� Balance perfecto uso mixto",
                Warnings = "� Ligeramente menos agresivo que modo extremo",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            ["dns_cloudflare"] = new TweakInfo
            {
                Id = "dns_cloudflare",
                Title = "DNS Cloudflare (1.1.1.1)",
                Category = "Red & Ping",
                Description = "Configura los servidores DNS m�s r�pidos del mundo. Cloudflare tiene latencia <10ms globalmente y es ultra-confiable.",
                Benefits = "� Latencia DNS <10ms\n� Resoluci�n ultra-r�pida\n� Ping reducido 10-50ms\n� M�s estable que DNS del ISP\n� Sin censura ni logging",
                Warnings = "� Cambio permanente hasta revertir\n� Algunos ISP pueden detectarlo",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            ["dns_cache"] = new TweakInfo
            {
                Id = "dns_cache",
                Title = "Optimizar Caché DNS",
                Category = "Red & Ping",
                Description = "Optimiza la configuración del caché DNS de Windows. MaxCacheTtl=86400, NegativeCacheTime=0. Mejora velocidad de resolución.",
                Benefits = "• Resolución DNS más rápida\n• Menos consultas a servidores\n• Navegación web más fluida\n• Conexiones más rápidas",
                Warnings = "• Cambios de DNS tardan más en aplicarse",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            // ———————————————————————————————————————————————————————————————————
            // ANTI-BUFFERBLOAT TWEAKS
            // ———————————————————————————————————————————————————————————————————

            ["ecn_capability"] = new TweakInfo
            {
                Id = "ecn_capability",
                Title = "Activar ECN (Anti-Bufferbloat)",
                Category = "Red & Ping",
                Description = "Activa Explicit Congestion Notification (ECN). Permite a los routers notificar a Windows sobre la congestión de la red antes de que se pierdan paquetes.",
                Benefits = "• Mitiga el bufferbloat severo\n• Evita packet loss (pérdida de paquetes)\n• Estabiliza el ping bajo carga de red\n• Mejor streaming y gaming simultáneo",
                Warnings = "• Requiere que tu router soporte ECN\n• En routers muy antiguos/malos puede causar problemas de conexión",
                Recommended = true,
                Risk = RiskLevel.Moderate
            },

            ["tcp_congestion"] = new TweakInfo
            {
                Id = "tcp_congestion",
                Title = "Algoritmo de Congestión TCP (CUBIC/BBR)",
                Category = "Red & Ping",
                Description = "Cambia el algoritmo de control de congestión de Windows a CUBIC (o BBR en Win 11). Optimiza cómo se envían los paquetes de datos para no saturar los buffers del router.",
                Benefits = "• Reduce latencia bajo estrés\n• Recuperación más rápida de caídas de red\n• Mejor aprovechamiento del ancho de banda\n• Evita inundar el router con paquetes",
                Warnings = "• Puede cambiar ligeramente la velocidad de descarga máxima",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            ["disable_lso"] = new TweakInfo
            {
                Id = "disable_lso",
                Title = "Deshabilitar LSO (Large Send Offload)",
                Category = "Red & Ping",
                Description = "Evita que la tarjeta de red envíe ráfagas masivas de datos (LSO). Obliga a enviar paquetes más pequeños y consistentes, evitando ahogar el buffer del router de golpe.",
                Benefits = "• Elimina micro-picos de ping (jitter)\n• Flujo de datos más constante\n• Reduce estrés en routers económicos\n• Tráfico de red más predecible",
                Warnings = "• Aumenta muy levemente el uso del CPU\n• Puede reducir la velocidad en transferencias LAN (red local) de varios Gigabits",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            ["qos_prioritization"] = new TweakInfo
            {
                Id = "qos_prioritization",
                Title = "Habilitar QoS (Priorización de Juegos)",
                Category = "Red & Ping",
                Description = "Combina el etiquetado DSCP y la liberación del 20% de ancho de banda reservado por Windows. Asegura que los paquetes de juegos tengan prioridad absoluta en la red local.",
                Benefits = "• Los juegos tienen prioridad sobre otros programas\n• Libera el 20% de ancho de banda reservado\n• Reduce latencia si descargas algo de fondo\n• Prepara los paquetes para routers con QoS activo",
                Warnings = "• Tu router debe soportar QoS para sacar el máximo provecho",
                Recommended = true,
                Risk = RiskLevel.Safe
            },
            ["tcp_autotuning"] = new TweakInfo
            {
                Id = "tcp_autotuning",
                Title = "Restringir TCP Auto-Tuning",
                Category = "Red & Ping",
                Description = "Limita dinámicamente el tamaño de la ventana de recepción TCP. Evita que Windows solicite demasiados datos a la vez, lo cual ayuda a prevenir el bufferbloat de bajada (Download Bufferbloat).",
                Benefits = "• Previene que las descargas saturen el ping\n• Ping más estable viendo streams o videos\n• Mejor control de flujo de datos",
                Warnings = "• Puede reducir tu velocidad máxima de descarga si tienes fibra óptica muy rápida (+500 Mbps)",
                Recommended = false,
                Risk = RiskLevel.Moderate
            },

            // ────────────────────────────────────────────?
            // SISTEMA & GPU TWEAKS
            // ────────────────────────────────────────────?

            ["system_profile"] = new TweakInfo
            {
                Id = "system_profile",
                Title = "System Profile Games Priority",
                Category = "Sistema & GPU",
                Description = "Configura prioridad m�xima de GPU y CPU para gaming. GPU Priority=8, CPU Priority=6, Scheduling=High. Cr�tico para gaming competitivo.",
                Benefits = "� Input lag reducido 3-8ms\n� 1% y 0.1% low FPS mejorado\n� Micro-stutters eliminados\n� Prioridad total para gaming\n� Frame times m�s estables",
                Warnings = "� Procesos en background m�s lentos\n� Multitasking afectado",
                Recommended = true,
                Risk = RiskLevel.Moderate
            },

            ["gamedvr_disable"] = new TweakInfo
            {
                Id = "gamedvr_disable",
                Title = "Deshabilitar GameDVR (Xbox Game Bar)",
                Category = "Sistema & GPU",
                Description = "Elimina completamente Xbox Game Bar y DVR. Libera overlay, reduce input lag y elimina grabaci�n en background.",
                Benefits = "� Input lag -5-15ms\n� CPU liberado del overlay\n� Sin interrupciones de Game Bar\n� Recursos dedicados al juego\n� Sin capturas accidentales",
                Warnings = "� Sin screenshots/grabaci�n de Xbox\n� Sin Game Bar overlay\n� Funciones Xbox Live afectadas",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            ["gpu_scheduling"] = new TweakInfo
            {
                Id = "gpu_scheduling",
                Title = "Hardware GPU Scheduling",
                Category = "Sistema & GPU",
                Description = "Activa/desactiva GPU scheduling por hardware. El efecto var�a seg�n el sistema - puede mejorar o empeorar latencia. Probar ambos modos.",
                Benefits = "� Puede reducir latencia GPU\n� Mejor multitasking de GPU\n� Scheduling m�s eficiente\n� Compatible con GPUs modernas",
                Warnings = "� Efecto impredecible por sistema\n� Algunos sistemas empeoran\n� Requiere GPU compatible\n� Probar ambos modos",
                Recommended = false,
                Risk = RiskLevel.Moderate
            },

            ["high_performance"] = new TweakInfo
            {
                Id = "high_performance",
                Title = "Plan de Energ�a: Alto Rendimiento",
                Category = "Sistema & GPU",
                Description = "Activa plan de energ�a de alto rendimiento. CPU siempre a m�xima frecuencia, sin throttling de energ�a. Cr�tico para gaming.",
                Benefits = "� CPU siempre a m�x frecuencia\n� Sin throttling de energ�a\n� Latencia CPU reducida\n� Frame times m�s consistentes\n� Sin power management delays",
                Warnings = "� Mayor consumo energ�tico\n� M�s calor generado\n� Laptops: menor duraci�n bater�a\n� Ventiladores m�s activos",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            // ────────────────────────────────────────────?
            // GHOST PACK TWEAKS  
            // ────────────────────────────────────────────?

            ["core_isolation"] = new TweakInfo
            {
                Id = "core_isolation",
                Title = "Deshabilitar Core Isolation (VBS)",
                Category = "GHOST Pack",
                Description = "Desactiva Virtualization Based Security y Memory Integrity. Elimina overhead de virtualización que causa pérdidas de FPS significativas.",
                Benefits = "• FPS +10-30% (especialmente Ryzen)\n• Input lag -3-5ms\n• Latencia de memoria reducida\n• Sin overhead de VBS\n• Mejor para gaming competitivo",
                Warnings = "• REQUIERE REINICIO\n• Reduce seguridad del sistema\n• Menor protección contra malware\n• Solo para PCs dedicados gaming",
                Recommended = false,
                Risk = RiskLevel.Advanced,
                RequiresRestart = true,
                FpsGain = 18,
                PingReduction = 0,
                RamFreedGB = 0
            },

            ["hpet_optimization"] = new TweakInfo
            {
                Id = "hpet_optimization",
                Title = "Optimización HPET",
                Category = "GHOST Pack",
                Description = "Forza uso de timers TSC más rápidos en lugar de HPET lento. Especialmente beneficioso en CPUs AMD Ryzen donde HPET causa stuttering.",
                Benefits = "• Micro-stuttering -80% (Ryzen)\n• Frame times más consistentes\n• 0.1% low FPS +15-25%\n• Timer ultra-preciso\n• Elimina hiccups de timer",
                Warnings = "• REQUIERE REINICIO OBLIGATORIO\n• Comando bcdedit (requiere admin)\n• Algunos sistemas pueden no beneficiarse",
                Recommended = false,
                Risk = RiskLevel.Advanced,
                RequiresRestart = true,
                FpsGain = 12,
                PingReduction = 0,
                RamFreedGB = 0
            },

            ["mpo_fix"] = new TweakInfo
            {
                Id = "mpo_fix",
                Title = "MPO Fix (Anti-Flicker)",
                Category = "GHOST Pack",
                Description = "Deshabilita Multiplane Overlay para eliminar stuttering y pantallazos negros. Fuerza modo legacy ms estable en composición de ventanas.",
                Benefits = " Elimina stuttering por MPO\n Sin pantallazos negros\n Frame pacing ms consistente\n Overlays funcionan sin problemas\n Mejor compatibilidad G-Sync/FreeSync",
                Warnings = " REQUIERE REINICIO\n Posible ligero aumento uso GPU\n Algunos sistemas pueden no necesitarlo",
                Recommended = false,
                Risk = RiskLevel.Moderate,
                RequiresRestart = true,
                FpsGain = 2,
                PingReduction = 0,
                RamFreedGB = 0
            },

            // ———————————————————————————————————————————————————————————————————
            // CLEANUP TWEAKS
            // ———————————————————————————————————————————————————————————————————

            ["temp_files_cleanup"] = new TweakInfo
            {
                Id = "temp_files_cleanup",
                Title = "Limpiar Archivos Temporales",
                Category = "Limpieza",
                Description = "Elimina archivos temporales de Windows, caché de aplicaciones y archivos de registro innecesarios. Libera espacio en disco y reduce fragmentación.",
                Benefits = "• Libera 500MB - 5GB+ de espacio\n• Mejora velocidad de acceso al disco\n• Reduce carga en el sistema de archivos\n• Limpieza profunda de %TEMP% y Prefetch",
                Warnings = "• Puede borrar historiales de búsqueda locales\n• Primer inicio de algunas apps puede ser lento",
                Recommended = true,
                Risk = RiskLevel.Safe,
                RamFreedGB = 2
            },

            ["win_update_cache"] = new TweakInfo
            {
                Id = "win_update_cache",
                Title = "Caché de Windows Update",
                Category = "Limpieza",
                Description = "Limpia la carpeta SoftwareDistribution donde se guardan actualizaciones ya instaladas. Muy útil para liberar varios GB de espacio.",
                Benefits = "• Libera hasta 10GB de espacio en disco\n• Soluciona problemas de Windows Update\n• Elimina instaladores residuales\n• Optimiza el almacenamiento del sistema",
                Warnings = "• Borra el historial visual de actualizaciones instaladas\n• La siguiente búsqueda de updates tardará más",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            // ———————————————————————————————————————————————————————————————————
            // ADVANCED SYSTEM TWEAKS
            // ———————————————————————————————————————————————————————————————————

            ["fso_game_dvr"] = new TweakInfo
            {
                Id = "fso_game_dvr",
                Title = "Desactivar FSO y Game DVR",
                Category = "Advanced",
                Description = "Deshabilita Fullscreen Optimizations y Game DVR globalmente. FSO añade 5-15ms de input lag al forzar el modo borderless.",
                Benefits = "• Input lag reducido 5-15ms\n• Frame pacing consistente en Fullscreen\n• Elimina micro-stuttering en juegos\n• Bypass total del compositor DWM",
                Warnings = "• Alt+Tab será ligeramente más lento\n• No podrás usar el overlay de Game Bar\n• REQUIERE REINICIO",
                Recommended = true,
                Risk = RiskLevel.Moderate,
                RequiresRestart = true,
                PingReduction = 0,
                FpsGain = 6,
                RamFreedGB = 0
            },

            ["uac_disable"] = new TweakInfo
            {
                Id = "uac_disable",
                Title = "Desactivar UAC (Notificaciones)",
                Category = "Advanced",
                Description = "Deshabilita el Control de Cuentas de Usuario (UAC). Evita que Windows pregunte '¿Quieres permitir que esta app haga cambios?' constantemente.",
                Benefits = "• Menos interrupciones visuales\n• Instalación de software más fluida\n• Elimina el parpadeo de pantalla al pedir permisos\n• Flujo de trabajo ininterrumpido",
                Warnings = "• REDUCE LA SEGURIDAD DEL SISTEMA\n• El malware se puede instalar sin avisar\n• Solo recomendado para usuarios avanzados",
                Recommended = false,
                Risk = RiskLevel.Moderate
            },

            ["hibernate_disable"] = new TweakInfo
            {
                Id = "hibernate_disable",
                Title = "Desactivar Hibernación",
                Category = "Advanced",
                Description = "Deshabilita la hibernación y elimina el archivo hiberfil.sys. Libera RAM y varios GB de espacio en disco (especialmente en SSDs).",
                Benefits = "• Libera espacio (4GB - 16GB según tu RAM)\n• Reduce escritura en el SSD (alarga vida útil)\n• Inicio limpio de Windows cada vez\n• Elimina procesos de hibernación de fondo",
                Warnings = "• No podrás usar el modo 'Hibernar'\n• El inicio rápido (Fast Startup) se desactivará",
                Recommended = true,
                Risk = RiskLevel.Safe,
                RamFreedGB = 8
            },

            // ———————————————————————————————————————————————————————————————————
            // LAPTOP & POWER TWEAKS
            // ———————————————————————————————————————————————————————————————————

            ["ultimate_power_plan"] = new TweakInfo
            {
                Id = "ultimate_power_plan",
                Title = "Plan: Máximo Rendimiento",
                Category = "Laptop & Power",
                Description = "Desbloquea el plan oculto 'Ultimate Performance'. Elimina micro-latencias de energía y mantiene el hardware al 100% de su capacidad.",
                Benefits = "• Máxima respuesta de CPU y GPU\n• Latencia de hardware mínima\n• Estabilidad total en frecuencias\n• Ideal para gaming competitivo extremo",
                Warnings = "• Alto consumo de batería en laptops\n• Genera más calor en el equipo\n• Ventiladores girarán más rápido",
                Recommended = true,
                Risk = RiskLevel.Safe,
                FpsGain = 8
            },

            ["usb_selective_suspend"] = new TweakInfo
            {
                Id = "usb_selective_suspend",
                Title = "Desactivar Suspensin USB",
                Category = "Laptop & Power",
                Description = "Evita que Windows apague los puertos USB para ahorrar Energía. Crucial para evitar que el mouse o teclado se 'duerman' durante el juego.",
                Benefits = " Mouse/Teclado siempre activos al 100%\n Elimina desconexiones USB aleatorias\n Latencia de perifricos constante\n Estabilidad en polling rate (1000Hz+)",
                Warnings = " Ligero aumento en consumo de batería",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            // ———————————————————————————————————————————————————————————————————
            // COMPETITIVE GAMING TWEAKS
            // ———————————————————————————————————————————————————————————————————

            ["max_timer_resolution"] = new TweakInfo
            {
                Id = "max_timer_resolution",
                Title = "Timer Resolution 0.5ms",
                Category = "Competitive",
                Description = "Establece la resolución del temporizador de Windows a 0.5ms (2000Hz). Reduce el jitter de frame times y mejora la precisión de entrada.",
                Benefits = "• Input lag reducido 1-2ms\n• Frame pacing ultra-consistente\n• Precisión de mira mejorada (Aim)\n• Menos micro-stuttering en shooters",
                Warnings = "• Aumenta uso de CPU en ~1%\n• Solo funciona mientras la app está abierta",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            ["gpu_irq_priority"] = new TweakInfo
            {
                Id = "gpu_irq_priority",
                Title = "GPU IRQ Priority (High)",
                Category = "Competitive",
                Description = "Forza al procesador a priorizar las interrupciones de la GPU sobre otros dispositivos. Mejora la comunicación entre CPU y tarjeta gráfica.",
                Benefits = "• Estabilidad de FPS mejorada\n• Reducción de latencia de renderizado\n• Menos caídas de FPS (1% Lows)\n• Hitreg más consistente en juegos",
                Warnings = "• Puede causar conflictos con tarjetas de sonido USB\n• Solo recomendado para GPUs modernas",
                Recommended = false,
                Risk = RiskLevel.Moderate
            },

            ["network_throttling_disable"] = new TweakInfo
            {
                Id = "network_throttling_disable",
                Title = "Disable Network Throttling",
                Category = "Competitive",
                Description = "Deshabilita el limitador de red de Windows (MMCSS). Evita que Windows reserve recursos de red para tareas multimedia de fondo.",
                Benefits = "• Ping más estable bajo carga\n• Sin picos de lag repentinos\n• Máximo ancho de banda para juegos\n• Latencia de red minimizada",
                Warnings = "• Puede afectar ligeramente la reproducción de video 4K de fondo",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            ["system_responsiveness"] = new TweakInfo
            {
                Id = "system_responsiveness",
                Title = "System Responsiveness (Gaming)",
                Category = "Competitive",
                Description = "Configura el programador de Windows para dar máxima prioridad a procesos interactivos (juegos) sobre servicios de fondo.",
                Benefits = "• Respuesta del sistema inmediata\n• Alt+Tab instantáneo\n• Sin tirones al mover el mouse rápido\n• Prioridad absoluta al juego activo",
                Warnings = "• Servicios de fondo (Windows Update, etc.) irán más lento",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            // ———————————————————————————————————————————————————————————————————
            // DISK & NTFS TWEAKS
            // ———————————————————————————————————————————————————————————————————

            ["ntfs_optimization"] = new TweakInfo
            {
                Id = "ntfs_optimization",
                Title = "Optimización NTFS (SSD)",
                Category = "Sistema & GPU",
                Description = "Desactiva Last Access Time y nombres 8.3 DOS. Reduce escrituras innecesarias en el SSD y acelera el acceso a archivos.",
                Benefits = "• Reduce escrituras SSD en ~30%\n• Alarga vida útil de tu disco\n• Elimina micro-freezes por IO overhead\n• Lectura de archivos más rápida",
                Warnings = "• Algunas apps muy antiguas (de los 90s) pueden fallar\n• REQUIERE REINICIO",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            ["memory_management"] = new TweakInfo
            {
                Id = "memory_management",
                Title = "Optimizar Memory Management",
                Category = "Sistema & GPU",
                Description = "Ajusta cmo Windows gestiona el cach de archivos en RAM. Mejora el rendimiento de carga de texturas y assets en juegos de mundo abierto.",
                Benefits = " Menos stuttering en world loading\n Tiempos de carga mejorados\n Uso eficiente de la memoria cach\n Ideal para juegos pesados (Starfield, Cyberpunk)",
                Warnings = " Puede consumir un poco ms de RAM en reposo",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            // ———————————————————————————————————————————————————————————————————
            // PRIVACY & SECURITY TWEAKS
            // ———————————————————————————————————————————————————————————————————

            ["telemetry_safe"] = new TweakInfo
            {
                Id = "telemetry_safe",
                Title = "Telemetría (Safe Mode)",
                Category = "Advanced",
                Description = "Desactiva la recolección de datos de uso de Windows de forma segura. No afecta servicios críticos de Bluetooth o aplicaciones de chat.",
                Benefits = "• Mejora la privacidad del usuario\n• Reduce procesos de tracking en fondo\n• Menos uso de ancho de banda\n• CPU liberado de tareas de logging",
                Warnings = "• No se enviarán reportes automáticos de errores a Microsoft",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            ["advertising_id"] = new TweakInfo
            {
                Id = "advertising_id",
                Title = "Desactivar Advertising ID",
                Category = "Advanced",
                Description = "Deshabilita el ID de publicidad único de Windows que permite a las apps rastrearte para mostrar anuncios personalizados.",
                Benefits = "• Elimina el rastreo entre aplicaciones\n• Privacidad mejorada significativamente\n• Menos telemetría comercial",
                Warnings = "• Verás anuncios genéricos en vez de personalizados en la tienda",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            ["ceip_disable"] = new TweakInfo
            {
                Id = "ceip_disable",
                Title = "Desactivar CEIP (SQM)",
                Category = "Advanced",
                Description = "Deshabilita el Customer Experience Improvement Program. Evita que Windows envíe datos de uso anónimos periódicamente.",
                Benefits = "• Elimina el envío periódico de datos\n• Menos actividad de red en reposo\n• Privacidad reforzada",
                Warnings = "• Ayuda menos a Microsoft a mejorar Windows (¿A quién le importa?)",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            // ———————————————————————————————————————————————————————————————————
            // SYSTEM SERVICES (DEBLOAT)
            // ———————————————————————————————————————————————————————————————————

            ["print_spooler"] = new TweakInfo
            {
                Id = "print_spooler",
                Title = "Servicio de Impresión (Spooler)",
                Category = "Limpieza",
                Description = "Deshabilita el servicio que gestiona las colas de impresión. Innecesario si no usas impresora.",
                Benefits = "• Libera ~50MB de RAM\n• Un proceso menos en background\n• Mejora ligera en tiempos de inicio",
                Warnings = "• NO PODRÁS IMPRIMIR. Reactívalo si vas a usar una impresora.",
                Recommended = false,
                Risk = RiskLevel.Safe
            },

            ["xbox_services"] = new TweakInfo
            {
                Id = "xbox_services",
                Title = "Servicios de Xbox",
                Category = "Limpieza",
                Description = "Deshabilita servicios de autenticación y guardado en la nube de Xbox. Solo si NO usas la app de Xbox o Game Pass.",
                Benefits = "• Libera recursos del sistema\n• Elimina notificaciones de Xbox en juegos no-Xbox\n• Menos servicios de fondo activos",
                Warnings = "• No funcionará la App de Xbox ni el guardado en nube de Game Pass",
                Recommended = false,
                Risk = RiskLevel.Safe
            },

            ["windows_search_disable"] = new TweakInfo
            {
                Id = "windows_search_disable",
                Title = "Búsqueda de Windows (Indexing)",
                Category = "Advanced",
                Description = "Deshabilita el servicio de indexación de archivos. Útil si ya conoces tus carpetas o usas un buscador rápido como 'Everything'.",
                Benefits = "• Elimina el uso de disco del indexador\n• Reduce stuttering durante gaming\n• CPU usage -2-5% en reposo",
                Warnings = "• Buscar archivos en el explorador será MUCHO más lento",
                Recommended = false,
                Risk = RiskLevel.Moderate
            },

            ["sysmain_disable"] = new TweakInfo
            {
                Id = "sysmain_disable",
                Title = "SysMain (SuperFetch)",
                Category = "Advanced",
                Description = "Evita que Windows precargue apps en RAM. Solo recomendado si tienes 16GB+ de RAM o un SSD muy rápido.",
                Benefits = "• Libera 1GB-3GB de RAM\n• Reduce carga en el disco al iniciar sesión\n• Ideal para gaming competitivo",
                Warnings = "• Apps pesadas (Photoshop, etc.) pueden tardar más en abrir la primera vez",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            // ─
            // ADVANCED LATENCY TWEAKS
            // ─

            ["interrupt_moderation"] = new TweakInfo
            {
                Id = "interrupt_moderation",
                Title = "Deshabilitar Interrupt Moderation",
                Category = "Advanced",
                Description = "La Interrupt Moderation agrupa interrupciones de red para ahorrar CPU, pero introduce una latencia artificial de 2-10ms por cada grupo. Deshabilitarla fuerza al adaptador a procesar cada interrupción de forma inmediata.",
                Benefits = "• Latencia de red -2-10ms por paquete\n• Jitter reducido (ping más estable)\n• Mejor hitreg en FPS competitivos\n• Procesamiento de paquetes inmediato",
                Warnings = "• CPU usage de red puede subir +1-3%\n• Solo beneficioso con adaptadores Ethernet\n• Requiere reinicio para aplicar completamente",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            ["menu_show_delay"] = new TweakInfo
            {
                Id = "menu_show_delay",
                Title = "MenuShow Delay 0ms",
                Category = "Advanced",
                Description = "Windows tiene un delay artificial de 400ms antes de mostrar menús contextuales. Reducirlo a 0ms hace que la interfaz responda de forma instantánea.",
                Benefits = "• UI responde instantáneamente\n• Alt+Tab más rápido\n• Menús contextuales sin delay\n• Sensación general de fluidez +20%",
                Warnings = "• Efecto principalmente cosmético/perceptual\n• No afecta directamente FPS",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            ["data_queue_sizes"] = new TweakInfo
            {
                Id = "data_queue_sizes",
                Title = "Optimizar Data Queue Sizes",
                Category = "Advanced",
                Description = "Aumenta los buffers de la cola de datos para mouse (100→256) y teclado (100→200). Con polling rates de 1000Hz+, el buffer por defecto puede desbordarse causando inputs perdidos.",
                Benefits = "• Elimina 'skipped inputs' con 1000Hz+\n• Mouse: 256 eventos en cola vs 100\n• Teclado: 200 eventos en cola vs 100\n• Esencial para mice 2000-8000Hz\n• 0 inputs perdidos en momentos críticos",
                Warnings = "• REQUIERE REINICIO para aplicar\n• Solo notable con polling rates >800Hz",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            ["csrss_priority"] = new TweakInfo
            {
                Id = "csrss_priority",
                Title = "CSRSS High Priority",
                Category = "Advanced",
                Description = "csrss.exe es el proceso crítico de Windows que maneja el rendering del subsistema Win32 (ventanas, inputs, mensajes). Elevarlo a prioridad Alta reduce stuttering en la UI durante gaming intensivo.",
                Benefits = "• Rendering de UI más fluido\n• Alt+Tab sin stuttering\n• Input de Windows procesado con prioridad\n• Menos micro-freezes en juegos\n• Mejor responsividad general",
                Warnings = "• Cambio de prioridad es temporal (se restaura al reiniciar)\n• Efecto más notable en sistemas con pocos cores",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            ["gpu_irq_affinity"] = new TweakInfo
            {
                Id = "gpu_irq_affinity",
                Title = "GPU IRQ Affinity (Último Core)",
                Category = "Advanced",
                Description = "Asigna las interrupciones de la GPU (IRQ) a un core dedicado —el último core del procesador— separándolas del resto de workloads. Esto reduce DPC latency y hace más consistentes los frame times.",
                Benefits = "• DPC latency reducida\n• Frame times más consistentes\n• GPU interrupciones en core dedicado\n• Mejor separación de workloads CPU/GPU\n• Detectable con LatencyMon",
                Warnings = "• REQUIERE REINICIO OBLIGATORIO\n• Requiere mínimo 4 cores físicos\n• Puede requerir ajuste manual según tu hardware\n• Efecto varía entre sistemas",
                Recommended = false,
                Risk = RiskLevel.Moderate,
                RequiresRestart = true,
                FpsGain = 4
            },

            ["gpu_hw_scheduling_v2"] = new TweakInfo
            {
                Id = "gpu_hw_scheduling_v2",
                Title = "GPU HW Scheduling Agresivo",
                Category = "Sistema & GPU",
                Description = "Modo 2 de Hardware Scheduling. Mejor frame pacing en DX12/Vulkan.",
                Benefits = "• Mejor frame pacing\n• Menor overhead del scheduler\n• Optimización agresiva para GPUs modernas",
                Warnings = "• REQUIERE REINICIO\n• Solo GPUs compatibles",
                Recommended = false,
                Risk = RiskLevel.Moderate,
                RequiresRestart = true,
                FpsGain = 8
            },
            ["shader_cache_disable"] = new TweakInfo
            {
                Id = "shader_cache_disable",
                Title = "Shader Cache Limpieza",
                Category = "Sistema & GPU",
                Description = "Limpia y desactiva shader cache para eliminar stutters de compilación.",
                Benefits = "• Menos stutters por compilación\n• Caché limpia y coherente",
                Warnings = "• Puede aumentar compilación inicial de shaders",
                Recommended = false,
                Risk = RiskLevel.Safe,
                FpsGain = 3
            },
            ["disable_ipv6"] = new TweakInfo
            {
                Id = "disable_ipv6",
                Title = "Deshabilitar IPv6",
                Category = "Red & Ping",
                Description = "Elimina overhead del stack IPv6 si solo usas IPv4.",
                Benefits = "• Menor overhead de red\n• Conexiones IPv4 más directas",
                Warnings = "• Puede romper redes que dependan de IPv6",
                Recommended = false,
                Risk = RiskLevel.Moderate,
                PingReduction = 5
            },
            ["wasapi_exclusive_mode"] = new TweakInfo
            {
                Id = "wasapi_exclusive_mode",
                Title = "WASAPI Exclusive Mode",
                Category = "Sistema & GPU",
                Description = "Audio exclusivo para gaming. Reduce latencia de audio hasta 20ms.",
                Benefits = "• Menor latencia de audio\n• Menor mixing overhead",
                Warnings = "• Otras apps no tendrán sonido mientras el juego esté activo",
                Recommended = false,
                Risk = RiskLevel.Safe
            },
            ["audio_enhancements_off"] = new TweakInfo
            {
                Id = "audio_enhancements_off",
                Title = "Audio Enhancements OFF",
                Category = "Sistema & GPU",
                Description = "Desactiva procesamiento de audio de Windows. CPU -2-5%.",
                Benefits = "• Menor uso de CPU\n• Menor procesamiento de audio",
                Warnings = "• Sin mejoras de audio de Windows",
                Recommended = true,
                Risk = RiskLevel.Safe,
                FpsGain = 2
            },
            ["ssd_write_cache"] = new TweakInfo
            {
                Id = "ssd_write_cache",
                Title = "SSD Write Cache",
                Category = "Limpieza",
                Description = "Activa caché de escritura en disco. Velocidad de escritura +30-50%.",
                Benefits = "• Escritura más rápida\n• Mejor throughput de disco",
                Warnings = "• No recomendado sin UPS o con cortes de luz frecuentes",
                Recommended = false,
                Risk = RiskLevel.Moderate,
                FpsGain = 3
            },
            ["trim_optimization"] = new TweakInfo
            {
                Id = "trim_optimization",
                Title = "TRIM Forzado SSD",
                Category = "Limpieza",
                Description = "Fuerza TRIM en SSD para mantener velocidades máximas.",
                Benefits = "• Mantiene rendimiento SSD\n• Menor degradación de escritura",
                Warnings = "• Solo para SSD",
                Recommended = true,
                Risk = RiskLevel.Safe,
                FpsGain = 2
            },
            ["ntfs_mft_zone"] = new TweakInfo
            {
                Id = "ntfs_mft_zone",
                Title = "NTFS MFT Zone Reserva",
                Category = "Limpieza",
                Description = "Reserva más espacio para MFT. Reduce fragmentación en gaming.",
                Benefits = "• Menor fragmentación\n• Mejor acceso a metadata NTFS",
                Warnings = "• Cambia el comportamiento de reserva NTFS",
                Recommended = true,
                Risk = RiskLevel.Safe,
                FpsGain = 1
            },
            ["irq_network_priority"] = new TweakInfo
            {
                Id = "irq_network_priority",
                Title = "IRQ Network Priority",
                Category = "Advanced",
                Description = "Prioriza interrupciones de red sobre otros dispositivos.",
                Benefits = "• Ping más bajo\n• Menos latencia de red",
                Warnings = "• Puede afectar otros dispositivos",
                Recommended = false,
                Risk = RiskLevel.Moderate,
                PingReduction = 8
            },
            ["nagle_algorithm_off"] = new TweakInfo
            {
                Id = "nagle_algorithm_off",
                Title = "Nagle Algorithm OFF",
                Category = "Advanced",
                Description = "Elimina buffering de paquetes TCP. Respuesta de red instantánea.",
                Benefits = "• Menor latencia TCP\n• Paquetes más inmediatos",
                Warnings = "• Incrementa el número de paquetes pequeños",
                Recommended = true,
                Risk = RiskLevel.Safe,
                PingReduction = 15
            },
            ["cpu_affinity_auto_gaming"] = new TweakInfo
            {
                Id = "cpu_affinity_auto_gaming",
                Title = "CPU Affinity Auto-Gaming",
                Category = "Advanced",
                Description = "Asigna juegos a núcleos físicos automáticamente. Sin hyperthreading.",
                Benefits = "• Mejor consistencia de FPS\n• Núcleos físicos prioritarios",
                Warnings = "⚠️ Puede causar inestabilidad en algunos juegos. Prueba primero.",
                Recommended = false,
                Risk = RiskLevel.Advanced,
                FpsGain = 5
            },
            ["cpu_anti_throttling"] = new TweakInfo
            {
                Id = "cpu_anti_throttling",
                Title = "CPU Anti-Throttling",
                Category = "Advanced",
                Description = "Fuerza CPU a 100% sin throttling. Máximo rendimiento constante.",
                Benefits = "• Rendimiento constante\n• Sin throttling de CPU",
                Warnings = "• Mayor consumo y temperatura",
                Recommended = false,
                Risk = RiskLevel.Moderate,
                FpsGain = 10
            },
            ["do_solo_mode"] = new TweakInfo
            {
                Id = "do_solo_mode",
                Title = "DO Solo Mode",
                Category = "Advanced",
                Description = "Configura Delivery Optimization para descargar actualizaciones solo desde la LAN local (Solo Mode), deshabilitando la compartición con internet.",
                Benefits = "• Evita uso de ancho de banda de subida\n• Reduce pings altos y lag intermitente\n• Conexión más limpia y estable",
                Warnings = "• Las descargas de updates se limitan a la LAN o servidores oficiales directamente.",
                Recommended = true,
                Risk = RiskLevel.Safe
            },
            ["raw_aim_curve"] = new TweakInfo
            {
                Id = "raw_aim_curve",
                Title = "Raw Aim Curve",
                Category = "Input & Visuals",
                Description = "Aplica una optimización de curva de puntería a nivel de registro que desactiva por completo la aceleración del mouse a nivel de kernel.",
                Benefits = "• Movimiento de mouse perfecto 1:1\n• Sin interpolación del sistema operativo\n• Ideal para shooters de precisión (Valorant, CS2)",
                Warnings = "• Requiere reiniciar el sistema para surtir efecto.",
                Recommended = true,
                Risk = RiskLevel.Safe
            },
            ["trim_force_on"] = new TweakInfo
            {
                Id = "trim_force_on",
                Title = "TRIM Force-On",
                Category = "Almacenamiento",
                Description = "Fuerza la activación de la función TRIM en el sistema de archivos de Windows para mantener los SSD limpios y funcionando a la máxima velocidad de lectura/escritura.",
                Benefits = "• Previene la degradación del rendimiento del SSD\n• Mantiene tiempos de carga ultra-rápidos\n• Ejecución automática en background",
                Warnings = "• Solo compatible con discos SSD. Sin efecto en HDD.",
                Recommended = true,
                Risk = RiskLevel.Safe
            },
            ["win32_priority"] = new TweakInfo
            {
                Id = "win32_priority",
                Title = "Win32 Priority Optimization",
                Category = "Competitive",
                Description = "Ajusta la prioridad del planificador Win32 para favorecer de forma óptima a las aplicaciones en primer plano (juegos) sobre procesos en segundo plano.",
                Benefits = "• Menos micro-stutters\n• Mayor suavidad en FPS competitivos\n• Prioridad de CPU enfocada en el juego",
                Warnings = "• Puede ralentizar levemente tareas de fondo exigentes.",
                Recommended = true,
                Risk = RiskLevel.Safe
            },
            ["core_power_plan"] = new TweakInfo
            {
                Id = "core_power_plan",
                Title = "Core Power Plan (Ultimate)",
                Category = "Laptop & Power",
                Description = "Fuerza el uso de la directiva de energía de máximo rendimiento (Ultimate Performance), deshabilitando los estados de reposo profundo del CPU.",
                Benefits = "• Núcleos de CPU siempre activos y listos\n• Cero delay al demandar potencia de procesamiento\n• Frame pacing perfecto",
                Warnings = "• Incrementa el consumo eléctrico y la temperatura del CPU.",
                Recommended = true,
                Risk = RiskLevel.Safe
            },
            ["usb_power_guard"] = new TweakInfo
            {
                Id = "usb_power_guard",
                Title = "USB Power Guard",
                Category = "Laptop & Power",
                Description = "Deshabilita la suspensión selectiva de los puertos USB para asegurar que los periféricos gaming de alto rendimiento no se apaguen.",
                Benefits = "• Estabilidad de polling rate en ratones/teclados\n• Sin micro-desconexiones durante el juego\n• Máxima respuesta de dispositivos USB",
                Warnings = "• Aumento mínimo en el consumo de energía.",
                Recommended = true,
                Risk = RiskLevel.Safe
            },
            ["stickykeys_guard"] = new TweakInfo
            {
                Id = "stickykeys_guard",
                Title = "StickyKeys Guard",
                Category = "Input & Visuals",
                Description = "Fuerza la desactivación persistente del atajo de accesibilidad Sticky Keys para evitar popups accidentales.",
                Benefits = "• Sin interrupciones en partidas competitivas\n• Desactivación limpia a nivel de registro",
                Warnings = "• Desactiva la función Sticky Keys.",
                Recommended = true,
                Risk = RiskLevel.Safe
            },
            ["hibernation_wipe"] = new TweakInfo
            {
                Id = "hibernation_wipe",
                Title = "Hibernation Wipe",
                Category = "Advanced",
                Description = "Deshabilita la hibernación y libera el espacio ocupado por hiberfil.sys en el almacenamiento principal.",
                Benefits = "• Libera varios gigabytes de espacio en disco\n• Elimina lecturas/escrituras de hibernación innecesarias",
                Warnings = "• El modo de suspensión de hibernación dejará de estar disponible.",
                Recommended = true,
                Risk = RiskLevel.Safe
            }
        };

        public static TweakInfo GetTweakInfo(string tweakId)
        {
            if (string.IsNullOrEmpty(tweakId)) return CreateDefaultTweakInfo(tweakId);
            return _tweaks.TryGetValue(tweakId.ToLowerInvariant(), out TweakInfo info) ? info : CreateDefaultTweakInfo(tweakId);
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
