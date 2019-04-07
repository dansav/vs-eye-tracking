using Eyetracking.NET;
using EyeTrackingVsix.Common;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Windows;
using System.Windows.Media;

namespace EyeTrackingVsix.Features.Scroll
{
    internal class GazeScroll
    {
        private readonly IWpfTextView _textView;
        private readonly KeyboardListener _keyboard;
        private readonly IEyetracker _gaze;
        private DateTime _timestamp;
        private bool _scroll;
        private double _direction;

        public GazeScroll(IWpfTextView textView, KeyboardListener keyboard, IEyetracker eyetracker)
        {
            _textView = textView;
            _keyboard = keyboard;
            _gaze = eyetracker;

            _textView.Closed += OnTextViewClosed;
            _keyboard.UpdateScroll += OnUpdateScroll;
        }

        private void OnTextViewClosed(object sender, EventArgs e)
        {
            _textView.Closed -= OnTextViewClosed;
        }

        private void OnUpdateScroll(ScrollRequest newState)
        {
            switch (newState)
            {
                case ScrollRequest.Start:
                    StartScroll();
                    break;
                case ScrollRequest.Stop:
                    StopScroll();
                    break;
            }
        }

        private void StopScroll()
        {
            if (!_scroll) return;

            CompositionTarget.Rendering -= OnCompositionTargetRendering;
            _scroll = false;
        }

        private void StartScroll()
        {
            if (_scroll) return;

            _timestamp = DateTime.Now;
            _scroll = true;
            _direction = GetScrollDirection();

            CompositionTarget.Rendering += OnCompositionTargetRendering;
        }

        private void OnCompositionTargetRendering(object sender, EventArgs e)
        {
            const double scrollVelocityPixelsPerSecond = 300;

            if (!_scroll) return;

            var elapsed = (DateTime.Now - _timestamp).TotalSeconds;
            _timestamp = DateTime.Now;

            if (_direction > 0 || _direction < 0)
            {
                var scrollLength = scrollVelocityPixelsPerSecond * elapsed * _direction;
                _textView.ViewScroller.ScrollViewportVerticallyByPixels(scrollLength);
            }
        }

        private double GetScrollDirection()
        {
            var w = System.Windows.Forms.Screen.PrimaryScreen;
            var gx = _gaze.X * w.WorkingArea.Width;
            var gy = _gaze.Y * w.WorkingArea.Height;

            var elm = _textView.VisualElement;
            var elmTopLeftScreen = elm.PointToScreen(new Point(0, 0));
            var elmBottomRightScreen = elm.PointToScreen(new Point(elm.ActualWidth, elm.ActualHeight));

            if (gx > elmTopLeftScreen.X && gx < elmBottomRightScreen.X)
            {
                if (gy > elmTopLeftScreen.Y && gy < elmBottomRightScreen.Y)
                {
                    var center = elmTopLeftScreen.Y + (elmBottomRightScreen.Y - elmTopLeftScreen.Y) / 2;
                    return Math.Sign(center - gy);
                }
            }
            return 0;
        }
    }
}
