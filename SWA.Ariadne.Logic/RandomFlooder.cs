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
    internal class RandomFlooder : Flooder
    {
        #region Member variables

        /// <summary>
        /// A source of random numbers.
        /// </summary>
        private Random random = RandomFactory.CreateRandom();

        /// <summary>
        /// All squares passed in forward direction are collected in a list.
        /// </summary>
        private List<MazeSquare> list = new List<MazeSquare>();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="maze"></param>
        /// <param name="mazeDrawer"></param>
        public RandomFlooder(Maze maze, IMazeDrawer mazeDrawer)
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

            list.Clear();

            // Move to the start square.
            MazeSquare sq = maze.StartSquare;

            // Add the start square to the queue.
            list.Add(sq);
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

            List<MazeSquare.WallPosition> openWalls;

            while (true)
            {
                // Get a current square but leave it in the queue.
                int p = random.Next(list.Count);
                sq1 = list[p];

                // Possible choices of open walls (not visited).
                openWalls = SolverBase.OpenWalls(sq1, true);

                if (openWalls.Count == 0)
                {
                    list.RemoveAt(p);
                    MarkDeadBranch(sq1);
                }
                else
                {
                    // If this was the last open wall of sq1, it can be removed from the list.
                    if (openWalls.Count == 1)
                    {
                        list.RemoveAt(p);
                    }

                    break;
                }
            }

            // Select one (any) of the neighbor squares.
            MazeSquare.WallPosition wp = openWalls[random.Next(openWalls.Count)];

            sq2 = sq1.NeighborSquare(wp);
            forward = true;

            // Add the next square to the list.
            list.Add(sq2);
            sq2.isVisited = true;
        }

        /// <summary>
        /// Current number of open paths.
        /// </summary>
        protected override int CountOpenPaths
        {
            get { return this.list.Count; }
        }

        #endregion
    }
}
