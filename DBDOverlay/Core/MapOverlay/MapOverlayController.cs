﻿using DBDOverlay.Core.Extensions;
using DBDOverlay.Core.Languages;
using DBDOverlay.Core.Utils;
using System.Linq;

namespace DBDOverlay.Core.MapOverlay
{
    public static class MapOverlayController
    {
        private static string realm = string.Empty;
        private static string name = NamesOfMapsContainer.Empty;
        private static readonly int suffixLength = 2;

        private static MapOverlayWindow instance;

        public static MapOverlayWindow Instance
        {
            get
            {
                if (instance == null)
                    instance = new MapOverlayWindow();
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
            var suffix = name.GetLast(suffixLength);
            suffix = suffix.First().ToString().Equals("_") && suffix.Last().ToString().IsInt()
                ? $"_{suffix.Increment()}" : "_2";

            var newName = suffix.Equals("_2") ? $"{name}{suffix}" : name.Replace(name.GetLast(suffixLength), $"{suffix}");
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
            var suffix = name.GetLast(suffixLength);
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
                    name = name.Replace(suffix, $"_{suffix.Decrement()}");
                    Logger.Log.Info($"Switching map variation to '{name}'");
                    Instance.ChangeMapImageOverlay(new MapInfo(realm, name));
                }
            }
        }
    }
}
