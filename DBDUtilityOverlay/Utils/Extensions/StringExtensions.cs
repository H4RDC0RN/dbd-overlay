using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DBDUtilityOverlay.Utils.Extensions
{
    public static class StringExtensions
    {
        public static string ToProjectPath(this string str)
        {
            var projectPath = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/../../../";
            return $@"{projectPath}{str}";
        }

        public static string Remove(this string str, string pattern)
        {
            return Regex.Replace(str, pattern, string.Empty);
        }

        public static string[] Split(this string str, string charsArr)
        {
            return str.Split([charsArr], StringSplitOptions.None);
        }
    }
}
