using System;
using System.Windows.Forms;
using SWA.Ariadne.Gui;
using SWA.Ariadne.Gui.Dialogs;
using SWA.Ariadne.Gui.Mazes;
using SWA.Ariadne.Ctrl;

namespace SWA.Ariadne.App
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // Don't scan the image directory all too often.
            // In true screensaver mode, this will be set to -1 (infinity).
            SWA.Utilities.Directory.ResultValidForSeconds = 30;

            #region Check if we have been started by the Linux xscreensaver(1)
            // The protocol is like this:
            // * A fullscreen screensaver is called without parameters
            // * A preview screensaver is called with "-window-id 0xXXXX" arguments
            //   see: xscreensaver-5.44/driver/demo-Gtk.c : launch_preview_subproc()

            string windowHandleStr = Environment.GetEnvironmentVariable("XSCREENSAVER_WINDOW");
            if (!String.IsNullOrEmpty(windowHandleStr))
            {
                // Convert "0xXXXX" (hex) to "NNNN" (decimal)
                windowHandleStr = UInt32.Parse(windowHandleStr.Substring(2),
                    System.Globalization.NumberStyles.AllowHexSpecifier).ToString();

                // https://github.com/mono/mono/blob/master/mcs/class/System.Windows.Forms/System.Windows.Forms/XplatUIX11.cs
                // When the MONO_XEXCEPTIONS environment variable is set,
                // an Exception will be thrown when a X11 error is encountered;
                // by default a message is displayed but execution continues.
                //
                // We prefer the Exception. Otherwise the app would continue running
                // even when the connection to the XSCREENSAVER_WINDOW has been lost.
                //
                Environment.SetEnvironmentVariable("MONO_XEXCEPTIONS", "1");

                if (args.Length > 0 && args[0] == "-window-id")
                {
                    ScreenSaverPreviewController.Run(windowHandleStr);
                }
                else
                {
                    // Create the ImageLoader as early as possible.
                    SWA.Utilities.Directory.ResultValidForSeconds = -1;
                    ImageLoader imageLoader = ImageLoader.GetScreenSaverImageLoader(Screen.PrimaryScreen.Bounds);
                    BlankSecondaryScreens(); // TODO: consider Linux behavior...
                    ScreenSaverController.Run(windowHandleStr, imageLoader);
                }
            }
            #endregion
            else
            #region Evaluate the command line options
            if (args.Length > 0)
            {
                // Get the 2 character command line argument.
                string arg = args[0].ToLowerInvariant().Trim().Substring(0, 2);

                // Convert Linux style argument to Windows style.
                if (arg == "-o") { arg = "/c"; } // options
                if (arg == "-f") { arg = "/s"; } // fullscreen

                switch (arg)
                {
                    case "/c":
                        // Show the options dialog
                        Application.Run(new OptionsDialog());
                        break;
                    case "/p":
                        // Show a preview window within the Display settings panel.
                        ScreenSaverPreviewController.Run(args[1]);
                        break;
                    case "/s":
                        // Show screensaver form
                        SWA.Utilities.Display.EnableDpiAwareness();
                        SWA.Utilities.Directory.ResultValidForSeconds = -1;
                        // Create the ImageLoader as early as possible.
                        ImageLoader imageLoader = ImageLoader.GetScreenSaverImageLoader(Screen.PrimaryScreen.Bounds);
                        BlankSecondaryScreens();
                        Application.Run(new ScreenSaverForm(true, imageLoader));
                        break;
                    default:
                        MessageBox.Show("Invalid command line argument: " + args[0],
                            "Invalid Command Line Argument",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            #endregion
            else
            #region Run as a regular, standalone application
            {
                SWA.Utilities.Display.EnableDpiAwareness();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Form form = new MazeForm();
                form.Icon = Properties.Resources.AriadneIcon_32x32;
                Application.Run(form);
            }
            #endregion
        }

        /// <summary>
        /// Displays a blank black window on all screens but the primary screen.
        /// see: http://stackoverflow.com/questions/1363374/showing-a-windows-form-on-a-secondary-monitor
        /// </summary>
        private static void BlankSecondaryScreens()
        {
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.Primary == false)
                {
                    System.Drawing.Rectangle bounds = screen.Bounds;

                    Form form = new BlankForm();
                    form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                    form.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                    form.TopMost = true;
                    form.SetBounds(-400, -100, 10, 10);
                    form.Show();
                    form.Enabled = false;
                    form.SetBounds(bounds.X, bounds.Y, bounds.Width, bounds.Height);
                    form.WindowState = FormWindowState.Maximized;
                }
            }
        }
    }
}
