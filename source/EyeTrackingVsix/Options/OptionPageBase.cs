using Microsoft.VisualStudio.Shell;

namespace EyeTrackingVsix.Options
{
    internal abstract class OptionPageBase<T> : UIElementDialogPage where T : OptionModelBase<T>, new()
    {
        protected readonly T Model;

        protected OptionPageBase()
        {
#pragma warning disable VSTHRD104 // Offer async methods
            Model = ThreadHelper.JoinableTaskFactory.Run(OptionModelBase<T>.CreateAsync);
#pragma warning restore VSTHRD104 // Offer async methods
        }

        public override object AutomationObject => Model;

        public override void LoadSettingsFromStorage()
        {
            Model.Load();
        }

        public override void SaveSettingsToStorage()
        {
            Model.Save();
        }
    }
}
