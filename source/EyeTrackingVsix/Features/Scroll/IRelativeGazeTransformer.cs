using System.Windows;

namespace EyeTrackingVsix.Features.Scroll
{
    public interface IRelativeGazeTransformer
    {
        bool HasGaze { get; }

        (int X, int Y) Direction { get; }

        Vector NormalizedOffset { get; }
    }
}
