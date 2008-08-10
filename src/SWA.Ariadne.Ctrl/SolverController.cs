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
        private MazePainter mazePainter;

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
        public virtual long CountSteps
        {
            get
            {
                long result = countSteps;

                foreach (SolverController item in embeddedControllers)
                {
                    if (RunParallelSolvers)
                    {
                        // TODO: consider steps before item has really started
                        result = Math.Max(result, item.CountSteps);
                    }
                    else
                    {
                        result += item.CountSteps;
                    }
                }

                return result;
            }
        }

        public Maze Maze
        {
            get { return mazePainter.Maze; }
        }

        /// <summary>
        /// Returns true if the maze has been solved.
        /// </summary>
        public bool IsFinished
        {
            get
            {
                if (mazePainter.HasBufferAlternate)
                {
                    // When the mazePainter has prepared an alternate buffer, its maze is already the new maze.
                    // TODO: Avoid this quirk.
                    return true;
                }
                else if (Maze != null)
                {
                    return Maze.IsFinished;
                }
                else
                {
                    return false;
                }
            }
        }

        public int BlinkingCounter
        {
            get { return mazePainter.BlinkingCounter; }
            set { mazePainter.BlinkingCounter = value; }
        }

        /// <summary>
        /// The square that was last visited in backward direction.
        /// </summary>
        private MazeSquare currentBackwardSquare = null;

        /// <summary>
        /// Each EmbeddedMaze of our Maze gets its own EmbeddedSolverController.
        /// </summary>
        private List<EmbeddedSolverController> embeddedControllers = new List<EmbeddedSolverController>();

        /// <summary>
        /// Determines which controller will do the next step.
        /// 0: this controller;
        /// >0: one of the embedded controllers
        /// <0: all controllers in parallel
        /// </summary>
        private int doStepTurn = 0;

        public bool RunParallelSolvers
        {
            get { return (this.doStepTurn < 0); }
        }

        /// <summary>
        /// Returns true if this controller is ready to execute another step.
        /// Doesn't consider the embedded controllers' state.
        /// </summary>
        public virtual bool IsActive
        {
            get { return (this.Maze.IsSolved == false); }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public SolverController(IMazeForm mazeForm, MazePainter mazePainter, ProgressBar visitedProgressBar)
        {
            this.mazeForm = mazeForm;
            this.mazePainter = mazePainter;
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
                solver = SolverFactory.CreateSolver(mazeForm.StrategyName, this.Maze, mazePainter);
            }
            else
            {
                solver = SolverFactory.CreateSolver(null, this.Maze, mazePainter);
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

            // After all controllers have been created, some resources need to be shared with the master controller.
            if (this.Maze.MazeId == MazeSquare.PrimaryMazeId)
            {
                CoordinateEmbeddedControllers(this);
            }
        }

        /// <summary>
        /// Create controllers for our maze's embedded mazes.
        /// </summary>
        private void CreateEmbeddedSolvers()
        {
            this.mazePainter.ClearSharedPainters();

            foreach (Maze embeddedMaze in Maze.EmbeddedMazes)
            {
                MazePainter embeddedPainter = this.mazePainter.CreateSharedPainter(embeddedMaze);
                embeddedPainter.BlinkingCounter = -1;
                EmbeddedSolverController embeddedController = new EmbeddedSolverController(this, embeddedPainter);
                this.embeddedControllers.Add(embeddedController);

                embeddedController.StartDelayRelativeDistance = 0.2 + 0.4 * Maze.Random.NextDouble(); ;
            }
        }

        /// <summary>
        /// Coordinate shared resources of embedded objects with the master object.
        /// E.g. all solvers should share a common DeadEndChecker.
        /// </summary>
        /// <param name="masterCtrl"></param>
        private void CoordinateEmbeddedControllers(SolverController masterCtrl)
        {
            if (masterCtrl != this)
            {
                // Use the same progress bar.
                this.visitedProgressBar = masterCtrl.visitedProgressBar;

                // Coordinate our solver with the master solver.
                this.solver.CoordinateWithMaster(masterCtrl.solver);
            }

            foreach (SolverController ctrl in embeddedControllers)
            {
                ctrl.CoordinateEmbeddedControllers(masterCtrl);
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
        /// Returns the number of steps actually executed.
        /// </summary>
        /// <returns></returns>
        public int DoStep()
        {
            int result = 0;

            if (RunParallelSolvers)
            {
                // All controllers run in parallel.
                // Forward the message to the embedded controllers.
                foreach (EmbeddedSolverController item in embeddedControllers)
                {
                    result += item.DoStep();
                }
            }
            else
            {
                SolverController ctrl = ChooseDueController();

                if (ctrl != this)
                {
                    return ctrl.DoStep();
                }
            }

            if (this.Maze.IsSolved)
            {
                return result;
            }

            MazeSquare sq1, sq2;
            bool forward;

            solver.Step(out sq1, out sq2, out forward);
            mazePainter.DrawStep(sq1, sq2, forward);

            if (forward)
            {
                ++countForward;
                if (visitedProgressBar != null)
                {
                    visitedProgressBar.PerformStep(); // next visited square
                }

                // Let all embedded controllers know how far we have advanced.
                foreach (EmbeddedSolverController ctrl in embeddedControllers)
                {
                    ctrl.HostStep(sq2);
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
                mazePainter.DrawSolvedPath(solutionPath);
                currentBackwardSquare = null;

                if (this.Maze.MazeId != MazeSquare.PrimaryMazeId)
                {
                    mazePainter.DrawRemainingSquares();
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the controller who should execute the next step.
        /// Finished and not ready controllers are skipped.
        /// Increments this.doStepTurn.
        /// </summary>
        /// <returns></returns>
        private SolverController ChooseDueController()
        {
            if (embeddedControllers.Count == 0)
            {
                return this;
            }

            SolverController result = null;

            for (int n = 0; n <= embeddedControllers.Count; n++)
            {
                result = (doStepTurn == 0 ? this : embeddedControllers[doStepTurn - 1]);
                doStepTurn = (doStepTurn + 1) % (embeddedControllers.Count + 1);

                if (result.IsActive)
                {
                    break;
                }
            }

            return result;
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

            mazePainter.FinishPath(currentBackwardSquare);
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
                string steps = (CountSteps == 1 ? "step" : "steps");
                message.Append(CountSteps.ToString("#,##0") + " " + steps);

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
                string result = null;

                if (this.solver != null)
                {
                    result = (this.solver.IsEfficientSolver ? SolverFactory.EfficientPrefix : "") + this.solver.GetType().Name;
                }

                return result;
            }
        }

        #endregion
    }
}