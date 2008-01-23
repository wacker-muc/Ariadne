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

            // Adapt the progress bar to the maze area
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
        }

        public string StrategyName
        {
            get { return this.strategyComboBox.SelectedItem.ToString(); }
        }

        #endregion
    }
}
