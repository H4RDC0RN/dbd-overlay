﻿using System.Windows;
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

namespace DBDOverlay
{
    public partial class MainWindow : Window
    {
        private readonly MapOverlayTabViewModel mapOverlayTabVM;
        private readonly KillerOverlayTabViewModel killerOverlayTabVM;
        private readonly SettingsTabViewModel settingsTabVM;
        private readonly AboutTabViewModel aboutTabVM;

        public MainWindow()
        {
            HandleExceptions();
            Folders.CreateDefault();
            DownloadManager.Instance.DownloadDefaultLanguage();
            DownloadManager.Instance.CheckForUpdate();
            ReloadSettings();

            Logger.Info("---Open Application---");
            InitializeComponent();
            ScreenshotRecognizer.SetScreenBounds();
            SetKillerOverlaysBounds();

            mapOverlayTabVM = new MapOverlayTabViewModel();
            killerOverlayTabVM = new KillerOverlayTabViewModel();
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
            var rect = ScreenshotRecognizer.GetRect(RectType.Survivors, SystemParameters.PrimaryScreenWidth.Round(), SystemParameters.PrimaryScreenHeight.Round());
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

        private void HandleExceptions()
        {
            AppDomain.CurrentDomain.FirstChanceException += (sender, e) =>
            {
                Logger.Fatal(e.Exception.Message);
                Logger.Fatal(e.Exception.StackTrace);
                Logger.Info("---Close Application with exception---");
            };
        }
    }
}