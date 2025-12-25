using DBDOverlay.Core.Extensions;
using DBDOverlay.Core.Utils;
using DBDOverlay.Properties;
using DBDOverlay.UI.Styles;
using DBDOverlay.UI.Windows.Overlays;
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
        private static KillerOverlayWindow killerOverlayWindow;

        private int survivorsCount;
        private int maxTimer;
        private int unhookEndurance;

        private readonly int survivorsCountDefault = 4;
        private readonly int survivorsCount2v8 = 8;
        private readonly int unhookAnimationDelay = 1500;
        private readonly int maxTimerDefault = 60000;
        private readonly int unhookEnduranceDefault = 15000;
        private readonly int unhookEndurance2v8 = 10000;
        private readonly double threshold = 0.9;

        private readonly string defaultTimerValue;
        private readonly char delimiter;

        public static KillerOverlayController Instance
        {
            get
            {
                if (instance == null) instance = new KillerOverlayController();
                return instance;
            }
        }

        public static KillerOverlayWindow Overlay
        {
            get
            {
                if (killerOverlay == null) killerOverlay = new KillerOverlayWindow();
                return killerOverlay;
            }
        }

        public static KillerOverlayWindow Window
        {
            get
            {
                if (killerOverlayWindow == null) killerOverlayWindow = new KillerOverlayWindow();
                return killerOverlayWindow;
            }
        }

        public KillerOverlayController()
        {
            delimiter = 0.1.ToString().ReplaceRegex(@"\d", string.Empty).FirstOrDefault();
            defaultTimerValue = $"0{delimiter}0";
            survivorsCount = survivorsCountDefault;
            maxTimer = maxTimerDefault;
            unhookEndurance = unhookEnduranceDefault;
            SetSurvivors();
        }

        public void HookedCheck(int index, double similarity)
        {
            if (!Survivors[index].State.Equals(SurvivorState.Hooked) && similarity > threshold)
            {
                Logger.Info($"--- Survivor {index} is hooked. 'Hooked' image similarity = {similarity * 100} %");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Survivors[index].State = SurvivorState.Hooked;
                    Survivors[index].Hooks++;
                    HandleAction(KillerOverlayAction.IncrementHooks, index);
                });
            }
        }

        public void UnhookedCheck(int index, double similarity)
        {
            if (Survivors[index].State.Equals(SurvivorState.Hooked) && similarity < threshold)
            {
                Logger.Info($"--- Survivor {index} is unhooked. 'Hooked' image similarity = {similarity * 100} %");
                Survivors[index].State = SurvivorState.Unhooked;
                RunTimer(index);
            }
        }

        public void RefreshedCheck(int index, Dictionary<string, double> refreshStates)
        {
            var max = refreshStates.Values.Max();
            var pair = refreshStates.FirstOrDefault(x => x.Value.Equals(max));

            if ((Survivors[index].State.Equals(SurvivorState.Hooked) || Survivors[index].State.Equals(SurvivorState.Unhooked)) && pair.Value > threshold)
            {
                Logger.Info($"--- Survivor {index} is {pair.Key.ToLower()}. '{pair.Key}' image similarity = {pair.Value * 100} %");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    HandleAction(KillerOverlayAction.ResetHooks, index);
                    Survivors[index] = new Survivor();
                    HandleAction(KillerOverlayAction.SetTimerValue, index, defaultTimerValue);
                    HandleAction(KillerOverlayAction.SetDefaultTimer, index);
                });
            }
        }

        public void SetTimers()
        {
            for (int i = 0; i < survivorsCount; i++)
            {
                HandleAction(KillerOverlayAction.SetDefaultTimerConditional, i);
            }
        }

        public void ResetSurvivors()
        {
            var is2v8Mode = Settings.Default.Is2v8Mode;
            maxTimer = is2v8Mode ? unhookEndurance2v8 : maxTimerDefault;
            survivorsCount = is2v8Mode ? survivorsCount2v8 : survivorsCountDefault;
            unhookEndurance = is2v8Mode ? unhookEndurance2v8 : unhookEnduranceDefault;
            Survivors.Clear();
            SetSurvivors();
            for (int i = 0; i < survivorsCount; i++)
            {
                HandleAction(KillerOverlayAction.ResetHooks, i);
            }
            SetTimers();
            ResizeLabels();
        }

        private void SetSurvivors()
        {
            for (int i = 1; i <= survivorsCount; i++)
            {
                Survivors.Add(new Survivor());
            }
        }

        private void ResizeLabels()
        {
            for (int i = 0; i < survivorsCount; i++)
            {
                HandleAction(KillerOverlayAction.ResizeLabels, i);
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
                    HandleAction(KillerOverlayAction.SetEnduranceTimer, index);
                });

                while (Survivors[index].State.Equals(SurvivorState.Unhooked) && watch.ElapsedMilliseconds <= maxTimer)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var elapsedTime = watch.ElapsedMilliseconds;
                        var time = (elapsedTime / 1000.0).Round(1).ToString();
                        var newTimerValue = time.IsInt() ? $"{time}{delimiter}0" : time;
                        HandleAction(KillerOverlayAction.SetTimerValue, index, newTimerValue);

                        if (elapsedTime > unhookEndurance - 100)
                        {
                            HandleAction(KillerOverlayAction.SetDefaultTimer, index);
                        }
                    });
                }
                watch.Stop();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    HandleAction(KillerOverlayAction.SetTimerValue, index, defaultTimerValue);
                });
            };
            worker.RunWorkerAsync();
        }

        private void HandleAction(KillerOverlayAction actionType, int survivorIndex, string argument = null)
        {
            if (Settings.Default.IsHookMode || Settings.Default.IsPostUnhookTimerMode) ActionFactory(Overlay, actionType, survivorIndex, argument);
            if (Settings.Default.IsSidePanelMode) ActionFactory(Window, actionType, survivorIndex, argument);
        }

        private void ActionFactory(KillerOverlayWindow killerWindow, KillerOverlayAction actionType, int index, string argument = null)
        {
            switch (actionType)
            {
                case KillerOverlayAction.IncrementHooks:
                    killerWindow.GetHooksLabel((SurvivorNumber)(index + 1)).IncrementHooks();
                    break;
                case KillerOverlayAction.SetTimerValue:
                    killerWindow.GetTimerLabel((SurvivorNumber)(index + 1)).Content = argument;
                    break;
                case KillerOverlayAction.SetEnduranceTimer:
                    killerWindow.GetTimerLabel((SurvivorNumber)(index + 1)).UpdateColors(Palette.DarkYellowBrush, Palette.DarkestGrayBrush);
                    break;
                case KillerOverlayAction.SetDefaultTimer:
                    killerWindow.GetTimerLabel((SurvivorNumber)(index + 1)).UpdateColors(Palette.DarkestGrayBrush, Palette.WhiteBrush);
                    break;
                case KillerOverlayAction.SetDefaultTimerConditional:
                    {
                        var textBlock = killerWindow.GetTimerLabel((SurvivorNumber)(index + 1));
                        textBlock.Content = defaultTimerValue;
                        if (textBlock.GetBorder() != null)
                        {
                            textBlock.UpdateColors(Palette.DarkestGrayBrush, Palette.WhiteBrush);
                        }
                        break;
                    }
                case KillerOverlayAction.ResetHooks:
                    {
                        var textBlock = killerWindow.GetHooksLabel((SurvivorNumber)(index + 1));
                        textBlock.Content = "0";
                        var labelBorder = textBlock.GetBorder();
                        if (labelBorder != null)
                        {
                            labelBorder.Background = Palette.DarkestGrayBrush;
                        }
                        break;
                    }
                case KillerOverlayAction.ResizeLabels:
                    {
                        var is2v8Mode = Settings.Default.Is2v8Mode;
                        killerWindow.GetHooksLabel((SurvivorNumber)(index + 1)).FontSize = is2v8Mode ? 16 : 20;
                        killerWindow.GetTimerLabel((SurvivorNumber)(index + 1)).FontSize = is2v8Mode ? 12 : 16;
                        break;
                    }
                default:
                    break;
            }
        }
    }
}
