using DBDUtilityOverlay.Utils.Languages;
using DBDUtilityOverlay.Utils;
using System.Windows;
using System.Windows.Controls;
using UserControl = System.Windows.Controls.UserControl;
using System.Net.Http;

namespace DBDUtilityOverlay.MVVM.View
{
    public partial class SettingsTabView : UserControl
    {
        private readonly string downloadLink = "https://raw.github.com/tesseract-ocr/tessdata/main/";
        private readonly string fileExtension = ".traineddata";
        private string language;

        public SettingsTabView()
        {
            InitializeComponent();
            SetLanguages();
            SetDownloadLanguages();
        }

        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var newLanguage = LanguagesManager.GetValue(LanguageComboBox.SelectedItem.ToString());
            Properties.Settings.Default.Language = newLanguage;
            Properties.Settings.Default.Save();
        }

        private void DownloadLanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var value = LanguagesManager.GetValue(DownloadLanguageComboBox.SelectedItem.ToString());
            language = value.Equals(LanguagesManager.MexAbb) ? LanguagesManager.SpaAbb : value;
        }

        private void Download_Click(object sender, RoutedEventArgs e)
        {
            DownloadLanguageData();
            SetLanguages();
        }

        private void SetLanguages()
        {
            var downloadedLanguages = ScreenshotRecognizer.GetDownloadedLanguages();
            if (downloadedLanguages.Contains(LanguagesManager.SpaAbb)) downloadedLanguages.Add(LanguagesManager.MexAbb);
            var languages = LanguagesManager.GetOrderedKeyValuePairs(downloadedLanguages);
            LanguageComboBox.ItemsSource = languages.Select(x => x.Key);
            LanguageComboBox.SelectedIndex = languages.Select(x => x.Value).ToList().IndexOf(Properties.Settings.Default.Language);
        }

        private void SetDownloadLanguages()
        {
            DownloadLanguageComboBox.ItemsSource = LanguagesManager.GetLanguages();
            DownloadLanguageComboBox.SelectedIndex = LanguagesManager.GetCurrentLanguageIndex();
        }

        private void DownloadLanguageData()
        {
            if (ScreenshotRecognizer.GetDownloadedLanguages().Contains(language)) return;
            using HttpClient client = new();
            var url = $"{downloadLink}{language}{fileExtension}";
            var content = client.GetByteArrayAsync(url).Result;
            var fileName = $"{language}{fileExtension}";
            ScreenshotRecognizer.SaveTrainedData(fileName, content);
        }
    }
}
