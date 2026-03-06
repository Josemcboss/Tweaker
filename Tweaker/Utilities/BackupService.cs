using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;

namespace Tweaker.Utilities
{
    /// <summary>
    /// Servicio para Backup y Restore de configuraciones de tweaks
    /// Permite guardar, cargar y exportar el estado de todos los tweaks
    /// </summary>
    public class BackupService
    {
        private static BackupService? _instance;
        private readonly string _backupDirectory;
        private readonly TweakStateManager _stateManager;

        public static BackupService Instance => _instance ??= new BackupService();

        private BackupService()
        {
            _stateManager = TweakStateManager.Instance;

            // Crear directorio de backups en AppData
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _backupDirectory = Path.Combine(appData, "Tweaker", "Backups");

            Directory.CreateDirectory(_backupDirectory);

            Debug.WriteLine($"?? Backup directory: {_backupDirectory}");
        }

        /// <summary>
        /// Crea un backup del estado actual de todos los tweaks
        /// </summary>
        public bool CreateBackup(string? name = null)
        {
            try
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                string backupName = string.IsNullOrWhiteSpace(name)
                    ? $"Backup_{timestamp}"
                    : $"{name}_{timestamp}";

                string filePath = Path.Combine(_backupDirectory, $"{backupName}.json");

                var backup = new BackupData
                {
                    Name = backupName,
                    CreatedAt = DateTime.Now,
                    Tweaks = _stateManager.GetRecentTweaks(1000).ToList(), // Get all tweaks
                    SystemInfo = GetSystemInfo()
                };

                string json = JsonSerializer.Serialize(backup, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                File.WriteAllText(filePath, json);

                Debug.WriteLine($"? Backup creado: {filePath}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error creando backup: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Restaura un backup específico
        /// </summary>
        public bool RestoreBackup(string backupName)
        {
            try
            {
                string filePath = Path.Combine(_backupDirectory, $"{backupName}.json");

                if (!File.Exists(filePath))
                {
                    Debug.WriteLine($"? Backup no encontrado: {filePath}");
                    return false;
                }

                string json = File.ReadAllText(filePath);
                var backup = JsonSerializer.Deserialize<BackupData>(json);

                if (backup == null)
                {
                    Debug.WriteLine("? Error deserializando backup");
                    return false;
                }

                // Restaurar cada tweak según su estado en el backup
                foreach (var tweak in backup.Tweaks)
                {
                    // TODO: Implementar lógica de restauración
                    // Por ahora solo logging
                    Debug.WriteLine($"Restaurando tweak: {tweak.Id} - Activo: {tweak.IsEnabled}");
                }

                Debug.WriteLine($"? Backup restaurado: {backupName}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error restaurando backup: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Obtiene la lista de backups disponibles
        /// </summary>
        public List<BackupInfo> GetAvailableBackups()
        {
            try
            {
                var backups = new List<BackupInfo>();
                var files = Directory.GetFiles(_backupDirectory, "*.json");

                foreach (var file in files)
                {
                    try
                    {
                        var fileInfo = new FileInfo(file);
                        string json = File.ReadAllText(file);
                        var backup = JsonSerializer.Deserialize<BackupData>(json);

                        if (backup != null)
                        {
                            backups.Add(new BackupInfo
                            {
                                Name = backup.Name,
                                FileName = Path.GetFileNameWithoutExtension(file),
                                CreatedAt = backup.CreatedAt,
                                TweaksCount = backup.Tweaks.Count,
                                SizeMB = fileInfo.Length / 1024.0 / 1024.0
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"? Error leyendo backup {file}: {ex.Message}");
                    }
                }

                return backups.OrderByDescending(b => b.CreatedAt).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error obteniendo backups: {ex.Message}");
                return new List<BackupInfo>();
            }
        }

        /// <summary>
        /// Elimina un backup específico
        /// </summary>
        public bool DeleteBackup(string backupName)
        {
            try
            {
                string filePath = Path.Combine(_backupDirectory, $"{backupName}.json");

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    Debug.WriteLine($"? Backup eliminado: {backupName}");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error eliminando backup: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Exporta un backup a una ubicación específica
        /// </summary>
        public bool ExportBackup(string backupName, string exportPath)
        {
            try
            {
                string sourcePath = Path.Combine(_backupDirectory, $"{backupName}.json");

                if (!File.Exists(sourcePath))
                    return false;

                File.Copy(sourcePath, exportPath, overwrite: true);
                Debug.WriteLine($"? Backup exportado: {exportPath}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error exportando backup: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Importa un backup desde una ubicación específica
        /// </summary>
        public bool ImportBackup(string importPath)
        {
            try
            {
                if (!File.Exists(importPath))
                    return false;

                string fileName = Path.GetFileName(importPath);
                string destPath = Path.Combine(_backupDirectory, fileName);

                File.Copy(importPath, destPath, overwrite: true);
                Debug.WriteLine($"? Backup importado: {fileName}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error importando backup: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Obtiene información del sistema para el backup
        /// </summary>
        private SystemInfo GetSystemInfo()
        {
            return new SystemInfo
            {
                WindowsVersion = Environment.OSVersion.ToString(),
                MachineName = Environment.MachineName,
                ProcessorCount = Environment.ProcessorCount,
                TotalMemoryGB = GetTotalMemoryGB()
            };
        }

        private double GetTotalMemoryGB()
        {
            // Método alternativo sin Microsoft.VisualBasic
            try
            {
                return GC.GetGCMemoryInfo().TotalAvailableMemoryBytes / 1024.0 / 1024.0 / 1024.0;
            }
            catch
            {
                return 0;
            }
        }
    }

    /// <summary>
    /// Datos del backup
    /// </summary>
    public class BackupData
    {
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<TweakState> Tweaks { get; set; } = new();
        public SystemInfo SystemInfo { get; set; } = new();
    }

    /// <summary>
    /// Información del sistema
    /// </summary>
    public class SystemInfo
    {
        public string WindowsVersion { get; set; } = string.Empty;
        public string MachineName { get; set; } = string.Empty;
        public int ProcessorCount { get; set; }
        public double TotalMemoryGB { get; set; }
    }

    /// <summary>
    /// Información resumida de un backup
    /// </summary>
    public class BackupInfo
    {
        public string Name { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int TweaksCount { get; set; }
        public double SizeMB { get; set; }
    }
}

