using DBDOverlay.Core.Extensions;
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
using System;
using DBDOverlay.Images.Identificators;
using DBDOverlay.Core.WindowControllers.KillerOverlay;
using DBDOverlay.Core.WindowControllers.MapOverlay;
using DBDOverlay.Core.WindowControllers.MapOverlay.Languages;

namespace DBDOverlay.Core.ImageProcessing
{
    public class ImageReader
    {
        private int width;
        private int height;
        private readonly double maxScaleManual = 3;
        private readonly double maxScaleAuto = 1.0;
        private readonly double scaleStepManual = 1;
        private readonly double scaleStepAuto = 0.5;
        private readonly int maxThresholdForAuto = 750;
        private readonly int maxThresholdForManual = 400;
        private readonly int minThreshold = 300;
        private readonly int thresholdStep = 50;
        private readonly int gearThreshold = 400;
        private string text = string.Empty;
        private int hooksThreshold;

        private TesseractEngine engine;
        private static ImageReader instance;

        public static ImageReader Instance
        {
            get
            {
                if (instance == null)
                    instance = new ImageReader();
                return instance;
            }
        }

        public void Initialize()
        {
            SetScreenBounds();
            SetEngine();
            SetHooksThreshold(Settings.Default.HooksThreshold);
        }

        public void SetEngine()
        {
            engine = new TesseractEngine(FileSystem.TessData, LanguagesManager.ConvertMexToSpa(Settings.Default.Language));
        }

