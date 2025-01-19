using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DBDUtilityOverlay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MapOverlay overlay;
        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            WindowStyle = WindowStyle.None;

            overlay = new MapOverlay();
            overlay.WindowStyle = WindowStyle.None;
            overlay.AllowsTransparency = true;
            overlay.Opacity = 0.9;
            overlay.ShowInTaskbar = false;
            overlay.Topmost = true;
            overlay.IsHitTestVisible = false;
            overlay.Left = SystemParameters.PrimaryScreenWidth - overlay.Width;
            overlay.Top = 0;
        }

        private void OpenButtonClick(object sender, RoutedEventArgs e)
        {
            overlay.Show();
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            overlay.Hide();
        }

        private void ExitButtonClick(object sender, RoutedEventArgs e)
        {
            overlay.Close();
            Close();
            Application.Current.Shutdown();
        }
    }
}