using DBDOverlay.Core.Hotkeys;
using DBDOverlay.Core.WindowControllers.KillerOverlay;
using DBDOverlay.Core.WindowControllers.MapOverlay;
using DBDOverlay.Core.Windows;
using DBDOverlay.Properties;
using DBDOverlay.UI.Tabs;
using System;
using System.Windows;
using System.Windows.Input;
using Application = System.Windows.Application;
using Logger = DBDOverlay.Core.Utils.Logger;
using Window = System.Windows.Window;

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
            LocationChanged += MainWindow_LocationChanged;
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

        private void MainWindow_LocationChanged(object sender, EventArgs e)
        {
            if (!IsVisible) return;
            if (Settings.Default.IsSidePanelMode) KillerOverlayController.Window.SetKillerWindowShowPosition();
            else KillerOverlayController.Window.SetKillerWindowHiddenPosition();
        }

        private void ExitButtonClick(object sender, RoutedEventArgs e)
        {
            Logger.Info("---Close Application---");
            WindowsServices.Instance.StopMonitoring();
            HotKeysController.Dispose();
            MapOverlayController.Overlay.Close();
            KillerOverlayController.Overlay.Close();
            KillerOverlayController.Window.Close();
            Close();
            WindowsServices.Instance.CloseRedundantProcesses();
            Application.Current.Shutdown();
        }
    }
}