﻿using System.Windows;
using System.Windows.Controls;
using UserControl = System.Windows.Controls.UserControl;
using System.Windows.Input;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using ComboBox = System.Windows.Controls.ComboBox;
using TextBox = System.Windows.Controls.TextBox;
using DBDOverlay.Core.Download;
using DBDOverlay.Core.Hotkeys;
using DBDOverlay.Core.Languages;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DBDOverlay.Core.Utils;

namespace DBDOverlay.MVVM.View
{
    public partial class SettingsTabView : UserControl
    {
        private readonly Dictionary<ModifierKeys, string> modifiers = new Dictionary<ModifierKeys, string>();

        public SettingsTabView()
        {
            InitializeComponent();
            SetLanguages();
            SetDownloadLanguages();
            SetModifiers();
            SetKeys();
            DownloadManager.Instance.Downloading += HandleDownloading;
        }

        private void InitializeModifiers()
        {
            modifiers.Add(ModifierKeys.None, "-None-");
            modifiers.Add(ModifierKeys.Alt, "Alt");
            modifiers.Add(ModifierKeys.Control, "Ctrl");
            modifiers.Add(ModifierKeys.Shift, "Shift");
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

        private void SetLanguages()
        {
            var downloadedLanguages = DownloadManager.Instance.GetDownloadedLanguages();
            if (downloadedLanguages.Contains(LanguagesManager.Spa)) downloadedLanguages.Add(LanguagesManager.Mex);
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

        private void Download_Click(object sender, RoutedEventArgs e)
        {
            DownloadManager.Instance.DownloadLanguage(LanguagesManager.GetValue(DownloadLanguageComboBox.SelectedItem.ToString()));
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
            var settingName = $"{hotKeyType}Modifier";
            var oldModifier = (ModifierKeys)(uint)Properties.Settings.Default[settingName];
            var newModifier = modifiers.FirstOrDefault(x => x.Value.Equals(comboBox.SelectedItem.ToString())).Key;
            
            Properties.Settings.Default[settingName] = (uint)newModifier;
            Properties.Settings.Default.Save();

            Logger.Log.Info($"Modifier for '{hotKeyType}' hotkey is updated");
            Logger.Log.Info($"Old value '{oldModifier}'");
            Logger.Log.Info($"New value '{newModifier}'");
        }

        private void UpdateKey(TextBox textBox, HotKeyType hotKeyType, Key key)
        {
            var settingName = $"{hotKeyType}Key";
            var oldKey = (Keys)(uint)Properties.Settings.Default[settingName];
            var newKey = (Keys)KeyInterop.VirtualKeyFromKey(key);

            textBox.Text = KeyboardHook.Instance.GetCharFromKey(newKey).ToString().ToUpper();
            Properties.Settings.Default[settingName] = (uint)newKey;
            Properties.Settings.Default.Save();

            Logger.Log.Info($"Key for '{hotKeyType}' hotkey is updated");
            Logger.Log.Info($"Old value '{oldKey}'");
            Logger.Log.Info($"New value '{newKey}'");
        }

        private void HandleDownloading(object sender, DownloadEventArgs e)
        {
            if (e.IsDownloading)
            {
                DownloadButton.IsEnabled = false;
            }
            else
            {
                DownloadButton.IsEnabled = true;
                SetLanguages();
            }
        }
    }
}
