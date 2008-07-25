using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Win32;
using SWA.Ariadne.Gui;

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
                        // Show screensaver form in preview mode
                        // Catch any exceptions when the preview panel is closed.
                        try
                        {
                            Application.Run(new ScreenSaverForm(args[1]));
                        }
                        catch(Exception) { }
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
#if false
                ScreenSaverForm form = new ScreenSaverForm(false);
                Application.Run(form);
#else
                // If no arguments were passed in, run as a regular application.
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Form form = new MazeForm();
                form.Icon = Properties.Resources.AriadneIcon_32x32;
                Application.Run(form);
#endif
            }
        }
    }
}