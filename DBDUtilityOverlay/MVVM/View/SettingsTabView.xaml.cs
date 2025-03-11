using DBDUtilityOverlay.Utils.Languages;
using DBDUtilityOverlay.Utils;
using System.Windows;
using System.Windows.Controls;
using UserControl = System.Windows.Controls.UserControl;
using System.Net.Http;
using System.Windows.Input;

namespace DBDUtilityOverlay.MVVM.View
{
    public partial class SettingsTabView : UserControl
    {
        private string language;
        private readonly string downloadLink = "https://raw.github.com/tesseract-ocr/tessdata/main/";
        private readonly string fileExtension = ".traineddata";
        
        private readonly Dictionary<ModifierKeys, string> modifiers = [];

        public SettingsTabView()
        {
            InitializeComponent();
            SetLanguages();
            SetDownloadLanguages();
            SetModifiers();
        }

        private void SetModifiers()
        {
            InitializeModifiers();

            ReadModifierComboBox.ItemsSource = modifiers.Values;
            NextModifierComboBox.ItemsSource = modifiers.Values;
            PreviousModifierComboBox.ItemsSource = modifiers.Values;

            ReadModifierComboBox.SelectedIndex = modifiers.Keys.ToList().IndexOf((ModifierKeys)Properties.Settings.Default.ReadModifier); 
            NextModifierComboBox.SelectedIndex = modifiers.Keys.ToList().IndexOf((ModifierKeys)Properties.Settings.Default.NextMapModifier); 
            PreviousModifierComboBox.SelectedIndex = modifiers.Keys.ToList().IndexOf((ModifierKeys)Properties.Settings.Default.PreviousMapModifier); 
        }

        private void InitializeModifiers()
        {
            modifiers.Add(ModifierKeys.None, "None");
            modifiers.Add(ModifierKeys.Alt, "Alt");
            modifiers.Add(ModifierKeys.Control, "Ctrl");
            modifiers.Add(ModifierKeys.Shift, "Shift");
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
            DownloadLanguageComboBox.SelectedIndex = 0;
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

        private void ReadModifierComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var newReadModifier =  modifiers.FirstOrDefault(x => x.Value.Equals(ReadModifierComboBox.SelectedItem.ToString())).Key;
            HotKeysController.UnregisterHotKey(HotKeyType.Read);
            HotKeysController.RegisterHotKey(HotKeyType.Read, newReadModifier, (Keys)Properties.Settings.Default.ReadKey);
            Properties.Settings.Default.ReadModifier = (uint)newReadModifier;
            Properties.Settings.Default.Save();
        }

        private void NextModifierComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var newNextModifier = modifiers.FirstOrDefault(x => x.Value.Equals(NextModifierComboBox.SelectedItem.ToString())).Key;
            HotKeysController.UnregisterHotKey(HotKeyType.NextMap);
            HotKeysController.RegisterHotKey(HotKeyType.NextMap, newNextModifier, (Keys)Properties.Settings.Default.NextMapKey);
            Properties.Settings.Default.NextMapModifier = (uint)newNextModifier;
            Properties.Settings.Default.Save();
        }

        private void PreviousModifierComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var newPreviousModifier = modifiers.FirstOrDefault(x => x.Value.Equals(PreviousModifierComboBox.SelectedItem.ToString())).Key;
            HotKeysController.UnregisterHotKey(HotKeyType.PreviousMap);
            HotKeysController.RegisterHotKey(HotKeyType.PreviousMap, newPreviousModifier, (Keys)Properties.Settings.Default.PreviousMapKey);
            Properties.Settings.Default.PreviousMapModifier = (uint)newPreviousModifier;
            Properties.Settings.Default.Save();
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
