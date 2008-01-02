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
        private readonly int xSize, ySize;
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
        /// Minimum and maximum maze dimensions: number of squares.
        /// </summary>
        public const int MinSize = 8, MaxXSize = 600, MaxYSize = 400;

        /// <summary>
        /// Coordinates of the start and end point of the path through the maze.
        /// </summary>
        private int xStart, yStart, xEnd, yEnd;

        /// <summary>
        /// A source of random numbers.
        /// </summary>
        private readonly Random random;

        /// <summary>
        /// The seed used to initialize this.random.
        /// </summary>
        private readonly int seed;

        /// <summary>
        /// Position and dimensions of a reserved area.
        /// TODO: Support more than one reserved area.
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

        /// <summary>
        /// Constructor.
        /// Create a maze with the given dimensions.
        /// </summary>
        /// <param name="xSize"></param>
        /// <param name="ySize"></param>
        public Maze(int xSize, int ySize)
        {
            this.xSize = Math.Min(xSize, MaxXSize);
            this.ySize = Math.Min(ySize, MaxYSize);

            // Get an initial random seed and use that to create the Random.
            Random r = new Random();
            this.seed = r.Next();
            this.random = new Random(seed);
        }

        /// <summary>
        /// Constructor.
        /// Create a maze whose parameters are encoded in the given code (see property Code).
        /// </summary>
        /// <param name="code">a string of seven letters (case is ignored)</param>
        public Maze(string code)
        {
            Decode(code, out this.xSize, out this.ySize, out this.seed);
            this.random = new Random(seed);
        }

        #endregion

        #region Encoding of the maze parameters

        /// <summary>
        /// A string of seven characters (A..Z) that encodes the maze parameters.
        /// This code can be used to construct an identical maze.
        /// </summary>
        public string Code
        {
            get
            {
                long nCode = 0;

                nCode += seed;              // < 32.768

                nCode *= (MaxXSize + 1);    // < 19,693,568
                nCode += xSize;             // < 19,694,168

                nCode *= (MaxYSize + 1);    // < 7,897,361,368
                nCode += ySize;             // < 7,897,361,768

                // range = Math.Pow(26, 7); // = 8,031,810,176

                StringBuilder result = new StringBuilder(7);

                for (int p = 7; p-- > 0; )
                {
                    int digit = (int)(nCode % 26);
                    nCode /= 26;
                    result.Insert(0, (char)(digit + 'A'));
                }

                return result.ToString();
            }
        }

        private static void Decode(string code, out int xSize, out int ySize, out int seed)
        {
            long nCode = 0;

            if (!(code.Length == 7))
            {
                throw new ArgumentOutOfRangeException("code", code, "Must be seven characters.");
            }

            char[] a = code.ToUpper().ToCharArray();
            for (int p = 0; p++ < code.Length; p++)
            {
                int digit = a[p] - 'A';
                if (!(0 <= digit && digit < 26))
                {
                    throw new ArgumentOutOfRangeException("code", code, "Allowed characters are 'A'..'Z' only.");
                }
                nCode *= 26;
                nCode += digit;
            }

            ySize = (int)(nCode % (MaxYSize + 1));
            nCode /= (MaxYSize + 1);

            xSize = (int)(nCode % (MaxXSize + 1));
            nCode /= (MaxXSize + 1);

            seed = (int)nCode;

            if (!(seed >= 0))
            {
                throw new ArgumentOutOfRangeException("seed(code)", seed, "Must be a nonnegative integer.");
            }
            if (!(MinSize <= xSize && xSize <= MaxXSize))
            {
                throw new ArgumentOutOfRangeException("xSize(code)", xSize, "Must be between " + MinSize.ToString() + " and " + MaxXSize.ToString() + ".");
            }
            if (!(MinSize <= ySize && ySize <= MaxYSize))
            {
                throw new ArgumentOutOfRangeException("ySize(code)", ySize, "Must be between " + MinSize.ToString() + " and " + MaxYSize.ToString() + ".");
            }

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
                    squares[x, y] = new MazeSquare(x, y);
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

        /// <summary>
        /// Choose a start and end point near opposite borders.
        /// The end point should be in a dead end (a square with three closed walls).
        /// </summary>
        public void PlaceEndpoints()
        {
            bool reject = true;
            while (reject)
            {
                // Choose a travel direction (one of four)
                int direction = this.random.Next(4);

                // a small portion of the maze size (in travel direction)
                int edgeWidth = 0;
                switch (direction)
                {
                    case 0:
                    case 2:
                        // vertical
                        edgeWidth = 2 + ySize * 2 / 100;
                        break;
                    case 1:
                    case 3:
                        // horizontal
                        edgeWidth = 2 + xSize * 2 / 100;
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

                // The two rows (or columns) that make up the (positive) travel direction.
                int lesserRow = 0, greaterRow = 0;

                switch (direction)
                {
                    case 0:
                        // start at bottom, end at top
                        xStart = random.Next(xSize);
                        yStart = greaterRow = ySize - 1 - edgeDistStart;
                        xEnd = random.Next(xSize);
                        yEnd = lesserRow = edgeDistEnd;
                        break;
                    case 1:
                        // start at left, end at right
                        xStart = lesserRow = edgeDistEnd;
                        yStart = random.Next(ySize);
                        xEnd = greaterRow = xSize - 1 - edgeDistStart;
                        yEnd = random.Next(ySize);
                        break;
                    case 2:
                        // start at top, end at bottom
                        xStart = random.Next(xSize);
                        yStart = lesserRow = edgeDistEnd;
                        xEnd = random.Next(xSize);
                        yEnd = greaterRow = ySize - 1 - edgeDistStart;
                        break;
                    case 3:
                        // start at right, end at left
                        xStart = greaterRow = xSize - 1 - edgeDistStart;
                        yStart = random.Next(ySize);
                        xEnd = lesserRow = edgeDistEnd;
                        yEnd = random.Next(ySize);
                        break;
                }

                #region Reject unusable squares

                // Verify that the endpoints are not in the restricted area.
                //
                reject = (squares[xStart,yStart].isReserved || squares[xEnd,yEnd].isReserved);

                // Verify that the squares are not aligned against the intended travel direction.
                // This also eliminates two other cases: same square and squares outside the maze.
                //
                if (lesserRow >= greaterRow)
                {
                    reject = true;
                }

                // Prefer real dead ends.
                // Reject an end point with less than three walls (with probability 90%).
                //
                if ((this.CountClosedWalls(squares[xEnd, yEnd]) < MazeSquare.WP_NUM - 1) && (random.Next(100) < 90))
                {
                    reject = true;
                }

                #endregion
            }
        }

        /// <summary>
        /// Reset to the initial state (before the maze is solved).
        /// </summary>
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
        public bool IsSolved
        {
            get { return (squares[xEnd, yEnd].isVisited); }
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

        /// <summary>
        /// Returns the coordinates of the end point.
        /// </summary>
        /// <param name="xEnd"></param>
        /// <param name="yEnd"></param>
        public void GetEndCoordinates(out int xEnd, out int yEnd)
        {
            xEnd = this.xEnd;
            yEnd = this.yEnd;
        }

        public MazeSquare StartSquare
        {
            get { return this[xStart, yStart]; }
        }

        public MazeSquare EndSquare
        {
            get { return this[xEnd, yEnd]; }
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

        #region Auxiliary methods

        private int CountClosedWalls(MazeSquare sq)
        {
            int result = 0;

            for (MazeSquare.WallPosition wp = MazeSquare.WP_MIN; wp <= MazeSquare.WP_MAX; wp++)
            {
                if (sq.walls[(int)wp] == MazeSquare.WallState.WS_CLOSED)
                {
                    ++result;
                }
            }

            return result;
        }

        #endregion
    }
}
