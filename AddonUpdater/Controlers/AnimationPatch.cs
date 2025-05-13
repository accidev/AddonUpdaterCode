using AddonUpdater.Models;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AddonUpdater.Controlers
{
    class AnimationPatch
    {
        private const string PatchUrl = "https://d1st4r.ru/patch/patch-ruRU-[.mpq";
        private const string PatchFileName = "patch-ruRU-[.mpq";

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

                MessageBox.Show("Патч анимаций успешно установлен", "Успех");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при установке патча: {ex.Message}", "Ошибка");
            }
        }
    }
} 