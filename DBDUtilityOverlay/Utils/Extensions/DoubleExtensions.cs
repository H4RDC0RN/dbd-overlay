﻿namespace DBDUtilityOverlay.Utils.Extensions
{
    public static class DoubleExtensions
    {
        public static int Round(this double value)
        {
            return (int)Math.Round(value, MidpointRounding.AwayFromZero);
        }
    }
}
