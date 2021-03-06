using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model;
using SWA.Ariadne.Model.Interfaces;

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
        protected WallPosition currentDirection;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="maze"></param>
        /// <param name="mazeDrawer"></param>
        public DeterministicWalker(Maze maze, IMazeDrawer mazeDrawer)
            : base(maze, mazeDrawer)
        {
        }

        #endregion

        #region Setup methods

        /// <summary>
        /// Reset to the initial state (before the maze is solved).
        /// Subclasses should call their base class' method first.
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            // Move to the start square.
            currentSquare = maze.StartSquare;
            currentSquare.isVisited = true;

            // Start in an arbitrary direction (with a wall in the back).
            for (currentDirection = WallPosition.WP_MIN; currentDirection < WallPosition.WP_MAX; currentDirection++)
            {
                if (currentSquare[currentDirection] == WallState.WS_CLOSED)
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
        protected override void StepI(out MazeSquare sq1, out MazeSquare sq2, out bool forward)
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
        protected abstract void Turn();

        /// <summary>
        /// Turn the currentDirection one quarter to the left (counterclockwise)
        /// </summary>
        protected void TurnLeft()
        {
            if (currentDirection == WallPosition.WP_MAX)
            {
                currentDirection = WallPosition.WP_MIN;
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
            if (currentDirection == WallPosition.WP_MIN)
            {
                currentDirection = WallPosition.WP_MAX;
            }
            else
            {
                --currentDirection;
            }
        }

        /// <summary>
        /// Check if a move to the neighbor square in the current direction would be valid.
        /// </summary>
        /// <returns>true if the move is invalid</returns>
        protected virtual bool CurrentDirectionIsInvalid()
        {
            // Check for open/closed walls.
            if (currentSquare[currentDirection] != WallState.WS_OPEN)
            {
                return true;
            }

            // Check for dead ends.
            if (deadEndChecker != null)
            {
                MazeSquare sq = currentSquare.NeighborSquare(currentDirection);

                if (sq.isVisited)
                {
                    // Backward moves must be respected.
                    return false;
                }

                if (deadEndChecker.IsDead(sq))
                {
                    return true;
                }
            }

            // No problem.
            return false;
        }

        #endregion
    }
}
