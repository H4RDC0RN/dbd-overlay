using DBDOverlay.Core.BackgroundProcesses;
using DBDOverlay.Core.Extensions;
using DBDOverlay.Core.ImageProcessing;
using DBDOverlay.Core.WindowControllers.KillerOverlay;
using DBDOverlay.Core.Windows;
using DBDOverlay.Properties;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DBDOverlay.MVVM.View
{
    public partial class KillerOverlayTabView : UserControl
    {
        private readonly int RGBSum = 765;
        private readonly int defaultHooksThreshold = 600;

        public KillerOverlayTabView()
        {
            InitializeComponent();
            KillerOverlayController.Overlay.HideHooks();
            KillerOverlayController.Overlay.HideTimer();
            HooksToggleButton.IsChecked = KillerMode.Instance.IsActive && KillerMode.Instance.IsHookMode
                ? true : HooksToggleButton.IsChecked = Settings.Default.IsHookMode;
            PostUnhookTimerToggleButton.IsChecked = KillerMode.Instance.IsActive && KillerMode.Instance.IsPostUnhookTimerMode
                ? true : PostUnhookTimerToggleButton.IsChecked = Settings.Default.IsPostUnhookTimerMode;

            if (KillerOverlayController.Instance.CanBeMoved) SelectAreaToggleButton.IsChecked = true;
            WindowsServices.Instance.KillerOverlayMoveModeOff += HandleMoveModeOff;
            ImageReader.Instance.UpdatinghooksImage += UpdateHooksImage;

            SetSliderValue(Settings.Default.HooksThreshold);
        }

        private void Hooks_Checked(object sender, RoutedEventArgs e)
        {
            if (!KillerMode.Instance.IsActive) KillerMode.Instance.Run();

            KillerOverlayController.Overlay.ShowHooks();
            KillerMode.Instance.IsHookMode = true;
            Settings.Default.IsHookMode = true;
            Settings.Default.Save();
        }

        private void Hooks_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!KillerMode.Instance.IsPostUnhookTimerMode) KillerMode.Instance.Stop();

            KillerOverlayController.Overlay.HideHooks();
            KillerMode.Instance.IsHookMode = false;
            Settings.Default.IsHookMode = false;
            Settings.Default.Save();
        }

        private void PostUnhookTimer_Checked(object sender, RoutedEventArgs e)
        {
            if (!KillerMode.Instance.IsActive) KillerMode.Instance.Run();

            KillerOverlayController.Overlay.ShowTimer();
            KillerMode.Instance.IsPostUnhookTimerMode = true;
            Settings.Default.IsPostUnhookTimerMode = true;
            Settings.Default.Save();
        }

        private void PostUnhookTimer_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!KillerMode.Instance.IsHookMode) KillerMode.Instance.Stop();

            KillerOverlayController.Overlay.HideTimer();
            KillerMode.Instance.IsPostUnhookTimerMode = false;
            Settings.Default.IsPostUnhookTimerMode = false;
            Settings.Default.Save();
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            KillerOverlayController.Instance.ResetSurvivors();
        }

        private void SelectArea_Checked(object sender, RoutedEventArgs e)
        {
            KillerOverlayController.Overlay.ShowGrid();
            KillerOverlayController.Instance.CanBeMoved = true;
            WindowsServices.Instance.RevertWindowExTransparent(KillerOverlayController.Overlay, KillerOverlayController.Overlay.DefaultStyle);
        }

        private void SelectArea_Unchecked(object sender, RoutedEventArgs e)
        {
            KillerOverlayController.Overlay.HideGrid();
            KillerOverlayController.Overlay.DefaultStyle = WindowsServices.Instance.SetWindowExTransparent(KillerOverlayController.Overlay);
            KillerOverlayController.Instance.CanBeMoved = false;
            KillerOverlayController.Overlay.SaveBounds();
        }

        private void ResetPosSize_Click(object sender, RoutedEventArgs e)
        {
            KillerOverlayController.Overlay.ResetBounds();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetThreshold((ThresholdSlider.Value * RGBSum / 100).Round());
        }

        private void ResetThreshold_Click(object sender, RoutedEventArgs e)
        {
            SetThreshold(defaultHooksThreshold);
            SetSliderValue(defaultHooksThreshold);
        }

        private void HandleMoveModeOff(object sender, EventArgs e)
        {
            SelectAreaToggleButton.Uncheck();
            KillerOverlayController.Overlay.SaveBounds();
        }

        private void Calibration_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Calibration_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void SetThreshold(int threshold)
        {
            ImageReader.Instance.SetHooksThreshold(threshold);
            Settings.Default.HooksThreshold = threshold;
            Settings.Default.Save();
        }

        private void SetSliderValue(int threshold)
        {
            ThresholdSlider.Value = (threshold * 100.0 / RGBSum).Round();
        }

        private void UpdateHooksImage(object sender, UpdateImageEventArgs e)
        {
           //if (SurvivorsAreaImage.IsVisible) 
                SurvivorsAreaImage.Source = e.Image.ToBitmapImage();
        }
    }
}
