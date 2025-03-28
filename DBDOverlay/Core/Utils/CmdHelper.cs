using System.Diagnostics;

namespace DBDOverlay.Core.Utils
{
    public static class CmdHelper
    {
        public static void RunCommand(string line)
        {
            Logger.Log.Info("Run cmd command");
            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd",
                Arguments = $"/c {line} | taskkill /F /IM cmd.exe",
                WindowStyle = ProcessWindowStyle.Hidden
            });
        }
    }
}
