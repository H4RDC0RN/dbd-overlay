using System.Windows.Forms;

namespace DBDOverlay.Core.Extensions
{
    public static class KeysExtensions
    {
        public static string ToStringKey(this Keys key)
        {
            switch (key)
            {
                case Keys.Back:
                    return "{BACKSPACE}";
                case Keys.Separator:
                    return "{BREAK}";
                case Keys.Escape:
                    return "{ESC}";
                case Keys.PageDown:
                    return "{PGDN}";
                case Keys.PageUp:
                    return "{PGUP}";
                case Keys.PrintScreen:
                    return "{PRTSC}";
                case Keys.Scroll:
                    return "{SCROLLLOCK}";
                default:
                    {
                        var stringKey = key.ToString();
                        if (stringKey.Length == 1) return stringKey.ToLower();
                        if (stringKey.Length == 2 && stringKey[0].Equals('D') && stringKey[1].ToString().IsInt()) return stringKey[1].ToString();
                        return $"{{{stringKey.ToUpper()}}}";
                    }
            }
        }
    }
}
