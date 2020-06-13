using System;
using System.Runtime.InteropServices;

namespace SWA.Utilities
{
    public static class Display
    {
        [DllImport("SHCore.dll", SetLastError = true)]
        private static extern bool SetProcessDpiAwareness(PROCESS_DPI_AWARENESS awareness);

        [DllImport("SHCore.dll", SetLastError = true)]
        private static extern void GetProcessDpiAwareness(IntPtr hprocess, out PROCESS_DPI_AWARENESS awareness);

        public enum PROCESS_DPI_AWARENESS
        {
            Process_DPI_Unaware = 0,
            Process_System_DPI_Aware = 1,
            Process_Per_Monitor_DPI_Aware = 2
        }

        /// <summary>
        /// Enables the application's DPI awareness.
        /// Must be called very early, before creating any GUI objects.
        /// </summary>
        /// <remarks>
        /// https://stackoverflow.com/questions/32148151/setprocessdpiawareness-not-having-effect
        /// I'm using this approach because an application manifest, as often recommended,
        /// did not work for me.
        /// </remarks>
        public static void EnableDpiAwareness()
        {
            // Don't try this unless we're on a MS Windows system.
            if (Environment.NewLine.Length < 2) { return; }

            try
            {
                SetProcessDpiAwareness(PROCESS_DPI_AWARENESS.Process_System_DPI_Aware);
            }
            catch (Exception ex)
            {
                Log.WriteLine("Cannot use DpiAwareness API. " + ex.Message, true);
            }
        }
    }
}
