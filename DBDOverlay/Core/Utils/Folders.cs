using DBDOverlay.Core.Extensions;
using System.IO;

namespace DBDOverlay.Core.Utils
{
    public static class Folders
    {
        public static readonly string TessData = "Tessdata".ToProjectPath();
        public static readonly string Update = "Update".ToProjectPath();
        public static readonly string Images = "Images".ToProjectPath();

        public static void CreateDefault()
        {
            Directory.CreateDirectory(TessData);
            Directory.CreateDirectory(Images);
        }
    }
}
