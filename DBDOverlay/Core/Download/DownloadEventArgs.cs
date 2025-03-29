using System;

namespace DBDOverlay.Core.Download
{
    public class DownloadEventArgs : EventArgs
    {
        public bool IsDownloading { get; }

        internal DownloadEventArgs(bool isDownloading)
        {
            IsDownloading = isDownloading;
        }
    }
}
