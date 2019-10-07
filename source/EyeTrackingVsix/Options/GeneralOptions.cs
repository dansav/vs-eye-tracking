using System;
using System.ComponentModel;
using System.Threading.Tasks;
using EyeTrackingVsix.Common.Keyboard;
using EyeTrackingVsix.Features.Scroll;

namespace EyeTrackingVsix.Options
{
    internal class GeneralOptions : OptionModelBase<GeneralOptions>
    {
        [Category("Keyboard")]
        [DisplayName("Key tap release time (ms)")]
        [Description("The max time a key can be pressed during a double tap")]
        [DefaultValue(150)]
        public int KeyTapReleaseTimeMs { get; set; } = 150;

        [Category("Keyboard")]
        [DisplayName("Double tap interval time (ms)")]
        [Description("The max time between last key release and next key press during a double tap")]
        [DefaultValue(250)]
        public int DoubleTapIntervalTimeMs { get; set; } = 250;

        [Category("Keyboard")]
        [DisplayName("Key hold time (ms)")]
        [Description("The min time a key must be pressed")]
        [DefaultValue(550)]
        public int KeyTapHoldTimeMs { get; set; } = 550;

        // --------------------------------------------------------------------

        [Category("Caret")]
        [DisplayName("Enabled")]
        [Description("Enable or disable the caret feature")]
        [DefaultValue(true)]
        public bool CaretEnabled { get; set; } = true;

        [Category("Caret")]
        [DisplayName("Keyboard key")]
        [Description("Select the key on the keyboard that will be used to trigger the caret jump.")]
        [DefaultValue(InteractionKey.RightCtrl)]
        public InteractionKey CaretKey { get; set; } = InteractionKey.RightCtrl;

        // --------------------------------------------------------------------

        [Category("Window Focus")]
        [DisplayName("Enabled")]
        [Description("Enable or disable the window focus feature")]
        [DefaultValue(true)]
        public bool WindowFocusEnabled { get; set; } = true;

        // --------------------------------------------------------------------

        [Category("Scroll")]
        [DisplayName("Enabled")]
        [Description("Enable or disable the scroll feature")]
        [DefaultValue(true)]
        public bool ScrollEnabled { get; set; } = true;

        [Category("Scroll")]
        [DisplayName("Keyboard key")]
        [Description("Select the key on the keyboard that will be used to trigger a scroll.")]
        [DefaultValue(InteractionKey.RightCtrl)]
        public InteractionKey ScrollKey { get; set; } = InteractionKey.RightCtrl;

        [Category("Scroll")]
        [DisplayName("Scroll type (experimental)")]
        [Description("Select the type of scroll you prefer. NOTE: Changes to this value only applies to documents opened after the value has changed.")]
        [DefaultValue(ScrollType.Exponential)]
        public ScrollType ScrollType { get; set; } = ScrollType.Exponential;
        
        [Category("Scroll")]
        [DisplayName("Scroll velocity (pixels per second)")]
        [Description("The base velocity. This is how fast the document will scroll on the screen.")]
        [DefaultValue(400)]
        public int ScrollVelocity { get; set; } = 800;

        [Category("Scroll")]
        [DisplayName("Scroll velocity: Linear acceleration time (seconds)")]
        [Description("The time it takes the linear scroll to reach its max velocity (only applicable if Linear scroll type is selected)")]
        [DefaultValue(3.0)]
        public double ScrollLinearAccelerationTime { get; set; } = 1.5;

        [Category("Scroll")]
        [DisplayName("Scroll velocity: Exponential acceleration power (experimental)")]
        [Description("A higher number accelerates to max speed faster. Reccomended values: between 0.001 and 0.05. (only applicable if Exponential scroll type is selected)")]
        [DefaultValue(0.005)]
        public double ScrollExponentialAccelerationPower { get; set; } = 0.005;

        [Category("Scroll")]
        [DisplayName("Scroll velocity: Exponential scroll inertia (experimental)")]
        [Description("A higher number breaks to no scroll faster. Reccomended values: between 0.001 and 0.05. (only applicable if Exponential scroll type is selected)")]
        [DefaultValue(0.025)]
        public double ScrollExponentialInertia { get; set; } = 0.025;

        // --------------------------------------------------------------------

        public event Action GeneralOptionsChanged;

        public override Task SaveAsync()
        {
            GeneralOptionsChanged?.Invoke();
            return base.SaveAsync();
        }
    }
}
