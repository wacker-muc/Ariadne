using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model;
using SWA.Ariadne.Model.Interfaces;

namespace SWA.Ariadne.Logic
{
    /// <summary>
    /// A MazeSolver with many concurrent paths.
    /// Is guided by the distance to a reference point.
    /// </summary>
    internal abstract class DistanceGuidedFlooderBase : FlooderBase
    {
        #region Member variables

        /// <summary>
        /// The (euclidian) distance to this square should be minimized (or maximized).
        /// </summary>
        protected MazeSquare referenceSquare;

        /// <summary>
        /// +1 (minimize distanze) or -1 (mazimize distance)
        /// </summary>
        protected int distanceSign = +1;

        /// <summary>
        /// A positive value (less than 1.0) means that SelectPathIdx() should select a random path.
        /// High values (close to 1.0) will result in an unbiased (evenly distributed) random choice.
        /// Low values (close to 0.0) will hardly ever choose the path with the worst PathValue().
        /// </summary>
        protected double randomScale = 0.0;

        #endregion

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
            if (0.0 < randomScale && randomScale < 1.0)
            {
                return SelectRandomPathIdx();
            }

            int bestIdx = 0;
            double bestValue = double.MaxValue;

            for (int i = 0; i < list.Count; i++)
            {
                double value = PathValue(i);

                if (value == double.MinValue)
                {
                    return i;
                }
                if (!IsSelectablePathIdx(i)) continue;

                if (value < bestValue)
                {
                    bestIdx = i;
                    bestValue = value;
                }
            }

            return bestIdx;
        }

        /// <summary>
        /// Select an index within the flooder's list of open paths.
        /// The paths with a low PathValue() are prefered but other paths may be selected, as well.
        /// </summary>
        /// <returns></returns>
        private int SelectRandomPathIdx()
        {
            double[] values = new double[list.Count];
            double bestValue = double.MaxValue, worstValue = double.MinValue;

            // Collect all path values.
            for (int i = 0; i < list.Count; i++)
            {
                double value = values[i] = PathValue(i);

                if (value == double.MinValue)
                {
                    return i;
                }
                if (!IsSelectablePathIdx(i)) continue;

                if (value < bestValue)
                {
                    bestValue = value;
                }
                if (value > worstValue)
                {
                    worstValue = value;
                }
            }

            // If all values are equal, return a random path index.
            // E.g. if there is only one path.
            if (bestValue == worstValue)
            {
                return this.random.Next(list.Count);
            }

            // Normalize the values to the range [R .. 1.0] (worst value to best value).
            double sum = 0.0;
            for (int i = 0; i < list.Count; i++)
            {
                double value = values[i];
                sum += values[i] = ((value - bestValue) * randomScale + (worstValue - value) * 1.0) / (worstValue - bestValue);
            }

            // Pick an index, where each index has the (relative) probability of its value.
            double choice = sum * this.random.NextDouble();
            for (int i = 0; i < list.Count; i++)
            {
                choice -= values[i];
                if (choice <= 0.0)
                {
                    return i;
                }
            }

            // If no path was chosen, return the last one.
            // E.g. if the arithmetic in the previous loop failed because of rounding errors.
            return list.Count - 1;
        }

        /// <summary>
        /// Returns the value of a given currently open path.
        /// This value should be minimized.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        protected virtual double PathValue(int i)
        {
            return distanceSign * Maze.Distance(referenceSquare, list[i]);
        }

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
                double distance = distanceSign * Maze.Distance(referenceSquare, sq1.NeighborSquare(openWalls[i]));
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
