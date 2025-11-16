using DBDOverlay.Core.BackgroundProcesses;
using DBDOverlay.Core.Extensions;
using DBDOverlay.Core.Utils;
using DBDOverlay.UI.Windows.Overlays;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using Application = System.Windows.Application;

namespace DBDOverlay.Core.WindowControllers.KillerOverlay
{
    public class KillerOverlayController
    {
        public bool CanBeMoved { get; set; } = false;
        public List<Survivor> Survivors = new List<Survivor>();
        private static KillerOverlayController instance;
        private static KillerOverlayWindow killerOverlay;

        private int survivorsCount = 4;
        private int otrTimer = 80000;

        private readonly double threshold = 0.9;
        private readonly int unhookAnimationDelay = 1500;

        private readonly string defaultTimerValue;
        private readonly char delimiter;

        public static KillerOverlayController Instance
        {
            get
            {
                if (instance == null)
                    instance = new KillerOverlayController();
                return instance;
            }
        }

        public static KillerOverlayWindow Overlay
        {
            get
            {
                if (killerOverlay == null)
                    killerOverlay = new KillerOverlayWindow();
                return killerOverlay;
            }
        }

        public KillerOverlayController()
        {
            delimiter = 0.1.ToString().ReplaceRegex(@"\d", string.Empty).FirstOrDefault();
            defaultTimerValue = $"0{delimiter}0";
            SetSurvivors();
        }

        public void CheckIfHooked(int index, double similarity)
        {
            if (!Survivors[index].State.Equals(SurvivorState.Hooked) && similarity > threshold)
            {
                Logger.Info($"--- Survivor {index} is hooked. 'Hooked' image similarity = {similarity * 100} %");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var textBlock = Overlay.GetHooksLabel((SurvivorNumber)(index + 1));
                    textBlock.Content = textBlock.Content.ToString().Increment();
                    Survivors[index].State = SurvivorState.Hooked;
                    Survivors[index].Hooks++;
                    if (KillerMode.Instance.Is2v8Mode)
                    {
                        ((Border)textBlock.Template.FindName("LabelBorder", textBlock)).Background = Survivors[index].Hooks == 2
                        ? (SolidColorBrush)Application.Current.FindResource("RedLightBrush")
                        : (SolidColorBrush)Application.Current.FindResource("DarkestGrayBrush");
                    }
                });
            }
        }

        public void CheckIfUnhooked(int index, double similarity)
        {
            if (Survivors[index].State.Equals(SurvivorState.Hooked) && similarity < threshold)
            {
                Logger.Info($"--- Survivor {index} is unhooked. 'Hooked' image similarity = {similarity * 100} %");
                Survivors[index].State = SurvivorState.Unhooked;
                RunTimer(index);
            }
        }

        public void CheckIfRefreshed(int index, Dictionary<string, double> refreshStates)
        {
            var max = refreshStates.Values.Max();
            var pair = refreshStates.FirstOrDefault(x => x.Value.Equals(max));

            if ((Survivors[index].State.Equals(SurvivorState.Hooked) || Survivors[index].State.Equals(SurvivorState.Unhooked)) && pair.Value > threshold)
            {
                Logger.Info($"--- Survivor {index} is {pair.Key.ToLower()}. '{pair.Key}' image similarity = {pair.Value * 100} %");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ResetSurvivor(index, KillerMode.Instance.Is2v8Mode);
                    Survivors[index] = new Survivor();
                    Overlay.GetTimerLabel((SurvivorNumber)(index + 1)).Content = defaultTimerValue;
                });
            }
        }

        public void SetTimers()
        {
            for (int i = 0; i < survivorsCount; i++)
            {
                Overlay.GetTimerLabel((SurvivorNumber)(i + 1)).Content = defaultTimerValue;
            }
        }

        public void ResetSurvivors(bool is2v8Mode = false)
        {
            otrTimer = is2v8Mode ? 9900 : 80000;
            survivorsCount = is2v8Mode ? 8 : 4;
            Survivors.Clear();
            SetSurvivors();
            for (int i = 0; i < survivorsCount; i++)
            {
                ResetSurvivor(i, is2v8Mode);
            }
            SetTimers();
            ResizeLabels(is2v8Mode);
        }

        private void ResetSurvivor(int index, bool is2v8Mode = false)
        {
            var textBlock = Overlay.GetHooksLabel((SurvivorNumber)(index + 1));
            textBlock.Content = "0";
            if (is2v8Mode)
            {
                ((Border)textBlock.Template.FindName("LabelBorder", textBlock)).Background = (SolidColorBrush)Application.Current.FindResource("DarkestGrayBrush");
            }
        }

        private void SetSurvivors()
        {
            for (int i = 1; i <= survivorsCount; i++)
            {
                Survivors.Add(new Survivor());
            }
        }

        private void ResizeLabels(bool is2v8Mode = false)
        {
            for (int i = 0; i < survivorsCount; i++)
            {
                Overlay.GetHooksLabel((SurvivorNumber)(i + 1)).FontSize = is2v8Mode ? 16 : 20;
                Overlay.GetTimerLabel((SurvivorNumber)(i + 1)).FontSize = is2v8Mode ? 12 : 16;
            }
        }

        private void RunTimer(int index)
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (s, e) =>
            {
                Thread.Sleep(unhookAnimationDelay);
                var watch = Stopwatch.StartNew();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (KillerMode.Instance.Is2v8Mode)
                    {
                        var textBlock = Overlay.GetTimerLabel((SurvivorNumber)(index + 1));
                        ((Border)textBlock.Template.FindName("LabelBorder", textBlock)).Background = (SolidColorBrush)Application.Current.FindResource("DarkYellowBrush");
                        textBlock.Foreground = (SolidColorBrush)Application.Current.FindResource("DarkestGrayBrush");
                    }
                });

                while (KillerMode.Instance.IsPostUnhookTimerMode && Survivors[index].State.Equals(SurvivorState.Unhooked) && watch.ElapsedMilliseconds <= otrTimer)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var time = (watch.ElapsedMilliseconds / 1000.0).Round(1).ToString();
                        Overlay.GetTimerLabel((SurvivorNumber)(index + 1)).Content = time.IsInt() ? $"{time}{delimiter}0" : time;
                    });
                }
                watch.Stop();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Overlay.GetTimerLabel((SurvivorNumber)(index + 1)).Content = defaultTimerValue;
                    if (KillerMode.Instance.Is2v8Mode)
                    {
                        var textBlock = Overlay.GetTimerLabel((SurvivorNumber)(index + 1));
                        ((Border)textBlock.Template.FindName("LabelBorder", textBlock)).Background = (SolidColorBrush)Application.Current.FindResource("DarkestGrayBrush");
                        textBlock.Foreground = new SolidColorBrush(Colors.White);
                    }
                });
            };
            worker.RunWorkerAsync();
        }
    }
}
