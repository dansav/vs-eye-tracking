using System.Collections.Generic;
using System.Windows.Input;
using EyeTrackingVsix.Common.Keyboard;
using EyeTrackingVsix.Options;

namespace EyeTrackingVsix.Common
{
    /// <summary>
    /// NOTE: keys are still hardcoded
    /// </summary>
    public class KeyboardSettings : IKeyboardSettings
    {
        private static readonly IReadOnlyDictionary<InteractionKey, Key> _lookup = new Dictionary<InteractionKey, Key>
        {
            { InteractionKey.LeftShift, Key.LeftShift },
            { InteractionKey.LeftCtrl, Key.LeftCtrl },
            { InteractionKey.RightShift, Key.RightShift },
            { InteractionKey.RightCtrl, Key.RightCtrl },
        };

        private readonly GeneralOptions _options;

        internal KeyboardSettings(GeneralOptions options)
        {
            _options = options;
        }

        public Key MoveCaretKey => _lookup[_options.CaretKey];

        public Key ScrollKey => _lookup[_options.ScrollKey];

        public int DoubleTapReleaseTimeMs => _options.KeyTapReleaseTimeMs;

        public int DoubleTapIntervalTimeMs => _options.DoubleTapIntervalTimeMs;

        public int DoubleTapHoldTimeMs => _options.KeyTapHoldTimeMs;
    }

    public class HardcodedKeyboardSettings : IKeyboardSettings
    {
        public Key MoveCaretKey => Key.RightCtrl;

        public Key ScrollKey => Key.RightCtrl;

        public int DoubleTapReleaseTimeMs => 150;

        public int DoubleTapIntervalTimeMs => 250;

        public int DoubleTapHoldTimeMs => 550;
    }
}
