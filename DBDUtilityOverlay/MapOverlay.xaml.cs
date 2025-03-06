﻿using System.Windows;
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
        private static readonly string imageElementName = "Map";

        public MapOverlay()
        {
            InitializeComponent();
            SetOverlaySettings();
            AddImageMapOverlay();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            WindowsServices.SetWindowExTransparent(new WindowInteropHelper(this).Handle);
        }

        private void SetOverlaySettings()
        {
            Opacity = Properties.Settings.Default.OverlayOpacity / 100.0;
            Left = Properties.Settings.Default.OverlayX;
            Top = Properties.Settings.Default.OverlayY;
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
