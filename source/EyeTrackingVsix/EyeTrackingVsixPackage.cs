using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using EnvDTE;
using Eyetracking.NET;
using EyeTrackingVsix.Common;
using EyeTrackingVsix.Options;
using EyeTrackingVsix.Utils;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;
using Window = EnvDTE.Window;

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
    [Guid(PackageGuidString)]
    public sealed class EyeTrackingVsixPackage : AsyncPackage
    {
        /// <summary>
        /// EyeTrackingVsixPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "30e9100a-2ae9-4ce5-a1d3-d5d9ae4057e7";

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await Logger.InitializeAsync(this, "Eye Tracking for Visual Studio");

            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            var dte = (DTE)(await GetServiceAsync(typeof(DTE)));
            new FocusableWindowManager(dte.Windows, dte.Events.WindowEvents);
        }
    }

    public class FocusableWindowManager
    {
        private readonly List<Window> _openWindows;
        private readonly Eyetracker _eyetracker;
        private System.Windows.Window _wpfWindow;

        public FocusableWindowManager(Windows windows, WindowEvents events)
        {
            _openWindows = new List<Window>();
            foreach (Window window in windows)
            {
                _openWindows.Add(window);
            }
            events.WindowCreated += OnWindowCreated;
            events.WindowClosing += OnWindowClosing;
            events.WindowActivated += OnWindowActivated;
            events.WindowMoved += OnWindowMoved;

            Window main = windows.Parent.MainWindow;
            _wpfWindow = System.Windows.Application.Current.MainWindow;

            var dpiX = 1.25; //main.Width / _wpfWindow.ActualWidth;
            var dpiY = 1.25; //main.Height / _wpfWindow.ActualHeight;


            _wpfWindow.PreviewKeyUp += (sender, args) =>
            {
                if (args.Key == Key.LeftCtrl)
                {
                    var gazeScreenPoint = ScreenHelpers.GetGazePointInScreenPixels(_eyetracker);
                    Logger.Log($"Gaze point: {gazeScreenPoint}");

                    bool LookingAtApp = _eyetracker.IsLookingAt(main);
                    Logger.Log($"Gaze point: {(LookingAtApp ? "" : "not")} looking at VS");
                    if (!LookingAtApp)
                    {
                        return;
                    }

                    foreach (var openWindow in _openWindows)
                    {
                        if (openWindow != main && openWindow.Visible)
                        {
                            var winRect = CreateWindowRect(openWindow, dpiX, dpiY);
                            if (winRect.Contains(gazeScreenPoint))
                            {
                                Logger.Log($"You looked at {openWindow.Caption} ({_openWindows.IndexOf(openWindow) + 1})");
                                openWindow.SetFocus();
                            }
                        }
                    }
                }
            };

            _eyetracker = new Eyetracker();
        }

        private static Rect CreateWindowRect(Window win, double dpiX, double DpiY)
        {
            // fix inverted dpi
            var x = win.Left / dpiX;
            var y = win.Top / DpiY;
            var w = win.Width / dpiX;
            var h = win.Height / DpiY;

            return new Rect(x, y, w, h);
        }

        private void OnWindowMoved(Window window, int top, int left, int width, int height)
        {
            // it appears like this method is always called for new windows
            if (_openWindows.IndexOf(window) < 0)
            {
                _openWindows.Add(window);
            }

            Logger.Log($"Window was moved: {_openWindows.IndexOf(window) + 1} {window.Visible} top:{top} left:{left} width:{width} height:{height}");
        }

        private void OnWindowActivated(Window gotfocus, Window lostfocus)
        {
            Logger.Log($"Window got focus: {_openWindows.IndexOf(gotfocus)+1}, lost focus: {_openWindows.IndexOf(lostfocus)+1}");

            var kind = gotfocus.Kind;

            if (gotfocus.Object is OutputWindow ow)
            {
                var p = ow.ActivePane;
            }

        }

        private void OnWindowCreated(Window window)
        {
            if (_openWindows.IndexOf(window) < 0)
            {
                _openWindows.Add(window);
            }
            Logger.Log($"Window was created. Total active count: {_openWindows.Count}");
        }

        private void OnWindowClosing(Window window)
        {
            Logger.Log($"Window is closing: {_openWindows.IndexOf(window)+1}");
            _openWindows.Remove(window);
        }
    }

    public static class DteWindow
    {
        public static bool IsLookingAt(this IEyetracker eyetracker, EnvDTE.Window window)
        {
            var gazePixels = ScreenHelpers.GetGazePointInScreenPixels(eyetracker);

            var x = gazePixels.X - window.Left;
            var y = gazePixels.Y - window.Top;

            return x > 0 && x < window.Width && y > 0 && y < window.Height;
        }
    }
}
