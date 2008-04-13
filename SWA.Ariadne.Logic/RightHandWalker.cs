using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model;

namespace SWA.Ariadne.Logic
{
    /// <summary>
    /// A MazeSolver with one path and a local strategy.
    /// At a crossing: Chooses the open wall closest to the right hand.
    /// </summary>
    internal class RightHandWalker : DeterministicWalker
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="maze"></param>
        /// <param name="mazeDrawer"></param>
        public RightHandWalker(Maze maze, IMazeDrawer mazeDrawer)
            : base(maze, mazeDrawer)
        {
        }

        #endregion

        #region DeterministicWalker implementation

        /// <summary>
        /// Set a new (valid) current direction.
        /// Turn to the valid direction closest to the right hand.
        /// </summary>
        /// <param name="sq1"></param>
        protected override void Turn()
        {
            TurnRight();
            while (CurrentDirectionIsInvalid())
            {
                TurnLeft();
            }
        }

        #endregion
    }
}
