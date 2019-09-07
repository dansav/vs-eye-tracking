﻿using System;
using Eyetracking.NET;
using EyeTrackingVsix.Common;
using EyeTrackingVsix.Features.MoveCarret;
using EyeTrackingVsix.Features.Scroll;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using EyeTrackingVsix.Options;
using EyeTrackingVsix.Utils;

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
            IEyetracker eyetracker;
            try
            {
                eyetracker = new Eyetracker();
            }
            catch (Exception e)
            {
                Logger.Log("Could not connect to eye tracker (see exception below). All features are disabled.");
                Logger.Log(e);
                return;
            }

            if (!GeneralOptions.Instance.CaretEnabled && !GeneralOptions.Instance.ScrollEnabled)
                return;

            var keyboard = new KeyboardEventAggregator(textView, new KeyboardSettings(GeneralOptions.Instance));

            if (GeneralOptions.Instance.ScrollEnabled)
            {
                // TODO: make it possible to switch velocity provider in an open document
                // NOTE: currently each open document will keep its velocity provider until the document is closed
                var velocityProvider = VelocityProviderFactory.Create(GeneralOptions.Instance.ScrollType);
                new GazeScroll(textView, keyboard, eyetracker, velocityProvider);
            }

            if (GeneralOptions.Instance.CaretEnabled)
            {
                new GazeCaret(textView, keyboard, eyetracker);
            }
        }
    }
}
