using System;

namespace EyeTrackingVsix.Common
{
    public class DateTimeClock : IMeassureTime
    {
        private DateTime _start;

        public TimeSpan Elapsed => (DateTime.Now - _start);

        public void Start()
        {
            _start = DateTime.Now;
        }
    }
}
