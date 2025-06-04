using DBDOverlay.Core.BackgroundProcesses;
using DBDOverlay.Core.WindowControllers.KillerOverlay;
using DBDOverlay.Properties;
using System.Windows;
using System.Windows.Controls;

namespace DBDOverlay.MVVM.View
{
    public partial class KillerOverlayTabView : UserControl
    {
        public KillerOverlayTabView()
        {
            InitializeComponent();
            HooksToggleButton.IsChecked = KillerMode.Instance.IsActive && KillerMode.Instance.IsHookMode
                ? true : HooksToggleButton.IsChecked = Settings.Default.IsHookMode;

            PostUnhookTimerToggleButton.IsChecked = KillerMode.Instance.IsActive && KillerMode.Instance.IsPostUnhookTimerMode
                ? true : PostUnhookTimerToggleButton.IsChecked = Settings.Default.IsPostUnhookTimerMode;
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
        }

        private void SelectArea_Unchecked(object sender, RoutedEventArgs e)
        {
            KillerOverlayController.Overlay.HideGrid();
        }

        private void ResetPosSize_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //MapOverlayController.Overlay.Opacity = OpacitySlider.Value / 100;
            //Settings.Default.MapOverlayOpacity = (int)OpacitySlider.Value;
            //Settings.Default.Save();
        }

        private void Calibration_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Calibration_Unchecked(object sender, RoutedEventArgs e)
        {

        }
    }
}
