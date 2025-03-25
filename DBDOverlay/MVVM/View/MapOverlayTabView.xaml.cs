using DBDOverlay.Core.MapOverlay;
using DBDOverlay.Core.Windows;
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
            OpenCloseOverlay(Properties.Settings.Default.IsOverlayOpened);
            OpacitySlider.Value = Properties.Settings.Default.OverlayOpacity;
        }

        private void OpenClose_Checked(object sender, RoutedEventArgs e)
        {
            MapOverlayController.Instance.Show();
            Properties.Settings.Default.IsOverlayOpened = true;
            Properties.Settings.Default.Save();
        }

        private void OpenClose_Unchecked(object sender, RoutedEventArgs e)
        {
            MapOverlayController.Instance.Hide();
            Properties.Settings.Default.IsOverlayOpened = false;
            Properties.Settings.Default.Save();
        }

        private void Move_Checked(object sender, RoutedEventArgs e)
        {
            WindowsServices.Instance.RevertWindowExTransparent(new WindowInteropHelper(MapOverlayController.Instance).Handle);
        }

        private void Move_Unchecked(object sender, RoutedEventArgs e)
        {
            WindowsServices.Instance.SetWindowExTransparent(new WindowInteropHelper(MapOverlayController.Instance).Handle);
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (MapOverlayController.Instance != null)
            {
                MapOverlayController.Instance.Opacity = OpacitySlider.Value / 100;
                Properties.Settings.Default.OverlayOpacity = (int)OpacitySlider.Value;
                Properties.Settings.Default.Save();
            }
        }

        private void OpenCloseOverlay(bool IsOverlayOpened)
        {
            if (IsOverlayOpened) OpenCloseToggleButton.IsChecked = true; else OpenCloseToggleButton.IsChecked = false;
        }
    }
}
