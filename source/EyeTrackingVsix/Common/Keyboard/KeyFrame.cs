using System.Windows.Input;

namespace EyeTrackingVsix.Common
{
    public class KeyFrame
    {
        public KeyFrame(Key key, bool down, double time)
        {
            Key = key;
            Down = down;
            Time = time;
        }

        public Key Key { get; }
        public bool Down { get; }
        public double Time { get; }
    }
}
