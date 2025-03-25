using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Image = System.Windows.Controls.Image;
using Cursors = System.Windows.Input.Cursors;
using DBDOverlay.Images.Maps;
using DBDOverlay.Core.Extensions;
using DBDOverlay.Core.MapOverlay;
using DBDOverlay.Core.Windows;

namespace DBDOverlay
{
    public partial class MapOverlayWindow : Window
    {
        private static readonly string imageElementName = "Map";

        public MapOverlayWindow()
        {
            InitializeComponent();
            SetOverlaySettings();
            AddImageMapOverlay();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            WindowsServices.Instance.SetWindowExTransparent(new WindowInteropHelper(this).Handle);
        }

        private void SetOverlaySettings()
        {
            Opacity = Properties.Settings.Default.OverlayOpacity / 100.0;
            Left = Properties.Settings.Default.OverlayX;
            Top = Properties.Settings.Default.OverlayY;
        }

        private void OverlayMouseDown(object sender, MouseButtonEventArgs e)
        {
            Cursor = Cursors.SizeAll;
            DragMove();
        }

        private void OverlayMouseUp(object sender, MouseButtonEventArgs e)
        {
            Cursor = Cursors.Hand;
            Properties.Settings.Default.OverlayX = Left;
            Properties.Settings.Default.OverlayY = Top;
            Properties.Settings.Default.Save();
        }

        public void ChangeMapImageOverlay(MapInfo mapInfo)
        {
            var index = MapOverlayGrid.Children.OfType<Image>().ToList().FindIndex(x => x.Name.Equals(imageElementName));
            MapOverlayGrid.Children.RemoveAt(index);
            AddImageMapOverlay(mapInfo);
        }

        private void AddImageMapOverlay(MapInfo mapInfo = null)
        {
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
