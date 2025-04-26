using DBDOverlay.Core.ImageProcessing;

namespace DBDOverlay.Core.BackgroundProcesses
{
    public class KillerMode : BaseBackgroundProcess
    {
        public bool IsHookMode { get; set; } = false;
        public bool IsPostUnhookTimerMode { get; set; } = false;
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

        protected override void Action()
        {
            ImageReader.Instance.HandleSurvivors();
        }
    }
}
