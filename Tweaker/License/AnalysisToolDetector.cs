using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace Tweaker.License
{
    /// <summary>
    /// Detecta herramientas de an�lisis y descompilaci�n como dnSpy, ILSpy, dotPeek
    /// Dificulta la ingenier�a inversa del sistema de licencias
    /// </summary>
    internal static class AnalysisToolDetector
    {
        // Windows API para detecci�n de ventanas
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        /// <summary>
        /// Verifica si hay herramientas de an�lisis ejecut�ndose
        /// </summary>
        public static bool IsAnalysisToolRunning()
        {
            try
            {
                // Verificar procesos sospechosos
                if (DetectSuspiciousProcesses())
                {
                    LogToolDetection("Proceso sospechoso detectado");
                    return true;
                }

                // Verificar ventanas de herramientas conocidas
                if (DetectSuspiciousWindows())
                {
                    LogToolDetection("Ventana de herramienta detectada");
                    return true;
                }

                // Verificar m�dulos cargados sospechosos
                if (DetectSuspiciousModules())
                {
                    LogToolDetection("M�dulo sospechoso cargado");
                    return true;
                }

                return false;
            }
            catch
            {
                // Si falla la verificaci�n, asumir herramienta presente
                return true;
            }
        }

        /// <summary>
        /// Detecta procesos de herramientas de an�lisis conocidas
        /// </summary>
        private static bool DetectSuspiciousProcesses()
        {
            try
            {
                // Lista de herramientas de an�lisis conocidas
                string[] analysisTools =
                {
                    // Decompilers / Disassemblers
                    "dnspy",
                    "ilspy",
                    "dotpeek",
                    "justdecompile",
                    "reflector",
                    "resharp",
                    "jetbrains.resharper",
                    "rider",
                    
                    // Debuggers
                    "windbg",
                    "x64dbg",
                    "x32dbg",
                    "ollydbg",
                    "ida",
                    "ida64",
                    "idaq",
                    "idaq64",
                    
                    // Memory editors / Trainers
                    "cheatengine",
                    "artmoney",
                    "gameguardian",
                    
                    // Network analyzers
                    "wireshark",
                    "fiddler",
                    "charles",
                    
                    // PE editors
                    "cff explorer",
                    "pe explorer",
                    "pe-bear",
                    "pestudio",
                    "resource hacker",
                    
                    // .NET profilers
                    "dotmemory",
                    "dottrace",
                    "ants profiler",
                    
                    // De-obfuscators
                    "de4dot",
                    "simpleassemblyexplorer",
                    
                    // Process monitors
                    "procmon",
                    "process explorer",
                    "process hacker",
                    "systeminformer"
                };

                var runningProcesses = Process.GetProcesses();

                foreach (var process in runningProcesses)
                {
                    try
                    {
                        var processName = process.ProcessName.ToLower();

                        foreach (var tool in analysisTools)
                        {
                            if (processName.Contains(tool.Replace(" ", "")))
                            {
                                Debug.WriteLine($"?? Herramienta detectada: {process.ProcessName}");
                                return true;
                            }
                        }
                    }
                    catch
                    {
                        // Proceso sin acceso, continuar
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
        /// Detecta ventanas de herramientas de an�lisis
        /// </summary>
        private static bool DetectSuspiciousWindows()
        {
            try
            {
                // T�tulos de ventanas de herramientas conocidas
                string[] suspiciousWindowTitles =
                {
                    "dnSpy",
                    "ILSpy",
                    "dotPeek",
                    "JustDecompile",
                    ".NET Reflector",
                    "Cheat Engine",
                    "x64dbg",
                    "x32dbg",
                    "OllyDbg",
                    "WinDbg",
                    "IDA Pro",
                    "Fiddler",
                    "Wireshark",
                    "Process Monitor",
                    "Process Explorer",
                    "Process Hacker",
                    "System Informer"
                };

                bool toolDetected = false;

                EnumWindows((hWnd, lParam) =>
                {
                    try
                    {
                        var sb = new System.Text.StringBuilder(256);
                        GetWindowText(hWnd, sb, sb.Capacity);
                        var windowTitle = sb.ToString();

                        if (!string.IsNullOrEmpty(windowTitle))
                        {
                            foreach (var suspiciousTitle in suspiciousWindowTitles)
                            {
                                if (windowTitle.Contains(suspiciousTitle, StringComparison.OrdinalIgnoreCase))
                                {
                                    Debug.WriteLine($"?? Ventana sospechosa: {windowTitle}");
                                    toolDetected = true;
                                    return false; // Detener enumeraci�n
                                }
                            }
                        }
                    }
                    catch
                    {
                        // Error al obtener t�tulo, continuar
                    }

                    return true; // Continuar enumeraci�n
                }, IntPtr.Zero);

                return toolDetected;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Detecta m�dulos DLL sospechosos cargados en el proceso
        /// </summary>
        private static bool DetectSuspiciousModules()
        {
            try
            {
                var currentProcess = Process.GetCurrentProcess();

                // M�dulos sospechosos (profilers, hooks, etc.)
                string[] suspiciousModules =
                {
                    "easyhook",
                    "detours",
                    "minhook",
                    "harmony",
                    "profiler",
                    "injector",
                    "clrprofiler",
                    "cor_profiler",
                    "vsjitdebugger"
                };

                foreach (ProcessModule module in currentProcess.Modules)
                {
                    try
                    {
                        var moduleName = module.ModuleName.ToLower();

                        foreach (var suspicious in suspiciousModules)
                        {
                            if (moduleName.Contains(suspicious))
                            {
                                Debug.WriteLine($"?? M�dulo sospechoso: {module.ModuleName}");
                                return true;
                            }
                        }
                    }
                    catch
                    {
                        // M�dulo sin acceso, continuar
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
        /// Verifica si estamos corriendo bajo un profiler de .NET
        /// </summary>
        public static bool IsProfilerAttached()
        {
            try
            {
                // Verificar variable de entorno de profiler
                var profilerEnvVars = new[]
                {
                    "COR_ENABLE_PROFILING",
                    "COR_PROFILER",
                    "COR_PROFILER_PATH",
                    "CORECLR_ENABLE_PROFILING",
                    "CORECLR_PROFILER"
                };

                foreach (var envVar in profilerEnvVars)
                {
                    var value = Environment.GetEnvironmentVariable(envVar);
                    if (!string.IsNullOrEmpty(value) && value != "0")
                    {
                        Debug.WriteLine($"?? Profiler detectado via {envVar}");
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
        /// Verifica la integridad del ensamblado actual
        /// </summary>
        public static bool IsAssemblyModified()
        {
            try
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();

                // Usar AppContext.BaseDirectory en lugar de Assembly.Location
                // para compatibilidad con single-file apps
                var appDirectory = AppContext.BaseDirectory;
                var assemblyName = assembly.GetName().Name ?? "Tweaker";
                var assemblyPath = System.IO.Path.Combine(appDirectory, assemblyName + ".dll");

                // Verificar si el archivo existe donde deber�a
                if (!System.IO.File.Exists(assemblyPath))
                {
                    // En single-file apps, el ensamblado puede estar embebido
                    Debug.WriteLine("?? Aplicaci�n en modo single-file o ensamblado embebido");
                    return false; // No considerar como modificado en single-file
                }

                // Verificar firma digital (si existe)
                // Nota: Esto requerir�a implementaci�n adicional con certificados

                return false;
            }
            catch
            {
                return false; // No asumir modificado por error de verificaci�n
            }
        }

        /// <summary>
        /// Registra la detecci�n de herramienta
        /// </summary>
        private static void LogToolDetection(string details)
        {
            try
            {
                Debug.WriteLine($"?? ALERTA: Herramienta de an�lisis detectada");
                Debug.WriteLine($"   Detalles: {details}");
                Debug.WriteLine($"   Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            }
            catch
            {
                // Silenciar errores de logging
            }
        }

        /// <summary>
        /// Maneja la detecci�n de herramientas de an�lisis
        /// </summary>
        public static void HandleToolDetection()
        {
            try
            {
                Debug.WriteLine("?? HERRAMIENTA DE AN�LISIS DETECTADA - Terminando proceso");

                // Invalidar licencia
                LicenseStorage.DeleteLicense();

                // Esperar un momento
                System.Threading.Thread.Sleep(100);

                // Terminar proceso
                Environment.FailFast("Herramienta de an�lisis detectada. Proceso terminado por seguridad.");
            }
            catch
            {
                Environment.Exit(-1);
            }
        }

        /// <summary>
        /// Ejecuta todas las verificaciones y retorna si se detect� algo
        /// </summary>
        public static bool PerformFullCheck()
        {
            try
            {
                if (IsAnalysisToolRunning())
                    return true;

                if (IsProfilerAttached())
                    return true;

                if (IsAssemblyModified())
                    return true;

                return false;
            }
            catch
            {
                // En caso de error, asumir que hay herramientas
                return true;
            }
        }

        /// <summary>
        /// Verifica si estamos en entorno de desarrollo (permite herramientas)
        /// </summary>
        public static bool IsDevelopmentEnvironment()
        {
            try
            {
#if DEBUG
                return true;
#else
                // Verificar variables de entorno de desarrollo
                var devEnvVars = new[] 
                { 
                    "VSAPPIDDIR",
                    "VisualStudioVersion",
                    "RIDER_HOME",
                    "DEVELOPMENT_MODE"
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
