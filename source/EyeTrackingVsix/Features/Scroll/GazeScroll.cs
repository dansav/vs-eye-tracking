using Eyetracking.NET;
using EyeTrackingVsix.Common;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Collections;
using System.Windows;
using CoroutinesForWpf;

namespace EyeTrackingVsix.Features.Scroll
{
    public class GazeScroll
    {
        private readonly IWpfTextView _textView;
        private readonly KeyboardEventAggregator _keyboard;
        private readonly IEyetracker _eyetracker;
        private readonly IVelocityProvider _velocityProvider;

        private DateTime _timestamp;

        public GazeScroll(IWpfTextView textView, KeyboardEventAggregator keyboard, IEyetracker eyetracker, IVelocityProvider velocityProvider)
        {
            _textView = textView;
            _keyboard = keyboard;
            _eyetracker = eyetracker;

            _textView.Closed += OnTextViewClosed;
            _keyboard.UpdateScroll += OnUpdateScroll;
            _velocityProvider = velocityProvider;
        }

        private void OnTextViewClosed(object sender, EventArgs e)
        {
            _keyboard.UpdateScroll -= OnUpdateScroll;
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
            _velocityProvider.Stop();
        }

        private void StartScroll()
        {
            var elm = _textView.VisualElement;
            if (!_eyetracker.IsLookingAt(elm)) return;

            Executor.StartCoroutine(DoScroll());

            _timestamp = DateTime.Now;
            var direction = GetScrollDirection(elm);
            _velocityProvider.Start(direction);
        }

        private IEnumerator DoScroll()
        {
            while (_velocityProvider.HasVelocity)
            {
                var elapsed = (DateTime.Now - _timestamp).TotalSeconds;
                _timestamp = DateTime.Now;

                var scrollLength = _velocityProvider.Velocity * elapsed;
                _textView.ViewScroller.ScrollViewportVerticallyByPixels(scrollLength);

                yield return null;
            }
        }

        private int GetScrollDirection(FrameworkElement elm)
        {
            var gazePoint = elm.GetRelativeGazePoint(_eyetracker);
            var center = elm.ActualHeight / 2;
            return Math.Sign(center - gazePoint.Y);
        }
    }
}
