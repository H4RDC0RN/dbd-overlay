using DBDOverlay.Core.Extensions;
using DBDOverlay.Core.MapOverlay;
using DBDOverlay.Core.MapOverlay.Languages;
using DBDOverlay.Core.Utils;
using DBDOverlay.Core.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DBDOverlay.Core.Reshade
{
    public class ReshadeManager
    {
        public List<ReshadeHotKey> Keys { get; private set; }
        public List<string> Filters { get; private set; }

        private Dictionary<string, ReshadeHotKey> hotKeys;
        private IniFile ini;
        private readonly string generalSection = "GENERAL";
        private readonly string keysFieldName = "PresetShortcutKeys";
        private readonly string filtersFieldName = "PresetShortcutPaths";

        private static ReshadeManager instance;

        public static ReshadeManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new ReshadeManager();
                return instance;
            }
        }

        public void Initialize(string path)
        {
            Logger.Info($"Initializing file from '{path}'");
            ini = new IniFile(path);
            Keys = GetKeys();
            Filters = GetFilters();
            ResetHotKeys();
        }

        public void ApplyFilter(MapInfo mapInfo)
        {
            var hotKey = hotKeys.FirstOrDefault(x => mapInfo.FullName.Contains(x.Key)).Value;
            hotKey?.Press();
        }

        public void AddHotKey(string map, string filter)
        {
            var filterIndex = Filters.IndexOf(filter);
            AddHotKey(map, filterIndex);
            Logger.Info($"New filter is mapped: Map = {map}, Filter = {filter}");
        }

        public void AddHotKey(string map, int filterIndex)
        {
            hotKeys[map] = Keys[filterIndex];
            MappingsHandler.AddEntry(hotKeys.Keys.ToList().IndexOf(map), filterIndex);
        }

        public void ResetHotKeys()
        {
            hotKeys = new Dictionary<string, ReshadeHotKey>();
            foreach (var map in MapNamesContainer.GetReshadeMapsList())
            {
                hotKeys.Add(map, null);
            }
        }

        private List<ReshadeHotKey> GetKeys()
        {
            var keys = new List<ReshadeHotKey>();
            var keysFieldValue = ini.Read(keysFieldName, generalSection);
            if (!keysFieldValue.Equals(string.Empty))
            {
                var values = ini.Read(keysFieldName, generalSection).Split(',');
                for (int i = 0; i < values.Length; i += 4)
                {
                    keys.Add(new ReshadeHotKey((Keys)values[i].ToInt(), values[i + 1].ToBool(), values[i + 2].ToBool(), values[i + 3].ToBool()));
                }
            }
            Logger.Info($"Reshade keys: {keys}");
            return keys;
        }

        private List<string> GetFilters()
        {
            var filtersFieldValue = ini.Read(filtersFieldName, generalSection);
            if (filtersFieldValue.Equals(string.Empty)) return new List<string>();
            return ini.Read(filtersFieldName, generalSection).Split(',').Select(x => x.RegexMatch(@"(?<=\\)(?:.(?!\\))+(?=\.ini)")).ToList();
        }
    }
}
