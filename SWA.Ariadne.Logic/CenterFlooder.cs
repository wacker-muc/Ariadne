using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model;

namespace SWA.Ariadne.Logic
{
    /// <summary>
    /// A MazeSolver with many concurrent paths.
    /// Prefers visiting the square closest to the center of the maze.
    /// </summary>
    internal class CenterFlooder : DistanceGuidedFlooderBase
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="maze"></param>
        /// <param name="mazeDrawer"></param>
        public CenterFlooder(Maze maze, IMazeDrawer mazeDrawer)
            : base(maze, mazeDrawer)
        {
            this.referenceSquare = maze[maze.XSize/2, maze.YSize/2];
        }

        #endregion
    }
}
