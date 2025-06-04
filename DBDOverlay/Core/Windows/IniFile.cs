using DBDOverlay.Core.Extensions;
using System.IO;
using System.Linq;

namespace DBDOverlay.Core.Windows
{
    public class IniFile
    {
        private readonly string path;

        public IniFile(string iniPath)
        {
            path = new FileInfo(iniPath).FullName;
        }

        public string Read(string key)
        {
            var value = File.ReadAllText(path).Split("\r\n").First(x => x.StartsWith(key)).RemoveRegex($"{key}=");
            return value;
        }
    }
}
