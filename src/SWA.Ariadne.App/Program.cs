using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SWA.Ariadne.Gui;
using SWA.Ariadne.Gui.Dialogs;
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
                        Application.Run(new ScreenSaverForm(true));
                        break;
                    default:
                        MessageBox.Show("Invalid command line argument: " + arg, "Invalid Command Line Argument", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            else
            {
                // If no arguments were passed in, run as a regular application.
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Form form = new MazeForm();
                form.Icon = Properties.Resources.AriadneIcon_32x32;
                Application.Run(form);
            }
        }
    }
}