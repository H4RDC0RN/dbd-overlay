namespace DBDUtilityOverlay.Utils.Models
{
    public static class Languages
    {
        public static readonly Dictionary<string, string> Dictionary = new()
        {
            { "Deutsch", "deu" },
            { "English", "eng" },
            { "Español", "spa" },
            { "Français", "fra" },
            { "Italiano", "ita" },
            { "Polski", "pol" },
            { "Português", "por" },
            { "Türkçe", "tur" },
            { "Русский", "rus" }
        };

        public static string GetValue(string? key)
        {
            return Dictionary.FirstOrDefault(x => x.Key.Equals(key)).Value;
        }

        public static int GetCurrentLanguageIndex()
        {
            return Dictionary.Select(x => x.Value).ToList().IndexOf(Properties.Settings.Default.Language);
        }
    }
}
