using Microsoft.Win32;
using System;
using System.Diagnostics;

namespace Tweaker.Utilities
{
    /// <summary>
    /// RegistryHelper - Centraliza operaciones de registro para eliminar redundancia
    /// Proporciona métodos genéricos para operaciones comunes del registro
    /// </summary>
    public static class RegistryHelper
    {
        /// <summary>
        /// Establece un valor DWORD en el registro con manejo de errores completo
        /// </summary>
        /// <param name="hive">Hive del registro (LocalMachine o CurrentUser)</param>
        /// <param name="keyPath">Ruta de la clave</param>
        /// <param name="valueName">Nombre del valor</param>
        /// <param name="value">Valor a establecer</param>
        /// <param name="operationName">Nombre de la operación para logging</param>
        /// <returns>True si fue exitoso</returns>
        public static bool SetRegistryValue(RegistryKey hive, string keyPath, string valueName, int value, string operationName = "Registry Operation")
        {
            try
            {
                using (RegistryKey key = hive.CreateSubKey(keyPath))
                {
                    if (key != null)
                    {
                        key.SetValue(valueName, value, RegistryValueKind.DWord);
                        Debug.WriteLine($"? {operationName}: {valueName} = {value}");
                        return true;
                    }
                    else
                    {
                        Debug.WriteLine($"? {operationName}: No se pudo crear/abrir la clave {keyPath}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error en {operationName}: {ex.Message}");
                Debug.WriteLine($"   Clave: {keyPath}, Valor: {valueName}");
                return false;
            }
        }

        /// <summary>
        /// Establece un valor String en el registro
        /// </summary>
        public static bool SetRegistryValue(RegistryKey hive, string keyPath, string valueName, string value, string operationName = "Registry Operation")
        {
            try
            {
                using (RegistryKey key = hive.CreateSubKey(keyPath))
                {
                    if (key != null)
                    {
                        key.SetValue(valueName, value, RegistryValueKind.String);
                        Debug.WriteLine($"? {operationName}: {valueName} = {value}");
                        return true;
                    }
                    else
                    {
                        Debug.WriteLine($"? {operationName}: No se pudo crear/abrir la clave {keyPath}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error en {operationName}: {ex.Message}");
                Debug.WriteLine($"   Clave: {keyPath}, Valor: {valueName}");
                return false;
            }
        }

        /// <summary>
        /// Lee un valor del registro de forma segura
        /// </summary>
        public static T GetRegistryValue<T>(RegistryKey hive, string keyPath, string valueName, T defaultValue = default(T))
        {
            try
            {
                using (RegistryKey key = hive.OpenSubKey(keyPath, false))
                {
                    if (key != null)
                    {
                        object value = key.GetValue(valueName);
                        if (value != null)
                        {
                            return (T)Convert.ChangeType(value, typeof(T));
                        }
                    }
                }
                return defaultValue;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error leyendo registro: {ex.Message}");
                Debug.WriteLine($"   Clave: {keyPath}, Valor: {valueName}");
                return defaultValue;
            }
        }

        /// <summary>
        /// Verifica si un valor del registro tiene un valor específico
        /// </summary>
        public static bool IsRegistryValueEqual(RegistryKey hive, string keyPath, string valueName, object expectedValue)
        {
            try
            {
                using (RegistryKey key = hive.OpenSubKey(keyPath, false))
                {
                    if (key != null)
                    {
                        object value = key.GetValue(valueName);
                        return value != null && value.ToString() == expectedValue.ToString();
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
        /// Obtiene un valor DWORD del registro desde una ruta completa
        /// </summary>
        /// <param name="fullPath">Ruta completa del registro (ej: HKEY_LOCAL_MACHINE\SOFTWARE\...)</param>
        /// <param name="valueName">Nombre del valor</param>
        /// <returns>Valor DWORD o null si no existe</returns>
        public static uint? GetDwordValue(string fullPath, string valueName)
        {
            try
            {
                // Determinar el hive base
                RegistryKey hive;
                string keyPath;

                if (fullPath.StartsWith("HKEY_LOCAL_MACHINE\\", StringComparison.OrdinalIgnoreCase))
                {
                    hive = Registry.LocalMachine;
                    keyPath = fullPath.Substring("HKEY_LOCAL_MACHINE\\".Length);
                }
                else if (fullPath.StartsWith("HKEY_CURRENT_USER\\", StringComparison.OrdinalIgnoreCase))
                {
                    hive = Registry.CurrentUser;
                    keyPath = fullPath.Substring("HKEY_CURRENT_USER\\".Length);
                }
                else
                {
                    Debug.WriteLine($"?? Hive no soportado en path: {fullPath}");
                    return null;
                }

                using (RegistryKey key = hive.OpenSubKey(keyPath, false))
                {
                    if (key != null)
                    {
                        object value = key.GetValue(valueName);
                        if (value is int intValue)
                            return (uint)intValue;
                        else if (value is uint uintValue)
                            return uintValue;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"?? Error leyendo {valueName} de {fullPath}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Elimina un valor del registro de forma segura
        /// </summary>
        public static bool DeleteRegistryValue(RegistryKey hive, string keyPath, string valueName, string operationName = "Delete Registry Value")
        {
            try
            {
                using (RegistryKey key = hive.OpenSubKey(keyPath, true))
                {
                    if (key != null)
                    {
                        key.DeleteValue(valueName, false);
                        Debug.WriteLine($"? {operationName}: Eliminado {valueName}");
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"? Error en {operationName}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Ejecuta múltiples operaciones de registro en una transacción
        /// </summary>
        public static bool ExecuteRegistryTransaction(string operationName, params RegistryOperation[] operations)
        {
            Debug.WriteLine($"?? Iniciando {operationName}...");
            bool allSuccess = true;
            int successCount = 0;

            foreach (var operation in operations)
            {
                bool success = false;
                
                switch (operation.Type)
                {
                    case RegistryOperationType.SetDWord:
                        success = SetRegistryValue(operation.Hive, operation.KeyPath, operation.ValueName, 
                            Convert.ToInt32(operation.Value), operation.Description);
                        break;
                    case RegistryOperationType.SetString:
                        success = SetRegistryValue(operation.Hive, operation.KeyPath, operation.ValueName, 
                            operation.Value.ToString(), operation.Description);
                        break;
                    case RegistryOperationType.Delete:
                        success = DeleteRegistryValue(operation.Hive, operation.KeyPath, operation.ValueName, operation.Description);
                        break;
                }

                if (success)
                {
                    successCount++;
                }
                else
                {
                    allSuccess = false;
                }
            }

            if (allSuccess)
            {
                Debug.WriteLine($"? {operationName} COMPLETADO - {successCount}/{operations.Length} operaciones exitosas");
            }
            else
            {
                Debug.WriteLine($"?? {operationName} PARCIALMENTE COMPLETADO - {successCount}/{operations.Length} operaciones exitosas");
            }

            return allSuccess;
        }
    }

    /// <summary>
    /// Representa una operación de registro
    /// </summary>
    public class RegistryOperation
    {
        public RegistryOperationType Type { get; set; }
        public RegistryKey Hive { get; set; }
        public string KeyPath { get; set; }
        public string ValueName { get; set; }
        public object Value { get; set; }
        public string Description { get; set; }

        public static RegistryOperation SetDWord(RegistryKey hive, string keyPath, string valueName, int value, string description = "")
        {
            return new RegistryOperation
            {
                Type = RegistryOperationType.SetDWord,
                Hive = hive,
                KeyPath = keyPath,
                ValueName = valueName,
                Value = value,
                Description = description
            };
        }

        public static RegistryOperation SetString(RegistryKey hive, string keyPath, string valueName, string value, string description = "")
        {
            return new RegistryOperation
            {
                Type = RegistryOperationType.SetString,
                Hive = hive,
                KeyPath = keyPath,
                ValueName = valueName,
                Value = value,
                Description = description
            };
        }

        public static RegistryOperation Delete(RegistryKey hive, string keyPath, string valueName, string description = "")
        {
            return new RegistryOperation
            {
                Type = RegistryOperationType.Delete,
                Hive = hive,
                KeyPath = keyPath,
                ValueName = valueName,
                Description = description
            };
        }
    }

    public enum RegistryOperationType
    {
        SetDWord,
        SetString,
        Delete
    }
}