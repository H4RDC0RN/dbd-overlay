using DBDOverlay.Core.Utils;
using DBDOverlay.Core.WindowControllers.MapOverlay;
using DBDOverlay.Core.WindowControllers.MapOverlay.Languages;
using DBDOverlay.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DBDOverlay.Core.Reshade
{
    public class ReshadeManager
    {
        public List<string> Filters { get; private set; }

        private Dictionary<string, string> mapFilterPairs;
        private string filtersPath;

        private static ReshadeManager instance;

        public static ReshadeManager Instance
        {
            get
            {
                if (instance == null) instance = new ReshadeManager();
                return instance;
            }
        }

        public void Initialize(string path)
        {
            if (!path.Equals(string.Empty)) Logger.Info($"Initializing filters from '{path}'");
            filtersPath = path;
            Filters = GetFilters(path);
            ClearMapFilterPairs();
        }

        public void ReloadFilters()
        {
            Initialize(filtersPath);
        }

        public void SetMapFilterPairs()
        {
            var path = Settings.Default.ReshadeFiltersPath;
            if (!path.Equals(string.Empty))
            {
                Initialize(path);
                var maps = MapNamesContainer.GetReshadeMapsList();
                for (int mapIndex = 0; mapIndex < maps.Count; mapIndex++)
                {
                    var filterIndex = MappingsHandler.GetFilterIndex(mapIndex);
                    if (filterIndex != -1) AddFilterMapPair(maps[mapIndex], filterIndex);
                }
            }
        }

        public void ApplyFilter(MapInfo mapInfo)
        {
            if (mapInfo == null) return;
            var filterName = mapFilterPairs.FirstOrDefault(x => mapInfo.FullName.Contains(x.Key)).Value;
            if (filterName == null) return;

            var from = $"{filtersPath}{mapFilterPairs.FirstOrDefault(x => mapInfo.FullName.Contains(x.Key)).Value}";
            var to = $"{filtersPath}{Settings.Default.MainFilterName}";
            FileSystem.CopyIniFile(from, to);
        }

        public void AddFilterMapPair(string map, string filter)
        {
            var filterIndex = Filters.IndexOf(filter);
            AddFilterMapPair(map, filterIndex);
            Logger.Info($"New filter is mapped: Map = {map}, Filter = {filter}");
        }

        public void AddFilterMapPair(string map, int filterIndex)
        {
            mapFilterPairs[map] = Filters[filterIndex];
            MappingsHandler.AddEntry(mapFilterPairs.Keys.ToList().IndexOf(map), filterIndex);
        }

        public void ClearMapFilterPairs()
        {
            mapFilterPairs = new Dictionary<string, string>();
            foreach (var map in MapNamesContainer.GetReshadeMapsList())
            {
                mapFilterPairs.Add(map, null);
            }
        }

        public bool FilterExists(string name)
        {
            return FileSystem.IniExists($"{filtersPath}{name}");
        }

        private List<string> GetFilters(string path)
        {
            if (path.Equals(string.Empty)) return null;
            return FileSystem.GetIniFiles(path);
        }
    }
}
