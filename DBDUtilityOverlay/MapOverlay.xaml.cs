using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using DBDUtilityOverlay.Utils;
using DBDUtilityOverlay.Utils.Extensions;
using DBDUtilityOverlay.Utils.Models;
using Image = System.Windows.Controls.Image;

namespace DBDUtilityOverlay
{
    public partial class MapOverlay : Window
    {
        private readonly string imageElementName = "Map";

        private string realm;
        private string name;

        public MapOverlay()
        {
            InitializeComponent();
            realm = string.Empty;
            name = Values.Empty;
            AddMapOverlay();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            WindowsServices.SetWindowExTransparent(hwnd);
        }

        public void ChangeMap(MapInfo? mapInfo)
        {
            if (mapInfo == null || !mapInfo.HasImage) return;
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
            var mapInfo = new MapInfo(realm, newName);
            if (MapImages.ResourceManager.GetObject(mapInfo.ResourceName) != null)
            {
                name = newName;
                ChangeMap(mapInfo);
            }
        }

        public void SwitchMapVariationToPrevious()
        {
            var suffix = name[^2..];
            if (suffix.First().ToString().Equals("_") && suffix.Last().ToString().IsInt())
            {
                if (suffix.Last().ToString().Equals("2"))
                {
                    name = name.RemoveRegex(suffix);
                    ChangeMap(new MapInfo(realm, name));
                }
                else
                {
                    name = name.Replace(suffix, $"_{Convert.ToInt32(suffix.Last().ToString()) - 1}");
                    ChangeMap(new MapInfo(realm, name));
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
            if (mapInfo == null)
            {
                image.Source = MapImages.Empty.ToBitmapImage();
            }
            else
            {
                image.Source = ((Bitmap)MapImages.ResourceManager.GetObject(mapInfo.ResourceName)).ToBitmapImage();
            }
            image.Stretch = Stretch.Fill;
            image.Width = Width;
            image.Height = Height;
            image.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            image.VerticalAlignment = VerticalAlignment.Center;
            MapOverlayGrid.Children.Add(image);
        }
    }
}
