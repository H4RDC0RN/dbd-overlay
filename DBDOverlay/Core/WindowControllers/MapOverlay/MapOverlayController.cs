using DBDOverlay.Core.BackgroundProcesses;
using DBDOverlay.Core.Extensions;
using DBDOverlay.Core.Reshade;
using DBDOverlay.Core.Utils;
using DBDOverlay.UI.Windows.Overlays;
using System.Linq;

namespace DBDOverlay.Core.WindowControllers.MapOverlay
{
    public class MapOverlayController
    {
        public bool CanBeMoved { get; set; } = false;
        private string realm = string.Empty;
        private string name = string.Empty;
        private readonly int suffixLength = 2;

        private static MapOverlayController instance;
        private static MapOverlayWindow overlay;

        public static MapOverlayController Instance
        {
            get
            {
                if (instance == null)
                    instance = new MapOverlayController();
                return instance;
            }
        }

        public static MapOverlayWindow Overlay
        {
            get
            {
                if (overlay == null)
                    overlay = new MapOverlayWindow();
                return overlay;
            }
        }

        public void ChangeMap(MapInfo mapInfo)
        {
            if (CanMapOverlayBeApplied(mapInfo))
            {
                realm = mapInfo.Realm;
                name = mapInfo.Name;
                Overlay.ChangeMapImageOverlay(mapInfo);
                if (AutoMode.Instance.IsActive) AutoMode.Instance.IsMapNameMode = false;
            }
            ReshadeManager.Instance.ApplyFilter(mapInfo);
        }

        public bool CanMapOverlayBeApplied(MapInfo mapInfo)
        {
            return mapInfo != null && !name.Equals(mapInfo.Name);
        }

        public void SwitchMapVariationToNext()
        {
            var suffix = name.GetLast(suffixLength);
            suffix = suffix.First().ToString().Equals("_") && suffix.Last().ToString().IsInt()
                ? $"_{suffix.Increment()}" : "_2";

            var newName = suffix.Equals("_2") ? $"{name}{suffix}" : name.Replace(name.GetLast(suffixLength), $"{suffix}");
            var mapInfo = new MapInfo(realm, newName);
            if (mapInfo.HasImage)
            {
                name = newName;
                Logger.Info($"Switching map variation to '{name}'");
                Overlay.ChangeMapImageOverlay(mapInfo);
            }
        }

        public void SwitchMapVariationToPrevious()
        {
            var suffix = name.GetLast(suffixLength);
            if (suffix.First().ToString().Equals("_") && suffix.Last().ToString().IsInt())
            {
                if (suffix.Last().ToString().Equals("2"))
                {
                    name = name.RemoveRegex(suffix);
                    Logger.Info($"Switching map variation to '{name}'");
                    Overlay.ChangeMapImageOverlay(new MapInfo(realm, name));
                }
                else
                {
                    name = name.Replace(suffix, $"_{suffix.Decrement()}");
                    Logger.Info($"Switching map variation to '{name}'");
                    Overlay.ChangeMapImageOverlay(new MapInfo(realm, name));
                }
            }
        }

        public void HandleNewMapRecognized(object sender, NewMapEventArgs e)
        {
            e.Log();
            ChangeMap(e.MapInfo);
        }
    }
}
