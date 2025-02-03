﻿using DBDUtilityOverlay.Utils.Languages;

namespace DBDUtilityOverlay.Utils.Models
{
    public class MapInfo(string realm, string name)
    {
        public string Realm { get; set; } = realm;
        public string Name { get; set; } = name;
        public string FullName => Realm != string.Empty ? $@"{Realm}.{Name}" : Name;
        public string ResourceName => NamesOfMapsContainer.GetMapFileName(FullName);
        public bool HasImage => MapImages.ResourceManager.GetObject(ResourceName) != null;
    }
}
