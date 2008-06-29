using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model;

namespace SWA.Ariadne.Logic
{
    class DirectionGuidedFlooderBase : DistanceGuidedFlooderBase
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="maze"></param>
        /// <param name="mazeDrawer"></param>
        public DirectionGuidedFlooderBase(Maze maze, IMazeDrawer mazeDrawer)
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
            double bestDistanceGain = double.MaxValue;

            // Find the pair of squares with the highest (relative) distance gain.
            for (int i = 0; i < list.Count; i++)
            {
                MazeSquare sq1 = list[i];
                List<MazeSquare.WallPosition> openWalls = OpenWalls(sq1, true);
                if (openWalls.Count == 0)
                {
                    // Immediately report any dead branch.  Otherwise they would never be detected.
                    return i;
                }
                MazeSquare.WallPosition wp = SelectDirection(sq1, openWalls);
                MazeSquare sq2 = sq1.NeighborSquare(wp);

                double d1 = Distance(referenceSquare, sq1);
                double d2 = Distance(referenceSquare, sq2);
                double distanceGain = distanceSign * ((d2 - d1) / d1);
                if (distanceGain < bestDistanceGain)
                {
                    bestIdx = i;
                    bestDistanceGain = distanceGain;
                }
            }

            return bestIdx;
        }

        #endregion
    }
}
