using DBDOverlay.Core.Extensions;
using DBDOverlay.Core.ImageProcessing;
using DBDOverlay.Core.WindowControllers.KillerOverlay;
using DBDOverlay.Core.Windows;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DBDOverlay.Windows.Overlays
{
    public partial class KillerOverlayWindow : Window
    {
        public int DefaultStyle { get; set; }

        public KillerOverlayWindow()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            DefaultStyle = WindowsServices.Instance.SetWindowExTransparent(this);
            KillerOverlayController.Instance.SetTimers();
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

        public void SetBounds()
        {
            var rect = ImageReader.Instance.GetRect(RectType.Survivors, SystemParameters.PrimaryScreenWidth.Round(), SystemParameters.PrimaryScreenHeight.Round());

            KillerOverlayController.Overlay.Left = rect.Left - (rect.Width / 2);
            KillerOverlayController.Overlay.Top = rect.Top;
            KillerOverlayController.Overlay.Width = rect.Width * 2;
            KillerOverlayController.Overlay.Height = rect.Height;
        }

        public void ShowHooks()
        {
            SetHooksVisibility(Visibility.Visible);
            Show();
        }

        public void HideHooks()
        {
            SetHooksVisibility(Visibility.Hidden);
            if (!PostUnhookTimerGrid.IsVisible) Hide();
        }

        public void ShowTimer()
        {
            SetTimerVisibility(Visibility.Visible);
            Show();
        }

        public void HideTimer()
        {
            SetTimerVisibility(Visibility.Hidden);
            if (!HooksGrid.IsVisible) Hide();
        }

        public void ShowGrid()
        {
            ChangeGridThickness(true);
        }

        public void HideGrid()
        {
            ChangeGridThickness(false);
        }

        private void ChangeGridThickness(bool isVisible)
        {
            var i = isVisible ? 1 : 0;
            Survivor1.BorderThickness = new Thickness(i, i, i, i);
            Survivor2.BorderThickness = new Thickness(i, 0, i, i);
            Survivor3.BorderThickness = new Thickness(i, 0, i, i);
            Survivor4.BorderThickness = new Thickness(i, 0, i, i);
            MainGridBorder.BorderThickness = new Thickness(i, i, i, i);
            MainGrid.Margin = new Thickness(-i, -i, -i, -i);
        }

        private void SetHooksVisibility(Visibility visibility)
        {
            HooksGrid.Visibility = visibility;
        }

        private void SetTimerVisibility(Visibility visibility)
        {
            PostUnhookTimerGrid.Visibility = visibility;
        }
    }
}
