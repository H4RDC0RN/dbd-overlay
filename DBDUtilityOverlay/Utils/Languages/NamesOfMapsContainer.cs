using System.Collections;
using System.Globalization;

namespace DBDUtilityOverlay.Utils.Languages
{
    public static class NamesOfMapsContainer
    {
        public static readonly string Empty = "Empty";
        public static readonly string NotReady = "NotReady";

        private static List<string> GetNamesOfMaps()
        {
            var resources = MapImages.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            if (resources == null)
            {
                Logger.Log.Warn("MapImages resources are null");
                return [];
            }
            else
            {
                var nullableList = resources.Cast<DictionaryEntry>().Select(x => x.Key.ToString()).Where(x => x != null && !x.Equals(Empty) && !x.Equals(NotReady)).ToList();
                nullableList.Sort();
                List<string> result = [];
                foreach (var x in nullableList)
                {
                    if (x != null)
                    {
                        result.Add(x);
                    }
                }
                return result;
            }
        }

        private static readonly List<string> deu = [];
        private static readonly List<string> eng = GetNamesOfMaps();
        private static readonly List<string> spa = [];
        private static readonly List<string> fra = [];
        private static readonly List<string> ita = [];
        private static readonly List<string> pol = [];
        private static readonly List<string> por = [];
        private static readonly List<string> tur = [];
        private static readonly List<string> rus =
        [
            "СВАЛКА_АВТОХЕВЕН.МЕСТО_УПОКОЕНИЯ_АЗАРОВА",
            "СВАЛКА_АВТОХЕВЕН.КРОВАВАЯ_ЛАЧУГА",
            "СВАЛКА_АВТОХЕВЕН.ГЭС_ХЕВЕН",
            "СВАЛКА_АВТОХЕВЕН.ЗОНА_СВАЛКИ",
            "СВАЛКА_АВТОХЕВЕН.МАГАЗИН_БАРАХОЛЬЩИКА"
        ];

        private static readonly List<List<string>> maps = [deu, eng, spa, fra, ita, pol, por, tur, rus];
        private static readonly Dictionary<string, List<string>> mapsByLang = LanguagesManager.GetAbbreviations()
            .Zip(maps, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);

        public static string GetMapFileName(string mapName)
        {
            if (Properties.Settings.Default.Language.Equals(LanguagesManager.DefaultLanguage)) return mapName;
            var index = mapsByLang.FirstOrDefault(x => x.Key.Equals(Properties.Settings.Default.Language)).Value.IndexOf(mapName);
            return eng[index];
        }
    }
}
