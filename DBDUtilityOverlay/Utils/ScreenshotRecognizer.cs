using DBDUtilityOverlay.Utils.Extensions;
using System.Drawing;
using System.Windows;
using ImageFormat = System.Drawing.Imaging.ImageFormat;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace DBDUtilityOverlay.Utils
{
    public static class ScreenshotRecognizer
    {
        private static readonly int width = (int)SystemParameters.PrimaryScreenWidth;
        private static readonly int height = (int)SystemParameters.PrimaryScreenHeight;
        public static void CaptureMyScreen(string path)
        {
            var captureBitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            var rect = new Rectangle(0, 0, width, height);
            var captureGraphics = Graphics.FromImage(captureBitmap);
            captureGraphics.CopyFromScreen(rect.Left, rect.Top, 0, 0, rect.Size);
            captureBitmap.Save(path, ImageFormat.Jpeg);
        }

        public static void GetmageMapNameEsc(string path)
        {
            double xMultiplier = 0.13;
            double yMultiplier = 0.62;
            double wMultiplier = 0.36;
            double hMultiplier = 0.14;
            var imageWidth = (width * wMultiplier).Round();
            var imageHeight = (height * hMultiplier).Round();
            var rect = new Rectangle((width * xMultiplier).Round(), (height * yMultiplier).Round(), imageWidth, imageHeight);
            var captureBitmap = new Bitmap(imageWidth, imageHeight, PixelFormat.Format32bppArgb);
            var captureGraphics = Graphics.FromImage(captureBitmap);
            captureGraphics.CopyFromScreen(rect.Left, rect.Top, 0, 0, rect.Size);
            captureBitmap.Save(path, ImageFormat.Jpeg);
        }
    }
}
