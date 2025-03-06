using DBDUtilityOverlay.Utils.Extensions;
using DBDUtilityOverlay.Utils.Languages;
using DBDUtilityOverlay.Utils.Models;
using OpenCvSharp;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Tesseract;
using ImageFormat = System.Drawing.Imaging.ImageFormat;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Rectangle = System.Drawing.Rectangle;
using Size = OpenCvSharp.Size;

namespace DBDUtilityOverlay.Utils
{
    public static class ScreenshotRecognizer
    {
        private static readonly string tessdata = "Tessdata";
        private static readonly string traineddata = "traineddata";
        private static int width;
        private static int height;
        private static readonly int tries = 1;
        private static readonly int maxSizeMultiplier = 3;
        public static string Text { get; set; } = string.Empty;

        public static void SetScreenBounds()
        {
            var bounds = Screen.PrimaryScreen?.Bounds;
            if (bounds != null)
            {
                width = bounds.Value.Width;
                Logger.Log.Info($"Screen width = {width}");
                height = bounds.Value.Height;
                Logger.Log.Info($"Screen height = {height}");
            }
            else
            {
                Logger.Log.Error("Screen bounds was not found");
            }
        }

        public static void CaptureMyScreen(string path)
        {
            var captureBitmap = new Bitmap(width, height, PixelFormat.Format32bppRgb);
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
            Logger.Log.Info($"Screen area: x = {rect.X}, y = {rect.Y}, width = {rect.Width}, height = {rect.Height}");

            var captureBitmap = new Bitmap(imageWidth, imageHeight, PixelFormat.Format32bppRgb);
            var captureGraphics = Graphics.FromImage(captureBitmap);
            captureGraphics.CopyFromScreen(rect.Left, rect.Top, 0, 0, rect.Size);
            captureBitmap.Save(path, ImageFormat.Png);
        }

        public static void RecognizeText(string imagePath)
        {
            var watch = Stopwatch.StartNew();
            var value = Properties.Settings.Default.Language;
            var engine = new TesseractEngine(tessdata.ToProjectPath(), value.Equals(LanguagesManager.MexAbb) ? LanguagesManager.SpaAbb : value);
            var image = Pix.LoadFromFile(imagePath);
            Text = engine.Process(image).GetText();
            watch.Stop();
            Logger.Log.Info($"Recognize text from image - {watch.ElapsedMilliseconds} ms");
        }

        public static MapInfo GetMapInfo()
        {
            var imagePath = $"{Properties.Settings.Default.ScreenshotFileName}.png".ToProjectPath();
            MapInfo mapInfo;
            CreateImageMapNameEsc(imagePath);
            for (int sizeMultiplier = 1; sizeMultiplier <= maxSizeMultiplier; sizeMultiplier++)
            {
                for (int i = 0; i < tries; i++)
                {
                    Logger.Log.Info($"=============== Size = {sizeMultiplier}, Try = {i + 1}");
                    RecognizeText(PreProcessImage(imagePath, sizeMultiplier));
                    if (IsTextCorrect())
                    {
                        mapInfo = ConvertTextToMapInfo();
                        if (mapInfo.HasImage) return mapInfo;
                        if (sizeMultiplier == maxSizeMultiplier)
                        {
                            Logger.Log.Warn($"Map file for '{mapInfo.FullName}' doesn't exist");
                            return new MapInfo(string.Empty, NamesOfMapsContainer.NotReady);
                        }
                    }
                    else
                    {
                        Logger.Log.Warn("Incorrect text:");
                        Logger.Log.Warn(Text);
                    }
                }
            }
            return new MapInfo(string.Empty, NamesOfMapsContainer.Empty);
        }

        public static List<string> GetDownloadedLanguages()
        {
            var regex = $@"(?<={tessdata}\\).*(?=.{traineddata})";
            return [.. Directory.GetFiles(tessdata.ToProjectPath()).Select(x => Regex.Match(x, regex).Value)];
        }

        public static void SaveTrainedData(string fileName, byte[] content)
        {
            if (content != null) File.WriteAllBytes($@"{tessdata.ToProjectPath()}\{fileName}", content);
            else Logger.Log.Warn("Downloaded content is null");
        }

        private static bool IsTextCorrect()
        {
            return Text.Contains(LanguagesManager.GetMapInfoLocale()) && Text.Contains('\n') && Text.ContainsRegex(" - ");
        }

        private static MapInfo ConvertTextToMapInfo()
        {
            var res = Text.Split(" - ");
            var realm = res[0].Split('\n').Last().RemoveRegex("'").Replace(" ", "_").ToUpper();
            var mapName = res[1].Split('\n')[0].RemoveRegex("'").Replace(" ", "_").ToUpper();
            var mapInfo = new MapInfo(realm, HandleBadhamIssues(mapName));
            Logger.Log.Info($"Map info: realm = {mapInfo.Realm}, name = {mapInfo.Name}");
            return mapInfo;
        }

        private static string HandleBadhamIssues(string mapName)
        {
            var tessWicknessPattern = @"(I|L|\||1|!)";
            var badhamSuffixPattern = $"_{tessWicknessPattern}{{1,3}}$";
            if (mapName.ContainsRegex(badhamSuffixPattern))
            {
                var suffix = Regex.Match(mapName, badhamSuffixPattern).Value.Replace("|", @"\|");
                mapName = $"{mapName.RemoveRegex(suffix)}{suffix.ReplaceRegex(tessWicknessPattern, "I")}".RemoveRegex(@"\\");
            }
            if (mapName.EndsWith('№')) return mapName.ReplaceRegex("№", "IV");
            if (mapName.EndsWith(@"\/")) return mapName.ReplaceRegex(@"\\/", "V");
            return mapName;
        }

        private static string PreProcessImage(string path, int sizeMultiplier = 1)
        {
            var cvImage = Cv2.ImRead(path);
            Cv2.Resize(cvImage, cvImage, new Size(cvImage.Width * sizeMultiplier, cvImage.Height * sizeMultiplier));
            Cv2.CvtColor(cvImage, cvImage, ColorConversionCodes.BGR2GRAY);
            Cv2.Threshold(cvImage, cvImage, 100, 255, ThresholdTypes.Binary);
            var newPath = $"{Properties.Settings.Default.ScreenshotFileName}_edited.png".ToProjectPath();
            cvImage.SaveImage(newPath);
            return newPath;
        }
    }
}
