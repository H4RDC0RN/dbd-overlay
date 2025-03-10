using DBDUtilityOverlay.MVVM.ViewModel;
using DBDUtilityOverlay.Utils;
using DBDUtilityOverlay.Utils.Windows;
using System.Windows;
using System.Windows.Input;
using Application = System.Windows.Application;
using Logger = DBDUtilityOverlay.Utils.Logger;
using Window = System.Windows.Window;

namespace DBDUtilityOverlay
{
    public partial class MainWindow : Window
    {
        private readonly KeyboardHook hook = new();
        private readonly MapOverlayTabViewModel mapOverlayTabVM;
        private readonly SettingsTabViewModel settingsTabVM;
        private readonly AboutTabViewModel aboutTabVM;     

        public MainWindow()
        {
            Logger.Log.Info("---Open Application---");
            InitializeComponent();
            HandleExceptions();
            ScreenshotRecognizer.SetScreenBounds();
            MapOverlayController.Initialize();

            mapOverlayTabVM = new MapOverlayTabViewModel();
            settingsTabVM = new SettingsTabViewModel();
            aboutTabVM = new AboutTabViewModel();
            MapOverlayTab.IsChecked = true;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            hook.RegisterHotKey(ModifierKeys.Control, Keys.R, PressedRead);
            hook.RegisterHotKey(ModifierKeys.None, Keys.OemCloseBrackets, PressedNext);
            hook.RegisterHotKey(ModifierKeys.None, Keys.OemOpenBrackets, PressedPrevious);
        }

        protected override void OnClosed(EventArgs e)
        {
            hook.Dispose();
            base.OnClosed(e);
        }

        private void WindowMouseDown(object sender, MouseButtonEventArgs e)
        {
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
            ViewContent.Content= aboutTabVM;
        }

        private void ExitButtonClick(object sender, RoutedEventArgs e)
        {
            Logger.Log.Info("---Close Application---");
            MapOverlayController.Instance.Close();
            Close();
            Application.Current.Shutdown();
        }

        private void MinButtonClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void PressedRead(object sender, KeyPressedEventArgs e)
        {
            e.Log("Read map");
            MapOverlayController.ChangeMap(ScreenshotRecognizer.GetMapInfo());
        }

        private void PressedNext(object sender, KeyPressedEventArgs e)
        {
            e.Log("Next map");
            MapOverlayController.SwitchMapVariationToNext();
        }

        private void PressedPrevious(object sender, KeyPressedEventArgs e)
        {
            e.Log("Previous map");
            MapOverlayController.SwitchMapVariationToPrevious();
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