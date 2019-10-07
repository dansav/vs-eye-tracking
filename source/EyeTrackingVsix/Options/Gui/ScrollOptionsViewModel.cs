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

        public int Velocity
        {
            get => _model.ScrollVelocity;
            set
            {
                _model.ScrollVelocity = value;
                OnPropertyChanged();
            }
        }

        public double LinearAccelerationTime
        {
            get => _model.ScrollLinearAccelerationTime;
            set
            {
                _model.ScrollLinearAccelerationTime = value;
                OnPropertyChanged();
            }
        }

        public double ExponentialAcceleration
        {
            get => _model.ScrollExponentialAccelerationPower;
            set
            {
                _model.ScrollExponentialAccelerationPower = value;
                OnPropertyChanged();
            }
        }

        public double ExponentialInertia
        {
            get => _model.ScrollExponentialInertia;
            set
            {
                _model.ScrollExponentialInertia = value;
                OnPropertyChanged();
            }
        }
    }
}
