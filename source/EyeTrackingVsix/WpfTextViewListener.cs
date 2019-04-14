using Eyetracking.NET;
using EyeTrackingVsix.Common;
using EyeTrackingVsix.Features.MoveCarret;
using EyeTrackingVsix.Features.Scroll;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace EyeTrackingVsix
{
    /// <summary>
    /// Exports the <see cref="IWpfTextViewCreationListener"/> that will be the entry point for
    /// features that interact with the text editor (<see cref="IWpfTextView"/>)
    /// </summary>
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    public sealed class WpfTextViewListener : IWpfTextViewCreationListener
    {
        /// <summary>
        /// Set up keyboard and eye tracking handling related to text editor
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/> used to calculate gaze point</param>
        public void TextViewCreated(IWpfTextView textView)
        {
            var keyboard = new KeyboardEventAggregator(textView, new HardcodedKeyboardSettings());
            var eyetracker = Eyetracker.Desktop;
            new GazeScroll(textView, keyboard, eyetracker);
            new GazeCaret(textView, keyboard, eyetracker);
        }
    }
}
