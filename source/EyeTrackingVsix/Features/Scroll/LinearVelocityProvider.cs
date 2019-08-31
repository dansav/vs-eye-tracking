using System;
using EyeTrackingVsix.Common.Configuration;

namespace EyeTrackingVsix.Features.Scroll
{
    public class LinearVelocityProvider : IVelocityProvider
    {
        private readonly IScrollSettings _settings;
        private DateTimeOffset _start;
        
        private double _baseVelocity;

        public LinearVelocityProvider(IScrollSettings settings)
        {
            _settings = settings;
        }

        public bool HasVelocity { get; private set; }

        public double Velocity
        {
            get
            {
                var time = _settings.LinearAccelerationTime;
                if (time < 0.1) time = 0.1;

                var accProgress = Math.Min(time, (DateTimeOffset.Now - _start).TotalSeconds) / time;
                return _baseVelocity * accProgress;
            }
        }

        public void Start(int direction)
        {
            _start = DateTimeOffset.Now;
            _baseVelocity = direction * _settings.Velocity;
            HasVelocity = true;
        }

        public void Stop()
        {
            HasVelocity = false;
        }
    }
}
