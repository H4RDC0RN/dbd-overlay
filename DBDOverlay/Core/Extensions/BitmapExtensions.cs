using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace DBDOverlay.Core.Extensions
{
    public static class BitmapExtensions
    {
        public static BitmapImage ToBitmapImage(this Bitmap bitmap)
        {
            using var memory = new MemoryStream();
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

        public static Bitmap GrayScale(this Bitmap bitmap)
        {
            for (int y = 0; y < bitmap.Height; y++)
                for (int x = 0; x < bitmap.Width; x++)
                {
                    var c = bitmap.GetPixel(x, y);
                    var rgb = (c.R + c.G + c.B) / 3;
                    bitmap.SetPixel(x, y, Color.FromArgb(rgb, rgb, rgb));
                }
            return bitmap;
        }

        public static Bitmap ApplyThreshold(this Bitmap bitmap, short thresholdValue = 400)
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

        public static Bitmap Resize(this Bitmap bitmap, int scale)
        {
            var newWidth = bitmap.Width * scale;
            var newHeight = bitmap.Height * scale;
            var result = new Bitmap(newWidth, newHeight);
            using (Graphics graphics = Graphics.FromImage(result))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawImage(bitmap, 0, 0, newWidth, newHeight);
            }
            return result;
        }
    }
}
