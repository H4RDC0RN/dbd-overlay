using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input;

namespace DBDUtilityOverlay.Utils.Windows
{
    public sealed class KeyboardHook : IDisposable
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        
        [DllImport("user32.dll")]
        public static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        public static extern uint MapVirtualKey(uint uCode);

        [DllImport("user32.dll")]
        public static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[] lpKeyState,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszBuff, int cchBuff, uint wFlags, IntPtr dwhkl);

        private readonly HotKeyWindow _window = new();
        private static KeyboardHook instance;

        public static KeyboardHook Instance
        {
            get
            {
                instance ??= new KeyboardHook();
                return instance;
            }
        }

        public void RegisterHotKey(int id, ModifierKeys modifier, Keys key, Action<object, KeyPressedEventArgs> action)
        {
            RegisterHotKey(_window.Handle, id, (uint)modifier, (uint)key);
            _window.KeyPressedEvents.Add(id, delegate (object sender, KeyPressedEventArgs args)
            {
                new EventHandler<KeyPressedEventArgs>(action).Invoke(this, args);
            });
        }

        public void UnregisterHotKey(int id)
        {
            UnregisterHotKey(_window.Handle, id);
            _window?.KeyPressedEvents.Remove(id);
        }

        public void Dispose()
        {
            _window.KeyPressedEvents.Keys.ToList().ForEach(UnregisterHotKey);
            _window?.KeyPressedEvents.Clear();
            _window.Dispose();
        }

        public char GetCharFromKey(Keys virtualKey)
        {
            var keyboardState = new byte[256];
            GetKeyboardState(keyboardState);
            uint scanCode = MapVirtualKey((uint)virtualKey);
            var stringBuilder = new StringBuilder(2);
            var result = ToUnicodeEx((uint)virtualKey, scanCode, keyboardState, stringBuilder, 5, 0, 1033);

            if (result != 0 && result != -1) return stringBuilder[0];
            else return ' ';
        }
    }
}
