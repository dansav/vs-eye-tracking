using System.Windows;
using EyeTrackingVsix.Options.Gui;
using EyeTrackingVsix.Options.Gui.ViewModels;
using GeneralOptionsView = EyeTrackingVsix.Options.Gui.Views.GeneralOptionsView;

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
