﻿using System.ComponentModel.Composition;
using EyeTrackingVsix.Features.MoveCarret;
using EyeTrackingVsix.Features.Scroll;
using EyeTrackingVsix.Options;
using EyeTrackingVsix.Services;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

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
        private readonly IEyetrackerService _eyetracker;
        private readonly IKeyboardEventService _keyboardService;
        private readonly IScrollVelocityService _scrollVelocityService;

        [ImportingConstructor]
        public WpfTextViewListener(SVsServiceProvider serviceProvider)
        {
            _eyetracker =  (IEyetrackerService)serviceProvider.GetService(typeof(SEyetrackerService));
            _keyboardService = (IKeyboardEventService)serviceProvider.GetService(typeof(SKeyboardEventService));
            _scrollVelocityService = (IScrollVelocityService)serviceProvider.GetService(typeof(SScrollVelocityService));
        }

        /// <summary>
        /// Set up keyboard and eye tracking handling related to text editor
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/> used to calculate gaze point</param>
        public void TextViewCreated(IWpfTextView textView)
        {
            if (_eyetracker == null || _eyetracker.IsConnected == false)
                return;

            if (!GeneralOptions.Instance.CaretEnabled && !GeneralOptions.Instance.ScrollEnabled)
                return;

            if (GeneralOptions.Instance.ScrollEnabled)
            {
                new GazeScroll(textView, _keyboardService, _eyetracker, _scrollVelocityService);
            }

            if (GeneralOptions.Instance.CaretEnabled)
            {
                new GazeCaret(textView, _keyboardService, _eyetracker);
            }
        }
    }
}
