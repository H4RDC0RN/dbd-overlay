using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Cursors = System.Windows.Input.Cursors;
using DBDOverlay.Images.Maps;
using DBDOverlay.Core.Extensions;
using DBDOverlay.Core.Windows;
using System;
using System.Drawing;
using DBDOverlay.Properties;
using DBDOverlay.Core.WindowControllers.MapOverlay;
using DBDOverlay.Core.WindowControllers.MapOverlay.Languages;

namespace DBDOverlay.Windows.Overlays
{
    public partial class MapOverlayWindow : Window
    {
        public int DefaultStyle { get; set; }
        public double DefaultRatio { get; private set; } = 1.1;
        public Rect DefaultRect { get; set; } = new Rect(0, 0, 200, 220);
        public Rect CurrentRect { get; set; }

        public MapOverlayWindow()
        {
            InitializeComponent();
            SetOverlaySettings();
            AddImageMapOverlay(new MapInfo(MapNamesContainer.Empty));
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            DefaultStyle = WindowsServices.Instance.SetWindowExTransparent(this);
        }

        private void SetOverlaySettings()
        {
            CurrentRect = Settings.Default.MapOverlayRect.ToRect();
            Height = CurrentRect.Height;
            Width = CurrentRect.Width;
            MinHeight = Height / 2;
            MinWidth = Width / 2;
            MaxHeight = Height * 3;
            MaxWidth = Width * 3;

            Opacity = Settings.Default.MapOverlayOpacity / 100.0;
            Left = CurrentRect.X;
            Top = CurrentRect.Y;
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
            CurrentRect = new Rect(Left, Top, CurrentRect.Width, CurrentRect.Height);
            Settings.Default.MapOverlayRect = CurrentRect.ToString();
            Settings.Default.Save();
        }

        public void SaveSize()
        {
            CurrentRect = new Rect(CurrentRect.X, CurrentRect.Y, Width, Height);
            Settings.Default.MapOverlayRect = CurrentRect.ToString();
            Settings.Default.Save();
        }

        public void ResetPosition()
        {
            Left = DefaultRect.X;
            Top = DefaultRect.Y;
            SavePosition();
        }

        public void ResetSize()
        {
            Height = DefaultRect.Height;
            Width = DefaultRect.Width;
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
