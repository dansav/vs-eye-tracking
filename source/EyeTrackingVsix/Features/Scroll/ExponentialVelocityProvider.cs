using System;
using EyeTrackingVsix.Common.Configuration;

namespace EyeTrackingVsix.Features.Scroll
{
    public class ExponentialVelocityProvider : IVelocityProvider
    {
        private readonly IScrollSettings _settings;

        private bool _keyDown;
        private double _velocity;
        private double _baseVelocity;

        public ExponentialVelocityProvider(IScrollSettings settings)
        {
            _settings = settings;
        }

        public bool HasVelocity => _keyDown || Math.Abs(_velocity) > 0.1;
        public double Velocity
        {
            get
            {
                // break is quicker than accelerate
                var alpha = _keyDown
                    ? Clamp(_settings.ExponentialPower, 0, 1) // 0.005
                    : Clamp(_settings.ExponentialInertia, 0, 1); // 0.025;

                Exp(ref _velocity, _baseVelocity, alpha);

                return _velocity;
            }
        }
        public void Start(int direction)
        {
            _keyDown = true;
            _baseVelocity = direction * _settings.Velocity;
        }

        public void Stop()
        {
            _keyDown = false;
            _baseVelocity = 0;
        }

        private static void Exp(ref double mem, double value, double alpha)
        {
            mem = mem * (1 - alpha) + value * alpha;
        }

        private static double Clamp(double value, double min, double max)
        {
            if (value > max) return max;
            if (value < min) return min;
            return value;
        }
    }
}
