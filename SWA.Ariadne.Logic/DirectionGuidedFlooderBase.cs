using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model;
using SWA.Ariadne.Model.Interfaces;

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
        /// Returns the value of a given currently open path.
        /// This value should be minimized.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        protected override double PathValue(int i)
        {
            MazeSquare sq1 = list[i];
            List<WallPosition> openWalls = OpenWalls(sq1, true);
            if (openWalls.Count == 0)
            {
                // Immediately report any dead branch.  Otherwise they would never be detected.
                return double.MinValue;
            }
            WallPosition wp = SelectDirection(sq1, openWalls);
            MazeSquare sq2 = sq1.NeighborSquare(wp);

            double d1 = Maze.Distance(referenceSquare, sq1);
            double d2 = Maze.Distance(referenceSquare, sq2);
            double distanceGain = distanceSign * ((d2 - d1) / d1);
            return distanceGain;
        }

        #endregion
    }
}
