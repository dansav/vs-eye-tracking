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

        public int KeyTapReleaseTimeMs
        {
            get => _model.KeyTapReleaseTimeMs;
            set
            {
                _model.KeyTapReleaseTimeMs = value;
                OnPropertyChanged();
            }
        }

        public int DoubleTapIntervalTimeMs
        {
            get => _model.DoubleTapIntervalTimeMs;
            set
            {
                _model.DoubleTapIntervalTimeMs = value;
                OnPropertyChanged();
            }
        }

        public int KeyTapHoldTimeMs
        {
            get => _model.KeyTapHoldTimeMs;
            set
            {
                _model.KeyTapHoldTimeMs = value;
                OnPropertyChanged();
            }
        }
    }
}
