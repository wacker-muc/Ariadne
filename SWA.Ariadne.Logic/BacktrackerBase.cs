using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model;

namespace SWA.Ariadne.Logic
{
    /// <summary>
    /// A MazeSolver with one current path and backtracking.
    /// </summary>
    internal abstract class BacktrackerBase : SolverBase
    {
        #region Member variables

        /// <summary>
        /// All squares passed in forward direction are collected on a stack.
        /// </summary>
        private Stack<MazeSquare> stack = new Stack<MazeSquare>();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="maze"></param>
        /// <param name="mazeDrawer"></param>
        public BacktrackerBase(Maze maze, IMazeDrawer mazeDrawer)
            : base(maze, mazeDrawer)
        {
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

            stack.Clear();

            // Move to the start square.
            MazeSquare sq = maze.StartSquare;

            // Push the start square onto the stack.
            stack.Push(sq);
            sq.isVisited = true;
        }

        #endregion

        #region Runtime methods

        /// <summary>
        /// Travel from one visited square to a neighbor square (through an open wall).
        /// </summary>
        /// <param name="sq1">first (previously visited) square</param>
        /// <param name="sq2">next (neighbor) square</param>
        /// <param name="forward">true if the neighbor square was not visited previously</param>
        protected override void StepI(out MazeSquare sq1, out MazeSquare sq2, out bool forward)
        {
            if (maze.IsSolved)
            {
                throw new Exception("Maze is already solved.");
            }

            // Get the current square.
            sq1 = stack.Peek();

            // Possible choices of open walls (not visited).
            List<MazeSquare.WallPosition> openWalls = SolverBase.OpenWalls(sq1, true);

            if (openWalls.Count > 0)
            {
                // Select one of the neighbor squares.
                MazeSquare.WallPosition wp = SelectDirection(sq1, openWalls);

                sq2 = sq1.NeighborSquare(wp);
                forward = true;

                // Push the next square onto the stack.
                stack.Push(sq2);
                sq2.isVisited = true;
            }
            else
            {
                // Pop the current square from the stack.
                stack.Pop();

                sq2 = stack.Peek();
                forward = false;
            }
        }

        /// <summary>
        /// Select one of the open walls leading away from the given square.
        /// </summary>
        /// <param name="sq1"></param>
        /// <param name="openWalls"></param>
        /// <returns></returns>
        protected abstract MazeSquare.WallPosition SelectDirection(MazeSquare sq1, List<MazeSquare.WallPosition> openWalls);

        #endregion
    }
}
