using DBDOverlay.Core.ImageProcessing;
using DBDOverlay.Properties;

namespace DBDOverlay.Core.BackgroundProcesses
{
    public class KillerMode : BaseBackgroundProcess
    {
        private static KillerMode instance;

        public static KillerMode Instance
        {
            get
            {
                if (instance == null) instance = new KillerMode();
                return instance;
            }
        }

        protected override void Action()
        {
            ImageReader.Instance.HandleSurvivors(Settings.Default.Is2v8Mode);
        }

        public void RunConditional()
        {
            if (!IsActive) Run();
        }

        public void StopConditional()
        {
            if (!Settings.Default.IsHookMode && !Settings.Default.IsPostUnhookTimerMode && !Settings.Default.IsSidePanelMode) Stop();
        }
    }
}
