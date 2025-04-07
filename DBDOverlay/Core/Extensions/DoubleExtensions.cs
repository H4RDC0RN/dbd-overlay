using System;

namespace DBDOverlay.Core.Extensions
{
    public static class DoubleExtensions
    {
        public static int Round(this double value)
        {
            return (int)Math.Round(value, MidpointRounding.AwayFromZero);
        }

        public static double Round(this double value, int decCount)
        {
            return Math.Round(value, decCount);
        }
    }
}
