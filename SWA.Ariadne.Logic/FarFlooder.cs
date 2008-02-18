using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model;

namespace SWA.Ariadne.Logic
{
    /// <summary>
    /// A MazeSolver with many concurrent paths.
    /// Prefers visiting the square farthest away from the start point.
    /// </summary>
    class FarFlooder : CloseFlooder
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="maze"></param>
        /// <param name="mazeDrawer"></param>
        public FarFlooder(Maze maze, IMazeDrawer mazeDrawer)
            : base(maze, mazeDrawer)
        {
            // Invert the parent strategy: Maximize distance from the reference square.
            this.distanceSign = -1;
        }

        #endregion
    }
}
