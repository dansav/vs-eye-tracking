using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using EnvDTE;
using EnvDTE80;
using Eyetracking.NET;
using EyeTrackingVsix.Common;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace EyeTrackingVsix.Features.MoveCarret
{
    internal class GazeCaret
    {
        private readonly IWpfTextView _textView;
        private readonly KeyboardListener _keyboard;
        private readonly IEyetracker _eyetracker;

        public GazeCaret(IWpfTextView textView, KeyboardListener keyboard, IEyetracker eyetracker)
        {
            _textView = textView;
            _keyboard = keyboard;
            _eyetracker = eyetracker;

            _keyboard.CarretAction += OnCarretAction;
        }

        private void OnCarretAction()
        {
            var w = System.Windows.Forms.Screen.PrimaryScreen;
            var gx = _eyetracker.X * w.WorkingArea.Width;
            var gy = _eyetracker.Y * w.WorkingArea.Height;

            var elm = _textView.VisualElement;
            var elmTopLeftScreen = elm.PointToScreen(new Point(0, 0));
            var elmBottomRightScreen = elm.PointToScreen(new Point(elm.ActualWidth, elm.ActualHeight));

            if (gx > elmTopLeftScreen.X && gx < elmBottomRightScreen.X)
            {
                if (gy > elmTopLeftScreen.Y && gy < elmBottomRightScreen.Y)
                {
                    var localX = gx - elmTopLeftScreen.X;
                    var localY = gy - elmTopLeftScreen.Y;

                    var lookingAtLine = _textView.TextViewLines.GetTextViewLineContainingYCoordinate(localY + _textView.ViewportTop);

                    if (lookingAtLine == null)
                    {
                        Debug.WriteLine("Not found...");
                        return;
                    }

                    SnapshotPoint? snapshotPoint = lookingAtLine.GetBufferPositionFromXCoordinate(localX + _textView.ViewportLeft);
                    if (snapshotPoint.HasValue)
                    {
                        _textView.Caret.MoveTo(snapshotPoint.Value);

                        // TODO: no static access
                        var dte = (DTE2)Package.GetGlobalService(typeof(DTE));
                        dte.ExecuteCommand("View.QuickActions");
                    }

                }
            }
        }
    }
}
