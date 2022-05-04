using System;
using System.Runtime.InteropServices;

namespace Erd_Tools.Utils
{
    internal static class User32
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        public static uint GetForegroundProcessID()
        {
            IntPtr hWnd = GetForegroundWindow();
            GetWindowThreadProcessId(hWnd, out uint pid);
            return pid;
        }
    }
}
