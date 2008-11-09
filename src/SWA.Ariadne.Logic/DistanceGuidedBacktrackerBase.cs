using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model;
using SWA.Ariadne.Model.Interfaces;

namespace SWA.Ariadne.Logic
{
    /// <summary>
    /// A MazeSolver with one current path and backtracking.
    /// Is guided by the distance to a reference point.
    /// </summary>
    internal abstract class DistanceGuidedBacktrackerBase : BacktrackerBase
    {
        #region Member variables

        /// <summary>
        /// The (euclidian) distance to this square should be minimized.
        /// </summary>
        protected MazeSquare referenceSquare;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="maze"></param>
        /// <param name="mazeDrawer"></param>
        public DistanceGuidedBacktrackerBase(Maze maze, IMazeDrawer mazeDrawer)
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
            int bestIdx = 0;
            double bestDistance = double.MaxValue;

            for (int i = 0; i < openWalls.Count; i++)
            {
                double distance = Maze.Distance(referenceSquare, sq1.NeighborSquare(openWalls[i]));
                if (distance < bestDistance)
                {
                    bestIdx = i;
                    bestDistance = distance;
                }
            }

            return openWalls[bestIdx];
        }

        #endregion
    }
}
