using Eyetracking.NET;
using EyeTrackingVsix.Common;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Windows;
using System.Windows.Media;
using EyeTrackingVsix.Common.Configuration;
using EyeTrackingVsix.Options;

namespace EyeTrackingVsix.Features.Scroll
{
    public class GazeScroll
    {
        private readonly IWpfTextView _textView;
        private readonly KeyboardEventAggregator _keyboard;
        private readonly IEyetracker _eyetracker;
        private readonly IVelocityProvider _velocityProvider;

        private DateTime _timestamp;
        private bool _scroll;

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

            var elm = _textView.VisualElement;
            if (!_eyetracker.IsLookingAt(elm)) return;

            _timestamp = DateTime.Now;
            _scroll = true;
            var direction = GetScrollDirection(elm);
            _velocityProvider.Start(direction);

            CompositionTarget.Rendering += OnCompositionTargetRendering;
        }

        private void OnCompositionTargetRendering(object sender, EventArgs e)
        {
            if (!_scroll) return;

            var elapsed = (DateTime.Now - _timestamp).TotalSeconds;
            _timestamp = DateTime.Now;

            var scrollLength = _velocityProvider.Velocity * elapsed;
            _textView.ViewScroller.ScrollViewportVerticallyByPixels(scrollLength);
        }

        private int GetScrollDirection(FrameworkElement elm)
        {
            var gazePoint = elm.GetRelativeGazePoint(_eyetracker);
            var center = elm.ActualHeight / 2;
            return Math.Sign(center - gazePoint.Y);
        }
    }

    internal class ScrollSettings : IScrollSettings
    {
        private readonly GeneralOptions _options;

        public ScrollSettings(GeneralOptions options)
        {
            _options = options;
        }

        public int Velocity => _options.ScrollVelocity;
    }

    public interface IVelocityProvider
    {
        double Velocity { get; }

        void Start(int direction);
    }

    public class StaticVelocityProvider : IVelocityProvider
    {
        private readonly IScrollSettings _settings;

        public StaticVelocityProvider(IScrollSettings settings)
        {
            _settings = settings;
        }

        public double Velocity { get; private set; }

        public void Start(int direction)
        {
            Velocity = direction * _settings.Velocity;
        }
    }

}
