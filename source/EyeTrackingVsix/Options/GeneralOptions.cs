using System.ComponentModel;

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

        // --------------------------------------------------------------------

        [Category("Scroll")]
        [DisplayName("Enabled")]
        [Description("Enable or disable the scroll feature")]
        [DefaultValue(true)]
        public bool ScrollEnabled { get; set; } = true;
    }
}
