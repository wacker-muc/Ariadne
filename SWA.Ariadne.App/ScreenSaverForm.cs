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

namespace SWA.Ariadne.App
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
        private IntPtr parentHwnd = new IntPtr(0);

        #endregion

        #region Constructor

        public ScreenSaverForm()
        {
            InitializeComponent();

            this.ShowInTaskbar = false;
            this.DoubleBuffered = true;
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
                mazeUserControl.PaintMaze();
            }
        }

        #endregion

        #endregion

        #region Event handlers

        private void ScreenSaverForm_Load(object sender, EventArgs e)
        {
            if (!previewMode)
            {
                SetupScreenSaver();
            }

            // Switch auto repeat mode on.
            this.repeatMode = true;

            if (! previewMode)
            {
                // Let the MazeUserControl cover the whole form.
                this.mazeUserControl.Location = new Point(0, 0);
                this.mazeUserControl.Size = this.Size;
                this.mazeUserControl.BringToFront();
            }

            // Select the strategyComboBox's item that chooses a random strategy.
            strategyComboBox.SelectedItem = "(any)";

            this.OnNew(null, null);
            this.OnStart(null, null);
        }

        protected override void OnNew(object sender, EventArgs e)
        {
            // TODO: Quit if dialog is dismissed.  Check this periodically.
            if (previewMode && !IsWindowVisible(parentHwnd))
            {
                Application.Exit();
            }

            // Place the info panel at a random position
            if (this.outerInfoPanel != null && !previewMode)
            {
                Random r = RandomFactory.CreateRandom();
                int xMin = this.Size.Width / 20;
                int yMin = this.Size.Height / 20;
                int xMax = this.Size.Width - xMin - this.outerInfoPanel.Size.Width;
                int yMax = this.Size.Height - yMin - this.outerInfoPanel.Size.Height;
                int x = r.Next(xMin, xMax);
                int y = r.Next(yMin, yMax);
                this.outerInfoPanel.Location = new Point(x, y);
            }

            base.OnNew(sender, e);
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

        #region IMazeForm implementation

        /// <summary>
        /// Place reserved areas into the maze.
        /// This method is called from the MazeUserControl before actually building the maze.
        /// </summary>
        /// <param name="maze"></param>
        public override void MakeReservedAreas(Maze maze)
        {
            if (!previewMode && OptionsDialog.GetBoolSetting(OptionsDialog.OPT_SHOW_DETAILS_BOX))
            {
                mazeUserControl.ReserveArea(this.outerInfoPanel);
                this.outerInfoPanel.BringToFront();
            }
            else
            {
                this.outerInfoPanel.SendToBack();
            }
        }

        /// <summary>
        /// Displays information about the running MazeSolver in the status line.
        /// </summary>
        public override void UpdateStatusLine()
        {
            if (this.infoLabelStatus != null)
            {
                StringBuilder message = new StringBuilder(200);

                FillStatusMessage(message);

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