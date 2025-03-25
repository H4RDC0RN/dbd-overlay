using DBDOverlay.Core.MapOverlay;
using DBDOverlay.Core.Utils;
using System.Windows.Input;

namespace DBDOverlay.Core.Hotkeys
{
    public static class HotKeysController
    {
        public static void RegisterAllHotKeys()
        {
            Logger.Log.Info("Register all hotkeys");
            var readModifier = (ModifierKeys)Properties.Settings.Default.ReadModifier;
            var readKey = (Keys)Properties.Settings.Default.ReadKey;
            var nextModifier = (ModifierKeys)Properties.Settings.Default.NextMapModifier;
            var nextKey = (Keys)Properties.Settings.Default.NextMapKey;
            var previousModifier = (ModifierKeys)Properties.Settings.Default.PreviousMapModifier;
            var previousKey = (Keys)Properties.Settings.Default.PreviousMapKey;

            KeyboardHook.Instance.RegisterHotKey((int)HotKeyType.Read, readModifier, readKey, PressedRead);
            KeyboardHook.Instance.RegisterHotKey((int)HotKeyType.NextMap, nextModifier, nextKey, PressedNext);
            KeyboardHook.Instance.RegisterHotKey((int)HotKeyType.PreviousMap, previousModifier, previousKey, PressedPrevious);
        }

        public static void UnregisterAllHotKeys()
        {
            Logger.Log.Info("Unregister all hotkeys");
            KeyboardHook.Instance.UnregisterAllHotKeys();
        }

        public static void Dispose()
        {
            Logger.Log.Info("Dispose hotkeys window");
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
