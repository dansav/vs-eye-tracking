using Eyetracking.NET;
using EyeTrackingVsix.Common;
using Microsoft.VisualStudio.Shell;

namespace EyeTrackingVsix
{
    public static class DteWindowExtensions
    {
        public static bool IsLookingAt(this IEyetracker eyetracker, EnvDTE.Window window)
        {
            var gazePixels = ScreenHelpers.GetGazePointInScreenPixels(eyetracker);

            ThreadHelper.ThrowIfNotOnUIThread();

            var x = gazePixels.X - window.Left;
            var y = gazePixels.Y - window.Top;

            return x > 0 && x < window.Width && y > 0 && y < window.Height;
        }
    }
}
