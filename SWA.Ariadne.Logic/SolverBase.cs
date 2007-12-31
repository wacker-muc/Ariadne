using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model;

namespace SWA.Ariadne.Logic
{
    public abstract class SolverBase
    {
        #region Member variables

        protected readonly Maze maze;

        /// <summary>
        /// Coordinates of current position while solving the maze.
        /// </summary>
        protected int xCur, yCur;

        #endregion

        #region Constructor

        protected SolverBase(Maze maze)
        {
            this.maze = maze;
        }

        #endregion

        public void Reset()
        {
            maze.GetStartCoordinates(out xCur, out yCur);
        }
    }
}
