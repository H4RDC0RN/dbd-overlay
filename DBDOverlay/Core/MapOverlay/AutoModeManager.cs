using DBDOverlay.Core.ImageProcessing;
using DBDOverlay.Core.Windows;
using System;
using System.ComponentModel;
using Application = System.Windows.Application;

namespace DBDOverlay.Core.MapOverlay
{
    public class AutoModeManager
    {
        public bool IsAutoMode { get; set; } = false;
        public event EventHandler<NewMapEventArgs> NewMapRecognized;
        private static AutoModeManager instance;
        private readonly BackgroundWorker worker = new BackgroundWorker();

        public static AutoModeManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new AutoModeManager();
                return instance;
            }
        }

        public void RunAutoMode()
        {
            IsAutoMode = true;
            worker.WorkerSupportsCancellation = true;
            worker.WorkerReportsProgress = true;
            worker.DoWork += (s, e) =>
            {
                while (IsAutoMode)
                {
                    if (WindowsServices.Instance.IsDBDActiveWindow())
                    {
                        var mapInfo = ScreenshotRecognizer.GetMapInfo(true);
                        if (MapOverlayController.CanMapOverlayBeApplied(mapInfo))
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                NewMapRecognized?.Invoke(this, new NewMapEventArgs(mapInfo));
                            });
                        }
                    }
                }
            };
            worker.RunWorkerAsync();
        }

        public void StopAutoMode()
        {
            IsAutoMode = false;
            worker.CancelAsync();
            worker.Dispose();
        }
    }
}
