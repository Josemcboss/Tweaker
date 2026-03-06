using System;

namespace Tweaker.Models
{
    public enum StartupSource
    {
        RegistryUser,
        RegistryMachine,
        StartupFolder
    }

    public class StartupItem
    {
        public string? Name { get; set; }
        public string? Command { get; set; }
        public string? FilePath { get; set; }
        public StartupSource Source { get; set; }
        public bool IsEnabled { get; set; } = true;
        public string? IconPath { get; set; }
    }
}
