using DBDOverlay.Properties;
using System.Collections.Generic;
using System.Linq;

namespace DBDOverlay.Core.MapOverlay.Languages
{
    public static class LanguagesManager
    {
        public static readonly string Deu = "deu";
        public static readonly string Eng = "eng";
        public static readonly string Spa = "spa";
        public static readonly string Mex = "mex";
        public static readonly string Fra = "fra";
        public static readonly string Ita = "ita";
        public static readonly string Pol = "pol";
        public static readonly string Por = "por";
        public static readonly string Tur = "tur";
        public static readonly string Rus = "rus";

        private static readonly List<string> langAbbs = new List<string>() { Deu, Eng, Spa, Mex, Fra, Ita, Pol, Por, Tur, Rus };
        private static readonly List<string> langNames = new List<string>()
        {
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
        };
        private static readonly Dictionary<string, string> langsDictionary = langNames.Zip(langAbbs, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);
        private static readonly List<string> mapInfoLocales = new List<string>()
        {
            "KARTENINFO",
            "MAP INFO",
            "INFORMACIÓN DEL MAPA",
            "INFORMACIÓN DEL MAPA",
            "INFOS CARTE",
            "INFO MAPPA",
            "INFORMACJE O MAPIE",
            "INFORMAÇÕES DO MAPA",
            "HARİTA AYRINTILARI",
            "О КАРТЕ"
        };
        private static readonly Dictionary<string, string> mapsInfoDictionary = langAbbs.Zip(mapInfoLocales, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);

        public static string GetValue(string key)
        {
            return langsDictionary.FirstOrDefault(x => x.Key.Equals(key)).Value;
        }

        public static int GetCurrentLanguageIndex()
        {
            return langsDictionary.Select(x => x.Value).ToList().IndexOf(Settings.Default.Language);
        }

        public static Dictionary<string, string> GetOrderedKeyValuePairs(List<string> values)
        {
            return langsDictionary.Where(x => values.Contains(x.Value)).ToDictionary(x => x.Key, y => y.Value).OrderBy(x => x.Key).ToDictionary(x => x.Key, y => y.Value);
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
            return mapsInfoDictionary.FirstOrDefault(x => x.Key.Equals(Settings.Default.Language)).Value;
        }

        public static string ConvertMexToSpa(string value)
        {
            return value.Equals(Mex) ? Spa : value;
        }
    }
}
