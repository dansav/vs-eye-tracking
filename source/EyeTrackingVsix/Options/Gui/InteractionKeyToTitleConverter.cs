using System;
using System.Globalization;
using System.Windows.Data;
using EyeTrackingVsix.Common.Keyboard;
using EyeTrackingVsix.Features.Scroll;

namespace EyeTrackingVsix.Options.Gui
{
    public class InteractionKeyToTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is InteractionKey key)
            {
                switch (key)
                {
                    case InteractionKey.LeftShift:
                        return "Left Shift";
                    case InteractionKey.LeftCtrl:
                        return "Left Ctrl";
                    case InteractionKey.RightShift:
                        return "Right Shift";
                    case InteractionKey.RightCtrl:
                        return "Right Ctrl";
                    default:
                        return key.ToString();
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ScrollTypeToTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ScrollType key)
            {
                switch (key)
                {
                    case ScrollType.Static:
                        return "Static, a fixed speed";
                    case ScrollType.Linear:
                        return "Linear, accelerates from zero to max during a fixed time";
                    case ScrollType.Exponential:
                        return "Exponential, uses an exponential curve for both acceleration and inertia";
                    case ScrollType.Dynamic:
                        return "Dynamic, will constantly adjust the scroll velocity based on the vertical distance between document center and your gaze point.";
                    default:
                        return key.ToString();
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
