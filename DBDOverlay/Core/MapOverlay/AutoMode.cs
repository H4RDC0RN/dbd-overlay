using DBDOverlay.Core.ImageProcessing;
using DBDOverlay.Core.Windows;
using System;
using System.ComponentModel;
using Application = System.Windows.Application;

namespace DBDOverlay.Core.MapOverlay
{
    public class AutoMode
    {
        public bool IsActive { get; set; } = false;
        public event EventHandler<NewMapEventArgs> NewMapRecognized;
        private static AutoMode instance;
        private readonly BackgroundWorker worker = new BackgroundWorker();

        public static AutoMode Instance
        {
            get
            {
                if (instance == null)
                    instance = new AutoMode();
                return instance;
            }
        }

        public void Run()
        {
            IsActive = true;
            NewMapRecognized += MapOverlayController.Instance.HandleNewMapRecognized;
            worker.WorkerSupportsCancellation = true;
            worker.WorkerReportsProgress = true;
            worker.DoWork += (s, e) =>
            {
                while (IsActive)
                {
                    if (WindowsServices.Instance.IsDBDActiveWindow())
                    {
                        var mapInfo = ScreenshotRecognizer.GetMapInfo(true);
                        if (MapOverlayController.Instance.CanMapOverlayBeApplied(mapInfo))
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

        public void Stop()
        {
            IsActive = false;
            worker.CancelAsync();
            worker.Dispose();
        }
    }
}
