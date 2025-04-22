using DBDOverlay.Core.Extensions;
using DBDOverlay.Core.Windows;
using System.Windows.Forms;

namespace DBDOverlay.Core.Reshade
{
    public class ReshadeHotKey
    {
        public Keys Key { get; set; }
        public bool Ctrl { get; set; }
        public bool Shift { get; set; }
        public bool Alt { get; set; }

        public ReshadeHotKey(Keys key, bool ctrl, bool shift, bool alt)
        {
            Key = key;
            Ctrl = ctrl;
            Shift = shift;
            Alt = alt;
        }

        public void Press()
        {
            WindowsServices.Instance.Send($"{(Ctrl ? "^" : string.Empty)}{(Shift ? "+" : string.Empty)}{(Alt ? "%" : string.Empty)}{Key.ToStringKey()}");
        }
    }
}
