using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using SWA.Ariadne.Logic;
using SWA.Ariadne.Settings;

namespace SWA.Ariadne.App
{
    public partial class ArenaItem : UserControl
        //, IAriadneSettingsSource
        //, IMazeControl
        //, ISolverController
        , IMazeControlSetup
        , IMazeForm
    {
        #region Member variables and properties

        /// <summary>
        /// Gets the MazeUserControl.
        /// </summary>
        public MazeUserControl MazeUserControl
        {
            get { return this.mazeUserControl; }
        }

        /// <summary>
        /// The object that accepts the SolverController commands.
        /// </summary>
        public ISolverController SolverController
        {
            get { return this.solverController; }
        }

        /// <summary>
        /// The SolverController.
        /// </summary>
        private SolverController solverController;

        #endregion

        #region Constructor

        public ArenaItem()
        {
            InitializeComponent();
            InitializeComponent2();

            // Create a SolverController.
            this.solverController = new SolverController(
                this as IMazeForm,
                this.MazeUserControl as IMazeControl,
                this.visitedProgressBar as ProgressBar
            );
        }

        /// <summary>
        /// Continue after the designer generated code.
        /// </summary>
        private void InitializeComponent2()
        {
            this.mazeUserControl.MazeForm = this as IMazeForm;

            #region Fill the strategyComboBox with all known MazeSolvers

            strategyComboBox.Items.Clear();

            foreach (System.Type t in SolverFactory.SolverTypes)
            {
                // Add the solver's name to the combo box.
                strategyComboBox.Items.Add(t.Name);
            }
            foreach (System.Type t in SolverFactory.SolverTypes)
            {
                if (SolverFactory.HasEfficientVariant(t))
                {
                    // Add the solver's name to the combo box.
                    strategyComboBox.Items.Add(SolverFactory.EfficientPrefix + t.Name);
                }
            }
            strategyComboBox.Items.Add("(any)");
            strategyComboBox.SelectedItem = SolverFactory.DefaultStrategy.Name;

            #endregion
        }

        #endregion

        #region Event handlers

        #endregion

        #region IMazeControlSetup implementation

        /// <summary>
        /// Reset to the initial state (before the maze is solved).
        /// </summary>
        public void Reset()
        {
            mazeUserControl.Reset();
        }

        public void Setup()
        {
            Setup(true);
        }

        public void Setup(bool includingMazeControl)
        {
            if (includingMazeControl)
            {
                mazeUserControl.Setup();
            }

            ConfigureVisitedProgressBar();
        }

        /// <summary>
        /// Adapt the progress bar to the maze area.
        /// </summary>
        internal void ConfigureVisitedProgressBar()
        {
            visitedProgressBar.Minimum = 0;
            visitedProgressBar.Maximum = mazeUserControl.Maze.XSize * mazeUserControl.Maze.YSize;
            visitedProgressBar.Step = 1;
        }

        #endregion

        #region IMazeForm implementation

        public void MakeReservedAreas(SWA.Ariadne.Model.Maze maze)
        {
            // no action
        }

        public void UpdateStatusLine()
        {
            StringBuilder message = new StringBuilder(200);

            SolverController.FillStatusMessage(message);

            this.statusLabel.Text = message.ToString();
        }

        public void UpdateCaption()
        {
            // no action
            if (this.strategyComboBox.Text.StartsWith("("))
            {
                this.strategyComboBox.Text = "(" + this.StrategyName + ")";
            }
            else
            {
                this.strategyComboBox.Text = this.StrategyName;
            }
        }

        public string StrategyName
        {
            get
            {
                try
                {
                    return this.SolverController.StrategyName;
                }
                catch (NullReferenceException)
                {
                    return this.strategyComboBox.SelectedItem.ToString();
                }
            }
        }

        /// <summary>
        /// Enables or disables some controls depending on whether we are Ready or not.
        /// </summary>
        public void FixStateDependantControls(AriadneFormBase.SolverState state)
        {
            bool enabled = (state == AriadneFormBase.SolverState.Ready
                         || state == AriadneFormBase.SolverState.Finished);

            strategyComboBox.Enabled = enabled;
        }

        #endregion
    }
}
