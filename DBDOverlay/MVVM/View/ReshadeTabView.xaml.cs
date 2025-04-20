using DBDOverlay.Core.Extensions;
using DBDOverlay.Core.MapOverlay.Languages;
using DBDOverlay.Core.Windows;
using Microsoft.Win32;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DBDOverlay.MVVM.View
{
    public partial class ReshadeTabView : UserControl
    {
        public ReshadeTabView()
        {
            InitializeComponent();
            InitializeElements();
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
                    Width = 170,
                    Margin = new Thickness(5),
                    Style = (Style)FindResource("ComboBoxFlatStyle")
                };
                MapFiltersGrid.Children.Add(comboBox);
                Grid.SetRow(comboBox, MapFiltersGrid.RowDefinitions.Count - 1);
                Grid.SetColumn(comboBox, 1);
            }
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Ini files (*.ini)|*.ini|All files (*.*)|*.*"
            };
            if ((bool)openFileDialog.ShowDialog())
            {
                ReadReshadeIni(openFileDialog.FileName);
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ReadReshadeIni(string iniPath)
        {
            var ini = new IniFile(iniPath);
            var keys = ini.Read("PresetShortcutKeys", "GENERAL");
            var filters = ini.Read("PresetShortcutPaths", "GENERAL").Split(',').Select(x => x.RegexMatch(@"(?<=\\)(?:.(?!\\))+(?=\.ini)")).ToList();

            var list = MapNamesContainer.GetReshadeMapsList();
            var comboBoxes = MapFiltersGrid.Children.OfType<ComboBox>().ToList();
            foreach (var comboboxName in list)
            {
                var comboBox = comboBoxes.Find(x => x.Name.Equals(comboboxName));
                comboBox.ItemsSource = filters;
            }
        }
    }
}
