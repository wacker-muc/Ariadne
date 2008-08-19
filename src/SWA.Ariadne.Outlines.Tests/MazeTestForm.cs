using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SWA.Ariadne.Gui.Mazes;
using SWA.Ariadne.Model;
using SWA.Ariadne.Settings;

namespace SWA.Ariadne.Outlines.Tests
{
    public partial class MazeTestForm : Form
        , IMazeForm
    {
        #region Delegates, Member Variables, Properties

        public delegate void MazeConfiguratorDelegate(Maze maze);

        protected MazeConfiguratorDelegate mazeConfigurator;

        IMazeControl mazeControl { get { return this.mazeUserControl as IMazeControl; } }

        #endregion

        #region Constructor

        public MazeTestForm(MazeConfiguratorDelegate mazeConfigurator)
        {
            InitializeComponent();

            this.mazeConfigurator = mazeConfigurator;

            mazeUserControl.MazeForm = this as IMazeForm;
        }

        /// <summary>
        /// Default constructor for derived clases.
        /// </summary>
        protected MazeTestForm()
            : this(null)
        {
        }

        private void BuildNewMaze()
        {
            AriadneSettingsData data = new AriadneSettingsData();
            mazeControl.FillParametersInto(data);

            data.AutoSeed = true;
            data.AutoMazeWidth = data.AutoMazeHeight = true;
            data.AutoSquareWidth = data.AutoWallWidth = data.AutoPathWidth = true;

            data.GridWidth = (int)this.gridWidthNumericUpDown.Value;
            data.AutoGridWidth = false;

            data.WallVisibility = (this.visibleCheckBox.Checked ? AriadneSettingsData.WallVisibilityEnum.Always : AriadneSettingsData.WallVisibilityEnum.Never);

            data.VisibleOutlines = true;
            data.AsEmbeddedMaze = false;
            
            mazeControl.TakeParametersFrom(data);
            //mazeControl.Setup();
        }

        #endregion

        #region Event handlers

        private void MazeTestForm_Load(object sender, EventArgs e)
        {
            mazeControl.Setup();
            BuildNewMaze();
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            BuildNewMaze();
        }

        private void mazeUserControl_MouseClick(object sender, MouseEventArgs e)
        {
            double centerX = (double)e.X / mazeUserControl.Width;
            double centerY = (double)e.Y / mazeUserControl.Height;

            int corners = 3;
            double distortionWinding = 0.25;
            mazeConfigurator = DistortedOutlineShapeTest.DistortedPolygonConfiguratorDelegate(corners, 1, 0, centerX, centerY, 0.8, distortionWinding);
            this.Text = string.Format(
                "DistortedOutlineShape: Polygon({0}) @ ({1:0.##}, {2:0.##}), Spiral({3:0.##})",
                corners, centerX, centerY, distortionWinding);
            
            BuildNewMaze();
        }

        #endregion

        #region IMazeForm Members

        public void MakeReservedAreas(Maze maze)
        {
            // At this point, the maze configurator can be called.
            if (this.mazeConfigurator != null)
            {
                this.mazeConfigurator(maze);
            }
        }

        public void UpdateStatusLine()
        {
        }

        public void UpdateCaption()
        {
        }

        public string StrategyName
        {
            get { return null; }
        }

        #endregion
    }
}