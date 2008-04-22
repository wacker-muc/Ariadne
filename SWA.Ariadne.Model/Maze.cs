using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SWA.Ariadne.Settings;

namespace SWA.Ariadne.Model
{
    public class Maze
        : IAriadneSettingsSource
    {
        #region Static properties, derived from MazeDimensions

        /// <summary>
        /// Minimum width or height: number of squares.
        /// </summary>
        public static int MinSize
        {
            get
            {
                return MazeDimensions.MinSize;
            }
        }

        /// <summary>
        /// Maximum width: number of squares.
        /// </summary>
        public static int MaxXSize
        {
            get
            {
                return MazeDimensions.MaxXSize;
            }
        }

        /// <summary>
        /// Maximum height: number of squares.
        /// </summary>
        public static int MaxYSize
        {
            get
            {
                return MazeDimensions.MaxYSize;
            }
        }

        /// <summary>
        /// Maximum distance of start/end point from border.
        /// </summary>
        private static int MaxBorderDistance
        {
            get
            {
                return MazeDimensions.MaxBorderDistance;
            }
        }

        #endregion

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
        /// Travel direction: 0..3
        /// </summary>
        private MazeSquare.WallPosition direction;

        /// <summary>
        /// A source of random numbers.
        /// </summary>
        private Random random;

        /// <summary>
        /// Maximum initial seed value: 2^13-1.
        /// This value and the other Max... values are chosen so that the maze Code may be represented with 12 characters.
        /// </summary>
        internal const int SeedLimit = 8192;

        /// <summary>
        /// The seed used to initialize this.random.
        /// </summary>
        private int seed;

        /// <summary>
        /// Position and dimensions of some reserved areas.
        /// </summary>
        private List<Rectangle> reservedAreas = new List<Rectangle>();

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
            this.xSize = Math.Max(MinSize, Math.Min(MaxXSize, xSize));
            this.ySize = Math.Max(MinSize, Math.Min(MaxYSize, ySize));

            // Get an initial random seed and use that to create the Random.
            Random r = RandomFactory.CreateRandom();
            this.seed = r.Next(SeedLimit);
            this.random = RandomFactory.CreateRandom(seed);
        }

        /// <summary>
        /// Constructor.
        /// Create a maze whose parameters are encoded in the given code (see property Code).
        /// </summary>
        /// <param name="code">a string of seven letters (case is ignored)</param>
        public Maze(string code)
        {
            Decode(code
                , out this.seed
                , out this.xSize, out this.ySize
                , out this.direction
                , out this.xStart, out this.yStart
                , out this.xEnd, out this.yEnd
                );
            this.random = RandomFactory.CreateRandom(seed);
        }

