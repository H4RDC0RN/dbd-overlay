﻿using DBDOverlay.Core.Utils;
using System;
using System.Windows.Forms;
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
            Logger.Info($"'{hotKeyName}' hotkey is pressed ({Modifier} + {Key})");
        }
    }
}
