using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SWA.Ariadne.Model;
using SWA.Ariadne.Logic;
using SWA.Ariadne.Settings;

namespace SWA.Ariadne.App
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
            get { return this.solverController; }
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

            // Create a SolverController.
            this.solverController = new SolverController(
                this as IMazeForm,
                this.mazeUserControl as IMazeControl,
                this.visitedProgressBar.Control as ProgressBar
            );

            this.OnNew(null, null);
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

            strategyComboBox.Items.Clear();

            foreach (System.Type t in SolverFactory.SolverTypes)
            {
                // Add the solver's name to the combo box.
                strategyComboBox.Items.Add(t.Name);
            }
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
            this.SolverController.Reset();
            mazeUserControl.Reset();
        }

        /// <summary>
        /// Creates a new (different) maze.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnNew(object sender, EventArgs e)
        {
            if (State != SolverState.Ready)
            {
                OnReset(sender, e);
            }

            mazeUserControl.Setup();

            // Adapt the progress bar to the maze area
            visitedProgressBar.Minimum = 0;
            visitedProgressBar.Maximum = mazeUserControl.Maze.XSize * mazeUserControl.Maze.YSize;
            visitedProgressBar.Step = 1;
        }

        #endregion

        #region Parameter settings

        protected override void strategy_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateCaption();
        }

        #endregion

        #endregion

        #region IMazeForm implementation

        /// <summary>
        /// Enables or disables some controls depending on whether we are Ready or not.
        /// </summary>
        public override void FixStateDependantControls(SolverState state)
        {
            bool enabled = (State == SolverState.Ready);

            strategyComboBox.Enabled = enabled;
        }

        #endregion
    }
}