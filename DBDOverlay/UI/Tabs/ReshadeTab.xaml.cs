using DBDOverlay.Core.Reshade;
using DBDOverlay.Core.Utils;
using DBDOverlay.Properties;
using DBDOverlay.UI.Styles;
using DBDOverlay.UI.Windows;
using DBDOverlay.UI.Windows.Overlays;
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
            UpdateReshadePath(Settings.Default.ReshadeFiltersPath);
            UpdateFilters();
            UpdateGenerateFilterUI();
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
            UpdateGenerateFilterUI();
        }

        private void GenerateFilter_Click(object sender, RoutedEventArgs e)
        {
            if (!FileSystem.IsValidFileName(MainFilterNameTextBox.Text)) return;
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

        private void AssignFilters_Click(object sender, RoutedEventArgs e)
        {
            var overlay = new ModalOverlayWindow
            {
                Owner = App.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.Manual,
                Left = App.Current.MainWindow.Left,
                Top = App.Current.MainWindow.Top,
                Width = App.Current.MainWindow.ActualWidth,
                Height = App.Current.MainWindow.ActualHeight
            };

            overlay.Show();

            var assignFiltersWindow = new AssignFiltersWindow
            {
                Owner = App.Current.MainWindow
            };
            assignFiltersWindow.ShowDialog();
            overlay.Close();
            App.Current.MainWindow.Activate();
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
            var filters = ReshadeManager.Instance.Filters;
            if (filters != null && filters.Count > 0)
            {
                FiltersStatusLabel.Content = $"{filters.Count} filter(s) found ✓";
                FiltersStatusLabel.Foreground = Palette.WhiteBrush;
                AssignFiltersButton.IsEnabled = true;
            }
            else
            {
                FiltersStatusLabel.Content = $"No filters found ✕";
                FiltersStatusLabel.Foreground = Palette.RedLightBrush;
                AssignFiltersButton.IsEnabled = false;
            }
        }

        private void UpdateGenerateFilterUI()
        {
            UpdateMainFilter();
            MainFilterNameTextBox.Text = Settings.Default.MainFilterName;
            var isEnabled = !Settings.Default.ReshadeFiltersPath.Equals(string.Empty) && Settings.Default.MainFilterName.Equals(string.Empty);
            EnableGenerateFilterUI(isEnabled);
        }

        private void UpdateMainFilter()
        {
            if (!ReshadeManager.Instance.FilterExists(Settings.Default.MainFilterName))
            {
                Settings.Default.MainFilterName = string.Empty;
                Settings.Default.Save();
            }
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
