using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Path = System.IO.Path;

namespace DBDOverlay.Core.Extensions
{
    public static class StringExtensions
    {
        public static string ToProjectPath(this string str)
        {
            var projectPath = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\";
            return $@"{projectPath}{str}";
        }

        public static string ToTitle(this string str)
        {
            var words = str.ToLower().Split('_').ToList();
            var result = string.Empty;
            for (int i = 0; i < words.Count; i++)
            {
                result += words[i].First().ToString().ToUpper() + words[i].Substring(1) + (words.Count == i + 1 ? string.Empty : " ");
            }
            return result;
        }

        public static string RegexMatch(this string str, string pattern)
        {
            return Regex.Match(str, pattern).Value;
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
            return str.Split(new string[] { charsArr }, StringSplitOptions.None);
        }

        public static bool IsInt(this string str)
        {
            return int.TryParse(str, out _);
        }

        public static int ToInt(this string str)
        {
            return int.Parse(str);
        }

        public static bool ToBool(this string str)
        {
            return str.Equals("1");
        }

        public static string GetLast(this string str, int substringLenght)
        {
            return str.Substring(str.Length - substringLenght);
        }

        public static int Increment(this string str)
        {
            return Convert.ToInt32(str.Last().ToString()) + 1;
        }

        public static int Decrement(this string str)
        {
            return Convert.ToInt32(str.Last().ToString()) - 1;
        }
    }
}
