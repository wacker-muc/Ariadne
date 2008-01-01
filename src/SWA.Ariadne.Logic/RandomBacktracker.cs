using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model;

namespace SWA.Ariadne.Logic
{
    /// <summary>
    /// A MazeSolver with one current path and backtracking.
    /// At a crossing in forward direction: Chooses a random open wall.
    /// </summary>
    public class RandomBacktracker : SolverBase
    {
        #region Member variables

        /// <summary>
        /// A source of random numbers.
        /// </summary>
        private Random random;

        /// <summary>
        /// All squares passed in forward directions are collected on a stack.
        /// </summary>
        private Stack<MazeSquare> stack = new Stack<MazeSquare>();

        #endregion

        #region Constructor

        public RandomBacktracker(Maze maze)
            : base(maze)
        {
            this.random = new Random();
            this.Reset();
        }

        #endregion

        #region Setup methods

        /// <summary>
        /// Reset to the initial state (before the maze is solved).
        /// </summary>
        public override void Reset()
        {
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
        /// Travel from one visited square to a neighbor square (through an open all).
        /// </summary>
        /// <param name="sq1">first (previously visited) square</param>
        /// <param name="sq2">next (neighbor) square</param>
        /// <param name="forward">true if the neighbor square was not visited previously</param>
        public override void Step(out MazeSquare sq1, out MazeSquare sq2, out bool forward)
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
                MazeSquare.WallPosition wp = openWalls[random.Next(openWalls.Count)];

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

        #endregion
    }
}