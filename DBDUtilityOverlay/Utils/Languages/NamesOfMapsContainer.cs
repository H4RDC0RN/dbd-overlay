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

        private static readonly List<string> deu =
        [
            "AUTOHAVEN-SCHROTTPLATZ.AZAROVS_RUHESTÄTTE",
            "AUTOHAVEN-SCHROTTPLATZ.BLUTHÜTTE",
            "AUTOHAVEN-SCHROTTPLATZ.SPRITHIMMEL",
            "AUTOHAVEN-SCHROTTPLATZ.SCHROTTPLATZ",
            "AUTOHAVEN-SCHROTTPLATZ.WERKSTATT",
            "BACKWATER-SUMPF.SCHAURIGE_SPEISEKAMMER",
            "BACKWATER-SUMPF.DIE_PALE_ROSE",
            "COLDWIND_FARM.VERFALLENER_KUHSTALL",
            "COLDWIND_FARM.WIDERLICHES_SCHLACHTHAUS",
            "COLDWIND_FARM.FAULIGE_FELDER",
            "COLDWIND_FARM.DAS_THOMPSON-HAUS",
            "COLDWIND_FARM.TAL_DER_QUAL",
            "CROTUS-PRENN-ANSTALT.GESTÖRTENABTEILUNG",
            "CROTUS-PRENN-ANSTALT.PATER_CAMPBELLS_KAPELLE",
            "DVARKA-DSCHUNGEL.NOSTROMO-WRACK",
            "DVARKA-DSCHUNGEL.TOBA-LANDEPLATZ",
            "EINSAMER_FRIEDHOF.KRÄHENHORST",
            "GIDEON-FLEISCHFABRIK.DAS_SPIEL",
            "GRAB_VON_GLENVALE.DEAD_DAWG_SALOON",
            "HADDONFIELD.LAMPKIN_LANE",
            "HAWKINS_NATIONAL_LABORATORY.DER_UNTERGRUNDKOMPLEX",
            "LERYS_GEDENKINSTITUT.BEHANDLUNGSBEREICH",
            "ORMOND.SKIORT_AM_MOUNT_ORMOND",
            "ORMOND.SKIORT_AM_MOUNT_ORMOND_2",
            "ORMOND.SKIORT_AM_MOUNT_ORMOND_3",
            "RACCOON_CITY.OSTFLÜGEL_DER_POLIZEISTATION_VON_RACCOON_CITY",
            "RACCOON_CITY.WESTFLÜGEL_DER_POLIZEISTATION_VON_RACCOON_CITY",
            "ROTER_WALD.MUTTERS_BEHAUSUNG",
            "ROTER_WALD.DER_TEMPEL_DER_REINIGUNG",
            "SILENT_HILL.MIDWICH_ELEMENTARY_SCHOOL",
            "SPRINGWOOD.BADHAM-VORSCHULE",
            "SPRINGWOOD.BADHAM-VORSCHULE_II",
            "SPRINGWOOD.BADHAM-VORSCHULE_III",
            "SPRINGWOOD.BADHAM-VORSCHULE_IV",
            "SPRINGWOOD.BADHAM-VORSCHULE_V",
            "DAS_DEZIMIERTE_BORGO.VERGESSENE_RUINEN",
            "DAS_DEZIMIERTE_BORGO.DER_ZERSTÖRTE_PLATZ",
            "DAS_MACMILLAN-ANWESEN.KOHLELAGER",
            "DAS_MACMILLAN-ANWESEN.KOHLELAGER_2",
            "DAS_MACMILLAN-ANWESEN.SEUFZENDES_LAGERHAUS",
            "DAS_MACMILLAN-ANWESEN.SEUFZENDES_LAGERHAUS_2",
            "DAS_MACMILLAN-ANWESEN.EISENWERK_DER_QUAL",
            "DAS_MACMILLAN-ANWESEN.EISENWERK_DER_QUAL_2",
            "DAS_MACMILLAN-ANWESEN.SCHUTZWALD",
            "DAS_MACMILLAN-ANWESEN.SCHUTZWALD_2",
            "DAS_MACMILLAN-ANWESEN.GRUBENSCHACHT",
            "DAS_MACMILLAN-ANWESEN.GRUBENSCHACHT_2",
            "VERKÜMMERTE_INSEL.GARTEN_DER_FREUDE",
            "VERKÜMMERTE_INSEL.HAUPTPLATZ_VON_GREENVILLE",
            "YAMAOKA-ANWESEN.FAMILIENSITZ",
            "YAMAOKA-ANWESEN.FAMILIENSITZ_2",
            "YAMAOKA-ANWESEN.HEILIGTUM_DES_ZORNS",
            "YAMAOKA-ANWESEN.HEILIGTUM_DES_ZORNS_2"
        ];
        private static readonly List<string> eng = GetNamesOfMaps();
        private static readonly List<string> spa = [];
        private static readonly List<string> fra =
        [
            "CASSE_AUTOHAVEN.DERNIÈRE_DEMEURE_DAZAROV",
            "CASSE_AUTOHAVEN.CABANE_DU_SANG",
            "CASSE_AUTOHAVEN.GAS_HEAVEN",
            "CASSE_AUTOHAVEN.TERRAIN_DE_LA_CASSE",
            "CASSE_AUTOHAVEN.ATELIER_MISÉRABLE",
            "MARAIS_DE_BACKWATER.CELLIER_SINISTRE",
            "MARAIS_DE_BACKWATER.LE_PALE_ROSE",
            "COLDWIND_FARM.ÉTABLE_EN_RUINE",
            "COLDWIND_FARM.ABATTOIR_PUTRIDE",
            "COLDWIND_FARM.CHAMPS_POURRIS",
            "COLDWIND_FARM.LA_FERME_THOMPSON",
            "COLDWIND_FARM.VALLON_DES_TOURMENTS",
            "ASILE_DE_CROTUS_PRENN.SALLE_DES_ALIÉNÉS",
            "ASILE_DE_CROTUS_PRENN.CHAPELLE_DU_PÈRE_CAMPBELL",
            "FORÊT_PROFONDE_DE_DVARKA.ÉPAVE_DU_NOSTROMO",
            "FORÊT_PROFONDE_DE_DVARKA.SITE_DATTERRISSAGE_DE_TOBA",
            "CIMETIÈRE_ABANDONNÉ.AIRE_DE_CORBEAUX",
            "USINE_DE_CONDITIONNEMENT_DE_VIANDE_GIDEON.LE_JEU",
            "TOMBE_DE_GLENVALE.SALOON_DE_DEAD_DAWG",
            "HADDONFIELD.LAMPKIN_LANE",
            "LABORATOIRE_NATIONAL_DHAWKINS.LE_COMPLEXE_SOUTERRAIN",
            "INSTITUT_DE_LA_MÉMOIRE_DE_LÉRY.CENTRE_DE_TRAITEMENT",
            "ORMOND.STATION_DU_MONT_ORMOND",
            "ORMOND.STATION_DU_MONT_ORMOND",
            "ORMOND.STATION_DU_MONT_ORMOND",
            "RACCOON_CITY.AILE_EST_DU_COMMISSARIAT_DE_RACCOON_CITY",
            "RACCOON_CITY.AILE_OUEST_DU_COMMISSARIAT_DE_RACCOON_CITY",
            "FORÊT_ROUGE.LA_MAISON_MATERNELLE",
            "FORÊT_ROUGE.LE_TEMPLE_DE_PURGATION",
            "SILENT_HILL.ÉCOLE_PRIMAIRE_MIDWICH",
            "SPRINGWOOD.ÉCOLE_MATERNELLE_DE_BADHAM",
            "SPRINGWOOD.ÉCOLE_MATERNELLE_DE_BADHAM_II",
            "SPRINGWOOD.ÉCOLE_MATERNELLE_DE_BADHAM_III",
            "SPRINGWOOD.ÉCOLE_MATERNELLE_DE_BADHAM_IV",
            "SPRINGWOOD.ÉCOLE_MATERNELLE_DE_BADHAM_V",
            "LE_BORGO_DÉCIMÉ.RUINES_OUBLIÉES",
            "LE_BORGO_DÉCIMÉ.LA_PLACE_DÉVASTÉE",
            "LA_PROPRIÉTÉ_MACMILLAN.ENTREPÔT_DE_CHARBON",
            "LA_PROPRIÉTÉ_MACMILLAN.ENTREPÔT_DE_CHARBON",
            "LA_PROPRIÉTÉ_MACMILLAN.ENTREPÔT_GRONDANT",
            "LA_PROPRIÉTÉ_MACMILLAN.ENTREPÔT_GRONDANT",
            "LA_PROPRIÉTÉ_MACMILLAN.FONDERIE_DE_SOUFFRANCE",
            "LA_PROPRIÉTÉ_MACMILLAN.FONDERIE_DE_SOUFFRANCE",
            "LA_PROPRIÉTÉ_MACMILLAN.FORÊT_PROFONDE",
            "LA_PROPRIÉTÉ_MACMILLAN.FORÊT_PROFONDE",
            "LA_PROPRIÉTÉ_MACMILLAN.PUITS_DASPHYXIE",
            "LA_PROPRIÉTÉ_MACMILLAN.PUITS_DASPHYXIE",
            "ÎLE_FLÉTRIE.JARDIN_DE_LA_JOIE",
            "ÎLE_FLÉTRIE.PLACE_DE_GREENVILLE",
            "PROPRIÉTÉ_YAMAOKA.RÉSIDENCE_FAMILIALE",
            "PROPRIÉTÉ_YAMAOKA.RÉSIDENCE_FAMILIALE",
            "PROPRIÉTÉ_YAMAOKA.SANCTUAIRE_DU_COURROUX",
            "PROPRIÉTÉ_YAMAOKA.SANCTUAIRE_DU_COURROUX"
        ];
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
            "НАЦИОНАЛЬНАЯ_ЛАБОРАТОРИЯ_ХОУКИНСА.ПОДЗЕМНЫЙ_КОМПЛЕКС",
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
            "ПОМЕСТЬЕ_ЯМАОКИ.ПРИБЕЖИЩЕ_ГНЕВА_2"
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
                var result = mapsList.FirstOrDefault(x => x.StartsWith(mapName));
                index = mapsList.IndexOf(result ?? mapName);
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
