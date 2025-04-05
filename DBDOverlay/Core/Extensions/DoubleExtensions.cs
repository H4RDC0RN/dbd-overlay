using System;

namespace DBDOverlay.Core.Extensions
{
    public static class DoubleExtensions
    {
        public static int Round(this double value)
        {
            return (int)Math.Round(value, MidpointRounding.AwayFromZero);
        }

        public static double RoundTwoDec(this double value)
        {
            return Math.Round(value * 100) / 100;
        }
    }
}
