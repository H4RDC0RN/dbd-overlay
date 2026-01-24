using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace DBDOverlay.Core.CustomItems
{
    public class ReadOnlyTextBox : TextBox
    {
        private ScrollViewer _scrollViewer;
        private ScrollBar _scrollBar;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _scrollViewer = GetTemplateChild("PART_ContentHost") as ScrollViewer;
            _scrollBar = GetTemplateChild("PART_HorizontalScrollBar") as ScrollBar;

            if (_scrollViewer != null && _scrollBar != null)
            {
                _scrollBar.Scroll += ScrollBar_Scroll;
            }
        }

        private void ScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            _scrollViewer?.ScrollToHorizontalOffset(e.NewValue);
        }
    }
}
