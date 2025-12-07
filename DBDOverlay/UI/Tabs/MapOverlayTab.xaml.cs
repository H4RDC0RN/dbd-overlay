using DBDOverlay.Core.BackgroundProcesses;
using DBDOverlay.Core.WindowControllers.MapOverlay;
using DBDOverlay.Core.Windows;
using DBDOverlay.Properties;
using System;
using System.Windows;
using UserControl = System.Windows.Controls.UserControl;

namespace DBDOverlay.UI.Tabs
{
    public partial class MapOverlayTabView : UserControl
    {
        public MapOverlayTabView()
        {
            InitializeComponent();
            OpenCloseToggleButton.IsChecked = Settings.Default.IsMapOverlayOpened;
            OpacitySlider.Value = Settings.Default.MapOverlayOpacity;
            AutoModeToggleButton.IsChecked = AutoMode.Instance.IsActive ? true : AutoModeToggleButton.IsChecked = Settings.Default.IsAutoMode;

            if (MapOverlayController.Instance.CanBeMoved) ReToggleButton.IsChecked = true;
            WindowsServices.Instance.MapOverlayMoveModeOff += HandleMoveModeOff;
        }

        private void OpenClose_Checked(object sender, RoutedEventArgs e)
        {
            MapOverlayController.Overlay.Show();
            Settings.Default.IsMapOverlayOpened = true;
            Settings.Default.Save();
        }

        private void OpenClose_Unchecked(object sender, RoutedEventArgs e)
        {
            MapOverlayController.Overlay.Hide();
            Settings.Default.IsMapOverlayOpened = false;
            Settings.Default.Save();
        }

        private void AutoMode_Checked(object sender, RoutedEventArgs e)
        {
            if (!AutoMode.Instance.IsActive)
            {
                AutoMode.Instance.Run();
                Settings.Default.IsAutoMode = true;
                Settings.Default.Save();
            }
        }

        private void AutoMode_Unchecked(object sender, RoutedEventArgs e)
        {
            AutoMode.Instance.Stop();
            Settings.Default.IsAutoMode = false;
            Settings.Default.Save();
        }

        private void Re_Checked(object sender, RoutedEventArgs e)
        {
            MapOverlayController.Instance.CanBeMoved = true;
            WindowsServices.Instance.RevertWindowExTransparent(MapOverlayController.Overlay, MapOverlayController.Overlay.DefaultStyle);
        }

        private void Re_Unchecked(object sender, RoutedEventArgs e)
        {
            MapOverlayController.Overlay.DefaultStyle = WindowsServices.Instance.SetWindowExTransparent(MapOverlayController.Overlay);
            MapOverlayController.Instance.CanBeMoved = false;
            MapOverlayController.Overlay.SaveSize();
            MapOverlayController.Overlay.SavePosition();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (MapOverlayController.Overlay == null) return;

            MapOverlayController.Overlay.Opacity = OpacitySlider.Value / 100;
            Settings.Default.MapOverlayOpacity = (int)OpacitySlider.Value;
            Settings.Default.Save();
        }

        private void AlignH_Click(object sender, RoutedEventArgs e)
        {
            MapOverlayController.Overlay.Width = MapOverlayController.Overlay.Height / MapOverlayController.Overlay.DefaultRatio;
        }

        private void AlignV_Click(object sender, RoutedEventArgs e)
        {
            MapOverlayController.Overlay.Height = MapOverlayController.Overlay.Width * MapOverlayController.Overlay.DefaultRatio;
        }

        private void HandleMoveModeOff(object sender, EventArgs e)
        {
            ReToggleButton.Uncheck();
        }

        private void ResetPosition_Click(object sender, RoutedEventArgs e)
        {
            MapOverlayController.Overlay.ResetPosition();
        }

        private void ResetSize_Click(object sender, RoutedEventArgs e)
        {
            MapOverlayController.Overlay.ResetSize();
        }

        private void ResetAll_Click(object sender, RoutedEventArgs e)
        {
            MapOverlayController.Overlay.ResetPosition();
            MapOverlayController.Overlay.ResetSize();
        }
    }
}
