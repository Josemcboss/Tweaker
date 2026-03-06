using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using Microsoft.Win32;

using Tweaker.Models;

namespace Tweaker.Services
{
    public class StartupManagerService
    {
        private const string RUN_KEY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        public List<StartupItem> GetStartupItems()
        {
            var items = new List<StartupItem>();

            // 1. HKCU Run
            GetRegistryItems(Registry.CurrentUser, RUN_KEY, StartupSource.RegistryUser, items);

            // 2. HKLM Run
            GetRegistryItems(Registry.LocalMachine, RUN_KEY, StartupSource.RegistryMachine, items);

            // 3. Startup Folder (User)
            string userStartup = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup));
            GetFolderItems(userStartup, StartupSource.StartupFolder, items);

            return items;
        }

        private void GetRegistryItems(RegistryKey root, string keyPath, StartupSource source, List<StartupItem> items)
        {
            try
            {
                using (var key = root.OpenSubKey(keyPath))
                {
                    if (key == null) return;

                    foreach (var valueName in key.GetValueNames())
                    {
                        var command = key.GetValue(valueName)?.ToString() ?? "";
                        items.Add(new StartupItem
                        {
                            Name = valueName,
                            Command = command,
                            Source = source,
                            FilePath = ExtractPath(command)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error reading registry startup items: {ex.Message}");
            }
        }

        private void GetFolderItems(string folderPath, StartupSource source, List<StartupItem> items)
        {
            try
            {
                if (!Directory.Exists(folderPath)) return;

                foreach (var file in Directory.GetFiles(folderPath))
                {
                    items.Add(new StartupItem
                    {
                        Name = Path.GetFileNameWithoutExtension(file),
                        Command = file,
                        Source = source,
                        FilePath = file
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error reading folder startup items: {ex.Message}");
            }
        }

        private string ExtractPath(string command)
        {
            if (string.IsNullOrEmpty(command)) return "";

            command = command.Trim();
            if (command.StartsWith("\""))
            {
                int nextQuote = command.IndexOf("\"", 1);
                if (nextQuote > 0) return command.Substring(1, nextQuote - 1);
            }

            int firstSpace = command.IndexOf(" ");
            if (firstSpace > 0) return command.Substring(0, firstSpace);

            return command;
        }

        public bool DisableItem(StartupItem item)
        {
            try
            {
                switch (item.Source)
                {
                    case StartupSource.RegistryUser:
                        return RemoveFromRegistry(Registry.CurrentUser, RUN_KEY, item.Name);

                    case StartupSource.RegistryMachine:
                        return RemoveFromRegistry(Registry.LocalMachine, RUN_KEY, item.Name);

                    case StartupSource.StartupFolder:
                        if (File.Exists(item.FilePath))
                        {
                            File.Delete(item.FilePath);
                            return true;
                        }
                        return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error disabling startup item: {ex.Message}");
            }
            return false;
        }

        private bool RemoveFromRegistry(RegistryKey root, string keyPath, string name)
        {
            using (var key = root.OpenSubKey(keyPath, true))
            {
                if (key != null && key.GetValue(name) != null)
                {
                    key.DeleteValue(name);
                    return true;
                }
            }
            return false;
        }
    }
}
