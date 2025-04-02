using System.Windows;
using System.Windows.Input;
using Application = System.Windows.Application;
using Logger = DBDOverlay.Core.Utils.Logger;
using Window = System.Windows.Window;
using DBDOverlay.Core.Hotkeys;
using DBDOverlay.Core.MapOverlay;
using DBDOverlay.MVVM.ViewModel;
using System;
using DBDOverlay.Core.Download;
using System.Configuration;
using DBDOverlay.Properties;
using System.IO;
using DBDOverlay.Core.ImageProcessing;
using DBDOverlay.Core.Utils;

namespace DBDOverlay
{
    public partial class MainWindow : Window
    {
        private readonly MapOverlayTabViewModel mapOverlayTabVM;
        private readonly SettingsTabViewModel settingsTabVM;
        private readonly AboutTabViewModel aboutTabVM;

        public MainWindow()
        {
            HandleExceptions();
            Folders.CreateDefault();
            DownloadManager.Instance.DownloadDefaultLanguage();
            DownloadManager.Instance.CheckForUpdate();
            ReloadSettings();

            Logger.Info("---Open Application---");
            InitializeComponent();
            ScreenshotRecognizer.SetScreenBounds();

            mapOverlayTabVM = new MapOverlayTabViewModel();
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
            Close();
            Application.Current.Shutdown();
        }

        private void MinButtonClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ReloadSettings()
        {
            string configPath = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
            if (!File.Exists(configPath))
            {
                Settings.Default.Upgrade();
                Settings.Default.Reload();
                Settings.Default.Save();
            }
        }

        private void HandleExceptions()
        {
            AppDomain.CurrentDomain.FirstChanceException += (sender, e) =>
            {
                Logger.Fatal(e.Exception.Message);
                Logger.Fatal(e.Exception.StackTrace);
                Logger.Info("---Close Application with exception---");
            };
        }
    }
}