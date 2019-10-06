using System;
using System.Collections.Generic;
using System.Linq;
using EyeTrackingVsix.Common.Keyboard;

namespace EyeTrackingVsix.Options.Gui
{
    public class CaretOptionsViewMdoel : PropertyChangedBase
    {
        private readonly GeneralOptions _model;

        internal CaretOptionsViewMdoel(GeneralOptions model)
        {
            _model = model;
            AvailableKeys = Enum
                .GetValues(typeof(InteractionKey))
                .Cast<InteractionKey>()
                .ToArray();
        }

        public bool Enabled
        {
            get => _model.CaretEnabled;
            set
            {
                _model.CaretEnabled = value;
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
