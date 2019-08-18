using Microsoft.VisualStudio.Shell;
using System.Windows.Forms;

namespace EyeTrackingVsix.Options
{
    internal abstract class OptionPageBase<T> : DialogPage where T : OptionModelBase<T>, new()
    {
        private readonly OptionModelBase<T> _model;

        public OptionPageBase()
        {
#pragma warning disable VSTHRD104 // Offer async methods
            _model = ThreadHelper.JoinableTaskFactory.Run(OptionModelBase<T>.CreateAsync);
#pragma warning restore VSTHRD104 // Offer async methods
        }

        public override object AutomationObject => _model;

        public override void LoadSettingsFromStorage()
        {
            _model.Load();
        }

        public override void SaveSettingsToStorage()
        {
            _model.Save();
        }
    }
}
