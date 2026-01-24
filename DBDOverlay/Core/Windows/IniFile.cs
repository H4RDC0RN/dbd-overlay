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

        public void Write(string key, string value)
        {
            var lines = File.ReadAllLines(path);

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith($"{key}="))
                {
                    lines[i] = $@"{key}={value}";
                    break;
                }
            }

            File.WriteAllLines(path, lines);
        }
    }
}
