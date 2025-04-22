using DBDOverlay.Core.Hotkeys;
using DBDOverlay.Core.MapOverlay;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

namespace DBDOverlay.Core.Windows
{
    public class WindowsServices
    {
        public event EventHandler<EventArgs> MoveModeOff;
        private readonly WinEventDelegate winEventDelegate;
        private static WindowsServices instance;

        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int GWL_EXSTYLE = -20;
        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_SYSTEM_FOREGROUND = 3;

        private readonly string dbdWindowName = "DeadByDaylight";
        private readonly string dbdProcessName = "DeadByDaylight-Win64-Shipping";
        private readonly string appWindowName = "DBD Overlay";
        private readonly string overlayWindowName = "Map overlay";

        public WindowsServices()
        {
            winEventDelegate = new WinEventDelegate(HandleWindowEvent);
            var m_hhook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND,
                0, winEventDelegate, 0, 0, WINEVENT_OUTOFCONTEXT);
        }

        public static WindowsServices Instance
        {
            get
            {
                if (instance == null)
                    instance = new WindowsServices();
                return instance;
            }
        }

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(int hwnd, int index);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(int hwnd, int index, int newStyle);

        delegate void WinEventDelegate(int hWinEventHook, uint eventType,
            int hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        static extern int SetWinEventHook(uint eventMin, uint eventMax,
            int hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(int hWnd);

        [DllImport("user32.dll")]
        private static extern int GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(int hWnd, StringBuilder text, int count);

        public void HandleWindowEvent(int hWinEventHook, uint eventType,
            int hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            HandleHotkeys();
            HandleMapOverlayMoveMode();
        }

        public bool IsDBDActiveWindow()
        {
            return GetActiveWindowTitle().Contains(dbdWindowName);
        }

        public bool IsAppWindow()
        {
            return GetActiveWindowTitle().Equals(appWindowName) || GetActiveWindowTitle().Equals(overlayWindowName);
        }

        public int SetWindowExTransparent(Window window)
        {
            var hwnd = new WindowInteropHelper(window).Handle.ToInt32();
            var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
            return extendedStyle;
        }

        public void RevertWindowExTransparent(Window window, int extendedStyle)
        {
            var hwnd = new WindowInteropHelper(window).Handle.ToInt32();
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle);
        }

        public void Send(string key)
        {
            var dbdProcess = Process.GetProcessesByName(dbdProcessName).First();
            if (SetForegroundWindow((int)dbdProcess.MainWindowHandle))
            {
                SendKeys.SendWait(key);
            }
        }

        private void HandleHotkeys()
        {
            if (IsDBDActiveWindow()) HotKeysController.RegisterAllHotKeys();
            else HotKeysController.UnregisterAllHotKeys();
        }

        private void HandleMapOverlayMoveMode()
        {
            if (MapOverlayController.Instance.CanBeMoved && !IsAppWindow())
            {
                MoveModeOff?.Invoke(this, new RoutedEventArgs());
            }
        }

        private string GetActiveWindowTitle()
        {
            const int nChars = 256;
            var sb = new StringBuilder(nChars);
            var handle = GetForegroundWindow();

            if (GetWindowText(handle, sb, nChars) > 0)
            {
                return sb.ToString();
            }
            return string.Empty;
        }
    }
}
