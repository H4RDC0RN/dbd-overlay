using DBDOverlay.Core.Extensions;
using DBDOverlay.Properties;
using System.Collections.Generic;
using System.Linq;

namespace DBDOverlay.Core.Reshade
{
    public static class MappingsHandler
    {
        public static void AddEntry(int mapIndex, int filterIndex)
        {
            var newEntry = $"{mapIndex}-{filterIndex}";
            var mappingsString = Settings.Default.ReshadeMappings;
            if (mappingsString.Equals(string.Empty))
            {
                Settings.Default.ReshadeMappings = newEntry;
                Settings.Default.Save();
                return;
            }
            var mappings = ToMappings(mappingsString);
            var mappingIndex = mappings.FindIndex(x => x.MapIndex == mapIndex);
            if (mappingIndex != -1)
            {
                var entry = mappings[mappingIndex].Entry;
                if (!entry.Equals(newEntry))
                {
                    Settings.Default.ReshadeMappings = mappingsString.ReplaceRegex(entry, newEntry);
                    Settings.Default.Save();
                }
            }
            else
            {
                Settings.Default.ReshadeMappings = $"{mappingsString},{newEntry}";
                Settings.Default.Save();
            }
        }

        public static int GetFilterIndex(int mapIndex)
        {
            var mappingsString = Settings.Default.ReshadeMappings;
            if (mappingsString.Equals(string.Empty)) return -1;
            var mapping = ToMappings(mappingsString).FirstOrDefault(x => x.MapIndex == mapIndex);
            if (mapping == null)
                return -1;
            else
                return mapping.FilterIndex;
        }

        private static List<Mapping> ToMappings(string mappingsString)
        {
            return mappingsString.Split(',').Select(x => new Mapping(x)).ToList();
        }
    }
}
