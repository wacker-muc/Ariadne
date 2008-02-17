using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model;

namespace SWA.Ariadne.Logic
{
    /// <summary>
    /// A MazeSolver with many concurrent paths.
    /// Is guided by the distance to a reference point.
    /// </summary>
    internal abstract class DistanceGuidedFlooderBase : FlooderBase
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="maze"></param>
        /// <param name="mazeDrawer"></param>
        public DistanceGuidedFlooderBase(Maze maze, IMazeDrawer mazeDrawer)
            : base(maze, mazeDrawer)
        {
        }

        #endregion

        #region Runtime methods

        /// <summary>
        /// Select an index within the flooder's list of open paths.
        /// </summary>
        /// <returns></returns>
        protected override int SelectPathIdx()
        {
            int bestIdx = 0;
            double bestDistance = double.MaxValue;

            for (int i = 0; i < list.Count; i++)
            {
                double distance = Distance(ReferenceSquare, list[i]);
                if (distance < bestDistance)
                {
                    bestIdx = i;
                    bestDistance = distance;
                }
            }

            return bestIdx;
        }

        /// <summary>
        /// The (euclidian) distance to this square should be minimized.
        /// </summary>
        protected abstract MazeSquare ReferenceSquare { get; }

        /// <summary>
        /// Select one of the open walls leading away from the given square.
        /// </summary>
        /// <param name="sq1"></param>
        /// <param name="openWalls"></param>
        /// <returns></returns>
        protected override MazeSquare.WallPosition SelectDirection(MazeSquare sq1, List<MazeSquare.WallPosition> openWalls)
        {
            int bestIdx = 0;
            double bestDistance = double.MaxValue;

            for (int i = 0; i < openWalls.Count; i++)
            {
                double distance = Distance(ReferenceSquare, sq1.NeighborSquare(openWalls[i]));
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
