using System.Windows;
using System.Windows.Controls;
using EyeTrackingVsix.Features.Scroll;

namespace EyeTrackingVsix.Options.Gui
{
    public class ScrollTypeTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Static { get; set; }
        public DataTemplate Linear { get; set; }
        public DataTemplate Exponential { get; set; }
        public DataTemplate Dynamic { get; set; }
        public DataTemplate Default { get; } = new DataTemplate();
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ScrollType type)
            {
                switch (type)
                {
                    //case ScrollType.Static:
                    //    return Static;
                    case ScrollType.Linear:
                        return Linear;
                    case ScrollType.Exponential:
                        return Exponential;
                    case ScrollType.Dynamic:
                        return Dynamic;
                }
            }

            return Default;
        }
    }
}