        /// <summary>
        /// Create a copy of this maze.
        /// </summary>
        /// <returns></returns>
        public Maze Clone()
        {
            Maze clone = new Maze(xSize, ySize);

            clone.xStart = this.xStart;
            clone.yStart = this.yStart;
            clone.xEnd = this.xEnd;
            clone.yEnd = this.yEnd;
            clone.direction = this.direction;
            clone.seed = this.seed;
            clone.reservedAreas = this.reservedAreas;

            clone.CreateSquares();

            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    for (MazeSquare.WallPosition wp = MazeSquare.WP_MIN; wp <= MazeSquare.WP_MAX; wp++)
                    {
                        clone[x, y][wp] = this[x, y][wp];
                    }
                }
            }

            return clone;
        }

        #endregion

        #region Encoding of the maze parameters

        public const int CodeLength = 12;
        public const int CodeDigitRange = 26; // 'A' .. 'Z'

        /// <summary>
        /// A string of twelve characters (A..Z) that encodes the maze parameters.
        /// This code can be used to construct an identical maze.
        /// </summary>
        public string Code
        {
            get
            {
                long nCode = 0;

                #region Encode the relevant parameters into a numeric code

                // Items are encoded in reverse order of decoding.
                // Some items must be encoded before others if decoding requires them.

                // Encode the start and end points.
                // Instead of the direct coordinates we will use the following information:
                // * travel direction
                // * distance from the border (instead of coordinate)
                // * other coordinate
                // The scaling factor is always MaxXSize, as it is greater than MaxYSize.

                int d1, d2, c1, c2;

                switch (direction)
                {
                    case MazeSquare.WallPosition.WP_E:
                        d1 = xStart;
                        d2 = xSize - 1 - xEnd;
                        c1 = yStart;
                        c2 = yEnd;
                        break;
                    case MazeSquare.WallPosition.WP_W:
                        d1 = xEnd;
                        d2 = xSize - 1 - xStart;
                        c1 = yEnd;
                        c2 = yStart;
                        break;
                    case MazeSquare.WallPosition.WP_S:
                        d1 = yStart;
                        d2 = ySize - 1 - yEnd;
                        c1 = xStart;
                        c2 = xEnd;
                        break;
                    case MazeSquare.WallPosition.WP_N:
                        d1 = yEnd;
                        d2 = ySize - 1 - yStart;
                        c1 = xEnd;
                        c2 = xStart;
                        break;
                    default:
                        d1 = d2 = c1 = c2 = -1;
                        break;
                }

                nCode *= (MaxBorderDistance + 1);
                nCode += d1;

                nCode *= (MaxBorderDistance + 1);
                nCode += d2;

                nCode *= (MaxXSize + 1);
                nCode += c1;

                nCode *= (MaxXSize + 1);
                nCode += c2;

                nCode *= MazeSquare.WP_NUM;
                nCode += (int)direction;

                // Encode maze dimension.

                nCode *= (MaxYSize - MinSize + 1);
                nCode += (ySize - MinSize);

                nCode *= (MaxXSize - MinSize + 1);
                nCode += (xSize - MinSize);

                // Encode initial seed.

                nCode *= SeedLimit;
                nCode += seed;

                #endregion

                // The resulting nCode is less than 26^12.  See SWA.Ariadne.Model.Tests unit tests.

                #region Convert the numeric code into a character code (base 26)

                StringBuilder result = new StringBuilder(7);

                for (int p = CodeLength; p-- > 0; )
                {
                    int digit = (int)(nCode % CodeDigitRange);
                    nCode /= CodeDigitRange;
                    result.Insert(0, (char)(digit + 'A'));
                }

                result.Insert(8, '.');
                result.Insert(4, '.');

                #endregion

                return result.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="seed"></param>
        /// <param name="xSize"></param>
        /// <param name="ySize"></param>
        /// <param name="direction"></param>
        /// <param name="xStart"></param>
        /// <param name="yStart"></param>
        /// <param name="xEnd"></param>
        /// <param name="yEnd"></param>
        /// <exception cref="ArgumentOutOfRangeException">decoded parameters are invalid</exception>
        private static void Decode(string code
            , out int seed
            , out int xSize, out int ySize
            , out MazeSquare.WallPosition direction
            , out int xStart, out int yStart
            , out int xEnd, out int yEnd
            )
        {
            long nCode = 0;

            #region Convert the character code (base 26) into a numeric code

            char[] a = code.Replace(".","").ToUpper().ToCharArray();

            if (!(a.Length == CodeLength))
            {
                throw new ArgumentOutOfRangeException("code", code, "Must be " + CodeLength.ToString() + " characters.");
            }

            for (int p = 0; p < a.Length; p++)
            {
                int digit = a[p] - 'A';
                if (!(0 <= digit && digit < CodeDigitRange))
                {
                    throw new ArgumentOutOfRangeException("code", code, "Allowed characters are 'A'..'Z' only.");
                }
                nCode *= CodeDigitRange;
                nCode += digit;
            }

            #endregion

            #region Decode items in the code in reverse order of encoding

            long nCodeOriginal = nCode; // for debugging

            long itemRange;

            itemRange = SeedLimit;
            seed = (int)(nCode % itemRange);
            nCode /= itemRange;

            itemRange = MaxXSize - MinSize + 1;
            xSize = (int)(nCode % itemRange) + MinSize;
            nCode /= itemRange;

            itemRange = MaxYSize - MinSize + 1;
            ySize = (int)(nCode % itemRange) + MinSize;
            nCode /= itemRange;

            int d1, d2, c1, c2;

            itemRange = MazeSquare.WP_NUM;
            direction = (MazeSquare.WallPosition)(nCode % itemRange);
            nCode /= itemRange;

            itemRange = MaxXSize + 1;
            c2 = (int)(nCode % itemRange);
            nCode /= itemRange;

            itemRange = MaxXSize + 1;
            c1 = (int)(nCode % itemRange);
            nCode /= itemRange;

            itemRange = MaxBorderDistance + 1;
            d2 = (int)(nCode % itemRange);
            nCode /= itemRange;

            itemRange = MaxBorderDistance + 1;
            d1 = (int)(nCode % itemRange);
            nCode /= itemRange;

            switch (direction)
            {
                case MazeSquare.WallPosition.WP_E:
                    xStart = d1;
                    xEnd = xSize - 1 - d2;
                    yStart = c1;
                    yEnd = c2;
                    break;
                case MazeSquare.WallPosition.WP_W:
                    xEnd = d1;
                    xStart = xSize - 1 - d2;
                    yEnd = c1;
                    yStart = c2;
                    break;
                case MazeSquare.WallPosition.WP_S:
                    yStart = d1;
                    yEnd = ySize - 1 - d2;
                    xStart = c1;
                    xEnd = c2;
                    break;
                case MazeSquare.WallPosition.WP_N:
                    yEnd = d1;
                    yStart = ySize - 1 - d2;
                    xEnd = c1;
                    xStart = c2;
                    break;
                default:
                    xStart = yStart = xEnd = yEnd = -1;
                    break;
            }

            #endregion

            #region Verify the validity of the decoded items

            if (!(nCode == 0))
            {
                throw new ArgumentOutOfRangeException("remainder(code)", seed, "Must be a zero.");
            }
            ValidateCodeItemRange("seed", seed, 0, SeedLimit-1);
            ValidateCodeItemRange("xSize", xSize, MinSize, MaxXSize);
            ValidateCodeItemRange("ySize", ySize, MinSize, MaxYSize);
            ValidateCodeItemRange("xStart", xStart, 0, xSize);
            ValidateCodeItemRange("yStart", yStart, 0, ySize);
            ValidateCodeItemRange("xEnd", xEnd, 0, xSize);
            ValidateCodeItemRange("yEnd", yEnd, 0, yEnd);

            #endregion
        }

        private static void ValidateCodeItemRange(string item, int value, int min, int max)
        {
            if (!(min <= value && value <= max))
            {
                throw new ArgumentOutOfRangeException(item + "(code)", value, "Must be between " + min.ToString() + " and " + max.ToString() + ".");
            }
        }

        #endregion

        #region Setup methods

        /// <summary>
        /// Reserves a rectangular region of the given dimensions at a random location.
        /// The area must not touch any other reserved ares.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="borderDistance">minimum number of squares between the reserved area and the maze border</param>
        /// <param name="rect"></param>
        /// <returns>true if the reservation was successful</returns>
        public bool ReserveRectangle(int width, int height, int borderDistance, out Rectangle rect)
        {
            // Reject very large areas.
            if (width < 2 || height < 2 || width > xSize - Math.Max(4, 2 * borderDistance) || height > ySize - Math.Max(4, 2 * borderDistance))
            {
                rect = new Rectangle();
                return false;
            }

            for (int nTries = 0; nTries < 100; nTries++)
            {
                // Choose a random location.
                // The resulting rectangle may touch the borders.
                int x = random.Next(borderDistance, xSize - width - borderDistance);
                int y = random.Next(borderDistance, ySize - width - borderDistance);

                if (ReserveRectangle(x, y, width, height))
                {
                    rect = new Rectangle(x, y, width, height);
                    return true;
                }
            }

            rect = new Rectangle();
            return false;
        }

        /// <summary>
        /// Reserves a rectanglurar region of the given dimensions.
        /// The area must not touch any other reserved ares.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns>true if the reservation was successful</returns>
        public bool ReserveRectangle(int x, int y, int width, int height)
        {
            // Restrict to the actual maze area.
            if (x < 0)
            {
                width += x;
                x = 0;
            }
            if (y < 0)
            {
                height += y;
                y = 0;
            }
            width = Math.Min(xSize - x, width);
            height = Math.Min(ySize - y, height);

            // Reject very large areas.
            if (width < 1 || height < 1 || width > xSize - 4 || height > ySize - 4)
            {
                return false;
            }

            // The candidate rectangle.
            Rectangle candidate = new Rectangle(x, y, width, height);

            // The candidate, extended with one square around all four edges.
            Rectangle extendedCandidate = new Rectangle(x - 1, y - 1, width + 2, height + 2);

            bool reject = false;
            foreach (Rectangle rect in this.reservedAreas)
            {
                // Reject the candidate if its extension would intersect with another reserved area.
                if (extendedCandidate.IntersectsWith(rect))
                {
                    reject = true;
                }
            }

            if (!reject)
            {
                reservedAreas.Add(candidate);
                return true;
            }

            return false;
        }

        public void CreateMaze()
        {
            this.CreateSquares();
            this.BuildMaze();
        }

        private void CreateSquares()
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
                this.direction = (MazeSquare.WallPosition) this.random.Next(4);

                // a small portion of the maze size (in travel direction)
                int edgeWidth = 0;
                switch (direction)
                {
                    case MazeSquare.WallPosition.WP_N:
                    case MazeSquare.WallPosition.WP_S:
                        // vertical
                        edgeWidth = 2 + ySize * 2 / 100;
                        break;
                    case MazeSquare.WallPosition.WP_E:
                    case MazeSquare.WallPosition.WP_W:
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

                edgeDistStart = Math.Min(edgeDistStart, MaxBorderDistance);
                edgeDistEnd = Math.Min(edgeDistEnd, MaxBorderDistance);

                // The two rows (or columns) that make up the (positive) travel direction.
                int lesserRow = 0, greaterRow = 0;

                switch (direction)
                {
                    case MazeSquare.WallPosition.WP_N:
                        // start at bottom, end at top
                        xStart = random.Next(xSize);
                        yStart = greaterRow = ySize - 1 - edgeDistStart;
                        xEnd = random.Next(xSize);
                        yEnd = lesserRow = edgeDistEnd;
                        break;
                    case MazeSquare.WallPosition.WP_E:
                        // start at left, end at right
                        xStart = lesserRow = edgeDistEnd;
                        yStart = random.Next(ySize);
                        xEnd = greaterRow = xSize - 1 - edgeDistStart;
                        yEnd = random.Next(ySize);
                        break;
                    case MazeSquare.WallPosition.WP_S:
                        // start at top, end at bottom
                        xStart = random.Next(xSize);
                        yStart = lesserRow = edgeDistEnd;
                        xEnd = random.Next(xSize);
                        yEnd = greaterRow = ySize - 1 - edgeDistStart;
                        break;
                    case MazeSquare.WallPosition.WP_W:
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

                        if (sq.isConnected || sq.isReserved)
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

        /// <summary>
        /// Put closed walls around the maze.
        /// </summary>
        private void FixBorderWalls()
        {
            CloseWalls(0, xSize, 0, ySize, MazeSquare.WallState.WS_CLOSED);
        }

        /// <summary>
        /// Mark the squares inside the reserved areas and put walls around them.
        /// </summary>
        private void FixReservedAreas()
        {
            foreach (Rectangle rect in this.reservedAreas)
            {
                for (int x = rect.Left; x < rect.Right; x++)
                {
                    for (int y = rect.Top; y < rect.Bottom; y++)
                    {
                        this[x, y].isReserved = true;
                    }
                }

                // Close the walls around the area but open the walls on the border.
                CloseWalls(rect.Left, rect.Right, rect.Top, rect.Bottom, MazeSquare.WallState.WS_OPEN);
            }
        }

        /// <summary>
        /// Put closed walls around the given rectangle.
        /// </summary>
        /// <param name="left">left border x coordinate (inside)</param>
        /// <param name="right">right border x coordinate (outside)</param>
        /// <param name="top">top border y coordinate (inside)</param>
        /// <param name="bottom">bottom border y coordinate (outside)</param>
        /// <param name="borderState">WallState to be applied to walls on the border of the maze</param>
        private void CloseWalls(int left, int right, int top, int bottom, MazeSquare.WallState borderState)
        {
            for (int x = left; x < right; x++)
            {
                int y1 = top;
                int y2 = bottom - 1;
                this.squares[x, y1][MazeSquare.WallPosition.WP_N] = (y1 == 0 ? borderState : MazeSquare.WallState.WS_CLOSED);
                this.squares[x, y2][MazeSquare.WallPosition.WP_S] = (y2+1 == ySize ? borderState : MazeSquare.WallState.WS_CLOSED);
            }
            for (int y = top; y < bottom; y++)
            {
                int x1 = left;
                int x2 = right - 1;
                this.squares[x1, y][MazeSquare.WallPosition.WP_W] = (x1 == 0 ? borderState : MazeSquare.WallState.WS_CLOSED);
                this.squares[x2, y][MazeSquare.WallPosition.WP_E] = (x2+1 == xSize ? borderState : MazeSquare.WallState.WS_CLOSED);
            }
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

        public override string ToString()
        {
            return string.Format("{0}: [{1},{2}], ({3},{4}) -> ({5},{6})", Code, XSize, YSize, xStart, yStart, xEnd, yEnd);
        }

        #endregion

        #region IAriadneSettingsSource implementation

        /// <summary>
        /// Fill all modifyable parameters into the given data object.
        /// </summary>
        /// <param name="data"></param>
        public void FillParametersInto(AriadneSettingsData data)
        {
            data.MazeWidth = this.xSize;
            data.MazeHeight = this.ySize;
            data.Seed = this.seed;
            data.Code = this.Code;
        }

        /// <summary>
        /// Take all modifyable parameters from the given data object.
        /// </summary>
        /// <param name="data"></param>
        public void TakeParametersFrom(AriadneSettingsData data)
        {
            // The Auto... flags for Width and Height have already been checked by the MazeUserControl.
            this.xSize = Math.Max(MinSize, Math.Min(MaxXSize, data.MazeWidth));
            this.ySize = Math.Max(MinSize, Math.Min(MaxYSize, data.MazeHeight));

            if (!data.AutoSeed)
            {
                this.seed = Math.Max(0, Math.Min(SeedLimit - 1, data.Seed));
            }
            else
            {
                Random r = RandomFactory.CreateRandom();
                this.seed = r.Next(SeedLimit);
            }
            this.random = RandomFactory.CreateRandom(seed);

            this.reservedAreas.Clear();

            // Decode(data.Code);
        }

        #endregion
    }

    #region class MazeDimensions

    /// <summary>
    /// Provides Maze dimension limits based on the desired Maze.Code length.
    /// </summary>
    internal class MazeDimensions
    {
        private const double XYRatio = (4.0/3.0); // e.g. 1024x768

        /// <summary>
        /// Minimum width or height: number of squares.
        /// </summary>
        public static readonly int MinSize = 4;

        /// <summary>
        /// Maximum width: number of squares.
        /// </summary>
        public static int MaxXSize
        {
            get
            {
                return MinSize + xRange;
            }
        }
        private static readonly int xRange = 337; // expected value, manually calculated

        /// <summary>
        /// Maximum height: number of squares.
        /// </summary>
        public static int MaxYSize
        {
            get
            {
                return MinSize + yRange;
            }
        }
        private static readonly int yRange = 251; // expected value, manually calculated

        /// <summary>
        /// Maximum distance of start/end point from border.
        /// </summary>
        public static readonly int MaxBorderDistance = 16;

        /// <summary>
        /// Calculate maximum x and y dimensions, based on the desired Maze.Code length.
        /// </summary>
        static MazeDimensions()
        {
            double codeLimit = Math.Pow(Maze.CodeDigitRange, Maze.CodeLength);

            if (codeLimit > long.MaxValue)
            {
                throw new Exception("Maze.Code is too large to be represented as a 64 bit integer");
            }

            codeLimit /= Maze.SeedLimit;
            //           (MaxXSize - MinSize + 1)
            //           (MaxYSize - MinSize + 1)
            codeLimit /= MazeSquare.WP_NUM;
            codeLimit /= (MaxBorderDistance + 1);
            codeLimit /= (MaxBorderDistance + 1);
            //           (MaxXSize + 1)
            //           (MaxXSize + 1)

            /* We want to find the greatest integer MaxXSize and MaxYSize with the limitation:
             *          (x-m) * (y-m) * x * x < c
             * with:
             *          x = MaxXSize + 1
             *          y = MaxYSize + 1  =  MaxXSize / XYRatio + 1
             *          m = MinSize
             *          c = codeLimit
             *          r = XYRatio
             * 
             * This is approximately equivalent to:
             *          x*x*x*x < c*r
             * or
             *          x = (c*r)^^(1/4)
             * With m>0, that x is even too small.
             */

            double x = Math.Truncate(Math.Pow(codeLimit * XYRatio, 0.25));

            while ((x - MinSize) * (x / XYRatio - MinSize) * (x) * (x) < codeLimit)
            {
                x = x + 1;
            }

            /* Now, x is 1 greater than acceptable, i.e.
             *          x-1  =  MaxXSize + 1  =  MinSize + xRange + 1
             *          MinSize + yRange  =  MaxYSize  =  MaxXSize / XYRatio
             */
            xRange = (int)(x - MinSize - 2);
            yRange = (int)(MaxXSize / XYRatio - MinSize); // Note: MaxXSize is valid after xRange has been assigned
        }
    }

    #endregion
}
