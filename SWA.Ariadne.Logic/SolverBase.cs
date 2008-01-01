using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model;

namespace SWA.Ariadne.Logic
{
    /// <summary>
    /// Base class of all MazeSolver classes.
    /// </summary>
    public abstract class SolverBase : IMazeSolver
    {
        #region Member variables

        protected readonly Maze maze;

        #endregion

        #region Constructor

        protected SolverBase(Maze maze)
        {
            this.maze = maze;
        }

        #endregion

        #region IMazeSolver methods

        public abstract void Reset();
        public abstract void Step(out MazeSquare sq1, out MazeSquare sq2, out bool forward);

        #endregion
    }
}
