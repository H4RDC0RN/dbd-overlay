using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace DBDOverlay.Core.Windows
{
    public class IniFile
    {
        private readonly string path;
        private readonly string exe = Assembly.GetExecutingAssembly().GetName().Name;

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern long WritePrivateProfileString(string section, string key, string value, string filePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public IniFile(string iniPath = null)
        {
            path = new FileInfo(iniPath ?? exe + ".ini").FullName;
        }

        public string Read(string key, string section = null)
        {
            var retVal = new StringBuilder(255000);
            GetPrivateProfileString(section ?? exe, key, "", retVal, 255000, path);
            return retVal.ToString();
        }

        public void Write(string key, string value, string section = null)
        {
            WritePrivateProfileString(section ?? exe, key, value, path);
        }

        public void DeleteKey(string key, string section = null)
        {
            Write(key, null, section ?? exe);
        }

        public void DeleteSection(string section = null)
        {
            Write(null, null, section ?? exe);
        }

        public bool KeyExists(string key, string section = null)
        {
            return Read(key, section).Length > 0;
        }
    }
}
