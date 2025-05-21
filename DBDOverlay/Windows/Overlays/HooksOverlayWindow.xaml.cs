using DBDOverlay.Core.WindowControllers.KillerOverlay;
using DBDOverlay.Core.Windows;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DBDOverlay.Windows
{
    public partial class HooksOverlayWindow : Window
    {
        public int DefaultStyle { get; set; }

        public HooksOverlayWindow()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            DefaultStyle = WindowsServices.Instance.SetWindowExTransparent(this);
        }

        public Label GetHooksLabel(SurvivorNumber survivor)
        {
            switch (survivor)
            {
                case SurvivorNumber.First: return SurvivorHooks_1;
                case SurvivorNumber.Second: return SurvivorHooks_2;
                case SurvivorNumber.Third: return SurvivorHooks_3;
                case SurvivorNumber.Fourth: return SurvivorHooks_4;
                default: return null;
            }
        }
    }
}
