using System.Windows;
using EyeTrackingVsix.Options.Gui;

namespace EyeTrackingVsix.Options
{
    internal static class DialogPageProvider
    {
        public class General : OptionPageBase<GeneralOptions>
        {
            public General()
            {
                Child = new GeneralOptionsView(new GeneralOptionsViewModel(Model));
            }

            protected override UIElement Child { get; }
        }
    }
}
