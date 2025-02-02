using DBDUtilityOverlay.Utils;
using DBDUtilityOverlay.Utils.Models;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DBDUtilityOverlay
{
    public partial class DownloadLanguage : Window
    {
        private readonly string downloadLink = "https://raw.github.com/tesseract-ocr/tessdata/main/";
        private readonly string fileExtension = ".traineddata";
        private string? language;

        public DownloadLanguage()
        {
            InitializeComponent();
            SetLanguages();
            WindowStyle = WindowStyle.None;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void Download_Click(object sender, RoutedEventArgs e)
        {
            using (HttpClient client = new())
            {
                var url = $"{downloadLink}{language}{fileExtension}";
                var content = client.GetByteArrayAsync(url).Result;
                var fileName = $"{language}{fileExtension}";
                ScreenshotRecognizer.SaveTrainedData(fileName, content);
            }
        }

        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            language = Languages.GetValue(LanguageComboBox.SelectedItem.ToString());
        }

        private void SetLanguages()
        {
            LanguageComboBox.ItemsSource = Languages.Dictionary.Keys;
            LanguageComboBox.SelectedIndex = Languages.GetCurrentLanguageIndex();
        }

        private void WindowMouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ExitButtonClick(object sender, RoutedEventArgs e)
        {
            Hide();
        }
    }
}
