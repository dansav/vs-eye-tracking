using System.Windows.Input;

namespace EyeTrackingVsix.Common
{
    public interface IKeyboardSettings
    {
        Key MoveCaretKey { get; }

        Key ScrollKey { get; }

        int DoubleTapReleaseTimeMs { get; }

        int DoubleTapIntervalTimeMs { get; }

        int DoubleTapHoldTimeMs { get; }
    }
}
