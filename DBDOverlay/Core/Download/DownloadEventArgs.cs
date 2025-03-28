namespace DBDOverlay.Core.Download
{
    public class DownloadEventArgs
    {
        public bool IsDownloading { get; }

        internal DownloadEventArgs(bool isDownloading)
        {
            IsDownloading = isDownloading;
        }
    }
}
