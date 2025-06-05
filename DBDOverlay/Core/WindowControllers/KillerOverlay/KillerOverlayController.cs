using DBDOverlay.Core.BackgroundProcesses;
using DBDOverlay.Core.Extensions;
using DBDOverlay.Core.Utils;
using DBDOverlay.Windows.Overlays;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Application = System.Windows.Application;

namespace DBDOverlay.Core.WindowControllers.KillerOverlay
{
    public class KillerOverlayController
    {
        public bool CanBeMoved { get; set; } = false;
        public List<Survivor> Survivors = new List<Survivor>();
        private static KillerOverlayController instance;
        private static KillerOverlayWindow killerOverlay;
        private readonly double threshold = 0.9;
        private readonly int survivorsCount = 4;
        private readonly int unhookAnimationDelay = 1500;
        private readonly int otrTimer = 80000;

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
                    Overlay.GetHooksLabel((SurvivorNumber)(index + 1)).Content = "0";
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

        public void ResetSurvivors()
        {
            Survivors.Clear();
            SetSurvivors();
            for (int i = 0; i < survivorsCount; i++)
            {
                Overlay.GetHooksLabel((SurvivorNumber)(i + 1)).Content = "0";
            }
        }

        private void SetSurvivors()
        {
            for (int i = 1; i <= survivorsCount; i++)
            {
                Survivors.Add(new Survivor());
            }
        }

        private void RunTimer(int index)
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (s, e) =>
            {
                Thread.Sleep(unhookAnimationDelay);
                var watch = Stopwatch.StartNew();
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
                });
            };
            worker.RunWorkerAsync();
        }
    }
}
