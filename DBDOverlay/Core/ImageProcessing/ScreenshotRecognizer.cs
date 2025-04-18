using DBDOverlay.Core.Extensions;
using DBDOverlay.Core.KillerOverlay;
using DBDOverlay.Core.MapOverlay;
using DBDOverlay.Core.MapOverlay.Languages;
using DBDOverlay.Core.Utils;
using DBDOverlay.Images.SurvivorStates;
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
using System.Collections.Generic;

namespace DBDOverlay.Core.ImageProcessing
{
    public static class ScreenshotRecognizer
    {
        private static int width;
        private static int height;
        private static readonly double maxScaleManual = 3;
        private static readonly double maxScaleAuto = 1.5;
        private static readonly double scaleStepManual = 1;
        private static readonly double scaleStepAuto = 0.5;
        private static readonly int maxThreshold = 750;
        private static readonly int minThreshold = 400;
        private static readonly int thresholdStep = 50;
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
            var imagePath = GetImagePath(GetFileName(autoMode));
            var watch = Stopwatch.StartNew();
            CreateImageFromScreenArea(autoMode ? RectType.Auto : RectType.Manual, imagePath, log);

            var maxScale = autoMode ? maxScaleAuto : maxScaleManual;
            var scaleStep = autoMode ? scaleStepAuto : scaleStepManual;
            for (double scale = 1; scale <= maxScale; scale += scaleStep)
            {
                for (int threshold = autoMode ? maxThreshold : minThreshold; threshold >= minThreshold; threshold -= thresholdStep)
                {
                    if (log) Logger.Info($"===== Size = {scale}, Threshold = {threshold} =====");
                    RecognizeText(PreProcessImage(imagePath, scale, threshold, true, log), log);
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
                            mapInfo.Threshold = threshold;
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

        public static void HandleSurvivors(bool savePieces = false)
        {
            int parts = 4;
            var path = GetImagePath(Settings.Default.SurvivorsScreenshotName);
            CreateImageFromScreenArea(RectType.Survivors, path, false);
            var newPath = PreProcessImage(path, threshold: 600, saveAsNew: true, log: false);
            var image = new Bitmap(newPath);

            var width = image.Width;
            var height = image.Height / parts;
            var statusRectMulti = GetRectMultiplier(RectType.State);
            var srcRect = GetRect(statusRectMulti, width, height);
            var destRect = GetRect(new RectMultiplier(0, 0, statusRectMulti.Width, statusRectMulti.Height), width, height);
            var piece = new Bitmap(destRect.Width, destRect.Height);

            using (Graphics graphics = Graphics.FromImage(piece))
            {
                for (int i = 0; i < parts; i++)
                {
                    graphics.DrawImage(image, destRect, srcRect, GraphicsUnit.Pixel);

                    if (savePieces) piece.Save(GetImagePath($"survivor_{i}"), ImageFormat.Png);

                    KillerOverlayController.Instance.CheckIfHooked(i, piece.Compare(SurvivorStates.Hooked));
                    KillerOverlayController.Instance.CheckIfUnhooked(i, piece.Compare(SurvivorStates.Hooked));

                    var refreshStates = new Dictionary<string, double>
                    {
                        { "Sacrificed", piece.Compare(SurvivorStates.Sacrificed) },
                        { "Escaped", piece.Compare(SurvivorStates.Escaped) },
                        { "Dead", piece.Compare(SurvivorStates.Dead) }
                    };

                    KillerOverlayController.Instance.CheckIfRefreshed(i, refreshStates);
                    srcRect.Y += height;
                }
            }
            image.Dispose();
            piece.Dispose();
        }

        public static Rectangle GetRect(RectType rectType, int w = 0, int h = 0)
        {
            return GetRect(GetRectMultiplier(rectType), w, h);
        }

        public static Rectangle GetRect(RectMultiplier rectMultiplier, int w = 0, int h = 0)
        {
            if (w == 0) w = width;
            if (h == 0) h = height;
            var imageWidth = (w * rectMultiplier.Width).Round();
            var imageHeight = (h * rectMultiplier.Height).Round();
            return new Rectangle((w * rectMultiplier.X).Round(), (h * rectMultiplier.Y).Round(), imageWidth, imageHeight);
        }

        private static void CreateImageFromScreenArea(RectType rectType, string path, bool log = true)
        {
            var rect = GetRect(rectType);
            if (log) Logger.Info($"Screen area: x = {rect.X}, y = {rect.Y}, width = {rect.Width}, height = {rect.Height}");

            var image = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppRgb);
            var graphics = Graphics.FromImage(image);
            graphics.CopyFromScreen(rect.Left, rect.Top, 0, 0, rect.Size);
            image.Save(path, ImageFormat.Png);

            graphics.Dispose();
            image.Dispose();
            if (log) Logger.Info($"Image is saved to '{path}'");
        }

        private static RectMultiplier GetRectMultiplier(RectType rectType)
        {
            switch (rectType)
            {
                case RectType.Manual: return new RectMultiplier(0.13, 0.62, 0.36, 0.14);
                case RectType.Auto: return new RectMultiplier(0.04, 0.81, 0.56, 0.05);
                case RectType.Survivors: return new RectMultiplier(0.04, 0.38, 0.055, 0.326);
                case RectType.State: return new RectMultiplier(0.32, 0.25, 0.31, 0.5);
                default: return null;
            }
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
                ? text.Length > 5 && text.ContainsRegex(@"\w")
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
            var mapName = text.RemoveRegex(@"\n|'|\.").Replace(" ", "_").RemoveRegex(@"^_{1,}").ToUpper();
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

        private static string PreProcessImage(string path, double scale = 1, int threshold = 400, bool saveAsNew = false, bool log = true)
        {
            var newPath = saveAsNew ? path.ReplaceRegex($@"\.{png}", $"_edited.{png}") : path;
            var image = new Bitmap(path);
            image.Resize(scale).ToBlackWhite(threshold).Save(newPath);
            image.Dispose();
            if (log) Logger.Info($"Preprocessed image is saved to '{newPath}'");
            return newPath;
        }

        private static string GetImagePath(string imageName)
        {
            return $@"{Folders.Images}\{imageName}.{png}";
        }

        private static string GetFileName(bool autoMode = false)
        {
            return autoMode ? Settings.Default.AutoScreenshotFileName : Settings.Default.ManualScreenshotFileName;
        }
    }
}
