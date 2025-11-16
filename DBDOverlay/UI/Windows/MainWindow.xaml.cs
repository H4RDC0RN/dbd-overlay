using System.Windows;
using System.Windows.Input;
using Application = System.Windows.Application;
using Logger = DBDOverlay.Core.Utils.Logger;
using Window = System.Windows.Window;
using DBDOverlay.Core.Hotkeys;
using DBDOverlay.Core.WindowControllers.KillerOverlay;
using DBDOverlay.Core.WindowControllers.MapOverlay;
using DBDOverlay.Core.Windows;
using DBDOverlay.MVVM.View;

namespace DBDOverlay.UI.Windows
{
    public partial class MainWindow : Window
    {
        private readonly MapOverlayTabView mapOverlayTab;
        private readonly KillerOverlayTabView killerOverlayTab;
        private readonly ReshadeTabView reshadeTab;
        private readonly SettingsTabView settingsTab;
        private readonly AboutTabView aboutTab;

        public MainWindow()
        {
            InitializeComponent();

            mapOverlayTab = new MapOverlayTabView();
            killerOverlayTab = new KillerOverlayTabView();
            reshadeTab = new ReshadeTabView();
            settingsTab = new SettingsTabView();
            aboutTab = new AboutTabView();
            MapOverlayTab.IsChecked = true;
        }

        private void WindowMouseDown(object sender, MouseButtonEventArgs e)
        {
            MainGrid.Focus();
            DragMove();
        }

        private void MapOverlayTab_Selected(object sender, RoutedEventArgs e)
        {
            ViewContent.Content = mapOverlayTab;
        }

        private void KillerOverlayTab_Selected(object sender, RoutedEventArgs e)
        {
            ViewContent.Content = killerOverlayTab;
        }

        private void ReshadeTab_Selected(object sender, RoutedEventArgs e)
        {
            ViewContent.Content = reshadeTab;
        }

        private void SettingsTab_Selected(object sender, RoutedEventArgs e)
        {
            ViewContent.Content = settingsTab;
        }

        private void AboutTab_Selected(object sender, RoutedEventArgs e)
        {
            ViewContent.Content = aboutTab;
        }

        private void MinButtonClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ExitButtonClick(object sender, RoutedEventArgs e)
        {
            Logger.Info("---Close Application---");
            HotKeysController.Dispose();
            MapOverlayController.Overlay.Close();
            KillerOverlayController.Overlay.Close();
            Close();
            WindowsServices.Instance.CloseRedundantProcesses();
            Application.Current.Shutdown();
        }
    }
}