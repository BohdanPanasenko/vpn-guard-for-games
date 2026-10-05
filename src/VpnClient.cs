using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace VpnGuardForGames
{
    static class VpnClient
    {
        const string TrayIconWindowClassPrefix = "WPFTaskbarIcon_";
        const uint TrayCallbackMessage = 0x400;
        const int LeftButtonDown = 0x201;
        const int LeftButtonUp = 0x202;
        const int BringToFrontDelayMs = 300;

        delegate bool EnumWindowsCallback(IntPtr window, IntPtr parameter);

        [DllImport("user32.dll")]
        static extern bool EnumWindows(EnumWindowsCallback callback, IntPtr parameter);

        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr window, out uint processId);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern int GetClassName(IntPtr window, StringBuilder className, int maxCount);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern int GetWindowText(IntPtr window, StringBuilder text, int maxCount);

        [DllImport("user32.dll")]
        static extern bool IsWindowVisible(IntPtr window);

        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr window, uint message, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern bool AllowSetForegroundWindow(uint processId);

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr window);

        public static bool IsRunning(string client)
        {
            return !string.IsNullOrEmpty(client) && Process.GetProcessesByName(client).Length > 0;
        }

        public static void Open(string client)
        {
            IntPtr trayIcon = FindClientWindow(client, window => GetClassName(window).StartsWith(TrayIconWindowClassPrefix));
            if (trayIcon == IntPtr.Zero)
            {
                return;
            }

            uint processId;
            GetWindowThreadProcessId(trayIcon, out processId);
            AllowSetForegroundWindow(processId);

            PostMessage(trayIcon, TrayCallbackMessage, IntPtr.Zero, (IntPtr)LeftButtonDown);
            PostMessage(trayIcon, TrayCallbackMessage, IntPtr.Zero, (IntPtr)LeftButtonUp);

            var bringToFront = new Timer { Interval = BringToFrontDelayMs };
            bringToFront.Tick += delegate
            {
                bringToFront.Dispose();
                IntPtr mainWindow = FindClientWindow(client, window => IsWindowVisible(window) && GetWindowText(window) == client);
                if (mainWindow != IntPtr.Zero)
                {
                    SetForegroundWindow(mainWindow);
                }
            };
            bringToFront.Start();
        }

        static IntPtr FindClientWindow(string client, Func<IntPtr, bool> matches)
        {
            var processIds = new HashSet<uint>(Process.GetProcessesByName(client).Select(process => (uint)process.Id));
            IntPtr found = IntPtr.Zero;

            EnumWindows((window, parameter) =>
            {
                uint processId;
                GetWindowThreadProcessId(window, out processId);
                if (!processIds.Contains(processId) || !matches(window))
                {
                    return true;
                }

                found = window;
                return false;
            }, IntPtr.Zero);

            return found;
        }

        static string GetClassName(IntPtr window)
        {
            var className = new StringBuilder(256);
            GetClassName(window, className, className.Capacity);
            return className.ToString();
        }

        static string GetWindowText(IntPtr window)
        {
            var text = new StringBuilder(256);
            GetWindowText(window, text, text.Capacity);
            return text.ToString();
        }
    }
}
