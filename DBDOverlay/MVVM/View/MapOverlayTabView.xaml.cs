using DBDOverlay.Core.BackgroundProcesses;
using DBDOverlay.Core.MapOverlay;
using DBDOverlay.Core.Windows;
using DBDOverlay.Properties;
using System;
using System.Windows;
using UserControl = System.Windows.Controls.UserControl;

namespace DBDOverlay.MVVM.View
{
    public partial class MapOverlayTabView : UserControl
    {
        public MapOverlayTabView()
        {
            InitializeComponent();
            OpenCloseToggleButton.IsChecked = Settings.Default.IsOverlayOpened;
            OpacitySlider.Value = Settings.Default.OverlayOpacity;
            AutoModeToggleButton.IsChecked = AutoMode.Instance.IsActive ? true : AutoModeToggleButton.IsChecked = Settings.Default.IsAutoMode;

            if (MapOverlayController.Instance.CanBeMoved) MoveToggleButton.IsChecked = true;

            WindowsServices.Instance.MoveModeOff += HandleMoveModeOff;
        }

        private void OpenClose_Checked(object sender, RoutedEventArgs e)
        {
            MapOverlayController.Overlay.Show();
            Settings.Default.IsOverlayOpened = true;
            Settings.Default.Save();
        }

        private void OpenClose_Unchecked(object sender, RoutedEventArgs e)
        {
            MapOverlayController.Overlay.Hide();
            Settings.Default.IsOverlayOpened = false;
            Settings.Default.Save();
        }

        private void Move_Checked(object sender, RoutedEventArgs e)
        {
            MapOverlayController.Instance.CanBeMoved = true;
            WindowsServices.Instance.RevertWindowExTransparent(MapOverlayController.Overlay, MapOverlayController.Overlay.DefaultStyle);
        }

        private void Move_Unchecked(object sender, RoutedEventArgs e)
        {
            MapOverlayController.Overlay.DefaultStyle = WindowsServices.Instance.SetWindowExTransparent(MapOverlayController.Overlay);
            MapOverlayController.Instance.CanBeMoved = false;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (MapOverlayController.Overlay == null) return;

            MapOverlayController.Overlay.Opacity = OpacitySlider.Value / 100;
            Settings.Default.OverlayOpacity = (int)OpacitySlider.Value;
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

        private void HandleMoveModeOff(object sender, EventArgs e)
        {
            MoveToggleButton.Uncheck();
        }
    }
}
