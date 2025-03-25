using DBDOverlay.Core.Utils;

namespace DBDOverlay.Core.Download
{
    public class DownloadEventArgs
    {
        public bool IsDownloading { get; }
        public string Language { get; }

        internal DownloadEventArgs(bool isDownloading, string language)
        {
            IsDownloading = isDownloading;
            Language = language;
        }

        public void Log()
        {
            var state = IsDownloading ? "Starts" : "Finish";
            Logger.Log.Info($"'{state}' downloading '{Language}' language");
        }
    }
}
