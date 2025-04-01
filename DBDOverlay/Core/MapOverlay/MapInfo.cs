using DBDOverlay.Core.Languages;
using DBDOverlay.Images.Maps;

namespace DBDOverlay.Core.MapOverlay
{
    public class MapInfo
    {
        public string Realm { get; set; }
        public string Name { get; set; }
        public double Scale { get; set; }
        public int Treshold { get; set; }
        public long Time { get; set; }

        public MapInfo(string realm, string name)
        {
            Realm = realm;
            Name = name;
        }

        public MapInfo(string name, bool addRealm = false)
        {
            Name = addRealm ? NamesOfMapsContainer.GetNameByRecognizedName(name) : name;
            Realm = addRealm ? NamesOfMapsContainer.GetRealmByRecognizedName(name) : string.Empty;
        }

        public string FullName => Realm != string.Empty ? $@"{Realm}.{Name}" : Name;
        public string ResourceName => NamesOfMapsContainer.GetMapFileName(FullName);
        public bool HasImage => MapImages.ResourceManager.GetObject(ResourceName) != null;
    }
}
