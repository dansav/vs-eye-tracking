using EyeTrackingVsix.Features.Scroll;
using EyeTrackingVsix.Options;

namespace EyeTrackingVsix.Services
{
    class ScrollVelocityService : SScrollVelocityService, IScrollVelocityService
    {
        private readonly IVelocityProviderFactory _factory;

        private IVelocityProvider _current;

        public ScrollVelocityService(Microsoft.VisualStudio.OLE.Interop.IServiceProvider serviceProvider)
        {
            _factory = new VelocityProviderFactory();
        }

        public bool HasVelocity => _current?.HasVelocity == true;

        public double Velocity => _current?.Velocity ?? 0;
        
        public void Start(IRelativeGazeTransformer relativeGaze)
        {
            if (_current == null || _factory.VerifyProviderType(GeneralOptions.Instance.ScrollType, _current) == false)
            {
                _current = _factory.Create(GeneralOptions.Instance.ScrollType);
            }
            _current.Start(relativeGaze);
        }

        public void Stop()
        {
            _current.Stop();
        }
    }

    public interface IScrollVelocityService : IVelocityProvider
    { }

    // see https://docs.microsoft.com/en-us/visualstudio/extensibility/how-to-provide-a-service?view=vs-2019
    public interface SScrollVelocityService { }
}
