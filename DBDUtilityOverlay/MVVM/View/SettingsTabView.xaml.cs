using DBDUtilityOverlay.Core.Languages;
using System.Windows;
using System.Windows.Controls;
using UserControl = System.Windows.Controls.UserControl;
using System.Net.Http;
using System.Windows.Input;
using DBDUtilityOverlay.Core.Windows;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using ComboBox = System.Windows.Controls.ComboBox;
using TextBox = System.Windows.Controls.TextBox;
using DBDUtilityOverlay.Core.Utils;
using DBDUtilityOverlay.Core.Enums;

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
            SetKeys();
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

        private void SetKeys()
        {
            ReadKeyTextBox.Text = KeyboardHook.Instance.GetCharFromKey((Keys)Properties.Settings.Default.ReadKey).ToString().ToUpper();
            NextKeyTextBox.Text = KeyboardHook.Instance.GetCharFromKey((Keys)Properties.Settings.Default.NextMapKey).ToString().ToUpper();
            PreviousKeyTextBox.Text = KeyboardHook.Instance.GetCharFromKey((Keys)Properties.Settings.Default.PreviousMapKey).ToString().ToUpper();
        }

        private void InitializeModifiers()
        {
            modifiers.Add(ModifierKeys.None, "-None-");
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
            UpdateModifier((ComboBox)sender, HotKeyType.Read);
        }

        private void NextModifierComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateModifier((ComboBox)sender, HotKeyType.NextMap);
        }

        private void PreviousModifierComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateModifier((ComboBox)sender, HotKeyType.PreviousMap);
        }

        private void ReadKeyTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            UpdateKey((TextBox)sender, HotKeyType.Read, e.Key);
        }

        private void NextKeyTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            UpdateKey((TextBox)sender, HotKeyType.NextMap, e.Key);
        }

        private void PreviousKeyTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            UpdateKey((TextBox)sender, HotKeyType.PreviousMap, e.Key);
        }

        private void UpdateModifier(ComboBox comboBox, HotKeyType hotKeyType)
        {
            var newModifier = modifiers.FirstOrDefault(x => x.Value.Equals(comboBox.SelectedItem.ToString())).Key;
            var hotKeyName = hotKeyType.ToString();
            HotKeysController.UpdateHotKey(hotKeyType, newModifier, (Keys)(uint)Properties.Settings.Default[$"{hotKeyName}Key"]);
            Properties.Settings.Default[$"{hotKeyName}Modifier"] = (uint)newModifier;
            Properties.Settings.Default.Save();
        }

        private void UpdateKey(TextBox textBox, HotKeyType hotKeyType, Key key)
        {
            var newKey = (Keys)KeyInterop.VirtualKeyFromKey(key);
            textBox.Text = KeyboardHook.Instance.GetCharFromKey(newKey).ToString().ToUpper();
            var hotKeyName = hotKeyType.ToString();
            HotKeysController.UpdateHotKey(hotKeyType, (ModifierKeys)(uint)Properties.Settings.Default[$"{hotKeyName}Modifier"], newKey);
            Properties.Settings.Default[$"{hotKeyName}Key"] = (uint)newKey;
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
