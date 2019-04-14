using Eyetracking.NET;
using EyeTrackingVsix.Common;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Windows;
using System.Windows.Media;

namespace EyeTrackingVsix.Features.Scroll
{
    public class GazeScroll
    {
        private readonly IWpfTextView _textView;
        private readonly KeyboardEventAggregator _keyboard;
        private readonly IEyetracker _eyetracker;
        private DateTime _timestamp;
        private bool _scroll;
        private double _direction;

        public GazeScroll(IWpfTextView textView, KeyboardEventAggregator keyboard, IEyetracker eyetracker)
        {
            _textView = textView;
            _keyboard = keyboard;
            _eyetracker = eyetracker;

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

            _direction = 0;

            var elm = _textView.VisualElement;
            if (!_eyetracker.IsLookingAt(elm)) return;

            _timestamp = DateTime.Now;
            _scroll = true;
            _direction = GetScrollDirection(elm);

            CompositionTarget.Rendering += OnCompositionTargetRendering;
        }

        private void OnCompositionTargetRendering(object sender, EventArgs e)
        {
            const double scrollVelocityPixelsPerSecond = 400;

            if (!_scroll) return;

            var elapsed = (DateTime.Now - _timestamp).TotalSeconds;
            _timestamp = DateTime.Now;

            if (_direction > 0 || _direction < 0)
            {
                var scrollLength = scrollVelocityPixelsPerSecond * elapsed * _direction;
                _textView.ViewScroller.ScrollViewportVerticallyByPixels(scrollLength);
            }
        }

        private double GetScrollDirection(FrameworkElement elm)
        {
            var gazePoint = elm.GetRelativeGazePoint(_eyetracker);
            var center = elm.ActualHeight / 2;
            return Math.Sign(center - gazePoint.Y);
        }
    }
}
