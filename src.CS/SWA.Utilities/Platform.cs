using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace SWA.Utilities
{
    /// <summary>
    /// Offers a rather uniform interface to platform specific data and methods.
    /// </summary>
    public static class Platform
    {
        /// <summary>
        /// A subset of the System.PlatformID values.
        /// </summary>
        public enum PlatformIdSubset
        {
            Windows = PlatformID.Win32Windows,
            Linux = PlatformID.Unix,
            Other = -1,
        }

        /// <summary>
        /// Gets a simplified evaluation of Environment.OSVersion.Platform.
        /// </summary>
        public static PlatformIdSubset PlatformId
        {
            get
            {
                switch (Environment.OSVersion.Platform)
                {
                    case PlatformID.Win32Windows:
                        return PlatformIdSubset.Windows;
                    case PlatformID.Win32NT:
                        return PlatformIdSubset.Windows;
                    case PlatformID.Win32S:
                        return PlatformIdSubset.Windows;
                    case PlatformID.WinCE:
                        return PlatformIdSubset.Windows;
                    case PlatformID.Unix:
                        return PlatformIdSubset.Linux;
                    default:
                        return PlatformIdSubset.Other;
                }
            }
        }

        public static bool IsWindows => (PlatformId == PlatformIdSubset.Windows);

        public static bool IsLinux => (PlatformId == PlatformIdSubset.Linux);

        /// <summary>
        /// Returns the size of a window, given its window handle.
        /// </summary>
        public static Rectangle GetClientRectangle(IntPtr windowHandle)
        {
            switch (PlatformId)
            {
                case PlatformIdSubset.Windows:
                    return Windows.GetClientRect(windowHandle);
                case PlatformIdSubset.Linux:
                    return Linux.GetClientRect(windowHandle);
                default:
                    return new Rectangle(0, 0, 300, 300);
            }
        }

        /// <summary>
        /// Provides access to some OS specific functions.
        /// These can only be used on a Windows operating system!
        /// </summary>
        public static class Windows
        {
            #region Data and functions from the USER32 library

            private struct RECT
            {
                public int left, top, right, bottom;
            }

            [DllImport("user32.dll")]
            private static extern bool GetClientRect(IntPtr windowHandle, ref RECT rect);

            [DllImport("user32.dll", EntryPoint = "IsWindowVisible")]
            public static extern bool IsWindowVisible(IntPtr windowHandle);

            #endregion

            /// <summary>
            /// Returns the size of a window, given its window handle.
            /// </summary>
            public static Rectangle GetClientRect(IntPtr windowHandle)
            {
                RECT rect = new RECT();

                if (GetClientRect(windowHandle, ref rect) == false)
                {
                    return new Rectangle(0, 0, 300, 300);
                }
                return new Rectangle(rect.left, rect.top,
                    rect.right - rect.left, rect.bottom - rect.top);
            }
        }

        /// <summary>
        /// Provides access to some OS specific functions.
        /// These can only be used on a Linux operating system!
        /// </summary>
        public static class Linux
        {
            #region Data and functions from the X11 library

            /// <summary>
            /// see: https://tronche.com/gui/x/xlib/window-information/XGetWindowAttributes.html
            /// </summary>
            private struct XWindowAttributes
            {
                public int x, y;               /* location of window */
                public int width, height;      /* width and height of window */
                public int border_width;       /* border width of window */
                public int depth;              /* depth of window */
                public IntPtr visual;          /* the associated visual structure */
                public IntPtr root;            /* root of screen containing window */
                public int win_class;          /* InputOutput, InputOnly*/
                public int bit_gravity;        /* one of the bit gravity values */
                public int win_gravity;        /* one of the window gravity values */
                public int backing_store;      /* NotUseful, WhenMapped, Always */
                public ulong backing_planes;   /* planes to be preserved if possible */
                public ulong backing_pixel;    /* value to be used when restoring planes */
                public int save_under;         /* boolean, should bits under be saved? */
                public IntPtr colormap;        /* color map to be associated with window */
                public int map_installed;      /* boolean, is color map currently installed*/
                public int map_state;          /* IsUnmapped, IsUnviewable, IsViewable */
                public long all_event_masks;   /* set of events all people have interest in*/
                public long your_event_mask;   /* my event mask */
                public long do_not_propagate_mask; /* set of events that should not propagate */
                public int override_redirect;  /* boolean value for override-redirect */
                public IntPtr screen;          /* back pointer to correct screen */
            }

            [DllImport("X11.so")]
            private static extern IntPtr XOpenDisplay(string displayStr);

            [DllImport("X11.so")]
            private static extern void XGetWindowAttributes(
                IntPtr display, IntPtr window, ref XWindowAttributes attributes);

            #endregion

            /// <summary>
            /// Returns the size of a window, given its window handle.
            /// </summary>
            public static Rectangle GetClientRect(IntPtr windowHandle)
            {
                XWindowAttributes xwa = new XWindowAttributes();
                string displayStr = Environment.GetEnvironmentVariable("DISPLAY");
                if (String.IsNullOrEmpty(displayStr)) displayStr = ":0";
                IntPtr display = XOpenDisplay(displayStr);
                XGetWindowAttributes(display, windowHandle, ref xwa);

                // We don't want to use the x and y coordinates -- these need to be 0.
                return new Rectangle(0, 0, xwa.width, xwa.height);
            }
        }
    }
}
