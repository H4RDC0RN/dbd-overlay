using DBDOverlay.Core.KillerOverlay;
using DBDOverlay.Core.Windows;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DBDOverlay.Windows
{
    public partial class PostUnhookTimerOverlayWindow : Window
    {
        public int DefaultStyle { get; set; }

        public PostUnhookTimerOverlayWindow()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            DefaultStyle = WindowsServices.Instance.SetWindowExTransparent(this);
            KillerOverlayController.Instance.SetTimers();
        }

        public Label GetTimerLabel(SurvivorNumber survivor)
        {
            switch (survivor)
            {
                case SurvivorNumber.First: return SurvivorTimer_1;
                case SurvivorNumber.Second: return SurvivorTimer_2;
                case SurvivorNumber.Third: return SurvivorTimer_3;
                case SurvivorNumber.Fourth: return SurvivorTimer_4;
                default: return null;
            }
        }
    }
}
