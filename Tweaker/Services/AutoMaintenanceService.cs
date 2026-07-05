using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Tweaker.Models;
using Tweaker.Utilities;

namespace Tweaker.Services
{
    /// <summary>
    /// Servicio de mantenimiento automático en segundo plano.
    /// Limpia temporales, libera RAM y compacta logs cuando el sistema está inactivo.
    /// </summary>
    public class AutoMaintenanceService : INotifyPropertyChanged, IDisposable
    {
        #region Singleton

        private static AutoMaintenanceService? _instance;
        private static readonly object _lock = new();

        public static AutoMaintenanceService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock) { _instance ??= new AutoMaintenanceService(); }
                }
                return _instance;
            }
        }

        #endregion

        #region P/Invoke para detección de inactividad

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }

        #endregion

        #region Fields

        private Timer? _maintenanceTimer;
        private bool _isEnabled;
        private bool _isRunning;
        private DateTime? _lastCleanupTime;
        private long _totalMBFreed;
        private int _totalFilesDeleted;
        private readonly List<MaintenanceLog> _logs = new();
        private readonly string _logPath;

        // Configuración
        private int _intervalMinutes = 30;
        private int _idleThresholdMinutes = 5;
        private int _ramThresholdPercent = 85;

        #endregion

        #region Properties

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged();
                    if (value) Start(); else Stop();
                }
            }
        }

        public bool IsRunning
        {
            get => _isRunning;
            private set { if (_isRunning != value) { _isRunning = value; OnPropertyChanged(); } }
        }

        public DateTime? LastCleanupTime
        {
            get => _lastCleanupTime;
            private set { _lastCleanupTime = value; OnPropertyChanged(); OnPropertyChanged(nameof(LastCleanupTimeText)); }
        }

        public string LastCleanupTimeText => LastCleanupTime.HasValue
            ? LastCleanupTime.Value.ToString("HH:mm:ss dd/MM")
            : "Nunca";

        public long TotalMBFreed
        {
            get => _totalMBFreed;
            private set { _totalMBFreed = value; OnPropertyChanged(); }
        }

        public int TotalFilesDeleted
        {
            get => _totalFilesDeleted;
            private set { _totalFilesDeleted = value; OnPropertyChanged(); }
        }

        public string NextScheduledRun
        {
            get
            {
                if (!_isEnabled || _lastCleanupTime == null) return "—";
                return _lastCleanupTime.Value.AddMinutes(_intervalMinutes).ToString("HH:mm");
            }
        }

        public List<MaintenanceLog> RecentLogs => _logs.TakeLast(20).Reverse().ToList();

        #endregion

        #region Events

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<MaintenanceLog>? MaintenanceCompleted;

        #endregion

        private AutoMaintenanceService()
        {
            var appData = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "GhostOptimizer");
            Directory.CreateDirectory(appData);
            _logPath = Path.Combine(appData, "maintenance_logs.json");
            LoadLogs();
        }

        #region Control

        private void Start()
        {
            _maintenanceTimer?.Dispose();
            _maintenanceTimer = new Timer(
                async _ => await RunMaintenanceCycleAsync(),
                null,
                TimeSpan.FromMinutes(1), // Primer check en 1 minuto
                TimeSpan.FromMinutes(_intervalMinutes));
            Debug.WriteLine("🔧 AutoMaintenance: Timer iniciado");
        }

        private void Stop()
        {
            _maintenanceTimer?.Dispose();
            _maintenanceTimer = null;
            Debug.WriteLine("🔧 AutoMaintenance: Timer detenido");
        }

        #endregion

        #region Core Logic

        private async Task RunMaintenanceCycleAsync()
        {
            if (IsRunning) return;

            try
            {
                // Verificar inactividad del usuario
                int idleMinutes = GetIdleMinutes();
                Debug.WriteLine($"🔧 AutoMaintenance: Usuario inactivo por {idleMinutes} minutos");

                if (idleMinutes < _idleThresholdMinutes)
                {
                    Debug.WriteLine("🔧 AutoMaintenance: Usuario activo, pospuesto");
                    return;
                }

                IsRunning = true;

                // 1. Limpiar temporales
                var tempResult = await Task.Run(() => CleanTemporaryFiles());

                // 2. Liberar RAM si uso > umbral
                var ramResult = await Task.Run(() => FlushRamIfNeeded());

                // 3. Compactar logs internos
                await Task.Run(() => CompactInternalLogs());

                // Registrar resultado
                long totalFreed = tempResult.mbFreed + ramResult.mbFreed;
                int totalFiles = tempResult.filesDeleted;

                var log = new MaintenanceLog
                {
                    Timestamp = DateTime.Now,
                    Action = "Mantenimiento Automático",
                    MBFreed = totalFreed,
                    FilesDeleted = totalFiles,
                    Success = true,
                    Details = $"Temp: {tempResult.mbFreed}MB/{tempResult.filesDeleted} archivos. RAM: {ramResult.mbFreed}MB liberados."
                };

                _logs.Add(log);
                TotalMBFreed += totalFreed;
                TotalFilesDeleted += totalFiles;
                LastCleanupTime = DateTime.Now;

                SaveLogs();
                OnPropertyChanged(nameof(RecentLogs));
                OnPropertyChanged(nameof(NextScheduledRun));
                MaintenanceCompleted?.Invoke(this, log);

                Debug.WriteLine($"✅ AutoMaintenance: Completado - {totalFreed}MB liberados, {totalFiles} archivos eliminados");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ AutoMaintenance: Error - {ex.Message}");
                _logs.Add(new MaintenanceLog
                {
                    Action = "Error",
                    Success = false,
                    Details = ex.Message
                });
            }
            finally
            {
                IsRunning = false;
            }
        }

        /// <summary>
        /// Ejecuta el mantenimiento manualmente (para el botón de la UI).
        /// </summary>
        public async Task RunNowAsync()
        {
            if (IsRunning) return;

            try
            {
                IsRunning = true;

                var tempResult = await Task.Run(() => CleanTemporaryFiles());
                var ramResult = await Task.Run(() => FlushRamIfNeeded(forceFlush: true));
                await Task.Run(() => CompactInternalLogs());

                long totalFreed = tempResult.mbFreed + ramResult.mbFreed;
                int totalFiles = tempResult.filesDeleted;

                var log = new MaintenanceLog
                {
                    Timestamp = DateTime.Now,
                    Action = "Mantenimiento Manual",
                    MBFreed = totalFreed,
                    FilesDeleted = totalFiles,
                    Success = true,
                    Details = $"Temp: {tempResult.mbFreed}MB/{tempResult.filesDeleted} archivos. RAM: {ramResult.mbFreed}MB liberados."
                };

                _logs.Add(log);
                TotalMBFreed += totalFreed;
                TotalFilesDeleted += totalFiles;
                LastCleanupTime = DateTime.Now;

                SaveLogs();
                OnPropertyChanged(nameof(RecentLogs));
                OnPropertyChanged(nameof(NextScheduledRun));
                MaintenanceCompleted?.Invoke(this, log);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ AutoMaintenance Manual: Error - {ex.Message}");
            }
            finally
            {
                IsRunning = false;
            }
        }

        #endregion

        #region Cleanup Operations

        private (long mbFreed, int filesDeleted) CleanTemporaryFiles()
        {
            long totalBytesFreed = 0;
            int filesDeleted = 0;

            // Directorios a limpiar
            var dirsToClean = new[]
            {
                Path.GetTempPath(),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Microsoft", "Windows", "Explorer") // Thumbnail cache
            };

            foreach (var dir in dirsToClean)
            {
                if (!Directory.Exists(dir)) continue;

                try
                {
                    var files = Directory.GetFiles(dir, "*", SearchOption.TopDirectoryOnly);
                    foreach (var file in files)
                    {
                        try
                        {
                            var info = new FileInfo(file);
                            // Solo archivos antiguos (>24h) y no en uso
                            if (info.LastWriteTime < DateTime.Now.AddHours(-24))
                            {
                                long size = info.Length;
                                info.Delete();
                                totalBytesFreed += size;
                                filesDeleted++;
                            }
                        }
                        catch
                        {
                            // Archivo en uso o sin permiso - ignorar
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"⚠️ No se pudo limpiar {dir}: {ex.Message}");
                }
            }

            return (totalBytesFreed / 1024 / 1024, filesDeleted);
        }

        private (long mbFreed, bool didFlush) FlushRamIfNeeded(bool forceFlush = false)
        {
            try
            {
                var memInfo = MemoryCleaner.GetSystemMemoryInfo();

                if (forceFlush || memInfo.percentUsed >= _ramThresholdPercent)
                {
                    Debug.WriteLine($"🧹 RAM al {memInfo.percentUsed}% - Liberando...");
                    var result = MemoryCleaner.FlushMemory();
                    return (result.mbCleaned, true);
                }

                Debug.WriteLine($"✅ RAM al {memInfo.percentUsed}% - OK (umbral: {_ramThresholdPercent}%)");
                return (0, false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"⚠️ Error verificando RAM: {ex.Message}");
                return (0, false);
            }
        }

        private void CompactInternalLogs()
        {
            try
            {
                var appData = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "GhostOptimizer");

                if (!Directory.Exists(appData)) return;

                var logFiles = Directory.GetFiles(appData, "*.log", SearchOption.TopDirectoryOnly);
                foreach (var logFile in logFiles)
                {
                    try
                    {
                        var info = new FileInfo(logFile);
                        // Truncar logs mayores a 5MB
                        if (info.Length > 5 * 1024 * 1024)
                        {
                            var lines = File.ReadAllLines(logFile);
                            // Mantener solo las últimas 1000 líneas
                            var trimmed = lines.TakeLast(1000).ToArray();
                            File.WriteAllLines(logFile, trimmed);
                            Debug.WriteLine($"📋 Log compactado: {info.Name}");
                        }
                    }
                    catch { }
                }

                // Compactar nuestros propios logs de mantenimiento
                if (_logs.Count > 100)
                {
                    var trimmed = _logs.TakeLast(50).ToList();
                    _logs.Clear();
                    _logs.AddRange(trimmed);
                    SaveLogs();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"⚠️ Error compactando logs: {ex.Message}");
            }
        }

        #endregion

        #region Helpers

        private int GetIdleMinutes()
        {
            try
            {
                var info = new LASTINPUTINFO { cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf<LASTINPUTINFO>() };
                if (GetLastInputInfo(ref info))
                {
                    uint idleMs = (uint)Environment.TickCount - info.dwTime;
                    return (int)(idleMs / 60000);
                }
            }
            catch { }
            return 0;
        }

        private void LoadLogs()
        {
            try
            {
                if (File.Exists(_logPath))
                {
                    var json = File.ReadAllText(_logPath);
                    var logs = JsonSerializer.Deserialize<List<MaintenanceLog>>(json);
                    if (logs != null)
                    {
                        _logs.AddRange(logs);
                        _totalMBFreed = logs.Sum(l => l.MBFreed);
                        _totalFilesDeleted = logs.Sum(l => l.FilesDeleted);
                        _lastCleanupTime = logs.LastOrDefault()?.Timestamp;
                    }
                }
            }
            catch { }
        }

        private void SaveLogs()
        {
            try
            {
                var json = JsonSerializer.Serialize(_logs, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_logPath, json);
            }
            catch { }
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public void Dispose()
        {
            _maintenanceTimer?.Dispose();
        }

        #endregion
    }
}
