using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// Global High-Precision Timer Resolution Optimization
    /// Fija la resolución del temporizador del sistema a 0.500 ms (5000 unidades de 100ns)
    /// reduciendo la variabilidad en el despacho de frames de GPU y polling de periféricos.
    /// </summary>
    public static class TimerResolutionOptimization
    {
        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern uint NtSetTimerResolution(uint desiredResolution, bool setResolution, out uint currentResolution);

        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern uint NtQueryTimerResolution(out uint minimumResolution, out uint maximumResolution, out uint currentResolution);

        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod", SetLastError = true)]
        private static extern uint TimeBeginPeriod(uint uMilliseconds);

        [DllImport("winmm.dll", EntryPoint = "timeEndPeriod", SetLastError = true)]
        private static extern uint TimeEndPeriod(uint uMilliseconds);

        private static bool _isTimerSet = false;

        /// <summary>
        /// Aplica la resolución máxima del temporizador (0.5ms / 5000 x 100ns)
        /// </summary>
        public static bool EnableHighPrecisionTimer()
        {
            try
            {
                Debug.WriteLine("──────────────────────────────────────────");
                Debug.WriteLine("⚡ ESTABLECIENDO RESOLUCIÓN GLOBAL DE TIMER A 0.5ms");
                Debug.WriteLine("──────────────────────────────────────────");

                // Solicitar 0.5ms (5000 en escala de 100ns)
                uint status = NtSetTimerResolution(5000, true, out uint currentResolution);
                TimeBeginPeriod(1);

                NtQueryTimerResolution(out uint min, out uint max, out uint curr);
                Debug.WriteLine($"  ✔ Timer Resolution actual: {curr / 10000.0:F4} ms (Min: {min / 10000.0:F2}ms, Max: {max / 10000.0:F2}ms)");

                _isTimerSet = true;
                return status == 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error en EnableHighPrecisionTimer: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Restaura la resolución del temporizador a la gestión por defecto de Windows
        /// </summary>
        public static bool RestoreTimerResolution()
        {
            try
            {
                Debug.WriteLine("Restaurando resolución estándar de timer...");
                NtSetTimerResolution(5000, false, out _);
                TimeEndPeriod(1);
                _isTimerSet = false;
                Debug.WriteLine("✔ Timer Resolution restaurado");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error en RestoreTimerResolution: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Comprueba si el temporizador de alta precisión está activo
        /// </summary>
        public static bool? IsHighPrecisionTimerActive()
        {
            try
            {
                NtQueryTimerResolution(out _, out _, out uint current);
                // Si la resolución actual es <= 1.0 ms (10000 en escala 100ns), se considera optimizado
                return current <= 10000 || _isTimerSet;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Obtiene la resolución actual del temporizador en milisegundos
        /// </summary>
        public static double GetCurrentResolutionMs()
        {
            try
            {
                NtQueryTimerResolution(out _, out _, out uint current);
                return current / 10000.0;
            }
            catch
            {
                return 15.625;
            }
        }
    }
}
