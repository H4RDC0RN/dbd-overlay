using DBDOverlay.Core.Extensions;
using DBDOverlay.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DBDOverlay.Core.Utils
{
    public static class FileSystem
    {
        public static readonly string TessData = "Tessdata".ToProjectPath();
        public static readonly string Update = "Update".ToProjectPath();
        public static readonly string Images = "Images".ToProjectPath();
        private static readonly string png = "png";
        private static readonly string ini = "ini";
        private static readonly string ReShade = "ReShade";

        public static void CreateDefaultFolders()
        {
            Directory.CreateDirectory(TessData);
            Directory.CreateDirectory(Images);
        }

        public static string GetImagePath(string imageName, bool edited = false)
        {
            var path = $@"{Images}\{imageName}.{png}";
            return edited ? path.ReplaceRegex($@"\.{png}", $"_edited.{png}") : path;
        }

        public static string GetImagePath(bool autoMode = false, bool edited = false)
        {
            return GetImagePath(autoMode ? Settings.Default.AutoScreenshotFileName : Settings.Default.ManualScreenshotFileName, edited);
        }

        public static void CreateFile(string filePath)
        {
            if (!File.Exists(filePath)) File.Create(filePath).Dispose();
        }

        public static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath)) File.Delete(filePath);
        }

        public static List<string> GetIniFiles(string path)
        {
            return Directory.EnumerateFiles(path, $"*.{ini}", SearchOption.TopDirectoryOnly)
                .Select(Path.GetFileNameWithoutExtension)
                .Where(f => !string.Equals(f, $"{ReShade}") && !string.Equals(f, $"{Settings.Default.MainFilterName}")).ToList();
        }

        public static void CopyIniFile(string from, string to)
        {
            File.Copy($"{from}.{ini}", $"{to}.{ini}", true);
            File.AppendAllText($"{to}.{ini}", "\nupdated=" + DateTime.UtcNow.Ticks);
        }

        public static bool IniExists(string path)
        {
            return File.Exists($"{path}.{ini}");
        }

        public static bool IsValidFileName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            if (name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                return false;

            if (name.EndsWith(" ") || name.EndsWith("."))
                return false;

            var baseName = Path.GetFileNameWithoutExtension(name)
                               .TrimEnd('.', ' ')
                               .ToUpperInvariant();

            string[] reserved =
            {
                "CON","PRN","AUX","NUL",
                "COM1","COM2","COM3","COM4","COM5","COM6","COM7","COM8","COM9",
                "LPT1","LPT2","LPT3","LPT4","LPT5","LPT6","LPT7","LPT8","LPT9"
            };

            if (reserved.Contains(baseName))
                return false;

            return true;
        }
    }
}
