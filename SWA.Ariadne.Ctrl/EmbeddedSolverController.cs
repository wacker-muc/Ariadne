using System;
using System.Collections.Generic;
using System.Text;

namespace SWA.Ariadne.Ctrl
{
    /// <summary>
    /// An EmbeddedSolverController is applied for solving an EmbeddedMaze.
    /// </summary>
    internal class EmbeddedSolverController : SolverController
    {
        #region Member variables

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

        #endregion

        #region Constructor

        public EmbeddedSolverController(SolverController hostController, SWA.Ariadne.Logic.IMazeDrawer mazeDrawer)
            : base(null, mazeDrawer, null)
        {
            this.hostController = hostController;
            this.startDelayRelativeDistance = 1.0;
        }

        #endregion
    }
}
