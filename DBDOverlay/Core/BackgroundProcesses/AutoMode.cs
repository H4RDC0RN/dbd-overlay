using DBDOverlay.Core.ImageProcessing;
using DBDOverlay.Core.MapOverlay;
using System;
using Application = System.Windows.Application;

namespace DBDOverlay.Core.BackgroundProcesses
{
    public class AutoMode : BaseBackgroundProcess
    {
        public event EventHandler<NewMapEventArgs> NewMapRecognized;
        private static AutoMode instance;

        public static AutoMode Instance
        {
            get
            {
                if (instance == null)
                    instance = new AutoMode();
                return instance;
            }
        }

        protected AutoMode()
        {
            NewMapRecognized += MapOverlayController.Instance.HandleNewMapRecognized;
        }

        protected override void Action()
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
}
