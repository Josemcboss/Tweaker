using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Tweaker.License
{
    /// <summary>
    /// Sistema anti-debugging para dificultar an�lisis de ingenier�a inversa
    /// Detecta debuggers y termina el proceso si se detecta manipulaci�n
    /// </summary>
    internal static class AntiDebugger
    {
        // Windows API para detecci�n avanzada
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        private static extern bool CheckRemoteDebuggerPresent(IntPtr hProcess, ref bool isDebuggerPresent);

        [DllImport("kernel32.dll")]
        private static extern bool IsDebuggerPresent();

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentProcess();

        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int NtQueryInformationProcess(
            IntPtr processHandle,
            int processInformationClass,
            ref PROCESS_BASIC_INFORMATION processInformation,
            int processInformationLength,
            out int returnLength);

        [StructLayout(LayoutKind.Sequential)]
        private struct PROCESS_BASIC_INFORMATION
        {
            public IntPtr Reserved1;
            public IntPtr PebBaseAddress;
            public IntPtr Reserved2_0;
            public IntPtr Reserved2_1;
            public IntPtr UniqueProcessId;
            public IntPtr InheritedFromUniqueProcessId;
        }

        private static DateTime _lastCheck = DateTime.MinValue;
        private static readonly TimeSpan CheckInterval = TimeSpan.FromSeconds(5);

        /// <summary>
        /// Verifica si hay un debugger adjunto (m�ltiples m�todos)
        /// </summary>
        public static bool IsDebuggerAttached()
        {
            try
            {
                // M�todo 1: Debugger.IsAttached de .NET
                if (Debugger.IsAttached)
                {
                    LogDebuggerDetection("Debugger.IsAttached");
                    return true;
                }

                // M�todo 2: Windows API IsDebuggerPresent
                if (IsDebuggerPresent())
                {
                    LogDebuggerDetection("IsDebuggerPresent API");
                    return true;
                }

                // M�todo 3: CheckRemoteDebuggerPresent
                bool isDebuggerPresent = false;
                CheckRemoteDebuggerPresent(GetCurrentProcess(), ref isDebuggerPresent);
                if (isDebuggerPresent)
                {
                    LogDebuggerDetection("CheckRemoteDebuggerPresent API");
                    return true;
                }

                // M�todo 4: Verificar proceso padre (debuggers suelen ser padres)
                if (IsRunningUnderDebuggerProcess())
                {
                    LogDebuggerDetection("Proceso padre sospechoso");
                    return true;
                }

                // M�todo 5: Timing attack (debuggers ralentizan ejecuci�n)
                if (DetectTimingAnomaly())
                {
                    LogDebuggerDetection("Anomal�a de timing detectada");
                    return true;
                }

                return false;
            }
            catch
            {
                // Si falla la verificaci�n, asumir que hay debugger (medida de seguridad)
                return true;
            }
        }

        /// <summary>
        /// Verifica si el proceso est� siendo ejecutado desde un debugger conocido
        /// </summary>
        private static bool IsRunningUnderDebuggerProcess()
        {
            try
            {
                var currentProcess = Process.GetCurrentProcess();
                var parentProcessId = GetParentProcessId(currentProcess.Handle);

                if (parentProcessId == IntPtr.Zero)
                    return false;

                var parentProcess = Process.GetProcessById(parentProcessId.ToInt32());
                var parentName = parentProcess.ProcessName.ToLower();

                // Lista de procesos debuggers conocidos
                string[] knownDebuggers =
                {
                    "devenv",       // Visual Studio
                    "windbg",       // WinDbg
                    "x64dbg",       // x64dbg
                    "x32dbg",       // x32dbg
                    "ollydbg",      // OllyDbg
                    "ida",          // IDA Pro
                    "ida64",        // IDA Pro 64
                    "idaq",         // IDA Pro
                    "idaq64",       // IDA Pro 64
                    "dnspy",        // dnSpy
                    "ilspy",        // ILSpy
                    "dotpeek",      // dotPeek
                    "resharp",      // ReSharper
                    "rider"         // JetBrains Rider
                };

                foreach (var debugger in knownDebuggers)
                {
                    if (parentName.Contains(debugger))
                    {
                        return true;
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Obtiene el ID del proceso padre
        /// </summary>
        private static IntPtr GetParentProcessId(IntPtr processHandle)
        {
            try
            {
                var pbi = new PROCESS_BASIC_INFORMATION();
                int returnLength;
                int status = NtQueryInformationProcess(
                    processHandle,
                    0, // ProcessBasicInformation
                    ref pbi,
                    Marshal.SizeOf(pbi),
                    out returnLength);

                if (status == 0)
                {
                    return pbi.InheritedFromUniqueProcessId;
                }

                return IntPtr.Zero;
            }
            catch
            {
                return IntPtr.Zero;
            }
        }

        /// <summary>
        /// Detecta anomal�as en el tiempo de ejecuci�n (debuggers ralentizan el c�digo)
        /// </summary>
        private static bool DetectTimingAnomaly()
        {
            try
            {
                var sw = Stopwatch.StartNew();

                // Operaci�n simple que deber�a ejecutarse r�pido
                int sum = 0;
                for (int i = 0; i < 1000; i++)
                {
                    sum += i;
                }

                sw.Stop();

                // Si toma m�s de 10ms para una operaci�n tan simple, probablemente hay un debugger
                if (sw.ElapsedMilliseconds > 10)
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Ejecuta verificaciones continuas en background
        /// </summary>
        public static void StartContinuousCheck()
        {
            var checkThread = new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(CheckInterval);

                    if (IsDebuggerAttached())
                    {
                        // Debugger detectado - tomar acci�n
                        HandleDebuggerDetection();
                    }
                }
            })
            {
                IsBackground = true,
                Priority = ThreadPriority.BelowNormal
            };

            checkThread.Start();
        }

        /// <summary>
        /// Maneja la detecci�n de debugger
        /// </summary>
        internal static void HandleDebuggerDetection()
        {
            try
            {
                // Log para auditor�a
                Debug.WriteLine("?? DEBUGGER DETECTADO - Terminando proceso por seguridad");

                // Invalidar licencia
                InvalidateLicense();

                // Esperar un momento para dar tiempo al log
                Thread.Sleep(100);

                // Terminar el proceso de forma abrupta
                Environment.FailFast("Debugger detectado. Proceso terminado por seguridad.");
            }
            catch
            {
                // Fallback: forzar salida
                Environment.Exit(-1);
            }
        }

        /// <summary>
        /// Invalida la licencia actual por detecci�n de debugger
        /// </summary>
        private static void InvalidateLicense()
        {
            try
            {
                // Eliminar licencia almacenada
                LicenseStorage.DeleteLicense();
            }
            catch
            {
                // Silenciar errores
            }
        }

        /// <summary>
        /// Registra la detecci�n de debugger (para auditor�a)
        /// </summary>
        private static void LogDebuggerDetection(string method)
        {
            try
            {
                Debug.WriteLine($"?? ALERTA DE SEGURIDAD: Debugger detectado via {method}");
                Debug.WriteLine($"   Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                Debug.WriteLine($"   Proceso: {Process.GetCurrentProcess().ProcessName}");
            }
            catch
            {
                // Silenciar errores de logging
            }
        }

        /// <summary>
        /// Verifica debugger solo si ha pasado el intervalo de tiempo
        /// </summary>
        public static bool CheckDebuggerThrottled()
        {
            if (DateTime.Now - _lastCheck < CheckInterval)
            {
                return false; // No verificar a�n
            }

            _lastCheck = DateTime.Now;
            return IsDebuggerAttached();
        }

        /// <summary>
        /// Verifica si el entorno es de desarrollo (para permitir debugging leg�timo)
        /// </summary>
        public static bool IsDevelopmentEnvironment()
        {
            try
            {
                // Verificar si estamos en DEBUG build
#if DEBUG
                return true;
#else
                // Verificar variables de entorno de desarrollo
                var devEnvVars = new[] 
                { 
                    "VSAPPIDDIR",           // Visual Studio
                    "VisualStudioVersion",   // Visual Studio
                    "RIDER_HOME",            // JetBrains Rider
                    "DEVELOPMENT_MODE"       // Variable custom
                };

                foreach (var envVar in devEnvVars)
                {
                    if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable(envVar)))
                    {
                        return true;
                    }
                }

                return false;
#endif
            }
            catch
            {
                return false;
            }
        }
    }
}
