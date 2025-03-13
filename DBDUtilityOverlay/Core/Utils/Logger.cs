using log4net;

namespace DBDUtilityOverlay.Core.Utils
{
    public static class Logger
    {
        private static ILog instance;

        public static ILog Log
        {
            get
            {
                instance ??= LogManager.GetLogger(typeof(Logger));
                return instance;
            }
        }
    }
}
