using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model;

namespace SWA.Ariadne.Logic
{
    /// <summary>
    /// A MazeSolver with many concurrent paths.
    /// Prefers visiting the square closest to the end point.
    /// </summary>
    internal class ProximityFlooder : DistanceGuidedFlooderBase
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="maze"></param>
        /// <param name="mazeDrawer"></param>
        public ProximityFlooder(Maze maze, IMazeDrawer mazeDrawer)
            : base(maze, mazeDrawer)
        {
        }

        #endregion

        #region Runtime methods

        /// <summary>
        /// The (euclidian) distance to this square should be minimized.
        /// </summary>
        protected override MazeSquare ReferenceSquare
        {
            get
            {
                return maze.EndSquare;
            }
        }

        #endregion
    }
}
