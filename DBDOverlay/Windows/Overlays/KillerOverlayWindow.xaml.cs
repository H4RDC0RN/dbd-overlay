using DBDOverlay.Core.Extensions;
using DBDOverlay.Core.ImageProcessing;
using DBDOverlay.Core.WindowControllers.KillerOverlay;
using DBDOverlay.Core.Windows;
using DBDOverlay.Properties;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;

namespace DBDOverlay.Windows.Overlays
{
    public partial class KillerOverlayWindow : Window
    {
        public int DefaultStyle { get; set; }
        public Rectangle CurrentRect { get; set; }

        public KillerOverlayWindow()
        {
            InitializeComponent();
            SetBounds();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            DefaultStyle = WindowsServices.Instance.SetWindowExTransparent(this);
            KillerOverlayController.Instance.SetTimers();
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
                case SurvivorNumber.First: return SurvivorHooks_1;
                case SurvivorNumber.Second: return SurvivorHooks_2;
                case SurvivorNumber.Third: return SurvivorHooks_3;
                case SurvivorNumber.Fourth: return SurvivorHooks_4;
                default: return null;
            }
        }

        public Label GetTimerLabel(SurvivorNumber survivor)
        {
            switch (survivor)
            {
                case SurvivorNumber.First: return SurvivorTimer_1;
                case SurvivorNumber.Second: return SurvivorTimer_2;
                case SurvivorNumber.Third: return SurvivorTimer_3;
                case SurvivorNumber.Fourth: return SurvivorTimer_4;
                default: return null;
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

        private void SetBounds()
        {
            if (Settings.Default.KillerOverlayRect != string.Empty)
            {
                CurrentRect = Settings.Default.KillerOverlayRect.ToRect();
                Height = CurrentRect.Height;
                Width = CurrentRect.Width;
                Left = CurrentRect.X;
                Top = CurrentRect.Y;
            }
            else
            {
                SetDefaultBounds();
            }
        }

        private void SetDefaultBounds()
        {
            var rect = ImageReader.Instance.GetRect(RectType.Survivors, SystemParameters.PrimaryScreenWidth.Round(), SystemParameters.PrimaryScreenHeight.Round());
            Left = rect.Left - (rect.Width / 2);
            Top = rect.Top;
            Width = rect.Width * 2;
            Height = rect.Height;
        }

        private void ChangeGridThickness(bool isVisible)
        {
            var i = isVisible ? 1 : 0;
            Survivor1.BorderThickness = new Thickness(i, i, i, i);
            Survivor2.BorderThickness = new Thickness(i, 0, i, i);
            Survivor3.BorderThickness = new Thickness(i, 0, i, i);
            Survivor4.BorderThickness = new Thickness(i, 0, i, i);
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
