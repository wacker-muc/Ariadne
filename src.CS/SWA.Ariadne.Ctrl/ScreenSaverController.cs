using System;
using System.Drawing;
using System.Text;
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
    /// Except: We don't have access to the mouse or keyboard; those are
    /// managed exclusively by xscreensaver.
    /// </summary>
    public class ScreenSaverController : IAriadneEventHandler, IMazeForm
    {
        #region Code taken from ScreenSaverForm

        /// <summary>
        /// An ImageLoader supplied by the main program.
        /// </summary>
        private readonly ImageLoader imageLoader;

        /// <summary>
        /// When true, the built maze should not be too complicated.
        /// </summary>
        private static bool loadingFirstMaze = true;

        private readonly Random random = SWA.Utilities.RandomFactory.CreateRandom();

        private enum CaptionInfoEnum
        {
            Default = 0,
            ImagePath,
            Help,
            Count // number of relevant entries
        }
        private CaptionInfoEnum captionInfoItem;
        private int captionInfoImageNumber;

        /// <summary>
        /// Place reserved areas into the maze.
        /// This method is called from the MazeUserControl before actually building the maze.
        /// </summary>
        /// <param name="maze"></param>
        public void MakeReservedAreas(Maze maze)
        {
            if (infoPanelPainter != null)
            {
                infoPanelPainter.ChooseLocation(mazeUserControl.Size, this.random);
                mazeUserControl.ReserveArea(infoPanelPainter.Panel);
            }

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
                    shape = mazeUserControl.RandomShape(0.2, 1.0, random);

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
                OutlineShape shape = mazeUserControl.RandomShape(0.3, 0.7, random);
                mazeUserControl.Maze.OutlineShape = shape;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Process the keystrokes sent to the targetWindow.
        /// </summary>
        /// <param name="sender">The target window (a Form).</param>
        /// <param name="e"></param>
        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            switch (char.ToUpper(e.KeyChar))
            {
                case (char)Keys.P:
                    ariadneController.Pause();
                    e.Handled = true;
                    break;

                case (char)Keys.OemPeriod:
                case '.':
                    ariadneController.SingleStep();
                    e.Handled = true;
                    break;

                case (char)Keys.S:
                    ariadneController.SaveImage(this.mazeUserControl);
                    e.Handled = true;
                    break;

                case (char)Keys.I:
                    if (captionInfoItem == CaptionInfoEnum.ImagePath && captionInfoImageNumber + 1 < mazeUserControl.ImageCount)
                    {
                        captionInfoImageNumber++;
                    }
                    else
                    {
                        captionInfoImageNumber = 0;
                        captionInfoItem = (CaptionInfoEnum)((int)(captionInfoItem + 1) % (int)CaptionInfoEnum.Count);
                    }
                    UpdateCaption();
                    e.Handled = true;
                    break;

                case (char)Keys.OemQuestion:
                case '?':
                    captionInfoItem = CaptionInfoEnum.Help;
                    UpdateCaption();
                    e.Handled = true;
                    break;

                default:
                    (sender as Form).Close();
                    e.Handled = true;
                    break;
            }
        }

        #endregion

        private MazePainter painter;
        private AriadneController ariadneController;
        private MazeUserControl mazeUserControl;
        private InfoPanelPainter infoPanelPainter;

        #region Constructor

        /// <summary>
        /// Creates a ScreenSaverController instance,
        /// draws an initial maze
        /// and starts an AriadneController.
        /// </summary>
        /// <param name="windowHandleArg">The MazePainter will draw on this window.</param>
        /// <param name="imageLoader">Image loader.</param>
        public ScreenSaverController(
            string windowHandleArg,
            ImageLoader imageLoader)
        {
            this.imageLoader = imageLoader;

            #region Create a MazePainter.
            var windowHandle = (IntPtr)UInt32.Parse(windowHandleArg);
            var targetGraphics = Graphics.FromHwnd(windowHandle);
            var targetRectangle = Platform.GetClientRectangle(windowHandle);
            //Log.WriteLine("targetRectangle = " + targetRectangle, true); // {X=0,Y=0,Width=1366,Height=768}
            this.painter = new MazePainter(targetGraphics, targetRectangle, this as IMazePainterClient);
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

            if (RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_SHOW_DETAILS_BOX))
            {
                this.infoPanelPainter = new InfoPanelPainter(painter);
            }
            #endregion

            // Create and display the first maze.
            this.OnNew(null, null);

            #region Create and start an AriadneController.
            SolverController controller = new SolverController(this, painter, null);
            this.ariadneController = new AriadneController(this, controller);
            ariadneController.RepeatMode = true;
            ariadneController.Start();
            #endregion
        }

        #endregion

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
            if (ariadneController.State == Logic.SolverState.Finished)
            {
                // Draw the final state of the info panel.
                // Note: It may have been covered by MazePainter.DrawRemainingBackgroundSquares()
                infoPanelPainter.Paint();
            }
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
            // The ScreenSaverForm behavior is as follows:
