using System;
using System.IO;

namespace AddonUpdater.Utils
{
    /// <summary>
    /// Простой потокобезопасный файловый логгер.
    /// Пишет в AddonUpdater.log рядом с исполняемым файлом.
    /// </summary>
    public static class Logger
    {
        private static readonly string LogPath =
            Path.Combine(AppContext.BaseDirectory, "AddonUpdater.log");

        private const long MaxLogSize = 5 * 1024 * 1024; // 5 MB
        private static readonly object _lock = new();

        public static void LogError(string message, Exception? ex = null)
        {
            Write("ERROR", message, ex);
        }

        public static void LogWarning(string message, Exception? ex = null)
        {
            Write("WARN", message, ex);
        }

        public static void LogInfo(string message)
        {
            Write("INFO", message, null);
        }

        private static void Write(string level, string message, Exception? ex)
        {
            try
            {
                lock (_lock)
                {
                    TruncateIfNeeded();

                    string line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {level}: {message}";
                    if (ex != null)
                        line += Environment.NewLine + ex;

                    File.AppendAllText(LogPath, line + Environment.NewLine);
                }
            }
            catch
            {
                // Логгер не должен ронять приложение
            }
        }

        private static void TruncateIfNeeded()
        {
            try
            {
                var info = new FileInfo(LogPath);
                if (info.Exists && info.Length > MaxLogSize)
                {
                    File.Delete(LogPath);
                }
            }
            catch
            {
                // ignore
            }
        }
    }
}
