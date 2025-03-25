using DBDOverlay.Core.Download;
using DBDOverlay.Core.Extensions;
using DBDOverlay.Images.App;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Image = System.Windows.Controls.Image;

namespace DBDOverlay.Core.CustomItems
{
    public class DownloadImage : Image
    {
        private readonly string transformName = "AnimatedRotateTransform";
        private readonly RotateTransform rotateTransform;
        private readonly Storyboard storyboard;

        public DownloadImage()
        {
            rotateTransform = new RotateTransform { Angle = 0 };
            RenderTransform = rotateTransform;
            NameScope.SetNameScope(this, new NameScope());
            RegisterName(transformName, rotateTransform);

            storyboard = new Storyboard();
            var animation = new DoubleAnimation(0, 360, new Duration(new TimeSpan(0, 0, 0, 1, 0)));
            Storyboard.SetTargetName(animation, transformName);
            Storyboard.SetTargetProperty(animation, new PropertyPath(RotateTransform.AngleProperty));
            storyboard.Children.Add(animation);

            storyboard.Completed += HandleAnimationFinished;
            DownloadManager.Instance.Downloading += HandleDownloading;
        }

        private void HandleDownloading(object sender, DownloadEventArgs e)
        {
            if (e.IsDownloading)
            {
                Source = AppImages.Loading.ToBitmapImage();
                storyboard.Begin(this);
            }
            else
            {
                storyboard.Stop(this);
            }
        }

        private void HandleAnimationFinished(object sender, EventArgs e)
        {
            Source = AppImages.Download.ToBitmapImage();
        }
    }
}
