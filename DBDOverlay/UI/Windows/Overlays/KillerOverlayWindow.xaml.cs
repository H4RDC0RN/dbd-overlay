using DBDOverlay.Core.Extensions;
using DBDOverlay.Core.ImageProcessing;
using DBDOverlay.Core.WindowControllers.KillerOverlay;
using DBDOverlay.Core.Windows;
using DBDOverlay.Properties;
using DBDOverlay.UI.Styles;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;

namespace DBDOverlay.UI.Windows.Overlays
{
    public partial class KillerOverlayWindow : Window
    {
        public int DefaultStyle { get; set; }
        public Rectangle CurrentRect { get; set; }

        public KillerOverlayWindow(bool isTopMost = true)
        {
            InitializeComponent();
            SetBounds();
            Topmost = isTopMost;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            DefaultStyle = WindowsServices.Instance.SetWindowExTransparent(this);
            KillerOverlayController.Instance.SetTimers();
            App.Current.MainWindow.Owner = this;
        }

        private void OverlayMouseDown(object sender, MouseButtonEventArgs e)
        {
            Cursor = Cursors.SizeAll;
            DragMove();
        }

        private void OverlayMouseUp(object sender, MouseButtonEventArgs e)
        {
            Cursor = Cursors.Hand;
            SaveBounds();
        }

        public Label GetHooksLabel(SurvivorNumber survivor)
        {
            switch (survivor)
            {
                case SurvivorNumber.N1: return SurvivorHooks_1;
                case SurvivorNumber.N2: return SurvivorHooks_2;
                case SurvivorNumber.N3: return SurvivorHooks_3;
                case SurvivorNumber.N4: return SurvivorHooks_4;
                case SurvivorNumber.N5: return SurvivorHooks_5;
                case SurvivorNumber.N6: return SurvivorHooks_6;
                case SurvivorNumber.N7: return SurvivorHooks_7;
                case SurvivorNumber.N8: return SurvivorHooks_8;
                default: return null;
            }
        }

        public Label GetTimerLabel(SurvivorNumber survivor)
        {
            switch (survivor)
            {
                case SurvivorNumber.N1: return SurvivorTimer_1;
                case SurvivorNumber.N2: return SurvivorTimer_2;
                case SurvivorNumber.N3: return SurvivorTimer_3;
                case SurvivorNumber.N4: return SurvivorTimer_4;
                case SurvivorNumber.N5: return SurvivorTimer_5;
                case SurvivorNumber.N6: return SurvivorTimer_6;
                case SurvivorNumber.N7: return SurvivorTimer_7;
                case SurvivorNumber.N8: return SurvivorTimer_8;
                default: return null;
            }
        }

        public void SetOverlayVisible(bool isVisible)
        {
            if (Settings.Default.IsPostUnhookTimerMode)
            {
                if (isVisible) ShowTimer();
                else HideTimer();
            }
            if (Settings.Default.IsHookMode)
            {
                if (isVisible) ShowHooks();
                else HideHooks();
            }
        }

        public void ShowHooks()
        {
            SetHooksVisibility(Visibility.Visible);
            Show();
        }

        public void HideHooks()
        {
            SetHooksVisibility(Visibility.Hidden);
            if (!PostUnhookTimerGrid.IsVisible) Hide();
        }

        public void ShowTimer()
        {
            SetTimerVisibility(Visibility.Visible);
            Show();
        }

        public void HideTimer()
        {
            SetTimerVisibility(Visibility.Hidden);
            if (!HooksGrid.IsVisible) Hide();
        }

        public void ShowMoreSurvivors(bool resetPosition = true)
        {
            var autoValue = (GridLength)new GridLengthConverter().ConvertFrom("*");
            for (int i = 4; i < 8; i++)
            {
                PostUnhookTimerGrid.RowDefinitions[i].Height = autoValue;
                HooksGrid.RowDefinitions[i].Height = autoValue;
                SurvivorsAreaGrid.RowDefinitions[i].Height = autoValue;
            }
            SetBounds(resetPosition);
            KillerOverlayController.Instance.ResetSurvivors();
        }

        public void HideMoreSurvivors(bool resetPosition = true)
        {
            var zeroValue = (GridLength)new GridLengthConverter().ConvertFrom("0");
            for (int i = 4; i < 8; i++)
            {
                PostUnhookTimerGrid.RowDefinitions[i].Height = zeroValue;
                HooksGrid.RowDefinitions[i].Height = zeroValue;
                SurvivorsAreaGrid.RowDefinitions[i].Height = zeroValue;
            }
            SetBounds(resetPosition);
            KillerOverlayController.Instance.ResetSurvivors();
        }

        public void ShowGrid()
        {
            ChangeGridThickness(true);
            Background = new SolidColorBrush(Color.FromArgb(0x1A, 64, 69, 70));
        }

        public void HideGrid()
        {
            ChangeGridThickness(false);
            Background = Brushes.Transparent;
        }

