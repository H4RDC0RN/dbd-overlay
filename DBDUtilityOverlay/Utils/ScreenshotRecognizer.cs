﻿using DBDUtilityOverlay.Utils.Extensions;
using DBDUtilityOverlay.Utils.Models;
using OpenCvSharp;
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
        private static readonly string trainedDataFileName = "eng.traineddata";
        private static readonly string notReadyFileName = "NotReady";
        private static int width;
        private static int height;
        private static readonly int tries = 3;
        private static readonly int maxSizeMultiplier = 3;

        public static void SetScreenBounds()
        {
            var bounds = Screen.PrimaryScreen?.Bounds;
            if (bounds != null)
            {
                width = bounds.Value.Width;
                height = bounds.Value.Height;
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

        public static void CreateImageMapNameEsc(string path, int sizeMultiplier = 1)
        {
            double xMultiplier = 0.13;
            double yMultiplier = 0.62;
            double wMultiplier = 0.36;
            double hMultiplier = 0.14;
            var imageWidth = (width * wMultiplier).Round();
            var imageHeight = (height * hMultiplier).Round();
            var rect = new Rectangle((width * xMultiplier).Round(), (height * yMultiplier).Round(), imageWidth, imageHeight);

            var captureBitmap = new Bitmap(imageWidth, imageHeight, PixelFormat.Format32bppRgb);
            var captureGraphics = Graphics.FromImage(captureBitmap);
            captureGraphics.CopyFromScreen(rect.Left, rect.Top, 0, 0, rect.Size);
            captureBitmap.Save(path, ImageFormat.Png);
            PreProcessImage(path, sizeMultiplier);
        }

        public static void CreateTrainedData()
        {
            Directory.CreateDirectory(tessdata.ToProjectPath());
            File.WriteAllBytes($"{tessdata.ToProjectPath()}/{trainedDataFileName}", TrainedData.eng);
        }

        public static string RecognizeText(string imagePath)
        {
            var engine = new TesseractEngine(tessdata.ToProjectPath(), "eng");
            var image = Pix.LoadFromFile(imagePath);
            return engine.Process(image).GetText();
        }

        public static MapInfo? GetMapInfo()
        {
            var imagePath = $"{Properties.Settings.Default.ScreenshotFileName}.png".ToProjectPath();
            string text = string.Empty;
            for (int i = 1; i <= maxSizeMultiplier; i++)
            {
                for (int j = 0; j < tries; j++)
                {
                    CreateImageMapNameEsc(imagePath, i);
                    text = RecognizeText(imagePath);
                    if (IsTextCorrect(text) && ConvertTextToMapInfo(text).HasImage) goto Finish;
                }
            }
            if (!IsTextCorrect(text)) return null;
            if (!ConvertTextToMapInfo(text).HasImage) return new MapInfo(string.Empty, notReadyFileName);

            Finish:
            return ConvertTextToMapInfo(text);
        }

        private static bool IsTextCorrect(string text)
        {
            return text.Contains("MAP INFO") && text.Contains('\n') && text.Contains('-');
        }

        private static MapInfo ConvertTextToMapInfo(string text)
        {
            var res = text.Split(" - ");
            var realm = res[0].Split('\n').Last().RemoveRegex("'").Replace(" ", "_").Replace("é", "e").ToUpper();
            var mapName = res[1].Split('\n')[0].RemoveRegex("'").Replace(" ", "_").ToUpper();
            return new MapInfo(realm, HandleBadhamIssues(mapName));
        }

        private static string HandleBadhamIssues(string mapName)
        {
            var tessWicknessPattern = @"(I|L|\||1)";
            var badhamSuffixPattern = $"_{tessWicknessPattern}{{1,3}}$";
            if (mapName.ContainsRegex(badhamSuffixPattern))
            {
                var suffix = Regex.Match(mapName, badhamSuffixPattern).Value.Replace("|", @"\|");
                mapName = $"{mapName.RemoveRegex(suffix)}{suffix.ReplaceRegex(tessWicknessPattern, "I")}".RemoveRegex(@"\\");
            }
            return mapName;
        }

        private static void PreProcessImage(string path, int sizeMultiplier = 1)
        {
            var cvImage = Cv2.ImRead(path);
            Cv2.Resize(cvImage, cvImage, new Size(cvImage.Width * sizeMultiplier, cvImage.Height * sizeMultiplier));
            Cv2.FastNlMeansDenoisingColored(cvImage, cvImage);
            Cv2.CvtColor(cvImage, cvImage, ColorConversionCodes.BGR2GRAY);
            Cv2.Threshold(cvImage, cvImage, 100, 255, ThresholdTypes.Binary);
            cvImage.SaveImage(path);
        }
    }
}
