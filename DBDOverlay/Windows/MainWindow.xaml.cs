using System.Windows;
using System.Windows.Input;
using Application = System.Windows.Application;
using Logger = DBDOverlay.Core.Utils.Logger;
using Window = System.Windows.Window;
using DBDOverlay.Core.Hotkeys;
using DBDOverlay.Core.MapOverlay;
using DBDOverlay.MVVM.ViewModel;
using System;
using DBDOverlay.Core.Download;
using System.Configuration;
using DBDOverlay.Properties;
using System.IO;
using DBDOverlay.Core.ImageProcessing;
using DBDOverlay.Core.Utils;
using DBDOverlay.Core.KillerOverlay;
using DBDOverlay.Core.Extensions;
using System.Reflection;
using System.Windows.Forms;
using DBDOverlay.Core.Reshade;
using DBDOverlay.Core.MapOverlay.Languages;

namespace DBDOverlay
{
    public partial class MainWindow : Window
    {
        private readonly MapOverlayTabViewModel mapOverlayTabVM;
        private readonly KillerOverlayTabViewModel killerOverlayTabVM;
        private readonly ReshadeTabViewModel reshadeTabVM;
        private readonly SettingsTabViewModel settingsTabVM;
        private readonly AboutTabViewModel aboutTabVM;

        public MainWindow()
        {
            HandleExceptions();
            FileSystem.CreateDefaultFolders(); 
            DownloadManager.Instance.CheckForUpdate();
            DownloadManager.Instance.DownloadDefaultLanguage();
            ReloadSettings();
            AddNumToSendKeys();
            LoadReshadeIni();

            Logger.Info("---Open Application---");
            InitializeComponent();
            ImageReader.Instance.Initialize();
            SetKillerOverlaysBounds();

            mapOverlayTabVM = new MapOverlayTabViewModel();
            killerOverlayTabVM = new KillerOverlayTabViewModel();
            reshadeTabVM = new ReshadeTabViewModel();
            settingsTabVM = new SettingsTabViewModel();
            aboutTabVM = new AboutTabViewModel();
            MapOverlayTab.IsChecked = true;
        }

        private void WindowMouseDown(object sender, MouseButtonEventArgs e)
        {
            MainGrid.Focus();
            DragMove();
        }

        private void MapOverlayTab_Selected(object sender, RoutedEventArgs e)
        {
            ViewContent.Content = mapOverlayTabVM;
        }

        private void KillerOverlayTab_Selected(object sender, RoutedEventArgs e)
        {
            ViewContent.Content = killerOverlayTabVM;
        }

        private void ReshadeTab_Selected(object sender, RoutedEventArgs e)
        {
            ViewContent.Content = reshadeTabVM;
        }

        private void SettingsTab_Selected(object sender, RoutedEventArgs e)
        {
            ViewContent.Content = settingsTabVM;
        }

        private void AboutTab_Selected(object sender, RoutedEventArgs e)
        {
            ViewContent.Content = aboutTabVM;
        }

        private void ExitButtonClick(object sender, RoutedEventArgs e)
        {
            Logger.Info("---Close Application---");
            HotKeysController.Dispose();
            MapOverlayController.Overlay.Close();
            KillerOverlayController.HooksOverlay.Close();
            KillerOverlayController.PostUnhookTimerOverlay.Close();
            Close();
            Application.Current.Shutdown();
        }

        private void MinButtonClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ReloadSettings()
        {
            string configPath = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
            if (!File.Exists(configPath))
            {
                Settings.Default.Upgrade();
                Settings.Default.Reload();
                Settings.Default.Save();
            }
        }

        private void SetKillerOverlaysBounds()
        {
            var rect = ImageReader.Instance.GetRect(RectType.Survivors, SystemParameters.PrimaryScreenWidth.Round(), SystemParameters.PrimaryScreenHeight.Round());
            var halfWidth = rect.Width / 2;

            KillerOverlayController.HooksOverlay.Left = rect.Left + rect.Width - (rect.Width / 5);
            KillerOverlayController.HooksOverlay.Top = rect.Top;
            KillerOverlayController.HooksOverlay.Width = halfWidth;
            KillerOverlayController.HooksOverlay.Height = rect.Height;

            KillerOverlayController.PostUnhookTimerOverlay.Left = rect.Left - halfWidth;
            KillerOverlayController.PostUnhookTimerOverlay.Top = rect.Top;
            KillerOverlayController.PostUnhookTimerOverlay.Width = halfWidth;
            KillerOverlayController.PostUnhookTimerOverlay.Height = rect.Height;
        }

        private void LoadReshadeIni()
        {
            var path = Settings.Default.ReshadeIniPath;
            if (!path.Equals(string.Empty))
            {
                ReshadeManager.Instance.Initialize(path);
                var maps = MapNamesContainer.GetReshadeMapsList();
                for (int mapIndex = 0; mapIndex < maps.Count; mapIndex++)
                {
                    var filterIndex = MappingsHandler.GetFilterIndex(mapIndex);
                    if (filterIndex != -1) ReshadeManager.Instance.AddHotKey(maps[mapIndex], filterIndex);
                }
            }
        }

        private void AddNumToSendKeys()
        {
            var info = typeof(SendKeys).GetField("keywords", BindingFlags.Static | BindingFlags.NonPublic);
            var oldKeys = (Array)info.GetValue(null);
            var elementType = oldKeys.GetType().GetElementType();
            var newKeys = Array.CreateInstance(elementType, oldKeys.Length + 10);
            Array.Copy(oldKeys, newKeys, oldKeys.Length);
            for (int i = 0; i < 10; i++)
            {
                var newItem = Activator.CreateInstance(elementType, "NUMPAD" + i, (int)Keys.NumPad0 + i);
                newKeys.SetValue(newItem, oldKeys.Length + i);
            }
            info.SetValue(null, newKeys);
        }

        private void HandleExceptions()
        {
            AppDomain.CurrentDomain.FirstChanceException += (sender, e) =>
            {
                Logger.Fatal(e.Exception.GetType().Name);
                Logger.Fatal(e.Exception.Message);
                Logger.Fatal(e.Exception.StackTrace);
                Logger.Info("---Close Application with exception---");
            };
        }
    }
}