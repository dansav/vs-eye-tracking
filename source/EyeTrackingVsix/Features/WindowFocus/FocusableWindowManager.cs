using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using EnvDTE;
using EnvDTE80;
using EyeTrackingVsix.Services;
using EyeTrackingVsix.Utils;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Window = EnvDTE.Window;

namespace EyeTrackingVsix
{
    public class FocusableWindowManager
    {
        private readonly object _openWindowsLock = new object();
        private readonly List<IVsWindowFrame> _openWindows;
        private IVsUIShell _shell;

        public FocusableWindowManager(IVsUIShell shell, WindowEvents events, IEyetrackerService eyetracker, IKeyboardEventService keyboardEventService)
        {
            _shell = shell;
            _openWindows = new List<IVsWindowFrame>();

            events.WindowCreated += OnWindowCreated;
            events.WindowClosing += OnWindowClosing;
            events.WindowActivated += OnWindowActivated;
            events.WindowMoved += OnWindowMoved;

            var wpfWindow = System.Windows.Application.Current.MainWindow;
            var scaling = VisualTreeHelper.GetDpi(wpfWindow);

            var dpiX = scaling.DpiScaleX;
            var dpiY = scaling.DpiScaleY;

            keyboardEventService.ChangeFocus += () =>
            {
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
                        var winRect = CreateWindowRect(openWindow as IVsWindowFrame4, dpiX, dpiY);
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

        private static Rect CreateWindowRect(IVsWindowFrame4 win, double dpiX, double DpiY)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            win.GetWindowScreenRect(out var x, out var y, out var w, out var h);

            // fix inverted dpi
            //var x = win.Left / dpiX;
            //var y = win.Top / DpiY;
            //var w = win.Width / dpiX;
            //var h = win.Height / DpiY;

            return new Rect(x, y, w, h);
        }

        private void OnWindowMoved(Window window, int top, int left, int width, int height)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            Logger.Log($"Window was moved: {window.Caption} {window.Visible} top:{top} left:{left} width:{width} height:{height}");

            UpdateWindowList();
        }

        private void OnWindowActivated(Window gotfocus, Window lostfocus)
        {
#if DEBUG
            ThreadHelper.ThrowIfNotOnUIThread();
            Logger.Log($"Window got focus: {gotfocus?.Caption ?? "N/A"}, lost focus: {lostfocus?.Caption ?? "N/A"}");
#endif
        }

        private void OnWindowCreated(Window window)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            Logger.Log($"Window was created: {window.Caption}");

            UpdateWindowList();
        }

        private void OnWindowClosing(Window window)
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
            var tmpList = new List<IVsWindowFrame>();

            if (_shell.GetDocumentWindowEnum(out var ppenum) == 0)
            {
                var frame = new IVsWindowFrame[1];
                while (ppenum.Next(1, frame, out var count) == 0 && frame[0] != null)
                {
                    tmpList.Add(frame[0]);
                }
            }

            return tmpList;
        }

        private List<IVsWindowFrame> GetToolWindows()
        {
            var tmpList = new List<IVsWindowFrame>();
            if (_shell.GetToolWindowEnum(out var ppenum) == 0)
            {
                var frame = new IVsWindowFrame[1];
                while (ppenum.Next(1, frame, out var count) == 0 && frame[0] != null)
                {
                    tmpList.Add(frame[0]);
                }
            }

            return tmpList;
        }
    }
}
