using DBDOverlay.Core.MapOverlay;
using DBDOverlay.Core.Utils;
using DBDOverlay.Properties;
using System.Windows.Forms;
using System.Windows.Input;

namespace DBDOverlay.Core.Hotkeys
{
    public static class HotKeysController
    {
        public static void RegisterAllHotKeys()
        {
            var readModifier = (ModifierKeys)Settings.Default.ReadModifier;
            var readKey = (Keys)Settings.Default.ReadKey;
            var nextModifier = (ModifierKeys)Settings.Default.NextMapModifier;
            var nextKey = (Keys)Settings.Default.NextMapKey;
            var previousModifier = (ModifierKeys)Settings.Default.PreviousMapModifier;
            var previousKey = (Keys)Settings.Default.PreviousMapKey;

            KeyboardHook.Instance.RegisterHotKey((int)HotKeyType.Read, readModifier, readKey, PressedRead);
            KeyboardHook.Instance.RegisterHotKey((int)HotKeyType.NextMap, nextModifier, nextKey, PressedNext);
            KeyboardHook.Instance.RegisterHotKey((int)HotKeyType.PreviousMap, previousModifier, previousKey, PressedPrevious);
        }

        public static void UnregisterAllHotKeys()
        {
            KeyboardHook.Instance.UnregisterAllHotKeys();
        }

        public static void Dispose()
        {
            KeyboardHook.Instance.Dispose();
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
