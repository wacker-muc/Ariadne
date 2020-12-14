using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SWA.Ariadne.Gui.Mazes;
using SWA.Ariadne.Model;
using SWA.Ariadne.Outlines;
using SWA.Ariadne.Settings;
using SWA.Utilities;

namespace SWA.Ariadne.Gui
{
    public partial class ScreenSaverForm : MazeForm
    {
        #region Member variables

        private bool fullScreenMode = true;

        private enum CaptionInfoEnum
        {
            Default = 0,
            ImagePath,
            Count
        }
        private CaptionInfoEnum captionInfoItem;
        private int captionInfoImageNumber;

        /// <summary>
        /// During the first few seconds after the screen saver has been loaded,
        /// a MouseMove event will cause an application exit.
        /// </summary>
        private readonly DateTime startTime = DateTime.Now;
        private readonly int startSeconds = 6;
        private Point mouseLocation = new Point(-1, -1);

        /// <summary>
        /// An ImageLoader supplied by the main program.
        /// </summary>
        private ImageLoader imageLoader;

        /// <summary>
        /// When true, the built maze should not be too complicated.
        /// </summary>
        private bool loadingFirstMaze = true;

        private Random random = SWA.Utilities.RandomFactory.CreateRandom();

        /// <summary>
        /// These controls are displayed in reserved areas of the maze.
        /// </summary>
        List<Control> visibleControls = new List<Control>();

        /// <summary>
        /// These controls are placed at the locations their actual counterparts will take in the following iteration.
        /// </summary>
        List<Control> placeholderControls = new List<Control>();

        #endregion

        #region Constructor

