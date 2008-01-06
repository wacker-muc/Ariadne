using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model;

namespace SWA.Ariadne.Logic
{
    /// <summary>
    /// A MazeSolver with one path and a local strategy.
    /// At a crossing: Chooses the open wall closest to the left hand.
    /// </summary>
    internal class LeftHandWalker : DeterministicWalker
    {
        #region Constructor

        public LeftHandWalker(Maze maze)
            : base(maze)
        {
        }

        #endregion

        #region DeterministicWalker implementation

        /// <summary>
        /// Set a new (valid) current direction.
        /// Turn to the valid direction closest to the left hand.
        /// </summary>
        /// <param name="sq1"></param>
        protected override void Turn()
        {
            TurnLeft();
            while (currentSquare[currentDirection] != MazeSquare.WallState.WS_OPEN)
            {
                TurnRight();
            }
        }

        #endregion
    }
}
