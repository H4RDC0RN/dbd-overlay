using DBDUtilityOverlay.Utils;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Application = System.Windows.Application;
using Window = System.Windows.Window;

namespace DBDUtilityOverlay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            WindowStyle = WindowStyle.None;

            overlay = new MapOverlay();
            overlay.WindowStyle = WindowStyle.None;
            overlay.AllowsTransparency = true;
            overlay.Opacity = 0.9;
            overlay.ShowInTaskbar = false;
            overlay.Topmost = true;
            overlay.IsHitTestVisible = false;
            overlay.Left = SystemParameters.PrimaryScreenWidth - overlay.Width;
            overlay.Top = 0;
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

        private void OpenButtonClick(object sender, RoutedEventArgs e)
        {
            overlay.Show();
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            overlay.Hide();
        }

        private void ReadButtonClick(object sender, RoutedEventArgs e)
        {
            overlay.ChangeMap(ScreenshotRecognizer.GetMapInfo());
        }

        private void RegisterHotKey()
        {
            var helper = new WindowInteropHelper(this);
            const uint MOD_CTRL = 0x0002;
            const uint VK_R = (uint)Keys.R;
            const uint VK_0 = (uint)Keys.D0;
            const uint VK_9 = (uint)Keys.D9;
            RegisterHotKey(helper.Handle, READ_MAP_ID, MOD_CTRL, VK_R);
            RegisterHotKey(helper.Handle, NEXT_MAP_ID, MOD_CTRL, VK_0);
            RegisterHotKey(helper.Handle, PREVIOUS_MAP_ID, MOD_CTRL, VK_9);
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