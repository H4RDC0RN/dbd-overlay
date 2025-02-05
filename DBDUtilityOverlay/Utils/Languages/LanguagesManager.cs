using System.Windows.Input;

namespace DBDUtilityOverlay.Utils.Languages
{
    public static class LanguagesManager
    {
        public static readonly string DefaultLanguage = "eng";
        public static readonly string SpaAbb = "spa";
        public static readonly string MexAbb = "mex";
        private static readonly List<string> langAbbs = ["deu", DefaultLanguage, SpaAbb, MexAbb, "fra", "ita", "pol", "por", "tur", "rus"];
        private static readonly List<string> langNames =
        [
            "Deutsch",
            "English",
            "Español",
            "Español (México)",
            "Français",
            "Italiano",
            "Polski",
            "Português",
            "Türkçe",
            "Русский"
        ];
        private static readonly Dictionary<string, string> langsDictionary = langNames.Zip(langAbbs, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);
        private static readonly List<string> mapInfoLocales =
        [
            "KARTENINFO",
            "MAP INFO",
            "INFORMACIÓN DEL MAPA",
            "INFORMACIÓN DEL MAPA",
            "INFOS CARTE",
            "INFO MAPPA",
            "INFORMACJE O MAPIE",
            "Português",
            "Türkçe",
            "Русский"
        ];
        private static readonly Dictionary<string, string> mapsInfoDictionary = langAbbs.Zip(mapInfoLocales, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);

        public static string GetValue(string? key)
        {
            return langsDictionary.FirstOrDefault(x => x.Key.Equals(key)).Value;
        }

        public static int GetCurrentLanguageIndex()
        {
            return langsDictionary.Select(x => x.Value).ToList().IndexOf(Properties.Settings.Default.Language);
        }

        public static Dictionary<string, string> GetOrderedKeyValuePairs(List<string> values)
        {
            return langsDictionary.Where(x => values.Contains(x.Value)).ToDictionary().OrderBy(x => x.Key).ToDictionary();
        }

        public static List<string> GetAbbreviations()
        {
            return langAbbs;
        }

        public static List<string> GetLanguages()
        {
            return langNames;
        }

        public static string GetMapInfoLocale()
        {
            return mapsInfoDictionary.FirstOrDefault(x => x.Key.Equals(Properties.Settings.Default.Language)).Value;
        }
    }
}
