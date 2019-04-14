using System.Windows.Input;

namespace EyeTrackingVsix.Common
{
    public class HardcodedKeyboardSettings : IKeyboardSettings
    {
        public Key MoveCaretKey => Key.RightCtrl;

        public Key ScrollKey => Key.RightCtrl;

        public int DoubleTapReleaseTimeMs => 150;

        public int DoubleTapIntervalTimeMs => 250;

        public int DoubleTapHoldTimeMs => 550;
    }
}
