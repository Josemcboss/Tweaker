using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

using Microsoft.Win32;

namespace Tweaker.Utilities
{
    /// <summary>
    /// Servicio de backup automático de valores del registro
    /// Guarda los valores originales antes de modificarlos para poder restaurarlos
    /// </summary>
    public static class RegistryBackupService
    {
        private static readonly string BackupFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Tweaker",
            "registry_backup.json");

        private static readonly object _lock = new object();
        private static Dictionary<string, RegistryBackupEntry> _backups = new Dictionary<string, RegistryBackupEntry>();
        private static bool _isInitialized = false;

        /// <summary>
        /// Entrada de backup del registro
        /// </summary>
        private class RegistryBackupEntry
        {
            public string KeyPath { get; set; }
            public string ValueName { get; set; }
            public object OriginalValue { get; set; }
            public RegistryValueKind ValueKind { get; set; }
            public DateTime BackupDate { get; set; }
            public string TweakId { get; set; }

            public RegistryBackupEntry() { }

            public RegistryBackupEntry(string keyPath, string valueName, object originalValue,
                                      RegistryValueKind valueKind, string tweakId)
            {
                KeyPath = keyPath;
                ValueName = valueName;
                OriginalValue = originalValue;
                ValueKind = valueKind;
                BackupDate = DateTime.Now;
                TweakId = tweakId;
            }
        }

