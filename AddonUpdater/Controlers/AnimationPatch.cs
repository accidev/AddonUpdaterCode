using AddonUpdater.Models;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace AddonUpdater.Controlers
{
    public class PatchInfo
    {
        public int Build { get; set; }
        public string Hash { get; set; }
    }

    class AnimationPatch
    {
        private const string PatchUrl = "https://d1st4r.ru/patch/patch-ruRU-[.mpq";
        private const string PatchFileName = "patch-ruRU-[.mpq";
        private const string PatchInfoUrl = "https://api.d1st4r.ru/patchfx";
        private static PatchInfo _currentPatchInfo;
        private static DateTime _lastCheckTime = DateTime.MinValue;

        public static Task InstallPatchTask()
        {
            return Task.Run(async () => await InstallPatchAsync());
        }

        public static bool IsPatchInstalled()
        {
            string patchPath = GetPatchPath();
            return File.Exists(patchPath);
        }

        public static DateTime GetPatchInstallationDate()
        {
            string patchPath = GetPatchPath();
            if (File.Exists(patchPath))
            {
                return File.GetCreationTime(patchPath);
            }
            return DateTime.MinValue;
        }

        public static async Task<PatchInfo> GetCurrentPatchInfoAsync()
        {
            if (_currentPatchInfo != null && (DateTime.Now - _lastCheckTime).TotalMinutes < 15)
            {
                return _currentPatchInfo;
            }

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string json = await client.GetStringAsync(PatchInfoUrl);
                    _currentPatchInfo = JsonConvert.DeserializeObject<PatchInfo>(json);
                    _lastCheckTime = DateTime.Now;
                    return _currentPatchInfo;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка API: {ex.Message}");
                return null;
            }
        }

        public static async Task<bool> IsPatchUpToDateAsync()
        {
            if (!IsPatchInstalled())
            {
                return false;
            }

            try
            {
                PatchInfo currentInfo = await GetCurrentPatchInfoAsync();
                if (currentInfo == null)
                {
                    return false;
                }

                string patchPath = GetPatchPath();

                string fileHash = CalculateFileMD5(patchPath);
                if (string.IsNullOrEmpty(fileHash))
                {
                    return false;
                }
                //сравниваем хеш
                return string.Equals(fileHash, currentInfo.Hash, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private static string CalculateFileMD5(string filename)
        {
            try
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(filename))
                    {
                        byte[] hash = md5.ComputeHash(stream);
                        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                return null;
            }
        }

        private static string GetPatchPath()
        {
            return Path.Combine(AddonUpdaterSettingApp.SettingsApp.PathWow, "Data", "ruRU", PatchFileName);
        }

        private static async Task InstallPatchAsync()
        {
            try
            {
                string patchPath = GetPatchPath();
                string patchDirectory = Path.GetDirectoryName(patchPath);

                if (!Directory.Exists(patchDirectory))
                {
                    Directory.CreateDirectory(patchDirectory);
                }

                if (File.Exists(patchPath))
                {
                    File.Delete(patchPath);
                }

                using (HttpClient client = new HttpClient())
                {
                    byte[] fileBytes = await client.GetByteArrayAsync(PatchUrl);
                    await File.WriteAllBytesAsync(patchPath, fileBytes);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при установке патча: {ex.Message}", "Ошибка");
            }
        }
    }
}