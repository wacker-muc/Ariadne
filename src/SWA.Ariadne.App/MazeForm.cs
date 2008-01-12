using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SWA.Ariadne.Model;
using SWA.Ariadne.Logic;

namespace SWA.Ariadne.App
{
    /// <summary>
    /// A Windows Form that displays a Maze in a MazeUserControl.
    /// </summary>
    public partial class MazeForm : AriadneFormBase
        , IMazeForm
    {
        #region Member variables

        /// <summary>
        /// The object that accepts the MazeControl commands.
        /// </summary>
        protected override IMazeControl Control
        {
            get { return this.mazeUserControl as IMazeControl; }
        }

        /// <summary>
        /// The type of solver algorithm we will use.
        /// </summary>
        private System.Type strategy = SolverFactory.DefaultStrategy;

        /// <summary>
        /// A dictionary used by the strategyComboBox.
        /// </summary>
        private readonly Dictionary<string, System.Type> strategies;

        /// <summary>
        /// The maze solver algorithm.
        /// This is only set (not null) while we are in a solving mode.
        /// </summary>
        private IMazeSolver solver;

        /// <summary>
        /// Number of executed steps: in forward and backward direction.
        /// </summary>
        private long countForward, countBackward;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public MazeForm()
        {
            InitializeComponent();

            #region Unhide the non-common controls of AriadneFormBase we want to use

            strategyComboBox.Visible = true;
            visitedProgressBar.Visible = true;

            #endregion

            #region Fill the strategyComboBox with all known MazeSolvers

            strategies = new Dictionary<string, System.Type>();

            strategyComboBox.Items.Clear();

            foreach (System.Type t in SolverFactory.SolverTypes)
            {
                // Add the solver's name to the combo box.
                strategyComboBox.Items.Add(t.Name);

                // Add the solver's type and name to a Dictionary -- see strategy_Validated().
                strategies.Add(t.Name, t);

                // Note: Instead of a string we could add the Type object directly.
                // But the Type's ToString() method returns the FullName instead of the short Name.

                // Pre-select the default solver strategy.
                if (t == strategy)
                {
                    strategyComboBox.SelectedIndex = strategyComboBox.Items.Count - 1;
                }
            }

            #endregion

            this.OnNew(null, null);
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
            if (State != SolverState.Ready)
            {
                base.OnReset(sender, e);

                solver = null;
                visitedProgressBar.Value = 0;
            }

            ResetCounters();

            this.mazeUserControl.Reset();
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

        #region Solver controls

        /// <summary>
        /// Start a solver.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnStart(object sender, EventArgs e)
        {
            if (State != SolverState.Ready)
            {
                //OnReset(sender, e);
                return;
            }

            solver = SolverFactory.CreateSolver(strategy, mazeUserControl.Maze, mazeUserControl);

            base.OnStart(sender, e);
        }

        #endregion

        #region Parameter settings

        protected override void strategy_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.strategy = strategies[(string)strategyComboBox.SelectedItem];
            UpdateCaption();
        }

        #endregion

        #endregion

        #region IMazeForm implementation

        /// <summary>
        /// Place reserved areas into the maze.
        /// This method is called from the MazeUserControl before actually building the maze.
        /// </summary>
        /// <param name="maze"></param>
        public override void MakeReservedAreas(Maze maze)
        {
#if false
            Random r = new Random();
            while (maze.ReserveRectangle(r.Next(2, 8), r.Next(2, 8)))
            {
            }
#endif
        }

        #endregion

        #region AriadneFormBase implementation


        /// <summary>
        /// Advance a single step.
        /// The travelled steps are not rendered until FinishPath() is called.
        /// </summary>
        protected override void DoStep()
        {
            _currentSquare = SingleStep();
        }

        /// <summary>
        /// Used by DoStep() and FinishPath().
        /// </summary>
        private MazeSquare _currentSquare = null;

        /// <summary>
        /// Renders the path travelled so far.
        /// </summary>
        protected override void FinishPath()
        {
            mazeUserControl.FinishPath(_currentSquare);
            _currentSquare = null;
        }

        /// <summary>
        /// Write state information to the given StringBuilder.
        /// Derived classes should call their base class' method.
        /// </summary>
        /// <param name="message"></param>
        protected override void FillStatusMessage(StringBuilder message)
        {
            if (countSteps > 0)
            {
                message.Append(countSteps.ToString("#,##0") + " steps, "
                    + countForward.ToString("#,##0") + " forward, "
                    + countBackward.ToString("#,##0") + " backward"
                    );
            }

            // Add text from the base class.
            base.FillStatusMessage(message);
        }

        #endregion

        #region Auxiliary methods

        /// <summary>
        /// Reset step and runtime counters.
        /// </summary>
        private void ResetCounters()
        {
            countForward = countBackward = 0;
            visitedProgressBar.PerformStep(); // start square
        }

        /// <summary>
        /// Enables or disables some controls depending on whether we are Ready or not.
        /// </summary>
        private void FixStateDependantControls()
        {
            bool enabled = (State == SolverState.Ready);

            strategyComboBox.Enabled = enabled;
        }

        /// <summary>
        /// Executes one step in the solver and paints that section of the path.
        /// </summary>
        /// <returns>either null OR the square this step travelled to in backward direction</returns>
        private MazeSquare SingleStep()
        {
            if (State == SolverState.Finished)
            {
                return null;
            }

            MazeSquare sq1, sq2;
            bool forward;

            solver.Step(out sq1, out sq2, out forward);
            mazeUserControl.DrawStep(sq1, sq2, forward);

            if (forward)
            {
                ++countForward;
                visitedProgressBar.PerformStep(); // next visited square
            }
            else
            {
                ++countBackward;
            }
            ++countSteps;

            return (forward ? null : sq2);
        }

        #endregion
    }
}