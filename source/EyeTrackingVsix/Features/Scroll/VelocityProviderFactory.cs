using System;
using System.Collections.Generic;
using EyeTrackingVsix.Options;

namespace EyeTrackingVsix.Features.Scroll
{
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
