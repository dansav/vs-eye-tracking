using System;
using System.Collections.Generic;
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
    }

    public enum ScrollType
    {
        Static,
        Linear,
        Exponential,
        Dynamic
    }

    public static class VelocityProviderFactory
    {
        private static readonly Dictionary<ScrollType, Func<IVelocityProvider>> Generators = new Dictionary<ScrollType, Func<IVelocityProvider>>()
        {
            { ScrollType.Static, () => new StaticVelocityProvider(new ScrollSettings(GeneralOptions.Instance)) },
            { ScrollType.Linear, () => new LinearVelocityProvider(new ScrollSettings(GeneralOptions.Instance)) },
            { ScrollType.Exponential, () => new ExponentialVelocityProvider(new ScrollSettings(GeneralOptions.Instance)) },
            { ScrollType.Dynamic, () => new DynamicVelocityProvider(new ScrollSettings(GeneralOptions.Instance)) },
        };

        public static IVelocityProvider Create(ScrollType type)
        {
            return Generators[type]();
        }
    }

}
