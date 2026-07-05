using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;

using Tweaker.Models;
using Tweaker.Utilities;

namespace Tweaker.Services
{
    /// <summary>
    /// Servicio de métricas de rendimiento.
    /// Toma snapshots periódicos del sistema y mantiene un historial
    /// para comparativas antes/después de optimizaciones.
    /// </summary>
    public class PerformanceMetricsService : INotifyPropertyChanged, IDisposable
    {
        #region Singleton

        private static PerformanceMetricsService? _instance;
        private static readonly object _lock = new();

        public static PerformanceMetricsService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock) { _instance ??= new PerformanceMetricsService(); }
                }
                return _instance;
            }
        }

        #endregion

        #region Fields

        private readonly Timer _snapshotTimer;
        private readonly List<PerformanceSnapshot> _history = new();
        private readonly string _historyPath;
        private readonly PerformanceCounter? _cpuCounter;
        private PerformanceSnapshot _current = new();
        private bool _isRecording;

        // Estadísticas calculadas
        private float _avgCpuBefore;
        private float _avgCpuAfter;
        private float _avgRamBefore;
        private float _avgRamAfter;
        private int _avgLatencyBefore;
        private int _avgLatencyAfter;

        #endregion

        #region Properties

        public PerformanceSnapshot Current
        {
            get => _current;
            private set { _current = value; OnPropertyChanged(); }
        }

        public bool IsRecording
        {
            get => _isRecording;
            set
            {
                if (_isRecording != value)
                {
                    _isRecording = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<PerformanceSnapshot> History => _history.ToList();
        public int SnapshotCount => _history.Count;

        // Comparativas Antes/Después
        public float AvgCpuBefore { get => _avgCpuBefore; private set { _avgCpuBefore = value; OnPropertyChanged(); } }
        public float AvgCpuAfter { get => _avgCpuAfter; private set { _avgCpuAfter = value; OnPropertyChanged(); } }
        public float AvgRamBefore { get => _avgRamBefore; private set { _avgRamBefore = value; OnPropertyChanged(); } }
        public float AvgRamAfter { get => _avgRamAfter; private set { _avgRamAfter = value; OnPropertyChanged(); } }
        public int AvgLatencyBefore { get => _avgLatencyBefore; private set { _avgLatencyBefore = value; OnPropertyChanged(); } }
        public int AvgLatencyAfter { get => _avgLatencyAfter; private set { _avgLatencyAfter = value; OnPropertyChanged(); } }

        // Deltas calculados
        public float CpuDelta => AvgCpuBefore - AvgCpuAfter;
        public float RamDelta => AvgRamBefore - AvgRamAfter;
        public int LatencyDelta => AvgLatencyBefore - AvgLatencyAfter;

        // Últimos 10 snapshots para mini-gráfico
        public List<PerformanceSnapshot> RecentSnapshots =>
            _history.TakeLast(10).ToList();

        #endregion

        #region Events

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<PerformanceSnapshot>? SnapshotTaken;

        #endregion

        private PerformanceMetricsService()
        {
            var appData = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "GhostOptimizer");
            Directory.CreateDirectory(appData);
            _historyPath = Path.Combine(appData, "performance_history.json");

            // Inicializar PerformanceCounter para CPU
            try
            {
                _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                _cpuCounter.NextValue(); // Primera lectura siempre devuelve 0
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"⚠️ PerformanceMetrics: No se pudo crear CPU counter: {ex.Message}");
            }

            LoadHistory();

            // Timer cada 5 segundos para snapshot rápido (solo para UI)
            _snapshotTimer = new Timer(TakeQuickSnapshot, null, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5));
            _isRecording = true;
        }

        #region Snapshot Methods

        private void TakeQuickSnapshot(object? state)
        {
            try
            {
                var snapshot = new PerformanceSnapshot
                {
                    Timestamp = DateTime.Now,
                    CpuPercent = GetCpuUsage(),
                    RamPercent = GetRamUsage(),
                    NetworkLatencyMs = GetNetworkLatency(),
                    ActiveTweaksCount = TweakStateManager.Instance.ActiveTweaksCount
                };

                Current = snapshot;
                SnapshotTaken?.Invoke(this, snapshot);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"⚠️ PerformanceMetrics: Error en snapshot: {ex.Message}");
            }
        }

        /// <summary>
        /// Toma un snapshot completo y lo guarda en historial (se llama manualmente o cada X minutos).
        /// </summary>
        public void TakeFullSnapshot(string label = "")
        {
            try
            {
                var snapshot = new PerformanceSnapshot
                {
                    Timestamp = DateTime.Now,
                    CpuPercent = GetCpuUsage(),
                    RamPercent = GetRamUsage(),
                    NetworkLatencyMs = GetNetworkLatency(),
                    ActiveTweaksCount = TweakStateManager.Instance.ActiveTweaksCount,
                    Label = label
                };

                _history.Add(snapshot);

                // Mantener máximo 500 snapshots
                if (_history.Count > 500)
                {
                    _history.RemoveRange(0, _history.Count - 500);
                }

                SaveHistory();
                RecalculateComparisons();

                OnPropertyChanged(nameof(History));
                OnPropertyChanged(nameof(SnapshotCount));
                OnPropertyChanged(nameof(RecentSnapshots));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ PerformanceMetrics: Error guardando snapshot: {ex.Message}");
            }
        }

        /// <summary>
        /// Toma un snapshot de "baseline" (antes de optimizar).
        /// </summary>
        public void TakeBaselineSnapshot()
        {
            TakeFullSnapshot("BASELINE");
        }

        /// <summary>
        /// Toma un snapshot "after" (después de optimizar).
        /// </summary>
        public void TakeOptimizedSnapshot()
        {
            TakeFullSnapshot("OPTIMIZED");
        }

        #endregion

        #region Metric Getters

        private float GetCpuUsage()
        {
            try
            {
                return _cpuCounter?.NextValue() ?? 0f;
            }
            catch
            {
                return 0f;
            }
        }

        private float GetRamUsage()
        {
            try
            {
                var memInfo = MemoryCleaner.GetSystemMemoryInfo();
                return memInfo.percentUsed;
            }
            catch
            {
                return 0f;
            }
        }

        private int GetNetworkLatency()
        {
            try
            {
                using var ping = new Ping();
                // Ping a Google DNS para latencia de referencia
                var reply = ping.Send("8.8.8.8", 1000);
                if (reply.Status == IPStatus.Success)
                    return (int)reply.RoundtripTime;
            }
            catch { }
            return -1;
        }

        #endregion

        #region Comparisons

        private void RecalculateComparisons()
        {
            try
            {
                var baselines = _history.Where(s => s.Label == "BASELINE").TakeLast(5).ToList();
                var optimized = _history.Where(s => s.Label == "OPTIMIZED").TakeLast(5).ToList();

                if (baselines.Any())
                {
                    AvgCpuBefore = baselines.Average(s => s.CpuPercent);
                    AvgRamBefore = baselines.Average(s => s.RamPercent);
                    AvgLatencyBefore = (int)baselines.Where(s => s.NetworkLatencyMs > 0).DefaultIfEmpty(new PerformanceSnapshot()).Average(s => s.NetworkLatencyMs);
                }

                if (optimized.Any())
                {
                    AvgCpuAfter = optimized.Average(s => s.CpuPercent);
                    AvgRamAfter = optimized.Average(s => s.RamPercent);
                    AvgLatencyAfter = (int)optimized.Where(s => s.NetworkLatencyMs > 0).DefaultIfEmpty(new PerformanceSnapshot()).Average(s => s.NetworkLatencyMs);
                }

                OnPropertyChanged(nameof(CpuDelta));
                OnPropertyChanged(nameof(RamDelta));
                OnPropertyChanged(nameof(LatencyDelta));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"⚠️ PerformanceMetrics: Error en comparación: {ex.Message}");
            }
        }

        public void ClearHistory()
        {
            _history.Clear();
            AvgCpuBefore = 0; AvgCpuAfter = 0;
            AvgRamBefore = 0; AvgRamAfter = 0;
            AvgLatencyBefore = 0; AvgLatencyAfter = 0;
            SaveHistory();
            OnPropertyChanged(nameof(History));
            OnPropertyChanged(nameof(SnapshotCount));
            OnPropertyChanged(nameof(RecentSnapshots));
        }

        #endregion

        #region Persistence

        private void LoadHistory()
        {
            try
            {
                if (File.Exists(_historyPath))
                {
                    var json = File.ReadAllText(_historyPath);
                    var data = JsonSerializer.Deserialize<List<PerformanceSnapshot>>(json);
                    if (data != null)
                    {
                        _history.AddRange(data);
                        RecalculateComparisons();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"⚠️ PerformanceMetrics: Error cargando historial: {ex.Message}");
            }
        }

        private void SaveHistory()
        {
            try
            {
                var json = JsonSerializer.Serialize(_history, new JsonSerializerOptions { WriteIndented = false });
                File.WriteAllText(_historyPath, json);
            }
            catch { }
        }

        #endregion

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public void Dispose()
        {
            _snapshotTimer?.Dispose();
            _cpuCounter?.Dispose();
        }
    }
}
