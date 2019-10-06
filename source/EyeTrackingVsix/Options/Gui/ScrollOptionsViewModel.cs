using System;
using System.Collections.Generic;
using System.Linq;
using EyeTrackingVsix.Common.Keyboard;
using EyeTrackingVsix.Features.Scroll;

namespace EyeTrackingVsix.Options.Gui
{
    public class ScrollOptionsViewModel : PropertyChangedBase
    {
        private readonly GeneralOptions _model;

        internal ScrollOptionsViewModel(GeneralOptions model)
        {
            _model = model;
            AvailableKeys = Enum
                .GetValues(typeof(InteractionKey))
                .Cast<InteractionKey>()
                .ToArray();

            AvailableScrollProfiles = Enum
                .GetValues(typeof(ScrollType))
                .Cast<ScrollType>()
                .ToArray();
        }

        public bool Enabled
        {
            get => _model.ScrollEnabled;
            set
            {
                _model.ScrollEnabled = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<InteractionKey> AvailableKeys { get; }

        public InteractionKey SelectedKey
        {
            get => _model.ScrollKey;
            set => _model.ScrollKey = value;
        }

        public IEnumerable<ScrollType> AvailableScrollProfiles { get; }

        public ScrollType SelectedProfile
        {
            get => _model.ScrollType;
            set
            {
                _model.ScrollType = value;
                OnPropertyChanged();
            }
        }
    }
}
