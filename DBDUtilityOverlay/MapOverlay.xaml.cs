using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DBDUtilityOverlay.Utils;
using DBDUtilityOverlay.Utils.Extensions;

namespace DBDUtilityOverlay
{
    /// <summary>
    /// Interaction logic for MapOverlay.xaml
    /// </summary>
    public partial class MapOverlay : Window
    {
        private readonly string mapsPath = @"Images/Maps/";
        private readonly string imageElementName = "Map";

        public MapOverlay()
        {
            InitializeComponent();
            AddMapOverlay();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            WindowsServices.SetWindowExTransparent(hwnd);
        }

        public void ChangeMap(string realm, string mapName)
        {
            var index = MapOverlayGrid.Children.OfType<Image>().ToList().FindIndex(x => x.Name.Equals(imageElementName));
            MapOverlayGrid.Children.RemoveAt(index);
            AddMapOverlay(realm, mapName);
        }

        private void AddMapOverlay(string? realm = null, string? mapName = null)
        {
            realm ??= string.Empty;
            mapName ??= "Empty";

            var image = new Image();
            image.Name = imageElementName;
            image.Source = new BitmapImage(new Uri($@"{mapsPath}{realm}/{mapName}.png".ToProjectPath()));
            image.Stretch = Stretch.Fill;
            image.Width = Width;
            image.Height = Height;
            image.HorizontalAlignment = HorizontalAlignment.Center;
            image.VerticalAlignment = VerticalAlignment.Center;
            MapOverlayGrid.Children.Add(image);
        }
    }
}
