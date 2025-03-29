using System;

namespace DBDOverlay.Core.MapOverlay
{
    public class AutoModeEventArgs : EventArgs
    {
        public MapInfo MapInfo { get; }

        internal AutoModeEventArgs(MapInfo mapInfo)
        {
            MapInfo = mapInfo;
        }
    }
}
