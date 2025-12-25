using DBDOverlay.UI.Styles;
using System.Windows.Controls;
using System.Windows.Media;

namespace DBDOverlay.Core.Extensions
{
    public static class LabelExtensions
    {
        public static Border GetBorder(this Label label)
        {
            return (Border)label.Template.FindName("LabelBorder", label);
        }

        public static void UpdateColors(this Label label, SolidColorBrush backgroundColor, SolidColorBrush foregroundColor)
        {
            label.GetBorder().Background = backgroundColor;
            label.Foreground = foregroundColor;
        }
    }
}