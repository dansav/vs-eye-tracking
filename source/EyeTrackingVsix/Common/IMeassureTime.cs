using System;

namespace EyeTrackingVsix.Common
{
    public interface IMeassureTime
    {
        TimeSpan Elapsed { get; }
        void Start();
    }
}
