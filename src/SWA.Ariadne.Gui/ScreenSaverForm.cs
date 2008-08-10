using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using SWA.Ariadne.Model;
using SWA.Ariadne.Outlines;
using SWA.Ariadne.Settings;

namespace SWA.Ariadne.Gui
{
    public partial class ScreenSaverForm : MazeForm
    {
        #region  Use Win32 API functions for dealing with preview dialog box

        [DllImport("user32.dll")]
        private static extern bool GetClientRect(IntPtr hWnd, ref RECT rect);
        [DllImport("user32.DLL", EntryPoint = "IsWindowVisible")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        #endregion

        #region Member variables

        private bool previewMode = false;
        private bool fullScreenMode = true;
        private IntPtr parentHwnd = new IntPtr(0);

        private Random random;

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

        public ScreenSaverForm(bool fullScreenMode)
        {
            InitializeComponent();

            random = RandomFactory.CreateRandom();

            this.fullScreenMode = fullScreenMode;
            this.ShowInTaskbar = false;
            this.DoubleBuffered = false;

            // Initially, the (optinally) displayed controls should be invisible until the maze has been built.
            this.outerInfoPanel.Visible = false;

            if (RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_PAINT_ALL_WALLS, true) == false)
            {
                this.mazeUserControl.RandomizeWallVisibility = true;
            }
        }

        /// <summary>
        /// Set up the main form as a full screen screensaver.
        /// </summary>
        private void SetupScreenSaver()
        {
            // Use double buffering to improve drawing performance
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            
            // Set the application to full screen mode and hide the mouse
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.TopMost = true;
            this.Bounds = Screen.PrimaryScreen.Bounds;
            this.WindowState = FormWindowState.Maximized;
            Cursor.Hide();

            // Make this the active Form and capture the mouse
            this.Activate();
            this.Capture = true;
        }

        #region Preview mode constructor

        private struct RECT
        {
            public int left, top, right, bottom;
        }

        public ScreenSaverForm(string windowHandleArg)
        {
            InitializeComponent();

            // set the preview mode flag
            previewMode = true;
            fullScreenMode = false;
            ShowInTaskbar = false;

            // prevent an 
            //outerInfoPanel = null;

            parentHwnd = (IntPtr)UInt32.Parse(windowHandleArg);

            RECT rect = new RECT();

            // Let the mazeUserControl paint into the parent window's graphics.
            if (GetClientRect(parentHwnd, ref rect))
            {
                Graphics g = Graphics.FromHwnd(parentHwnd);
                mazeUserControl.SetGraphics(g, new Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top));

                // Hide the form.
                this.Hide();
                WindowState = FormWindowState.Minimized;

                // Create the first maze and allocate a graphics buffer.
                mazeUserControl.Setup(5, 2, 3);
                mazeUserControl.MazePainter.PaintMaze(null); // TODO: This is probably executed twice.
            }
        }

        #endregion

        #endregion

        #region Event handlers

        private void ScreenSaverForm_Load(object sender, EventArgs e)
        {
            if (!previewMode)
            {
                if (fullScreenMode)
                {
                    SetupScreenSaver();
                }
            }

            // Switch auto repeat mode on.
            ariadneController.RepeatMode = true;

            if (! previewMode)
            {
                if (fullScreenMode)
                {
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
            }

            // Select the strategyComboBox's item that chooses a random strategy.
            strategyComboBox.SelectedItem = "(any)";

            this.OnNew(null, null); // TODO: remove this message
            this.OnStart(null, null);
        }

        public override void OnNew(object sender, EventArgs e)
        {
            // Discard this call until ScreenSaverForm_Load() has been executed.
            // Note: The repeatMode flag is initially false and will be set above.
            if (ariadneController.RepeatMode == false)
            {
                return;
            }

            // Quit if dialog is dismissed.  Check this periodically.
            if (previewMode && !IsWindowVisible(parentHwnd))
            {
                Application.Exit();
            }

            base.OnNew(sender, e); // TODO: remove this call

            // Choose new locations of controls, before the maze is built.
            if (!previewMode)
            {
                if (visibleControls.Count == 0)
                {
                    PreparePlaceholderControls();
                }
            }

            // Build and display the new maze.
            base.OnNew(sender, e);

            // Place visible controls at the locations determined for their placeholders.
            if (!previewMode)
            {
                for (int i = 0; i < visibleControls.Count; i++)
                {
                    Control control = visibleControls[i];
                    Control placeholder = placeholderControls[i];

                    control.Location = placeholder.Location;
                    control.Visible = true;
                    
                    this.Controls.Remove(placeholder);
                }
            }
        }

        private void ScreenSaverForm_KeyDown(object sender, KeyEventArgs e)
        {
            Close();
        }

        private void ScreenSaverForm_MouseDown(object sender, MouseEventArgs e)
        {
            Close();
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
                if (!previewMode)
                {
                    this.PrepareImages();
                    this.PreparePlaceholderControls();
                }
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

            if (!previewMode)
            {
                // Images.
                if (!mazeUserControl.HasPreparedImages)
                {
                    this.PrepareImages();
                }
                mazeUserControl.ReserveAreaForImages();

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
                if (RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_IRREGULAR_MAZES, false) && random.Next(100) < 10)
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
            int count = RegisteredOptions.GetIntSetting(RegisteredOptions.OPT_IMAGE_NUMBER, 0);
            int minSize = RegisteredOptions.GetIntSetting(RegisteredOptions.OPT_IMAGE_MIN_SIZE, 300);
            int maxSize = RegisteredOptions.GetIntSetting(RegisteredOptions.OPT_IMAGE_MAX_SIZE, 400);
            string imageFolder = RegisteredOptions.GetStringSetting(RegisteredOptions.OPT_IMAGE_FOLDER);

            mazeUserControl.PrepareImages(count, minSize, maxSize, imageFolder);
        }

        /// <summary>
        /// Add an embedded maze.
        /// </summary>
        private bool AddEmbeddedMaze()
        {
            int percentage = (RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_MULTIPLE_MAZES, false) ? 10 : 0);
            Random r = RandomFactory.CreateRandom();
            if (r.Next(100) < percentage)
            {
                OutlineShape shape = null;
                int area = mazeUserControl.Maze.XSize * mazeUserControl.Maze.YSize;
                int minArea = (int)(0.1 * area), maxArea = (int)(0.5 * area);

                while (true)
                {
                    shape = OutlineShape.RandomInstance(r, mazeUserControl.Maze.XSize, mazeUserControl.Maze.YSize, 0.2, 1.0);

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
            int percentage = (RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_OUTLINE_SHAPES, false) ? 66 : 0);
            Random r = RandomFactory.CreateRandom();
            if (r.Next(100) < percentage)
            {
                OutlineShape shape = OutlineShape.RandomInstance(r, mazeUserControl.Maze.XSize, mazeUserControl.Maze.YSize, 0.3, 0.7);
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

                FillCaption(caption);

                this.infoLabelCaption.Text = caption.ToString();
            }
        }

        #endregion
    }
}