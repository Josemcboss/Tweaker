using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Tweaker.Utilities
{
    /// <summary>
    /// Limpieza de memoria RAM mediante vaciado del Working Set de procesos
    /// Libera memoria física no esencial para mejorar rendimiento en juegos
    /// </summary>
    public static class MemoryCleaner
    {
        // P/Invoke para EmptyWorkingSet - Reduce el Working Set de un proceso
        [DllImport("psapi.dll", SetLastError = true)]
        private static extern bool EmptyWorkingSet(IntPtr hProcess);

        // P/Invoke para SetProcessWorkingSetSize - Alternativa más agresiva
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetProcessWorkingSetSize(IntPtr hProcess, IntPtr dwMinimumWorkingSetSize, IntPtr dwMaximumWorkingSetSize);

        /// <summary>
        /// Limpia memoria de todos los procesos del usuario (excepto críticos y el actual)
        /// </summary>
        /// <returns>Tuple con (éxito, MB liberados, procesos procesados)</returns>
        public static (bool success, long mbCleaned, int processesProcessed) FlushMemory()
        {
            try
            {
                var currentProcess = Process.GetCurrentProcess();
                var currentProcessId = currentProcess.Id;
                
                long totalMemoryBefore = 0;
                long totalMemoryAfter = 0;
                int processesProcessed = 0;
                int successCount = 0;

                Debug.WriteLine("?? MEMORY CLEANER - Iniciando limpieza de memoria...");

                // Obtener todos los procesos del sistema
                var processes = Process.GetProcesses();

                // Lista de procesos críticos que NO se deben tocar
                string[] criticalProcesses = 
                {
                    "system",
                    "csrss",
                    "smss",
                    "lsass",
                    "services",
                    "winlogon",
                    "dwm",
                    "explorer",
                    "svchost",
                    "audiodg",
                    "conhost",
                    "fontdrvhost",
                    "wininit",
                    "ghost optimizer", // Nuestra propia aplicación
                    "tweaker"
                };

                foreach (var process in processes)
                {
                    try
                    {
                        // Saltar si es el proceso actual
                        if (process.Id == currentProcessId)
                            continue;

                        // Saltar procesos del sistema (ID < 10)
                        if (process.Id < 10)
                            continue;

                        var processName = process.ProcessName.ToLower();

                        // Saltar procesos críticos
                        bool isCritical = false;
                        foreach (var critical in criticalProcesses)
                        {
                            if (processName.Contains(critical))
                            {
                                isCritical = true;
                                break;
                            }
                        }

                        if (isCritical)
                            continue;

                        // Obtener memoria antes
                        long memoryBefore = 0;
                        try
                        {
                            memoryBefore = process.WorkingSet64 / 1024 / 1024; // MB
                            totalMemoryBefore += memoryBefore;
                        }
                        catch
                        {
                            continue; // Sin acceso al proceso
                        }

                        // Solo limpiar si usa más de 50MB
                        if (memoryBefore < 50)
                            continue;

                        // Intentar vaciar el Working Set
                        bool success = EmptyWorkingSet(process.Handle);
                        
                        if (success)
                        {
                            processesProcessed++;
                            
                            // Esperar un momento para que se libere la memoria
                            System.Threading.Thread.Sleep(5);

                            try
                            {
                                process.Refresh();
                                long memoryAfter = process.WorkingSet64 / 1024 / 1024; // MB
                                totalMemoryAfter += memoryAfter;
                                
                                long freed = memoryBefore - memoryAfter;
                                if (freed > 10) // Solo loggear si liberó más de 10MB
                                {
                                    Debug.WriteLine($"   ? {process.ProcessName}: {freed}MB liberados");
                                    successCount++;
                                }
                            }
                            catch
                            {
                                // Proceso pudo haber terminado
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Error procesando este proceso específico, continuar con el siguiente
                        Debug.WriteLine($"   ?? Error procesando {process.ProcessName}: {ex.Message}");
                    }
                    finally
                    {
                        process.Dispose();
                    }
                }

                long totalFreed = totalMemoryBefore - totalMemoryAfter;

                Debug.WriteLine($"? MEMORY CLEANER - Completado:");
                Debug.WriteLine($"   Procesos procesados: {processesProcessed}");
                Debug.WriteLine($"   Limpieza exitosa: {successCount}");
                Debug.WriteLine($"   Memoria liberada: ~{totalFreed}MB");

                return (true, totalFreed, processesProcessed);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? MEMORY CLEANER - Error general: {ex.Message}");
                return (false, 0, 0);
            }
        }

        /// <summary>
        /// Limpia memoria de un proceso específico
        /// </summary>
        public static bool FlushProcessMemory(Process process)
        {
            try
            {
                if (process == null || process.HasExited)
                    return false;

                long memoryBefore = process.WorkingSet64 / 1024 / 1024; // MB

                bool success = EmptyWorkingSet(process.Handle);

                if (success)
                {
                    System.Threading.Thread.Sleep(10);
                    process.Refresh();
                    long memoryAfter = process.WorkingSet64 / 1024 / 1024; // MB
                    long freed = memoryBefore - memoryAfter;

                    Debug.WriteLine($"?? Proceso {process.ProcessName}: {freed}MB liberados");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error limpiando memoria del proceso: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Obtiene información de memoria del sistema
        /// </summary>
        public static (long totalMB, long availableMB, int percentUsed) GetSystemMemoryInfo()
        {
            try
            {
                // Usar Performance Counter como alternativa multiplataforma
                var totalMemory = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes / 1024 / 1024;
                
                // Obtener memoria disponible del sistema
                var pc = new PerformanceCounter("Memory", "Available MBytes");
                long availableMB = (long)pc.NextValue();
                
                long totalMB = totalMemory;
                int percentUsed = totalMB > 0 ? (int)((totalMB - availableMB) * 100 / totalMB) : 0;

                return (totalMB, availableMB, percentUsed);
            }
            catch
            {
                return (0, 0, 0);
            }
        }

        /// <summary>
        /// Limpia memoria agresivamente usando SetProcessWorkingSetSize
        /// </summary>
        public static bool AggressiveMemoryFlush(Process process)
        {
            try
            {
                if (process == null || process.HasExited)
                    return false;

                // Forzar Working Set mínimo (-1 = dejar que Windows decida)
                IntPtr minWorkingSet = new IntPtr(-1);
                IntPtr maxWorkingSet = new IntPtr(-1);

                bool success = SetProcessWorkingSetSize(process.Handle, minWorkingSet, maxWorkingSet);

                if (success)
                {
                    Debug.WriteLine($"?? Flush agresivo exitoso en {process.ProcessName}");
                }

                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error en flush agresivo: {ex.Message}");
                return false;
            }
        }
    }
}
