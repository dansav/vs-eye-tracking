using EyeTrackingVsix.Common.Configuration;
using EyeTrackingVsix.Options;

namespace EyeTrackingVsix.Features.Scroll
{
    internal class ScrollSettings : IScrollSettings
    {
        private readonly GeneralOptions _options;

        public ScrollSettings(GeneralOptions options)
        {
            _options = options;
        }

        public int Velocity => _options.ScrollVelocity;

        public double LinearAccelerationTime => _options.ScrollLinearAccelerationTime;

        public double ExponentialPower => _options.ScrollExponentialAccelerationPower;

        public double ExponentialInertia => _options.ScrollExponentialInertia;

        public VelocityCurve DynamicVelocityCurve => _options.ScrollDynamicCurve;
    }
}
