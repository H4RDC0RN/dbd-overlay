using DBDOverlay.Core.Extensions;
using DBDOverlay.Core.Reshade;
using DBDOverlay.Core.WindowControllers.MapOverlay.Languages;
using DBDOverlay.Properties;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DBDOverlay.UI.Windows
{
    public partial class AssignFiltersWindow : Window
    {
        public AssignFiltersWindow()
        {
            InitializeComponent();
            InitializeElements();
            SetComboboxValues();
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

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            if (comboBox.SelectedItem != null && comboBox.IsVisible) ReshadeManager.Instance.AddFilterMapPair(comboBox.Name, comboBox.SelectedItem.ToString());
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

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
