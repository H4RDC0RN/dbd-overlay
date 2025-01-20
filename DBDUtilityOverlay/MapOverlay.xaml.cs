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

        public void ChangeMap(string mapName)
        {
            var index = MapOverlayGrid.Children.OfType<Image>().ToList().FindIndex(x => x.Name.Equals(imageElementName));
            MapOverlayGrid.Children.RemoveAt(index);
            AddMapOverlay(mapName);
        }

        private void AddMapOverlay(string? mapName = null)
        {
            mapName = mapName ?? "Empty";
            var image = new Image();
            image.Name = imageElementName;
            image.Source = new BitmapImage(new Uri($@"{mapsPath}{mapName}.png".ToProjectPath()));
            image.Stretch = Stretch.Fill;
            image.Width = 200;
            image.Height = 200;
            image.HorizontalAlignment = HorizontalAlignment.Center;
            image.VerticalAlignment = VerticalAlignment.Center;
            MapOverlayGrid.Children.Add(image);
        }
    }
}
