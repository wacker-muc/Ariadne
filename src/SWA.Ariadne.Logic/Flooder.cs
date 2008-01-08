using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model;

namespace SWA.Ariadne.Logic
{
    /// <summary>
    /// Base class for all MazeSolvers using a flooding strategy.
    /// </summary>
    abstract class Flooder : SolverBase
    {
        private struct MazeSquareExtension
        {
            /// <summary>
            /// Number of the open paths that lead away from this square.
            /// </summary>
            public int openPathCount;

            /// <summary>
            /// Predecessor of this square on its path.
            /// </summary>
            public MazeSquare previousSquare;
        }
        /// <summary>
        /// For every MazeSquare: a counter of the open paths that lead away from it.
        /// </summary>
        /// A path is considered "open" if its end point is in the list of active squares.
        private MazeSquareExtension[,] mazeExtension;

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="maze"></param>
        public Flooder(Maze maze)
            : base(maze)
        {
            this.mazeExtension = new MazeSquareExtension[maze.XSize, maze.YSize];
        }

        #endregion

        #region Setup methods

        /// <summary>
        /// Reset to the initial state (before the maze is solved).
        /// Subclasses should call their base class' method first.
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            // As we may not retract beyond the start square, it needs to have a positive count.
            MazeSquare sq = maze.StartSquare;
            mazeExtension[sq.XPos, sq.YPos].openPathCount = 1;
        }

        #endregion

        #region Runtime methods

        /// <summary>
        /// Travel from one visited square to a neighbor square (through an open wall).
        /// Wrapper. Calls the implementing method StepI().
        /// </summary>
        /// <param name="sq1">first (previously visited) square</param>
        /// <param name="sq2">next (neighbor) square</param>
        /// <param name="forward">true if the neighbor square was not visited previously</param>
        public override void Step(out MazeSquare sq1, out MazeSquare sq2, out bool forward)
        {
            StepI(out sq1, out sq2, out forward);

            if (forward) // Always true; Flooders only travel forwards.
            {
                mazeExtension[sq1.XPos, sq1.YPos].openPathCount += 1;
                mazeExtension[sq2.XPos, sq2.YPos].previousSquare = sq1;

                // This might also be done in the Reset() method.  But it is not too late here.
                mazeExtension[sq2.XPos, sq2.YPos].openPathCount = 0;
            }
        }

        /// <summary>
        /// Calls the MarkDeadBranchDelegate with the branch ending in the given square.
        /// </summary>
        /// <param name="sq">a MazeSquare in a dead end of the Maze</param>
        protected void MarkDeadBranch(MazeSquare sq)
        {
            if (SolverBase.OpenWalls(sq, false).Count > 1)
            {
                /* This is a false call.
                 * The subclass MazeSolver regards this square as no longer usable
                 * but it is not a true dead end.
                 */
                return;
            }
            List<MazeSquare> deadBranch = new List<MazeSquare>();

            deadBranch.Add(sq);

            do {
                sq = mazeExtension[sq.XPos, sq.YPos].previousSquare;
                deadBranch.Add(sq);
            } while(
                --mazeExtension[sq.XPos, sq.YPos].openPathCount == 0 // no more open paths
             && SolverBase.OpenWalls(sq, true).Count == 0            // no more unvisited open walls
                );

            if (this.markDeadBranchDelegate != null)
            {
                this.markDeadBranchDelegate(deadBranch);
            }
        }

        #endregion
    }
}
