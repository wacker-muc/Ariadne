using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SWA.Ariadne.Logic;
using SWA.Ariadne.Settings;
using SWA.Ariadne.Gui.Mazes;
using SWA.Ariadne.Ctrl;

namespace SWA.Ariadne.Gui
{
    /// <summary>
    /// A Windows Form that displays a Maze in a MazeUserControl.
    /// </summary>
    public partial class MazeForm : AriadneFormBase
    {
        #region Member variables

        /// <summary>
        /// The object that accepts the MazeControlProperties commands.
        /// </summary>
        protected override IMazeControlProperties MazeControlProperties
        {
            get { return this.mazeUserControl as IMazeControlProperties; }
        }

        /// <summary>
        /// The object that accepts the SolverController commands.
        /// </summary>
        protected override ISolverController SolverController
        {
            get
            {
                if (this.solverController == null)
                {
                    this.solverController = new SolverController(
                        this as IMazeForm,
                        this.mazeUserControl.MazePainter,
                        this.visitedProgressBar.Control as ProgressBar
                    );
                }
                return this.solverController;
            }
        }
        private SolverController solverController;

        /// <summary>
        /// The object that accepts the AriadneSettingsSource commands.
        /// </summary>
        protected override IAriadneSettingsSource AriadneSettingsSource
        {
            get { return (this.mazeUserControl as IAriadneSettingsSource); }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public MazeForm()
        {
            InitializeComponent();
            InitializeComponent2();
        }

        /// <summary>
        /// Continue after the designer generated code.
        /// </summary>
        private void InitializeComponent2()
        {
            this.mazeUserControl.MazeForm = this as IMazeForm;

            #region Unhide the non-common controls of AriadneFormBase we want to use

            strategyComboBox.Visible = true;
            visitedProgressBar.Visible = true;

            #endregion

            #region Fill the strategyComboBox with all known MazeSolvers

            SolverFactory.FillWithSolverTypes(strategyComboBox.Items);
            strategyComboBox.SelectedItem = SolverFactory.DefaultStrategy.Name;

            #endregion
        }

        #endregion

        #region Event handlers

        #region Maze controls

        /// <summary>
        /// Stops the solver and returns the maze to its original (unsolved) state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnReset(object sender, EventArgs e)
        {
            base.OnReset(sender, e);
            mazeUserControl.Reset();
        }

        /// <summary>
        /// Creates a new (different) maze.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void OnNew(object sender, EventArgs e)
        {
            base.OnNew(sender, e);
            mazeUserControl.Setup();
            ConfigureVisitedProgressBar();
        }

        /// <summary>
        /// Adapt the progress bar to the maze area.
        /// </summary>
        private void ConfigureVisitedProgressBar()
        {
            visitedProgressBar.Minimum = 0;
            visitedProgressBar.Maximum = mazeUserControl.Maze.CountSquares;
            visitedProgressBar.Step = 1;
        }

        #endregion

        #endregion

        #region AriadneFormBase implementation

        protected override void PrepareForNextStart(bool baseFirst)
        {
            if (baseFirst)
            {
                base.PrepareForNextStart(baseFirst);
                // more code
            }
            else
            {
                this.mazeUserControl.PrepareAlternateBuffer();
                base.PrepareForNextStart(baseFirst);
            }
        }

        /// <summary>
        /// Returns an image of the current maze, without the Form border.
        /// </summary>
        /// <returns></returns>
        protected override Image GetImage()
        {
            return this.mazeUserControl.GetImage();
        }

        #endregion

        #region IMazeForm implementation

        /// <summary>
        /// Reset step and runtime counters.
        /// </summary>
        public override void ResetCounters()
        {
            base.ResetCounters();

            ConfigureVisitedProgressBar();
        }

        #endregion
    }
}