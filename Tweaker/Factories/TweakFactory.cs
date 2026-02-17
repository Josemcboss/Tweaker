using System.Collections.Generic;
using Tweaker.Models;
using Tweaker.Data;

namespace Tweaker.Factories
{
    /// <summary>
    /// Factory para crear configuraciones de tweaks dinámicamente
    /// Elimina hardcoding y centraliza configuración
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
                "Desactivar Aceleración del Mouse",
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
                "Optimización TCP/IP Completa",
                "TcpAckFrequency = 1, TCPNoDelay = 1, NetworkThrottling OFF\nReduce ping 5-30ms, mejora hitreg, elimina packet loss"
            ));

            section.Tweaks.Add(CreateTweakModel(
                "dns_cloudflare",
                "DNS Cloudflare (1.1.1.1)",
                "DNS más rápido del mundo, latencia <10ms\nPrimario: 1.1.1.1 | Secundario: 1.0.0.1\nReduce ping 10-50ms, mejor resolución de dominios",
                useApplyMode: true
            ));

            section.Tweaks.Add(new TweakModel
            {
                Title = "DNS Google (8.8.8.8)",
                Description = "DNS confiable y estable, latencia ~15ms\nPrimario: 8.8.8.8 | Secundario: 8.8.4.4\nAlternativa probada, ideal para juegos en línea",
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
                Subtitle = "GPU, CPU y Configuración del Sistema",
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
                "Plan de Energía: Alto Rendimiento",
                "CPU siempre a máxima frecuencia"
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
                CreateSystemGpuSection()
                // Agregar más secciones según sea necesario
            };
        }
    }
}