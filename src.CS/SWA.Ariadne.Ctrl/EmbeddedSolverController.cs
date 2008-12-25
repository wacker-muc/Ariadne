using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model;

namespace SWA.Ariadne.Ctrl
{
    /// <summary>
    /// An EmbeddedSolverController is applied for solving an EmbeddedMaze.
    /// </summary>
    internal class EmbeddedSolverController : SolverController
    {
        #region Member variables and properties

        private SolverController hostController;

        /// <summary>
        /// This controller delays its Start event until the hostController's solver
        /// gets closer than this distance to its target square.
        /// Measured as a fraction of the Maze diagonal.
        /// 1: start immediately.
        /// 0: start when the host maze is solved.
        /// </summary>
        public double StartDelayRelativeDistance
        {
            get { return startDelayRelativeDistance; }
            set { startDelayRelativeDistance = value; }
        }
        private double startDelayRelativeDistance;

        /// <summary>
        /// Returns true if this controller is ready to execute another step.
        /// Doesn't consider the embedded controllers' state.
        /// </summary>
        public override bool IsActive
        {
            get
            {
                if (!base.IsActive)
                {
                    // If the maze is finished, we are certainly not active.
                    return false;
                }
                if (!this.waitingForStart)
                {
                    // The maze is not finished and we are not waiting for our start condition.
                    return true;
                }
                if (!hostController.IsActive)
                {
                    // If our host controller is not active anymore, we cannot wait any longer.
                    this.SetActive();
                }
                return (this.waitingForStart == false);
            }
        }
        private void SetActive()
        {
            if (this.waitingForStart == true)
            {
                this.waitingForStart = false;
                this.skippedSteps = hostController.CountSteps;
            }
        }
        private bool waitingForStart = true;

        /// <summary>
        /// Number of steps that were skipped until the solver was really started.
        /// </summary>
        public long SkippedSteps
        {
            get { return this.skippedSteps; }
        }
        private long skippedSteps = 0;

        #endregion

        #region Constructor

        public EmbeddedSolverController(SolverController hostController, SWA.Ariadne.Gui.Mazes.MazePainter mazePainter)
            : base(null, mazePainter, null)
        {
            this.hostController = hostController;
            this.startDelayRelativeDistance = 1.0;
        }

        #endregion

        /// <summary>
        /// Called whenever the host controller advances to a new square.
        /// May switch the embedded solver from inactive to active.
        /// </summary>
        /// <param name="sq2"></param>
        internal void HostStep(MazeSquare sq)
        {
            if (IsActive == true)
            {
                return;
            }

            double dx = sq.XPos - hostController.Maze.EndSquare.XPos;
            double dy = sq.YPos - hostController.Maze.EndSquare.YPos;
            double distance = Math.Sqrt(dx * dx + dy * dy);
            double diagonal = Math.Sqrt(Maze.XSize * Maze.XSize + Maze.YSize * Maze.YSize);
            double startDelayDistance = this.StartDelayRelativeDistance * diagonal;

            if (distance <= startDelayDistance)
            {
                this.SetActive();
            }
        }
    }
}
