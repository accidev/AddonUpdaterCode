using AddonUpdater.Controlers;
using AddonUpdater.Models;
using AddonUpdater.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AddonUpdater
{
    class DownloadAddonGitHub
    {
        // ---- Thread-safe доступ к GitHubs ----
        private static readonly object _gitHubsLock = new();
        private static List<GitHub> _gitHubs = new();

        public static List<GitHub> GitHubs
        {
            get { lock (_gitHubsLock) { return _gitHubs; } }
            set { lock (_gitHubsLock) { _gitHubs = value ?? new List<GitHub>(); } }
        }

        public static bool UpdateInfo = true;
        public static List<WTF> WTF = new();

        // ---- Aupdatecheck (GitHub poll) ----
        private async Task Aupdatecheck()
        {
            await AddonUpdaterSetting.GetSettingsTask();
            List<GitHub> GitHubsNew = new();

            if (AddonUpdaterSetting.Setting?.Tocs != null)
            {
                foreach (Toc toc in AddonUpdaterSetting.Setting.Tocs)
                {
                    try
                    {
                        GitHubsNew.AddRange(await AupdatecheckTocAsync(toc.Link, toc.Regex));
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"AupdatecheckToc failed for {toc.Link}", ex);
                    }
                }
            }

            GitHubsNew.Sort((left, right) => string.Compare(left.Name, right.Name, StringComparison.OrdinalIgnoreCase));

            lock (_gitHubsLock)
            {
                if (_gitHubs.Count < 1)
                {
                    _gitHubs = new List<GitHub>(GitHubsNew);
                }
                else if (GitHubsNew.Count > 0)
                {
                    if (!ListCheckInternal(_gitHubs, GitHubsNew))
                    {
                        UpdateGitHubInternal(GitHubsNew);
                        UpdateInfo = true;
                    }
                }
            }

            GetWTF();
        }

        public async Task AupdatecheckAsync()
        {
            try
            {
                await Aupdatecheck();
            }
            catch (Exception ex)
            {
                Logger.LogError("AupdatecheckAsync failed", ex);
            }
        }

        private void AupdatecheckLocal()
        {
            // Работаем со снимком, чтобы не держать lock длительно
            List<GitHub> snapshot;
            lock (_gitHubsLock)
            {
                snapshot = new List<GitHub>(_gitHubs);
            }

            foreach (var addon in snapshot)
            {
                string myVersion = GetMyVersion(addon.Directory, addon.Regex);
                if (addon.MyVersion != myVersion)
                {
                    addon.MyVersion = myVersion;
                    addon.NeedUpdate = GetNeedUpdate(addon.Version, addon.MyVersion, GetAddonUpdate(addon.Name));
                    UpdateInfo = true;
                }
            }

            GetWTF();
        }

        public async Task AupdatecheckLocalAsync()
        {
            try
            {
                await Task.Run(() => AupdatecheckLocal());
            }
            catch (Exception ex)
            {
                Logger.LogError("AupdatecheckLocalAsync failed", ex);
            }
        }

        public async Task<List<GitHub>> AupdatecheckTocAsync(string link, string regex)
        {
            List<GitHub> result = new();
            string getUrlGithub = await GetContentAsync(link);
            if (string.IsNullOrWhiteSpace(getUrlGithub))
                return result;

            string[] url_addons = getUrlGithub.Split('\n');
            if (url_addons.Length == 0)
                return result;

            // Параллельная обработка через Task.Run + WhenAll
            int threadCount = Math.Min(4, Math.Max(1, url_addons.Length));
            int chunkSize = (int)Math.Ceiling(url_addons.Length / (double)threadCount);

            var tasks = new List<Task<List<GitHub>>>(threadCount);
            for (int t = 0; t < threadCount; t++)
            {
                int start = t * chunkSize;
                int end = Math.Min(start + chunkSize, url_addons.Length);
                if (start >= end) break;

                tasks.Add(Task.Run(async () => await GetInfoByAddonsAsync(start, end, url_addons, regex)));
            }

            var chunks = await Task.WhenAll(tasks);
            foreach (var chunk in chunks)
                result.AddRange(chunk);

            return result;
        }

        private async Task<List<GitHub>> GetInfoByAddonsAsync(int start, int end, string[] url_addons, string regex)
        {
            List<GitHub> result = new();
            for (int i = start; i < end; i++)
            {
                try
                {
                    string line = url_addons[i];
                    string name = GetValues(line, "NameAddon");
                    if (string.IsNullOrEmpty(name)) continue;

                    string linkVersion = GetValues(line, "Version");
                    string branches = GetValues(line, "Branches");

                    var dirMatch = Regex.Match(
                        linkVersion.Replace($"{branches}/", ""),
                        @"\/[a-zA-Z-_\d]*(\/[a-zA-Z-_\d]*.toc)");
                    string directory = dirMatch.Success ? dirMatch.Value : string.Empty;
                    if (string.IsNullOrEmpty(directory))
                    {
                        Logger.LogWarning($"Не удалось извлечь directory из {linkVersion}");
                        continue;
                    }

                    string description = GetValues(line, "Description");
                    string version = await GetVersionAsync(linkVersion, regex);
                    string? myVersion = GetMyVersion(directory, regex);
                    bool addonUpdate = GetAddonUpdate(name);
                    bool needUpdate = GetNeedUpdate(version, myVersion, addonUpdate);

                    var filesValue = GetValues(line, "Files");
                    var files = string.IsNullOrEmpty(filesValue)
                        ? new List<string>()
                        : filesValue.Split(',').ToList();

                    result.Add(new GitHub()
                    {
                        Name = name,
                        Link = linkVersion,
                        Branches = branches,
                        Files = files,
                        Version = version,
                        Directory = directory,
                        MyVersion = myVersion,
                        Description = description,
                        NeedUpdate = needUpdate,
                        GithubLink = GetValues(line, "GitHub"),
                        Forum = GetValues(line, "Forum"),
                        BugReport = GetValues(line, "BugReport"),
                        Author = GetValues(line, "Author"),
                        Regex = regex,
                        SavedVariables = GetSavedVariables(directory),
                        SavedVariablesPerCharacter = GetSavedVariablesPerCharacter(directory),
                        Category = GetValues(line, "Category")
                    });
                }
                catch (Exception ex)
                {
                    Logger.LogError($"GetInfoByAddons failed at index {i}", ex);
                }
            }
            return result;
        }

        #region Delete

        private void AddonUpdaterDeleteDirectory(string? path)
        {
            if (AddonUpdaterSetting.Setting?.DeleteDirectory == null)
                return;

            string? baseWow = AddonUpdaterSettingApp.SettingsApp.PathWow;

            foreach (string directory in AddonUpdaterSetting.Setting.DeleteDirectory)
            {
                if (path != null)
                {
                    string target = Path.Combine(path, directory);
                    if (Directory.Exists(target))
                    {
                        DirectoryDelete(target);
                        Thread.Sleep(100);
                    }
                }

                if (!string.IsNullOrEmpty(baseWow))
                {
                    string target = WoWPaths.GetAddonPath(baseWow, directory);
                    if (Directory.Exists(target))
                    {
                        DirectoryDelete(target);
                        Thread.Sleep(100);
                    }
                }
            }
        }

        public void DirectoryDelete(string path)
        {
            try
            {
                if (Directory.Exists(path))
                    new DirectoryInfo(path).Delete(true);
            }
            catch (Exception ex)
            {
                Logger.LogWarning($"DirectoryDelete failed: {path}", ex);
            }
        }

        public void DeleteAddon(GitHub gitHub)
        {
            string? baseWow = AddonUpdaterSettingApp.SettingsApp.PathWow;
            if (string.IsNullOrEmpty(baseWow)) return;

            string directoryDelete = WoWPaths.GetAddonPath(baseWow, $"AddonsOld{gitHub.Name}");

            if (Directory.Exists(directoryDelete)) DirectoryDelete(directoryDelete);
            Directory.CreateDirectory(directoryDelete);

            foreach (string file in gitHub.Files)
            {
                string sourceDirectory = WoWPaths.GetAddonPath(baseWow, file);
                string destinationDirectory = Path.Combine(directoryDelete, file);
                try
                {
                    if (Directory.Exists(sourceDirectory))
                    {
                        if (Directory.Exists(destinationDirectory))
                            Directory.Delete(destinationDirectory, true);
                        Directory.Move(sourceDirectory, destinationDirectory);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"DeleteAddon move failed: {sourceDirectory} -> {destinationDirectory}", ex);
                }
            }

            AddonUpdaterDeleteDirectory(null);
            DirectoryDelete(directoryDelete);
        }

        #endregion

        public async Task DownloadAddonGitHubTask(string name, string githublink, string branches)
        {
            string githubDirectory = githublink.Replace("https://github.com/", "");
            var uri = new Uri($"https://github.com/{githubDirectory}/archive/refs/heads/{branches}.zip");

            using var response = await HttpClientProvider.Instance.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            string zipPath = $"{name}.zip";
            if (File.Exists(zipPath))
            {
                try { File.Delete(zipPath); }
                catch (Exception ex) { Logger.LogWarning($"Cannot delete existing zip {zipPath}", ex); }
            }

            await using var fs = new FileStream(zipPath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
            await response.Content.CopyToAsync(fs);
        }

        #region Get

        private async Task<string> GetVersionAsync(string link, string regex)
        {
            string content = await GetContentAsync(link);
            if (string.IsNullOrEmpty(content)) return "0";

            Match reg = Regex.Match(content, regex);
            return reg.Success ? reg.Groups[1].Value.Trim('\r') : "0";
        }

        /// <summary>
        /// Возвращает версию установленного аддона.
        /// null — аддон не установлен (файла .toc нет). Это используется UI
        /// для разделения "Мои аддоны" (MyVersion != null) и "Все аддоны" (== null).
        /// "0" — файл есть, но версия не найдена/ошибка чтения.
        /// </summary>
        public string? GetMyVersion(string directory, string regex)
        {
            try
            {
                string? baseWow = AddonUpdaterSettingApp.SettingsApp.PathWow;
                if (string.IsNullOrEmpty(baseWow)) return null;

                string filePath = WoWPaths.GetAddonPath(baseWow, directory);
                if (!File.Exists(filePath)) return null;

                using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var reader = new StreamReader(stream);

                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    Match reg = Regex.Match(line, regex);
                    if (reg.Success)
                    {
                        string value = reg.Groups[1].Value.Trim();
                        return value.Length == 0 ? "0" : value;
                    }
                }
                return "0";
            }
            catch (Exception ex)
            {
                Logger.LogWarning($"GetMyVersion failed: {directory}", ex);
                return "0";
            }
        }

        public bool GetAddonUpdate(string name)
        {
            return AddonUpdaterSettingApp.SettingsApp.UpdateAddon?.Contains(name) == true;
        }

        public bool GetNeedUpdate(string version, string? MyVersion, bool addonUpdate)
        {
            if (!addonUpdate) return false;
            if (string.IsNullOrEmpty(MyVersion) || MyVersion == "0") return false;

            if (!TryParseVersion(MyVersion, out double my)) return false;
            if (!TryParseVersion(version, out double cur)) return false;

            return cur > my;
        }

        private static bool TryParseVersion(string version, out double result)
        {
            result = 0;
            if (string.IsNullOrWhiteSpace(version)) return false;

            // Оставляем только первую точку как десятичный разделитель,
            // остальные удаляем (например "1.2.3" -> "1.23").
            int firstDot = version.IndexOf('.');
            string normalized;
            if (firstDot >= 0)
            {
                string head = version.Substring(0, firstDot + 1);
                string tail = version.Substring(firstDot + 1).Replace(".", "");
                normalized = head + tail;
            }
            else
            {
                normalized = version;
            }

            return double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
        }

        public string GetValues(string text, string type)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(type))
                return string.Empty;

            int start = text.IndexOf($"{type}[", StringComparison.Ordinal);
            if (start == -1) return string.Empty;

            int end = text.IndexOf(']', start + 1);
            if (end == -1) return string.Empty;

            int valueStart = start + type.Length + 1;
            if (valueStart >= end) return string.Empty;

            return text.Substring(valueStart, end - valueStart);
        }

        public bool GetSavedVariables(string directory)
            => HasTocMarker(directory, "## SavedVariables:");

        public bool GetSavedVariablesPerCharacter(string directory)
            => HasTocMarker(directory, "## SavedVariablesPerCharacter:");

        private static bool HasTocMarker(string directory, string marker)
        {
            try
            {
                string? baseWow = AddonUpdaterSettingApp.SettingsApp.PathWow;
                if (string.IsNullOrEmpty(baseWow)) return false;

                string filePath = WoWPaths.GetAddonPath(baseWow, directory);
                if (!File.Exists(filePath)) return false;

                using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var reader = new StreamReader(stream);

                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains(marker, StringComparison.Ordinal))
                        return true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning($"HasTocMarker failed: {directory} / {marker}", ex);
            }
            return false;
        }

        public async Task<string> GetContentAsync(string url)
        {
            try
            {
                return await HttpClientProvider.Instance.GetStringAsync(url);
            }
            catch (Exception ex)
            {
                Logger.LogWarning($"GetContent failed: {url}", ex);
                return string.Empty;
            }
        }

        private void GetWTF()
        {
            var list = new List<WTF>();
            string? baseWow = AddonUpdaterSettingApp.SettingsApp.PathWow;
            if (!string.IsNullOrEmpty(baseWow))
            {
                string pathWTF = WoWPaths.GetWTFAccountPath(baseWow);
                if (Directory.Exists(pathWTF))
                {
                    foreach (string account in Directory.GetDirectories(pathWTF))
                    {
                        list.Add(new WTF { Account = account, Realms = GetRealm(account) });
                    }
                }
            }
            WTF = list;
        }

        List<Realms> GetRealm(string path)
        {
            List<Realms> realms = new();
            foreach (string realm in Directory.GetDirectories(path))
            {
                if (!realm.Contains("SavedVariables", StringComparison.Ordinal))
                {
                    realms.Add(new Realms
                    {
                        Name = realm,
                        Persons = Directory.GetDirectories(realm).ToList()
                    });
                }
            }
            return realms;
        }

        #endregion

        #region GetAddon

        private void GetAddon(List<GitHub> NeedUpdate)
        {
            string? baseWow = AddonUpdaterSettingApp.SettingsApp.PathWow;
            if (string.IsNullOrEmpty(baseWow)) return;

            string oldPath = WoWPaths.GetOldBackupPath(baseWow);

            try
            {
                if (Directory.Exists(oldPath))
                    DirectoryDelete(oldPath);
                Directory.CreateDirectory(oldPath);

                foreach (var addon in NeedUpdate)
                {
                    ProcessAddonUpdate(addon, baseWow, oldPath);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("GetAddon(list) failed", ex);
            }
            finally
            {
                DirectoryDelete(oldPath);
            }
        }

        private void ProcessAddonUpdate(GitHub addon, string baseWow, string oldPath)
        {
            string tempDir = addon.Name;
            string tempZip = $"{addon.Name}.zip";

            try
            {
                if (Directory.Exists(tempDir))
                    DirectoryDelete(tempDir);
                Directory.CreateDirectory(tempDir);

                ZipFile.ExtractToDirectory(tempZip, tempDir);

                string[] allfiles = Directory.GetDirectories(tempDir);
                if (allfiles.Length == 0)
                {
                    Logger.LogWarning($"Empty zip for addon {addon.Name}");
                    return;
                }

                if (allfiles[0].Contains($"-{addon.Branches}", StringComparison.Ordinal))
                {
                    string moveDest = Path.Combine(tempDir, addon.Name);
                    if (Directory.Exists(moveDest)) Directory.Delete(moveDest, true);
                    Directory.Move(allfiles[0], moveDest);
                }

                allfiles = Directory.GetDirectories(tempDir);
                if (allfiles.Length == 0) return;

                string[] allfiles3 = Directory.GetFiles(allfiles[0]);

                AddonUpdaterDeleteDirectory(allfiles[0]);

                string innerTocPath = Path.Combine(tempDir, addon.Directory.TrimStart('\\', '/'));

                if (!File.Exists(innerTocPath))
                {
                    foreach (string file in allfiles3)
                    {
                        try { File.Delete(file); }
                        catch (Exception ex) { Logger.LogWarning($"Delete file failed: {file}", ex); }
                    }
                }

                string innerZip = Path.Combine(tempDir, $"{addon.Name}.zip");
                if (File.Exists(innerTocPath))
                {
                    if (File.Exists(tempZip)) File.Delete(tempZip);
                    ZipFile.CreateFromDirectory(tempDir, tempZip);
                    if (File.Exists(innerZip)) File.Delete(innerZip);
                    File.Move(tempZip, innerZip);
                }
                else
                {
                    if (File.Exists(innerZip)) File.Delete(innerZip);
                    ZipFile.CreateFromDirectory(allfiles[0], innerZip);
                }

                foreach (string dir in addon.Files)
                {
                    string sourceDirectory = WoWPaths.GetAddonPath(baseWow, dir);
                    string destinationDirectory = Path.Combine(oldPath, dir);

                    if (Directory.Exists(sourceDirectory))
                    {
                        try
                        {
                            if (Directory.Exists(destinationDirectory))
                                Directory.Delete(destinationDirectory, true);
                            Directory.Move(sourceDirectory, destinationDirectory);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError($"Move to backup failed: {sourceDirectory}", ex);
                        }
                    }
                }

                string addonsPath = WoWPaths.GetAddonsPath(baseWow);
                if (Directory.Exists(addonsPath))
                    ZipFile.ExtractToDirectory(innerZip, addonsPath, overwriteFiles: true);

                UpdateLastUpdateRecord(addon.Name);
                AddonUpdaterSettingApp.Save();
            }
            catch (Exception ex)
            {
                Logger.LogError($"ProcessAddonUpdate failed for {addon.Name}", ex);
            }
            finally
            {
                try { if (Directory.Exists(tempDir)) DirectoryDelete(tempDir); } catch { }
                try { if (File.Exists(tempZip)) File.Delete(tempZip); } catch { }
            }
        }

        private static void UpdateLastUpdateRecord(string addonName)
        {
            var list = AddonUpdaterSettingApp.SettingsApp.LastUpdate;
            int idx = list.FindIndex(f => f.AddonName == addonName);
            string now = DateTime.Now.ToString("dd-MM-yyyy HH:mm");
            if (idx > -1)
                list[idx].LastUpdate = now;
            else
                list.Add(new LastUpdateAddon { AddonName = addonName, LastUpdate = now });
        }

        public async Task GetAddonAsync(List<GitHub> NeedUpdate)
        {
            await Task.Run(() => GetAddon(NeedUpdate));
        }

        private void GetAddon(GitHub gitHub)
        {
            string? baseWow = AddonUpdaterSettingApp.SettingsApp.PathWow;
            if (string.IsNullOrEmpty(baseWow)) return;

            string oldPath = WoWPaths.GetOldBackupPath(baseWow, gitHub.Name);

            try
            {
                if (Directory.Exists(oldPath))
                    DirectoryDelete(oldPath);
                Directory.CreateDirectory(oldPath);

                ProcessAddonUpdate(gitHub, baseWow, oldPath);
            }
            catch (Exception ex)
            {
                Logger.LogError($"GetAddon(single) failed for {gitHub.Name}", ex);
            }
            finally
            {
                DirectoryDelete(oldPath);
            }
        }

        public async Task GetAddonAsync(GitHub gitHub)
        {
            await Task.Run(() => GetAddon(gitHub));
        }

        #endregion

        public bool ListCheck(List<GitHub> G1, List<GitHub> G2) => ListCheckInternal(G1, G2);

        private static bool ListCheckInternal(List<GitHub> G1, List<GitHub> G2)
        {
            if (G1.Count != G2.Count) return false;
            for (int i = 0; i < G1.Count; i++)
            {
                if (G1[i] != G2[i]) return false;
            }
            return true;
        }

        private void UpdateGitHubInternal(List<GitHub> incoming)
        {
            // Вызывается из под _gitHubsLock
            foreach (var src in incoming)
            {
                int index = _gitHubs.FindIndex(a => a.Name == src.Name);
                if (index > -1)
                {
                    var target = _gitHubs[index];
                    target.Name = src.Name;
                    target.Link = src.Link;
                    target.Directory = src.Directory;
                    target.Version = src.Version;
                    target.MyVersion = src.MyVersion;
                    target.Branches = src.Branches;
                    target.Description = src.Description;
                    target.Author = src.Author;
                    target.GithubLink = src.GithubLink;
                    target.Forum = src.Forum;
                    target.BugReport = src.BugReport;
                    target.Regex = src.Regex;
                    target.Category = src.Category;
                    target.NeedUpdate = src.NeedUpdate;
                    target.SavedVariables = src.SavedVariables;
                    target.SavedVariablesPerCharacter = src.SavedVariablesPerCharacter;
                    target.Files = new List<string>(src.Files);
                }
                else
                {
                    _gitHubs.Add(src);
                }
            }
        }
    }
}
