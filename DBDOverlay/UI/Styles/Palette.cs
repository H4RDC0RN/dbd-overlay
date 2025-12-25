using System.Windows;
using System.Windows.Media;

namespace DBDOverlay.UI.Styles
{
    public static class Palette
    {
        public static SolidColorBrush RedLightBrush => GetSolidBrushFromResources("RedLightBrush");
        public static SolidColorBrush DarkGrayBrush => GetSolidBrushFromResources("DarkGrayBrush");
        public static SolidColorBrush DarkestGrayBrush => GetSolidBrushFromResources("DarkestGrayBrush");
        public static SolidColorBrush DarkYellowBrush => GetSolidBrushFromResources("DarkYellowBrush");
        public static SolidColorBrush WhiteBrush => new SolidColorBrush(Colors.White);

        private static SolidColorBrush GetSolidBrushFromResources(string resourceName) => (SolidColorBrush)Application.Current.FindResource(resourceName);
    }
}
