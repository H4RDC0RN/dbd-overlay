using System;
using System.Drawing;

namespace DBDOverlay.Core.ImageProcessing
{
    public class UpdateImageEventArgs : EventArgs
    {
        public Bitmap Image { get; }

        internal UpdateImageEventArgs(Bitmap image)
        {
            Image = image;
        }
    }
}
