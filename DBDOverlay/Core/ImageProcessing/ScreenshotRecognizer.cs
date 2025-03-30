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
            Logger.ConditionalInfo($"=============== Start getting map info ===============");
            var imagePath = $@"{Folders.Images}\{GetFileName(autoMode)}.png";
            var watch = Stopwatch.StartNew();
            CreateImageFromScreenArea(GetRectMultiplier(autoMode), imagePath);
            for (int scale = 1; scale <= (autoMode ? 1 : maxScale); scale++)
            {
                for (int treshold = autoMode ? maxTreshold : minTreshold; treshold >= minTreshold; treshold -= 50)
                {
                    Logger.ConditionalInfo($"===== Size = {scale}, Treshold = {treshold} =====");
                    RecognizeText(PreProcessImage(imagePath, scale, treshold, autoMode));
                    if (IsMapTextCorrect(autoMode))
                    {
                        Logger.ConditionalInfo("Map text is correct");
                        var mapInfo = ConvertTextToMapInfo(autoMode);
                        if (mapInfo.HasImage)
                        {
                            Logger.ConditionalInfo("Map has image file");
                            watch.Stop();
                            Logger.ConditionalInfo($"=============== Finish getting map info ===============");
                            Logger.ConditionalInfo($"=============== ({watch.ElapsedMilliseconds} ms) ===============");
                            return mapInfo;
                        }
                        if (scale == maxScale)
                        {
                            Logger.ConditionalWarn($"Map file for '{mapInfo.FullName}' doesn't exist");
                        }
                    }
                    else
                    {
                        Logger.ConditionalWarn("Incorrect map text:");
                        Logger.ConditionalWarn(text);
                    }
                }
            }
            watch.Stop();
            Logger.ConditionalInfo($"=============== Finish getting map info ===============");
            Logger.ConditionalInfo($"=============== ({watch.ElapsedMilliseconds} ms) ===============");
            return null;
        }

        private static void CreateImageFromScreenArea(RectMultiplier rectMultiplier, string path)
        {
            var imageWidth = (width * rectMultiplier.Width).Round();
            var imageHeight = (height * rectMultiplier.Height).Round();
            var rect = new Rectangle((width * rectMultiplier.X).Round(), (height * rectMultiplier.Y).Round(), imageWidth, imageHeight);
            Logger.ConditionalInfo($"Screen area: x = {rect.X}, y = {rect.Y}, width = {rect.Width}, height = {rect.Height}");

            var captureBitmap = new Bitmap(imageWidth, imageHeight, PixelFormat.Format32bppRgb);
            var captureGraphics = Graphics.FromImage(captureBitmap);
            captureGraphics.CopyFromScreen(rect.Left, rect.Top, 0, 0, rect.Size);
            captureBitmap.Save(path, ImageFormat.Png);

            captureGraphics.Dispose();
            captureBitmap.Dispose();
            Logger.ConditionalInfo($"Image is saved to '{path}'");
        }

        private static RectMultiplier GetRectMultiplier(bool autoMode)
        {
            return autoMode ? new RectMultiplier(0.04, 0.81, 0.56, 0.05) : new RectMultiplier(0.13, 0.62, 0.36, 0.14);
        }

        private static void RecognizeText(string imagePath)
        {
            var watch = Stopwatch.StartNew();
            var engine = new TesseractEngine(Folders.TessData, LanguagesManager.ConvertMexToSpa(Settings.Default.Language));
            var image = Pix.LoadFromFile(imagePath);
            text = engine.Process(image).GetText();
            watch.Stop();
            Logger.ConditionalInfo($"Text from image is recognized ({watch.ElapsedMilliseconds} ms)");
        }

        private static bool IsMapTextCorrect(bool autoMode = false)
        {
            return autoMode
                ? text.Length > 5 && text.Contains('\n')
                : text.Contains(LanguagesManager.GetMapInfoLocale()) && text.Contains('\n') && text.ContainsRegex(" - ");
        }

        private static MapInfo ConvertTextToMapInfo(bool autoMode = false)
        {
            var mapInfo = autoMode ? ConvertStartTextToMapInfo() : ConvertEscTextToMapInfo();

            Logger.ConditionalInfo("Text is converted to 'Map info' object");
            Logger.ConditionalInfo($"Map info: realm = {mapInfo.Realm}, name = {mapInfo.Name}");
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
            return new MapInfo(HandleBadhamIssues(mapName), true);
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

                Logger.ConditionalInfo("Specific symbols are replaced");
                Logger.ConditionalInfo($"Old map name: '{oldMapName}'");
                Logger.ConditionalInfo($"New map name: '{mapName}'");
            }
            mapName = ReplaceSymbol(mapName, "№", "IV");
            mapName = ReplaceSymbol(mapName, @"\/", "V");
            return mapName;
        }

        private static string ReplaceSymbol(string mapName, string oldString, string newString)
        {
            if (!mapName.EndsWith(oldString)) return mapName;
            if (oldString.StartsWith(@"\")) oldString = $@"\{oldString}";
            mapName = mapName.ReplaceRegex(oldString, newString);
            Logger.ConditionalInfo($"'{oldString}' symbol is replaced with '{newString}'");
            return mapName;
        }

        private static string PreProcessImage(string path, int scale = 1, int treshold = 400, bool autoMode = false)
        {
            var newPath = $@"{Folders.Images}\{GetFileName(autoMode)}_edited.png";
            var image = new Bitmap(path);
            image.Resize(scale).GrayScale().ApplyThreshold(treshold).Save(newPath);
            image.Dispose();
            Logger.ConditionalInfo($"Preprocessed image is saved to '{newPath}'");
            return newPath;
        }

        private static string GetFileName(bool autoMode = false)
        {
            return autoMode ? Settings.Default.AutoScreenshotFileName : Settings.Default.ManualScreenshotFileName;
        }
    }
}
