using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace DBDOverlay.Core.CustomItems
{
    public class ReadOnlyTextBox : TextBox
    {
        private ScrollViewer scrollViewer;
        private ScrollBar scrollBar;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            scrollViewer = GetTemplateChild("PART_ContentHost") as ScrollViewer;
            scrollBar = GetTemplateChild("PART_HorizontalScrollBar") as ScrollBar;

            if (scrollViewer != null && scrollBar != null)
            {
                scrollBar.Scroll += ScrollBar_Scroll;
            }

            TextChanged -= OnTextChangedInternal;
            TextChanged += OnTextChangedInternal;
        }

        private void ScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            scrollViewer?.ScrollToHorizontalOffset(e.NewValue);
        }

        private void OnTextChangedInternal(object sender, TextChangedEventArgs e)
        {
            if (scrollViewer == null || scrollBar == null)
                return;

            Dispatcher.BeginInvoke(DispatcherPriority.Loaded,
                (Action)(() =>
                    {
                        scrollViewer.ScrollToHorizontalOffset(0);
                        scrollBar.Value = 0;
                    }));
        }
    }
}
