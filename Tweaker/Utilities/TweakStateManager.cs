using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Tweaker.Utilities
{
    /// <summary>
    /// Gestor de estado de tweaks - Trackea qué optimizaciones están activas
    /// Implementa INotifyPropertyChanged para actualización dinámica del Dashboard
    /// </summary>
    public class TweakStateManager : INotifyPropertyChanged
    {
        private static TweakStateManager _instance;
        private static readonly object _lock = new object();

        private Dictionary<string, TweakState> _tweakStates;
        private readonly string _stateFilePath;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static TweakStateManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new TweakStateManager();
                        }
                    }
                }
                return _instance;
            }
        }

        private TweakStateManager()
        {
            _stateFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "GhostOptimizer",
                "tweaks_state.json"
            );

            LoadState();
        }

        /// <summary>
        /// Carga el estado desde archivo JSON
        /// </summary>
        private void LoadState()
        {
            try
            {
                if (File.Exists(_stateFilePath))
                {
                    string json = File.ReadAllText(_stateFilePath);
                    _tweakStates = JsonSerializer.Deserialize<Dictionary<string, TweakState>>(json);
                }
                else
                {
                    _tweakStates = new Dictionary<string, TweakState>();
                }
            }
            catch
            {
                _tweakStates = new Dictionary<string, TweakState>();
            }
        }

        /// <summary>
        /// Guarda el estado a archivo JSON
        /// </summary>
        private void SaveState()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_stateFilePath));
                string json = JsonSerializer.Serialize(_tweakStates, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(_stateFilePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving tweak state: {ex.Message}");
            }
        }

        /// <summary>
        /// Marca un tweak como activado
        /// </summary>
        public void SetTweakEnabled(string tweakId, string category)
        {
            if (!_tweakStates.ContainsKey(tweakId))
            {
                _tweakStates[tweakId] = new TweakState
                {
                    Id = tweakId,
                    Category = category,
                    IsEnabled = false
                };
            }

            _tweakStates[tweakId].IsEnabled = true;
            _tweakStates[tweakId].LastModified = DateTime.Now;
            
            SaveState();
            OnPropertyChanged(nameof(ActiveTweaksCount));
            OnPropertyChanged(nameof(TweaksByCategory));
        }

        /// <summary>
        /// Marca un tweak como desactivado
        /// </summary>
        public void SetTweakDisabled(string tweakId)
        {
            if (_tweakStates.ContainsKey(tweakId))
            {
                _tweakStates[tweakId].IsEnabled = false;
                _tweakStates[tweakId].LastModified = DateTime.Now;
                
                SaveState();
                OnPropertyChanged(nameof(ActiveTweaksCount));
                OnPropertyChanged(nameof(TweaksByCategory));
            }
        }

        /// <summary>
        /// Verifica si un tweak está activado
        /// </summary>
        public bool IsTweakEnabled(string tweakId)
        {
            return _tweakStates.ContainsKey(tweakId) && _tweakStates[tweakId].IsEnabled;
        }

        /// <summary>
        /// Contador de tweaks activos (propiedad observable)
        /// </summary>
        public int ActiveTweaksCount => _tweakStates.Count(t => t.Value.IsEnabled);

        /// <summary>
        /// Total de tweaks disponibles
        /// </summary>
        public int TotalTweaksCount => 32;

        /// <summary>
        /// Porcentaje de optimización aplicado
        /// </summary>
        public int OptimizationPercentage => 
            TotalTweaksCount > 0 ? (ActiveTweaksCount * 100) / TotalTweaksCount : 0;

        /// <summary>
        /// Tweaks agrupados por categoría
        /// </summary>
        public Dictionary<string, int> TweaksByCategory
        {
            get
            {
                return _tweakStates
                    .Where(t => t.Value.IsEnabled)
                    .GroupBy(t => t.Value.Category)
                    .ToDictionary(g => g.Key, g => g.Count());
            }
        }

        /// <summary>
        /// Obtiene los tweaks más recientes
        /// </summary>
        public List<TweakState> GetRecentTweaks(int count = 5)
        {
            return _tweakStates.Values
                .Where(t => t.IsEnabled)
                .OrderByDescending(t => t.LastModified)
                .Take(count)
                .ToList();
        }

        /// <summary>
        /// Resetea todos los tweaks
        /// </summary>
        public void ResetAllTweaks()
        {
            _tweakStates.Clear();
            SaveState();
            OnPropertyChanged(nameof(ActiveTweaksCount));
            OnPropertyChanged(nameof(TweaksByCategory));
        }

        /// <summary>
        /// Obtiene estadísticas completas
        /// </summary>
        public DashboardStats GetDashboardStats()
        {
            var stats = new DashboardStats
            {
                TotalTweaks = TotalTweaksCount,
                ActiveTweaks = ActiveTweaksCount,
                OptimizationPercentage = OptimizationPercentage,
                TweaksByCategory = TweaksByCategory,
                LastUpdated = DateTime.Now
            };

            // Calcular beneficios estimados
            stats.EstimatedFpsGain = CalculateEstimatedFpsGain();
            stats.EstimatedLatencyReduction = CalculateEstimatedLatencyReduction();
            stats.EstimatedRamFreed = CalculateEstimatedRamFreed();

            return stats;
        }

        /// <summary>
        /// Calcula ganancia estimada de FPS basado en tweaks activos
        /// </summary>
        private int CalculateEstimatedFpsGain()
        {
            int fpsGain = 0;

            // Input & Visuals
            if (IsTweakEnabled("VisualEffects")) fpsGain += 5;
            if (IsTweakEnabled("MemoryOptimization")) fpsGain += 3;

            // Sistema & GPU
            if (IsTweakEnabled("SystemProfile")) fpsGain += 10;
            if (IsTweakEnabled("GameDVR")) fpsGain += 15;
            if (IsTweakEnabled("GpuScheduling")) fpsGain += 5;

            // GHOST Pack
            if (IsTweakEnabled("CoreIsolation")) fpsGain += 20;
            if (IsTweakEnabled("MPO")) fpsGain += 8;
            if (IsTweakEnabled("UltimatePower")) fpsGain += 12;

            // Advanced
            if (IsTweakEnabled("SpectreMeltdown")) fpsGain += 10;

            return fpsGain;
        }

        /// <summary>
        /// Calcula reducción estimada de latencia
        /// </summary>
        private int CalculateEstimatedLatencyReduction()
        {
            int latencyReduction = 0;

            // Red & Ping
            if (IsTweakEnabled("NetworkOptimization")) latencyReduction += 20;
            if (IsTweakEnabled("DnsCloudflare")) latencyReduction += 30;
            if (IsTweakEnabled("NetworkPower")) latencyReduction += 15;

            // Input
            if (IsTweakEnabled("MouseAcceleration")) latencyReduction += 10;
            if (IsTweakEnabled("Keyboard")) latencyReduction += 5;

            // Sistema
            if (IsTweakEnabled("CoreParking")) latencyReduction += 8;

            return latencyReduction;
        }

        /// <summary>
        /// Calcula RAM liberada estimada (en GB)
        /// </summary>
        private double CalculateEstimatedRamFreed()
        {
            double ramFreed = 0;

            // Limpieza
            if (IsTweakEnabled("Hibernation")) ramFreed += 8.0;
            if (IsTweakEnabled("WindowsSearch")) ramFreed += 0.3;
            if (IsTweakEnabled("SysMain")) ramFreed += 2.0;

            // Sistema
            if (IsTweakEnabled("GameDVR")) ramFreed += 1.5;

            return Math.Round(ramFreed, 1);
        }

        /// <summary>
        /// REVIERTE TODOS LOS TWEAKS - Limpia el estado completamente
        /// </summary>
        public void RevertAllTweaks()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("?? REVIRTIENDO ESTADO DE TODOS LOS TWEAKS");
                
                // Limpiar todos los estados
                _tweakStates.Clear();
                
                // Guardar estado limpio
                SaveState();
                
                // Notificar cambios
                OnPropertyChanged(nameof(DashboardStats));
                
                System.Diagnostics.Debug.WriteLine("? Estado de tweaks completamente limpio");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"? Error revirtiendo estado: {ex.Message}");
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Estado de un tweak individual
    /// </summary>
    public class TweakState
    {
        public string Id { get; set; }
        public string Category { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime LastModified { get; set; }
    }

    /// <summary>
    /// Estadísticas para el Dashboard
    /// </summary>
    public class DashboardStats
    {
        public int TotalTweaks { get; set; }
        public int ActiveTweaks { get; set; }
        public int OptimizationPercentage { get; set; }
        public Dictionary<string, int> TweaksByCategory { get; set; }
        public DateTime LastUpdated { get; set; }
        
        // Beneficios estimados
        public int EstimatedFpsGain { get; set; }
        public int EstimatedLatencyReduction { get; set; }
        public double EstimatedRamFreed { get; set; }
    }
}
