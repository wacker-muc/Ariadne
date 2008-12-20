using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model;

namespace SWA.Ariadne.Logic
{
    /// <summary>
    /// A MazeSolver with many concurrent paths.
    /// Prefers the direction leading towards the end point.
    /// Any path may be picked with a probability of 1 in 20 (relative to the preferred path).
    /// </summary>
    internal class RandomForwardFlooder : ForwardFlooder
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="maze"></param>
        /// <param name="mazeDrawer"></param>
        public RandomForwardFlooder(Maze maze, IMazeDrawer mazeDrawer)
            : base(maze, mazeDrawer)
        {
            this.randomScale = 1.0 / (20.0 - 1.0);
        }
    }

    /// <summary>
    /// A MazeSolver with many concurrent paths.
    /// Prefers the direction leading away from the end point.
    /// Any path may be picked with a probability of 1 in 20 (relative to the preferred path).
    /// </summary>
    internal class RandomBackwardFlooder : BackwardFlooder
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="maze"></param>
        /// <param name="mazeDrawer"></param>
        public RandomBackwardFlooder(Maze maze, IMazeDrawer mazeDrawer)
            : base(maze, mazeDrawer)
        {
            this.randomScale = 1.0 / (20.0 - 1.0);
        }
    }
}
