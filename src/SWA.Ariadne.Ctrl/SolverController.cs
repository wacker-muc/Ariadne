using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using SWA.Ariadne.Model;
using SWA.Ariadne.Logic;
using SWA.Ariadne.Gui.Mazes;

namespace SWA.Ariadne.Ctrl
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
        /// The MazePainter.
        /// </summary>
        private IMazeDrawer mazeDrawer;

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
        /// The path that will be highlighted when the solution is complete.
        /// </summary>
        private List<MazeSquare> solutionPath;

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

        private Maze Maze
        {
            get { return mazeDrawer.Maze; }
        }

        /// <summary>
        /// The square that was last visited in backward direction.
        /// </summary>
        private MazeSquare currentBackwardSquare = null;

        /// <summary>
        /// Each EmbeddedMaze of our Maze gets its own EmbeddedSolverController.
        /// </summary>
        private List<EmbeddedSolverController> embeddedControllers = new List<EmbeddedSolverController>();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public SolverController(IMazeForm mazeForm, IMazeDrawer mazeDrawer, ProgressBar visitedProgressBar)
        {
            this.mazeForm = mazeForm;
            this.mazeDrawer = mazeDrawer;
            this.visitedProgressBar = visitedProgressBar;
        }

        #endregion

        #region Setup methods

        public void Reset()
        {
            ReleaseResources();

            if (visitedProgressBar != null)
            {
                visitedProgressBar.Value = 0;
            }
        }

        /// <summary>
        /// When the controller is Ready or Finished, memory consuming resources should be deallocated.
        /// </summary>
        public void ReleaseResources()
        {
            solver = null;
            solutionPath = null;
            embeddedControllers.Clear();
        }

        /// <summary>
        /// Reset step counters.
        /// </summary>
        public void ResetCounters()
        {
            countSteps = countForward = countBackward = 0;
            if (visitedProgressBar != null)
            {
                visitedProgressBar.PerformStep(); // start square
            }

            // Prepare embedded solvers.
            foreach (EmbeddedSolverController item in embeddedControllers)
            {
                item.ResetCounters();
            }
        }

        /// <summary>
        /// Complex and memory consuming resources may be prepared before the controller is started.
        /// </summary>
        public void PrepareForStart()
        {
            // Prepare the solver.
            if (mazeForm != null)
            {
                solver = SolverFactory.CreateSolver(mazeForm.StrategyName, this.Maze, mazeDrawer);
            }
            else
            {
                solver = SolverFactory.CreateSolver(this.Maze, mazeDrawer);
            }

            // Prepare the solution path.
            solutionPath = SolverFactory.SolutionPath(this.Maze);

            // If our Maze has embedded mazes, we need to supply them with embedded solvers.
            CreateEmbeddedSolvers();

            // Prepare embedded solvers.
            foreach (EmbeddedSolverController item in embeddedControllers)
            {
                item.PrepareForStart();
            }
        }

        /// <summary>
        /// Create controllers for our maze's embedded mazes.
        /// </summary>
        private void CreateEmbeddedSolvers()
        {
            foreach (Maze embeddedMaze in Maze.EmbeddedMazes)
            {
                MazePainter thisPainter = this.mazeDrawer as MazePainter; // TODO: avoid this upcast
                MazePainter embeddedPainter = thisPainter.CreateSharedPainter(embeddedMaze);
                EmbeddedSolverController embeddedController = new EmbeddedSolverController(this, embeddedPainter);
                this.embeddedControllers.Add(embeddedController);

                // TODO: use random values
                embeddedController.StartDelayRelativeDistance = 1.0;
            }
        }

        public void Start()
        {
            if (solver == null)
            {
                PrepareForStart();
            }
            if (mazeForm != null)
            {
                mazeForm.UpdateCaption();
            }
        }

        #endregion

        #region Solver methods

        /// <summary>
        /// Advance a single step.
        /// The travelled steps are not rendered until FinishPath() is called.
        /// </summary>
        public void DoStep()
        {
            if (this.Maze.IsSolved)
            {
                return;
            }

            // Forward the message to the embedded controllers.
            foreach (EmbeddedSolverController item in embeddedControllers)
            {
                item.DoStep();
            }

            MazeSquare sq1, sq2;
            bool forward;

            solver.Step(out sq1, out sq2, out forward);
            mazeDrawer.DrawStep(sq1, sq2, forward);

            if (forward)
            {
                ++countForward;
                if (visitedProgressBar != null)
                {
                    visitedProgressBar.PerformStep(); // next visited square
                }
            }
            else
            {
                ++countBackward;
            }
            ++countSteps;

            currentBackwardSquare = (forward ? null : sq2);

            if (this.Maze.IsSolved)
            {
                FinishPath();
                mazeDrawer.DrawSolvedPath(solutionPath);
                currentBackwardSquare = null;
            }
        }

        /// <summary>
        /// Renders the path travelled so far.
        /// </summary>
        public void FinishPath()
        {
            // Forward the message to the embedded controllers.
            foreach (EmbeddedSolverController item in embeddedControllers)
            {
                item.FinishPath();
            }

            mazeDrawer.FinishPath(currentBackwardSquare);
            currentBackwardSquare = null;
        }

        #endregion

        #region Status methods

        /// <summary>
        /// Displays information about the running MazeSolver in the status line.
        /// </summary>
        public void UpdateStatusLine()
        {
            if (mazeForm != null)
            {
                mazeForm.UpdateStatusLine();
            }
        }

        /// <summary>
        /// Write state information to the given StringBuilder.
        /// </summary>
        /// <param name="message"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int64.ToString(System.String)")]
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

                if (solver != null)
                {
                    solver.FillStatusMessage(message);
                }
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
                return mazeDrawer.BlinkingCounter;
            }
            set
            {
                mazeDrawer.BlinkingCounter = value;
            }
        }

        #endregion
    }
}