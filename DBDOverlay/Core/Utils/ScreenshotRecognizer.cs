using DBDOverlay.Core.Download;
using DBDOverlay.Core.Extensions;
using DBDOverlay.Core.Languages;
using DBDOverlay.Core.MapOverlay;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Tesseract;
using ImageFormat = System.Drawing.Imaging.ImageFormat;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Rectangle = System.Drawing.Rectangle;

namespace DBDOverlay.Core.Utils
{
    public static class ScreenshotRecognizer
    {
        private static int width;
        private static int height;
        private static readonly int tries = 1;
        private static readonly int maxScale = 3;
        private static string text = string.Empty;

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
            captureBitmap.Dispose();
            Logger.Log.Info($"Image is saved to '{path}'");
        }

        public static void RecognizeText(string imagePath)
        {
            var watch = Stopwatch.StartNew();
            var value = Properties.Settings.Default.Language;
            var engine = new TesseractEngine(DownloadManager.Instance.TessDataFolder.ToProjectPath(), LanguagesManager.ConvertMexToSpa(value));
            var image = Pix.LoadFromFile(imagePath);
            text = engine.Process(image).GetText();
            watch.Stop();
            Logger.Log.Info($"Text from image is recognized ({watch.ElapsedMilliseconds} ms)");
        }

        public static MapInfo GetMapInfo()
        {
            Logger.Log.Info($"=============== Start getting map info ===============");
            var imagePath = $"{Properties.Settings.Default.ScreenshotFileName}.png".ToProjectPath();
            CreateImageMapNameEsc(imagePath);
            for (int scale = 1; scale <= maxScale; scale++)
            {
                for (int i = 0; i < tries; i++)
                {
                    Logger.Log.Info($"=============== Size = {scale}, Try = {i + 1} ===============");
                    RecognizeText(PreProcessImage(imagePath, scale));
                    if (IsMapTextCorrect())
                    {
                        Logger.Log.Info("Map text is correct");
                        var mapInfo = ConvertTextToMapInfo();
                        if (mapInfo.HasImage)
                        {
                            Logger.Log.Info("Map has image file");
                            return mapInfo;
                        }
                        if (scale == maxScale)
                        {
                            Logger.Log.Warn($"Map file for '{mapInfo.FullName}' doesn't exist");
                            return new MapInfo(string.Empty, NamesOfMapsContainer.NotReady);
                        }
                    }
                    else
                    {
                        Logger.Log.Warn("Incorrect map text:");
                        Logger.Log.Warn(text);
                    }
                }
            }
            Logger.Log.Info($"=============== Finish getting map info ===============");
            return new MapInfo(string.Empty, NamesOfMapsContainer.Empty);
        }

        private static bool IsMapTextCorrect()
        {
            return text.Contains(LanguagesManager.GetMapInfoLocale()) && text.Contains('\n') && text.ContainsRegex(" - ");
        }

        private static MapInfo ConvertTextToMapInfo()
        {
            var res = text.Split(" - ");
            var realm = res[0].Split('\n').Last().RemoveRegex("'").Replace(" ", "_").ToUpper();
            var mapName = res[1].Split('\n')[0].RemoveRegex("'").Replace(" ", "_").ToUpper();
            var mapInfo = new MapInfo(realm, HandleBadhamIssues(mapName));

            Logger.Log.Info("Text is converted to 'Map info' object");
            Logger.Log.Info($"Map info: realm = {mapInfo.Realm}, name = {mapInfo.Name}");
            return mapInfo;
        }

        private static string HandleBadhamIssues(string mapName)
        {
            var tessWicknessPattern = @"(I|L|\||1|!)";
            var badhamSuffixPattern = $"_{tessWicknessPattern}{{1,3}}$";

            if (mapName.ContainsRegex(badhamSuffixPattern))
            {
                var oldMapName = mapName;
                var suffix = Regex.Match(mapName, badhamSuffixPattern).Value.Replace("|", @"\|");
                mapName = $"{mapName.RemoveRegex(suffix)}{suffix.ReplaceRegex(tessWicknessPattern, "I")}".RemoveRegex(@"\\");

                Logger.Log.Info($"Specific symbols are replaced");
                Logger.Log.Info($"Old map name: '{oldMapName}'");
                Logger.Log.Info($"New map name: '{mapName}'");
            }
            mapName = ReplaceSymbol(mapName, "№", "IV");
            mapName = ReplaceSymbol(mapName, @"\/", "V");
            return mapName;
        }

        private static string ReplaceSymbol(string mapName, string oldString, string newString)
        {
            if (mapName.EndsWith(oldString))
            {
                if (oldString.StartsWith(@"\")) oldString = $@"\{oldString}";
                mapName = mapName.ReplaceRegex(oldString, newString);
                Logger.Log.Info($"'{oldString}' symbol is replaced with '{newString}'");
                return mapName;
            }
            return mapName;
        }

        private static string PreProcessImage(string path, int scale = 1)
        {
            var newPath = $"{Properties.Settings.Default.ScreenshotFileName}_edited.png".ToProjectPath();
            var image = new Bitmap(path);
            image.Resize(scale).GrayScale().ApplyThreshold().Save(newPath);
            image.Dispose();
            Logger.Log.Info($"Preprocessed image is saved to '{newPath}'");
            return newPath;
        }
    }
}
