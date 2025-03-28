using System.Diagnostics;

namespace DBDOverlay.Core.Utils
{
    public static class CmdHelper
    {
        public static void RunCommand(string line)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd",
                Arguments = $"/c {line} | taskkill /F /IM cmd.exe",
                WindowStyle = ProcessWindowStyle.Hidden
            });
        }
    }
}
