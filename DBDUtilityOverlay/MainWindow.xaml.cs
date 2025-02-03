using DBDUtilityOverlay.Utils;
using DBDUtilityOverlay.Utils.Languages;
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

        private HwndSource? source;
        private const int READ_MAP_ID = 9000;
        private const int NEXT_MAP_ID = 9001;
        private const int PREVIOUS_MAP_ID = 9002;

        private readonly MapOverlay overlay;
        private readonly DownloadLanguage downloadLanguage;

        public MainWindow()
        {
            InitializeComponent();
            HandleExceptions();
            overlay = new MapOverlay();
            downloadLanguage = new DownloadLanguage();
            SetOverlaySettings();
            SetLanguages();

            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            WindowStyle = WindowStyle.None;

            OpenCloseOverlay(Properties.Settings.Default.IsOverlayOpened);
            OpacitySlider.Value = Properties.Settings.Default.OverlayOpacity;
            ScreenshotRecognizer.SetScreenBounds();
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

        private void SetOverlaySettings()
        {
            overlay.WindowStyle = WindowStyle.None;
            overlay.AllowsTransparency = true;
            overlay.Opacity = Properties.Settings.Default.OverlayOpacity / 100.0;
            overlay.ShowInTaskbar = false;
            overlay.Topmost = true;
            overlay.Left = Properties.Settings.Default.OverlayX;
            overlay.Top = Properties.Settings.Default.OverlayY;
        }

        private void SetLanguages()
        {
            var downloadedLanguages = ScreenshotRecognizer.GetDownloadedLanguages();
            var languages = LanguagesManager.GetOrderedKeyValuePairs(downloadedLanguages);
            LanguageComboBox.ItemsSource = languages.Select(x => x.Key);
            LanguageComboBox.SelectedIndex = languages.Select(x => x.Value).ToList().IndexOf(Properties.Settings.Default.Language);
        }

        private void WindowMouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ExitButtonClick(object sender, RoutedEventArgs e)
        {
            overlay.Close();
            Close();
            Application.Current.Shutdown();
        }

        private void MinButtonClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void OpenRB_Checked(object sender, RoutedEventArgs e)
        {
            overlay.Show();
            Properties.Settings.Default.IsOverlayOpened = true;
            Properties.Settings.Default.Save();
        }

        private void CloseRB_Checked(object sender, RoutedEventArgs e)
        {
            overlay.Hide();
            Properties.Settings.Default.IsOverlayOpened = false;
            Properties.Settings.Default.Save();
        }

        private void ReadButtonClick(object sender, RoutedEventArgs e)
        {
            overlay.ChangeMap(ScreenshotRecognizer.GetMapInfo());
        }

        private void MoveCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            WindowsServices.RevertWindowExTransparent(new WindowInteropHelper(overlay).Handle);
        }

        private void MoveCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            WindowsServices.SetWindowExTransparent(new WindowInteropHelper(overlay).Handle);
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (overlay != null)
            {
                overlay.Opacity = OpacitySlider.Value / 100;
                Properties.Settings.Default.OverlayOpacity = (int)OpacitySlider.Value;
                Properties.Settings.Default.Save();
            }
        }

        private void OpenCloseOverlay(bool IsOverlayOpened)
        {
            if (IsOverlayOpened) OpenRB.IsChecked = true; else CloseRB.IsChecked = true;
        }

        private void LanguageComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var newLanguage = LanguagesManager.GetValue(LanguageComboBox.SelectedItem.ToString());
            Properties.Settings.Default.Language = newLanguage;
            Properties.Settings.Default.Save();
        }

        private void RefreshButtonClick(object sender, RoutedEventArgs e)
        {
            SetLanguages();
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            downloadLanguage.ShowDialog();
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
                            overlay.ChangeMap(ScreenshotRecognizer.GetMapInfo());
                            handled = true;
                            break;
                        case NEXT_MAP_ID:
                            Logger.Log.Info("'Next map' hotkey is pressed (0)");
                            overlay.SwitchMapVariationToNext();
                            handled = true;
                            break;
                        case PREVIOUS_MAP_ID:
                            Logger.Log.Info("'Previous map' hotkey is pressed (9)");
                            overlay.SwitchMapVariationToPrevious();
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
            };
        }
    }
}