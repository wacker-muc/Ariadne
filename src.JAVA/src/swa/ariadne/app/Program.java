package swa.ariadne.app;

import java.awt.Frame;

import swa.ariadne.gui.MazeFrame;

/**
 * Short introduction sentence. More description afterwards.
 * 
 * @author Stephan.Wacker@web.de
 */
public class Program {

    /**
     * @param args
     */
    public static void main(String[] args)
    {
        if (args.length > 0 && args[0].charAt(0) == '/')
        {
            // Get the 2 character command line argument.
            char arg = args[0].toLowerCase().trim().charAt(1);
            switch (arg)
            {
                /* TODO
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
                    Application.Run(new ScreenSaverForm(true, imageLoader));
                    break;
                 */
                default:
                    throw new RuntimeException("Invalid command line argument: " + arg);
                    // TODO: MessageBox.Show("Invalid command line argument: " + arg, "Invalid Command Line Argument", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // break;
            }
        }
        else
        {
            // If no arguments were passed in, run as a regular application.
            Frame form = new MazeFrame();
            // TODO: form.setIconImage(Properties.Resources.AriadneIcon_32x32);
            form.pack();
            form.show();
        }
    }
}
