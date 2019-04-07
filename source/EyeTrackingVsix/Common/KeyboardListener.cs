using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Windows;
using System.Windows.Input;

namespace EyeTrackingVsix.Common
{
    internal class KeyboardListener
    {
        private readonly IWpfTextView _textView;

        private ScrollRequest _scrollState;

        public KeyboardListener(IWpfTextView textView)
        {
            _textView = textView;

            _textView.Closed += OnTextViewClosed;
            _textView.VisualElement.Loaded += VisualElementOnLoaded;
            _textView.VisualElement.Unloaded += VisualElementOnUnloaded;
        }

        public event Action<ScrollRequest> UpdateScroll;

        public event Action CarretAction;

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
            if (e.Key == Key.RightShift && e.IsUp) CarretAction?.Invoke();

            if (e.Key == Key.RightCtrl && e.IsUp)
            {
                if (_scrollState != ScrollRequest.Stop)
                {
                    _scrollState = ScrollRequest.Stop;
                    UpdateScroll?.Invoke(ScrollRequest.Stop);
                }
            }
        }

        private void VisualElementOnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.RightCtrl && e.IsDown)
            {
                if (_scrollState != ScrollRequest.Start)
                {
                    _scrollState = ScrollRequest.Start;
                    UpdateScroll?.Invoke(ScrollRequest.Start);
                }
            }
        }
    }
}
