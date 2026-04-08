using AddonUpdater.Models;
using AddonUpdater.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace AddonUpdater.Controlers
{
    class BackupWTF
    {
        private const string BackupDir = "BackupWTF";

        public static Task CreateFileBackupTask()
            => Task.Run(() => CreateFileBackup());

        public static Task DeleteFileBackupTask(DateTime max)
            => Task.Run(() => DeleteFileBackup(max));

        public static Task DeleteFileBackupTask(int n)
            => Task.Run(() => DeleteFileBackup(n));

        public static Task BackupWtfTask()
            => Task.Run(() => BackupWtf());

        private static void CreateFileBackup()
        {
            try
            {
                string? baseWow = AddonUpdaterSettingApp.SettingsApp.PathWow;
                if (string.IsNullOrEmpty(baseWow)) return;

                string pathWTF = WoWPaths.GetWTFPath(baseWow);
                if (!Directory.Exists(pathWTF)) return;

                Directory.CreateDirectory(BackupDir);
                string pathBackupWTF = Path.Combine(
                    BackupDir,
                    "WTF " + DateTime.Now.ToString("dd MM yyyy HH mm ss") + ".zip");

                ZipFile.CreateFromDirectory(pathWTF, pathBackupWTF);
            }
            catch (Exception ex)
            {
                Logger.LogError("CreateFileBackup failed", ex);
            }
        }

        private static void DeleteFileBackup(DateTime max)
        {
            if (!Directory.Exists(BackupDir)) return;
            try
            {
                foreach (string file in Directory.GetFiles(BackupDir))
                {
                    if (File.GetCreationTime(file) < max)
                        File.Delete(file);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("DeleteFileBackup(DateTime) failed", ex);
            }
        }

        private static void DeleteFileBackup(int keepLast)
        {
            if (!Directory.Exists(BackupDir)) return;
            try
            {
                var files = Directory.GetFiles(BackupDir)
                    .Select(f => new FileCustom { Path = f, DateTimeCreate = File.GetCreationTime(f) })
                    .OrderBy(f => f.DateTimeCreate)
                    .ToList();

                int toDelete = files.Count - keepLast;
                for (int i = 0; i < toDelete; i++)
                {
                    File.Delete(files[i].Path);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("DeleteFileBackup(int) failed", ex);
            }
        }

        private static void BackupWtf()
        {
            if (!Directory.Exists(BackupDir)) return;
            try
            {
                string latestPath = string.Empty;
                DateTime max = DateTime.MinValue;
                foreach (string file in Directory.GetFiles(BackupDir))
                {
                    DateTime dt = File.GetCreationTime(file);
                    if (dt > max)
                    {
                        max = dt;
                        latestPath = file;
                    }
                }

                if (string.IsNullOrEmpty(latestPath)) return;

                string? baseWow = AddonUpdaterSettingApp.SettingsApp.PathWow;
                if (string.IsNullOrEmpty(baseWow)) return;

                string pathToWtf = WoWPaths.GetWTFPath(baseWow);
                if (Directory.Exists(pathToWtf))
                    Directory.Delete(pathToWtf, true);

                ZipFile.ExtractToDirectory(latestPath, pathToWtf);
                System.Windows.Forms.MessageBox.Show(latestPath + " восстановлен");
            }
            catch (Exception ex)
            {
                Logger.LogError("BackupWtf failed", ex);
            }
        }

        public static DateTime GetLastBackupWtf()
        {
            DateTime max = DateTime.MinValue;
            try
            {
                if (!Directory.Exists(BackupDir)) return new DateTime();

                foreach (string file in Directory.GetFiles(BackupDir))
                {
                    DateTime dt = File.GetCreationTime(file);
                    if (dt > max) max = dt;
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning("GetLastBackupWtf failed", ex);
            }
            return max == DateTime.MinValue ? new DateTime() : max;
        }
    }
}
