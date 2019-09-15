using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using EyeTrackingVsix.Services;
using EyeTrackingVsix.Utils;
using Microsoft.VisualStudio.Text.Editor;

namespace EyeTrackingVsix.Features.MoveCarret
{
    public class GazeCaret
    {
        private readonly IWpfTextView _textView;
        private readonly IKeyboardEventService _keyboard;
        private readonly IEyetrackerService _eyetracker;

        public GazeCaret(IWpfTextView textView, IKeyboardEventService keyboard, IEyetrackerService eyetracker)
        {
            _textView = textView;
            _keyboard = keyboard;
            _eyetracker = eyetracker;

            _textView.Closed += OnTextViewClosed;
            _keyboard.MoveCaret += OnCarretAction;
        }

        private void OnTextViewClosed(object sender, EventArgs e)
        {
            _keyboard.MoveCaret -= OnCarretAction;
            _textView.Closed -= OnTextViewClosed;
        }

        private void OnCarretAction()
        {
            Logger.Log($"GazeCaret.OnCarretAction {_textView.HasAggregateFocus}");


            //TODO: maybe...
            if (!_textView.HasAggregateFocus) return;

            var elm = _textView.VisualElement;
            if (!_eyetracker.IsLookingAt(elm)) return;

            var gazePoint = _eyetracker.GetRelativeGazePoint(elm);
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
