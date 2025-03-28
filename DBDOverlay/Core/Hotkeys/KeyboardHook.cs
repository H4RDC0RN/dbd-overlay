using DBDOverlay.Core.Utils;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;

namespace DBDOverlay.Core.Hotkeys
{
    public sealed class KeyboardHook : IDisposable
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(int hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(int hWnd, int id);

        [DllImport("user32.dll")]
        public static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        public static extern uint MapVirtualKey(uint uCode);

        [DllImport("user32.dll")]
        public static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[] lpKeyState,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszBuff, int cchBuff, uint wFlags, int dwhkl);

        private readonly HotKeyWindow _window = new HotKeyWindow();
        private static KeyboardHook instance;

        public static KeyboardHook Instance
        {
            get
            {
                if (instance == null)
                    instance = new KeyboardHook();
                return instance;
            }
        }

        public void RegisterHotKey(int id, ModifierKeys modifier, Keys key, Action<object, KeyPressedEventArgs> action)
        {
            if (_window.KeyPressedEvents.ContainsKey(id))
            {
                Logger.Log.Warn($"Hot key with id '{id}' already exists");
                Logger.Log.Warn($"Modifier = '{id}',  key = '{key}'");
                return;
            }

            RegisterHotKey(_window.Handle.ToInt32(), id, (uint)modifier, (uint)key);
            _window.KeyPressedEvents.Add(id, delegate (object sender, KeyPressedEventArgs args)
            {
                new EventHandler<KeyPressedEventArgs>(action).Invoke(this, args);
            });
        }

        public void UnregisterHotKey(int id)
        {
            UnregisterHotKey(_window.Handle.ToInt32(), id);
            _window?.KeyPressedEvents.Remove(id);
        }

        public void UnregisterAllHotKeys()
        {
            _window.KeyPressedEvents.Keys.ToList().ForEach(UnregisterHotKey);
            _window?.KeyPressedEvents.Clear();
        }

        public void Dispose()
        {
            _window.Dispose();
        }

        public char GetCharFromKey(Keys virtualKey)
        {
            var keyboardState = new byte[256];
            GetKeyboardState(keyboardState);
            uint scanCode = MapVirtualKey((uint)virtualKey);
            var stringBuilder = new StringBuilder(2);
            var engLayout = 1033;
            var result = ToUnicodeEx((uint)virtualKey, scanCode, keyboardState, stringBuilder, 5, 0, engLayout);

            if (result != 0 && result != -1) return stringBuilder[0];
            else return ' ';
        }
    }
}
