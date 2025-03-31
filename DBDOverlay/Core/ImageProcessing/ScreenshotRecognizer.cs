using DBDOverlay.Core.Extensions;
using DBDOverlay.Core.Languages;
using DBDOverlay.Core.MapOverlay;
using DBDOverlay.Core.Utils;
using DBDOverlay.Properties;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Tesseract;
using ImageFormat = System.Drawing.Imaging.ImageFormat;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Rectangle = System.Drawing.Rectangle;

namespace DBDOverlay.Core.ImageProcessing
{
    public static class ScreenshotRecognizer
    {
        private static int width;
        private static int height;
        private static readonly int maxScale = 3;
        private static readonly int maxTreshold = 750;
        private static readonly int minTreshold = 400;
        private static readonly string png = "png";
        private static string text = string.Empty;

        public static void SetScreenBounds()
        {
            var bounds = Screen.PrimaryScreen?.Bounds;
            if (bounds != null)
            {
                width = bounds.Value.Width;
                Logger.Info($"Screen width = {width}");
                height = bounds.Value.Height;
                Logger.Info($"Screen height = {height}");
            }
            else
            {
                Logger.Error("Screen bounds was not found");
            }
        }

        public static MapInfo GetMapInfo(bool autoMode = false)
        {
            var log = !autoMode;
            if (log) Logger.Info($"=============== Start getting map info ===============");
            var imagePath = $@"{Folders.Images}\{GetFileName(autoMode)}.{png}";
            var watch = Stopwatch.StartNew();
            CreateImageFromScreenArea(GetRectMultiplier(autoMode), imagePath, log);
            for (int scale = 1; scale <= (autoMode ? 1 : maxScale); scale++)
            {
                for (int treshold = autoMode ? maxTreshold : minTreshold; treshold >= minTreshold; treshold -= 50)
                {
                    if (log) Logger.Info($"===== Size = {scale}, Treshold = {treshold} =====");
                    RecognizeText(PreProcessImage(imagePath, scale, treshold, true, log), log);
                    if (IsMapTextCorrect(autoMode))
                    {
                        if (log) Logger.Info("Map text is correct");
                        var mapInfo = ConvertTextToMapInfo(autoMode, log);
                        if (mapInfo.HasImage)
                        {
                            if (log) Logger.Info("Map has image file");
                            watch.Stop();
                            var time = watch.ElapsedMilliseconds;
                            mapInfo.Scale = scale;
                            mapInfo.Treshold = treshold;
                            mapInfo.Time = time;
                            if (log) Logger.Info($"=============== Finish getting map info ===============");
                            if (log) Logger.Info($"=============== ({time} ms) ===============");
                            return mapInfo;
                        }
                        if (scale == maxScale)
                        {
                            if (log) Logger.Warn($"Map file for '{mapInfo.FullName}' doesn't exist");
                        }
                    }
                    else
                    {
                        if (log) Logger.Warn("Incorrect map text:");
                        if (log) Logger.Warn(text);
                    }
                }
            }
            watch.Stop();
            if (log) Logger.Info($"=============== Finish getting map info ===============");
            if (log) Logger.Info($"=============== ({watch.ElapsedMilliseconds} ms) ===============");
            return null;
        }

        private static void CreateImageFromScreenArea(RectMultiplier rectMultiplier, string path, bool log = true)
        {
            var imageWidth = (width * rectMultiplier.Width).Round();
            var imageHeight = (height * rectMultiplier.Height).Round();
            var rect = new Rectangle((width * rectMultiplier.X).Round(), (height * rectMultiplier.Y).Round(), imageWidth, imageHeight);
            if (log) Logger.Info($"Screen area: x = {rect.X}, y = {rect.Y}, width = {rect.Width}, height = {rect.Height}");

            var captureBitmap = new Bitmap(imageWidth, imageHeight, PixelFormat.Format32bppRgb);
            var captureGraphics = Graphics.FromImage(captureBitmap);
            captureGraphics.CopyFromScreen(rect.Left, rect.Top, 0, 0, rect.Size);
            captureBitmap.Save(path, ImageFormat.Png);

            captureGraphics.Dispose();
            captureBitmap.Dispose();
            if (log) Logger.Info($"Image is saved to '{path}'");
        }

