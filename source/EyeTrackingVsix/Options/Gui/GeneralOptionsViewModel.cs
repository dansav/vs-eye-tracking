namespace EyeTrackingVsix.Options.Gui
{
    public class GeneralOptionsViewModel : PropertyChangedBase
    {
        private readonly GeneralOptions _model;

        internal GeneralOptionsViewModel(GeneralOptions model)
        {
            _model = model;
            Caret = new CaretOptionsViewMdoel(model);
            Scroll = new ScrollOptionsViewModel(model);
            WindowFocus = new WindowFocusOptionsViewModel(model);
        }

        public CaretOptionsViewMdoel Caret { get; }
        public ScrollOptionsViewModel Scroll { get; }
        public WindowFocusOptionsViewModel WindowFocus { get; }
    }
}
