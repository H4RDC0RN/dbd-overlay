using DBDOverlay.Core.Utils;
using DBDOverlay.Core.WindowControllers.MapOverlay;
using DBDOverlay.Core.WindowControllers.MapOverlay.Languages;
using DBDOverlay.Properties;
using System.Collections.Generic;
using System.Linq;

namespace DBDOverlay.Core.Reshade
{
    public class ReshadeManager
    {
        public List<string> Filters { get; private set; }

        private Dictionary<string, string> mapFilterPairs;

        private static ReshadeManager instance;

        public static ReshadeManager Instance
        {
            get
            {
                if (instance == null) instance = new ReshadeManager();
                return instance;
            }
        }

        public void Initialize()
        {
            var path = Settings.Default.ReshadeFiltersPath;
            if (path.Equals(string.Empty)) return;
            Logger.Info($"Initializing filters from '{path}'");
            Filters = GetFilters(path);
            ClearMapFilterPairs();
        }

        public void SetMapFilterPairs()
        {
            if (!Settings.Default.ReshadeFiltersPath.Equals(string.Empty))
            {
                Initialize();
                var maps = MapNamesContainer.GetReshadeMapsList();
                for (int mapIndex = 0; mapIndex < maps.Count; mapIndex++)
                {
                    var filterIndex = MappingsHandler.GetFilterIndex(mapIndex);
                    if (filterIndex != -1) AddFilterMapPair(maps[mapIndex], filterIndex);
                }
            }
            else
            {
                ClearMapFilterPairs();
            }
        }

        public void ApplyFilter(MapInfo mapInfo)
        {
            if (mapInfo == null) return;
            var filterName = mapFilterPairs.FirstOrDefault(x => mapInfo.FullName.Contains(x.Key)).Value;
            if (filterName == null) return;

            var filtersPath = Settings.Default.ReshadeFiltersPath;
            var from = $"{filtersPath}{filterName}";
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
            return FileSystem.IniExists($"{Settings.Default.ReshadeFiltersPath}{name}");
        }

        private List<string> GetFilters(string path)
        {
            if (path.Equals(string.Empty)) return null;
            return FileSystem.GetIniFiles(path);
        }
    }
}