        private static RectMultiplier GetRectMultiplier(bool autoMode)
        {
            return autoMode ? new RectMultiplier(0.04, 0.81, 0.56, 0.05) : new RectMultiplier(0.13, 0.62, 0.36, 0.14);
        }

        private static void RecognizeText(string imagePath, bool log = true)
        {
            var watch = Stopwatch.StartNew();
            var engine = new TesseractEngine(Folders.TessData, LanguagesManager.ConvertMexToSpa(Settings.Default.Language));
            var image = Pix.LoadFromFile(imagePath);
            text = engine.Process(image).GetText();
            watch.Stop();
            if (log) Logger.Info($"Text from image is recognized ({watch.ElapsedMilliseconds} ms)");
        }

        private static bool IsMapTextCorrect(bool autoMode = false)
        {
            return autoMode
                ? text.Length > 5 && text.Contains('\n')
                : text.Contains(LanguagesManager.GetMapInfoLocale()) && text.Contains('\n') && text.ContainsRegex(" - ");
        }

        private static MapInfo ConvertTextToMapInfo(bool autoMode = false, bool log = true)
        {
            var mapInfo = autoMode ? ConvertStartTextToMapInfo() : ConvertEscTextToMapInfo();

            if (log) Logger.Info("Text is converted to 'Map info' object");
            if (log) Logger.Info($"Map info: realm = {mapInfo.Realm}, name = {mapInfo.Name}");
            return mapInfo;
        }

        private static MapInfo ConvertEscTextToMapInfo()
        {
            var res = text.Split(" - ");
            var realm = res[0].Split('\n').Last().RemoveRegex("'").Replace(" ", "_").ToUpper();
            var mapName = res[1].Split('\n')[0].RemoveRegex("'").Replace(" ", "_").ToUpper();
            return new MapInfo(realm, HandleBadhamIssues(mapName));
        }

        private static MapInfo ConvertStartTextToMapInfo()
        {
            var mapName = text.RemoveRegex(@"\n|'|\.").Replace(" ", "_").RemoveRegex(@"_{1,}$").ToUpper();
            return new MapInfo(HandleBadhamIssues(mapName, false), true);
        }

        private static string HandleBadhamIssues(string mapName, bool log = true)
        {
            var tessWicknessPattern = @"(I|L|\||1|!)";
            var badhamSuffixPattern = $"_{tessWicknessPattern}{{1,3}}$";

            if (mapName.ContainsRegex(badhamSuffixPattern))
            {
                var oldMapName = mapName;
                var suffix = Regex.Match(mapName, badhamSuffixPattern).Value.Replace("|", @"\|");
                mapName = $"{mapName.RemoveRegex(suffix)}{suffix.ReplaceRegex(tessWicknessPattern, "I")}".RemoveRegex(@"\\");

                if (log) Logger.Info("Specific symbols are replaced");
                if (log) Logger.Info($"Old map name: '{oldMapName}'");
                if (log) Logger.Info($"New map name: '{mapName}'");
            }
            mapName = ReplaceSymbol(mapName, "№", "IV", log);
            mapName = ReplaceSymbol(mapName, @"\/", "V", log);
            return mapName;
        }

        private static string ReplaceSymbol(string mapName, string oldString, string newString, bool log = true)
        {
            if (!mapName.EndsWith(oldString)) return mapName;
            if (oldString.StartsWith(@"\")) oldString = $@"\{oldString}";
            mapName = mapName.ReplaceRegex(oldString, newString);
            if (log) Logger.Info($"'{oldString}' symbol is replaced with '{newString}'");
            return mapName;
        }

        private static string PreProcessImage(string path, int scale = 1, int treshold = 400, bool saveAsNew = false, bool log = true)
        {
            var newPath = saveAsNew ? path.ReplaceRegex($@"\.{png}", $"_edited.{png}") : path;
            var image = new Bitmap(path);
            image.Resize(scale).GrayScale().ApplyThreshold(treshold).Save(newPath);

            image.Dispose();
            if (log) Logger.Info($"Preprocessed image is saved to '{newPath}'");
            return newPath;
        }

        private static string GetFileName(bool autoMode = false)
        {
            return autoMode ? Settings.Default.AutoScreenshotFileName : Settings.Default.ManualScreenshotFileName;
        }
    }
}
