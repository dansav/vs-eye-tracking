using System;
using System.Windows;
using Eyetracking.NET;
using EyeTrackingVsix.Common;
using EyeTrackingVsix.Utils;

namespace EyeTrackingVsix.Services
{
    public class EyetrackerService : SEyetrackerService, IEyetrackerService
    {
        private readonly Eyetracker _eyetracker;

        public EyetrackerService(Microsoft.VisualStudio.OLE.Interop.IServiceProvider serviceProvider)
        {
            //TODO: this needs to be improved. Handle reconnection etc.
            try
            {
                _eyetracker = new Eyetracker();
            }
            catch (Exception e)
            {
                Logger.Log("Could not connect to eye tracker (see exception below). All features are disabled.");
                Logger.Log(e);
            }
        }

        public bool IsConnected => _eyetracker != null;

        public bool IsGazeInScreenRegion(Rect region)
        {
            var gazeScreenPoint = ScreenHelpers.GetGazePointInScreenPixels(_eyetracker);
            Logger.Log($"Gaze point: {gazeScreenPoint}");

            return region.Contains(gazeScreenPoint);
        }

        public bool IsLookingAt(FrameworkElement element)
        {
            return _eyetracker.IsLookingAt(element);
        }

        public Point GetRelativeGazePoint(FrameworkElement element)
        {
            var gazeScreenPoint = ScreenHelpers.GetGazePointInScreenPixels(_eyetracker);
            return element.PointFromScreen(gazeScreenPoint);
        }
    }

    public interface IEyetrackerService
    {
        bool IsConnected { get; }

        /// <summary>
        /// Region must be defined in native screen pixels with no DPI/Scaling applied.
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        bool IsGazeInScreenRegion(Rect region);

        bool IsLookingAt(FrameworkElement element);
        Point GetRelativeGazePoint(FrameworkElement elm);
    }

    // see https://docs.microsoft.com/en-us/visualstudio/extensibility/how-to-provide-a-service?view=vs-2019
    public interface SEyetrackerService { }
}
