using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;

namespace DBDOverlay.Core.Hotkeys
{
    public class HotKeyWindow : NativeWindow, IDisposable
    {
        private static readonly int WM_HOTKEY = 0x0312;
        public Dictionary<int, EventHandler<KeyPressedEventArgs>> KeyPressedEvents = new Dictionary<int, EventHandler<KeyPressedEventArgs>>();

        public HotKeyWindow()
        {
            CreateHandle(new CreateParams());
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_HOTKEY)
            {
                var key = (Keys)((int)m.LParam >> 16 & 0xFFFF);
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
}
