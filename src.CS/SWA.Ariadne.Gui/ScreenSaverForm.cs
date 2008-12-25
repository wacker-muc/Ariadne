using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SWA.Ariadne.Gui.Mazes;
using SWA.Ariadne.Model;
using SWA.Ariadne.Outlines;
using SWA.Ariadne.Settings;

namespace SWA.Ariadne.Gui
{
    public partial class ScreenSaverForm : MazeForm
    {
        #region Member variables

        private bool fullScreenMode = true;

        private enum CaptionInfoEnum : int
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

            this.fullScreenMode = fullScreenMode;
            this.imageLoader = imageLoader;
            this.ShowInTaskbar = false;
            this.DoubleBuffered = false;

            // Initially, the (optinally) displayed controls should be invisible until the maze has been built.
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
        private void SetupScreenSaver()
        {
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

        #endregion

        #region Event handlers

        private void ScreenSaverForm_Load(object sender, EventArgs e)
        {
            // Switch auto repeat mode on.
            ariadneController.RepeatMode = true;

            if (fullScreenMode)
            {
                SetupScreenSaver();

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

        private void ScreenSaverForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.P: // Pause
                case Keys.S: // Save Image
                    // These keys will be handled in the AriadneFormBase.OnKeyPress() method.
                    break;
                
                case Keys.I:
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
            int xMin = this.Size.Width / 20;
            int yMin = this.Size.Height / 20;
            int xMax = this.Size.Width - xMin - control.Size.Width;
            int yMax = this.Size.Height - yMin - control.Size.Height;
            int x = this.random.Next(xMin, xMax);
            int y = this.random.Next(yMin, yMax);

            #region Try to find a Y coordinate that leaves enough room for an image.

            int imgCount = RegisteredOptions.GetIntSetting(RegisteredOptions.OPT_IMAGE_NUMBER);
            int imgSize = 20 + RegisteredOptions.GetIntSetting(RegisteredOptions.OPT_IMAGE_MAX_SIZE);

            for (int i = 0; i < 8; i++)
            {
                if (imgCount < 1 || y > imgSize || y < this.Size.Height - imgSize - control.Size.Height)
                {
                    break;
                }

                y = this.random.Next(yMin, yMax);
            }

            #endregion

            placeholder.Location = new Point(x, y);

            // Add the invisible placeholder control to this form.
            placeholder.Visible = false;
            this.Controls.Add(placeholder);
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
                    mazeUserControl.ImageLoader = ImageLoader.GetScreenSaverImageLoader();
                }
            }

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

        /// <summary>
        /// Displays information about the running MazeSolver in the status line.
        /// </summary>
        public override void UpdateStatusLine()
        {
            if (this.infoLabelStatus != null && ariadneController != null)
            {
                StringBuilder message = new StringBuilder(200);

                ariadneController.FillStatusMessage(message);

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

                        caption.Append(" - ");
                        caption.Append(System.DateTime.Now.ToString("t"));
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
