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
            if (!File.Exists(filePath))
                File.Create(filePath).Dispose();
        }

        public static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        public static List<string> GetIniFiles(string path)
        {
            return Directory.EnumerateFiles(path, $"*.{ini}", SearchOption.TopDirectoryOnly)
                .Select(Path.GetFileName)
                .Where(f => !string.Equals(f, $"{ReShade}.{ini}") && !string.Equals(f, $"{Settings.Default.MainFilterName}.{ini}")).ToList();
        }

        public static void CopyIniFile(string from, string to)
        {
            File.Copy($"{from}.{ini}", $"{to}.{ini}", true);
            File.AppendAllText($"{to}.{ini}", "\nupdated=" + DateTime.UtcNow.Ticks);
        }
    }
}
