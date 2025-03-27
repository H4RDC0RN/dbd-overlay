using log4net;

namespace DBDOverlay.Core.Utils
{
    public static class Logger
    {
        private static ILog instance;

        public static ILog Log
        {
            get
            {
                if (instance == null)
                    instance = LogManager.GetLogger(typeof(Logger));
                return instance;
            }
        }
    }
}
