namespace DBDUtilityOverlay.Utils.Languages
{
    public static class LanguagesManager
    {
        public static readonly string DefaultLanguage = "eng";
        private static readonly List<string> langAbbs = ["deu", "eng", "spa", "fra", "ita", "pol", "por", "tur", "rus"];
        private static readonly List<string> langNames = ["Deutsch", "English", "Español", "Français", "Italiano", "Polski", "Português", "Türkçe", "Русский"];
        private static readonly Dictionary<string, string> dictionary = langNames.Zip(langAbbs, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);

        public static string GetValue(string? key)
        {
            return dictionary.FirstOrDefault(x => x.Key.Equals(key)).Value;
        }

        public static int GetCurrentLanguageIndex()
        {
            return dictionary.Select(x => x.Value).ToList().IndexOf(Properties.Settings.Default.Language);
        }

        public static Dictionary<string, string> GetOrderedKeyValuePairs(List<string> values)
        {
            return dictionary.Where(x => values.Contains(x.Value)).ToDictionary().OrderBy(x => x.Key).ToDictionary();
        }

        public static List<string> GetAbbreviations()
        {
            return langAbbs;
        }

        public static List<string> GetLanguages()
        {
            return langNames;
        }
    }
}
