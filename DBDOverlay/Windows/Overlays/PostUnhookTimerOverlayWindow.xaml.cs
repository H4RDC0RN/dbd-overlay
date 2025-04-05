using DBDOverlay.Core.Windows;
using System;
using System.Windows;

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
        }
    }
}
