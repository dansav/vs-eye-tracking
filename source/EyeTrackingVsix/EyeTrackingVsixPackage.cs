using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using EnvDTE;
using EyeTrackingVsix.Common;
using EyeTrackingVsix.Options;
using EyeTrackingVsix.Services;
using EyeTrackingVsix.Utils;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace EyeTrackingVsix
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.ShellInitialized_string, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideOptionPage(typeof(DialogPageProvider.General), "Eye tracking", "General", 0, 0, true)]
    [ProvideService(typeof(SEyetrackerService), IsAsyncQueryable = true)]
    [ProvideService(typeof(SKeyboardEventService), IsAsyncQueryable = true)]
    [Guid(PackageGuidString)]
    public sealed class EyeTrackingVsixPackage : AsyncPackage
    {
        /// <summary>
        /// EyeTrackingVsixPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "30e9100a-2ae9-4ce5-a1d3-d5d9ae4057e7";

        private static EyeTrackingVsixPackage _instance;

        private KeyboardEventService _keyboardEventService;
        private EyetrackerService _eyetrackerService;

        public static object GetMyService(Type t)
        {
            return _instance.GetService(t);
        }

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            _instance = this;

            AddService(typeof(SEyetrackerService), CreateServiceAsync, true);
            AddService(typeof(SKeyboardEventService), CreateServiceAsync, true);

            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            await Logger.InitializeAsync(this, "Eye Tracking for Visual Studio");

            if (await GetServiceAsync(typeof(DTE)) is DTE dte)
            {
                _eyetrackerService = new EyetrackerService(this);
                _keyboardEventService = new KeyboardEventService(this, System.Windows.Application.Current.MainWindow, GeneralOptions.Instance);

                new FocusableWindowManager(dte.Windows, dte.Events.WindowEvents, _eyetrackerService, _keyboardEventService);
            }
        }

        private Task<object> CreateServiceAsync(IAsyncServiceContainer asyncServiceContainer, CancellationToken cancellationToken, Type serviceType)
        {
            if (serviceType == typeof(SEyetrackerService))
            {
                return Task.FromResult((object)_eyetrackerService);
            }

            if (serviceType == typeof(SKeyboardEventService))
            {
                return Task.FromResult((object)_keyboardEventService);
            }

            // ???
            return Task.FromResult((object)null);
        }
    }
}
