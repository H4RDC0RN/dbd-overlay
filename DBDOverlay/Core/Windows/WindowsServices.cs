using DBDOverlay.Core.Hotkeys;
using DBDOverlay.Core.WindowControllers.KillerOverlay;
using DBDOverlay.Core.WindowControllers.MapOverlay;
using DBDOverlay.Properties;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Application = System.Windows.Application;

namespace DBDOverlay.Core.Windows
{
    public class WindowsServices
    {
        public event EventHandler<EventArgs> MapOverlayMoveModeOff;
        public event EventHandler<EventArgs> KillerOverlayMoveModeOff;
        public bool IsMonitoringActive { get; private set; } = false;

        private readonly WinEventDelegate winEventDelegate;
        private static WindowsServices instance;

        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int GWL_EXSTYLE = -20;
        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_SYSTEM_FOREGROUND = 3;

        private readonly string[] dbdProcessNames = { "DeadByDaylight-Win64-Shipping", "DeadByDaylight-EGS-Shipping" };
        private readonly string appProcessName = "DBDOverlay";
        private readonly string appWindowName = "DBD Overlay";
        private readonly string mapOverlayWindowName = "Map overlay";
        private readonly string killerOverlayWindowName = "Killer Overlay";

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
                if (instance == null) instance = new WindowsServices();
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
        static extern uint GetWindowThreadProcessId(int hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        static extern int GetWindowText(int hWnd, StringBuilder text, int count);

        public void HandleWindowEvent(int hWinEventHook, uint eventType,
            int hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            HandleMapOverlayMoveMode();
            HandleKillerOverlayMoveMode();
        }

        public bool IsAppWindow()
        {
            return GetActiveWindowTitle().Equals(appWindowName)
                || GetActiveWindowTitle().Equals(mapOverlayWindowName)
                || GetActiveWindowTitle().Equals(killerOverlayWindowName);
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

        public void CloseRedundantProcesses()
        {
            var processes = Process.GetProcessesByName(appProcessName).OrderBy(x => x.StartTime).ToList();
            processes.RemoveAt(processes.Count - 1);
            processes.ForEach(x => x.Kill());
        }

        public void StartMonitoring(int interval = 200)
        {
            IsMonitoringActive = true;
            bool lastState = false;

            Task.Run(async () =>
            {
                while (IsMonitoringActive)
                {
                    var currentState = IsDBDActive();

                    if (currentState != lastState)
                    {
                        lastState = currentState;

                        _ = Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            HandleActiveWindow(currentState);
                        }));
                    }
                    await Task.Delay(interval);
                }
            });
        }

        public void StopMonitoring()
        {
            IsMonitoringActive = false;
        }

        public void CheckActiveWindow()
        {
            bool isActive = IsDBDActive();

            _ = Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                HandleActiveWindow(isActive);
            }));
        }

        private bool IsDBDActive()
        {
            string processName = null;

            var handle = GetForegroundWindow();
            if (handle != 0)
            {
                GetWindowThreadProcessId(handle, out uint pid);
                if (pid != 0)
                {
                    var process = Process.GetProcessById((int)pid);
                    processName = process.ProcessName;
                }
            }

            return dbdProcessNames.Contains(processName);
        }

        private void HandleActiveWindow(bool isActive)
        {
            HandleHotkeys(isActive);
            SetOverlaysVisible(!Settings.Default.IsHidingOverlaysMode || isActive);
        }

        private void HandleHotkeys(bool isDBDActive)
        {
            if (isDBDActive) HotKeysController.RegisterAllHotKeys();
            else HotKeysController.UnregisterAllHotKeys();
        }

        private void SetOverlaysVisible(bool isVisible)
        {
            KillerOverlayController.Overlay.SetOverlayVisible(isVisible);
            MapOverlayController.Overlay.SetOverlayVisible(isVisible);
        }

        private void HandleMapOverlayMoveMode()
        {
            if (MapOverlayController.Instance.CanBeMoved && !IsAppWindow())
            {
                MapOverlayMoveModeOff?.Invoke(this, new RoutedEventArgs());
            }
        }

        private void HandleKillerOverlayMoveMode()
        {
            if (KillerOverlayController.Instance.CanBeMoved && !IsAppWindow())
            {
                KillerOverlayMoveModeOff?.Invoke(this, new RoutedEventArgs());
            }
        }

        private string GetActiveWindowTitle()
        {
            const int nChars = 256;
            var sb = new StringBuilder(nChars);
            var handle = GetForegroundWindow();

            return (GetWindowText(handle, sb, nChars) > 0) ? sb.ToString() : string.Empty;
        }
    }
}
