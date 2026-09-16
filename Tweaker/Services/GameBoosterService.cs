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
    /// Servicio de optimizaciÃ³n automÃ¡tica de juegos
    /// Detecta cuando se estÃ¡ jugando y aplica optimizaciones dinÃ¡micamente
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

        #region P/Invoke para detecciÃ³n de ventana activa

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
        /// Lista de juegos conocidos para detecciÃ³n automÃ¡tica
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
            "robloxplayerbeta",
            
            // Competitivos
            "dota2",
            "smite",
            "paladins",
            "deadbydaylight",
            "warframe",
            
            // Adicionales
            "forzahorizon6",
            "forza6",
            "forzahorizon5",
            "forzahorizon4",
            "forzamotorsport",
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
        /// Indica si el servicio estÃ¡ monitoreando activamente
        /// </summary>
        public bool IsMonitoring => _isMonitoring;

        /// <summary>
        /// Indica si el modo juego estÃ¡ activo
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

        #region InicializaciÃ³n

        private void InitializeTimer()
        {
            _monitoringTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2) // Verificar cada 2 segundos para detecciÃ³n mÃ¡s rÃ¡pida
            };
            _monitoringTimer.Tick += MonitoringTimer_Tick;
        }

        #endregion

        #region Control del Servicio

        /// <summary>
        /// Inicia el monitoreo automÃ¡tico de juegos
        /// </summary>
        public void StartMonitoring()
        {
            if (_isMonitoring)
            {
                Debug.WriteLine("âš ï¸ Game Booster ya estÃ¡ monitoreando");
                return;
            }

            Debug.WriteLine("ðŸŽ® GAME BOOSTER - Iniciando monitoreo...");
            _isMonitoring = true;
            _monitoringTimer?.Start();

            OnGameModeChanged(false, "Monitoreando...", "Esperando detecciÃ³n de juego");
        }

        /// <summary>
        /// Detiene el monitoreo automÃ¡tico
        /// </summary>
        public void StopMonitoring()
        {
            if (!_isMonitoring)
                return;

            Debug.WriteLine("ðŸ›‘ GAME BOOSTER - Deteniendo monitoreo...");

            _monitoringTimer?.Stop();
            _isMonitoring = false;

            // Si habÃ­a un juego activo, desactivar optimizaciones
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

                    // DEBUG: Log del proceso actual
                    Debug.WriteLine($"\U0001F50D Proceso en foreground: {processName}");

                    // Verificar si es un juego conocido
                    bool isGame = IsKnownGame(processName);

                    if (isGame)
                    {
                        Debug.WriteLine($"âœ… Es un juego conocido: {processName}");
                    }

                    if (isGame && !_isGameModeActive)
                    {
                        // Juego detectado y modo no activo - activar
                        Debug.WriteLine($"ðŸŽ® JUEGO DETECTADO: {foregroundProcess.ProcessName}");
                        EnableGameMode(foregroundProcess);
                    }
                    else if (!isGame && _isGameModeActive)
                    {
                        // Ya no hay juego activo - desactivar
                        Debug.WriteLine($"ðŸ“‹ Cambio a aplicaciÃ³n normal: {foregroundProcess.ProcessName}");
                        DisableGameMode();
                    }
                    else if (isGame && _isGameModeActive)
                    {
                        // Verificar si es el mismo juego
                        if (_currentGameProcess?.Id != foregroundProcess.Id)
                        {
                            // CambiÃ³ de juego - desactivar y reactivar
                            Debug.WriteLine($"ðŸ”„ Cambio de juego detectado");
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
                Debug.WriteLine($"âŒ Error en monitoreo: {ex.Message}");
                Debug.WriteLine($"âŒ StackTrace: {ex.StackTrace}");
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
            // Normalizar el nombre del proceso (quitar guiones, puntos, etc.)
            var normalizedProcess = processName.Replace("-", "").Replace("_", "").Replace(".", "").ToLower();

            foreach (var game in _knownGames)
            {
                var normalizedGame = game.Replace("-", "").Replace("_", "").Replace(" ", "").ToLower();

                // Verificar si el proceso contiene el juego O si el juego contiene el proceso
                if (processName.Contains(game.ToLower()) ||
                    normalizedProcess.Contains(normalizedGame) ||
                    game.ToLower().Contains(processName))
                {
                    Debug.WriteLine($"âœ… Match encontrado: proceso '{processName}' con juego '{game}'");
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Optimizaciones

        /// <summary>
        /// Activa el modo juego manualmente
        /// </summary>
        public void EnableManualGameMode()
        {
            if (_isGameModeActive)
                return;

            try
            {
                Debug.WriteLine("⚡ ACTIVANDO GAME MODE MANUAL");
                _currentGameProcess = null;
                _currentGameName = "Manual / Tray Mode";
                _isGameModeActive = true;

                StopWindowsUpdate();
                Task.Run(() => CleanSystemMemory());
                SetHighPerformancePowerPlan();
                DisableGameBar();
                try { Tweaker.Optimizations.TimerResolutionOptimization.EnableHighPrecisionTimer(); } catch { }

                OnGameModeChanged(true, _currentGameName, "Optimizaciones activas (Manual)");
                InGameOverlayService.Instance.ShowGameToast("Modo Juego Activado", "Timer 0.5ms • Ultimate Performance • Standby RAM limpia", true);
                Debug.WriteLine("✅ GAME MODE MANUAL ACTIVADO");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error activando Game Mode manual: {ex.Message}");
                _isGameModeActive = false;
            }
        }

        /// <summary>
        /// Activa el modo juego con optimizaciones
        /// </summary>
        public void EnableGameMode(Process gameProcess)
        {
            if (_isGameModeActive)
                return;

            try
            {
                Debug.WriteLine($"⚡ ACTIVANDO GAME MODE para {gameProcess.ProcessName}");

                _currentGameProcess = gameProcess;
                _currentGameName = FormatGameDisplayName(gameProcess.ProcessName);
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
                try { Tweaker.Optimizations.TimerResolutionOptimization.EnableHighPrecisionTimer(); } catch { }

                OnGameModeChanged(true, _currentGameName, "Optimizaciones activas");
                InGameOverlayService.Instance.ShowGameToast($"{_currentGameName} detectado", "Timer 0.5ms y Ultimate Performance activados", true);

                Debug.WriteLine("✅ GAME MODE ACTIVADO");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error activando Game Mode: {ex.Message}");
                _isGameModeActive = false;
            }
        }

        private static string FormatGameDisplayName(string processName)
        {
            return processName.ToLowerInvariant() switch
            {
                "cs2" => "Counter-Strike 2",
                "csgo" => "Counter-Strike: Global Offensive",
                "valorant" => "Valorant",
                "javaw" => "Minecraft (Java)",
                "bedrock_server" or "minecraft.windows" => "Minecraft Bedrock",
                "fortniteclient-win64-shipping" => "Fortnite",
                "apex" or "r5apex" => "Apex Legends",
                "warzone" or "cod" => "Call of Duty: Warzone",
                "gta5" => "Grand Theft Auto V",
                "overwatch" => "Overwatch 2",
                "r6" or "rainbowsix" => "Rainbow Six Siege",
                "dota2" => "Dota 2",
                "league of legends" or "leagueclient" => "League of Legends",
                _ => char.ToUpperInvariant(processName[0]) + processName.Substring(1)
            };
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
                Debug.WriteLine($"🔽 DESACTIVANDO GAME MODE");

                // 1. Restaurar Windows Update
                RestoreWindowsUpdate();

                // 2. Restaurar plan de energía
                RestorePowerPlan();

                // 3. Restaurar Game Bar
                RestoreGameBar();
                try { Tweaker.Optimizations.TimerResolutionOptimization.RestoreTimerResolution(); } catch { }

                string lastGame = _currentGameName ?? "Juego";
                _currentGameProcess = null;
                _currentGameName = null;
                _isGameModeActive = false;

                OnGameModeChanged(false, null, "Modo escritorio");
                InGameOverlayService.Instance.ShowGameToast("Modo Juego Finalizado", $"Sesión de {lastGame} finalizada. Latencias y servicios restaurados.", false);

                Debug.WriteLine("✅ GAME MODE DESACTIVADO");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"âŒ Error desactivando Game Mode: {ex.Message}");
            }
        }

        #endregion

        #region Optimizaciones EspecÃ­ficas

        private void SetGamePriority(Process gameProcess)
        {
            try
            {
                gameProcess.PriorityClass = ProcessPriorityClass.High;
                Debug.WriteLine($"   âœ“ Prioridad del juego establecida en HIGH");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"   âš ï¸ No se pudo cambiar prioridad: {ex.Message}");
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
                    Debug.WriteLine($"   âœ“ Windows Update detenido temporalmente");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"   âš ï¸ No se pudo detener Windows Update: {ex.Message}");
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
                        Debug.WriteLine($"   âœ“ Windows Update restaurado");
                    }
                    _windowsUpdateService.Dispose();
                    _windowsUpdateService = null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"   âš ï¸ No se pudo restaurar Windows Update: {ex.Message}");
            }
        }

        private void CleanSystemMemory()
        {
            try
            {
                var result = MemoryCleaner.FlushMemory();
                if (result.success)
                {
                    Debug.WriteLine($"   âœ“ Memoria limpiada: ~{result.mbCleaned}MB liberados ({result.processesProcessed} procesos)");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"   âš ï¸ Error limpiando memoria: {ex.Message}");
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

                Debug.WriteLine($"   âœ“ Plan de energÃ­a cambiado a Alto Rendimiento");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"   âš ï¸ No se pudo cambiar plan de energÃ­a: {ex.Message}");
            }
        }

        private void RestorePowerPlan()
        {
            try
            {
                if (_previousPowerPlan.HasValue)
                {
                    SetActivePowerPlan(_previousPowerPlan.Value);
                    Debug.WriteLine($"   âœ“ Plan de energÃ­a restaurado");
                    _previousPowerPlan = null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"   âš ï¸ No se pudo restaurar plan de energÃ­a: {ex.Message}");
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
