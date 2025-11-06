namespace DBDOverlay.Core.ImageProcessing
{
    public class HookData
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public double Equality { get; set; }

        public HookData(int x, int y, int width, int height, double equality)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Equality = equality;
        }
    }
}
