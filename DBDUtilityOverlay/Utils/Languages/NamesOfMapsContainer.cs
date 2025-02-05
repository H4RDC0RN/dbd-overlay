﻿using System.Collections;
using System.Globalization;

namespace DBDUtilityOverlay.Utils.Languages
{
    public static class NamesOfMapsContainer
    {
        public static readonly string Empty = "Empty";
        public static readonly string NotReady = "NotReady";
        private static readonly int maxLength = 48;

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
        private static readonly List<string> spa =
        [
            "DESGUACE_AUTOHAVEN.LUGAR_DE_DESCANSO_DE_AZAROV",
            "DESGUACE_AUTOHAVEN.CABAÑA_DE_SANGRE",
            "DESGUACE_AUTOHAVEN.GASOLINERA",
            "DESGUACE_AUTOHAVEN.DESGUACE",
            "DESGUACE_AUTOHAVEN.TALLER_RUINOSO",
            "PANTANO_DE_AGUAS_ESTANCADAS.DESPENSA_SOMBRÍA",
            "PANTANO_DE_AGUAS_ESTANCADAS.LA_ROSA_PÁLIDA",
            "GRANJA_COLDWIND.ESTABLO_EN_RUINAS",
            "GRANJA_COLDWIND.MATADERO_PESTILENTE",
            "GRANJA_COLDWIND.CAMPOS_PODRIDOS",
            "GRANJA_COLDWIND.LA_CASA_DE_LOS_THOMPSON",
            "GRANJA_COLDWIND.ARROYO_DEL_TORMENTO",
            "PSIGUIÁTRICO_CROTUS_PRENN.SALA_DE_TRANSTORNADOS",
            "PSIQUIÁTRICO_CROTUS_PRENN.CAPILLA_DEL_PADRE_CAMPBELL",
            "ESPESURA_DE_DVARKA.RESTOS_DE_LA_NOSTROMO",
            "ESPESURA_DE_DVARKA.ATERRIZAJE_EN_TOBA",
            "CEMENTERIO_ABANDONADO.NIDO_DE_CUERVOS",
            "PLANTA_PROCESADORA_DE_CARNE_GIDEON.EL_JUEGO",
            "TUMBA_DE_GLENVALE.SALÓN_DEAD_DAWG",
            "HADDONFIELD.AVENIDA_LAMPKIN",
            "LABORATORIO_NACIONAL_DE_HAWKINS.EL_COMPLEJO_SUBTERRÁNEO",
            "INSTITUTO_CONMEMORATIVO_LÉRY.CENTRO_DE_TRATAMIENTO",
            "ORMOND.COMPLEJO_DEL_MONTE_ORMOND",
            "ORMOND.COMPLEJO_DEL_MONTE_ORMOND_2",
            "ORMOND.COMPLEJO_DEL_MONTE_ORMOND_3",
            "RACCOON_CITY.ALA_ESTE_DE_LA_COMISARÍA_DE_RACCOON_CITY",
            "RACCOON_CITY.ALA_OESTE_DE_LA_COMISARÍA_DE_RACCOON_CITY",
            "BOSQUE_ROJO.MORADA_MATERNAL",
            "BOSQUE_ROJO.EL_TEMPLO_DE_LA_PURGACIÓN",
            "SILENT_HILL.ESCUELA_DE_PRIMARIA_MIDWICH",
            "SPRINGWOOD.GUARDERÍA_BADHAM",
            "SPRINGWOOD.GUARDERÍA_BADHAM_II",
            "SPRINGWOOD.GUARDERÍA_BADHAM_III",
            "SPRINGWOOD.GUARDERÍA_BADHAM_IV",
            "SPRINGWOOD.GUARDERÍA_BADHAM_V",
            "EL_BURGO_DIEZMADO.RUINAS_OLVIDADAS",
            "EL_BURGO_DIEZMADO.LA_PLAZA_DESOLADA",
            "LA_FINCA_MACMILLAN.TORRE_DE_CARBÓN",
            "LA_FINCA_MACMILLAN.TORRE_DE_CARBÓN_2",
            "LA_FINCA_MACMILLAN.ALMACÉN_QUEJUMBROSO",
            "LA_FINCA_MACMILLAN.ALMACÉN_QUEJUMBROSO_2",
            "LA_FINCA_MACMILLAN.FUNDICIÓN_DE_LA_MISERIA",
            "LA_FINCA_MACMILLAN.FUNDICIÓN_DE_LA_MISERIA_2",
            "LA_FINCA_MACMILLAN.BOSQUE_REFUGIO",
            "LA_FINCA_MACMILLAN.BOSQUE_REFUGIO_2",
            "LA_FINCA_MACMILLAN.POZO_DE_ASFIXIA",
            "LA_FINCA_MACMILLAN.POZO_DE_ASFIXIA_2",
            "ISLA_MARCHITA.JARDÍN_DEL_JÚBILO",
            "ISLA_MARCHITA.PLAZA_DE_GREENVILLE",
            "RESIDENCIA_YAMAOKA.RESIDENCIA_FAMILIAR",
            "RESIDENCIA_YAMAOKA.RESIDENCIA_FAMILIAR_2",
            "RESIDENCIA_YAMAOKA.SANTUARIO_DE_LA_CÓLERA",
            "RESIDENCIA_YAMAOKA.SANTUARIO_DE_LA_CÓLERA_2"
        ];
        private static readonly List<string> mex =
        [
            "DESGUACE_AUTOHAVEN.LUGAR_DE_DESCANSO_DE_AZAROV",
            "DESGUACE_AUTOHAVEN.CABAÑA_DE_SANGRE",
            "DESGUACE_AUTOHAVEN.GASOLINERA",
            "DESGUACE_AUTOHAVEN.DESGUACE",
            "DESGUACE_AUTOHAVEN.TALLER_RUINOSO",
            "PANTANO_DE_AGUAS_ESTANCADAS.DESPENSA_SOMBRÍA",
            "PANTANO_DE_AGUAS_ESTANCADAS.LA_ROSA_PÁLIDA",
            "GRANJA_COLDWIND.ESTABLO_EN_RUINAS",
            "GRANJA_COLDWIND.MATADERO_PESTILENTE",
            "GRANJA_COLDWIND.CAMPOS_PODRIDOS",
            "GRANJA_COLDWIND.LA_CASA_DE_LOS_THOMPSON",
            "GRANJA_COLDWIND.ARROYO_DEL_TORMENTO",
            "PSIGUIÁTRICO_CROTUS_PRENN.SALA_DE_TRANSTORNADOS",
            "PSIQUIÁTRICO_CROTUS_PRENN.CAPILLA_DEL_PADRE_CAMPBELL",
            "BOSQUE_ESPESO_DE_DVARKA.RUINAS_DE_LA_NOSTROMO",
            "BOSQUE_ESPESO_DE_DVARKA.DESEMBARCO_EN_TOBA",
            "CEMENTERIO_ABANDONADO.AVIARIO_DE_CUERVOS",
            "PLANTA_PROCESADORA_DE_CARNE_GIDEON.EL_JUEGO",
            "TUMBA_DE_GLENVALE.CANTINA_EL_PERRO_MUERTO",
            "HADDONFIELD.AVENIDA_LAMPKIN",
            "LABORATORIO_NACIONAL_DE_HAWKINS.EL_COMPLEJO_SUBTERRÁNEO",
            "INSTITUTO_CONMEMORATIVO_LÉRY.CENTRO_DE_TRATAMIENTO",
            "ORMOND.COMPLEJO_DEL_MONTE_ORMOND",
            "ORMOND.COMPLEJO_DEL_MONTE_ORMOND_2",
            "ORMOND.COMPLEJO_DEL_MONTE_ORMOND_3",
            "RACCOON_CITY.ALA_ESTE_DEL_DEPARTAMENTO_DE_POLICÍA_DE_RACCOON_CITY",
            "RACCOON_CITY.ALA_OESTE_DEL_DEPARTAMENTO_DE_POLICÍA_DE_RACCOON_CITY",
            "BOSQUE_ROJO.MORADA_MATERNAL",
            "BOSQUE_ROJO.EL_TEMPLO_DE_LA_PURGACIÓN",
            "SILENT_HILL.PRIMARIA_MIDWICH",
            "SPRINGWOOD.GUARDERÍA_BADHAM",
            "SPRINGWOOD.GUARDERÍA_BADHAM_II",
            "SPRINGWOOD.GUARDERÍA_BADHAM_III",
            "SPRINGWOOD.GUARDERÍA_BADHAM_IV",
            "SPRINGWOOD.GUARDERÍA_BADHAM_V",
            "EL_PUEBLO_DIEZMADO.RUINAS_OLVIDADAS",
            "EL_PUEBLO_DIEZMADO.EL_CUADRADO_ROTO",
            "LA_FINCA_MACMILLAN.TORRE_DE_CARBÓN",
            "LA_FINCA_MACMILLAN.TORRE_DE_CARBÓN_2",
            "LA_FINCA_MACMILLAN.ALMACÉN_QUEJUMBROSO",
            "LA_FINCA_MACMILLAN.ALMACÉN_QUEJUMBROSO_2",
            "LA_FINCA_MACMILLAN.FUNDICIÓN_DE_LA_MISERIA",
            "LA_FINCA_MACMILLAN.FUNDICIÓN_DE_LA_MISERIA_2",
            "LA_FINCA_MACMILLAN.BOSQUE_REFUGIO",
            "LA_FINCA_MACMILLAN.BOSQUE_REFUGIO_2",
            "LA_FINCA_MACMILLAN.POZO_DE_ASFIXIA",
            "LA_FINCA_MACMILLAN.POZO_DE_ASFIXIA_2",
            "ISLA_MARCHITA.JARDÍN_DE_DICHA",
            "ISLA_MARCHITA.PLAZA_DE_GREENVILLE",
            "RESIDENCIA_YAMAOKA.RESIDENCIA_FAMILIAR",
            "RESIDENCIA_YAMAOKA.RESIDENCIA_FAMILIAR_2",
            "RESIDENCIA_YAMAOKA.SANTUARIO_DE_LA_IRA",
            "RESIDENCIA_YAMAOKA.SANTUARIO_DE_LA_IRA_2"
        ];
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
            "ORMOND.STATION_DU_MONT_ORMOND_2",
            "ORMOND.STATION_DU_MONT_ORMOND_3",
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
            "LA_PROPRIÉTÉ_MACMILLAN.ENTREPÔT_DE_CHARBON_2",
            "LA_PROPRIÉTÉ_MACMILLAN.ENTREPÔT_GRONDANT",
            "LA_PROPRIÉTÉ_MACMILLAN.ENTREPÔT_GRONDANT_2",
            "LA_PROPRIÉTÉ_MACMILLAN.FONDERIE_DE_SOUFFRANCE",
            "LA_PROPRIÉTÉ_MACMILLAN.FONDERIE_DE_SOUFFRANCE_2",
            "LA_PROPRIÉTÉ_MACMILLAN.FORÊT_PROFONDE",
            "LA_PROPRIÉTÉ_MACMILLAN.FORÊT_PROFONDE_2",
            "LA_PROPRIÉTÉ_MACMILLAN.PUITS_DASPHYXIE",
            "LA_PROPRIÉTÉ_MACMILLAN.PUITS_DASPHYXIE_2",
            "ÎLE_FLÉTRIE.JARDIN_DE_LA_JOIE",
            "ÎLE_FLÉTRIE.PLACE_DE_GREENVILLE",
            "PROPRIÉTÉ_YAMAOKA.RÉSIDENCE_FAMILIALE",
            "PROPRIÉTÉ_YAMAOKA.RÉSIDENCE_FAMILIALE_2",
            "PROPRIÉTÉ_YAMAOKA.SANCTUAIRE_DU_COURROUX",
            "PROPRIÉTÉ_YAMAOKA.SANCTUAIRE_DU_COURROUX_2"
        ];
        private static readonly List<string> ita = 
        [
            "DEMOLIZIONI_AUTOHAVEN.SEPOLCRO_DI_AZAROV",
            "DEMOLIZIONI_AUTOHAVEN.RIFUGIO_INSANGUINATO",
            "DEMOLIZIONI_AUTOHAVEN.GAS_HEAVEN",
            "DEMOLIZIONI_AUTOHAVEN.DEPOSITO_DEMOLIZIONI",
            "DEMOLIZIONI_AUTOHAVEN.NEGOZIO_SQUALLIDO",
            "PALUDE_BACKWATER.DISPENSA_TRUCIDA",
            "PALUDE_BACKWATER.IL_PALE_ROSE",
            "FATTORIA_COLDWIND.STALLA_DANNEGGIATA",
            "FATTORIA_COLDWIND.MATTATOIO_IRRANCIDITO",
            "FATTORIA_COLDWIND.CAMPI_PUTREFATTI",
            "FATTORIA_COLDWIND.LA_CASA_DI_THOMPSON",
            "FATTORIA_COLDWIND.TORRENTE_DEL_TORMENTO",
            "OSPEDALE_PSICHIATRICO_CROTUS_PRENN.REPARTO_SQUILIBRATI",
            "OSPEDALE_PSICHIATRICO_CROTUS_PRENN.CHIESA_DI_PADRE_CAMP",
            "FORESTA_OMBROSA_DI_DVARKA.RELITTO_NOSTROMO",
            "FORESTA_OMBROSA_DI_DVARKA.APPRODO_AL_LAGO_TOBA",
            "CIMNITERO_ABBANDONATO.NIDO_DI_CORVI",
            "MATTATOIO_DI_GIDEON.IL_GIOCO",
            "TOMBA_DI_GLENVALE.SALOON_DEAD_DAWG",
            "HADDONFIELD.LAMPKIN_LANE",
            "LABORATORIO_NAZIONALE_DI_HAWKINS.IL_COMPLESSO_SOTTERRANE",
            "ISTITUTO_COMMEMORATIVO_LÉRY.CLINICA_CURATIVA",
            "ORMOND.RESORT_DEL_MONTE_ORMOND",
            "ORMOND.RESORT_DEL_MONTE_ORMOND_2",
            "ORMOND.RESORT_DEL_MONTE_ORMOND_3",
            "RACCOON_CITY.ALA_EST_DELLA_STAZIONE_DI_POLIZIA_DI_RAC",
            "RACCOON_CITY.ALA_OVEST_DELLA_STAZIONE_DI_POLIZIA_DI_RAC",
            "FORESTA_ROSSA.LA_CASETTA_DELLA_MAMMA",
            "FORESTA_ROSSA.IL_TEMPIO_DELLA_PURIFICAZIONE",
            "SILENT_HILL.SCUOLA_ELEMENTARE_MIDWICH",
            "SPRINGWOOD.SCUOLA_MATERNA_DI_BADHAM",
            "SPRINGWOOD.SCUOLA_MATERNA_DI_BADHAM_II",
            "SPRINGWOOD.SCUOLA_MATERNA_DI_BADHAM_III",
            "SPRINGWOOD.SCUOLA_MATERNA_DI_BADHAM_IV",
            "SPRINGWOOD.SCUOLA_MATERNA_DI_BADHAM_V",
            "IL_BORGO_DECIMATO.ROVINE_DIMENTICATE",
            "IL_BORGO_DECIMATO.LA_PIAZZA_DISTRUTTA",
            "LA_TENUTA_MACMILLAN.TORRE_DI_CARBONE",
            "LA_TENUTA_MACMILLAN.TORRE_DI_CARBONE_2",
            "LA_TENUTA_MACMILLAN.DEPOSITO_SCRICCHIOLANTE",
            "LA_TENUTA_MACMILLAN.DEPOSITO_SCRICCHIOLANTE_2",
            "LA_TENUTA_MACMILLAN.FERRIERA_DEL_TORMENTO",
            "LA_TENUTA_MACMILLAN.FERRIERA_DEL_TORMENTO_2",
            "LA_TENUTA_MACMILLAN.RIFUGI_BOSCHIVI",
            "LA_TENUTA_MACMILLAN.RIFUGI_BOSCHIVI_2",
            "LA_TENUTA_MACMILLAN.MINIERA_DEL_SOFFOCAMENTO",
            "LA_TENUTA_MACMILLAN.MINIERA_DEL_SOFFOCAMENTO_2",
            "ISOLOTTO_PUTREFATTO.GIARDINO_DI_GIOIA",
            "ISOLOTTO_PUTREFATTO.GREENVILLE_SQUARE",
            "TENUTA_YAMAOKA.RESIDENZA_DI_FAMIGLIA",
            "TENUTA_YAMAOKA.RESIDENZA_DI_FAMIGLIA_2",
            "TENUTA_YAMAOKA.SACRARIO_DELLIRA",
            "TENUTA_YAMAOKA.SACRARIO_DELLIRA_2"
        ];
        private static readonly List<string> pol = 
        [
            "ZŁOMOWISKO_AUTOHAVEN.MIEJSCE_SPOCZYNKU_AZAROWA",
            "ZŁOMOWISKO_AUTOHAVEN.KRWAWA_STRÓŻÓWKA",
            "ZŁOMOWISKO_AUTOHAVEN.STACJA_PALIW",
            "ZŁOMOWISKO_AUTOHAVEN.PODWÓRZE_ZŁOMOWISKA",
            "ZŁOMOWISKO_AUTOHAVEN.SKLEP_NĘDZY",
            "BAGIENNE_ROZLEWISKO.PONURA_SPIŻARNIA",
            "BAGIENNE_ROZLEWISKO.BLADA_RÓŻA",
            "FARMA_COLDWIND.SPĘKANA_OBORA",
            "FARMA_COLDWIND.ZJEŁCZAŁA_RZEŹNIA",
            "FARMA_COLDWIND.ZBUTWIAŁE_POLA",
            "FARMA_COLDWIND.DOM_THOMPSONA",
            "FARMA_COLDWIND.PRZEŁĘCZ_UDRĘKI",
            "PRZYTUŁEK_CROTUS_PRENN.NIESPOKOJNY_ODDZIAŁ",
            "PRZYTUŁEK_CROTUS_PRENN.KAPLICA_OJCA_CAMPBELLA",
            "GĘSTY_LAS_DVARKI.WRAK_NOSTROMO",
            "GĘSTY_LAS_DVARKI.ZIEMIE_TOBA",
            "ZAPOMNIANY_CMENTARZ.GNIAZDO_KRUKÓW",
            "ZAKŁAD_MIĘSNY_GIDEON.ROZGRYWKA",
            "GRÓB_W_GLENVALE.KNAJPA_POD_MARTWYM_ZIOMKIEM",
            "HADDONFIELD.ALEJKA_LAMPKIN",
            "HAWKINS_NATIONAL_LABORATORY.PODZIEMNY_KOMPLEKS",
            "LÓRYS_MEMORIAL_INSTITUTE.PLACÓWKA_MEDYCZNA",
            "ORMOND.OŚRODEK_MOUNT_ORMOND",
            "ORMOND.OŚRODEK_MOUNT_ORMOND_2",
            "ORMOND.OŚRODEK_MOUNT_ORMOND_3",
            "RACCOON_CITY.POSTERUNEK_POLICJI_RACCOON_CITY_—_WSCHODNIE",
            "RACCOON_CITY.POSTERUNEK_POLICJI_RACCOON_CITY_—_ZACHODNIE",
            "CZERWONY_LAS.DOM_MATKI",
            "CZERWONY_LAS.ŚWIĄTYNIA_OCZYSZCZENIA",
            "SILENT_HILL.SZKOŁA_PODSTAWOWA_MIDWICH",
            "SPRINGWOOD.PRZEDSZKOLE_W_BADHAM",
            "SPRINGWOOD.PRZEDSZKOLE_W_BADHAM_II",
            "SPRINGWOOD.PRZEDSZKOLE_W_BADHAM_III",
            "SPRINGWOOD.PRZEDSZKOLE_W_BADHAM_IV",
            "SPRINGWOOD.PRZEDSZKOLE_W_BADHAM_V",
            "ZDZIESIĄTKOWANE_BORGO.ZAPOMNIANE_RUINY",
            "ZDZIESIĄTKOWANE_BORGO.ROZTRZASKANY_PLAC",
            "POSIADŁOŚĆ_MACMILLANÓW.WIEŻA_WĘGLOWA",
            "POSIADŁOŚĆ_MACMILLANÓW.WIEŻA_WĘGLOWA_2",
            "POSIADŁOŚĆ_MACMILLANÓW.JĘCZĄCY_MAGAZYN",
            "POSIADŁOŚĆ_MACMILLANÓW.JĘCZĄCY_MAGAZYN_2",
            "POSIADŁOŚĆ_MACMILLANÓW.HUTA_NIEDOLI",
            "POSIADŁOŚĆ_MACMILLANÓW.HUTA_NIEDOLI_2",
            "POSIADŁOŚĆ_MACMILLANÓW.LEŚNE_SCHRONISKO",
            "POSIADŁOŚĆ_MACMILLANÓW.LEŚNE_SCHRONISKO_2",
            "POSIADŁOŚĆ_MACMILLANÓW.DUSZNA_KOPALNIA",
            "POSIADŁOŚĆ_MACMILLANÓW.DUSZNA_KOPALNIA_2",
            "USCHNIĘTA_WYSPA.OGRÓD_RADOŚCI",
            "USCHNIĘTA_WYSPA.PLAC_GREENVILLE",
            "POSIADŁOŚĆ_RODZINY_YAMAOKA.REZYDENCJA_RODZINNA",
            "POSIADŁOŚĆ_RODZINY_YAMAOKA.REZYDENCJA_RODZINNA_2",
            "POSIADŁOŚĆ_RODZINY_YAMAOKA.SANKTUARIUM_GNIEWU",
            "POSIADŁOŚĆ_RODZINY_YAMAOKA.SANKTUARIUM_GNIEWU_2"
        ];
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

        private static readonly List<List<string>> maps = [deu, eng, spa, mex, fra, ita, pol, por, tur, rus];
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
