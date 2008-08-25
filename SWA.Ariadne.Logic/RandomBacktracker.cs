using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model;
using SWA.Ariadne.Model.Interfaces;

namespace SWA.Ariadne.Logic
{
    /// <summary>
    /// A MazeSolver with one current path and backtracking.
    /// At a crossing in forward direction: Chooses a random open wall.
    /// </summary>
    internal class RandomBacktracker : BacktrackerBase
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="maze"></param>
        /// <param name="mazeDrawer"></param>
        public RandomBacktracker(Maze maze, IMazeDrawer mazeDrawer)
            : base(maze, mazeDrawer)
        {
        }

        #endregion

        #region Runtime methods

        /// <summary>
        /// Select one of the open walls leading away from the given square.
        /// </summary>
        /// <param name="sq1"></param>
        /// <param name="openWalls"></param>
        /// <returns></returns>
        protected override WallPosition SelectDirection(MazeSquare sq1, List<WallPosition> openWalls)
        {
            return openWalls[random.Next(openWalls.Count)];
        }

        #endregion
    }
}