        public void SaveBounds()
        {
            CurrentRect = new Rectangle(Left.Round(), Top.Round(), Width.Round(), Height.Round());
            Settings.Default.KillerOverlayRect = $"{CurrentRect.X},{CurrentRect.Y},{CurrentRect.Width},{CurrentRect.Height}";
            Settings.Default.Save();
        }

        public void ResetBounds()
        {
            SetDefaultBounds();
            CurrentRect = new Rectangle(Left.Round(), Top.Round(), Width.Round(), Height.Round());
            Settings.Default.KillerOverlayRect = $"{CurrentRect.X},{CurrentRect.Y},{CurrentRect.Width},{CurrentRect.Height}";
            Settings.Default.Save();
        }

        public void SetKillerWindowHiddenPosition()
        {
            Left = GetLeftSidePanel(false);
            Top = GetTopSidePanel();
        }

        public void SetKillerWindowShowPosition()
        {
            Left = GetLeftSidePanel();
            Top = GetTopSidePanel();
        }

        public void ShowSidePanel()
        {
            MainGridBorder.CornerRadius = new CornerRadius(0, 10, 10, 0);
            MainGridBorder.Background = Palette.DarkGrayBrush;
            SetHooksVisibility(Visibility.Visible);
            SetTimerVisibility(Visibility.Visible);
            Show();
            MoveWindow(GetLeftSidePanel(), GetTopSidePanel());
        }

        public void HideSidePanel()
        {
            MoveWindow(GetLeftSidePanel(false), GetTopSidePanel());
            SetHooksVisibility(Visibility.Hidden);
            SetTimerVisibility(Visibility.Hidden);
        }

        private double GetLeftSidePanel(bool show = true) => App.Current.MainWindow.Left + App.Current.MainWindow.ActualWidth - (show ? 0 : Width + 5);
        private double GetTopSidePanel() => App.Current.MainWindow.Top + 30;

        private void MoveWindow(double newLeft, double newTop)
        {

            var duration = TimeSpan.FromMilliseconds(300);

            var leftAnim = new DoubleAnimation
            {
                To = newLeft,
                Duration = duration,
                FillBehavior = FillBehavior.Stop,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            var topAnim = new DoubleAnimation
            {
                To = newTop,
                Duration = duration,
                FillBehavior = FillBehavior.Stop,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            BeginAnimation(LeftProperty, leftAnim);
            BeginAnimation(TopProperty, topAnim);

            Left = newLeft;
            Top = newTop;
        }

        private void SetBounds(bool resetPosition = true)
        {
            var overlayRect = Settings.Default.Is2v8Mode ? Settings.Default.Killer2v8OverlayRect : Settings.Default.KillerOverlayRect;
            if (overlayRect != string.Empty)
            {
                CurrentRect = overlayRect.ToRect();
                Height = CurrentRect.Height;
                Width = CurrentRect.Width;
                Left = CurrentRect.X;
                Top = CurrentRect.Y;
            }
            else
            {
                SetDefaultBounds(resetPosition);
            }
        }

        private void SetDefaultBounds(bool resetPosition = true)
        {
            var rectType = Settings.Default.Is2v8Mode ? RectType.Survivors2v8 : RectType.Survivors;
            var rect = ImageReader.Instance.GetRect(rectType, SystemParameters.PrimaryScreenWidth.Round(), SystemParameters.PrimaryScreenHeight.Round());
            if (resetPosition)
            {
                Left = rect.Left - (rect.Width / 2);
                Top = rect.Top;
            }
            Width = rect.Width * 2;
            Height = rect.Height;
            CurrentRect = new Rectangle((int)Left, (int)Top, (int)Width, (int)Height);
        }

        private void ChangeGridThickness(bool isVisible)
        {
            var i = isVisible ? 1 : 0;
            Survivor1.BorderThickness = new Thickness(i, i, i, i);
            Survivor2.BorderThickness = new Thickness(i, 0, i, i);
            Survivor3.BorderThickness = new Thickness(i, 0, i, i);
            Survivor4.BorderThickness = new Thickness(i, 0, i, i);

            if (Settings.Default.Is2v8Mode)
            {
                Survivor5.BorderThickness = new Thickness(i, 0, i, i);
                Survivor6.BorderThickness = new Thickness(i, 0, i, i);
                Survivor7.BorderThickness = new Thickness(i, 0, i, i);
                Survivor8.BorderThickness = new Thickness(i, 0, i, i);
            }

            MainGridBorder.BorderThickness = new Thickness(i, i, i, i);
            MainGrid.Margin = new Thickness(-i, -i, -i, -i);
        }

        private void SetHooksVisibility(Visibility visibility)
        {
            HooksGrid.Visibility = visibility;
        }

        private void SetTimerVisibility(Visibility visibility)
        {
            PostUnhookTimerGrid.Visibility = visibility;
        }
    }
}
