using DBDOverlay.Core.MapOverlay;
using log4net;

namespace DBDOverlay.Core.Utils
{
    public static class Logger
    {
        private static ILog instance;

        private static ILog Log
        {
            get
            {
                if (instance == null)
                    instance = LogManager.GetLogger(typeof(Logger));
                return instance;
            }
        }

        public static void Info(string message)
        {
            Log.Info(message);
        }

        public static void Warn(string message)
        {
            Log.Warn(message);
        }

        public static void Error(string message)
        {
            Log.Error(message);
        }

        public static void Fatal(string message)
        {
            Log.Fatal(message);
        }

        public static void ConditionalInfo(string message)
        {
            if (!AutoModeManager.Instance.IsAutoMode) Log.Info(message);
        }

        public static void ConditionalWarn(string message)
        {
            if (!AutoModeManager.Instance.IsAutoMode) Log.Warn(message);
        }
    }
}
