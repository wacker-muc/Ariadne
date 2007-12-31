using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model;

namespace SWA.Ariadne.Logic
{
    public class RandomSolver : SolverBase
    {
        #region Member variables

        /// <summary>
        /// A source of random numbers.
        /// </summary>
        private Random random;

        #endregion

        #region Constructor

        public RandomSolver(Maze maze)
            : base(maze)
        {
            this.random = new Random();
        }

        #endregion

        public void Step()
        {
            if (maze.Solved())
            {
                return;
            }

            // possible choices
            int[] xNext = new int[4];
            int[] yNext = new int[4];
            int numNext = 0;

            // minimum of visited count of the current square's neighbors
            byte minVisited = 2;

            // TODO: find most promising neighbor squares

            // select one of the neighbor squares
            int iNext = random.Next(numNext);

            // TODO: draw a path from xyCur to xyNext[iNext]

            xCur = xNext[iNext];
            yCur = yNext[iNext];
        }

    }
}
