using System.Windows;

namespace DBDOverlay.Windows
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
