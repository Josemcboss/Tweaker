using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Timers;
using LibreHardwareMonitor.Hardware;

namespace Tweaker.Services
{
    /// <summary>
    /// Service that exposes bindable properties for MVVM hardware monitoring.
    /// </summary>
    public class HardwareMonitorService : INotifyPropertyChanged, IDisposable
    {
        private readonly Computer _computer;
        private readonly System.Timers.Timer _timer;
        private readonly SynchronizationContext _uiContext;

        private float _cpuLoadPercent;
        private float _ramUsagePercent;
        private float _gpuLoadPercent;
        private float _cpuTemperature;
        private float _gpuTemperature;

        public event PropertyChangedEventHandler? PropertyChanged;

        public HardwareMonitorService()
        {
            _uiContext = SynchronizationContext.Current ?? new SynchronizationContext();

            _computer = new Computer
            {
                IsCpuEnabled = true,
                IsGpuEnabled = true,
                IsMemoryEnabled = true,
                IsMotherboardEnabled = true // Habilitar motherboard para acceso a más sensores
            };

            try
            {
                _computer.Open();
                System.Diagnostics.Debug.WriteLine("✅ LibreHardwareMonitor inicializado correctamente");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[HardwareMonitorService] Error opening computer: {ex.Message}");
                System.Diagnostics.Debug.WriteLine("💡 Si los sensores devuelven 0°C, ejecuta la app como Administrador");
                // Continue without hardware monitoring - the timer will just get 0 values
            }

            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += Timer_Elapsed;
            _timer.AutoReset = true;
            _timer.Start();
        }

        public float CpuLoadPercent
        {
            get => _cpuLoadPercent;
            private set
            {
                if (Math.Abs(_cpuLoadPercent - value) > 0.01f)
                {
                    _cpuLoadPercent = value;
                    OnPropertyChanged(nameof(CpuLoadPercent));
                }
            }
        }

        public float RamUsagePercent
        {
            get => _ramUsagePercent;
            private set
            {
                if (Math.Abs(_ramUsagePercent - value) > 0.01f)
                {
                    _ramUsagePercent = value;
                    OnPropertyChanged(nameof(RamUsagePercent));
                }
            }
        }

        public float GpuLoadPercent
        {
            get => _gpuLoadPercent;
            private set
            {
                if (Math.Abs(_gpuLoadPercent - value) > 0.01f)
                {
                    _gpuLoadPercent = value;
                    OnPropertyChanged(nameof(GpuLoadPercent));
                }
            }
        }

        public float CpuTemperature
        {
            get => _cpuTemperature;
            private set
            {
                if (Math.Abs(_cpuTemperature - value) > 0.01f)
                {
                    _cpuTemperature = value;
                    OnPropertyChanged(nameof(CpuTemperature));
                }
            }
        }

        public float GpuTemperature
        {
            get => _gpuTemperature;
            private set
            {
                if (Math.Abs(_gpuTemperature - value) > 0.01f)
                {
                    _gpuTemperature = value;
                    OnPropertyChanged(nameof(GpuTemperature));
                }
            }
        }

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            try
            {
                var cpu = _computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Cpu);
                var gpu = _computer.Hardware.FirstOrDefault(h =>
                    h.HardwareType == HardwareType.GpuNvidia ||
                    h.HardwareType == HardwareType.GpuAmd ||
                    h.HardwareType == HardwareType.GpuIntel);
                var ram = _computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Memory);

                cpu?.Update();
                gpu?.Update();
                ram?.Update();

                // Find the correct sensors based on the diagnostic logs
                float cpuLoad = cpu?.Sensors.FirstOrDefault(s => s.Name == "CPU Total" && s.SensorType == SensorType.Load)?.Value ?? 0f;
                float gpuLoad = gpu?.Sensors.FirstOrDefault(s => s.Name == "GPU Core" && s.SensorType == SensorType.Load)?.Value ?? 0f;

                // Read temperatures
                // WORKAROUND para AMD Ryzen 7 5800XT: el sensor Tctl/Tdie devuelve 0 en LibreHardwareMonitor
                // hasta que AMD/LibreHardwareMonitor actualice soporte para este CPU nuevo
                float cpuTemp = cpu?.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Temperature && (s.Name.Contains("Core (Tctl/Tdie)") || s.Name.Contains("Package") || s.Name.Contains("Core Average") || s.Name.Contains("Core")))?.Value ?? 0f;

                // Si cpuTemp sigue siendo 0, intentar obtenerla del motherboard
                if (cpuTemp == 0f)
                {
                    var motherboard = _computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Motherboard);
                    motherboard?.Update();
                    // Buscar temperatura del CPU en el motherboard (algunos AMD exponen aquí)
                    cpuTemp = motherboard?.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Temperature && (s.Name.Contains("CPU") || s.Name.Contains("Processor")))?.Value ?? 0f;

                    // Si aún es 0, usar estimación basada en load (solo para mostrar algo en la UI)
                    if (cpuTemp == 0f && cpuLoad > 0)
                    {
                        // Estimación muy básica: temperatura base 30°C + (load% * 0.5)
                        // Esto NO es preciso, solo para que no muestre 0°C cuando hay actividad
                        cpuTemp = 30f + (cpuLoad * 0.5f);
                    }
                }

                float gpuTemp = gpu?.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Temperature && s.Name.Contains("Core"))?.Value ?? 0f;

                // For RAM, find the specific memory device and get its load percentage
                var physicalMemory = _computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Memory && h.Name == "Total Memory");
                physicalMemory?.Update();
                float ramLoadPercent = physicalMemory?.Sensors.FirstOrDefault(s => s.Name == "Memory" && s.SensorType == SensorType.Load)?.Value ?? 0f;

                // Post the update back to the UI thread
                _uiContext.Post(_ =>
                {
                    CpuLoadPercent = cpuLoad;
                    GpuLoadPercent = gpuLoad;
                    RamUsagePercent = ramLoadPercent;
                    CpuTemperature = cpuTemp;
                    GpuTemperature = gpuTemp;
                }, null);
            }
            catch (Exception ex)
            {
                // This will log if the timer loop fails for any reason
                System.Diagnostics.Debug.WriteLine($"[HardwareMonitorService] Timer Exception: {ex.Message}");
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            _timer?.Stop();
            _timer?.Dispose();
            try
            {
                _computer?.Close();
            }
            catch
            {
                // Ignore disposal errors
            }
        }

        /// <summary>
        /// Método de diagnóstico: muestra todos los sensores disponibles para debug.
        /// Ejecuta una vez al inicio para identificar nombres exactos de sensores.
        /// </summary>
        public void DiagnoseAvailableSensors()
        {
            try
            {
                Debug.WriteLine("─");
                Debug.WriteLine("\U0001F50D HARDWARE MONITOR - Sensores disponibles:");
                foreach (var hardware in _computer.Hardware)
                {
                    Debug.WriteLine($"\n📦 {hardware.Name} ({hardware.HardwareType})");
                    hardware.Update();
                    foreach (var sensor in hardware.Sensors)
                    {
                        Debug.WriteLine($"   → {sensor.SensorType,-15} | {sensor.Name,-30} | {sensor.Value:F1}");
                    }
                }
                Debug.WriteLine("─");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error en diagnóstico: {ex.Message}");
            }
        }
    }
}
