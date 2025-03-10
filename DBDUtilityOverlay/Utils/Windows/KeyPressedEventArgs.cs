using System.Windows.Input;

namespace DBDUtilityOverlay.Utils.Windows
{
    public class KeyPressedEventArgs : EventArgs
    {
        internal KeyPressedEventArgs(ModifierKeys modifier, Keys key)
        {
            Modifier = modifier;
            Key = key;
        }

        public ModifierKeys Modifier { get; }
        public Keys Key { get; }

        public void Log(string hotKeyName)
        {
            Logger.Log.Info($"'{hotKeyName}' hotkey is pressed ({Modifier} + {Key})");
        }
    }
}
