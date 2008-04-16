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

        /// <summary>
        /// Some ("efficient") subclasses may make use of a DeadEndChecker to avoid certain areas.
        /// </summary>
        protected DeadEndChecker deadEndChecker = null;

        public void MakeEfficient()
        {
            // An efficient MazeSolver has a DeadEndChecker.
            deadEndChecker = new DeadEndChecker(maze);
        }

        /// <summary>
        /// Returns true if this MazeSolver can detect areas unreachable the end square.
        /// </summary>
        public bool IsEfficientSolver
        {
            get { return (deadEndChecker != null); }
        }

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

            #region Apply the dead end checker.
            if (deadEndChecker != null && mazeDrawer != null)
            {
#if false
                // TODO: disable this debug version
                List<MazeSquare> confirmedSquares = new List<MazeSquare>();
                List<MazeSquare> deadSquares = deadEndChecker.Visit(sq2, confirmedSquares);
                
                // TODO: disable this version
                if (sq1 == maze.StartSquare)
                {
                    for (int i = 0; i < maze.XSize; i++)
                    {
                        for (int j = 0; j < maze.YSize; j++)
                        {
                            MazeSquare sq = maze[i, j];
                            mazeDrawer.DrawAliveSquare(sq, deadEndChecker.Distance(sq), true);
                        }
                    }
                }
                foreach (MazeSquare sq in confirmedSquares)
                {
                    mazeDrawer.DrawAliveSquare(sq, deadEndChecker.Distance(sq), false);
                }
#else
                List<MazeSquare> deadSquares = deadEndChecker.Visit(sq2);
#endif
                foreach (MazeSquare deadSq in deadSquares)
                {
                    mazeDrawer.DrawDeadSquare(deadSq);
                }
            }
            #endregion
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

        /// <summary>
        /// Write state information to the given StringBuilder.
        /// The default is to do nothing.
        /// </summary>
        /// <param name="message"></param>
        public virtual void FillStatusMessage(StringBuilder message)
        {
            return;
        }

        #endregion

        #region Auxiliary methods for derived classes

        /// <summary>
        /// Returns a list of directions leading from the given square to neighbors through open walls.
        /// Neighbors that have been identified as dead ends are excluded (efficient solvers only).
        /// </summary>
        /// <param name="sq"></param>
        /// <param name="notVisitedOnly">
        /// When true: Exclude neighbors that have already been visited.
        /// </param>
        /// <returns></returns>
        protected List<MazeSquare.WallPosition> OpenWalls(MazeSquare sq, bool notVisitedOnly)
        {
            List<MazeSquare.WallPosition> result = new List<MazeSquare.WallPosition>(MazeSquare.WP_NUM);

            for (MazeSquare.WallPosition wp = MazeSquare.WP_MIN; wp <= MazeSquare.WP_MAX; wp++)
            {
                if (sq[wp] == MazeSquare.WallState.WS_OPEN)
                {
                    MazeSquare sq2 = sq.NeighborSquare(wp);

                    if (notVisitedOnly)
                    {
                        // Exclude squares that have already been visited.
                        if (sq2.isVisited)
                        {
                            continue;
                        }
                    }

                    // Exclude squares that need not be visited because they are dead ends.
                    if (this.deadEndChecker != null && deadEndChecker.IsDead(sq2))
                    {
                        continue;
                    }

                    result.Add(wp);
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the euclidian distance between two squares.
        /// </summary>
        /// <param name="sq1"></param>
        /// <param name="sq2"></param>
        /// <returns></returns>
        protected static double Distance(MazeSquare sq1, MazeSquare sq2)
        {
            double dx = sq1.XPos - sq2.XPos;
            double dy = sq1.YPos - sq2.YPos;
            return Math.Sqrt((dx * dx) + (dy * dy));
        }

        #endregion
    }
}
