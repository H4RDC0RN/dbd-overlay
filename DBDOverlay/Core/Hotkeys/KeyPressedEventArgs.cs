using DBDOverlay.Core.Utils;
using System.Windows.Input;

namespace DBDOverlay.Core.Hotkeys
{
    public class KeyPressedEventArgs : EventArgs
    {
        public ModifierKeys Modifier { get; }
        public Keys Key { get; }

        internal KeyPressedEventArgs(ModifierKeys modifier, Keys key)
        {
            Modifier = modifier;
            Key = key;
        }

        public void Log(string hotKeyName)
        {
            Logger.Log.Info($"'{hotKeyName}' hotkey is pressed ({Modifier} + {Key})");
        }
    }
}
