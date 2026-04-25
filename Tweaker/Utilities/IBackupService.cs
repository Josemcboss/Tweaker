using System.Collections.Generic;

namespace Tweaker.Utilities
{
    public interface IBackupService
    {
        bool CreateBackup(string? name = null);
        bool RestoreBackup(string backupName);
        List<BackupInfo> GetAvailableBackups();
        bool DeleteBackup(string backupName);
        bool ExportBackup(string backupName, string exportPath);
        bool ImportBackup(string importPath);
    }
}
