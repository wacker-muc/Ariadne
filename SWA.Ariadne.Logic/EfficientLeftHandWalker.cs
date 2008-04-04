using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model;

namespace SWA.Ariadne.Logic
{
    class EfficientLeftHandWalker : LeftHandWalker
    {
        #region Member variables

        private readonly DeadEndChecker deadEndChecker;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="maze"></param>
        /// <param name="mazeDrawer"></param>
        public EfficientLeftHandWalker(Maze maze, IMazeDrawer mazeDrawer)
            : base(maze, mazeDrawer)
        {
            deadEndChecker = new DeadEndChecker(maze);
        }

        #endregion

        #region Implementation

        public override void Step(out MazeSquare sq1, out MazeSquare sq2, out bool forward)
        {
            base.Step(out sq1, out sq2, out forward);

            List<MazeSquare> deadSquares = deadEndChecker.Visit(sq2);
            foreach (MazeSquare deadSq in deadSquares)
            {
                mazeDrawer.DrawDeadSquare(deadSq, deadEndChecker.Distance(deadSq.XPos, deadSq.YPos));
            }
#if false // debug code
            for (int i = 0; i < maze.XSize; i++)
            {
                for (int j = 0; j < maze.YSize; j++)
                {
                    MazeSquare sq = maze[i, j];
                    mazeDrawer.DrawAliveSquare(sq, deadEndChecker.Distance(sq.XPos, sq.YPos));
                }
            }
#endif
        }

        /// <summary>
        /// Check if a move to the neighbor square in tne current direction would be valid.
        /// </summary>
        /// <returns>true if the move is invalid</returns>
        protected override bool CurrentDirectionIsInvalid()
        {
            if (base.CurrentDirectionIsInvalid())
            {
                return true;
            }

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

            return false;
        }

        #endregion
    }
}