        public MapInfo GetMapInfo(bool autoMode = false)
        {
            var log = !autoMode;
            if (log) Logger.Info($"=============== Start getting map info ===============");
            var watch = Stopwatch.StartNew();
            var bitmap = CreateFromScreenArea(autoMode ? RectType.Auto : RectType.Manual, log);

            var maxScale = autoMode ? maxScaleAuto : maxScaleManual;
            var scaleStep = autoMode ? scaleStepAuto : scaleStepManual;
            for (double scale = 1; scale <= maxScale; scale += scaleStep)
            {
                for (int threshold = autoMode ? maxThresholdForAuto : maxThresholdForManual; threshold >= minThreshold; threshold -= thresholdStep)
                {
                    if (log) Logger.Info($"===== Size = {scale}, Threshold = {threshold} =====");
                    var saveName = autoMode ? null : Settings.Default.ManualScreenshotFileName;
                    RecognizeText(bitmap.PreProcess(scale, threshold, saveName), log);
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
                            bitmap.Dispose();
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
            bitmap.Dispose();
            return null;
        }

        public void HandleSurvivors(bool saveImages = false)
        {
            var watch = Stopwatch.StartNew();
            int survCount = 4;
            //var bitmap = new Bitmap(@"D:\survivorsSS.png");
            var bitmap = CreateFromScreenArea(RectType.Survivors, false).PreProcess(threshold: hooksThreshold, imageName: saveImages ? "survivors_area" : null);

            var width = bitmap.Width;
            var height = bitmap.Height / survCount;
            var statusRectMulti = GetRectMultiplier(RectType.State);
            var srcRect = GetRect(statusRectMulti, width, height);
            var destRect = GetRect(new RectMultiplier(0, 0, statusRectMulti.Width, statusRectMulti.Height), width, height);
            var hooked = SurvivorStates.Hooked;
            //var hookedHuge = new Bitmap(@"D:\survivorhuge.png");

            for (int i = 0; i < survCount; i++)
            {
                var piece = new Bitmap(destRect.Width, destRect.Height);
                using (Graphics graphics = Graphics.FromImage(piece))
                {
                    graphics.DrawImage(bitmap, destRect, srcRect, GraphicsUnit.Pixel);
                }

                //if (!piece.Width.Equals(hookedHuge.Width)) hookedHuge = hookedHuge.Resize(piece.Width, piece.Height).ToBlackWhite(400);
                //hookedHuge.Save(FileSystem.GetImagePath($"survivorOPA"), ImageFormat.Png);
                //var hookComparison = piece.Compare(hookedHuge);

                if (!piece.Width.Equals(hooked.Width)) piece = piece.Resize(hooked.Width, hooked.Height).ToBlackWhite(400);
                var hookComparison = Math.Max(Math.Max(piece.Compare(hooked), piece.Compare(SurvivorStates.Hooked2)), piece.Compare(SurvivorStates.Hooked3));

                if (saveImages)
                {
                    piece.Save(FileSystem.GetImagePath($"survivor_{i}"), ImageFormat.Png);
                    Logger.Info($"--- Survivor {i} 'Hooked' image similarity = {hookComparison * 100} %");
                }
                KillerOverlayController.Instance.CheckIfHooked(i, hookComparison);
                KillerOverlayController.Instance.CheckIfUnhooked(i, hookComparison);

                var refreshStates = new Dictionary<string, double>
                    {
                        { "Sacrificed", Math.Max(piece.Compare(SurvivorStates.Sacrificed), piece.Compare(SurvivorStates.Sacrificed2)) },
                        { "Escaped", piece.Compare(SurvivorStates.Escaped) },
                        { "Dead", piece.Compare(SurvivorStates.Dead) }
                    };

                KillerOverlayController.Instance.CheckIfRefreshed(i, refreshStates);
                srcRect.Y += height;
                piece.Dispose();
            }
            bitmap.Dispose();
            watch.Stop();
        }

        public void HandleSurvivorsSmart(bool saveImages = false)
        {
            var watch = Stopwatch.StartNew();
            int survCount = 4;
            //var bitmap = new Bitmap(@"D:\survivorsSS.png");
            var bitmap = CreateFromScreenArea(RectType.Survivors, false).PreProcess(threshold: hooksThreshold, imageName: saveImages ? "survivors_area" : null);
            if (saveImages) bitmap.Save(FileSystem.GetImagePath($"survivors_area"), ImageFormat.Png);

            var width = bitmap.Width;
            var height = bitmap.Height / survCount;
            var rect = new Rectangle(0, 0, width, height);
            var hooked = SurvivorStates.Hooked;
            //var hookedHuge = new Bitmap(@"D:\survivorhuge.png");

            for (int i = 0; i < survCount; i++)
            {
                var piece = new Bitmap(rect.Width, rect.Height);
                using (Graphics graphics = Graphics.FromImage(piece))
                {
                    graphics.DrawImage(bitmap, 0, 0, rect, GraphicsUnit.Pixel);
                }

                //var statusRectMulti = GetRectMultiplier(RectType.State);
                //var srcRect = GetRect(statusRectMulti, width, height);
                //if (!srcRect.Width.Equals(hookedHuge.Width)) hookedHuge = hookedHuge.Resize(srcRect.Width, srcRect.Height).ToBlackWhite(400);
                //hookedHuge.Save(FileSystem.GetImagePath($"survivorOPA"), ImageFormat.Png);

                var hookComparison = piece.Find(hooked);
                if (saveImages)
                {
                    piece.Save(FileSystem.GetImagePath($"survivor_{i}"), ImageFormat.Png);
                    Logger.Info($"--- Survivor {i} 'Hooked' image similarity = {hookComparison * 100} %");
                }
                KillerOverlayController.Instance.CheckIfHooked(i, hookComparison);
                KillerOverlayController.Instance.CheckIfUnhooked(i, hookComparison);

                var refreshStates = new Dictionary<string, double>
                    {
                        { "Sacrificed", piece.Find(SurvivorStates.Sacrificed) },
                        { "Escaped", piece.Find(SurvivorStates.Escaped) },
                        { "Dead", piece.Find(SurvivorStates.Dead) }
                    };

                KillerOverlayController.Instance.CheckIfRefreshed(i, refreshStates);
                rect.Y += height;
                piece.Dispose();
            }
            bitmap.Dispose();
            watch.Stop();
            //Logger.Info($"SMART {watch.ElapsedMilliseconds} ms");
        }

        public bool IsMatchFinished(bool saveImage = false)
        {
            var saveName = saveImage ? Settings.Default.GearScreenshotFileName : null;
            var bitmap = CreateFromScreenArea(RectType.Gear, false).PreProcess(threshold: gearThreshold, imageName: saveName);
            var similarity = bitmap.Compare(Identificators.Gear);
            var isFinished = similarity >= 0.99;
            bitmap.Dispose();
            if (isFinished) Logger.Info($"Match is finished. 'Gear' image similarity = {similarity} %");
            return isFinished;
        }

        public Rectangle GetRect(RectType rectType, int w = 0, int h = 0)
        {
            return GetRect(GetRectMultiplier(rectType), w, h);
        }

        public Rectangle GetRect(RectMultiplier rectMultiplier, int w = 0, int h = 0)
        {
            if (w == 0) w = width;
            if (h == 0) h = height;
            var imageWidth = (w * rectMultiplier.Width).Round();
            var imageHeight = (h * rectMultiplier.Height).Round();
            return new Rectangle((w * rectMultiplier.X).Round(), (h * rectMultiplier.Y).Round(), imageWidth, imageHeight);
        }

        public void SetHooksThreshold(int value)
        {
            hooksThreshold = value;
        }

        private void SetScreenBounds()
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

        private Bitmap CreateFromScreenArea(RectType rectType, bool save = true)
        {
            var rect = GetRect(rectType);
            var bitmap = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppRgb);
            var graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(rect.Left, rect.Top, 0, 0, rect.Size);
            graphics.Dispose();

            if (save)
            {
                var path = FileSystem.GetImagePath();
                bitmap.Save(path, ImageFormat.Png);
                Logger.Info($"Image is saved to '{path}'");
            }
            return bitmap;
        }

        private RectMultiplier GetRectMultiplier(RectType rectType)
        {
            switch (rectType)
            {
                case RectType.Manual: return new RectMultiplier(0.34, 0.8, 0.32, 0.08);
                case RectType.Auto: return new RectMultiplier(0.04, 0.81, 0.56, 0.05);
                case RectType.Gear: return new RectMultiplier(0.0472, 0.918, 0.0228, 0.0394);
                case RectType.Survivors: return new RectMultiplier(0.04, 0.38, 0.055, 0.326);
                case RectType.State: return new RectMultiplier(0.32, 0.25, 0.31, 0.5);
                default: return null;
            }
        }

        private void RecognizeText(Bitmap bitmap, bool log = true)
        {
            var watch = Stopwatch.StartNew();
            var pixImage = PixConverter.ToPix(bitmap);
            SetText(pixImage);
            watch.Stop();
            if (log) Logger.Info($"Text from image is recognized ({watch.ElapsedMilliseconds} ms)");
        }

        private void SetText(Pix pixImage)
        {
            Page page;
            try
            {
                page = engine.Process(pixImage);
            }
            catch (InvalidOperationException)
            {
                SetEngine();
                page = engine.Process(pixImage);
            }
            text = page.GetText();
            page.Dispose();
        }

        private bool IsMapTextCorrect(bool autoMode = false)
        {
            return autoMode
                ? text.Length > 5 && text.ContainsRegex(@"\w")
                : text.Length > 5 && text.Contains('\n') && text.ContainsRegex(" - ");
        }

        private MapInfo ConvertTextToMapInfo(bool autoMode = false, bool log = true)
        {
            var mapInfo = autoMode ? ConvertStartTextToMapInfo() : ConvertEscTextToMapInfo();
            if (log) Logger.Info("Text is converted to 'Map info' object");
            if (log) Logger.Info($"Map info: realm = {mapInfo.Realm}, name = {mapInfo.Name}");
            return mapInfo;
        }

        private MapInfo ConvertEscTextToMapInfo()
        {
            var res = text.Split(" - ");
            var realm = res[0].RemoveRegex("'").Replace(" ", "_").ToUpper();
            var mapName = res[1].Split('\n')[0].RemoveRegex("'").Replace(" ", "_").ToUpper();
            return new MapInfo(realm, HandleBadhamIssues(mapName));
        }

        private MapInfo ConvertStartTextToMapInfo()
        {
            var mapName = text.RemoveRegex(@"\n|'|\.").Replace(" ", "_").RemoveRegex(@"^_{1,}").ToUpper();
            return new MapInfo(HandleBadhamIssues(mapName, false), true);
        }

        private string HandleBadhamIssues(string mapName, bool log = true)
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

        private string ReplaceSymbol(string mapName, string oldString, string newString, bool log = true)
        {
            if (!mapName.EndsWith(oldString)) return mapName;
            if (oldString.StartsWith(@"\")) oldString = $@"\{oldString}";
            mapName = mapName.ReplaceRegex(oldString, newString);
            if (log) Logger.Info($"'{oldString}' symbol is replaced with '{newString}'");
            return mapName;
        }
    }
}
