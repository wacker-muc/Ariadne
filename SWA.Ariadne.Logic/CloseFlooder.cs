using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model;

namespace SWA.Ariadne.Logic
{
    /// <summary>
    /// A MazeSolver with many concurrent paths.
    /// Prefers visiting the square closest to the start point.
    /// </summary>
    internal class CloseFlooder : DistanceGuidedFlooderBase
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="maze"></param>
        /// <param name="mazeDrawer"></param>
        public CloseFlooder(Maze maze, IMazeDrawer mazeDrawer)
            : base(maze, mazeDrawer)
        {
            this.referenceSquare = maze.StartSquare;
        }

        #endregion
    }
}
