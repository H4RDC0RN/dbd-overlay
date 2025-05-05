using DBDOverlay.Core.Extensions;
using DBDOverlay.Properties;
using System.IO;

namespace DBDOverlay.Core.Utils
{
    public static class FileSystem
    {        
        public static readonly string TessData = "Tessdata".ToProjectPath();
        public static readonly string Update = "Update".ToProjectPath();
        public static readonly string Images = "Images".ToProjectPath();
        private static readonly string png = "png";

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
    }
}
