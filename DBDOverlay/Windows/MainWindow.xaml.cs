using System.Windows;
using System.Windows.Input;
using Application = System.Windows.Application;
using Logger = DBDOverlay.Core.Utils.Logger;
using Window = System.Windows.Window;
using DBDOverlay.Core.Hotkeys;
using DBDOverlay.MVVM.ViewModel;
using DBDOverlay.Core.WindowControllers.KillerOverlay;
using DBDOverlay.Core.WindowControllers.MapOverlay;

namespace DBDOverlay.Windows
{
    public partial class MainWindow : Window
    {
        private readonly MapOverlayTabViewModel mapOverlayTabVM;
        private readonly KillerOverlayTabViewModel killerOverlayTabVM;
        private readonly ReshadeTabViewModel reshadeTabVM;
        private readonly SettingsTabViewModel settingsTabVM;
        private readonly AboutTabViewModel aboutTabVM;

        public MainWindow()
        {
            InitializeComponent();

            mapOverlayTabVM = new MapOverlayTabViewModel();
            killerOverlayTabVM = new KillerOverlayTabViewModel();
            reshadeTabVM = new ReshadeTabViewModel();
            settingsTabVM = new SettingsTabViewModel();
            aboutTabVM = new AboutTabViewModel();
            MapOverlayTab.IsChecked = true;
        }

        private void WindowMouseDown(object sender, MouseButtonEventArgs e)
        {
            MainGrid.Focus();
            DragMove();
        }

        private void MapOverlayTab_Selected(object sender, RoutedEventArgs e)
        {
            ViewContent.Content = mapOverlayTabVM;
        }

        private void KillerOverlayTab_Selected(object sender, RoutedEventArgs e)
        {
            ViewContent.Content = killerOverlayTabVM;
        }

        private void ReshadeTab_Selected(object sender, RoutedEventArgs e)
        {
            ViewContent.Content = reshadeTabVM;
        }

        private void SettingsTab_Selected(object sender, RoutedEventArgs e)
        {
            ViewContent.Content = settingsTabVM;
        }

        private void AboutTab_Selected(object sender, RoutedEventArgs e)
        {
            ViewContent.Content = aboutTabVM;
        }

        private void ExitButtonClick(object sender, RoutedEventArgs e)
        {
            Logger.Info("---Close Application---");
            HotKeysController.Dispose();
            MapOverlayController.Overlay.Close();
            KillerOverlayController.Overlay.Close();
            Close();
            Application.Current.Shutdown();
        }

        private void MinButtonClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}