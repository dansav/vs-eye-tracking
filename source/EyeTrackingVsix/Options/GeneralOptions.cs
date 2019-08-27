using System.ComponentModel;
using EyeTrackingVsix.Common.Keyboard;

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
        [DisplayName("Scroll velocity (pixels per second)")]
        [Description("Select the key on the keyboard that will be used to trigger a scroll.")]
        [DefaultValue(400)]
        public int ScrollVelocity { get; set; } = 400;
    }
}
