using System;
using System.Drawing;

namespace DBDOverlay.Core.ImageProcessing
{
    public class UpdateImageEventArgs : EventArgs
    {
        public Bitmap Image { get; }
        public int Threshold { get; }

        internal UpdateImageEventArgs(Bitmap image, int threshold)
        {
            Image = image;
            Threshold = threshold;
        }
    }
}
