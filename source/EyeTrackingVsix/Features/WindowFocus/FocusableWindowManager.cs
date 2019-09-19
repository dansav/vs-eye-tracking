using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using EnvDTE;
using EyeTrackingVsix.Options;
using EyeTrackingVsix.Services;
using EyeTrackingVsix.Utils;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace EyeTrackingVsix
{
    public class FocusableWindowManager
    {
        private readonly object _openWindowsLock = new object();
        private readonly List<IVsWindowFrame> _openWindows;
        private readonly IVsUIShell _shell;

        public FocusableWindowManager(IVsUIShell shell, WindowEvents events, IEyetrackerService eyetracker, IKeyboardEventService keyboardEventService)
        {
            _shell = shell;
            _openWindows = new List<IVsWindowFrame>();

            events.WindowCreated += OnWindowCreated;
            events.WindowClosing += OnWindowClosing;
            events.WindowActivated += OnWindowActivated;
            events.WindowMoved += OnWindowMoved;

            var wpfWindow = Application.Current.MainWindow;
            var scaling = VisualTreeHelper.GetDpi(wpfWindow);

            var dpiX = scaling.DpiScaleX;
            var dpiY = scaling.DpiScaleY;

            keyboardEventService.ChangeFocus += () =>
            {
                if (!GeneralOptions.Instance.WindowFocusEnabled) return;

                ThreadHelper.ThrowIfNotOnUIThread();

                // note: there might be floating windows outside main window bounds
                var lookingAtApp = eyetracker.IsLookingAt(wpfWindow);
                Logger.Log($"FocusableWindowManager ChangeFocus: {(lookingAtApp ? "L" : "Not l")}ooking at VS");

                IVsWindowFrame[] openWindows;
                lock (_openWindowsLock)
                {
                    openWindows = _openWindows.ToArray();
                }

                foreach (var openWindow in openWindows)
                {
                    if (openWindow.IsVisible() == 0 && openWindow.IsOnScreen(out var onScreen) == 0 && onScreen != 0)
                    {
                        var winRect = CreateWindowRect(openWindow, dpiX, dpiY);
                        if (eyetracker.IsGazeInScreenRegion(winRect))
                        {
                            dynamic test = openWindow;

                            string title = test.Title;

                            Logger.Log($"You looked at {title} ({openWindow.GetType()})");
                            openWindow.Show();
                        }
                    }
                }
            };
        }

        private static Rect CreateWindowRect(IVsWindowFrame win, double dpiX, double dpiY)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (win is IVsWindowFrame4 win4)
            {
                win4.GetWindowScreenRect(out var x, out var y, out var w, out var h);
                return new Rect(x, y, w, h);
            }

            // fix inverted dpi (see https://developercommunity.visualstudio.com/content/problem/733252/wrong-scaling-of-bounds-of-window-objects-via-envd.html)
            //var x = win.Left / dpiX;
            //var y = win.Top / dpiY;
            //var w = win.Width / dpiX;
            //var h = win.Height / dpiY;

            return new Rect(0, 0, 0, 0);
        }

        private void OnWindowMoved(EnvDTE.Window window, int top, int left, int width, int height)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            Logger.Log($"Window was moved: {window.Caption} {window.Visible} top:{top} left:{left} width:{width} height:{height}");

            UpdateWindowList();
        }

        private void OnWindowActivated(EnvDTE.Window gotFocus, EnvDTE.Window lostFocus)
        {
#if DEBUG
            ThreadHelper.ThrowIfNotOnUIThread();
            Logger.Log($"Window got focus: {gotFocus?.Caption ?? "N/A"}, lost focus: {lostFocus?.Caption ?? "N/A"}");
#endif
        }

        private void OnWindowCreated(EnvDTE.Window window)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            Logger.Log($"Window was created: {window.Caption}");

            UpdateWindowList();
        }

        private void OnWindowClosing(EnvDTE.Window window)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            Logger.Log($"Window is closing: {window.Caption}");

            UpdateWindowList();
        }

        private void UpdateWindowList()
        {
            var documents = GetDocumentWindows();
            var tools = GetToolWindows();

            lock (_openWindowsLock)
            {
                _openWindows.Clear();
                _openWindows.AddRange(documents);
                _openWindows.AddRange(tools);
            }
        }

        private List<IVsWindowFrame> GetDocumentWindows()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var tmpList = new List<IVsWindowFrame>();

            if (_shell.GetDocumentWindowEnum(out var documentWindowEnum) == 0)
            {
                var frame = new IVsWindowFrame[1];
                while (documentWindowEnum.Next(1, frame, out var count) == 0 && frame[0] != null)
                {
                    tmpList.Add(frame[0]);
                }
            }

            return tmpList;
        }

        private List<IVsWindowFrame> GetToolWindows()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var tmpList = new List<IVsWindowFrame>();
            if (_shell.GetToolWindowEnum(out var toolWindowEnum) == 0)
            {
                var frame = new IVsWindowFrame[1];
                while (toolWindowEnum.Next(1, frame, out var count) == 0 && frame[0] != null)
                {
                    tmpList.Add(frame[0]);
                }
            }

            return tmpList;
        }
    }
}
