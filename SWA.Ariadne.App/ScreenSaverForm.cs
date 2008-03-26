using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SWA.Ariadne.Model;

namespace SWA.Ariadne.App
{
    public partial class ScreenSaverForm : MazeForm
    {
        #region Constructor

        public ScreenSaverForm()
        {
            InitializeComponent();
            SetupScreenSaver();
        }

        /// <summary>
        /// Set up the main form as a full screen screensaver.
        /// </summary>
        private void SetupScreenSaver()
        {
            // Use double buffering to improve drawing performance
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            // Capture the mouse
            this.Capture = true;

            // Set the application to full screen mode and hide the mouse
            Cursor.Hide();
            Bounds = Screen.PrimaryScreen.Bounds;
            WindowState = FormWindowState.Maximized;
            TopMost = true;

            ShowInTaskbar = false;
            DoubleBuffered = true;
        }

        #endregion

        #region Event handlers

        private void ScreenSaverForm_Load(object sender, EventArgs e)
        {
            // Switch auto repeat mode on.
            this.repeatMode = true;

            // Let the MazeUserControl cover the whole form.
            this.mazeUserControl.Location = new Point(0, 0);
            this.mazeUserControl.Size = this.Size;
            this.mazeUserControl.BringToFront();

            // Select the strategyComboBox's item that chooses a random strategy.
            strategyComboBox.SelectedItem = "(any)";

            this.OnNew(null, null);
            this.OnStart(null, null);
        }

        protected override void OnNew(object sender, EventArgs e)
        {
            // Place the info panel at a random position
            if (this.outerInfoPanel != null)
            {
                Random r = RandomFactory.CreateRandom();
                int xMin = this.Size.Width / 20;
                int yMin = this.Size.Height / 20;
                int xMax = this.Size.Width - xMin - this.outerInfoPanel.Size.Width;
                int yMax = this.Size.Height - yMin - this.outerInfoPanel.Size.Height;
                int x = r.Next(xMin, xMax);
                int y = r.Next(yMin, yMax);
                this.outerInfoPanel.Location = new Point(x, y);
                this.outerInfoPanel.BringToFront();
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
            mazeUserControl.ReserveArea(this.outerInfoPanel);
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