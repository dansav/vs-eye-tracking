using EyeTrackingVsix.Features.Scroll;

namespace EyeTrackingVsix.Common.Configuration
{
    public interface IScrollSettings
    {
        int Velocity { get; }

        double LinearAccelerationTime { get; }

        double ExponentialPower { get; }

        double ExponentialInertia { get; }
    }
}
