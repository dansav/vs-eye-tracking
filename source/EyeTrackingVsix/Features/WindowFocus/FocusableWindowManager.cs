using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using EnvDTE;
using EyeTrackingVsix.Services;
using EyeTrackingVsix.Utils;
using Microsoft.VisualStudio.Shell;
using Window = EnvDTE.Window;

namespace EyeTrackingVsix
{
    public class FocusableWindowManager
    {
        private readonly List<Window> _openWindows;

        public FocusableWindowManager(Windows windows, WindowEvents events, IEyetrackerService eyetracker, IKeyboardEventService keyboardEventService)
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

            var wpfWindow = System.Windows.Application.Current.MainWindow;
            var scaling = VisualTreeHelper.GetDpi(wpfWindow);

            var dpiX = scaling.DpiScaleX;
            var dpiY = scaling.DpiScaleY;

            // todo: use the keyboard event aggregator
            keyboardEventService.ChangeFocus += () =>
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                // note: there might be floating windows outside main window bounds
                var lookingAtApp = eyetracker.IsLookingAt(wpfWindow);
                Logger.Log($"Gaze point: {(lookingAtApp ? "" : "not")} looking at VS");

                var main = windows.Parent.MainWindow;

                foreach (var openWindow in _openWindows)
                {
                    if (openWindow != main && openWindow.Visible)
                    {
                        var winRect = CreateWindowRect(openWindow, dpiX, dpiY);
                        if (eyetracker.IsGazeInScreenRegion(winRect))
                        {
                            Logger.Log($"You looked at {openWindow.Caption} ({_openWindows.IndexOf(openWindow) + 1})");
                            openWindow.SetFocus();
                        }
                    }
                }
            };
        }

        private static Rect CreateWindowRect(Window win, double dpiX, double DpiY)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

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

            ThreadHelper.ThrowIfNotOnUIThread();

            Logger.Log($"Window was moved: {_openWindows.IndexOf(window) + 1} {window.Visible} top:{top} left:{left} width:{width} height:{height}");
        }

        private void OnWindowActivated(Window gotfocus, Window lostfocus)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            Logger.Log($"Window got focus: {_openWindows.IndexOf(gotfocus) + 1}, lost focus: {_openWindows.IndexOf(lostfocus) + 1}");

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
            ThreadHelper.ThrowIfNotOnUIThread();

            Logger.Log($"Window was created. Total active count: {_openWindows.Count}");
        }

        private void OnWindowClosing(Window window)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            Logger.Log($"Window is closing: {_openWindows.IndexOf(window) + 1}");
            _openWindows.Remove(window);
        }
    }
}
