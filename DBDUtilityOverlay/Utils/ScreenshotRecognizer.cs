using DBDUtilityOverlay.Utils.Extensions;
using DBDUtilityOverlay.Utils.Models;
using System.Windows;
using Tesseract;
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
            captureBitmap.Save(path, ImageFormat.Png);
        }

        public static void CreateImageMapNameEsc(string path)
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
            captureBitmap.Save(path, ImageFormat.Png);
        }

        public static string RecognizeText(string imagePath)
        {
            var engine = new TesseractEngine("Tessdata".ToProjectPath(), "eng");
            var image = Pix.LoadFromFile(imagePath);
            return engine.Process(image).GetText();
        }

        public static MapInfo? GetMapInfo()
        {
            var imagePath = @"Images\Recognition\test.png".ToProjectPath();
            CreateImageMapNameEsc(imagePath);
            var text = RecognizeText(imagePath);
            if (!text.Contains("MAP INFO") || !text.Contains('\n') || !text.Contains('-')) return null;

            var res = text.Split(" - ");
            var realm = res[0].Split('\n').Last().Remove("'").Replace(" ", "_").Replace("é", "e").ToUpper();
            var mapName = res[1].Split('\n')[0].Remove("'").Replace(" ", "_").ToUpper();

            return new MapInfo(realm, mapName);
        }
    }
}
