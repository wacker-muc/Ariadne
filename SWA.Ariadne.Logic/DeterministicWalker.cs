using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model;

namespace SWA.Ariadne.Logic
{
    /// <summary>
    /// A MazeSolver with one path and a local strategy.
    /// The choice of an open wall is implemented by subclasses.
    /// </summary>
    internal abstract class DeterministicWalker : SolverBase
    {
        #region Member variables

        protected MazeSquare currentSquare;
        protected MazeSquare.WallPosition currentDirection;

        #endregion

        #region Constructor

        public DeterministicWalker(Maze maze)
            : base(maze)
        {
        }

        #endregion

        #region Setup methods

        /// <summary>
        /// Reset to the initial state (before the maze is solved).
        /// </summary>
        public override void Reset()
        {
            // Move to the start square.
            currentSquare = maze.StartSquare;
            currentSquare.isVisited = true;

            // Start in an arbitrary direction (with a wall in the back).
            for (currentDirection = MazeSquare.WP_MIN; currentDirection < MazeSquare.WP_MAX; currentDirection++)
            {
                if (currentSquare[currentDirection] == MazeSquare.WallState.WS_CLOSED)
                {
                    currentDirection = MazeSquare.OppositeWall(currentDirection);
                    break;
                }
            }
        }

        #endregion

        #region Runtime methods

        /// <summary>
        /// Travel from one visited square to a neighbor square (through an open wall).
        /// </summary>
        /// <param name="sq1">first (previously visited) square</param>
        /// <param name="sq2">next (neighbor) square</param>
        /// <param name="forward">true if the neighbor square was not visited previously</param>
        public override void Step(out MazeSquare sq1, out MazeSquare sq2, out bool forward)
        {
            if (maze.IsSolved)
            {
                throw new Exception("Maze is already solved.");
            }

            // Get the current position.
            sq1 = currentSquare;

            // Set a new (valid) current direction.
            Turn();

            sq2 = sq1.NeighborSquare(currentDirection);
            forward = (sq2.isVisited == false);

            // Remember the new position.
            currentSquare = sq2;
            sq2.isVisited = true;
        }

        #endregion

        #region Auxiliary methods

        /// <summary>
        /// Set a new (valid) current direction.
        /// </summary>
        /// <param name="sq1"></param>
        protected abstract void Turn();

        /// <summary>
        /// Turn the currentDirection one quarter to the left (counterclockwise)
        /// </summary>
        protected void TurnLeft()
        {
            if (currentDirection == MazeSquare.WP_MAX)
            {
                currentDirection = MazeSquare.WP_MIN;
            }
            else
            {
                ++currentDirection;
            }
        }

        /// <summary>
        /// Turn the currentDirection one quarter to the right (clockwise)
        /// </summary>
        protected void TurnRight()
        {
            if (currentDirection == MazeSquare.WP_MIN)
            {
                currentDirection = MazeSquare.WP_MAX;
            }
            else
            {
                --currentDirection;
            }
        }

        #endregion
    }
}
