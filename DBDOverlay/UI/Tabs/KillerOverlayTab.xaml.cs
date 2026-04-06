using DBDOverlay.Core.BackgroundProcesses;
using DBDOverlay.Core.Extensions;
using DBDOverlay.Core.ImageProcessing;
using DBDOverlay.Core.WindowControllers.KillerOverlay;
using DBDOverlay.Core.Windows;
using DBDOverlay.Properties;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;

namespace DBDOverlay.UI.Tabs
{
    public partial class KillerOverlayTabView : UserControl
    {
        private readonly int RGBSum = 765;
        private readonly int defaultHooksThreshold = 600;
        private Bitmap currentImage;

        public KillerOverlayTabView()
        {
            InitializeComponent();
            KillerOverlayController.Overlay.HideHooks();
            KillerOverlayController.Overlay.HideTimer();

            HooksToggleButton.IsChecked = Settings.Default.IsHookMode;
            PostUnhookTimerToggleButton.IsChecked = Settings.Default.IsPostUnhookTimerMode;
            Mode2v8ToggleButton.IsChecked = Settings.Default.Is2v8Mode;
            SidePanelToggleButton.IsChecked = Settings.Default.IsSidePanelMode;

            //if (KillerOverlayController.Instance.CanBeMoved) SelectAreaToggleButton.IsChecked = true;
            WindowsServices.Instance.KillerOverlayMoveModeOff += HandleMoveModeOff;
            ImageReader.Instance.UpdatinghooksImage += UpdateHooksImage;

            SetSliderValue(Settings.Default.HooksThreshold);
        }

        private void Hooks_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Default.IsHookMode = true;
            Settings.Default.Save();
            KillerOverlayController.Overlay.ShowHooks();
            KillerMode.Instance.RunConditional();
        }

        private void Hooks_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.Default.IsHookMode = false;
            Settings.Default.Save();
            KillerOverlayController.Overlay.HideHooks();
            KillerMode.Instance.StopConditional();
        }

        private void PostUnhookTimer_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Default.IsPostUnhookTimerMode = true;
            Settings.Default.Save();
            KillerOverlayController.Overlay.ShowTimer();
            KillerMode.Instance.RunConditional();
        }

        private void PostUnhookTimer_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.Default.IsPostUnhookTimerMode = false;
            Settings.Default.Save();
            KillerOverlayController.Overlay.HideTimer();
            KillerMode.Instance.StopConditional();
        }

        private void Mode2v8_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Default.Is2v8Mode = true;
            Settings.Default.Save();
            KillerOverlayController.Overlay.ShowMoreSurvivors();
            KillerOverlayController.Window.ShowMoreSurvivors(false);
        }

        private void Mode2v8_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.Default.Is2v8Mode = false;
            Settings.Default.Save();
            KillerOverlayController.Overlay.HideMoreSurvivors();
            KillerOverlayController.Window.HideMoreSurvivors(false);
        }

        private void SidePanel_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Default.IsSidePanelMode = true;
            Settings.Default.Save();
            KillerMode.Instance.RunConditional();
            if (SidePanelToggleButton.IsVisible)
            {
                KillerOverlayController.Window.ShowSidePanel();
            }
        }

        private void SidePanel_Unchecked(object sender, RoutedEventArgs e)
        {
            if (SidePanelToggleButton.IsVisible)
            {
                KillerOverlayController.Window.HideSidePanel();                
            }
            KillerMode.Instance.StopConditional();
            Settings.Default.IsSidePanelMode = false;
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
            var threshold = (ThresholdSlider.Value * RGBSum / 100).Round();
            SetThreshold(threshold);
            if (currentImage != null) UpdateImageSource(threshold);
        }

        private void ResetThreshold_Click(object sender, RoutedEventArgs e)
        {
            SetThreshold(defaultHooksThreshold);
            SetSliderValue(defaultHooksThreshold);
        }

        private void HandleMoveModeOff(object sender, EventArgs e)
        {
            //SelectAreaToggleButton.Uncheck();
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
            currentImage = new Bitmap(e.Image);
            UpdateImageSource(e.Threshold);
        }

        private void UpdateImageSource(int threshold)
        {
            //SurvivorsAreaImage.Source = new Bitmap(currentImage).PreProcess(threshold: threshold).ToBitmapImage();
        }
    }
}
