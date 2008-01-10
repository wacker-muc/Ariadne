using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model;

namespace SWA.Ariadne.Logic
{
    /// <summary>
    /// A MazeSolver with one path and no percievable strategy.
    /// At a crossing: Chooses a random open wall.
    /// </summary>
    internal class RandomWalker : SolverBase
    {
        #region Member variables

        /// <summary>
        /// A source of random numbers.
        /// </summary>
        private Random random;

        MazeSquare currentSquare;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="maze"></param>
        /// <param name="mazeDrawer"></param>
        public RandomWalker(Maze maze, IMazeDrawer mazeDrawer)
            : base(maze, mazeDrawer)
        {
            this.random = new Random();
        }

        #endregion

        #region Setup methods

        /// <summary>
        /// Reset to the initial state (before the maze is solved).
        /// </summary>
        public override void Reset()
        {
            // Move to the start square.
            currentSquare = maze.StartSquare;
            currentSquare.isVisited = true;
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

            // Get the current position.
            sq1 = currentSquare;

            // Possible choices of open walls.
            List<MazeSquare.WallPosition> openWalls = SolverBase.OpenWalls(sq1, false);

            // Select one of the neighbor squares.
            MazeSquare.WallPosition wp = openWalls[random.Next(openWalls.Count)];

            sq2 = sq1.NeighborSquare(wp);
            forward = (sq2.isVisited == false);

            // Remember the new position.
            currentSquare = sq2;
            sq2.isVisited = true;
        }

        #endregion
    }
}
