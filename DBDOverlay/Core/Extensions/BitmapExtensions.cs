using DBDOverlay.Core.Utils;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace DBDOverlay.Core.Extensions
{
    public static class BitmapExtensions
    {
        public static BitmapImage ToBitmapImage(this Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }

        public static Bitmap Resize(this Bitmap bitmap, double scale)
        {
            if (scale == 1) return bitmap;

            var newWidth = bitmap.Width * scale;
            var newHeight = bitmap.Height * scale;
            return Resize(bitmap, newWidth, newHeight);
        }

        public static Bitmap Resize(this Bitmap bitmap, double newWidth, double newHeight)
        {
            var result = new Bitmap((int)newWidth, (int)newHeight);
            using (var graphics = Graphics.FromImage(result))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawImage(bitmap, 0, 0, (int)newWidth, (int)newHeight);
            }
            return result;
        }

        public static Bitmap ToBlackWhite(this Bitmap bitmap, int thresholdValue)
        {
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            int size = bitmapData.Stride * bitmapData.Height;
            byte[] data = new byte[size];
            Marshal.Copy(bitmapData.Scan0, data, 0, size);

            for (int i = 0; i < size; i += 4)
            {
                var totalRGB = data[i] + data[i + 1] + data[i + 2];
                if (totalRGB <= thresholdValue)
                {
                    data[i] = 0;
                    data[i + 1] = 0;
                    data[i + 2] = 0;
                }
                else
                {
                    data[i] = 255;
                    data[i + 1] = 255;
                    data[i + 2] = 255;
                }
            }
            Marshal.Copy(data, 0, bitmapData.Scan0, data.Length);

            bitmap.UnlockBits(bitmapData);
            return bitmap;
        }

        public static Bitmap PreProcess(this Bitmap bitmap, double scale = 1, int threshold = 400, bool save = true)
        {
            bitmap = bitmap.Resize(scale).ToBlackWhite(threshold);
            if (save)
            {
                var newPath = FileSystem.GetImagePath(edited: true);
                bitmap.Save(newPath);
                Logger.Info($"Preprocessed image is saved to '{newPath}'");
            }
            return bitmap;
        }

        public static double Compare(this Bitmap bitmap, Bitmap bitmapToCompare)
        {
            var current = bitmap.ToHashMap();
            var toCompare = bitmapToCompare.ToHashMap();
            var equals = current.Zip(toCompare, (i, j) => i == j).Count(x => x);
            var result = equals / (double)current.Count;
            return result.Round(2);
        }

        private static List<bool> ToHashMap(this Bitmap bitmap, int thresholdValue = 400)
        {
            var hashMap = new List<bool>();
            var bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);

            unsafe
            {
                int TotalRGB;
                byte* ptr = (byte*)bmpData.Scan0.ToPointer();
                int stopAddress = (int)ptr + bmpData.Stride * bmpData.Height;

                while ((int)ptr <= stopAddress)
                {
                    TotalRGB = ptr[0] + ptr[1] + ptr[2];
                    hashMap.Add(TotalRGB <= thresholdValue);
                    ptr += 4;
                }
            }
            bitmap.UnlockBits(bmpData);
            return hashMap;
        }
    }
}
