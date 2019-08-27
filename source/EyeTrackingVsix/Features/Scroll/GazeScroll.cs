using Eyetracking.NET;
using EyeTrackingVsix.Common;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using EyeTrackingVsix.Common.Configuration;
using EyeTrackingVsix.Options;
using Microsoft.VisualStudio.Debugger.Interop;

namespace EyeTrackingVsix.Features.Scroll
{
    public class GazeScroll
    {
        private readonly IWpfTextView _textView;
        private readonly KeyboardEventAggregator _keyboard;
        private readonly IEyetracker _eyetracker;
        private readonly IVelocityProvider _velocityProvider;

        private DateTime _timestamp;
        private IDisposable _scroll;

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

            _scroll?.Dispose();
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

            Animator.StartCoroutine(DoScroll());

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
        bool HasVelocity { get; }

        double Velocity { get; }

        void Start(int direction);

        void Stop();
    }

    public class StaticVelocityProvider : IVelocityProvider
    {
        private readonly IScrollSettings _settings;

        public StaticVelocityProvider(IScrollSettings settings)
        {
            _settings = settings;
        }

        public bool HasVelocity { get; private set; }

        public double Velocity { get; private set; }
        
        public void Start(int direction)
        {
            Velocity = direction * _settings.Velocity;
            HasVelocity = true;
        }

        public void Stop()
        {
            Velocity = 0;
            HasVelocity = false;
        }
    }

    public class LinearVelocityProvider : IVelocityProvider
    {
        private const double AccelerationTimeSeconds = 3.0;

        private readonly IScrollSettings _settings;
        private DateTimeOffset _start;
        
        private double _baseVelocity;

        public LinearVelocityProvider(IScrollSettings settings)
        {
            _settings = settings;
        }

        public bool HasVelocity { get; private set; }

        public double Velocity
        {
            get
            {
                var accProgress = Math.Min(1, (DateTimeOffset.Now - _start).TotalSeconds);
                return _baseVelocity * accProgress;
            }
        }

        public void Start(int direction)
        {
            _start = DateTimeOffset.Now;
            _baseVelocity = direction * _settings.Velocity;
            HasVelocity = true;
        }

        public void Stop()
        {
            HasVelocity = false;
        }
    }

    public static class Animator
    {
        public static IDisposable StartCoroutine(IEnumerator routine)
        {
            return new Runner(routine);
        }

        private class Runner : IDisposable
        {
            private Routine _routine;

            public Runner(IEnumerator routine)
            {
                _routine = new Routine(routine);
                CompositionTarget.Rendering += Pump;
            }

            public void Dispose()
            {
                CompositionTarget.Rendering -= Pump;
            }

            private void Pump(object sender, EventArgs eventArgs)
            {
                if (_routine.Value.MoveNext())
                {
                    if (_routine.Value.Current is IEnumerator e)
                    {
                        _routine = new Routine(e, _routine);
                    }
                }
                else if (_routine.Parent != null)
                {
                    _routine = _routine.Parent;
                    Pump(sender, eventArgs);
                }
                else
                {
                    CompositionTarget.Rendering -= Pump;
                }
            }
        }

        private class Routine
        {
            public Routine(IEnumerator value, Routine parent = null)
            {
                Value = value;
                Parent = parent;
            }

            public Routine Parent { get; }
            public IEnumerator Value { get; }
        }
    }

}
