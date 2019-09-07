using Eyetracking.NET;
using EyeTrackingVsix.Common;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Collections;
using CoroutinesForWpf;

namespace EyeTrackingVsix.Features.Scroll
{
    public class GazeScroll
    {
        private readonly IWpfTextView _textView;
        private readonly KeyboardEventAggregator _keyboard;
        private readonly IEyetracker _eyetracker;
        private readonly IVelocityProvider _velocityProvider;

        private DateTime _timestamp;

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
            _keyboard.UpdateScroll -= OnUpdateScroll;
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
            _velocityProvider.Stop();
        }

        private void StartScroll()
        {
            IRelativeGazeTransformer relativeGaze = new CenterOfElementGazeTransformer(_textView.VisualElement, _eyetracker);

            if (!relativeGaze.HasGaze) return;

            Executor.StartCoroutine(DoScroll());

            _timestamp = DateTime.Now;
            _velocityProvider.Start(relativeGaze);
        }

        private IEnumerator DoScroll()
        {
            while (_velocityProvider.HasVelocity)
            {
                var elapsed = (DateTime.Now - _timestamp).TotalSeconds;
                _timestamp = DateTime.Now;

                var scrollLength = _velocityProvider.Velocity * elapsed;

                // invert direction since it is the viewport that scrolls and not the document
                // see https://docs.microsoft.com/en-us/dotnet/api/microsoft.visualstudio.text.editor.iviewscroller.scrollviewportverticallybypixels?view=visualstudiosdk-2019
                _textView.ViewScroller.ScrollViewportVerticallyByPixels(-scrollLength);

                yield return null;
            }
        }
    }
}
