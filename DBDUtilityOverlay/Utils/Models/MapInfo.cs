namespace DBDUtilityOverlay.Utils.Models
{
    public class MapInfo(string realm, string name)
    {
        public string Realm { get; set; } = realm;
        public string Name { get; set; } = name;
        public string ResourceName => $@"{Realm}.{Name}";
        public bool HasImage => MapImages.ResourceManager.GetObject(ResourceName) != null;
    }
}
