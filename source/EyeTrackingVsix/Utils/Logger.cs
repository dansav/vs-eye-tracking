using System;
using System.Diagnostics;
using Microsoft;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace EyeTrackingVsix.Utils
{
    // based on https://github.com/madskristensen/ExtensibilityTools/blob/master/src/Shared/Helpers/Logger.cs
    internal static class Logger
    {
        private static string _name;
        private static IVsOutputWindowPane _pane;
        private static IVsOutputWindow _output;

        public static async Task InitializeAsync(AsyncPackage provider, string name)
        {
            _output = await provider.GetServiceAsync(typeof(SVsOutputWindow)) as IVsOutputWindow;
            Assumes.Present(_output);
            _name = name;
        }

        public static void Log(object message)
        {
            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                if (_pane == null)
                {
                    var guid = Guid.NewGuid();
                    _output.CreatePane(ref guid, _name, 1, 1);
                    _output.GetPane(ref guid, out _pane);
                }

                _pane?.OutputString($"{DateTime.Now}: {message}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
            }
        }
    }
}
