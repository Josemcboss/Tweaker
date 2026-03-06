using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace Tweaker.Utilities
{
    internal enum TransactionalRegistryOperation
    {
        SetValue,
        DeleteValue
    }

    /// <summary>
    /// Represents a single change made to the Windows Registry within a transaction.
    /// This class is intended for internal use by the RegistryTransaction class.
    /// </summary>
    internal class TransactionalRegistryChange
    {
        public string KeyPath { get; }
        public string ValueName { get; }
        public object OriginalValue { get; }
        public RegistryValueKind OriginalValueKind { get; }
        public bool OriginalValueExisted { get; }
        public TransactionalRegistryOperation Operation { get; }

        public TransactionalRegistryChange(string keyPath, string valueName, object originalValue, RegistryValueKind originalValueKind, bool originalValueExisted, TransactionalRegistryOperation operation)
        {
            KeyPath = keyPath;
            ValueName = valueName;
            OriginalValue = originalValue;
            OriginalValueKind = originalValueKind;
            OriginalValueExisted = originalValueExisted;
            Operation = operation;
        }
    }

    /// <summary>
    /// Provides a transactional mechanism for making changes to the Windows Registry.
    /// Changes are tracked and can be committed at once, or rolled back if an error occurs.
    /// This class implements IDisposable to ensure that uncommitted transactions are automatically rolled back.
    /// </summary>
    public class RegistryTransaction : IDisposable
    {
        private readonly List<TransactionalRegistryChange> _changes = new List<TransactionalRegistryChange>();
        private bool _committed = false;
        private bool _disposed = false;

        public void Dispose()
        {
            if (!_disposed)
            {
                if (!_committed)
                {
                    Rollback();
                }
                _disposed = true;
            }
        }

        public void Commit()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(RegistryTransaction));
            }
            _committed = true;
        }

        public void Rollback()
        {
            if (_disposed)
            {
                return;
            }

            for (int i = _changes.Count - 1; i >= 0; i--)
            {
                var change = _changes[i];
                try
                {
                    using (var rootKey = GetRootKey(change.KeyPath, out string subKeyPath, writable: true))
                    {
                        if (rootKey == null) continue;

                        using (var key = rootKey.OpenSubKey(subKeyPath, true))
                        {
                            if (key == null) continue;

                            if (change.Operation == TransactionalRegistryOperation.SetValue)
                            {
                                if (change.OriginalValueExisted)
                                {
                                    key.SetValue(change.ValueName, change.OriginalValue, change.OriginalValueKind);
                                }
                                else
                                {
                                    key.DeleteValue(change.ValueName, false);
                                }
                            }
                            else if (change.Operation == TransactionalRegistryOperation.DeleteValue)
                            {
                                if (change.OriginalValueExisted)
                                {
                                    key.SetValue(change.ValueName, change.OriginalValue, change.OriginalValueKind);
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    // Log error if needed, but continue rollback
                }
            }
            _changes.Clear();
        }

        public void SetValue(string keyPath, string valueName, object value, RegistryValueKind valueKind)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(RegistryTransaction));

            object originalValue = null;
            RegistryValueKind originalValueKind = RegistryValueKind.None;
            bool originalValueExisted = false;

            using (var rootKey = GetRootKey(keyPath, out string subKeyPath, writable: false))
            {
                if (rootKey != null)
                {
                    using (var key = rootKey.OpenSubKey(subKeyPath))
                    {
                        if (key != null)
                        {
                            originalValue = key.GetValue(valueName);
                            if (originalValue != null)
                            {
                                originalValueKind = key.GetValueKind(valueName);
                                originalValueExisted = true;
                            }
                        }
                    }
                }
            }

            _changes.Add(new TransactionalRegistryChange(keyPath, valueName, originalValue, originalValueKind, originalValueExisted, TransactionalRegistryOperation.SetValue));

            using (var rootKey = GetRootKey(keyPath, out string subKeyPath, writable: true))
            {
                if (rootKey != null)
                {
                    using (var key = rootKey.CreateSubKey(subKeyPath))
                    {
                        key?.SetValue(valueName, value, valueKind);
                    }
                }
            }
        }

        public void DeleteValue(string keyPath, string valueName)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(RegistryTransaction));

            object originalValue = null;
            RegistryValueKind originalValueKind = RegistryValueKind.None;
            bool originalValueExisted = false;

            using (var rootKey = GetRootKey(keyPath, out string subKeyPath, writable: false))
            {
                if (rootKey != null)
                {
                    using (var key = rootKey.OpenSubKey(subKeyPath))
                    {
                        if (key != null)
                        {
                            originalValue = key.GetValue(valueName);
                            if (originalValue != null)
                            {
                                originalValueKind = key.GetValueKind(valueName);
                                originalValueExisted = true;
                            }
                        }
                    }
                }
            }

            _changes.Add(new TransactionalRegistryChange(keyPath, valueName, originalValue, originalValueKind, originalValueExisted, TransactionalRegistryOperation.DeleteValue));

            using (var rootKey = GetRootKey(keyPath, out string subKeyPath, writable: true))
            {
                if (rootKey != null)
                {
                    using (var key = rootKey.OpenSubKey(subKeyPath, true))
                    {
                        try
                        {
                            key?.DeleteValue(valueName, false);
                        }
                        catch (ArgumentException)
                        {
                            // Ignore if the value doesn't exist.
                        }
                    }
                }
            }
        }

        private RegistryKey GetRootKey(string fullPath, out string subKeyPath, bool writable)
        {
            subKeyPath = string.Empty;
            if (string.IsNullOrEmpty(fullPath)) return null;

            string[] parts = fullPath.Split(new[] { '\\' }, 2);
            string rootName = parts[0].ToUpper();
            subKeyPath = parts.Length > 1 ? parts[1] : string.Empty;

            RegistryKey rootKey;
            switch (rootName)
            {
                case "HKEY_CLASSES_ROOT":
                    rootKey = Registry.ClassesRoot;
                    break;
                case "HKEY_CURRENT_USER":
                    rootKey = Registry.CurrentUser;
                    break;
                case "HKEY_LOCAL_MACHINE":
                    rootKey = Registry.LocalMachine;
                    break;
                case "HKEY_USERS":
                    rootKey = Registry.Users;
                    break;
                case "HKEY_CURRENT_CONFIG":
                    rootKey = Registry.CurrentConfig;
                    break;
                default:
                    return null;
            }

            return rootKey;
        }
    }
}
