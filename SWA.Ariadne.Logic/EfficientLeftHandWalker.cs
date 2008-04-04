using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model;

namespace SWA.Ariadne.Logic
{
    class EfficientLeftHandWalker : LeftHandWalker
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="maze"></param>
        /// <param name="mazeDrawer"></param>
        public EfficientLeftHandWalker(Maze maze, IMazeDrawer mazeDrawer)
            : base(maze, mazeDrawer)
        {
            // An efficient MazeSolver has a DeadEndChecker.
            deadEndChecker = new DeadEndChecker(maze);
        }

        #endregion
    }
}
