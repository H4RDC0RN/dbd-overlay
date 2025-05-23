﻿using System.Windows;
using System.Windows.Input;
using Application = System.Windows.Application;
using Logger = DBDOverlay.Core.Utils.Logger;
using Window = System.Windows.Window;
using DBDOverlay.Core.Hotkeys;
using DBDOverlay.MVVM.ViewModel;
using DBDOverlay.Core.ImageProcessing;
using DBDOverlay.Core.Extensions;
using DBDOverlay.Core.WindowControllers.KillerOverlay;
using DBDOverlay.Core.WindowControllers.MapOverlay;

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
            InitializeComponent();            
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
    }
}