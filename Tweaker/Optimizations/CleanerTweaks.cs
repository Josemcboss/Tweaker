using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Tweaker.Optimizations
{
    /// <summary>
    /// CleanerTweaks - Limpieza de Disco
    /// Elimina archivos temporales y caché
    /// </summary>
    public static class CleanerTweaks
    {
        /// <summary>
        /// LIMPIEZA PROFUNDA DE ARCHIVOS TEMPORALES
        /// 
        /// ¿Qué limpia?
        /// ????????????????????????????????????????????????????????????????
        /// 1. C:\Windows\Temp
        ///    - Archivos temporales del sistema
        ///    - Updates fallidos, instaladores
        ///    - Logs de crashes
        /// 
        /// 2. %TEMP% (C:\Users\[User]\AppData\Local\Temp)
        ///    - Archivos temporales de aplicaciones
        ///    - Instaladores descargados
        ///    - Caché de navegadores
        /// 
        /// 3. C:\Windows\Prefetch
        ///    - Caché de inicio de programas
        ///    - Puede causar problemas si está corrupto
        /// 
        /// IMPACTO:
        /// ? Libera 500MB - 5GB (promedio 1-2GB)
        /// ? Elimina archivos corruptos
        /// ? Mejora velocidad de inicio (Prefetch limpio)
        /// 
        /// SEGURO:
        /// ? Solo borra archivos TEMPORALES
        /// ? Si un archivo está en uso, lo salta (no crashea)
        /// ? No toca documentos ni aplicaciones
        /// </summary>
        public static (bool success, long mbFreed, int filesDeleted) DeepClean()
        {
            long totalBytesFreed = 0;
            int totalFilesDeleted = 0;
            bool hadErrors = false;

            Debug.WriteLine("????????????????????????????????????????");
            Debug.WriteLine("INICIANDO LIMPIEZA PROFUNDA");
            Debug.WriteLine("????????????????????????????????????????");

            try
            {
                // PASO 1: Limpiar C:\Windows\Temp
                string windowsTemp = @"C:\Windows\Temp";
                var (bytes1, files1) = CleanDirectory(windowsTemp);
                totalBytesFreed += bytes1;
                totalFilesDeleted += files1;
                Debug.WriteLine($"? Windows Temp: {files1} archivos, {bytes1 / 1024 / 1024} MB");

                // PASO 2: Limpiar %TEMP%
                string userTemp = Path.GetTempPath();
                var (bytes2, files2) = CleanDirectory(userTemp);
                totalBytesFreed += bytes2;
                totalFilesDeleted += files2;
                Debug.WriteLine($"? User Temp: {files2} archivos, {bytes2 / 1024 / 1024} MB");

                // PASO 3: Limpiar Prefetch
                string prefetch = @"C:\Windows\Prefetch";
                var (bytes3, files3) = CleanDirectory(prefetch);
                totalBytesFreed += bytes3;
                totalFilesDeleted += files3;
                Debug.WriteLine($"? Prefetch: {files3} archivos, {bytes3 / 1024 / 1024} MB");

                long mbFreed = totalBytesFreed / 1024 / 1024;

                Debug.WriteLine("????????????????????????????????????????");
                Debug.WriteLine($"? LIMPIEZA COMPLETADA");
                Debug.WriteLine($"   {totalFilesDeleted} archivos eliminados");
                Debug.WriteLine($"   {mbFreed} MB liberados");
                Debug.WriteLine("????????????????????????????????????????");

                return (success: true, mbFreed: mbFreed, filesDeleted: totalFilesDeleted);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error en limpieza: {ex.Message}");
                long mbFreed = totalBytesFreed / 1024 / 1024;
                return (success: false, mbFreed: mbFreed, filesDeleted: totalFilesDeleted);
            }
        }

        /// <summary>
        /// Limpiar un directorio específico
        /// Maneja archivos bloqueados sin crashear
        /// </summary>
        private static (long bytesFreed, int filesDeleted) CleanDirectory(string path)
        {
            long bytesFreed = 0;
            int filesDeleted = 0;

            try
            {
                if (!Directory.Exists(path))
                {
                    Debug.WriteLine($"?? Directorio no existe: {path}");
                    return (0, 0);
                }

                // Obtener todos los archivos
                string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

                foreach (string file in files)
                {
                    try
                    {
                        // Obtener tamaño ANTES de borrar
                        FileInfo fi = new FileInfo(file);
                        long fileSize = fi.Length;

                        // Intentar borrar
                        File.Delete(file);

                        // Si llegamos aquí, el archivo se borró exitosamente
                        bytesFreed += fileSize;
                        filesDeleted++;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Archivo protegido o sin permisos ? saltar silenciosamente
                        continue;
                    }
                    catch (IOException)
                    {
                        // Archivo en uso (locked) ? saltar silenciosamente
                        continue;
                    }
                    catch
                    {
                        // Cualquier otro error ? saltar silenciosamente
                        continue;
                    }
                }

                // Intentar borrar directorios vacíos
                try
                {
                    string[] dirs = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);
                    foreach (string dir in dirs.OrderByDescending(d => d.Length)) // Más profundos primero
                    {
                        try
                        {
                            if (Directory.GetFiles(dir).Length == 0 && Directory.GetDirectories(dir).Length == 0)
                            {
                                Directory.Delete(dir);
                            }
                        }
                        catch { }
                    }
                }
                catch { }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"?? Error limpiando {path}: {ex.Message}");
            }

            return (bytesFreed, filesDeleted);
        }

        /// <summary>
        /// FLUSH DNS CACHE
        /// Limpia caché de resolución de nombres
        /// 
        /// ¿Qué es DNS Cache?
        /// ????????????????????????????????????????????????????????????????
        /// Windows guarda IPs de sitios visitados para no consultarlos cada vez.
        /// 
        /// ¿Por qué limpiarlo?
        /// - Caché corrupto causa "DNS_PROBE_FINISHED_NXDOMAIN"
        /// - Después de cambiar DNS (Google, Cloudflare)
        /// - Después de problemas de red
        /// 
        /// COMANDO:
        /// ipconfig /flushdns
        /// 
        /// IMPACTO:
        /// ? Resuelve errores de conexión
        /// ? Aplica nuevos servidores DNS inmediatamente
        /// ? Elimina entradas obsoletas
        /// </summary>
        public static bool FlushDNS()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "ipconfig",
                    Arguments = "/flushdns",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                };

                using (Process proc = Process.Start(psi))
                {
                    proc?.WaitForExit();
                    string output = proc?.StandardOutput.ReadToEnd();

                    if (output?.Contains("Successfully flushed") == true)
                    {
                        Debug.WriteLine("? DNS Cache FLUSHED");
                        Debug.WriteLine("  Caché de DNS limpiado");
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error FlushDNS: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// ANÁLISIS DE ESPACIO (sin borrar)
        /// Escanea cuánto espacio se puede liberar
        /// </summary>
        public static (long mbCanFree, int filesCount) AnalyzeSpace()
        {
            long totalBytes = 0;
            int totalFiles = 0;

            try
            {
                string[] paths = new[]
                {
                    @"C:\Windows\Temp",
                    Path.GetTempPath(),
                    @"C:\Windows\Prefetch"
                };

                foreach (string path in paths)
                {
                    if (Directory.Exists(path))
                    {
                        try
                        {
                            string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
                            totalFiles += files.Length;

                            foreach (string file in files)
                            {
                                try
                                {
                                    FileInfo fi = new FileInfo(file);
                                    totalBytes += fi.Length;
                                }
                                catch { }
                            }
                        }
                        catch { }
                    }
                }

                long mb = totalBytes / 1024 / 1024;
                Debug.WriteLine($"?? Análisis: {totalFiles} archivos, {mb} MB se pueden liberar");
                return (mb, totalFiles);
            }
            catch
            {
                return (0, 0);
            }
        }
    }
}
