using EyeTrackingVsix.Common.Configuration;

namespace EyeTrackingVsix.Features.Scroll
{
    public class DynamicVelocityProvider : IVelocityProvider
    {
        private readonly IScrollSettings _settings;

        private IRelativeGazeTransformer _relativeGaze;

        public DynamicVelocityProvider(IScrollSettings settings)
        {
            _settings = settings;
        }

        public bool HasVelocity => _relativeGaze != null;

        public double Velocity => HasVelocity ? CalculateVelocityBasedOnCurrentGaze() : 0;

        public void Start(IRelativeGazeTransformer relativeGaze)
        {
            _relativeGaze = relativeGaze;
        }

        public void Stop()
        {
            _relativeGaze = null;
        }

        private double CalculateVelocityBasedOnCurrentGaze()
        {
            var verticalOffset = _relativeGaze.NormalizedOffset.Y;

            // apply Quadratic easing
            //verticalOffset = Math.Sign(verticalOffset) * (verticalOffset * verticalOffset);

            // apply sine easing
            //verticalOffset = 0.5 * Math.Sign(verticalOffset) * (1 - Math.Cos(Math.PI * verticalOffset));

            // apply cubic easing
            verticalOffset = verticalOffset * verticalOffset * verticalOffset;

            // visualization of the curves: https://www.desmos.com/calculator/x8us9obuu0
            //TODO: allow user to select easing

            return _settings.Velocity * verticalOffset;
        }
    }
}
