using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace VpnGuardForGames
{
    static class VpnClient
    {
        const string ProcessName = "NordLayer";
        const string TrayIconWindowClassPrefix = "WPFTaskbarIcon_";
        const uint TrayCallbackMessage = 0x400;
        const int LeftButtonDown = 0x201;
        const int LeftButtonUp = 0x202;

        delegate bool EnumWindowsCallback(IntPtr window, IntPtr parameter);

        [DllImport("user32.dll")]
        static extern bool EnumWindows(EnumWindowsCallback callback, IntPtr parameter);

        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr window, out uint processId);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern int GetClassName(IntPtr window, StringBuilder className, int maxCount);

        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr window, uint message, IntPtr wParam, IntPtr lParam);

        public static bool IsRunning()
        {
            return Process.GetProcessesByName(ProcessName).Length > 0;
        }

        public static void Open()
        {
            IntPtr trayIcon = FindTrayIconWindow();
            if (trayIcon == IntPtr.Zero)
            {
                return;
            }

            PostMessage(trayIcon, TrayCallbackMessage, IntPtr.Zero, (IntPtr)LeftButtonDown);
            PostMessage(trayIcon, TrayCallbackMessage, IntPtr.Zero, (IntPtr)LeftButtonUp);
        }

        static IntPtr FindTrayIconWindow()
        {
            var processIds = new HashSet<uint>(Process.GetProcessesByName(ProcessName).Select(process => (uint)process.Id));
            IntPtr found = IntPtr.Zero;

            EnumWindows((window, parameter) =>
            {
                uint processId;
                GetWindowThreadProcessId(window, out processId);
                if (!processIds.Contains(processId))
                {
                    return true;
                }

                var className = new StringBuilder(256);
                GetClassName(window, className, className.Capacity);
                if (!className.ToString().StartsWith(TrayIconWindowClassPrefix))
                {
                    return true;
                }

                found = window;
                return false;
            }, IntPtr.Zero);

            return found;
        }
    }
}
