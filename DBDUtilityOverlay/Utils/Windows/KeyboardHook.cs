using System.Runtime.InteropServices;
using System.Windows.Input;

namespace DBDUtilityOverlay.Utils.Windows
{
    public sealed class KeyboardHook : IDisposable
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private int _currentId = 9000;
        private readonly Window _window = new();

        private class Window : NativeWindow, IDisposable
        {
            private static readonly int WM_HOTKEY = 0x0312;
            public Dictionary<int, EventHandler<KeyPressedEventArgs>> KeyPressedEvents = [];

            public Window()
            {
                CreateHandle(new CreateParams());
            }

            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);

                if (m.Msg == WM_HOTKEY)
                {
                    var key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);
                    var modifier = (ModifierKeys)((int)m.LParam & 0xFFFF);
                    var id = (int)m.WParam & 0xFFFF;

                    KeyPressedEvents.FirstOrDefault(x => x.Key.Equals(id)).Value.Invoke(this, new KeyPressedEventArgs(modifier, key));
                }
            }

            public void Dispose()
            {
                DestroyHandle();
            }
        }

        public void RegisterHotKey(ModifierKeys modifier, Keys key, Action<object, KeyPressedEventArgs> action)
        {
            _currentId++;
            RegisterHotKey(_window.Handle, _currentId, (uint)modifier, (uint)key);
            _window.KeyPressedEvents.Add(_currentId, delegate (object sender, KeyPressedEventArgs args)
            {
                new EventHandler<KeyPressedEventArgs>(action).Invoke(this, args);
            });
        }

        public void Dispose()
        {
            _window.KeyPressedEvents.Keys.ToList().ForEach(x => UnregisterHotKey(_window.Handle, x));
            _window.Dispose();
        }
    }
}
