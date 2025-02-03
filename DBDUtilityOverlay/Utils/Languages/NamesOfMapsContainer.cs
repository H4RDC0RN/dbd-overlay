using System.Collections;
using System.Globalization;

namespace DBDUtilityOverlay.Utils.Languages
{
    public static class NamesOfMapsContainer
    {
        public static readonly string Empty = "Empty";
        public static readonly string NotReady = "NotReady";
        private static readonly int maxLength = 45;

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
            "СВАЛКА_АВТОХЕВЕН.МАГАЗИН_БАРАХОЛЬЩИКА",
            "ГНИЛОЕ_БОЛОТО.ЧУДОВИЩНАЯ_КЛАДОВАЯ",
            "ГНИЛОЕ_БОЛОТО.БЛЕДНАЯ_РОЗА",
            "ФЕРМА_КОЛВИНД.РАЗРУШЕННЫЙ_ХЛЕВ",
            "ФЕРМА_КОЛВИНД.ЗАБРОШЕННАЯ_СКОТОБОЙНЯ",
            "ФЕРМА_КОЛВИНД.СГНИВШИЕ_ПОЛЯ",
            "ФЕРМА_КОЛВИНД.ДОМ_ТОМПСОНОВ",
            "ФЕРМА_КОЛВИНД.РУЧЕЙ_СТРАДАНИЙ",
            "ПСИХЛЕЧЕБНИЦА_КРОТУС_ПРЕНН.ОТДЕЛЕНИЕ_ДЛЯ_БУЙНЫХ",
            "ПСИХЛЕЧЕБНИЦА_КРОТУС_ПРЕНН.ЧАСОВНЯ_ОТЦА_КЭМПБЕЛЛА",
            "ЗАРОСЛИ_ДВАРКИ.КРУШЕНИЕ_«НОСТРОМО»",
            "ЗАРОСЛИ_ДВАРКИ.ПЛОЩАДКА_ТОБА",
            "ЗАБРОШЕННОЕ_КЛАДБИЩЕ.ВОРОНЬЕ_ГНЕЗДОВЬЕ",
            "МЯСОКОМБИНАТ_«ГИДЕОН».ИГРА",
            "МОГИЛА_В_ГЛЕНВЕЙЛЕ.САЛУН_«МЕРТВЯК_ВУСМЕРТЬ»",
            "ХЭДДОНФИЛД.ПЕРЕУЛОК_ЛЭМПКИН",
            "НАЦИОНАЛЬНАЯ_ЛАБОРАТОРИЯ_ХОУКИНСА.ПОДЗЕМНЫЙ_КОМ",
            "МЕМОРИАЛЬНЫЙ_ИНСТИТУТ_ЛЭРИ.ЛЕЧЕБНЫЙ_ЗАЛ",
            "ОРМОНД.КУРОРТ_«ГОРА_ОРМОНД»",
            "ОРМОНД.КУРОРТ_«ГОРА_ОРМОНД»_2",
            "ОРМОНД.КУРОРТ_«ГОРА_ОРМОНД»_3",
            "РАККУН-СИТИ.ВОСТОЧНОЕ_КРЫЛО_ПОЛИЦЕЙСКОГО_УЧАСТКА_РАККУН-СИТИ",
            "РАККУН-СИТИ.ЗАПАДНОЕ_КРЫЛО_ПОЛИЦЕЙСКОГО_УЧАСТКА_РАККУН-СИТИ",
            "КРАСНЫЙ_ЛЕС.ОБИТЕЛЬ_МАТЕРИ",
            "КРАСНЫЙ_ЛЕС.ХРАМ_ОЧИЩЕНИЯ",
            "САЙЛЕНТ_ХИЛЛ.НАЧАЛЬНАЯ_ШКОЛА_МИДВИЧА",
            "СПРИНГВУД.ДЕТСКИЙ_САД_БЭДХЕМ",
            "СПРИНГВУД.ДЕТСКИЙ_САД_БЭДХЕМА_II",
            "СПРИНГВУД.ДЕТСКИЙ_САД_БЭДХЕМА_III",
            "СПРИНГВУД.ДЕТСКИЙ_САД_БЭДХЕМА_IV",
            "СПРИНГВУД.ДЕТСКИЙ_САД_БЭДХЕМА_V",
            "РАЗОРЕННАЯ_ДЕРЕВУШКА-БОРГО.ЗАБЫТЫЕ_РУИНЫ",
            "РАЗОРЕННАЯ_ДЕРЕВУШКА-БОРГО.РАЗРУШЕННАЯ_ПЛОЩАДЬ",
            "ПОМЕСТЬЕ_МАКМИЛЛАН.УГОЛЬНАЯ_БАШНЯ",
            "ПОМЕСТЬЕ_МАКМИЛЛАН.УГОЛЬНАЯ_БАШНЯ_2",
            "ПОМЕСТЬЕ_МАКМИЛЛАН.СТОНУЩИЙ_СКЛАД",
            "ПОМЕСТЬЕ_МАКМИЛЛАН.СТОНУЩИЙ_СКЛАД_2",
            "ПОМЕСТЬЕ_МАКМИЛЛАН.ЗАВОД_СТРАДАНИЙ",
            "ПОМЕСТЬЕ_МАКМИЛЛАН.ЗАВОД_СТРАДАНИЙ_2",
            "ПОМЕСТЬЕ_МАКМИЛЛАН.ЛЕСНАЯ_ОПУШКА",
            "ПОМЕСТЬЕ_МАКМИЛЛАН.ЛЕСНАЯ_ОПУШКА_2",
            "ПОМЕСТЬЕ_МАКМИЛЛАН.УДУШАЮЩАЯ_ЯМА",
            "ПОМЕСТЬЕ_МАКМИЛЛАН.УДУШАЮЩАЯ_ЯМА_2",
            "ЗАЧАХШИЙ_ОСТРОВ.САД_СЧАСТЬЯ",
            "ЗАЧАХШИЙ_ОСТРОВ.ПЛОЩАДЬ_ГРИНВИЛЛА",
            "ПОМЕСТЬЕ_ЯМАОКИ.СЕМЕЙНАЯ_РЕЗИДЕНЦИЯ",
            "ПОМЕСТЬЕ_ЯМАОКИ.СЕМЕЙНАЯ_РЕЗИДЕНЦИЯ_2",
            "ПОМЕСТЬЕ_ЯМАОКИ.ПРИБЕЖИЩЕ_ГНЕВА",
            "ПОМЕСТЬЕ_ЯМАОКИ.ПРИБЕЖИЩЕ_ГНЕВА_2",
        ];

        private static readonly List<List<string>> maps = [deu, eng, spa, fra, ita, pol, por, tur, rus];
        private static readonly Dictionary<string, List<string>> mapsByLang = LanguagesManager.GetAbbreviations()
            .Zip(maps, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);

        public static string GetMapFileName(string mapName)
        {
            if (Properties.Settings.Default.Language.Equals(LanguagesManager.DefaultLanguage)) return mapName;
            var mapsList = mapsByLang.FirstOrDefault(x => x.Key.Equals(Properties.Settings.Default.Language)).Value;

            int index = -1;
            if (mapName.Length > maxLength)
            {
                mapName = mapName[..maxLength];
                index = mapsList.IndexOf(mapsList.First(x => x.StartsWith(mapName)));
            }
            else index = mapsList.IndexOf(mapName);

            if (index == -1)
            {
                Logger.Log.Warn($"No entry in dictionary for '{mapName}'");
                return mapName;
            }
            return eng[index];
        }
    }
}
