using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using SWA.Ariadne.Model;
using SWA.Ariadne.Logic;

namespace SWA.Ariadne.App
{
    /// <summary>
    /// A controller for a MazeSolver and a MazeControl.
    /// </summary>
    public class SolverController
        : ISolverController
    {
        #region Member variables

        /// <summary>
        /// The object knowing the selected solver strategy.
        /// </summary>
        private IMazeForm mazeForm;

        /// <summary>
        /// The control displaying the maze.
        /// </summary>
        private IMazeControl mazeControl;

        /// <summary>
        /// The control displaying the number of visited squares.
        /// </summary>
        private ProgressBar visitedProgressBar;

        /// <summary>
        /// The maze solver algorithm.
        /// This is only set (not null) while we are in a solving mode.
        /// </summary>
        private IMazeSolver solver;

        /// <summary>
        /// Number of executed steps: total and in forward and backward direction.
        /// </summary>
        private long countSteps, countForward, countBackward;

        /// <summary>
        /// Number of executed steps.
        /// </summary>
        public long CountSteps
        {
            get { return countSteps; }
        }

        /// <summary>
        /// The square that was last visited in backward direction.
        /// </summary>
        private MazeSquare currentBackwardSquare = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public SolverController(IMazeForm mazeForm, IMazeControl mazeControl, ProgressBar visitedProgressBar)
        {
            this.mazeForm = mazeForm;
            this.mazeControl = mazeControl;
            this.visitedProgressBar = visitedProgressBar;
        }

        #endregion

        #region Setup methods

        public void Reset()
        {
            solver = null;
            visitedProgressBar.Value = 0;
        }

        /// <summary>
        /// Reset step counters.
        /// </summary>
        public void ResetCounters()
        {
            countSteps = countForward = countBackward = 0;
            visitedProgressBar.PerformStep(); // start square
        }

        public void Start()
        {
            string strategyName = mazeForm.StrategyName;
            bool isEfficient = false;

            if (strategyName.StartsWith(SolverFactory.EfficientPrefix))
            {
                strategyName = strategyName.Substring(SolverFactory.EfficientPrefix.Length);
                isEfficient = true;
            }

            try
            {
                // If strategyName is a valid solver type name:
                Type strategy = SolverFactory.SolverType(strategyName);
                solver = SolverFactory.CreateSolver(strategy, mazeControl.Maze, mazeControl);
                if (isEfficient)
                {
                    solver.MakeEfficient();
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                // Otherwise (strategy name is "any"):
                solver = SolverFactory.CreateSolver(mazeControl.Maze, mazeControl);
            }
            this.mazeForm.UpdateCaption();
        }

        #endregion

        #region Solver methods

        /// <summary>
        /// Advance a single step.
        /// The travelled steps are not rendered until FinishPath() is called.
        /// </summary>
        public void DoStep()
        {
            if (mazeControl.Maze.IsSolved)
            {
                return;
            }

            MazeSquare sq1, sq2;
            bool forward;

            solver.Step(out sq1, out sq2, out forward);
            mazeControl.DrawStep(sq1, sq2, forward);

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

            currentBackwardSquare = (forward ? null : sq2);

            if (mazeControl.Maze.IsSolved)
            {
                FinishPath();
                mazeControl.DrawSolvedPath(SolverFactory.SolutionPath(mazeControl.Maze));
                currentBackwardSquare = null;
            }
        }

        /// <summary>
        /// Renders the path travelled so far.
        /// </summary>
        public void FinishPath()
        {
            mazeControl.FinishPath(currentBackwardSquare);
            currentBackwardSquare = null;
        }

        #endregion

        #region Status methods

        /// <summary>
        /// Displays information about the running MazeSolver in the status line.
        /// </summary>
        public void UpdateStatusLine()
        {
            mazeForm.UpdateStatusLine();
        }

        /// <summary>
        /// Write state information to the given StringBuilder.
        /// </summary>
        /// <param name="message"></param>
        public void FillStatusMessage(StringBuilder message)
        {
            if (countSteps > 0)
            {
                string steps = (countSteps == 1 ? "step" : "steps");
                message.Append(countSteps.ToString("#,##0") + " " + steps);

                if (countBackward > 0)
                {
                    message.Append(", "
                        + countForward.ToString("#,##0") + " forward, "
                        + countBackward.ToString("#,##0") + " backward"
                        );
                }

                solver.FillStatusMessage(message);
            }
        }

        public string StrategyName
        {
            get
            {
                return (this.solver.IsEfficientSolver ? SolverFactory.EfficientPrefix : "") + this.solver.GetType().Name;
            }
        }

        #endregion

        #region MazeControl methods

        public int BlinkingCounter
        {
            get
            {
                return mazeControl.BlinkingCounter;
            }
            set
            {
                mazeControl.BlinkingCounter = value;
            }
        }

        #endregion
    }
}