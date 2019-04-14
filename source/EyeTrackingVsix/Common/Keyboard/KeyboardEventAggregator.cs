using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Windows;
using System.Windows.Input;

namespace EyeTrackingVsix.Common
{
    public class KeyboardEventAggregator
    {
        private readonly IWpfTextView _textView;
        private readonly KeyboardSequenceDetector[] _detectors;

        private ScrollRequest _scrollState;

        public KeyboardEventAggregator(IWpfTextView textView, IKeyboardSettings settings)
        {
            _textView = textView;

            var moveCaret = new KeyboardSequenceDetector(
                new DateTimeClock(),
                () => MoveCaret?.Invoke(),
                new[]
                {
                    new KeyFrame(settings.MoveCaretKey, true, 0),
                    new KeyFrame(settings.MoveCaretKey, false, settings.DoubleTapReleaseTimeMs),
                    new KeyFrame(settings.MoveCaretKey, true, settings.DoubleTapIntervalTimeMs),
                    new KeyFrame(settings.MoveCaretKey, false, settings.DoubleTapReleaseTimeMs),
                });

            var startScroll = new KeyboardSequenceDetector(
                new DateTimeClock(),
                () =>
                {
                    if (_scrollState != ScrollRequest.Start)
                    {
                        _scrollState = ScrollRequest.Start;
                        UpdateScroll?.Invoke(ScrollRequest.Start);
                    }
                },
                new[]
                {
                    new KeyFrame(settings.ScrollKey, true, 0),
                    new KeyFrame(settings.ScrollKey, false, settings.DoubleTapReleaseTimeMs),
                    new KeyFrame(settings.ScrollKey, true, settings.DoubleTapIntervalTimeMs),
                    new KeyFrame(settings.ScrollKey, true, settings.DoubleTapHoldTimeMs),
                });

            var stopScroll = new KeyboardSequenceDetector(
                new DateTimeClock(),
                () =>
                {
                    if (_scrollState != ScrollRequest.Stop)
                    {
                        _scrollState = ScrollRequest.Stop;
                        UpdateScroll?.Invoke(ScrollRequest.Stop);
                    }
                },
                new[]
                {
                    new KeyFrame(settings.ScrollKey, false, 0),
                });

            _detectors = new[]
            {
                moveCaret,
                startScroll,
                stopScroll
            };

            _textView.Closed += OnTextViewClosed;
            _textView.VisualElement.Loaded += VisualElementOnLoaded;
            _textView.VisualElement.Unloaded += VisualElementOnUnloaded;
        }

        public event Action<ScrollRequest> UpdateScroll;

        public event Action MoveCaret;

        private void OnTextViewClosed(object sender, EventArgs e)
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
            foreach (var detector in _detectors)
            {
                detector.Update(e.Key, e.IsDown);
            }
        }

        private void VisualElementOnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            foreach (var detector in _detectors)
            {
                detector.Update(e.Key, e.IsDown);
            }
        }
    }
}
