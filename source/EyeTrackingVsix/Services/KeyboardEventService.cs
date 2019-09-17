using System;
using System.Windows.Input;
using EyeTrackingVsix.Common;
using EyeTrackingVsix.Options;
using EyeTrackingVsix.Utils;

namespace EyeTrackingVsix.Services
{
    public class KeyboardEventService : SKeyboardEventService, IKeyboardEventService
    {
        private readonly KeyboardEventAggregator _aggregator;

        internal KeyboardEventService(Microsoft.VisualStudio.OLE.Interop.IServiceProvider serviceProvider, InputManager inputManager, GeneralOptions options)
        {
            _aggregator = new KeyboardEventAggregator(inputManager, new KeyboardSettings(options));
            _aggregator.MoveCaret += OnMoveCaret;
            _aggregator.UpdateScroll += OnUpdateScroll;

            _aggregator.Rebuild();
            options.GeneralOptionsChanged += () => _aggregator.Rebuild();
        }

        public event Action ChangeFocus;
        public event Action MoveCaret;
        public event Action<ScrollRequest> UpdateScroll;

        private void OnUpdateScroll(ScrollRequest request)
        {
            UpdateScroll?.Invoke(request);
        }

        private void OnMoveCaret()
        {
            Logger.Log("KeyboardEventService.OnMoveCaret 1");
            ChangeFocus?.Invoke();
            Logger.Log("KeyboardEventService.OnMoveCaret 2");
            MoveCaret?.Invoke();
            Logger.Log("KeyboardEventService.OnMoveCaret 3");
        }
    }

    public interface IKeyboardEventService
    {
        event Action ChangeFocus;
        event Action MoveCaret;
        event Action<ScrollRequest> UpdateScroll;
    }

    public interface SKeyboardEventService { }
}
