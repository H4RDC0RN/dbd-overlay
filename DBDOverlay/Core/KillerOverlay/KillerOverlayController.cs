using DBDOverlay.Windows;
using System.Collections.Generic;

namespace DBDOverlay.Core.KillerOverlay
{
    public class KillerOverlayController
    {
        public List<Survivor> Survivors = new List<Survivor>();
        private static KillerOverlayController instance;
        private static HooksOverlayWindow hooksOverlay;
        private static PostUnhookTimerOverlayWindow postUnhookTimerOverlay;

        public static KillerOverlayController Instance
        {
            get
            {
                if (instance == null)
                    instance = new KillerOverlayController();
                return instance;
            }
        }

        public static HooksOverlayWindow HooksOverlay
        {
            get
            {
                if (hooksOverlay == null)
                    hooksOverlay = new HooksOverlayWindow();
                return hooksOverlay;
            }
        }

        public static PostUnhookTimerOverlayWindow PostUnhookTimerOverlay
        {
            get
            {
                if (postUnhookTimerOverlay == null)
                    postUnhookTimerOverlay = new PostUnhookTimerOverlayWindow();
                return postUnhookTimerOverlay;
            }
        }

        public KillerOverlayController()
        {
            for (int i = 1; i <= 4; i++)
            {
                Survivors.Add(new Survivor());
            }
        }
    }
}
