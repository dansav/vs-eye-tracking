using System;
using System.Collections.Generic;
using EyeTrackingVsix.Options;

namespace EyeTrackingVsix.Features.Scroll
{
    public class VelocityProviderFactory : IVelocityProviderFactory
    {
        private static readonly Dictionary<ScrollType, Func<IVelocityProvider>> Generators = new Dictionary<ScrollType, Func<IVelocityProvider>>
        {
            { ScrollType.Static, () => new StaticVelocityProvider(new ScrollSettings(GeneralOptions.Instance)) },
            { ScrollType.Linear, () => new LinearVelocityProvider(new ScrollSettings(GeneralOptions.Instance)) },
            { ScrollType.Exponential, () => new ExponentialVelocityProvider(new ScrollSettings(GeneralOptions.Instance)) },
            { ScrollType.Dynamic, () => new DynamicVelocityProvider(new ScrollSettings(GeneralOptions.Instance)) },
        };

        private static readonly Dictionary<ScrollType, Type> ImplementationTypeLookup = new Dictionary<ScrollType, Type>
        {
            { ScrollType.Static, typeof(StaticVelocityProvider) },
            { ScrollType.Linear, typeof(LinearVelocityProvider) },
            { ScrollType.Exponential, typeof(ExponentialVelocityProvider) },
            { ScrollType.Dynamic, typeof(DynamicVelocityProvider) },
        };

        public bool VerifyProviderType(ScrollType type, IVelocityProvider provider)
        {
            return ImplementationTypeLookup.ContainsKey(type) 
                && ImplementationTypeLookup[type] == provider?.GetType();
        }

        public IVelocityProvider Create(ScrollType type)
        {
            return Generators[type]();
        }
    }

    public interface IVelocityProviderFactory
    {
        bool VerifyProviderType(ScrollType type, IVelocityProvider provider);

        IVelocityProvider Create(ScrollType type);
    }
}
