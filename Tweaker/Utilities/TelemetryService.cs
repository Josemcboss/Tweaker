using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Tweaker.Utilities
{
    /// <summary>
    /// Servicio de telemetría interna (NO envía datos externos)
    /// Trackea uso de tweaks para mostrar estadísticas y recomendaciones
    /// </summary>
    public class TelemetryService
    {
        private static TelemetryService? _instance;
        private readonly string _telemetryFilePath;
        private TelemetryData _data;

        public static TelemetryService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TelemetryService();
                }
                return _instance;
            }
        }

        private TelemetryService()
        {
            _telemetryFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "GhostOptimizer",
                "telemetry.json"
            );

            LoadTelemetry();
        }

        private void LoadTelemetry()
        {
            try
            {
                if (File.Exists(_telemetryFilePath))
                {
                    string json = File.ReadAllText(_telemetryFilePath);
                    _data = JsonSerializer.Deserialize<TelemetryData>(json);
                }
                else
                {
                    _data = new TelemetryData();
                }
            }
            catch
            {
                _data = new TelemetryData();
            }
        }

        private void SaveTelemetry()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_telemetryFilePath));
                string json = JsonSerializer.Serialize(_data, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(_telemetryFilePath, json);
            }
            catch
            {
                // Silently fail
            }
        }

        /// <summary>
        /// Registra cuando se activa un tweak
        /// </summary>
        public void TrackTweakEnabled(string tweakId, string category)
        {
            _data.TotalTweaksApplied++;
            _data.LastActivityDate = DateTime.Now;

            // Registrar en historial
            var logEntry = new TweakLogEntry
            {
                TweakId = tweakId,
                Category = category,
                Action = "Enabled",
                Timestamp = DateTime.Now
            };
            _data.TweakHistory.Add(logEntry);

            // Limitar historial a últimos 1000 entries
            if (_data.TweakHistory.Count > 1000)
            {
                _data.TweakHistory.RemoveRange(0, 100);
            }

            // Incrementar contador de uso
            if (!_data.TweakUsageCount.ContainsKey(tweakId))
            {
                _data.TweakUsageCount[tweakId] = 0;
            }
            _data.TweakUsageCount[tweakId]++;

            // Incrementar contador de categoría
            if (!_data.CategoryUsageCount.ContainsKey(category))
            {
                _data.CategoryUsageCount[category] = 0;
            }
            _data.CategoryUsageCount[category]++;

            SaveTelemetry();
        }

        /// <summary>
        /// Registra cuando se desactiva un tweak
        /// </summary>
        public void TrackTweakDisabled(string tweakId, string category)
        {
            _data.LastActivityDate = DateTime.Now;

            var logEntry = new TweakLogEntry
            {
                TweakId = tweakId,
                Category = category,
                Action = "Disabled",
                Timestamp = DateTime.Now
            };
            _data.TweakHistory.Add(logEntry);

            if (_data.TweakHistory.Count > 1000)
            {
                _data.TweakHistory.RemoveRange(0, 100);
            }

            SaveTelemetry();
        }

        /// <summary>
        /// Registra apertura de la aplicación
        /// </summary>
        public void TrackAppLaunch()
        {
            _data.TotalLaunches++;
            _data.FirstLaunchDate ??= DateTime.Now;
            _data.LastLaunchDate = DateTime.Now;
            SaveTelemetry();
        }

        /// <summary>
        /// Registra navegación entre páginas
        /// </summary>
        public void TrackPageVisit(string pageName)
        {
            if (!_data.PageVisits.ContainsKey(pageName))
            {
                _data.PageVisits[pageName] = 0;
            }
            _data.PageVisits[pageName]++;
            SaveTelemetry();
        }

        /// <summary>
        /// Registra cuando se crea un punto de restauración
        /// </summary>
        public void TrackRestorePointCreated()
        {
            _data.RestorePointsCreated++;
            SaveTelemetry();
        }

        /// <summary>
        /// Obtiene los tweaks más usados
        /// </summary>
        public List<TweakUsageStats> GetMostUsedTweaks(int count = 10)
        {
            return _data.TweakUsageCount
                .OrderByDescending(kvp => kvp.Value)
                .Take(count)
                .Select(kvp => new TweakUsageStats
                {
                    TweakId = kvp.Key,
                    UsageCount = kvp.Value
                })
                .ToList();
        }

        /// <summary>
        /// Obtiene las categorías más usadas
        /// </summary>
        public List<CategoryUsageStats> GetCategoryUsage()
        {
            return _data.CategoryUsageCount
                .OrderByDescending(kvp => kvp.Value)
                .Select(kvp => new CategoryUsageStats
                {
                    CategoryName = kvp.Key,
                    UsageCount = kvp.Value
                })
                .ToList();
        }

        /// <summary>
        /// Obtiene el historial reciente de cambios
        /// </summary>
        public List<TweakLogEntry> GetRecentHistory(int count = 20)
        {
            return _data.TweakHistory
                .OrderByDescending(e => e.Timestamp)
                .Take(count)
                .ToList();
        }

        /// <summary>
        /// Obtiene estadísticas generales
        /// </summary>
        public AppStatistics GetAppStatistics()
        {
            var stats = new AppStatistics
            {
                TotalLaunches = _data.TotalLaunches,
                TotalTweaksApplied = _data.TotalTweaksApplied,
                RestorePointsCreated = _data.RestorePointsCreated,
                FirstLaunchDate = _data.FirstLaunchDate,
                LastLaunchDate = _data.LastLaunchDate,
                LastActivityDate = _data.LastActivityDate,
                DaysSinceFirstUse = _data.FirstLaunchDate.HasValue
                    ? (DateTime.Now - _data.FirstLaunchDate.Value).Days
                    : 0
            };

            return stats;
        }

        /// <summary>
        /// Obtiene la página más visitada
        /// </summary>
        public string GetMostVisitedPage()
        {
            if (_data.PageVisits.Count == 0)
                return "Dashboard";

            return _data.PageVisits.OrderByDescending(kvp => kvp.Value).First().Key;
        }

        /// <summary>
        /// Resetea toda la telemetría (útil para testing)
        /// </summary>
        public void ResetTelemetry()
        {
            _data = new TelemetryData();
            SaveTelemetry();
        }

        /// <summary>
        /// Exporta telemetría a archivo (para debug/support)
        /// </summary>
        public bool ExportTelemetry(string destinationPath)
        {
            try
            {
                string json = JsonSerializer.Serialize(_data, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(destinationPath, json);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    // ???????????????????????????????????????????????????????????????????
    // MODELOS DE DATOS
    // ???????????????????????????????????????????????????????????????????

    public class TelemetryData
    {
        public int TotalLaunches { get; set; } = 0;
        public int TotalTweaksApplied { get; set; } = 0;
        public int RestorePointsCreated { get; set; } = 0;
        public DateTime? FirstLaunchDate { get; set; }
        public DateTime? LastLaunchDate { get; set; }
        public DateTime? LastActivityDate { get; set; }

        public Dictionary<string, int> TweakUsageCount { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> CategoryUsageCount { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> PageVisits { get; set; } = new Dictionary<string, int>();
        public List<TweakLogEntry> TweakHistory { get; set; } = new List<TweakLogEntry>();
    }

    public class TweakLogEntry
    {
        public string? TweakId { get; set; }
        public string? Category { get; set; }
        public string? Action { get; set; } // "Enabled" or "Disabled"
        public DateTime Timestamp { get; set; }
    }

    public class TweakUsageStats
    {
        public string? TweakId { get; set; }
        public int UsageCount { get; set; }
    }

    public class CategoryUsageStats
    {
        public string? CategoryName { get; set; }
        public int UsageCount { get; set; }
    }

    public class AppStatistics
    {
        public int TotalLaunches { get; set; }
        public int TotalTweaksApplied { get; set; }
        public int RestorePointsCreated { get; set; }
        public DateTime? FirstLaunchDate { get; set; }
        public DateTime? LastLaunchDate { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public int DaysSinceFirstUse { get; set; }
    }
}
