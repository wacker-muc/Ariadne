using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model;

namespace SWA.Ariadne.Logic
{
    /// <summary>
    /// A MazeSolver with many concurrent paths.
    /// Visits all neighbor squares of the current path's end before advancing to the next path.
    /// </summary>
    internal class RoundRobinFlooder : SolverBase
    {
        #region Member variables

        /// <summary>
        /// All squares passed in forward direction are collected in a queue.
        /// </summary>
        private Queue<MazeSquare> queue = new Queue<MazeSquare>();

        #endregion

        #region Constructor

        public RoundRobinFlooder(Maze maze)
            : base(maze)
        {
        }

        #endregion

        #region Setup methods

        /// <summary>
        /// Reset to the initial state (before the maze is solved).
        /// </summary>
        public override void Reset()
        {
            queue.Clear();

            // Move to the start square.
            MazeSquare sq = maze.StartSquare;

            // Add the start square to the queue.
            queue.Enqueue(sq);
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
        public override void Step(out MazeSquare sq1, out MazeSquare sq2, out bool forward)
        {
            if (maze.IsSolved)
            {
                throw new Exception("Maze is already solved.");
            }

            List<MazeSquare.WallPosition> openWalls;
            
            while(true)
            {
                // Get the current square but leave it in the queue.
                sq1 = queue.Peek();

                // Possible choices of open walls (not visited).
                openWalls = SolverBase.OpenWalls(sq1, true);

                if (openWalls.Count == 0)
                {
                    queue.Dequeue();
                }
                else
                {
                    break;
                }
            }

            // Select one (any) of the neighbor squares.
            MazeSquare.WallPosition wp = openWalls[0];

            sq2 = sq1.NeighborSquare(wp);
            forward = true;

            // Add the next square to the queue.
            queue.Enqueue(sq2);
            sq2.isVisited = true;
        }

        #endregion
    }
}
