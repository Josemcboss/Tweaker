using System;
using Microsoft.Win32;

namespace Tweaker.Optimizations
{
    public class SystemMitigationItem
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string StatusColor { get; set; } = "#107C10";
        public string ImpactOnFps { get; set; } = string.Empty;
    }

    public static class SystemMitigationDiagnostics
    {
        public static SystemMitigationItem CheckVbs()
        {
            bool isVbs = SystemSecurityOptimization.IsCoreIsolationEnabled();
            return new SystemMitigationItem
            {
                Name = "VBS / Core Isolation (HVCI)",
                Description = "Aislamiento de núcleo por hipervisor de Windows.",
                Status = isVbs ? "Activado (Seguro)" : "Desactivado (Máximo Rendimiento)",
                StatusColor = isVbs ? "#D83B01" : "#107C10",
                ImpactOnFps = isVbs ? "-5% a -15% FPS en CPUs Ryzen/Intel" : "0% (Máxima fluidez de frames)"
            };
        }

        public static SystemMitigationItem CheckHags()
        {
            bool isHags = false;
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\GraphicsDrivers", false);
                var val = key?.GetValue("HwSchMode");
                if (val is int intVal && intVal == 2) isHags = true;
            }
            catch { }

            return new SystemMitigationItem
            {
                Name = "HAGS (Hardware-Accelerated GPU Scheduling)",
                Description = "Planificación de GPU acelerada por hardware de Windows.",
                Status = isHags ? "Habilitado" : "Deshabilitado",
                StatusColor = isHags ? "#107C10" : "#D83B01",
                ImpactOnFps = isHags ? "Menor latencia de fotogramas (Frame Generation compatible)" : "Mayor latencia de GPU"
            };
        }

        public static SystemMitigationItem CheckGameBar()
        {
            bool? isMitigated = GameBarMitigationTweaks.IsGameBarMitigated();
            bool mitigated = isMitigated.HasValue && isMitigated.Value;
            return new SystemMitigationItem
            {
                Name = "GameBar DVR & Presence Writer",
                Description = "Captura fantasma en segundo plano y superposiciones de Xbox.",
                Status = mitigated ? "Mitigado / Desactivado" : "Activo en segundo plano",
                StatusColor = mitigated ? "#107C10" : "#D83B01",
                ImpactOnFps = mitigated ? "Sin microstuttering ni caídas de 1% lows" : "Puede causar stuttering en pantalla completa"
            };
        }

        public static SystemMitigationItem CheckSpectreMeltdown()
        {
            bool disabled = false;
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", false);
                var mask = key?.GetValue("FeatureSettingsOverrideMask");
                if (mask is int intMask && intMask == 3) disabled = true;
            }
            catch { }

            return new SystemMitigationItem
            {
                Name = "Spectre / Meltdown CPU Mitigations",
                Description = "Mitigaciones por software para vulnerabilidades de ejecución especulativa de CPU.",
                Status = disabled ? "Desactivadas (Gaming Puro)" : "Activas (Estándar Windows)",
                StatusColor = disabled ? "#107C10" : "#FFAA00",
                ImpactOnFps = disabled ? "Menor latencia en llamadas al kernel de CPU" : "Leve coste de CPU en I/O masivo"
            };
        }
    }
}

