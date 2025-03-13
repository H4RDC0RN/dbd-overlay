using DBDUtilityOverlay.Core.Enums;
using DBDUtilityOverlay.Core.Windows;
using System.Windows.Input;

namespace DBDUtilityOverlay.Core.Utils
{
    public static class HotKeysController
    {
        public static void RegisterHotKey(HotKeyType hotKeyType, ModifierKeys modifier, Keys key)
        {
            KeyboardHook.Instance.RegisterHotKey((int)hotKeyType, modifier, key, GetHotKeyAction(hotKeyType));
        }

        public static void UnregisterHotKey(HotKeyType hotKeyType)
        {
            KeyboardHook.Instance.UnregisterHotKey((int)hotKeyType);
        }

        public static void UpdateHotKey(HotKeyType hotKeyType, ModifierKeys modifier, Keys key)
        {
            UnregisterHotKey(hotKeyType);
            RegisterHotKey(hotKeyType, modifier, key);
        }

        public static void UnregisterAllHotKeys()
        {
            KeyboardHook.Instance.Dispose();
        }

        private static Action<object, KeyPressedEventArgs> GetHotKeyAction(HotKeyType hotKeyType)
        {
            return hotKeyType switch
            {
                HotKeyType.Read => PressedRead,
                HotKeyType.NextMap => PressedNext,
                HotKeyType.PreviousMap => PressedPrevious,
                _ => null,
            };
        }

        private static void PressedRead(object sender, KeyPressedEventArgs e)
        {
            e.Log("Read map");
            MapOverlayController.ChangeMap(ScreenshotRecognizer.GetMapInfo());
        }

        private static void PressedNext(object sender, KeyPressedEventArgs e)
        {
            e.Log("Next map");
            MapOverlayController.SwitchMapVariationToNext();
        }

        private static void PressedPrevious(object sender, KeyPressedEventArgs e)
        {
            e.Log("Previous map");
            MapOverlayController.SwitchMapVariationToPrevious();
        }
    }
}
