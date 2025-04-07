using DBDOverlay.Core.Extensions;
using DBDOverlay.Core.Utils;
using DBDOverlay.Windows;
using System.Collections.Generic;
using System.Linq;
using Application = System.Windows.Application;

namespace DBDOverlay.Core.KillerOverlay
{
    public class KillerOverlayController
    {
        public List<Survivor> Survivors = new List<Survivor>();
        private readonly double treshold = 0.9;
        private static KillerOverlayController instance;
        private static HooksOverlayWindow hooksOverlay;
        private static PostUnhookTimerOverlayWindow postUnhookTimerOverlay;

        public static KillerOverlayController Instance
        {
            get
            {
                if (instance == null)
                    instance = new KillerOverlayController();
                return instance;
            }
        }

        public static HooksOverlayWindow HooksOverlay
        {
            get
            {
                if (hooksOverlay == null)
                    hooksOverlay = new HooksOverlayWindow();
                return hooksOverlay;
            }
        }

        public static PostUnhookTimerOverlayWindow PostUnhookTimerOverlay
        {
            get
            {
                if (postUnhookTimerOverlay == null)
                    postUnhookTimerOverlay = new PostUnhookTimerOverlayWindow();
                return postUnhookTimerOverlay;
            }
        }

        public KillerOverlayController()
        {
            for (int i = 1; i <= 4; i++)
            {
                Survivors.Add(new Survivor());
            }
        }

        public void CheckIfHooked(int index, double similarity)
        {
            if (!Survivors[index].State.Equals(SurvivorState.Hooked) && similarity > treshold)
            {
                Logger.Info($"--- Survovor {index} is hooked. 'Hooked' image similarity = {similarity * 100} %");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var textBlock = HooksOverlay.GetHooksLabel((SurvivorNumber)(index + 1));
                    textBlock.Content = textBlock.Content.ToString().Increment().ToString();
                    Survivors[index].State = SurvivorState.Hooked;
                    Survivors[index].Hooks++;
                });
            }
        }

        public void CheckIfUnhooked(int index, double similarity)
        {
            if (Survivors[index].State.Equals(SurvivorState.Hooked) && similarity < treshold)
            {
                Logger.Info($"--- Survovor {index} is unhooked. 'Hooked' image similarity = {similarity * 100} %");
                Survivors[index].State = SurvivorState.Unhooked;
            }
        }

        public void CheckIfRefreshed(int index, Dictionary<string, double> refreshStates)
        {
            var max = refreshStates.Values.Max();
            var pair = refreshStates.FirstOrDefault(x => x.Value.Equals(max));

            if ((Survivors[index].State.Equals(SurvivorState.Hooked) || Survivors[index].State.Equals(SurvivorState.Unhooked)) && pair.Value > treshold)
            {
                Logger.Info($"--- Survovor {index} is {pair.Key.ToLower()}. '{pair.Key}' image similarity = {pair.Value * 100} %");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var textBlock = HooksOverlay.GetHooksLabel((SurvivorNumber)(index + 1));
                    textBlock.Content = "0";
                    Survivors[index] = new Survivor();
                });
            }
        }
    }
}