#if false
            this.PrepareImages();
            this.PreparePlaceholderControls(); // i.e. place the info panel
            this.mazeUserControl.PrepareAlternateBuffer();
#endif
            // I don't know if the alternate buffer method would work in a
            // xscreensaver context. As I also don't experience any unwanted
            // delays between successive mazes (anyway, there is an intentional
            // 3 second delay), no special preparations are implemented here.
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
            if (infoPanelPainter == null) return;

            var text = new StringBuilder();
            if (ariadneController != null)
            {
                ariadneController.FillStatusMessage(text);
#if true
                // Append current time to the status line.
                if (text.Length > 0)
                {
                    text.Append(" - ");
                    text.Append(System.DateTime.Now.ToString("t"));
                }
#endif
            }

            infoPanelPainter.SetStatus(text.ToString());
        }

        public void UpdateCaption()
        {
            if (infoPanelPainter == null) return;

            var caption = new StringBuilder();
            switch (captionInfoItem)
            {
                default:
                case CaptionInfoEnum.Default:
                    FillCaption(caption);
                    break;

                case CaptionInfoEnum.Help:
                    caption.Append("[P]: pause/resume");
                    caption.Append("  [.]: single step");
                    caption.Append("  [S]: screenshot");
                    caption.Append("  [I]: switch info");
                    break;

                case CaptionInfoEnum.ImagePath:
                    string imagePath = mazeUserControl.GetImagePath(captionInfoImageNumber);
                    int maxLength = 65;
                    if (imagePath.Length > maxLength + 2)
                    {
                        caption.Append("...");
                        caption.Append(imagePath.Substring(imagePath.Length - maxLength));
                    }
                    else
                    {
                        caption.Append(imagePath);
                    }
                    break;
            }

            infoPanelPainter.SetCaption(caption.ToString());
        }

        private void FillCaption (StringBuilder text)
        {
            text.Append("Ariadne");

            if (ariadneController != null)
            {
                text.Append(" - ");
                text.Append(this.StrategyName);
            }

            if (mazeUserControl != null)
            {
                text.Append(" - ");
                text.Append(mazeUserControl.XSize.ToString() + "x" + mazeUserControl.YSize.ToString());
            }

            if (ariadneController != null)
            {
                text.Append(" - ");
                text.Append(ariadneController.StepsPerSecond.ToString());
            }

            if (mazeUserControl != null)
            {
                text.Append(" - ");
                text.Append("ID: " + mazeUserControl.Code);
            }
        }

        #endregion

        #region Simulation of a Screen Saver environment

        /// <summary>
        /// Run an Ariadne maze within an arbitrary window.
        /// Used for simulating the environment given by xscreensaver on Linux.
        /// </summary>
        public static void Run()
        {
            var form = CreateTargetWindow();
            var control = new Control // will provide the drawing area
            {
                Size = form.ClientSize,
                Location = new Point(0, 0),
                Enabled = false, // so that it doesn't consume keystrokes
            };
            form.Controls.Add(control);
            form.Show();

            string windowHandleArg = control.Handle.ToString();

            ImageLoader imageLoader = ImageLoader.GetScreenSaverImageLoader(
                new Rectangle(new Point(0, 0), form.ClientSize));

            var ctrl = new ScreenSaverController(windowHandleArg, imageLoader);

            // Send the form's events to the controller.
            form.KeyPress += ctrl.OnKeyPress;
            form.FormClosing += ctrl.TargetWindowClosing;

            // The first maze has already been loaded
            // when the ScreenSaverController was constructed.
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
            //result.ShowInTaskbar = false;

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
