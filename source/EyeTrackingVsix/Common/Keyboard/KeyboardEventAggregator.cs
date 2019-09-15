﻿using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Windows;
using System.Windows.Input;

namespace EyeTrackingVsix.Common
{
    public class KeyboardEventAggregator
    {
        private readonly FrameworkElement _element;
        private readonly IKeyboardSettings _settings;

        private KeyboardSequenceDetector[] _detectors;

        private ScrollRequest _scrollState;

        public KeyboardEventAggregator(FrameworkElement element, IKeyboardSettings settings)
        {
            _element = element;
            _settings = settings;

            _element.PreviewKeyDown += VisualElementOnPreviewKeyDown;
            _element.PreviewKeyUp += VisualElementOnPreviewKeyUp;
        }

        public event Action<ScrollRequest> UpdateScroll;

        public event Action MoveCaret;

        public void Rebuild()
        {
            //string docName = "UNKNOWN";
            //if (_textView.TextBuffer.Properties.TryGetProperty<ITextDocument>(typeof(ITextDocument), out var textDoc))
            //{
            //    docName = System.IO.Path.GetFileName(textDoc.FilePath);
            //}
            //Logger.Log($"Rebuilding keyboard handler for {docName}");

            var moveCaret = new KeyboardSequenceDetector(
                new DateTimeClock(),
                () => MoveCaret?.Invoke(),
                new[]
                {
                    new KeyFrame(_settings.MoveCaretKey, true, 0),
                    new KeyFrame(_settings.MoveCaretKey, false, _settings.DoubleTapReleaseTimeMs),
                    new KeyFrame(_settings.MoveCaretKey, true, _settings.DoubleTapIntervalTimeMs),
                    new KeyFrame(_settings.MoveCaretKey, false, _settings.DoubleTapReleaseTimeMs),
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
                    new KeyFrame(_settings.ScrollKey, true, 0),
                    new KeyFrame(_settings.ScrollKey, false, _settings.DoubleTapReleaseTimeMs),
                    new KeyFrame(_settings.ScrollKey, true, _settings.DoubleTapIntervalTimeMs),
                    new KeyFrame(_settings.ScrollKey, true, _settings.DoubleTapHoldTimeMs),
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
                    new KeyFrame(_settings.ScrollKey, false, 0),
                });

            _detectors = new[]
            {
                moveCaret,
                startScroll,
                stopScroll
            };
        }

        private void VisualElementOnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            var detectors = _detectors;
            foreach (var detector in detectors)
            {
                detector.Update(e.Key, e.IsDown);
            }
        }

        private void VisualElementOnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            var detectors = _detectors;
            foreach (var detector in detectors)
            {
                detector.Update(e.Key, e.IsDown);
            }
        }
    }
}
