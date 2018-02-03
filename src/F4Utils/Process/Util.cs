using System;
using System.Linq;
using System.Management;
using System.Text;
using Common.Win32;

namespace F4Utils.Process
{
    public static class Util
    {
        private const string WINDOW_CLASS_FALCONDISPLAY = "FalconDisplay";

        public static void ActivateFalconWindow()
        {
            var windowHandle = GetFalconWindowHandle();
            if (windowHandle != IntPtr.Zero)
            {
                NativeMethods.SetForegroundWindow(windowHandle);
            }
        }


        public static string GetFalconWindowTitle()
        {
            var txt = new StringBuilder(NativeMethods.MAX_PATH);
            var windowHandle = GetFalconWindowHandle();
            string toReturn = null;
            if (windowHandle != IntPtr.Zero)
            {
                NativeMethods.GetWindowText(windowHandle, txt, NativeMethods.MAX_PATH);
                toReturn = txt.ToString();
            }
            return toReturn;
        }

        private static DateTime _lastFalconRunningCheck = DateTime.MinValue;
        private static bool _falconWasRunningOnLastCheck;
        public static bool IsFalconRunning()
        {
            if (!(DateTime.UtcNow.Subtract(_lastFalconRunningCheck).TotalMilliseconds > 500))
                return _falconWasRunningOnLastCheck;
            var windowHandle = GetFalconWindowHandle();
            _falconWasRunningOnLastCheck = (windowHandle != IntPtr.Zero);
            _lastFalconRunningCheck = DateTime.UtcNow;
            return _falconWasRunningOnLastCheck;
        }

        public static string GetFalconExePath()
        {
            var windowHandle = GetFalconWindowHandle();
            if (windowHandle == IntPtr.Zero) return null;
            int procId;
            NativeMethods.GetWindowThreadProcessId(windowHandle, out procId);
            return ExePath(procId);
        }
        private static int? _lastFalconExeProcessId;
        private static string _lastFalconExePath;
        private static string ExePath(int processId)
        {
            if (_lastFalconExeProcessId.HasValue && _lastFalconExeProcessId == processId && _lastFalconExePath !=null )
            {
                return _lastFalconExePath;
            }
            var wmiQueryString =
                $"SELECT ProcessId, ExecutablePath, CommandLine FROM Win32_Process WHERE ProcessId={processId}";
            using (var searcher = new ManagementObjectSearcher(wmiQueryString))
            using (var results = searcher.Get())
            {
                var item = (from p in System.Diagnostics.Process.GetProcesses()
                            join mo in results.Cast<ManagementObject>()
                            on p.Id equals (int)(uint)mo["ProcessId"]
                            select new
                            {
                                Process = p,
                                Path = (string)mo["ExecutablePath"],
                                CommandLine = (string)mo["CommandLine"],
                            }).FirstOrDefault();
                var exePath= item != null ? item.Path : null;
                _lastFalconExeProcessId = processId;
                _lastFalconExePath = exePath;
                return exePath;
            }
        }

        public static IntPtr GetFalconWindowHandle()
        {
            var hWnd = NativeMethods.FindWindow(WINDOW_CLASS_FALCONDISPLAY, null);
            return hWnd;
        }

    }
}