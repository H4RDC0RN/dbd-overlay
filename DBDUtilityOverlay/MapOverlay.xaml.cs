using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DBDUtilityOverlay.Utils;
using DBDUtilityOverlay.Utils.Extensions;
using DBDUtilityOverlay.Utils.Models;
using Image = System.Windows.Controls.Image;

namespace DBDUtilityOverlay
{
    /// <summary>
    /// Interaction logic for MapOverlay.xaml
    /// </summary>
    public partial class MapOverlay : Window
    {
        private readonly string mapsPath = @"Images/Maps/";
        private readonly string imageElementName = "Map";

        private string realm = string.Empty;
        private string name = "Empty";

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

        public void ChangeMap(MapInfo? mapInfo = null)
        {
            var index = MapOverlayGrid.Children.OfType<Image>().ToList().FindIndex(x => x.Name.Equals(imageElementName));
            MapOverlayGrid.Children.RemoveAt(index);
            AddMapOverlay(mapInfo);
        }

        public void SwitchMapVariationToNext()
        {
            var suffix = name[^2..];
            suffix = (suffix.First().ToString().Equals("_") && suffix.Last().ToString().IsInt())
                ? $"_{Convert.ToInt32(suffix.Last().ToString()) + 1}" : "_2";

            var newName = suffix.Equals("_2") ? $"{name}{suffix}" : name.Replace(name[^2..], $"{suffix}");
            var variationPath = $@"{mapsPath}{realm}/{newName}.png".ToProjectPath();
            if (File.Exists(variationPath))
            {
                name = newName;
                ChangeMap();
            }
        }

        public void SwitchMapVariationToPrevious()
        {
            var suffix = name[^2..];
            if (suffix.First().ToString().Equals("_") && suffix.Last().ToString().IsInt())
            {
                if (suffix.Last().ToString().Equals("2"))
                {
                    name = name.Remove(suffix);
                    ChangeMap();
                }
                else
                {
                    name = name.Replace(suffix, $"_{Convert.ToInt32(suffix.Last().ToString()) - 1}");
                    ChangeMap();
                }
            }
        }

        private void AddMapOverlay(MapInfo? mapInfo = null)
        {
            if (mapInfo != null)
            {
                realm = mapInfo.Realm;
                name = mapInfo.Name;
            }

            var image = new Image();
            image.Name = imageElementName;
            image.Source = new BitmapImage(new Uri($@"{mapsPath}{realm}/{name}.png".ToProjectPath()));
            image.Stretch = Stretch.Fill;
            image.Width = Width;
            image.Height = Height;
            image.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            image.VerticalAlignment = VerticalAlignment.Center;
            MapOverlayGrid.Children.Add(image);
        }
    }
}
