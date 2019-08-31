namespace EyeTrackingVsix.Features.Scroll
{
    public interface IVelocityProvider
    {
        bool HasVelocity { get; }

        double Velocity { get; }

        void Start(int direction);

        void Stop();
    }
}