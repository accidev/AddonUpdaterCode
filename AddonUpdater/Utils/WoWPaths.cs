using System.IO;

namespace AddonUpdater.Utils
{
    /// <summary>
    /// Централизованное построение путей внутри каталога WoW.
    /// Использует Path.Combine и нормализует слэши у пользовательских значений.
    /// </summary>
    public static class WoWPaths
    {
        public static string GetInterfacePath(string baseWowPath)
            => Path.Combine(baseWowPath, "Interface");

        public static string GetAddonsPath(string baseWowPath)
            => Path.Combine(baseWowPath, "Interface", "AddOns");

        public static string GetWTFPath(string baseWowPath)
            => Path.Combine(baseWowPath, "WTF");

        public static string GetWTFAccountPath(string baseWowPath)
            => Path.Combine(baseWowPath, "WTF", "Account");

        /// <summary>
        /// Путь к конкретному файлу/каталогу внутри AddOns.
        /// Поддерживает relativePath с ведущими / или \ (нормализует).
        /// </summary>
        public static string GetAddonPath(string baseWowPath, string relativePath)
        {
            string normalized = (relativePath ?? string.Empty)
                .Replace('/', Path.DirectorySeparatorChar)
                .TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            return Path.Combine(GetAddonsPath(baseWowPath), normalized);
        }

        public static string GetOldBackupPath(string baseWowPath)
            => Path.Combine(GetAddonsPath(baseWowPath), "old");

        public static string GetOldBackupPath(string baseWowPath, string addonName)
            => Path.Combine(GetAddonsPath(baseWowPath), $"old{addonName}");
    }
}
