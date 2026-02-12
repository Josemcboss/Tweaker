using System.Collections.Generic;
using Tweaker.Models;

namespace Tweaker.Factories
{
    /// <summary>
    /// Factory para crear configuraciones de tweaks dinámicamente
    /// Elimina hardcoding y centraliza configuración
    /// </summary>
    public static class TweakFactory
    {
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

            section.Tweaks.Add(new TweakModel
            {
                Title = "Desactivar Aceleración del Mouse",
                Description = "MouseSpeed = 0, Thresholds = 0\nAim 1:1 pixel perfect, muscle memory consistente",
                IsRecommended = true,
                ShowInfoButton = true,
                TweakId = "mouse_acceleration",
                UseApplyMode = false
            });

            section.Tweaks.Add(new TweakModel
            {
                Title = "Optimizar Teclado",
                Description = "KeyboardDelay = 0, Input lag -50ms",
                IsRecommended = true,
                ShowInfoButton = true,
                TweakId = "keyboard_optimization",
                UseApplyMode = false
            });

            section.Tweaks.Add(new TweakModel
            {
                Title = "Efectos Visuales OFF",
                Description = "FPS +3-8%, GPU +5-10%",
                IsRecommended = true,
                ShowInfoButton = true,
                TweakId = "visual_effects",
                UseApplyMode = false
            });

            section.Tweaks.Add(new TweakModel
            {
                Title = "Optimizar RAM",
                Description = "DisablePagingExecutive = 1, Requiere 16GB+",
                IsRecommended = false,
                ShowInfoButton = true,
                TweakId = "memory_optimization",
                UseApplyMode = false
            });

            section.Tweaks.Add(new TweakModel
            {
                Title = "Disable Transparency Effects",
                Description = "Deshabilita efectos de transparencia de Windows\nGPU usage -3-8%, VRAM +50-200MB liberada",
                IsRecommended = true,
                ShowInfoButton = true,
                TweakId = "transparency_effects",
                UseApplyMode = false
            });

            section.Tweaks.Add(new TweakModel
            {
                Title = "Deshabilitar Sticky Keys",
                Description = "Elimina popups molestos (Shift x5, Num Lock hold)\nGaming sin interrupciones",
                IsRecommended = true,
                ShowInfoButton = true,
                TweakId = "sticky_keys",
                UseApplyMode = false
            });

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

            section.Tweaks.Add(new TweakModel
            {
                Title = "Optimización TCP/IP Completa",
                Description = "TcpAckFrequency = 1, TCPNoDelay = 1, NetworkThrottling OFF\nReduce ping 5-30ms, mejora hitreg, elimina packet loss",
                IsRecommended = false, // Marcado como avanzado
                ShowInfoButton = true,
                TweakId = "network_optimization",
                UseApplyMode = false
            });

            section.Tweaks.Add(new TweakModel
            {
                Title = "DNS Cloudflare (1.1.1.1)",
                Description = "DNS más rápido del mundo, latencia <10ms\nPrimario: 1.1.1.1 | Secundario: 1.0.0.1\nReduce ping 10-50ms, mejor resolución de dominios",
                IsRecommended = true,
                ShowInfoButton = true,
                TweakId = "dns_cloudflare",
                UseApplyMode = true // Usa botón APLICAR en lugar de ON/OFF
            });

            section.Tweaks.Add(new TweakModel
            {
                Title = "DNS Google (8.8.8.8)",
                Description = "DNS confiable y estable, latencia ~15ms\nPrimario: 8.8.8.8 | Secundario: 8.8.4.4\nAlternativa probada, ideal para juegos en línea",
                IsRecommended = true,
                ShowInfoButton = false,
                TweakId = "dns_google",
                UseApplyMode = true
            });

            section.Tweaks.Add(new TweakModel
            {
                Title = "Optimizar Caché DNS",
                Description = "MaxCacheTtl = 86400, NegativeCacheTime = 0\nMejora velocidad de resolución, reduce consultas",
                IsRecommended = true,
                ShowInfoButton = true,
                TweakId = "dns_cache",
                UseApplyMode = false
            });

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

            section.Tweaks.Add(new TweakModel
            {
                Title = "System Profile Games Priority",
                Description = "GPU Priority: 8, CPU Priority: 6, Scheduling: High",
                IsRecommended = true,
                ShowInfoButton = true,
                TweakId = "system_profile",
                UseApplyMode = false
            });

            section.Tweaks.Add(new TweakModel
            {
                Title = "Deshabilitar GameDVR (Xbox Game Bar)",
                Description = "Elimina overlay, reduce input lag 5-15ms",
                IsRecommended = true,
                ShowInfoButton = true,
                TweakId = "gamedvr_disable",
                UseApplyMode = false
            });

            section.Tweaks.Add(new TweakModel
            {
                Title = "Hardware GPU Scheduling",
                Description = "Puede mejorar o empeorar latencia (probar ambos)",
                IsRecommended = false, // Efecto impredecible
                ShowInfoButton = true,
                TweakId = "gpu_scheduling",
                UseApplyMode = false
            });

            section.Tweaks.Add(new TweakModel
            {
                Title = "Plan de Energía: Alto Rendimiento",
                Description = "CPU siempre a máxima frecuencia",
                IsRecommended = true,
                ShowInfoButton = true,
                TweakId = "high_performance",
                UseApplyMode = false
            });

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