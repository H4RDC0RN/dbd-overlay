using DBDOverlay.Core.Hotkeys;
using System.Runtime.InteropServices;
using System.Text;

namespace DBDOverlay.Core.Windows
{
    public class WindowsServices
    {
        private readonly WinEventDelegate winEventDelegate;
        private static WindowsServices instance;

        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int GWL_EXSTYLE = -20;
        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_SYSTEM_FOREGROUND = 3;
        private readonly string dbdWindowName = "DeadByDaylight";

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
                instance = instance ?? new WindowsServices();
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
        static extern int GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(int hWnd, StringBuilder text, int count);

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

        public void HandleWindowEvent(int hWinEventHook, uint eventType,
            int hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (GetActiveWindowTitle().Contains(dbdWindowName)) HotKeysController.RegisterAllHotKeys();
            else HotKeysController.UnregisterAllHotKeys();
        }

        public void SetWindowExTransparent(int hwnd)
        {
            var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
        }

        public void RevertWindowExTransparent(int hwnd)
        {
            var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle * WS_EX_TRANSPARENT);
        }
    }
}
