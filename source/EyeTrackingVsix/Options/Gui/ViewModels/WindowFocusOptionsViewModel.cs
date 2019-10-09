using System;
using System.Collections.Generic;
using System.Linq;
using EyeTrackingVsix.Common.Keyboard;

namespace EyeTrackingVsix.Options.Gui.ViewModels
{
    public class WindowFocusOptionsViewModel : PropertyChangedBase
    {
        private readonly GeneralOptions _model;

        internal WindowFocusOptionsViewModel(GeneralOptions model)
        {
            _model = model;
            AvailableKeys = Enum
                .GetValues(typeof(InteractionKey))
                .Cast<InteractionKey>()
                .ToArray();
        }

        public bool Enabled
        {
            get => _model.WindowFocusEnabled;
            set
            {
                _model.WindowFocusEnabled = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<InteractionKey> AvailableKeys { get; }

        public InteractionKey SelectedKey
        {
            get => _model.CaretKey;
            set => _model.CaretKey = value;
        }
    }
}
