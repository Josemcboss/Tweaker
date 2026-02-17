using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows.Threading;
using Microsoft.Win32;
using Tweaker.Utilities;

namespace Tweaker.Services
{
    /// <summary>
    /// Servicio de optimización automática de juegos
    /// Detecta cuando se está jugando y aplica optimizaciones dinámicamente
    /// </summary>
    public class GameBoosterService
    {
        #region Singleton

        private static GameBoosterService? _instance;
        private static readonly object _lock = new object();

        public static GameBoosterService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new GameBoosterService();
                    }
                }
                return _instance;
            }
        }

        private GameBoosterService()
        {
            InitializeTimer();
        }

        #endregion

        #region P/Invoke para detección de ventana activa

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        #endregion

        #region Propiedades

        private DispatcherTimer? _monitoringTimer;
        private bool _isGameModeActive = false;
        private bool _isMonitoring = false;
        private Process? _currentGameProcess;
        private string? _currentGameName;
        private ServiceController? _windowsUpdateService;
        private Guid? _previousPowerPlan;

        /// <summary>
        /// Lista de juegos conocidos para detección automática
        /// </summary>
        private readonly List<string> _knownGames = new List<string>
        {
            // Battle Royale / Shooters
            "valorant",
            "valorant-win64-shipping",
            "cs2",
            "csgo",
            "gta5",
            "gtav",
            "fortnite",
            "fortniteclient-win64-shipping",
            "r5apex",
            "apex_legends",
            
            // MOBA
            "league of legends",
            "leagueclient",
            "leagueclientux",
            "lol",
            
            // Call of Duty
            "cod",
            "codmw",
            "modernwarfare",
            "warzone",
            "blackops",
            
            // Populares
            "overwatch",
            "overwatch2",
            "pubg",
            "battlegrounds",
            "destiny2",
            "rainbowsix",
            "r6s",
            "battlefield",
            "bf2042",
            "minecraft",
            "javaw",
            "rocketleague",
            
            // Competitivos
            "dota2",
            "smite",
            "paladins",
            "deadbydaylight",
            "warframe",
            
            // Adicionales
            "elden ring",
            "eldenring",
            "cyberpunk2077",
            "witcher3",
            "rdr2",
            "sekiro",
            
            // Gacha / RPG
            "genshinimpact",
            "yuanshen",
            "honkaistarrail",
            "starrail",
            
            // Sports
            "fifa",
            "fifa23",
            "fifa24",
            "fc24",
            "eafc",
            
            // Survival / Sandbox
            "rust",
            "valheim",
            "7daystodie",
            "ark",
            "arksurvivalevolved",
            
            // MMO
            "newworld",
            "lostark",
            "ffxiv_dx11",
            "worldofwarcraft",
            "wow",
            "guildwars2"
        };

        /// <summary>
        /// Indica si el servicio está monitoreando activamente
        /// </summary>
        public bool IsMonitoring => _isMonitoring;

        /// <summary>
        /// Indica si el modo juego está activo
        /// </summary>
        public bool IsGameModeActive => _isGameModeActive;

        /// <summary>
        /// Nombre del juego actualmente detectado
        /// </summary>
        public string? CurrentGameName => _currentGameName;

        /// <summary>
        /// Evento que se dispara cuando cambia el estado del Game Mode
        /// </summary>
        public event EventHandler<GameModeChangedEventArgs>? GameModeChanged;

        #endregion

        #region Inicialización

        private void InitializeTimer()
        {
            _monitoringTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5) // Verificar cada 5 segundos
            };
            _monitoringTimer.Tick += MonitoringTimer_Tick;
        }

        #endregion

        #region Control del Servicio

        /// <summary>
        /// Inicia el monitoreo automático de juegos
        /// </summary>
        public void StartMonitoring()
        {
            if (_isMonitoring)
            {
                Debug.WriteLine("?? Game Booster ya está monitoreando");
                return;
            }

            Debug.WriteLine("?? GAME BOOSTER - Iniciando monitoreo...");
            _isMonitoring = true;
            _monitoringTimer?.Start();
            
            OnGameModeChanged(false, "Monitoreando...", "Esperando detección de juego");
        }

        /// <summary>
        /// Detiene el monitoreo automático
        /// </summary>
        public void StopMonitoring()
        {
            if (!_isMonitoring)
                return;

            Debug.WriteLine("?? GAME BOOSTER - Deteniendo monitoreo...");
            
            _monitoringTimer?.Stop();
            _isMonitoring = false;

            // Si había un juego activo, desactivar optimizaciones
            if (_isGameModeActive)
            {
                DisableGameMode();
            }

            OnGameModeChanged(false, "Desactivado", "Monitoreo detenido");
        }

        #endregion

        #region Monitoreo

        private void MonitoringTimer_Tick(object? sender, EventArgs e)
        {
            try
            {
                var foregroundProcess = GetForegroundProcess();

                if (foregroundProcess != null)
                {
                    var processName = foregroundProcess.ProcessName.ToLower();

                    // Verificar si es un juego conocido
                    bool isGame = IsKnownGame(processName);

                    if (isGame && !_isGameModeActive)
                    {
                        // Juego detectado y modo no activo - activar
                        Debug.WriteLine($"?? JUEGO DETECTADO: {foregroundProcess.ProcessName}");
                        EnableGameMode(foregroundProcess);
                    }
                    else if (!isGame && _isGameModeActive)
                    {
                        // Ya no hay juego activo - desactivar
                        Debug.WriteLine($"?? Cambio a aplicación normal: {foregroundProcess.ProcessName}");
                        DisableGameMode();
                    }
                    else if (isGame && _isGameModeActive)
                    {
                        // Verificar si es el mismo juego
                        if (_currentGameProcess?.Id != foregroundProcess.Id)
                        {
                            // Cambió de juego - desactivar y reactivar
                            Debug.WriteLine($"?? Cambio de juego detectado");
                            DisableGameMode();
                            EnableGameMode(foregroundProcess);
                        }
                    }
                }
                else
                {
                    // No se pudo obtener proceso en primer plano
                    if (_isGameModeActive)
                    {
                        DisableGameMode();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error en monitoreo: {ex.Message}");
            }
        }

        private Process? GetForegroundProcess()
        {
            try
            {
                IntPtr hwnd = GetForegroundWindow();
                if (hwnd == IntPtr.Zero)
                    return null;

                uint processId;
                GetWindowThreadProcessId(hwnd, out processId);

                if (processId == 0)
                    return null;

                return Process.GetProcessById((int)processId);
            }
            catch
            {
                return null;
            }
        }

        private bool IsKnownGame(string processName)
        {
            foreach (var game in _knownGames)
            {
                if (processName.Contains(game))
                    return true;
            }
            return false;
        }

        #endregion

        #region Optimizaciones

        /// <summary>
        /// Activa el modo juego con optimizaciones
        /// </summary>
        public void EnableGameMode(Process gameProcess)
        {
            if (_isGameModeActive)
                return;

            try
            {
                Debug.WriteLine($"? ACTIVANDO GAME MODE para {gameProcess.ProcessName}");
                
                _currentGameProcess = gameProcess;
                _currentGameName = gameProcess.ProcessName;
                _isGameModeActive = true;

                // 1. Aumentar prioridad del juego
                SetGamePriority(gameProcess);

                // 2. Detener Windows Update
                StopWindowsUpdate();

                // 3. Limpiar memoria RAM
                Task.Run(() => CleanSystemMemory());

                // 4. Cambiar plan de energía a Alto Rendimiento
                SetHighPerformancePowerPlan();

                // 5. Deshabilitar Game Bar temporalmente (opcional)
                DisableGameBar();

                OnGameModeChanged(true, _currentGameName, "Optimizaciones activas");

                Debug.WriteLine("? GAME MODE ACTIVADO");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error activando Game Mode: {ex.Message}");
                _isGameModeActive = false;
            }
        }

        /// <summary>
        /// Desactiva el modo juego y restaura configuraciones
        /// </summary>
        public void DisableGameMode()
        {
            if (!_isGameModeActive)
                return;

            try
            {
                Debug.WriteLine($"?? DESACTIVANDO GAME MODE");

                // 1. Restaurar Windows Update
                RestoreWindowsUpdate();

                // 2. Restaurar plan de energía
                RestorePowerPlan();

                // 3. Restaurar Game Bar
                RestoreGameBar();

                _currentGameProcess = null;
                _currentGameName = null;
                _isGameModeActive = false;

                OnGameModeChanged(false, null, "Modo escritorio");

                Debug.WriteLine("? GAME MODE DESACTIVADO");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error desactivando Game Mode: {ex.Message}");
            }
        }

        #endregion

        #region Optimizaciones Específicas

        private void SetGamePriority(Process gameProcess)
        {
            try
            {
                gameProcess.PriorityClass = ProcessPriorityClass.High;
                Debug.WriteLine($"   ? Prioridad del juego establecida en HIGH");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"   ?? No se pudo cambiar prioridad: {ex.Message}");
            }
        }

        private void StopWindowsUpdate()
        {
            try
            {
                _windowsUpdateService = new ServiceController("wuauserv");
                
                if (_windowsUpdateService.Status == ServiceControllerStatus.Running)
                {
                    _windowsUpdateService.Stop();
                    _windowsUpdateService.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                    Debug.WriteLine($"   ? Windows Update detenido temporalmente");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"   ?? No se pudo detener Windows Update: {ex.Message}");
            }
        }

        private void RestoreWindowsUpdate()
        {
            try
            {
                if (_windowsUpdateService != null)
                {
                    _windowsUpdateService.Refresh();
                    if (_windowsUpdateService.Status == ServiceControllerStatus.Stopped)
                    {
                        _windowsUpdateService.Start();
                        Debug.WriteLine($"   ? Windows Update restaurado");
                    }
                    _windowsUpdateService.Dispose();
                    _windowsUpdateService = null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"   ?? No se pudo restaurar Windows Update: {ex.Message}");
            }
        }

        private void CleanSystemMemory()
        {
            try
            {
                var result = MemoryCleaner.FlushMemory();
                if (result.success)
                {
                    Debug.WriteLine($"   ? Memoria limpiada: ~{result.mbCleaned}MB liberados ({result.processesProcessed} procesos)");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"   ?? Error limpiando memoria: {ex.Message}");
            }
        }

        private void SetHighPerformancePowerPlan()
        {
            try
            {
                // Guardar plan actual
                _previousPowerPlan = GetActivePowerPlan();

                // Establecer Alto Rendimiento
                // GUID del plan "Alto rendimiento": 8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c
                var highPerformanceGuid = new Guid("8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c");
                SetActivePowerPlan(highPerformanceGuid);

                Debug.WriteLine($"   ? Plan de energía cambiado a Alto Rendimiento");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"   ?? No se pudo cambiar plan de energía: {ex.Message}");
            }
        }

        private void RestorePowerPlan()
        {
            try
            {
                if (_previousPowerPlan.HasValue)
                {
                    SetActivePowerPlan(_previousPowerPlan.Value);
                    Debug.WriteLine($"   ? Plan de energía restaurado");
                    _previousPowerPlan = null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"   ?? No se pudo restaurar plan de energía: {ex.Message}");
            }
        }

        private void DisableGameBar()
        {
            // Opcional: Deshabilitar temporalmente Game Bar
        }

        private void RestoreGameBar()
        {
            // Opcional: Restaurar Game Bar
        }

        #endregion

        #region Power Plan Helpers

        private Guid? GetActivePowerPlan()
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Power\User\PowerSchemes"))
                {
                    if (key != null)
                    {
                        var activeGuid = key.GetValue("ActivePowerScheme") as string;
                        if (!string.IsNullOrEmpty(activeGuid))
                        {
                            return new Guid(activeGuid);
                        }
                    }
                }
            }
            catch { }
            return null;
        }

        private void SetActivePowerPlan(Guid planGuid)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "powercfg",
                    Arguments = $"/setactive {planGuid:D}",
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                using (var process = Process.Start(psi))
                {
                    process?.WaitForExit(5000);
                }
            }
            catch { }
        }

        #endregion

        #region Eventos

        private void OnGameModeChanged(bool isActive, string? gameName, string status)
        {
            GameModeChanged?.Invoke(this, new GameModeChangedEventArgs
            {
                IsActive = isActive,
                GameName = gameName,
                Status = status,
                Timestamp = DateTime.Now
            });
        }

        #endregion
    }

    /// <summary>
    /// Argumentos del evento de cambio de Game Mode
    /// </summary>
    public class GameModeChangedEventArgs : EventArgs
    {
        public bool IsActive { get; set; }
        public string? GameName { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
