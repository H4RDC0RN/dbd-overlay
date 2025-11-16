using DBDOverlay.UI.Windows;
using System.Windows;

namespace DBDOverlay.Core.WindowControllers.Loading
{
    public class LoadingWindowController
    {
        private static LoadingWindowController instance;
        private static LoadingWindow window;

        public static LoadingWindowController Instance
        {
            get
            {
                if (instance == null)
                    instance = new LoadingWindowController();
                return instance;
            }
        }

        public static LoadingWindow Window
        {
            get
            {
                if (window == null)
                    window = new LoadingWindow();
                return window;
            }
        }

        public void SetStatus(string text)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Window.SetText(text);
            });
        }
    }
}
