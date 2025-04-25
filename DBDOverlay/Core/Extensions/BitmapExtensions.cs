using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
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
                    if (TotalRGB <= thresholdValue)
                    {
                        ptr[2] = 0;
                        ptr[1] = 0;
                        ptr[0] = 0;
                    }
                    else
                    {
                        ptr[2] = 255;
                        ptr[1] = 255;
                        ptr[0] = 255;
                    }
                    ptr += 4;
                }
            }
            bitmap.UnlockBits(bmpData);
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
