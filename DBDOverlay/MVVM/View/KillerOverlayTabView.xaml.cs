using DBDOverlay.Core.BackgroundProcesses;
using DBDOverlay.Core.KillerOverlay;
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

            KillerOverlayController.HooksOverlay.Show();
            KillerMode.Instance.IsHookMode = true;
            Settings.Default.IsHookMode = true;
            Settings.Default.Save();
        }

        private void Hooks_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!KillerMode.Instance.IsPostUnhookTimerMode) KillerMode.Instance.Stop();

            KillerOverlayController.HooksOverlay.Hide();
            KillerMode.Instance.IsHookMode = false;
            Settings.Default.IsHookMode = false;
            Settings.Default.Save();
        }

        private void PostUnhookTimer_Checked(object sender, RoutedEventArgs e)
        {
            if (!KillerMode.Instance.IsActive) KillerMode.Instance.Run();

            KillerOverlayController.PostUnhookTimerOverlay.Show();
            KillerMode.Instance.IsPostUnhookTimerMode = true;
            Settings.Default.IsPostUnhookTimerMode = true;
            Settings.Default.Save();
        }

        private void PostUnhookTimer_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!KillerMode.Instance.IsHookMode) KillerMode.Instance.Stop();

            KillerOverlayController.PostUnhookTimerOverlay.Hide();
            KillerMode.Instance.IsPostUnhookTimerMode = false;
            Settings.Default.IsPostUnhookTimerMode = false;
            Settings.Default.Save();
        }
    }
}
