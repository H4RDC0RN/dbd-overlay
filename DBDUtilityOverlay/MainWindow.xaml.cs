﻿using DBDUtilityOverlay.MVVM.ViewModel;
using System.Windows;
using System.Windows.Input;
using Application = System.Windows.Application;
using Logger = DBDUtilityOverlay.Core.Utils.Logger;
using Window = System.Windows.Window;
using DBDUtilityOverlay.Core.Utils;
using DBDUtilityOverlay.Core.Enums;

namespace DBDUtilityOverlay
{
    public partial class MainWindow : Window
    {
        private readonly MapOverlayTabViewModel mapOverlayTabVM;
        private readonly SettingsTabViewModel settingsTabVM;
        private readonly AboutTabViewModel aboutTabVM;

        public MainWindow()
        {
            Logger.Log.Info("---Open Application---");
            InitializeComponent();
            HandleExceptions();
            ScreenshotRecognizer.SetScreenBounds();
            InitializeHotKeys();

            mapOverlayTabVM = new MapOverlayTabViewModel();
            settingsTabVM = new SettingsTabViewModel();
            aboutTabVM = new AboutTabViewModel();
            MapOverlayTab.IsChecked = true;
        }

        private void InitializeHotKeys()
        {
            var readModifier = (ModifierKeys)Properties.Settings.Default.ReadModifier;
            var readKey = (Keys)Properties.Settings.Default.ReadKey;
            var nextModifier = (ModifierKeys)Properties.Settings.Default.NextMapModifier;
            var nextKey = (Keys)Properties.Settings.Default.NextMapKey;
            var previousModifier = (ModifierKeys)Properties.Settings.Default.PreviousMapModifier;
            var previousKey = (Keys)Properties.Settings.Default.PreviousMapKey;

            HotKeysController.RegisterHotKey(HotKeyType.Read, readModifier, readKey);
            HotKeysController.RegisterHotKey(HotKeyType.NextMap, nextModifier, nextKey);
            HotKeysController.RegisterHotKey(HotKeyType.PreviousMap, previousModifier, previousKey);
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
            Logger.Log.Info("---Close Application---");
            HotKeysController.UnregisterAllHotKeys();
            MapOverlayController.Instance.Close();
            Close();
            Application.Current.Shutdown();
        }

        private void MinButtonClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void HandleExceptions()
        {
            AppDomain.CurrentDomain.FirstChanceException += (sender, e) =>
            {
                Logger.Log.Fatal(e.Exception.Message);
                Logger.Log.Fatal(e.Exception.StackTrace);
                Logger.Log.Fatal("Recognized text:");
                Logger.Log.Fatal(ScreenshotRecognizer.Text);
                Logger.Log.Info("---Close Application with exception---");
            };
        }
    }
}