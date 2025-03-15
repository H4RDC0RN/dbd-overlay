using DBDUtilityOverlay.MVVM.ViewModel;
using System.Windows;
using System.Windows.Input;
using Application = System.Windows.Application;
using Logger = DBDUtilityOverlay.Core.Utils.Logger;
using Window = System.Windows.Window;
using DBDUtilityOverlay.Core.Utils;

namespace DBDUtilityOverlay
{
    public partial class MainWindow : Window
    {
        private readonly MapOverlayTabViewModel mapOverlayTabVM;
        private readonly SettingsTabViewModel settingsTabVM;
        private readonly AboutTabViewModel aboutTabVM;

        public MainWindow()
        {
            Logger.Log.Info("---Open Application---");
            InitializeComponent();
            HandleExceptions();
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
            Logger.Log.Info("---Close Application---");
            HotKeysController.Dispose();
            MapOverlayController.Instance.Close();
            Close();
            Application.Current.Shutdown();
        }

        private void MinButtonClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void HandleExceptions()
        {
            AppDomain.CurrentDomain.FirstChanceException += (sender, e) =>
            {
                Logger.Log.Fatal(e.Exception.Message);
                Logger.Log.Fatal(e.Exception.StackTrace);
                Logger.Log.Fatal("Recognized text:");
                Logger.Log.Fatal(ScreenshotRecognizer.Text);
                Logger.Log.Info("---Close Application with exception---");
            };
        }
    }
}