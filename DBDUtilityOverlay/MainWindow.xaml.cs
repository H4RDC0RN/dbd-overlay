using DBDUtilityOverlay.Utils;
using DBDUtilityOverlay.Utils.Extensions;
using System.Windows;
using System.Windows.Input;
using Tesseract;

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

        private void WindowMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }

        private void ExitButtonClick(object sender, RoutedEventArgs e)
        {
            overlay.Close();
            Close();
            Application.Current.Shutdown();
        }

        private void MinButtonClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void OpenButtonClick(object sender, RoutedEventArgs e)
        {
            overlay.Show();
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            overlay.Hide();
        }

        private void ReadButtonClick(object sender, RoutedEventArgs e)
        {
            var imagePath = @"Images\Recognition\test.jpg".ToProjectPath();
            ScreenshotRecognizer.GetmageMapNameEsc(imagePath);
            var engine = new TesseractEngine("Tessdata".ToProjectPath(), "eng");
            var image = Pix.LoadFromFile(imagePath);
            var text = engine.Process(image).GetText();
            var mapName = text.Split('-')[1].Split('\n')[0].Remove(" ").ToUpper();

            overlay.ChangeMap(mapName);
        }
    }
}