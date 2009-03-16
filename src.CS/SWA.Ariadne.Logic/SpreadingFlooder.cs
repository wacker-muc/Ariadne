using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model;
using SWA.Ariadne.Model.Interfaces;

namespace SWA.Ariadne.Logic
{
    /// <summary>
    /// A MazeSolver with many concurrent paths.
    /// Prefers advancing one of the paths that are closest to each other.
    /// Effectively, this tends to spread out the path ends as wide as possible.
    /// </summary>
    class SpreadingFlooder : FlooderBase
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="maze"></param>
        /// <param name="mazeDrawer"></param>
        public SpreadingFlooder(Maze maze, IMazeDrawer mazeDrawer)
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
                if (!IsSelectablePathIdx(i)) continue;
                MazeSquare sq = list[i];

                for (int j = i+1; j < list.Count; j++)
                {
                    double distance = Maze.Distance(sq, list[j]);
                    if (distance < bestDistance)
                    {
                        bestIdx = i;
                        bestDistance = distance;
                    }
                }
            }

            return bestIdx;
        }

        /// <summary>
        /// Select one of the open walls leading away from the given square.
        /// </summary>
        /// <param name="sq1"></param>
        /// <param name="openWalls"></param>
        /// <returns></returns>
        protected override WallPosition SelectDirection(MazeSquare sq1, List<WallPosition> openWalls)
        {
            return openWalls[0];
        }

        #endregion
    }
}
