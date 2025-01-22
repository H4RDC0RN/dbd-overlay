using System.Reflection;
using System.Text.RegularExpressions;
using Path = System.IO.Path;

namespace DBDUtilityOverlay.Utils.Extensions
{
    public static class StringExtensions
    {
        public static string ToProjectPath(this string str)
        {
            var projectPath = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/";
            return $@"{projectPath}{str}";
        }

        public static string RemoveRegex(this string str, string pattern)
        {
            return str.ReplaceRegex(pattern, string.Empty);
        }

        public static string ReplaceRegex(this string str, string pattern, string newValue)
        {
            return Regex.Replace(str, pattern, newValue);
        }

        public static bool ContainsRegex(this string str, string pattern)
        {
            return Regex.IsMatch(str, pattern);
        }

        public static string[] Split(this string str, string charsArr)
        {
            return str.Split([charsArr], StringSplitOptions.None);
        }

        public static bool IsInt(this string str)
        {
            return int.TryParse(str, out _);
        }
    }
}
