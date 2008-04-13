using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model;

namespace SWA.Ariadne.Logic
{
    internal class EfficientRightHandWalker : RightHandWalker
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="maze"></param>
        /// <param name="mazeDrawer"></param>
        public EfficientRightHandWalker(Maze maze, IMazeDrawer mazeDrawer)
            : base(maze, mazeDrawer)
        {
            // An efficient MazeSolver has a DeadEndChecker.
            deadEndChecker = new DeadEndChecker(maze);
        }

        #endregion
    }
}
