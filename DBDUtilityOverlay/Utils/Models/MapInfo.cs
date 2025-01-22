using DBDUtilityOverlay.Utils.Extensions;
using System.IO;

namespace DBDUtilityOverlay.Utils.Models
{
    public class MapInfo(string realm, string name)
    {
        public string Realm { get; set; } = realm;
        public string Name { get; set; } = name;
        public string Path => $@"{Values.MapsPath}{Realm}/{Name}.png".ToProjectPath();
        public bool HasFile => File.Exists(Path);
    }
}
