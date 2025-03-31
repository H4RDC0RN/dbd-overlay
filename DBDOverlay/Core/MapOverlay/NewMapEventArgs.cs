using DBDOverlay.Core.Utils;
using System;

namespace DBDOverlay.Core.MapOverlay
{
    public class NewMapEventArgs : EventArgs
    {
        public MapInfo MapInfo { get; }

        internal NewMapEventArgs(MapInfo mapInfo)
        {
            MapInfo = mapInfo;
        }

        public void Log()
        {
            Logger.Info("Auto mode detected new map");
            Logger.Info("=====Map info=====");
            Logger.Info($"-----Realm: {MapInfo.Realm}");
            Logger.Info($"-----Name: {MapInfo.Name}");
            Logger.Info("=====Recognize details=====");
            Logger.Info($"-----Scale: {MapInfo.Scale}");
            Logger.Info($"-----Treshold: {MapInfo.Treshold}");
            Logger.Info($"-----Time: {MapInfo.Time} ms");
        }
    }
}
