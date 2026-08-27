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
                Description = "Deshabilita Multiplane Overlay para eliminar stuttering y pantallazos negros. Fuerza modo legacy más estable en composición de ventanas.",
                Benefits = "• Elimina stuttering por MPO\n• Sin pantallazos negros al Alt+Tab\n• Frame pacing más consistente\n• Overlays funcionan sin problemas\n• Mejor compatibilidad G-Sync/FreeSync",
                Warnings = "• REQUIERE REINICIO\n• Posible ligero aumento uso GPU\n• Algunos sistemas pueden no necesitarlo",
                Recommended = false,
                Risk = RiskLevel.Moderate,
                RequiresRestart = true,
                FpsGain = 2,
                PingReduction = 0,
                RamFreedGB = 0
            },

            ["hyperv_disable"] = new TweakInfo
            {
                Id = "hyperv_disable",
                Title = "Deshabilitar Hyper-V",
                Category = "GHOST Pack",
                Description = "Deshabilita el hipervisor de Windows para eliminar capas de abstracción y reducir latencia en GPU y renderizado.",
                Benefits = "• Latencia GPU -2-5ms\n• Compatibilidad anti-cheat (Vanguard/EAC/BattlEye)\n• 0.1% low FPS mejorados",
                Warnings = "• REQUIERE REINICIO\n• Inhabilita Docker Desktop, WSL2 y Windows Sandbox",
                Recommended = false,
                Risk = RiskLevel.Advanced,
                RequiresRestart = true,
                FpsGain = 5,
                PingReduction = 0,
                RamFreedGB = 0
            },

            ["apex_gaming_power_plan"] = new TweakInfo
            {
                Id = "apex_gaming_power_plan",
                Title = "Plan: Apex Ultra Gaming",
                Category = "GHOST Pack",
                Description = "Activa el esquema de energía dedicado Ultra Gaming con Core Parking al 0% (100% de núcleos activos), Power Throttling global desactivado y PCIe ASPM Link State Power Management apagado.",
                Benefits = "• 100% núcleos CPU activos todo el tiempo\n• Frecuencia de CPU al máximo sin fluctuaciones\n• Cero latencia por transiciones de energía\n• Latencia DPC ultra-baja",
                Warnings = "• Mayor consumo energético en laptops\n• Temperaturas ligeramente más altas en reposo",
                Recommended = true,
                Risk = RiskLevel.Safe,
                RequiresRestart = false,
                FpsGain = 12,
                PingReduction = 5,
                RamFreedGB = 0
            },

            ["gpu_driver_telemetry_clean"] = new TweakInfo
            {
                Id = "gpu_driver_telemetry_clean",
                Title = "Drivers GPU - Sin Telemetría",
                Category = "GHOST Pack",
                Description = "Deshabilita tareas programadas de telemetría y recolección de crash reporters de NVIDIA y AMD al estilo NVCleanStall.",
                Benefits = "• Menos procesos en segundo plano de GPU\n• DPC latency reducida\n• Cero llamadas de telemetría a servidores de NVIDIA/AMD",
                Warnings = "• Desactiva el reporte automático de errores de GPU",
                Recommended = true,
                Risk = RiskLevel.Safe,
                RequiresRestart = false,
                FpsGain = 3,
                PingReduction = 0,
                RamFreedGB = 0
            },

            ["safe_mode_ddu_prep"] = new TweakInfo
            {
                Id = "safe_mode_ddu_prep",
                Title = "Preparar Modo Seguro (DDU Helper)",
                Category = "GHOST Pack",
                Description = "Configura el arranque de Windows para iniciar en Modo Seguro Mínimo, facilitando una limpieza 100% libre de residuos con Display Driver Uninstaller.",
                Benefits = "• Arranque directo en Safe Mode sin pulsar F8 o Shift\n• Permite desinstalación limpia sin drivers bloqueados en memoria\n• Previene corrupción de drivers al actualizar GPU",
                Warnings = "• El próximo reinicio entrará en Modo Seguro (desactivar switch para volver al modo normal)",
                Recommended = false,
                Risk = RiskLevel.Moderate,
                RequiresRestart = true,
                FpsGain = 0,
                PingReduction = 0,
                RamFreedGB = 0
            },

            ["text_input_host_disable"] = new TweakInfo
            {
                Id = "text_input_host_disable",
                Title = "Desactivar TextInputHost.exe",
                Category = "GHOST Pack",
                Description = "Mitiga y suprime la ejecución en segundo plano de TextInputHost.exe (teclado táctil/panel de emojis de Windows) usando IFEO.",
                Benefits = "• Elimina micro-congelamientos y picos de latencia en juegos competitivos\n• Menos hilos de sistema compitiendo por ciclos de CPU",
                Warnings = "• Puede afectar el teclado táctil en pantallas touch",
                Recommended = true,
                Risk = RiskLevel.Safe,
                RequiresRestart = false,
                FpsGain = 4,
                PingReduction = 0,
                RamFreedGB = 0
            },

            ["hvci_disable"] = new TweakInfo
            {
                Id = "hvci_disable",
                Title = "Desactivar HVCI (Memory Integrity)",
                Category = "GHOST Pack",
                Description = "Deshabilita Hypervisor-Protected Code Integrity de Windows Defender para maximizar el throughput de memoria y CPU.",
                Benefits = "• FPS +10-25% en procesadores Ryzen e Intel\n• Latencia de memoria drásticamente menor\n• Elimina sobrecarga del hipervisor de seguridad",
                Warnings = "• REQUIERE REINICIO\n• Reduce aislamiento de seguridad del kernel contra drivers no firmados",
                Recommended = false,
                Risk = RiskLevel.Advanced,
                RequiresRestart = true,
                FpsGain = 15,
                PingReduction = 0,
                RamFreedGB = 0
            },

            ["interrupt_steering"] = new TweakInfo
            {
                Id = "interrupt_steering",
                Title = "Interrupt Steering (Distribución IRQ)",
                Category = "GHOST Pack",
                Description = "Configura el kernel de Windows para distribuir timers e interrupciones de hardware entre varios núcleos en lugar de saturar el Core 0.",
                Benefits = "• Libera el Core 0 para el hilo principal del juego\n• Frame times mucho más estables\n• Reduce saturación por interrupciones de red y periféricos USB",
                Warnings = "• REQUIERE REINICIO\n• En algunos CPUs muy antiguos puede no generar diferencia medible",
                Recommended = true,
                Risk = RiskLevel.Safe,
                RequiresRestart = true,
                FpsGain = 6,
                PingReduction = 3,
                RamFreedGB = 0
            },

            ["gpu_irq_affinity"] = new TweakInfo
            {
                Id = "gpu_irq_affinity",
                Title = "GPU IRQ Affinity (Último Core)",
                Category = "GHOST Pack",
                Description = "Asigna la interrupción y procesamiento de driver de la GPU dedicada al último núcleo del procesador.",
                Benefits = "• DPC Latency minimizada\n• El hilo de renderizado no compite con el hilo principal del juego\n• 0.1% low FPS más altos y estables",
                Warnings = "• REQUIERE REINICIO OBLIGATORIO\n• Requiere al menos 4 núcleos en el procesador",
                Recommended = true,
                Risk = RiskLevel.Moderate,
                RequiresRestart = true,
                FpsGain = 8,
                PingReduction = 0,
                RamFreedGB = 0
            },

            ["spectre_meltdown_disable"] = new TweakInfo
            {
                Id = "spectre_meltdown_disable",
                Title = "Desactivar Mitigaciones Spectre/Meltdown",
                Category = "GHOST Pack",
                Description = "Deshabilita las mitigaciones por software de Spectre v2 y Meltdown para recuperar el rendimiento nativo del procesador.",
                Benefits = "• Ganancia de +5-15% IPC y FPS en CPUs Intel y AMD\n• Llamadas al sistema y context switching ultrarrápidos\n• Menor overhead en DirectX 11/12",
                Warnings = "• REQUIERE REINICIO\n• Expone el procesador a vulnerabilidades de ejecución especulativa (solo para gaming)",
                Recommended = false,
                Risk = RiskLevel.Advanced,
                RequiresRestart = true,
                FpsGain = 10,
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
            },
            ["text_input_host_disable"] = new TweakInfo
            {
                Id = "text_input_host_disable",
                Title = "Desactivar TextInputHost (KernelOS)",
                Category = "Advanced",
                Description = "Deshabilita la ejecución en segundo plano del proceso TextInputHost.exe para reducir latencia e interrupciones de CPU.",
                Benefits = "• Menos procesos consumiendo ciclo de CPU\n• Reducción de latencia en juegos competitivos",
                Warnings = "• Puede afectar el teclado en pantalla táctil de Windows.",
                Recommended = true,
                Risk = RiskLevel.Moderate
            },
            ["hop_limit_opt"] = new TweakInfo
            {
                Id = "hop_limit_opt",
                Title = "Optimizar Hop Limit TCP/IP (KernelOS)",
                Category = "Red & Ping",
                Description = "Ajusta el TTL (DefaultTTL = 64) en la pila TCP/IP para optimizar el enrutamiento de paquetes.",
                Benefits = "• Mejor consistencia en ruteo de red\n• Reducción potencial de saltos innecesarios",
                Warnings = "• Ninguno en conexiones estándar de internet.",
                Recommended = true,
                Risk = RiskLevel.Safe,
                PingReduction = 2
            },
            ["hvci_disable"] = new TweakInfo
            {
                Id = "hvci_disable",
                Title = "Desactivar HVCI / Memory Integrity (KernelOS)",
                Category = "Advanced",
                Description = "Desactiva Hypervisor-Protected Code Integrity (HVCI) para recuperar rendimiento de CPU y eliminar micro-stuttering por virtualización.",
                Benefits = "• Incremento de 5-15% en rendimiento de CPU\n• Disminución drástica de micro-stutters",
                Warnings = "• Disminuye el aislamiento de seguridad por hipervisor.",
                Recommended = true,
                Risk = RiskLevel.Advanced,
                FpsGain = 10
            },

            // ────────────────────────────────────────────
            // WINUTIL INTEGRATED TWEAKS (ChrisTitusTech)
            // ────────────────────────────────────────────

            ["winutil_activity_feed"] = new TweakInfo
            {
                Id = "winutil_activity_feed",
                Title = "Desactivar Historial de Actividad (WinUtil)",
                Category = "WinUtil & Debloat",
                Description = "Elimina la recolección y sincronización en la nube del historial de actividades, documentos recientes y portapapeles de Windows.",
                Benefits = "• Elimina la telemetría de uso del usuario\n• Libera recursos del sistema y disco\n• Mayor privacidad",
                Warnings = "• Desactiva la sincronización del historial entre dispositivos",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            ["winutil_hibernation"] = new TweakInfo
            {
                Id = "winutil_hibernation",
                Title = "Desactivar Hibernación (WinUtil)",
                Category = "WinUtil & Debloat",
                Description = "Desactiva la hibernación de Windows y elimina el archivo hiberfil.sys liberando varios Gigabytes en el disco principal.",
                Benefits = "• Libera de 4GB a 32GB de espacio en disco SSD/NVMe\n• Elimina el desgaste innecesario en SSD",
                Warnings = "• La función de inicio rápido/hibernar no estará disponible (ideal para PCs de escritorio)",
                Recommended = true,
                Risk = RiskLevel.Safe,
                RamFreedGB = 8.0
            },

            ["winutil_end_task"] = new TweakInfo
            {
                Id = "winutil_end_task",
                Title = "Habilitar 'Finalizar Tarea' en Barra de Tareas (WinUtil)",
                Category = "WinUtil & Debloat",
                Description = "Agrega la opción nativa al menú contextual al hacer clic derecho en una aplicación de la barra de tareas para cerrarla inmediatamente sin abrir el Administrador de Tareas.",
                Benefits = "• Permite forzar el cierre instantáneo de juegos o apps congeladas",
                Warnings = "• Cierre forzado de la aplicación sin guardar cambios no guardados",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            ["winutil_wpbt"] = new TweakInfo
            {
                Id = "winutil_wpbt",
                Title = "Desactivar Inyección OEM WPBT (WinUtil)",
                Category = "WinUtil & Debloat",
                Description = "Desactiva el ejecutable Windows Platform Binary Table (WPBT) en BIOS/UEFI que instala software de fabricante en segundo plano sin permiso.",
                Benefits = "• Evita la inyección de bloatware OEM de fábrica en Windows",
                Warnings = "• Ninguno en sistemas personalizados o de escritorio",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            ["winutil_location"] = new TweakInfo
            {
                Id = "winutil_location",
                Title = "Desactivar Rastreo de Ubicación (WinUtil)",
                Category = "WinUtil & Debloat",
                Description = "Desactiva el servicio de geolocalización `lfsvc` y bloquea el acceso de aplicaciones a la ubicación del dispositivo.",
                Benefits = "• Detiene servicios en segundo plano consumiendo red y CPU",
                Warnings = "• Apps como Mapas o Clima no detectarán tu ubicación automática",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            ["winutil_rdp_warnings"] = new TweakInfo
            {
                Id = "winutil_rdp_warnings",
                Title = "Desactivar Advertencias de RDP no firmados (WinUtil)",
                Category = "WinUtil & Debloat",
                Description = "Elimina los molestos diálogos de confirmación al abrir archivos de conexión a Escritorio Remoto (.rdp).",
                Benefits = "• Acceso directo y más rápido a conexiones RDP",
                Warnings = "• No te advertirá al conectar a servidores sin certificado verificado",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            ["winutil_svchost_split"] = new TweakInfo
            {
                Id = "winutil_svchost_split",
                Title = "Ajustar SvcHost Split Threshold por RAM (WinUtil)",
                Category = "WinUtil & Debloat",
                Description = "Configura SvcHostSplitThresholdInKB en el Registro según la memoria RAM física instalada, reduciendo la cantidad excesiva de procesos svchost.exe.",
                Benefits = "• Reduce drásticamente la sobrecarga de procesos svchost en segundo plano\n• Menor consumo de memoria RAM y CPU",
                Warnings = "• Ninguno, adaptado automáticamente a la RAM de tu PC",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            ["winutil_brave_debloat"] = new TweakInfo
            {
                Id = "winutil_brave_debloat",
                Title = "Brave Browser - Debloat Completo (WinUtil)",
                Category = "WinUtil & Debloat",
                Description = "Desactiva Brave Rewards, Crypto Wallet, VPN integradas, Leo AI Chat, Brave News y telemetría mediante directivas de grupo.",
                Benefits = "• Navegador Brave ultra limpio y ligero\n• Menos consumo de memoria RAM y procesos secundarios en background",
                Warnings = "• Desactiva funciones Web3/Cripto/VPN nativas del navegador Brave",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            ["winutil_edge_debloat"] = new TweakInfo
            {
                Id = "winutil_edge_debloat",
                Title = "Microsoft Edge - Debloat Completo (WinUtil)",
                Category = "WinUtil & Debloat",
                Description = "Desactiva compras de Edge, botón de Copilot/Bing, barra lateral Hubs, colecciones y envío de telemetría de navegación.",
                Benefits = "• Microsoft Edge significativamente más rápido y sin elementos molestos",
                Warnings = "• Oculta el asistente Copilot y la barra lateral de Edge",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            ["winutil_remove_widgets"] = new TweakInfo
            {
                Id = "winutil_remove_widgets",
                Title = "Eliminar Widgets de Windows 11 (WinUtil)",
                Category = "WinUtil & Debloat",
                Description = "Elimina los paquetes AppX de Widgets y WebExperience de la barra de tareas de Windows 11.",
                Benefits = "• Libera CPU y RAM consumida por el feed de noticias/widgets\n• Elimina el icono innecesario de la barra de tareas",
                Warnings = "• Los widgets no estarán disponibles en la barra de tareas",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            // ────────────────────────────────────────────
            // PARAGON TWEAKING UTILITY (PTU) TWEAKS
            // ────────────────────────────────────────────

            ["game_shader_cache_clean"] = new TweakInfo
            {
                Id = "game_shader_cache_clean",
                Title = "Limpieza de Shader Cache (DirectX, NVIDIA, AMD)",
                Category = "PTU Gaming Suite",
                Description = "Limpia y purga las carpetas de caché de sombreadores (DXCache, GLCache, ComputeCache, D3DSCache) y cachés temporales de Unreal Engine / Fortnite / Valorant / Apex.",
                Benefits = "• Elimina micro-stuttering y caídas súbitas de FPS por shaders corruptos\n• Libera cientos de MB o GB en disco\n• Fuerza compilación limpia y fluida",
                Warnings = "• Los primeros minutos de juego tras la limpieza pueden tardar unos segundos en recompilar shaders",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            ["game_profiles_latency"] = new TweakInfo
            {
                Id = "game_profiles_latency",
                Title = "Optimización de Perfiles de Juegos y Latencia DWM",
                Category = "PTU Gaming Suite",
                Description = "Desactiva GameDVR, optimizaciones de pantalla completa problemáticas y fuerza el perfil de GPU DirectX en modo Alto Rendimiento para todos los juegos.",
                Benefits = "• Reduce input lag en 5-15ms en modo pantalla completa\n• Elimina latencia y stuttering del compositor DWM\n• Prioridad de GPU máxima en procesos de juego",
                Warnings = "• Desactiva la barra de juegos de Xbox (Game Bar)",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            ["discord_gamer_optimization"] = new TweakInfo
            {
                Id = "discord_gamer_optimization",
                Title = "Discord - Optimización Gamer (Sin Lag)",
                Category = "PTU Gaming Suite",
                Description = "Desactiva la aceleración por hardware en Discord y el overlay dentro de los juegos, evitando que Discord robe recursos del codificador y memoria de video.",
                Benefits = "• Previene caídas de FPS mientras hablas en llamadas de Discord\n• Elimina congelamientos y micro-cortes por overlay\n• Libera VRAM de tu GPU",
                Warnings = "• La interfaz de Discord usará renderizado por software (CPU)",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            ["spotify_gamer_optimization"] = new TweakInfo
            {
                Id = "spotify_gamer_optimization",
                Title = "Spotify - Optimización de Rendimiento",
                Category = "PTU Gaming Suite",
                Description = "Desactiva la aceleración por GPU del cliente web de Spotify para no interferir con los FPS del juego.",
                Benefits = "• Cero interferencia de GPU mientras escuchas música jugando\n• Mayor estabilidad de frame times",
                Warnings = "• Ninguno perceptible en el uso diario",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            ["browser_gamer_background"] = new TweakInfo
            {
                Id = "browser_gamer_background",
                Title = "Navegadores - Suspender Procesos en Fondo",
                Category = "PTU Gaming Suite",
                Description = "Configura directivas para Chrome, Edge y Brave para evitar que mantengan procesos y extensiones activos en segundo plano tras cerrarse.",
                Benefits = "• Libera 500MB - 2GB de RAM al cerrar el navegador\n• Cero uso de CPU residual durante tus partidas",
                Warnings = "• Las aplicaciones web no enviarán notificaciones con el navegador cerrado",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            ["apex_gaming_power_plan"] = new TweakInfo
            {
                Id = "apex_gaming_power_plan",
                Title = "Plan de Energía Apex / Ultra Gaming",
                Category = "PTU Gaming Suite",
                Description = "Activa un plan de energía ultra afinado con Core Parking al 0% (100% de núcleos listos), Power Throttling desactivado y latencia de transición C-State instantánea.",
                Benefits = "• Frecuencias de CPU fijas y estables sin caídas térmicas ni de ahorro\n• Tiempos de frame consistentes (1% y 0.1% lows más altos)\n• Respuesta de periféricos instantánea",
                Warnings = "• Mayor consumo eléctrico en portátiles (usar conectado a corriente)",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            ["gpu_driver_telemetry_clean"] = new TweakInfo
            {
                Id = "gpu_driver_telemetry_clean",
                Title = "Drivers GPU - Eliminar Telemetría (NVIDIA / AMD)",
                Category = "PTU Gaming Suite",
                Description = "Desactiva las tareas programadas de telemetría y crash reporters en segundo plano instalados por los controladores de GPU (estilo NVCleanStall).",
                Benefits = "• Menos procesos en segundo plano ejecutándose\n• Mayor privacidad y estabilidad de DPC latency",
                Warnings = "• No afecta el panel de control de NVIDIA ni el software Adrenalin",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            ["safe_mode_ddu_prep"] = new TweakInfo
            {
                Id = "safe_mode_ddu_prep",
                Title = "Preparar Reinicio en Modo Seguro (DDU Helper)",
                Category = "PTU Gaming Suite",
                Description = "Configura el arranque de Windows en Modo Seguro mínimo para poder ejecutar Display Driver Uninstaller (DDU) y limpiar drivers corruptos.",
                Benefits = "• Limpieza de drivers 100% libre de archivos bloqueados\n• Solución definitiva para crashes y BSODs por drivers de video",
                Warnings = "• El sistema iniciará en Modo Seguro en el próximo reinicio (puedes restaurar con el switch)",
                Recommended = false,
                Risk = RiskLevel.Moderate
            },

            ["system_file_checker"] = new TweakInfo
            {
                Id = "system_file_checker",
                Title = "Reparar Archivos Corruptos de Windows (SFC /scannow)",
                Category = "PTU Gaming Suite",
                Description = "Ejecuta el Comprobador de Archivos de Sistema (SFC) para reparar archivos dañados o modificados que puedan causar errores o inestabilidad.",
                Benefits = "• Repara DLLs y archivos críticos dañados de Windows\n• Mejora la estabilidad general del sistema",
                Warnings = "• Puede tardar entre 2 y 5 minutos en completar el escaneo",
                Recommended = true,
                Risk = RiskLevel.Safe
            },

            ["dism_restore_health"] = new TweakInfo
            {
                Id = "dism_restore_health",
                Title = "Reparar Imagen del Sistema (DISM RestoreHealth)",
                Category = "PTU Gaming Suite",
                Description = "Repara la imagen de componentes de Windows mediante el almacén de componentes en línea (DISM /Cleanup-Image /RestoreHealth).",
                Benefits = "• Corrige corrupciones profundas de la imagen de Windows\n• Requisito ideal antes o después de aplicar optimizaciones mayores",
                Warnings = "• Requiere conexión a internet activa y puede tardar varios minutos",
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
