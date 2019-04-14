using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Eyetracking.NET;
using EyeTrackingVsix.Common;
using Microsoft.VisualStudio.Text.Editor;

namespace EyeTrackingVsix.Features.MoveCarret
{
    public class GazeCaret
    {
        private readonly IWpfTextView _textView;
        private readonly KeyboardEventAggregator _keyboard;
        private readonly IEyetracker _eyetracker;

        public GazeCaret(IWpfTextView textView, KeyboardEventAggregator keyboard, IEyetracker eyetracker)
        {
            _textView = textView;
            _keyboard = keyboard;
            _eyetracker = eyetracker;

            _keyboard.MoveCaret += OnCarretAction;
        }

        private void OnCarretAction()
        {
            var elm = _textView.VisualElement;
            if (!_eyetracker.IsLookingAt(elm)) return;

            var gazePoint = elm.GetRelativeGazePoint(_eyetracker);
            var gazePointInText = new Point(gazePoint.X + _textView.ViewportLeft, gazePoint.Y + _textView.ViewportTop);
            var verticalMargin = _textView.LineHeight * 3;

            var lookingAtLine = _textView.TextViewLines
                .Where(l => l.TextTop > gazePointInText.Y - verticalMargin && l.TextBottom < gazePointInText.Y + verticalMargin)
                .Where(l => l.TextWidth > gazePointInText.X)
                .OrderBy(l => Math.Abs(l.TextTop + (l.TextBottom - l.TextTop) / 2 - gazePointInText.Y))
                .FirstOrDefault();

            if (lookingAtLine == null)
            {
                Debug.WriteLine("Not found...");
                return;
            }

            var snapshotPoint = lookingAtLine.GetBufferPositionFromXCoordinate(gazePointInText.X);
            if (snapshotPoint.HasValue)
            {
                _textView.Caret.MoveTo(snapshotPoint.Value);
            }
        }
    }
}
