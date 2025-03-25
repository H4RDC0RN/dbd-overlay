using System.Windows;
using System.Windows.Controls.Primitives;

namespace DBDUtilityOverlay.Core.CustomItems
{
    public class CustomToggleButton : ToggleButton
    {
        public static readonly RoutedEvent CheckClickEvent = EventManager.RegisterRoutedEvent(
            "CheckClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CustomToggleButton));

        public static readonly RoutedEvent UncheckClickEvent = EventManager.RegisterRoutedEvent(
            "UncheckClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CustomToggleButton));

        public event RoutedEventHandler CheckClick
        {
            add { AddHandler(CheckClickEvent, value); }
            remove { RemoveHandler(CheckClickEvent, value); }
        }

        public event RoutedEventHandler UncheckClick
        {
            add { AddHandler(UncheckClickEvent, value); }
            remove { RemoveHandler(UncheckClickEvent, value); }
        }

        void RaiseCheckClickEvent()
        {
            var routedEventArgs = new RoutedEventArgs(CheckClickEvent);
            RaiseEvent(routedEventArgs);
        }

        void RaiseUnCheckClickEvent()
        {
            var routedEventArgs = new RoutedEventArgs(UncheckClickEvent);
            RaiseEvent(routedEventArgs);
        }

        protected override void OnClick()
        {
            base.OnClick();
            if (IsChecked == true) RaiseCheckClickEvent();
            else RaiseUnCheckClickEvent();
        }
    }
}
