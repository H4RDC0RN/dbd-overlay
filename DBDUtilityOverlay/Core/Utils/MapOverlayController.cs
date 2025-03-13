using DBDUtilityOverlay.Core.Extensions;
using DBDUtilityOverlay.Core.Languages;
using DBDUtilityOverlay.Core.Models;

namespace DBDUtilityOverlay.Core.Utils
{
    public static class MapOverlayController
    {
        private static string realm = string.Empty;
        private static string name = NamesOfMapsContainer.Empty;

        private static MapOverlay instance;

        public static MapOverlay Instance
        {
            get
            {
                instance ??= new MapOverlay();
                return instance;
            }
        }

        public static void ChangeMap(MapInfo mapInfo)
        {
            if (mapInfo != null)
            {
                realm = mapInfo.Realm;
                name = mapInfo.Name;
            }
            Instance.ChangeMapImageOverlay(mapInfo);
        }

        public static void SwitchMapVariationToNext()
        {
            var suffix = name[^2..];
            suffix = suffix.First().ToString().Equals("_") && suffix.Last().ToString().IsInt()
                ? $"_{Convert.ToInt32(suffix.Last().ToString()) + 1}" : "_2";

            var newName = suffix.Equals("_2") ? $"{name}{suffix}" : name.Replace(name[^2..], $"{suffix}");
            var mapInfo = new MapInfo(realm, newName);
            if (mapInfo.HasImage)
            {
                name = newName;
                Logger.Log.Info($"Switching map variation to '{name}'");
                Instance.ChangeMapImageOverlay(mapInfo);
            }
        }

        public static void SwitchMapVariationToPrevious()
        {
            var suffix = name[^2..];
            if (suffix.First().ToString().Equals("_") && suffix.Last().ToString().IsInt())
            {
                if (suffix.Last().ToString().Equals("2"))
                {
                    name = name.RemoveRegex(suffix);
                    Logger.Log.Info($"Switching map variation to '{name}'");
                    Instance.ChangeMapImageOverlay(new MapInfo(realm, name));
                }
                else
                {
                    name = name.Replace(suffix, $"_{Convert.ToInt32(suffix.Last().ToString()) - 1}");
                    Logger.Log.Info($"Switching map variation to '{name}'");
                    Instance.ChangeMapImageOverlay(new MapInfo(realm, name));
                }
            }
        }
    }
}
