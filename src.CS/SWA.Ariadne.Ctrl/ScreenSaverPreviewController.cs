using System;
using System.Windows.Forms;
using System.Drawing;
using SWA.Ariadne.Gui.Mazes;
using SWA.Utilities;

namespace SWA.Ariadne.Ctrl
{
    /// <summary>
    /// Controls a Maze built within a Screen Saver Preview window.
    /// Takes the role of an AriadneFormBase.
    /// </summary>
    public class ScreenSaverPreviewController
        : IAriadneEventHandler
        , IMazePainterClient
    {
        #region Member variables

        /// <summary>
        /// A handle of the parent window, i.e. the Screen Saver Preferences dialog.
        /// </summary>
        private IntPtr parentHwnd;

        private Graphics targetGraphics;
        private Rectangle targetRectangle;

        /// <summary>
        /// The object controlling a running solver.
        /// </summary>
        private AriadneController ariadneController;

        /// <summary>
        /// The MazePainter responsible for all painting activities.
        /// </summary>
        private MazePainter painter;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// Creates and starts an AriadneController.
        /// </summary>
        /// <param name="windowHandleArg"></param>
        private ScreenSaverPreviewController(string windowHandleArg)
        {
            // Get the window in which we are supposed to paint.
            this.parentHwnd = (IntPtr)UInt32.Parse(windowHandleArg);
            this.targetRectangle = Platform.GetClientRectangle(parentHwnd);
            //Log.WriteLine("targetRectangle = " + targetRectangle);

            // Create a MazePainter.
            this.targetGraphics = Graphics.FromHwnd(parentHwnd);
            this.painter = new MazePainter(targetGraphics, targetRectangle, this as IMazePainterClient, true);

            // Create and display the first maze.
            this.OnNew(null, null);

            // Create an AriadneController.
            SolverController controller = new SolverController(null, painter, null);
            this.ariadneController = new AriadneController(this, controller);
            ariadneController.RepeatMode = true;

            // Start the AriadneController.
            ariadneController.Start();

            // Start a supervisor timer.
            CreateSupervisorTimer();
        }

        #endregion

        #region Watch for when the target window is closed.

        private Timer supervisorTimer;

        /// <summary>
        /// Starts a timer with moderately high frequency for checking if we are still alive.
        /// This complements the AriadneController timers that also check the Alive property.
        /// </summary>
        private void CreateSupervisorTimer()
        {
            supervisorTimer = new Timer();
            supervisorTimer.Interval = 100;
            supervisorTimer.Tick += new EventHandler(this.OnSupervisorTimer);
            supervisorTimer.Start();
        }

        /// <summary>
        /// Check that the target window still exists.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSupervisorTimer(object sender, EventArgs e)
        {
            // Just evaluate the Alive property.
            if (Alive) { }
        }

        #endregion

        #region IAriadneEventHandler Members

        /// <summary>
        /// Returns false if the window is no longer visible.
        /// Also exits the main application loop.
        /// </summary>
        public bool Alive
        {
            get
            {
                // Quit if the preview dialog is dismissed.  Check this periodically.
                // Note: There is no relevant equivalent on Linux. There, the
                // xscreensaver(1) driver program will kill this program when
                // it is no longer needed.
                if (Platform.IsWindows && !Platform.Windows.IsWindowVisible(parentHwnd))
                {
                    painter.Reset();
                    ariadneController.Stop();
                    Application.Exit();
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public void NotifyControllerStateChanged()
        {
            // do nothing
        }

        /// <summary>
        /// Creates a new maze.
        /// </summary>
        public void OnNew(object sender, EventArgs e)
        {
            #region Set up the painter.

            painter.Setup();

            int squareWidth;
            int pathWidth;
            int wallWidth;
            MazePainter.SuggestWidths(painter.GridWidth, painter.VisibleWalls, out squareWidth, out pathWidth, out wallWidth);

            painter.Setup(squareWidth, wallWidth, pathWidth);

            #endregion

            painter.Reset();
            painter.BlinkingCounter = 0;

            // Create and display a maze.
            painter.CreateMaze(null);
            painter.PaintMaze(null);
        }

        public void PrepareForNextStart()
        {
            // do nothing
        }

        public void ResetCounters()
        {
            // do nothing
        }

        public void Update()
        {
            // do nothing
        }

        #endregion

        #region IMazePainterClient Members

        public Graphics CreateGraphics()
        {
            return this.targetGraphics;
        }

        public Rectangle DisplayRectangle
        {
            get { return this.targetRectangle; }
        }

        #endregion

        #region Static class methods

        public static void Run(string windowHandleArg)
        {
            ScreenSaverPreviewController ctrl
                = new ScreenSaverPreviewController(windowHandleArg);

            // Start a main application loop.
            Application.Run();
        }

        #endregion

        #region Simulation of a Screen Saver Preview environment

        /// <summary>
        /// Run an Ariadne maze in a small window.
        /// Used for simulating the Screen Saver Preview environment.
        /// </summary>
        public static void Run()
        {
            Form form = CreateTargetWindow();
            var control = new Control { Size = form.ClientSize, Location = new Point(0, 0) };
            form.Controls.Add(control);
            form.Show();
            string windowHandleArg = control.Handle.ToString();

            var ctrl = new ScreenSaverPreviewController(windowHandleArg);
            form.FormClosing += ctrl.TargetWindowClosing;

            // Now, the controller runs within the existing main application loop.
        }

        /// <summary>
        /// Create a small window for simulating the Screen Saver Preview environment.
        /// </summary>
        private static Form CreateTargetWindow()
        {
            Form result = new Form();
            result.Name = result.Text = "Ariadne Preview";
            result.ClientSize = new System.Drawing.Size(240, 180);
            result.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            result.ControlBox = true;
            result.MaximizeBox = false;
            result.MinimizeBox = false;
            //result.ShowInTaskbar = false;

            return result;
        }

        /// <summary>
        /// Will stop the AriadneController and de-construct this object.
        /// </summary>
        private void TargetWindowClosing(object sender, FormClosingEventArgs e)
        {
            // When the form is closed, stop the controller.
            ariadneController.Stop();

            // Discard all member variables.
            this.ariadneController = null;
            this.painter = null;
        }

        #endregion
    }
}
