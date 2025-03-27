using DBDOverlay.Core.Languages;
using DBDOverlay.Images.Maps;

namespace DBDOverlay.Core.MapOverlay
{
    public class MapInfo
    {
        public MapInfo(string realm, string name)
        {
            Realm = realm;
            Name = name;
        }

        public string Realm { get; set; }
        public string Name { get; set; }
        public string FullName => Realm != string.Empty ? $@"{Realm}.{Name}" : Name;
        public string ResourceName => NamesOfMapsContainer.GetMapFileName(FullName);
        public bool HasImage => MapImages.ResourceManager.GetObject(ResourceName) != null;
    }
}
