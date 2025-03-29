using DBDOverlay.Core.MapOverlay;
using DBDOverlay.Core.Windows;
using DBDOverlay.Properties;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;
using UserControl = System.Windows.Controls.UserControl;

namespace DBDOverlay.MVVM.View
{
    public partial class MapOverlayTabView : UserControl
    {
        public MapOverlayTabView()
        {
            InitializeComponent();
            OpenCloseOverlay(Settings.Default.IsOverlayOpened);
            OpacitySlider.Value = Settings.Default.OverlayOpacity;
        }

        private void OpenClose_Checked(object sender, RoutedEventArgs e)
        {
            MapOverlayController.Instance.Show();
            Settings.Default.IsOverlayOpened = true;
            Settings.Default.Save();
        }

        private void OpenClose_Unchecked(object sender, RoutedEventArgs e)
        {
            MapOverlayController.Instance.Hide();
            Settings.Default.IsOverlayOpened = false;
            Settings.Default.Save();
        }

        private void Move_Checked(object sender, RoutedEventArgs e)
        {
            WindowsServices.Instance.RevertWindowExTransparent(new WindowInteropHelper(MapOverlayController.Instance).Handle.ToInt32());
        }

        private void Move_Unchecked(object sender, RoutedEventArgs e)
        {
            WindowsServices.Instance.SetWindowExTransparent(new WindowInteropHelper(MapOverlayController.Instance).Handle.ToInt32());
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (MapOverlayController.Instance != null)
            {
                MapOverlayController.Instance.Opacity = OpacitySlider.Value / 100;
                Settings.Default.OverlayOpacity = (int)OpacitySlider.Value;
                Settings.Default.Save();
            }
        }

        private void AutoMode_Checked(object sender, RoutedEventArgs e)
        {
            AutoModeManager.Instance.RunAutoMode();
        }

        private void AutoMode_Unchecked(object sender, RoutedEventArgs e)
        {
            AutoModeManager.Instance.StopAutoMode();
        }

        private void OpenCloseOverlay(bool IsOverlayOpened)
        {
            if (IsOverlayOpened) OpenCloseToggleButton.IsChecked = true; else OpenCloseToggleButton.IsChecked = false;
        }
    }
}
