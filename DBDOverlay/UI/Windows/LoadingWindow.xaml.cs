using System.Windows;

namespace DBDOverlay.UI.Windows
{
    public partial class LoadingWindow : Window
    {
        public LoadingWindow()
        {
            InitializeComponent();
        }

        public void SetText(string text)
        {
            StatusTextBlock.Text = text;
        }
    }
}
