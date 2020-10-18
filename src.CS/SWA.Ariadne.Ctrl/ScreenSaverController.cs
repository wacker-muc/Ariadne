using System;
using System.Drawing;
using System.Windows.Forms;
using SWA.Ariadne.Gui.Mazes;
using SWA.Ariadne.Model;
using SWA.Ariadne.Outlines;
using SWA.Ariadne.Settings;
using SWA.Utilities;

namespace SWA.Ariadne.Ctrl
{
    /// <summary>
    /// Supports running an Ariadne application in a window provided by
    /// the Linux xscreensaver(1) program, much like ScreenSaverForm
    /// works on a Windows platform.
    /// Except: We don't have access to the mouse or keyboard, that is
    /// managed exclusively by xscreensaver.
    /// </summary>
    public class ScreenSaverController : IAriadneEventHandler, IMazeForm
    {
        #region Code taken from ScreenSaverForm

        /// <summary>
        /// An ImageLoader supplied by the main program.
        /// </summary>
        private ImageLoader imageLoader;

        /// <summary>
        /// When true, the built maze should not be too complicated.
        /// </summary>
        private static bool loadingFirstMaze = true;

        private readonly Random random = SWA.Utilities.RandomFactory.CreateRandom();

        /// <summary>
        /// Place reserved areas into the maze.
        /// This method is called from the MazeUserControl before actually building the maze.
        /// </summary>
        /// <param name="maze"></param>
        public void MakeReservedAreas(Maze maze)
        {
#if false // TODO: here we need a different approach
            #region Info Panel (and other controls)

            for (int i = 0; i < placeholderControls.Count; i++)
            {
                mazeUserControl.ReserveArea(placeholderControls[i]);
            }
            #endregion
#endif

            #region Images and other adornments

            // Images.
            if (!mazeUserControl.HasPreparedImages)
            {
                this.PrepareImages();
            }
            mazeUserControl.ReserveAreaForImages();

            // The remaining adornments are not applied to the first maze.
            if (loadingFirstMaze == false)
            {
                bool hasEmbeddedShape = false;

                if (!hasEmbeddedShape)
                {
                    // Embedded mazes.
                    hasEmbeddedShape |= this.AddEmbeddedMaze();
                }

                if (!hasEmbeddedShape)
                {
                    // Outline shapes.
                    hasEmbeddedShape |= this.AddOutlineShape();
                }

                // Irregular maze shapes.
                if (RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_IRREGULAR_MAZES) && random.Next(100) < 10)
                {
                    maze.Irregular = true;
                    maze.Irregularity = 80;
                }
            }

            #endregion
        }

        /// <summary>
        /// Load a number of images and place them in the maze.
        /// </summary>
        private void PrepareImages()
        {
            // TODO: Get the count from the ImageLoader object.
            int count = RegisteredOptions.GetIntSetting(RegisteredOptions.OPT_IMAGE_NUMBER);
            mazeUserControl.PrepareImages(count);
        }

        /// <summary>
        /// Add an embedded maze.
        /// </summary>
        private bool AddEmbeddedMaze()
        {
            int percentage = (RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_MULTIPLE_MAZES) ? 15 : 0);
            if (random.Next(100) < percentage)
            {
                OutlineShape shape = null;
                int area = mazeUserControl.Maze.XSize * mazeUserControl.Maze.YSize;
                int minArea = (int)(0.1 * area), maxArea = (int)(0.5 * area);

                while (true)
                {
                    shape = RandomShape(0.2, 1.0);

                    // Discard shapes that are too small or too large.
                    if (minArea > shape.Area || shape.Area > maxArea)
                    {
                        continue;
                    }

                    // Discard shapes that cover the whole maze.
                    if (shape.BoundingBox.Width >= mazeUserControl.Maze.XSize || shape.BoundingBox.Height >= mazeUserControl.Maze.YSize)
                    {
                        continue;
                    }

                    break; // Terminate the loop.
                }

                mazeUserControl.Maze.AddEmbeddedMaze(shape);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Add an outline shape to the maze.
        /// </summary>
        private bool AddOutlineShape()
        {
            int percentage = (RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_OUTLINE_SHAPES) ? 80 : 0);
            if (random.Next(100) < percentage)
            {
                OutlineShape shape = RandomShape(0.3, 0.7);
                mazeUserControl.Maze.OutlineShape = shape;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns an OutlineShape for the given location and size.
        /// If the image displayed in the maze control has a defined contour,
        /// the shape is preferrably derived from that contour.
        /// </summary>
        /// <param name="offCenter"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private OutlineShape RandomShape(double offCenter, double size)
        {
            OutlineShape result = null;

            // The mazeUserControl may suggest a shape based on the displayed ContourImage.
            if (random.Next(100) < (ContourImage.DisplayProcessedImage ? 12 : 25))
            {
                result = mazeUserControl.SuggestOutlineShape(random, offCenter, size);
            }

            if (result == null)
            {
                result = OutlineShape.RandomInstance(random, mazeUserControl.Maze.XSize, mazeUserControl.Maze.YSize, offCenter, size);
            }

            return result;
        }

        #endregion

        private MazePainter painter;
        private AriadneController ariadneController;
        private MazeUserControl mazeUserControl;

        public ScreenSaverController(
            string windowHandleArg,
            ImageLoader imageLoader)
        {
            this.imageLoader = imageLoader;

            #region Create a MazePainter.
            var windowHandle = (IntPtr)UInt32.Parse(windowHandleArg);
            var targetGraphics = Graphics.FromHwnd(windowHandle);
            var targetRectangle = Platform.GetClientRectangle(windowHandle);
            Log.WriteLine("targetRectangle = " + targetRectangle, true); // {X=0,Y=0,Width=1366,Height=768}
            this.painter = new MazePainter(targetGraphics, targetRectangle, this as IMazePainterClient, true);
            #endregion

            #region Create a MazeUserControl.
            this.mazeUserControl = new MazeUserControl(painter, targetRectangle.Size);
            this.mazeUserControl.ImageLoader = this.imageLoader;
            this.mazeUserControl.MazeForm = this;
            #endregion

            #region Apply some registered options.
            if (RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_PAINT_ALL_WALLS) == false)
            {
                painter.RandomizeWallVisibility = true;
            }
            ContourImage.DisplayProcessedImage = RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_IMAGE_SUBTRACT_BACKGROUND);

            // Load background images.
            if (RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_BACKGROUND_IMAGES))
            {
                string imageFolder = RegisteredOptions.GetStringSetting(RegisteredOptions.OPT_BACKGROUND_IMAGE_FOLDER);
                if (imageFolder == "")
                {
                    imageFolder = RegisteredOptions.GetStringSetting(RegisteredOptions.OPT_IMAGE_FOLDER);
                }
                int percentage = ((RegisteredOptions.GetIntSetting(RegisteredOptions.OPT_IMAGE_NUMBER) > 0) ? 20 : 100);
                painter.CreateBackgroundImageLoader(imageFolder, percentage);
            }
            #endregion

