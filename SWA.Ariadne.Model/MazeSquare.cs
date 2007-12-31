using System;
using System.Collections.Generic;
using System.Text;

namespace SWA.Ariadne.Model
{
    sealed public class MazeSquare
    {
        /// <summary>
        /// Positions of the walls around a square: East, North, West, South.
        /// </summary>
        public enum WallPosition : int
        {
            WP_E = 0,
            WP_N = 1,
            WP_W = 2,
            WP_S = 3,
            WP_NUM = 4,
        }

        /// <summary>
        /// States of a wall: Open, Closed, Not determined.
        /// </summary>
        public enum WallState : byte
        {
            WS_OPEN = 0,
            WS_CLOSED = 1,
            WS_MAYBE = 2,
        }

        WallState[] wall = new WallState[(int)WallPosition.WP_NUM];

        /// <summary>
        /// Used while building: Square is connected to the maze.
        /// </summary>
        internal bool isConnected = false;

        /// <summary>
        /// Used while building: Square will not be used.
        /// </summary>
        internal bool isReserved = false;

        /// <summary>
        /// Used while solving: Square has been visited.
        /// </summary>
        internal bool isVisited = false;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MazeSquare()
        {
        }

        /// <summary>
        /// Returns the WallPosition on the opposite side of a square.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        internal static WallPosition OppositeWall(WallPosition p)
        {
            switch (p)
            {
                case WallPosition.WP_E: return WallPosition.WP_W;
                case WallPosition.WP_N: return WallPosition.WP_S;
                case WallPosition.WP_W: return WallPosition.WP_E;
                case WallPosition.WP_S: return WallPosition.WP_N;
                default: throw new ArgumentOutOfRangeException("p");
            }
        }
    }
}
