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
        #region Member variables and properties

        /// <summary>
        /// The problem to be solved.
        /// </summary>
        protected readonly Maze maze;

        /// <summary>
        /// An object that will draw the path while we are solving it.
        /// </summary>
        protected IMazeDrawer mazeDrawer;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="maze"></param>
        /// <param name="mazeDrawer"></param>
        protected SolverBase(Maze maze, IMazeDrawer mazeDrawer)
        {
            this.maze = maze;
            this.mazeDrawer = mazeDrawer;
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
        /// Travel from one visited square to a neighbor square (through an open wall).
        /// Wrapper. Calls the implementing method StepI().
        /// Subclasses may do additional bookkeeping.
        /// </summary>
        /// <param name="sq1">first (previously visited) square</param>
        /// <param name="sq2">next (neighbor) square</param>
        /// <param name="forward">true if the neighbor square was not visited previously</param>
        public virtual void Step(out MazeSquare sq1, out MazeSquare sq2, out bool forward)
        {
            StepI(out sq1, out sq2, out forward);
        }

        /// <summary>
        /// Travel from one visited square to a neighbor square (through an open wall).
        /// Implementation of Step().
        /// </summary>
        /// <param name="sq1">first (previously visited) square</param>
        /// <param name="sq2">next (neighbor) square</param>
        /// <param name="forward">true if the neighbor square was not visited previously</param>
        protected abstract void StepI(out MazeSquare sq1, out MazeSquare sq2, out bool forward);

        /// <summary>
        /// Find a path in the maze from the start to the end point.
        /// </summary>
        public void Solve()
        {
            MazeSquare sq1, sq2 = null;
            bool forward;

            while (!maze.IsSolved)
            {
                this.Step(out sq1, out sq2, out forward);
                if (mazeDrawer != null)
                {
                    mazeDrawer.DrawStep(sq1, sq2, forward);
                }
            }
        }

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