            // Create and display the first maze.
            this.OnNew(null, null);

            #region Create and start an AriadneController.
            SolverController controller = new SolverController(null, painter, null);
            this.ariadneController = new AriadneController(this, controller);
            ariadneController.RepeatMode = true;
            ariadneController.Start();
            #endregion
        }

        #region Static class methods

        public static void Run(
            string windowHandleArg,
            ImageLoader imageLoader)
        {
            var ctrl = new ScreenSaverController(windowHandleArg, imageLoader);

            // Now the first maze has been loaded.
            loadingFirstMaze = false;

            // Start a main application loop.
            Application.Run();
        }

        #endregion

        #region IAriadneEventHandler implementation

        public bool Alive => true;

        public void NotifyControllerStateChanged()
        {
            // do nothing
        }

        /// <summary>
        /// Creates a new maze.
        /// </summary>
        public void OnNew(object sender, EventArgs e)
        {
            mazeUserControl.Setup();
        }

        public void PrepareForNextStart()
        {
            // TODO: add ScreenSaverForm behavior
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

        #region IMazeForm implementation

        public string StrategyName => ariadneController.StrategyName;

        public void UpdateStatusLine()
        {
            // do nothing
        }

        public void UpdateCaption()
        {
            // do nothing
        }

        #endregion

        #region Simulation of a Screen Saver environment

        /// <summary>
        /// Run an Ariadne maze within an arbitrary window.
        /// Used for simulating the environment given by xscreensaver on Linux.
        /// </summary>
        public static void Run()
        {
            Form form = CreateTargetWindow();
            form.Show();
            string windowHandleArg = form.Handle.ToString();

            ImageLoader imageLoader = ImageLoader.GetScreenSaverImageLoader(
                new Rectangle(new Point(0,0), form.ClientSize));

            var ctrl = new ScreenSaverController(
                windowHandleArg, imageLoader/*, form.ClientSize.Width, form.ClientSize.Height*/);
            form.FormClosing += ctrl.TargetWindowClosing;

            // Now the first maze has been loaded.
            loadingFirstMaze = false;

            // Now, the controller runs within the existing main application loop.
        }

        /// <summary>
        /// Create a medium sized window for simulating the Screen Saver environment.
        /// Note: You might use this to capture medium sized images.
        /// </summary>
        private static Form CreateTargetWindow()
        {
            Form result = new Form();
            result.Name = result.Text = "Ariadne Screensaver";
            result.ClientSize = new System.Drawing.Size(800, 600);
            result.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            result.ControlBox = true;
            result.MaximizeBox = false;
            result.MinimizeBox = false;
            result.ShowInTaskbar = false;

            // TODO: make the Form handle keystrokes, e.g. Save, Pause

            return result;
        }

        /// <summary>
        /// Will stop the AriadneController and de-construct this object.
        /// </summary>
        private void TargetWindowClosing(object sender, FormClosingEventArgs e)
        {
            // When the target window is closed, stop the controller.
            ariadneController.Stop();

            // Discard all member variables.
            this.ariadneController = null;
            this.mazeUserControl = null;
            this.painter = null;
        }

        #endregion
    }
}
