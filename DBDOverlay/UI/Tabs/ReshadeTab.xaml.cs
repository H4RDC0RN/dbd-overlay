using DBDOverlay.Core.Extensions;
using DBDOverlay.Core.Reshade;
using DBDOverlay.Core.Utils;
using DBDOverlay.Core.WindowControllers.MapOverlay.Languages;
using DBDOverlay.Properties;
using DBDOverlay.UI.Styles;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DBDOverlay.UI.Tabs
{
    public partial class ReshadeTabView : UserControl
    {
        private readonly string reshadeFolderPlaceholder = "Select Reshade folder with filters (.ini files)";

        public ReshadeTabView()
        {
            InitializeComponent();
            InitializeElements();            
            UpdateReshadePath(Settings.Default.ReshadeFiltersPath);
            UpdateFilters();
            UpdateGenerateFilterUI();
        }

        private void InitializeElements()
        {
            var list = MapNamesContainer.GetReshadeMapsList();
            for (int i = 0; i < list.Count; i++)
            {
                MapFiltersGrid.RowDefinitions.Add(new RowDefinition());

                var textBlock = new TextBlock
                {
                    Text = list[i].ToTitle(),
                    Style = (Style)FindResource("TextBlockStyle")
                };
                MapFiltersGrid.Children.Add(textBlock);
                Grid.SetRow(textBlock, MapFiltersGrid.RowDefinitions.Count - 1);
                Grid.SetColumn(textBlock, 0);

                var comboBox = new ComboBox()
                {
                    Name = list[i],
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Width = 200,
                    Margin = new Thickness(5),
                    Style = (Style)FindResource("ComboBoxFlatStyle")
                };
                comboBox.SelectionChanged += new SelectionChangedEventHandler(FilterComboBox_SelectionChanged);
                MapFiltersGrid.Children.Add(comboBox);
                Grid.SetRow(comboBox, MapFiltersGrid.RowDefinitions.Count - 1);
                Grid.SetColumn(comboBox, 1);
            }
        }

        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = "Select ReShade folder"
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                HandleNewReshadeFolder(dialog.FileName);
            }
        }

        private void ClearFolder_Click(object sender, RoutedEventArgs e)
        {
            HandleNewReshadeFolder(string.Empty);
        }

        private void RefreshFilters_Click(object sender, RoutedEventArgs e)
        {
            ReshadeManager.Instance.ReloadFilters();
            UpdateFilters();
        }

        private void GenerateFilter_Click(object sender, RoutedEventArgs e)
        {
            if (!IsValidFileName(MainFilterNameTextBox.Text)) return;
            FileSystem.CreateFile(GetMainFilterPath(MainFilterNameTextBox.Text));
            EnableGenerateFilterUI(false);
            Settings.Default.MainFilterName = MainFilterNameTextBox.Text;
            Settings.Default.Save();
        }

        private void DeleteFilter_Click(object sender, RoutedEventArgs e)
        {
            FileSystem.DeleteFile(GetMainFilterPath(Settings.Default.MainFilterName));
            Settings.Default.MainFilterName = string.Empty;
            Settings.Default.Save();
            EnableGenerateFilterUI(!Settings.Default.ReshadeFiltersPath.Equals(string.Empty));
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            foreach (var comboBox in MapFiltersGrid.Children.OfType<ComboBox>().ToList())
            {
                comboBox.SelectedItem = null;
            }
            ReshadeManager.Instance.ClearMapFilterPairs();
            Settings.Default.ReshadeMappings = string.Empty;
            Settings.Default.Save();
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            if (comboBox.SelectedItem != null && comboBox.IsVisible) ReshadeManager.Instance.AddFilterMapPair(comboBox.Name, comboBox.SelectedItem.ToString());
        }

        private void HandleNewReshadeFolder(string folderPath)
        {
            if (folderPath.Equals(Settings.Default.ReshadeFiltersPath)) return;
            UpdateReshadePath(folderPath);
            Settings.Default.ReshadeFiltersPath = folderPath;
            Settings.Default.ReshadeMappings = string.Empty;
            Settings.Default.Save();
            ReshadeManager.Instance.Initialize(folderPath);
            UpdateFilters();
            EnableGenerateFilterUI(!folderPath.Equals(string.Empty));
            if (folderPath.Equals(string.Empty)) FileSystem.DeleteFile(GetMainFilterPath(Settings.Default.MainFilterName));
            Settings.Default.MainFilterName = string.Empty;
        }

        private void SetComboboxValues()
        {
            var list = MapNamesContainer.GetReshadeMapsList();
            var comboBoxes = MapFiltersGrid.Children.OfType<ComboBox>().ToList();
            for (int mapIndex = 0; mapIndex < list.Count; mapIndex++)
            {
                var comboBox = comboBoxes.Find(x => x.Name.Equals(list[mapIndex]));
                comboBox.ItemsSource = ReshadeManager.Instance.Filters;
                var filterIndex = MappingsHandler.GetFilterIndex(mapIndex);
                if (filterIndex != -1) comboBox.SelectedIndex = filterIndex;
            }
        }

        private void UpdateReshadePath(string folderPath)
        {
            if (folderPath.Equals(string.Empty))
            {
                ReShadePathTextBox.Text = reshadeFolderPlaceholder;
                ReShadePathTextBox.Foreground = Palette.LightGrayBrush;
                ReShadePathTextBox.BorderThickness = new Thickness(0, 0, 0, 1);
            }
            else
            {
                ReShadePathTextBox.Text = folderPath;
                ReShadePathTextBox.Foreground = Palette.WhiteBrush;
                ReShadePathTextBox.BorderThickness = new Thickness(0, 0, 0, 0);
            }
        }

        private void UpdateFilters()
        {
            SetComboboxValues();
            var filters = ReshadeManager.Instance.Filters;
            if (filters != null && filters.Count > 0)
            {
                FiltersStatusLabel.Content = $"{filters.Count} filter(s) found ✓";
                FiltersStatusLabel.Foreground = Palette.WhiteBrush;
            }
            else
            {
                FiltersStatusLabel.Content = $"No filters found ✕";
                FiltersStatusLabel.Foreground = Palette.RedLightBrush;
            }
        }

        private void UpdateGenerateFilterUI()
        {
            MainFilterNameTextBox.Text = Settings.Default.MainFilterName;
            var isEnabled = !Settings.Default.ReshadeFiltersPath.Equals(string.Empty) && Settings.Default.MainFilterName.Equals(string.Empty);
            EnableGenerateFilterUI(isEnabled);
        }

        private bool IsValidFileName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            if (name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                return false;

            if (name.EndsWith(" ") || name.EndsWith("."))
                return false;

            var baseName = Path.GetFileNameWithoutExtension(name)
                               .TrimEnd('.', ' ')
                               .ToUpperInvariant();

            string[] reserved =
            {
                "CON","PRN","AUX","NUL",
                "COM1","COM2","COM3","COM4","COM5","COM6","COM7","COM8","COM9",
                "LPT1","LPT2","LPT3","LPT4","LPT5","LPT6","LPT7","LPT8","LPT9"
            };

            if (reserved.Contains(baseName))
                return false;

            return true;
        }

        private string GetMainFilterPath(string name)
        {
            return $@"{Settings.Default.ReshadeFiltersPath}\{name}.ini";
        }

        private void EnableGenerateFilterUI(bool isEnable)
        {
            MainFilterNameTextBox.IsEnabled = isEnable;
            GenerateFilterButton.IsEnabled = isEnable;
        }
    }
}
