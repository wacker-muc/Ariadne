using System;
using System.Collections.Generic;
using System.Text;

namespace SWA.Ariadne.Model
{
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
        #region Properties
        public MazeSquare this[int x, int y]
        {
            get { return squares[x, y]; }
        }
        #endregion

        #endregion

        #region Constructor

        public Maze(int xSize, int ySize)
        {
            this.xSize = xSize;
            this.ySize = ySize;

            this.random = new Random();
        }

        #endregion

        #region Setup methods

        public void CreateMaze()
        {
            #region Create the squares.
            
            this.squares = new MazeSquare[xSize, ySize];
            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    squares[x, y] = new MazeSquare();
                }
            }

            #endregion

            #region Connect the squares with their neighbors.
            
            for (int x0 = 0; x0 < xSize; x0++)
            {
                for (int y0 = 0, y1 = 1; y1 < ySize; y0++, y1++)
                {
                    squares[x0, y0].SetNeighbor(MazeSquare.WallPosition.WP_S, squares[x0, y1]);
                    squares[x0, y1].SetNeighbor(MazeSquare.WallPosition.WP_N, squares[x0, y0]);
                }
            }
            for (int y0 = 0; y0 < ySize; y0++)
            {
                for (int x0 = 0, x1 = 1; x1 < xSize; x0++, x1++)
                {
                    squares[x0, y0].SetNeighbor(MazeSquare.WallPosition.WP_E, squares[x1, y0]);
                    squares[x1, y0].SetNeighbor(MazeSquare.WallPosition.WP_W, squares[x0, y0]);
                }
            }

            #endregion

            this.BuildMaze();
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

        #region Runtime methods

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

        #endregion

        #region Building a maze

        private void BuildMaze()
        {
            FixBorderWalls();
            FixReservedAreas();

            // We hold a number of active squares in a stack.
            // Make the initial capacity sufficient to hold all squares.
            //
            Stack<MazeSquare> stack = new Stack<MazeSquare>(xSize * ySize);

            #region Start with a single random cell in the stack.
            
            while (true)
            {
                int x = random.Next(xSize);
                int y = random.Next(ySize);
                MazeSquare sq = this[x, y];
                if (!sq.isReserved)
                {
                    sq.isConnected = true;
                    stack.Push(sq);
                    break;
                }
            }

            #endregion

            #region Extend the maze by visiting the cells next to those in the stack.
            
            while (stack.Count > 0)
            {
                List<MazeSquare.WallPosition> unresolvedWalls = new List<MazeSquare.WallPosition>(MazeSquare.WP_NUM);
                MazeSquare sq0 = stack.Pop();

                // Collect the unfixed walls of sq0.
                //
                //foreach (MazeSquare.WallPosition wp in Enum.GetValues(MazeSquare.WallPosition))
                for (MazeSquare.WallPosition wp = MazeSquare.WP_MIN; wp <= MazeSquare.WP_MAX; wp++)
                {
                    if (sq0[wp] == MazeSquare.WallState.WS_MAYBE)
                    {
                        MazeSquare sq = sq0.NeighborSquare(wp);

                        if (sq.isConnected)
                        {
                            sq0[wp] = sq[MazeSquare.OppositeWall(wp)] = MazeSquare.WallState.WS_CLOSED;
                        }
                        else
                        {
                            unresolvedWalls.Add(wp);
                        }
                    } // if WS_MAYBE
                } // foreach wp

                if (unresolvedWalls.Count == 0)
                {
                    continue; // no walls to choose from
                }

                // Choose one wall.
                MazeSquare.WallPosition wp0 = unresolvedWalls[random.Next(unresolvedWalls.Count)];
                MazeSquare sq1 = sq0.NeighborSquare(wp0);

                // Open the wall.
                sq0[wp0] = sq1[MazeSquare.OppositeWall(wp0)] = MazeSquare.WallState.WS_OPEN;

                // Add the current cell to the stack.
                if (unresolvedWalls.Count > 1)
                {
                    stack.Push(sq0);
                }

                // Add the new cell to the stack.
                sq1.isConnected = true;
                stack.Push(sq1);

            } // while stack is not empty

            #endregion
        }

        private void FixBorderWalls()
        {
            for (int x = 0; x < xSize; x++)
            {
                int y = ySize - 1;
                this.squares[x, 0][MazeSquare.WallPosition.WP_N] = MazeSquare.WallState.WS_CLOSED;
                this.squares[x, y][MazeSquare.WallPosition.WP_S] = MazeSquare.WallState.WS_CLOSED;
            }
            for (int y = 0; y < ySize; y++)
            {
                int x = xSize - 1;
                this.squares[0, y][MazeSquare.WallPosition.WP_W] = MazeSquare.WallState.WS_CLOSED;
                this.squares[x, y][MazeSquare.WallPosition.WP_E] = MazeSquare.WallState.WS_CLOSED;
            }
        }

        private void FixReservedAreas()
        {
            // TODO
        }

        #endregion
    }
}
