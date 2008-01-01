using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model;

namespace SWA.Ariadne.Logic
{
    /// <summary>
    /// A MazeSolver with one path and a local strategy.
    /// At a crossing: Chooses the right hand open wall.
    /// </summary>
    public class RightHandWalker : DeterministicWalker
    {
        #region Constructor

        public RightHandWalker(Maze maze)
            : base(maze)
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
            while (currentSquare[currentDirection] != MazeSquare.WallState.WS_OPEN)
            {
                TurnLeft();
            }
        }

        #endregion
    }
}
