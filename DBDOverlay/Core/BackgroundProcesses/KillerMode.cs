using DBDOverlay.Core.ImageProcessing;
using DBDOverlay.Core.MapOverlay;
using System;

namespace DBDOverlay.Core.BackgroundProcesses
{
    public class KillerMode : BaseBackgroundProcess
    {
        public bool IsHookMode { get; set; } = false;
        public bool IsPostUnhookTimerMode { get; set; } = false;
        public event EventHandler<NewMapEventArgs> NewMapRecognized;
        private static KillerMode instance;

        public static KillerMode Instance
        {
            get
            {
                if (instance == null)
                    instance = new KillerMode();
                return instance;
            }
        }

        protected KillerMode()
        {
            NewMapRecognized += MapOverlayController.Instance.HandleNewMapRecognized;
        }

        protected override void Action()
        {
            ScreenshotRecognizer.HandleSurvivors();
        }
    }
}
