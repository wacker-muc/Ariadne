using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using SWA.Ariadne.Logic;
using SWA.Ariadne.Settings;
using SWA.Ariadne.Gui.Mazes;
using SWA.Ariadne.Ctrl;

namespace SWA.Ariadne.Gui
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
                this.MazeUserControl.MazePainter,
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

            SolverFactory.FillWithSolverTypes(strategyComboBox.Items);
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
            visitedProgressBar.Maximum = mazeUserControl.Maze.CountSquares;
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
                string result = null;

                if (result == null && this.SolverController != null)
                {
                    result = this.SolverController.StrategyName;
                }
                if (result == null && this.strategyComboBox != null && this.strategyComboBox.SelectedItem != null)
                {
                    result = this.strategyComboBox.SelectedItem.ToString();
                }

                return result;
            }
        }

        #endregion
    }
}
