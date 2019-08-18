using System.Windows.Input;
using EyeTrackingVsix.Options;

namespace EyeTrackingVsix.Common
{
    /// <summary>
    /// NOTE: keys are still hardcoded
    /// </summary>
    public class KeyboardSettings : IKeyboardSettings
    {
        internal KeyboardSettings(GeneralOptions options)
        {
            // settings are constant during the lifetime of a document window
            DoubleTapReleaseTimeMs = options.KeyTapReleaseTimeMs;
            DoubleTapIntervalTimeMs = options.DoubleTapIntervalTimeMs;
            DoubleTapHoldTimeMs = options.KeyTapHoldTimeMs;
        }

        public Key MoveCaretKey => Key.RightCtrl;

        public Key ScrollKey => Key.RightCtrl;

        public int DoubleTapReleaseTimeMs { get; }

        public int DoubleTapIntervalTimeMs { get; }

        public int DoubleTapHoldTimeMs { get; }
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
