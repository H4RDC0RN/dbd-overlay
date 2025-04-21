using DBDOverlay.Core.Extensions;
using DBDOverlay.Core.MapOverlay;
using DBDOverlay.Core.MapOverlay.Languages;
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

        private readonly Dictionary<string, ReshadeHotKey> hotKeys = new Dictionary<string, ReshadeHotKey>();
        private IniFile ini;
        private readonly string generalSection = "GENERAL";
        private readonly string keysField = "PresetShortcutKeys";
        private readonly string filtersField = "PresetShortcutPaths";

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
            ini = new IniFile(path);
            Keys = GetKeys();
            Filters = GetFilters();
            foreach (string map in MapNamesContainer.GetReshadeMapsList())
            {
                hotKeys.Add(map, null);
            }
        }

        public void ApplyFilter(MapInfo mapInfo)
        {
            var hotKey = hotKeys.FirstOrDefault(x => mapInfo.FullName.Contains(x.Key)).Value;
            hotKey?.Press();
        }

        public void AddHotKey(string map, string filter)
        {
            hotKeys[map] = Keys[Filters.IndexOf(filter)];
        }

        private List<ReshadeHotKey> GetKeys()
        {
            var values = ini.Read(keysField, generalSection).Split(',');
            var keys = new List<ReshadeHotKey>();
            for (int i = 0; i < values.Length; i += 4)
            {
                keys.Add(new ReshadeHotKey((Keys)values[i].ToInt(), values[i + 1].ToBool(), values[i + 2].ToBool(), values[i + 3].ToBool()));
            }
            return keys;
        }

        private List<string> GetFilters()
        {
            return ini.Read(filtersField, generalSection).Split(',').Select(x => x.RegexMatch(@"(?<=\\)(?:.(?!\\))+(?=\.ini)")).ToList();
        }
    }
}
