using System;
using System.Collections.Generic;
using System.Text;

namespace SWA.Ariadne.Model
{
    #region class MazeSquare

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
        enum WallState : byte
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

    #endregion

    #region class Maze

    public class Maze
    {
        #region Member variables

        /// <summary>
        /// Maze dimension: number of squares.
        /// </summary>
        private int xSize, ySize;
        #region Properties
        public int YSize
        {
            get { return ySize; }
        }
        public int XSize
        {
            get { return xSize; }
        }
        #endregion

        /// <summary>
        /// Coordinates of the start and end point of the path through the maze.
        /// </summary>
        private int xStart, yStart, xEnd, yEnd;

        /// <summary>
        /// A source of random numbers.
        /// </summary>
        private Random random;

        /// <summary>
        /// Position and dimensions of a reserved area.
        /// TODO: Support more than one reserved areas.
        /// </summary>
        private int logoRow, logoCol, logoWidth, logoHeight;

        /// <summary>
        /// The maze is formed by a two-dimensional array of squares.
        /// </summary>
        private MazeSquare[,] squares;

        #endregion

        #region Constructor

        public Maze(int xSize, int ySize)
        {
            this.xSize = xSize;
            this.ySize = ySize;

            this.random = new Random();

            this.squares = new MazeSquare[xSize, ySize];
            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    squares[x, y] = new MazeSquare();
                }
            }
        }

        #endregion

        #region Setup methods

        public void CreateMaze()
        {
 	        //throw new Exception("The method or operation is not implemented.");
        }

        public void PlaceEndpoints()
        {
            bool invalid = true;
            while (invalid)
            {
                // the travel direction (one of four)
                int direction = this.random.Next(4);

                // a small portion of the maze size (in trave direction)
                int edgeWidth = 0;
                switch (direction)
                {
                    case 0:
                    case 2:
                        // vertical
                        edgeWidth = 1 + ySize * 2 / 100;
                        break;
                    case 1:
                    case 3:
                        // horizontal
                        edgeWidth = 1 + xSize * 2 / 100;
                        break;
                }

                // distance of start and end point from the maze border
                int edgeDistStart = 0
                    + random.Next(edgeWidth)
                    + random.Next(edgeWidth)
                    + random.Next(edgeWidth)
                    ;
                int edgeDistEnd = 0
                    + random.Next(edgeWidth)
                    + random.Next(edgeWidth)
                    + random.Next(edgeWidth)
                    ;

                switch (direction)
                {
                    case 0:
                        // start at top, end at bottom
                        xStart = random.Next(xSize);
                        yStart = ySize - 1 - edgeDistStart;
                        xEnd = random.Next(xSize);
                        yEnd = edgeDistEnd;
                        break;
                    case 1:
                        // start at left, end at right
                        xStart = edgeDistEnd;
                        yStart = random.Next(ySize);
                        xEnd = xSize - 1 - edgeDistStart;
                        yEnd = random.Next(ySize);
                        break;
                    case 2:
                        // start at bottom, end at top
                        xStart = random.Next(xSize);
                        yStart = edgeDistEnd;
                        xEnd = random.Next(xSize);
                        yEnd = ySize - 1 - edgeDistStart;
                        break;
                    case 3:
                        // start at right, end at left
                        xStart = xSize - 1 - edgeDistStart;
                        yStart = random.Next(ySize);
                        xEnd = edgeDistEnd;
                        yEnd = random.Next(ySize);
                        break;
                }

                // Verify that the endpoints are not in the restricted area.
                invalid = (squares[xStart,yStart].isReserved || squares[xEnd,yEnd].isReserved);
            }
        }

        public void Reset()
        {
            // clear the visited region
            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    squares[x, y].isVisited = false;
                }
            }
        }

        #endregion

        /// <summary>
        /// Returns true if the end point has been visited.
        /// </summary>
        /// <returns></returns>
        public bool Solved()
        {
            return (squares[xEnd, yEnd].isVisited);
        }

        /// <summary>
        /// Returns the coordinates of the start point.
        /// </summary>
        /// <param name="xStart"></param>
        /// <param name="yStart"></param>
        public void GetStartCoordinates(out int xStart, out int yStart)
        {
            xStart = this.xStart;
            yStart = this.yStart;
        }
    }

    #endregion
}
