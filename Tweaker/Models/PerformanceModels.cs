using System;

namespace Tweaker.Models
{
    /// <summary>
    /// Snapshot de métricas de rendimiento del sistema en un instante dado.
    /// Se usa para construir historial y comparativas antes/después.
    /// </summary>
    public class PerformanceSnapshot
    {
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public float CpuPercent { get; set; }
        public float RamPercent { get; set; }
        public float GpuPercent { get; set; }
        public float CpuTempC { get; set; }
        public float GpuTempC { get; set; }
        public int NetworkLatencyMs { get; set; }
        public int ActiveTweaksCount { get; set; }
        public string Label { get; set; } = string.Empty;
    }

    /// <summary>
    /// Registro de una acción de mantenimiento automático ejecutada por el sistema.
    /// </summary>
    public class MaintenanceLog
    {
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string Action { get; set; } = string.Empty;
        public long MBFreed { get; set; }
        public int FilesDeleted { get; set; }
        public bool Success { get; set; }
        public string Details { get; set; } = string.Empty;
    }
}
