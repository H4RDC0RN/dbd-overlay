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
                        if (stringKey.Length == 1)
                            return stringKey.ToLower();
                        else
                            return $"{{{stringKey.ToUpper()}}}";
                    }
            }
        }
    }
}
