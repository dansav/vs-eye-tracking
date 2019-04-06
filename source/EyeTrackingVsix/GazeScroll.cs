using Eyetracking.NET;
using Microsoft.VisualStudio.Text.Editor;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace EyeTrackingVsix
{
    internal class GazeScroll
    {
        private IWpfTextView _textView;
        private IEyetracker _gaze;

        public GazeScroll(IWpfTextView textView)
        {
            _textView = textView;
            _gaze = Eyetracker.Desktop;

            _textView.Closed += OnTextViewClosed;
            _textView.VisualElement.Loaded += VisualElementOnLoaded;
            _textView.VisualElement.Unloaded += VisualElementOnUnloaded;
        }

        private void OnTextViewClosed(object sender, System.EventArgs e)
        {
            _textView.Closed -= OnTextViewClosed;
            _textView.VisualElement.Loaded -= VisualElementOnLoaded;
            _textView.VisualElement.Unloaded -= VisualElementOnUnloaded;
            _textView.VisualElement.PreviewKeyDown -= VisualElementOnPreviewKeyDown;
            _textView.VisualElement.PreviewKeyUp -= VisualElementOnPreviewKeyUp;
        }

        private void VisualElementOnLoaded(object sender, RoutedEventArgs e)
        {
            _textView.VisualElement.PreviewKeyDown += VisualElementOnPreviewKeyDown;
            _textView.VisualElement.PreviewKeyUp += VisualElementOnPreviewKeyUp;
        }

        private void VisualElementOnUnloaded(object sender, RoutedEventArgs e)
        {
            _textView.VisualElement.PreviewKeyDown -= VisualElementOnPreviewKeyDown;
            _textView.VisualElement.PreviewKeyUp -= VisualElementOnPreviewKeyUp;
        }

        private void VisualElementOnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.RightCtrl && e.IsUp) Debug.WriteLine("----- UP");
        }

        private void VisualElementOnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.RightCtrl && e.IsDown)
            {
                var w = System.Windows.Forms.Screen.PrimaryScreen;
                var gx = _gaze.X * w.WorkingArea.Width;
                var gy = _gaze.Y * w.WorkingArea.Height;

                var elm = _textView.VisualElement;
                Point elmTopLeftScreen = elm.PointToScreen(new Point(0, 0));
                Point elmBottomRightScreen = elm.PointToScreen(new Point(elm.ActualWidth, elm.ActualHeight));

                if (gx > elmTopLeftScreen.X && gx < elmBottomRightScreen.X)
                {
                    if (gy > elmTopLeftScreen.Y && gy < elmBottomRightScreen.Y)
                    {
                        var center = elmTopLeftScreen.Y + (elmBottomRightScreen.Y - elmTopLeftScreen.Y) / 2;

                        var scrollLength = (center - gy) / 50;
                        Debug.WriteLine($"----- scrollLength: {scrollLength}");

                        _textView.ViewScroller.ScrollViewportVerticallyByPixels(scrollLength);
                    }
                }

                //Debug.WriteLine("----- DOWN");
            }
        }
    }
}