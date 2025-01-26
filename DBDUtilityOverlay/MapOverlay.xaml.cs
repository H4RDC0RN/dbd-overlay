using System.Windows;
using System.Windows.Input;
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
        private readonly string emptyFileName = "Empty";

        private string realm;
        private string name;

        public MapOverlay()
        {
            InitializeComponent();
            realm = string.Empty;
            name = emptyFileName;
            AddMapOverlay();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            WindowsServices.SetWindowExTransparent(new WindowInteropHelper(this).Handle);
        }

        private void OverlayMouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void OverlayMouseUp(object sender, MouseButtonEventArgs e)
        {
            Properties.Settings.Default.OverlayX = Left;
            Properties.Settings.Default.OverlayY = Top;
            Properties.Settings.Default.Save();
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
            if (mapInfo == null)
            {
                image.Source = MapImages.Empty.ToBitmapImage();
            }
            else
            {
                var resObject = MapImages.ResourceManager.GetObject(mapInfo.ResourceName);
                image.Source = resObject == null ? MapImages.NotReady.ToBitmapImage() : ((Bitmap)resObject).ToBitmapImage();
            }

            image.Name = imageElementName;
            image.Stretch = Stretch.Fill;
            image.Width = Width;
            image.Height = Height;
            image.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            image.VerticalAlignment = VerticalAlignment.Center;
            MapOverlayGrid.Children.Add(image);
        }
    }
}