        public ScreenSaverForm(bool fullScreenMode, ImageLoader imageLoader)
        {
            InitializeComponent();

            if (fullScreenMode)
            {
                this.Text += "/" + Process.GetCurrentProcess().Id; // make it unique
            }

            this.fullScreenMode = fullScreenMode;
            this.imageLoader = imageLoader;
            this.ShowInTaskbar = ! this.fullScreenMode;
            this.DoubleBuffered = false;

            // Initially, the (optionally) displayed controls should be invisible until the maze has been built.
            this.outerInfoPanel.Visible = false;

            if (RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_PAINT_ALL_WALLS) == false)
            {
                this.mazeUserControl.RandomizeWallVisibility = true;
            }
            ContourImage.DisplayProcessedImage = RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_IMAGE_SUBTRACT_BACKGROUND);
        }

        /// <summary>
        /// Set up the main form as a full screen screensaver.
        /// </summary>
        private void SetupFullscreenScreenSaver()
        {
            if (!fullScreenMode) return;

            // Set the application to full screen mode and hide the mouse cursor.
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.TopMost = true;
            this.Bounds = Screen.PrimaryScreen.Bounds;
            this.WindowState = FormWindowState.Maximized;
            Cursor.Hide();

            // Make the MazePainter use the whole screen area.
            this.mazeUserControl.MazePainter.Padding = 0;

            // Make this the active Form and capture the mouse.
            this.Activate();
            this.Capture = true;
        }

        /// <summary>
        /// Make the Form really cover the whole screen, including Panels.
        /// </summary>
        private void SetupFullscreenLinux()
        {
            if (!fullScreenMode) return;

            CallWmctrl("-r " + this.Text + " -b add,above,fullscreen", "set proper full screen mode");

        }

        /// <summary>
        /// Call the wmctrl(1) program with the given arguments.
        /// In case of an error, write a message to the Log.
        /// </summary>
        /// <remarks>
        /// This code is taken from
        /// https://stackoverflow.com/questions/2823299/mono-winforms-app-fullscreen-in-ubuntu
        /// We need an external program (wmctrl).
        /// </remarks>
        private static void CallWmctrl(string wmctrlArgs, string actionText)
        {
            int exitCode;
            string msg = null;

            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "wmctrl",
                    Arguments = wmctrlArgs,
                    CreateNoWindow = true
                }
            };
            try
            {
                process.Start();
                process.WaitForExit();

                exitCode = process.ExitCode;
            }
            catch (Exception ex)
            {
                exitCode = -999;
                msg = ex.Message;
            }

            if (exitCode != 0)
            {
                Log.WriteLine("Cannot " + actionText + ". Please install the 'wmctrl' program.", true);
                if (msg != null && msg != "")
                {
                    Log.WriteLine(msg, true);
                }
            }
        }

        #endregion

        #region Event handlers

        private void ScreenSaverForm_Load(object sender, EventArgs e)
        {
            // Switch auto repeat mode on.
            ariadneController.RepeatMode = true;

            if (fullScreenMode)
            {
                SetupFullscreenScreenSaver();

                // Let the MazeUserControl cover the whole form.
                this.mazeUserControl.Location = new Point(0, 0);
                this.mazeUserControl.Size = this.Size;
            }
            else
            {
                // Let the MazeUserControl cover most of the form.
                this.mazeUserControl.Location = new Point(0, 0);
                this.mazeUserControl.Size = this.DisplayRectangle.Size;
            }
            this.mazeUserControl.BringToFront();

            // Other optional controls need to be displayed in front of the maze.
            this.outerInfoPanel.BringToFront();

            // Load background images.
            if (RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_BACKGROUND_IMAGES))
            {
                string imageFolder = RegisteredOptions.GetStringSetting(RegisteredOptions.OPT_BACKGROUND_IMAGE_FOLDER);
                if (imageFolder == "")
                {
                    imageFolder = RegisteredOptions.GetStringSetting(RegisteredOptions.OPT_IMAGE_FOLDER);
                }
                int percentage = ((RegisteredOptions.GetIntSetting(RegisteredOptions.OPT_IMAGE_NUMBER) > 0) ? 20 : 100);
                this.mazeUserControl.MazePainter.CreateBackgroundImageLoader(imageFolder, percentage);
            }

            this.OnNew(null, null);
            this.OnStart(null, null);

            // Now the first maze has been loaded.
            this.loadingFirstMaze = false;
        }

        void ScreenSaverForm_Shown(object sender, EventArgs e)
        {
            if (Platform.IsLinux)
            {
                this.SetupFullscreenLinux();
            }
        }

        public override void OnNew(object sender, EventArgs e)
        {
            // Discard this call until ScreenSaverForm_Load() has been executed.
            // Note: The repeatMode flag is initially false and will be set above.
            if (ariadneController.RepeatMode == false)
            {
                return;
            }

            //base.OnNew(sender, e); // TODO: remove this call

            // Choose new locations of controls, before the maze is built.
            if (visibleControls.Count == 0)
            {
                PreparePlaceholderControls();
            }

            // Reset the displayed caption info.
            this.captionInfoItem = 0;

            // Build and display the new maze.
            base.OnNew(sender, e);

            // Place visible controls at the locations determined for their placeholders.
            for (int i = 0; i < visibleControls.Count; i++)
            {
                Control control = visibleControls[i];
                Control placeholder = placeholderControls[i];

                control.Location = placeholder.Location;
                control.Visible = true;
                
                this.Controls.Remove(placeholder);
            }
        }

        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            switch (char.ToUpper(e.KeyChar))
            {
                case (char)Keys.P: // Pause
                case (char)Keys.S: // Save Image
                    // These keys will be handled in the AriadneFormBase.OnKeyPress() method.
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

                default:
                    Close();
                    e.Handled = true;
                    break;
            }
        }

        private void ScreenSaverForm_MouseDown(object sender, MouseEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Closes the screen saver if the mouse is moved
        /// within the first few seconds after the program has started.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScreenSaverForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseLocation.X < 0)
            {
                mouseLocation = e.Location;
                return;
            }
            if (mouseLocation.X == e.Location.X && mouseLocation.Y == e.Location.Y)
            {
                return;
            }
            if ((DateTime.Now - this.startTime).TotalSeconds > this.startSeconds)
            {
                return;
            }
            Close();
        }

        private void ScreenSaverForm_ResizeEnd(object sender, EventArgs e)
        {
            // The ImageLoader should adjust its image size limits, as well.
            mazeUserControl.ImageLoader.UpdateImageSize(mazeUserControl.DisplayRectangle);
        }

        private void ScreenSaverForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (mazeUserControl.ImageLoader != null)
            {
                mazeUserControl.ImageLoader.Shutdown();
            }
            if (mazeUserControl.MazePainter.BackgroundImageLoader != null)
            {
                mazeUserControl.MazePainter.BackgroundImageLoader.Shutdown();
            }
        }

        #endregion

        #region AriadneFormBase implementation

        /// <summary>
        /// Prepare images; determine positions of other visible controls.
        /// </summary>
        /// <param name="baseFirst"></param>
        protected override void PrepareForNextStart(bool baseFirst)
        {
            if (baseFirst)
            {
                base.PrepareForNextStart(baseFirst);
                // more code
            }
            else
            {
                this.PrepareImages();
                this.PreparePlaceholderControls();
                base.PrepareForNextStart(baseFirst);
            }
        }

        /// <summary>
        /// While the actual controls are still visible, create and place (invisible) placeholders at new locations.
        /// These placeholders are used for reserving free areas in the maze.
        /// </summary>
        private void PreparePlaceholderControls()
        {
            visibleControls.Clear();
            placeholderControls.Clear();
            
            PreparePlaceholderControl(this.outerInfoPanel, RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_SHOW_DETAILS_BOX));
        }

        /// <summary>
        /// If the given control shall be visible, an invisible placeholder is added to this form at a new, random location.
        /// In the next iteration, the actual control will replace the placeholder at that location.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="isVisible"></param>
        private void PreparePlaceholderControl(Control control, bool isVisible)
        {
            if (control == null)
            {
                return;
            }

            if (!isVisible)
            {
                control.Visible = false;
                return;
            }

            // Add the control and a placeholder of the same size to their respective lists.
            visibleControls.Add(control);
            Control placeholder = new Control("", control.Left, control.Top, control.Width, control.Height);
            placeholderControls.Add(placeholder);

            // Place the placeholder at a random location.
            placeholder.Location = InfoPanelPainter.SuggestLocation(control.Size, this.Size, this.random);

            // Add the invisible placeholder control to this form.
            placeholder.Visible = false;
            this.Controls.Add(placeholder);
        }

        protected override void ApplyDefaultSettings()
        {
            // The ScreenSaverForm doesn't take its settings from a DetailsDialog.
            //base.ApplyDefaultSettings();
        }

        #endregion

        #region IMazeForm implementation

        /// <summary>
        /// Place reserved areas into the maze.
        /// This method is called from the MazeUserControl before actually building the maze.
        /// </summary>
        /// <param name="maze"></param>
        public override void MakeReservedAreas(Maze maze)
        {
            base.MakeReservedAreas(maze);

            #region Info Panel (and other controls)

            for (int i = 0; i < placeholderControls.Count; i++)
            {
                mazeUserControl.ReserveArea(placeholderControls[i]);
            }

            #endregion

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
            if (this.loadingFirstMaze)
            {
                if (this.imageLoader != null)
                {
                    mazeUserControl.ImageLoader = this.imageLoader;
                }
                else
                {
                    mazeUserControl.ImageLoader = ImageLoader.GetScreenSaverImageLoader(this.Bounds);
                }
            }

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
        /// Displays information about the running MazeSolver in the status line.
        /// </summary>
        public override void UpdateStatusLine()
        {
            if (this.infoLabelStatus != null && ariadneController != null)
            {
                StringBuilder message = new StringBuilder(200);

                ariadneController.FillStatusMessage(message);
#if true
                // Append current time to the status line.
                if (message.Length > 0)
                {
                    message.Append(" - ");
                    message.Append(System.DateTime.Now.ToString("t"));
                }
#endif

                this.infoLabelStatus.Text = message.ToString();
            }
        }

        /// <summary>
        /// Displays Maze and Solver characteristics in the window's caption bar.
        /// The maze ID, step rate and solver strategy name.
        /// </summary>
        public override void UpdateCaption()
        {
            if (this.infoLabelCaption != null)
            {
                StringBuilder caption = new StringBuilder(80);

                switch (this.captionInfoItem)
                {
                    default:
                    case CaptionInfoEnum.Default:
                        FillCaption(caption);

                        break;

                    case CaptionInfoEnum.ImagePath:
                        string imagePath = this.mazeUserControl.GetImagePath(this.captionInfoImageNumber);
                        int maxLength = 72;
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

                this.infoLabelCaption.Text = caption.ToString();
            }
        }

        public override string StrategyName
        {
            get
            {
                string result = null;

                if (result == null && ariadneController != null)
                {
                    result = ariadneController.StrategyName;
                }

                return result;
            }
        }

        #endregion
    }
}
