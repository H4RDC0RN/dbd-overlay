using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Cursors = System.Windows.Input.Cursors;
using DBDOverlay.Images.Maps;
using DBDOverlay.Core.Extensions;
using DBDOverlay.Core.MapOverlay;
using DBDOverlay.Core.Windows;
using System;
using System.Drawing;
using DBDOverlay.Properties;
using DBDOverlay.Core.MapOverlay.Languages;

namespace DBDOverlay
{
    public partial class MapOverlayWindow : Window
    {
        public int DefaultStyle { get; set; }
        public double DefaultRatio { get; private set; } = 1.1;
        public int DefaultHeight { get; private set; } = 220;
        public int DefaultWidth { get; private set; } = 200;
        public int DefaultX { get; private set; } = 0;
        public int DefaultY { get; private set; } = 0;

        public MapOverlayWindow()
        {
            InitializeComponent();
            SetOverlaySettings();
            AddImageMapOverlay(new MapInfo(NamesOfMapsContainer.Empty));
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            DefaultStyle = WindowsServices.Instance.SetWindowExTransparent(this);
        }

        private void SetOverlaySettings()
        {
            Height = Settings.Default.MapOverlayHeight;
            Width = Settings.Default.MapOverlayWidth;
            MinHeight = Height / 2;
            MinWidth = Width / 2;
            MaxHeight = Height * 3;
            MaxWidth = Width * 3;

            Opacity = Settings.Default.MapOverlayOpacity / 100.0;
            Left = Settings.Default.MapOverlayX;
            Top = Settings.Default.MapOverlayY;
        }

        private void OverlayMouseDown(object sender, MouseButtonEventArgs e)
        {
            Cursor = Cursors.SizeAll;
            DragMove();
        }

        private void OverlayMouseUp(object sender, MouseButtonEventArgs e)
        {
            Cursor = Cursors.Hand;
            SavePosition();
        }

        public void SavePosition()
        {
            Settings.Default.MapOverlayX = Left;
            Settings.Default.MapOverlayY = Top;
            Settings.Default.Save();
        }

        public void SaveSize()
        {
            Settings.Default.MapOverlayHeight = Height;
            Settings.Default.MapOverlayWidth = Width;
            Settings.Default.Save();
        }

        public void ResetPosition()
        {
            Left = DefaultX;
            Top = DefaultY;
            SavePosition();
        }

        public void ResetSize()
        {
            Height = DefaultHeight;
            Width = DefaultWidth;
            SaveSize();
        }

        public void ChangeMapImageOverlay(MapInfo mapInfo)
        {
            AddImageMapOverlay(mapInfo);
        }

        private void AddImageMapOverlay(MapInfo mapInfo)
        {
            var resObject = MapImages.ResourceManager.GetObject(mapInfo.ResourceName);
            ImageBrush.ImageSource = resObject == null ? MapImages.NotReady.ToBitmapImage() : ((Bitmap)resObject).ToBitmapImage();
        }
    }
}
