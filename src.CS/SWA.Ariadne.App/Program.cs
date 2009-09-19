using System;
using System.Collections.Generic;
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
            if (args.Length > 0)
            {
                // Get the 2 character command line argument.
                string arg = args[0].ToLowerInvariant().Trim().Substring(0, 2);
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
                        // Create the ImageLoader as early as possible.
                        ImageLoader imageLoader = ImageLoader.GetScreenSaverImageLoader();
                        BlankSecondaryScreens();
                        Application.Run(new ScreenSaverForm(true, imageLoader));
                        break;
                    default:
                        MessageBox.Show("Invalid command line argument: " + arg, "Invalid Command Line Argument", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            else
            {
#if false
                ImageLoader imageLoader = ImageLoader.GetScreenSaverImageLoader();
                Application.Run(new ScreenSaverForm(true, imageLoader));
#endif
#if true
                // If no arguments were passed in, run as a regular application.
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Form form = new MazeForm();
                form.Icon = Properties.Resources.AriadneIcon_32x32;
                Application.Run(form);
#endif
#if false
                (new SWA.Ariadne.Gui.Tests.ContourImageTest()).CI_ManualTest_01();
#endif
            }
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