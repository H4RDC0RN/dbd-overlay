using DBDUtilityOverlay.Core.Utils;
using DBDUtilityOverlay.Core.Windows;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using UserControl = System.Windows.Controls.UserControl;

namespace DBDUtilityOverlay.MVVM.View
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
            var a = OpenCloseToggleButton;
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
            WindowsServices.RevertWindowExTransparent(new WindowInteropHelper(MapOverlayController.Instance).Handle);
        }

        private void Move_Unchecked(object sender, RoutedEventArgs e)
        {
            WindowsServices.SetWindowExTransparent(new WindowInteropHelper(MapOverlayController.Instance).Handle);
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

        private void AnimateToggle()
        {
            var ticknessAnimation = new ThicknessAnimation();
            ticknessAnimation.From = new Thickness(5, 0, 0, 0);
            ticknessAnimation.To = new Thickness(45, 0, 0, 0);
            ticknessAnimation.Duration = TimeSpan.FromMicroseconds(15);

            var doubleAnimationForOff = new DoubleAnimation();
            doubleAnimationForOff.From = 1;
            doubleAnimationForOff.To = 0;
            doubleAnimationForOff.Duration = TimeSpan.FromMicroseconds(15);

            var doubleAnimationForOn = new DoubleAnimation();
            doubleAnimationForOn.From = 0;
            doubleAnimationForOn.To = 1;
            doubleAnimationForOn.Duration = TimeSpan.FromMicroseconds(15);            

            OpenCloseToggleButton.BeginAnimation(MarginProperty, ticknessAnimation);
            OpenCloseToggleButton.BeginAnimation(OpacityProperty, doubleAnimationForOff);
            OpenCloseToggleButton.BeginAnimation(OpacityProperty, doubleAnimationForOn);

            //< ThicknessAnimation Storyboard.TargetName = "Ellipse"
            //                                                Storyboard.TargetProperty = "(Margin)"
            //                                                From = "5,0,0,0"
            //                                                To = "45,0,0,0"
            //                                                Duration = "0:0:0.15"
            //                                                AutoReverse = "False" />
            //                            < DoubleAnimation Storyboard.TargetName = "TextOff"
            //                                             Storyboard.TargetProperty = "(Opacity)"
            //                                             From = "1"
            //                                             To = "0"
            //                                             Duration = "0:0:0.15"
            //                                             AutoReverse = "False" />
            //                            < DoubleAnimation Storyboard.TargetName = "TextOn"
            //                                             Storyboard.TargetProperty = "(Opacity)"
            //                                             From = "0"
            //                                             To = "1"
            //                                             Duration = "0:0:0.15"
            //                                             AutoReverse = "False" />
            //                            < ColorAnimation Storyboard.TargetName = "Border"
            //                                            Storyboard.TargetProperty = "Background.Color"
            //                                            From = "{StaticResource GrayColor}"
            //                                            To = "{StaticResource RedColor}"
            //                                            Duration = "0:0:0.15" />
        }
    }
}
