using DBDUtilityOverlay.Utils;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Application = System.Windows.Application;
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

        public MainWindow()
        {
            InitializeComponent();
            overlay = new MapOverlay();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            WindowStyle = WindowStyle.None;

            overlay.WindowStyle = WindowStyle.None;
            overlay.AllowsTransparency = true;
            overlay.Opacity = Properties.Settings.Default.OverlayOpacity / 100.0;
            overlay.ShowInTaskbar = false;
            overlay.Topmost = true;
            overlay.Left = Properties.Settings.Default.OverlayX;
            overlay.Top = Properties.Settings.Default.OverlayY;

            OpenCloseOverlay(Properties.Settings.Default.IsOverlayOpened);
            OpacitySlider.Value = Properties.Settings.Default.OverlayOpacity;
            ScreenshotRecognizer.SetScreenBounds();
            ScreenshotRecognizer.CreateTrainedData();
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
            if (e.ChangedButton == MouseButton.Left) DragMove();
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
                            overlay.ChangeMap(ScreenshotRecognizer.GetMapInfo());
                            handled = true;
                            break;
                        case NEXT_MAP_ID:
                            overlay.SwitchMapVariationToNext();
                            handled = true;
                            break;
                        case PREVIOUS_MAP_ID:
                            overlay.SwitchMapVariationToPrevious();
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }
    }
}