        /// <summary>
        /// Inicializa el servicio y carga backups existentes
        /// </summary>
        public static void Initialize()
        {
            lock (_lock)
            {
                if (_isInitialized)
                    return;

                try
                {
                    // Crear directorio si no existe
                    var directory = Path.GetDirectoryName(BackupFilePath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    // Cargar backups existentes
                    LoadFromDisk();

                    _isInitialized = true;
                    Debug.WriteLine("??? RegistryBackupService inicializado correctamente");
                    Debug.WriteLine($"   ?? Archivo de backup: {BackupFilePath}");
                    Debug.WriteLine($"   ?? Backups cargados: {_backups.Count}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"? Error inicializando RegistryBackupService: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Hace backup de un valor del registro ANTES de modificarlo
        /// </summary>
        /// <param name="keyPath">Ruta de la clave del registro (ej: HKLM\SYSTEM\...)</param>
        /// <param name="valueName">Nombre del valor</param>
        /// <param name="tweakId">ID del tweak que modifica este valor</param>
        /// <returns>True si se hizo el backup exitosamente</returns>
        public static bool BackupValue(string keyPath, string valueName, string tweakId)
        {
            lock (_lock)
            {
                try
                {
                    // Inicializar si no se ha hecho
                    if (!_isInitialized)
                        Initialize();

                    // Generar clave única para este valor
                    string backupKey = $"{keyPath}\\{valueName}";

                    // Si ya existe un backup, no sobrescribir (queremos el valor original)
                    if (_backups.ContainsKey(backupKey))
                    {
                        Debug.WriteLine($"   ?? Backup ya existe para: {backupKey}");
                        return true;
                    }

                    // Leer el valor actual del registro
                    var (exists, value, valueKind) = ReadRegistryValue(keyPath, valueName);

                    if (!exists)
                    {
                        Debug.WriteLine($"   ?? Valor no existe en registro (se guardará como null): {backupKey}");
                        // Guardar null para indicar que el valor no existía
                        value = null;
                        valueKind = RegistryValueKind.Unknown;
                    }

                    // Crear entrada de backup
                    var backupEntry = new RegistryBackupEntry(keyPath, valueName, value, valueKind, tweakId);
                    _backups[backupKey] = backupEntry;

                    Debug.WriteLine($"   ? Backup creado: {backupKey}");
                    Debug.WriteLine($"      Valor original: {value ?? "null"}");
                    Debug.WriteLine($"      Tipo: {valueKind}");

                    // Guardar a disco inmediatamente
                    SaveToDisk();

                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"? Error haciendo backup de {keyPath}\\{valueName}: {ex.Message}");
                    return false;
                }
            }
        }

        /// <summary>
        /// Lee un valor del registro
        /// </summary>
        private static (bool exists, object value, RegistryValueKind kind) ReadRegistryValue(string keyPath, string valueName)
        {
            try
            {
                // Parsear la ruta del registro
                var (rootKey, subKeyPath) = ParseRegistryPath(keyPath);
                if (rootKey == null)
                    return (false, null, RegistryValueKind.Unknown);

                using (var key = rootKey.OpenSubKey(subKeyPath, false))
                {
                    if (key == null)
                        return (false, null, RegistryValueKind.Unknown);

                    // Verificar si el valor existe
                    var valueNames = key.GetValueNames();
                    bool valueExists = Array.Exists(valueNames, v => v.Equals(valueName, StringComparison.OrdinalIgnoreCase));

                    if (!valueExists)
                        return (false, null, RegistryValueKind.Unknown);

                    // Leer valor y tipo
                    var value = key.GetValue(valueName);
                    var kind = key.GetValueKind(valueName);

                    return (true, value, kind);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"   ?? Error leyendo valor del registro: {ex.Message}");
                return (false, null, RegistryValueKind.Unknown);
            }
        }

        /// <summary>
        /// Parsea una ruta del registro en root key y subkey path
        /// </summary>
        private static (RegistryKey rootKey, string subKeyPath) ParseRegistryPath(string fullPath)
        {
            try
            {
                // Formato esperado: HKLM\System\... o HKEY_LOCAL_MACHINE\System\...
                string[] parts = fullPath.Split(new[] { '\\' }, 2);
                if (parts.Length < 2)
                    return (null, null);

                string rootKeyName = parts[0].ToUpper();
                string subKeyPath = parts[1];

                RegistryKey rootKey = rootKeyName switch
                {
                    "HKLM" or "HKEY_LOCAL_MACHINE" => Registry.LocalMachine,
                    "HKCU" or "HKEY_CURRENT_USER" => Registry.CurrentUser,
                    "HKCR" or "HKEY_CLASSES_ROOT" => Registry.ClassesRoot,
                    "HKU" or "HKEY_USERS" => Registry.Users,
                    "HKCC" or "HKEY_CURRENT_CONFIG" => Registry.CurrentConfig,
                    _ => null
                };

                return (rootKey, subKeyPath);
            }
            catch
            {
                return (null, null);
            }
        }

        /// <summary>
        /// Restaura TODOS los valores del registro a sus valores originales
        /// </summary>
        /// <returns>Número de valores restaurados exitosamente</returns>
        public static int RestoreAll()
        {
            lock (_lock)
            {
                int restored = 0;
                int failed = 0;

                Debug.WriteLine("???????????????????????????????????????????????????????????????");
                Debug.WriteLine("?? RESTAURANDO TODOS LOS VALORES DEL REGISTRO...");
                Debug.WriteLine("???????????????????????????????????????????????????????????????");

                foreach (var kvp in _backups)
                {
                    var entry = kvp.Value;

                    try
                    {
                        Debug.WriteLine($"   ?? Restaurando: {entry.KeyPath}\\{entry.ValueName}");

                        bool success = RestoreSingleValue(entry);
                        if (success)
                        {
                            restored++;
                            Debug.WriteLine($"      ? Restaurado a: {entry.OriginalValue ?? "null"}");
                        }
                        else
                        {
                            failed++;
                            Debug.WriteLine($"      ? Error restaurando");
                        }
                    }
                    catch (Exception ex)
                    {
                        failed++;
                        Debug.WriteLine($"      ? Excepción: {ex.Message}");
                    }
                }

                Debug.WriteLine("???????????????????????????????????????????????????????????????");
                Debug.WriteLine($"? Restauración completada:");
                Debug.WriteLine($"   • Exitosos: {restored}");
                Debug.WriteLine($"   • Fallidos: {failed}");
                Debug.WriteLine("???????????????????????????????????????????????????????????????");

                // Limpiar backups después de restaurar
                _backups.Clear();
                SaveToDisk();

                return restored;
            }
        }

        /// <summary>
        /// Restaura un solo valor del registro
        /// </summary>
        private static bool RestoreSingleValue(RegistryBackupEntry entry)
        {
            try
            {
                var (rootKey, subKeyPath) = ParseRegistryPath(entry.KeyPath);
                if (rootKey == null)
                    return false;

                using (var key = rootKey.OpenSubKey(subKeyPath, true))
                {
                    if (key == null)
                    {
                        // La clave no existe, intentar crearla
                        using (var newKey = rootKey.CreateSubKey(subKeyPath))
                        {
                            if (newKey == null)
                                return false;

                            RestoreValueToKey(newKey, entry);
                        }
                    }
                    else
                    {
                        RestoreValueToKey(key, entry);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"   ? Error restaurando valor: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Restaura el valor a una clave específica
        /// </summary>
        private static void RestoreValueToKey(RegistryKey key, RegistryBackupEntry entry)
        {
            if (entry.OriginalValue == null)
            {
                // El valor no existía originalmente, eliminarlo
                try
                {
                    key.DeleteValue(entry.ValueName, false);
                }
                catch
                {
                    // Valor no existe, no hay problema
                }
            }
            else
            {
                // Restaurar el valor original
                key.SetValue(entry.ValueName, entry.OriginalValue, entry.ValueKind);
            }
        }

        /// <summary>
        /// Guarda los backups en disco
        /// </summary>
        public static void SaveToDisk()
        {
            lock (_lock)
            {
                try
                {
                    var json = JsonSerializer.Serialize(_backups, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

                    File.WriteAllText(BackupFilePath, json);
                    Debug.WriteLine($"   ?? Backups guardados en disco: {_backups.Count} entradas");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"? Error guardando backups a disco: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Carga los backups desde disco
        /// </summary>
        public static void LoadFromDisk()
        {
            lock (_lock)
            {
                try
                {
                    if (!File.Exists(BackupFilePath))
                    {
                        Debug.WriteLine("   ?? No se encontró archivo de backups previo");
                        return;
                    }

                    var json = File.ReadAllText(BackupFilePath);
                    _backups = JsonSerializer.Deserialize<Dictionary<string, RegistryBackupEntry>>(json)
                              ?? new Dictionary<string, RegistryBackupEntry>();

                    Debug.WriteLine($"   ?? Backups cargados desde disco: {_backups.Count} entradas");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"? Error cargando backups desde disco: {ex.Message}");
                    _backups = new Dictionary<string, RegistryBackupEntry>();
                }
            }
        }

        /// <summary>
        /// Obtiene el número de backups almacenados
        /// </summary>
        public static int GetBackupCount()
        {
            lock (_lock)
            {
                return _backups.Count;
            }
        }

        /// <summary>
        /// Limpia todos los backups (sin restaurar)
        /// </summary>
        public static void ClearAllBackups()
        {
            lock (_lock)
            {
                _backups.Clear();
                SaveToDisk();
                Debug.WriteLine("??? Todos los backups han sido eliminados");
            }
        }

        /// <summary>
        /// Obtiene información sobre los backups almacenados
        /// </summary>
        public static string GetBackupInfo()
        {
            lock (_lock)
            {
                if (_backups.Count == 0)
                    return "No hay backups almacenados.";

                var info = new System.Text.StringBuilder();
                info.AppendLine($"Backups almacenados: {_backups.Count}");
                info.AppendLine();

                foreach (var kvp in _backups)
                {
                    var entry = kvp.Value;
                    info.AppendLine($"• {entry.KeyPath}\\{entry.ValueName}");
                    info.AppendLine($"  Valor original: {entry.OriginalValue ?? "null"}");
                    info.AppendLine($"  Fecha: {entry.BackupDate:yyyy-MM-dd HH:mm:ss}");
                    info.AppendLine($"  Tweak ID: {entry.TweakId}");
                    info.AppendLine();
                }

                return info.ToString();
            }
        }
    }
}
