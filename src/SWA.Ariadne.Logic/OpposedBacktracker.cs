using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model;

namespace SWA.Ariadne.Logic
{
    /// <summary>
    /// A MazeSolver with one current path and backtracking.
    /// Prefers visiting the square that lies on the opposite side of the maze from the start point.
    /// </summary>
    internal class OpposedBacktracker : DistanceGuidedBacktrackerBase
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="maze"></param>
        /// <param name="mazeDrawer"></param>
        public OpposedBacktracker(Maze maze, IMazeDrawer mazeDrawer)
            : base(maze, mazeDrawer)
        {
            this.referenceSquare = maze[maze.XSize - 1 - maze.StartSquare.XPos, maze.YSize - 1 - maze.StartSquare.YPos];
        }

        #endregion
    }
}
