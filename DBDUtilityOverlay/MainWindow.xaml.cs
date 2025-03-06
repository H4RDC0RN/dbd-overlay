using DBDUtilityOverlay.MVVM.ViewModel;
using DBDUtilityOverlay.Utils;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Application = System.Windows.Application;
using Logger = DBDUtilityOverlay.Utils.Logger;
using Window = System.Windows.Window;

namespace DBDUtilityOverlay
{
    public partial class MainWindow : Window
    {
        [DllImport("User32.dll")]
        private static extern bool RegisterHotKey([In] IntPtr hWnd, [In] int id, [In] uint fsModifiers, [In] uint vk);

        [DllImport("User32.dll")]
        private static extern bool UnregisterHotKey([In] IntPtr hWnd, [In] int id);

        private HwndSource source;
        private const int READ_MAP_ID = 9000;
        private const int NEXT_MAP_ID = 9001;
        private const int PREVIOUS_MAP_ID = 9002;

        private MapOverlayTabViewModel mapOverlayTabVM;
        private SettingsTabViewModel settingsTabVM;
        private AboutTabViewModel aboutTabVM;


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
            var helper = new WindowInteropHelper(this);
            source = HwndSource.FromHwnd(helper.Handle);
            source.AddHook(HwndHook);
            RegisterHotKey();
        }

        protected override void OnClosed(EventArgs e)
        {
            source?.RemoveHook(HwndHook);
            source = null;
            UnregisterHotKey();
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

        private void RegisterHotKey()
        {
            var helper = new WindowInteropHelper(this);
            const uint MOD_CTRL = 0x0002;
            const uint MOD_NON = 0x00;
            const uint VK_R = (uint)Keys.R;
            const uint VK_0 = (uint)Keys.D0;
            const uint VK_9 = (uint)Keys.D9;
            RegisterHotKey(helper.Handle, READ_MAP_ID, MOD_CTRL, VK_R);
            RegisterHotKey(helper.Handle, NEXT_MAP_ID, MOD_NON, VK_0);
            RegisterHotKey(helper.Handle, PREVIOUS_MAP_ID, MOD_NON, VK_9);
        }

        private void UnregisterHotKey()
        {
            var helper = new WindowInteropHelper(this);
            UnregisterHotKey(helper.Handle, READ_MAP_ID);
            UnregisterHotKey(helper.Handle, NEXT_MAP_ID);
            UnregisterHotKey(helper.Handle, PREVIOUS_MAP_ID);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case READ_MAP_ID:
                            Logger.Log.Info("'Read map' hotkey is pressed (CTRL + R)");
                            MapOverlayController.ChangeMap(ScreenshotRecognizer.GetMapInfo());
                            handled = true;
                            break;
                        case NEXT_MAP_ID:
                            Logger.Log.Info("'Next map' hotkey is pressed (0)");
                            MapOverlayController.SwitchMapVariationToNext();
                            handled = true;
                            break;
                        case PREVIOUS_MAP_ID:
                            Logger.Log.Info("'Previous map' hotkey is pressed (9)");
                            MapOverlayController.SwitchMapVariationToPrevious();
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
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