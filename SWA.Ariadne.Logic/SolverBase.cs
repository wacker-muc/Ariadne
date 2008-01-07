using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model;

namespace SWA.Ariadne.Logic
{
    /// <summary>
    /// Base class of all MazeSolver classes.
    /// </summary>
    internal abstract class SolverBase : IMazeSolver
    {
        #region Member variables

        protected readonly Maze maze;

        #endregion

        #region Constructor

        protected SolverBase(Maze maze)
        {
            this.maze = maze;
            this.Reset();
        }

        #endregion

        #region IMazeSolver methods

        /// <summary>
        /// Reset to the initial state (before the maze is solved).
        /// Subclasses should call their base class' method first.
        /// </summary>
        public virtual void Reset()
        {
            // no action
        }

        /// <summary>
        /// Call the implementing method StepI().
        /// Subclasses may do additional bookkeeping.
        /// </summary>
        public virtual void Step(out MazeSquare sq1, out MazeSquare sq2, out bool forward)
        {
            StepI(out sq1, out sq2, out forward);
        }

        public abstract void StepI(out MazeSquare sq1, out MazeSquare sq2, out bool forward);

        #endregion

        #region Auxiliary methods for derived classes

        protected static List<MazeSquare.WallPosition> OpenWalls(MazeSquare sq, bool notVisitedOnly)
        {
            List<MazeSquare.WallPosition> result = new List<MazeSquare.WallPosition>(MazeSquare.WP_NUM);

            for (MazeSquare.WallPosition wp = MazeSquare.WP_MIN; wp <= MazeSquare.WP_MAX; wp++)
            {
                if (sq[wp] == MazeSquare.WallState.WS_OPEN)
                {
                    MazeSquare sq2 = sq.NeighborSquare(wp);

                    if (!notVisitedOnly || !sq2.isVisited)
                    {
                        result.Add(wp);
                    }
                }
            }

            return result;
        }

        #endregion
    }
}
