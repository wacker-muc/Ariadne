using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SWA.Ariadne.Outlines;
using SWA.Ariadne.Settings;

namespace SWA.Ariadne.Model
{
    public class Maze
        : IAriadneSettingsSource
    {
        #region Member variables

        /// <summary>
        /// Provides minimum and maximum dimension parameters.
        /// </summary>
        private MazeDimensions dimensionsObj;

        /// <summary>
        /// Can encode and decode maze parameters into maze ID strings.
        /// </summary>
        private MazeCode codeObj;

        /// <summary>
        /// The primary maze has ID = 1.
        /// </summary>
        protected virtual int MazeId
        {
            get { return MazeSquare.PrimaryMazeId; }
        }

        /// <summary>
        /// Maze dimension: number of rows.
        /// </summary>
        public int YSize
        {
            get { return ySize; }
        }
        /// <summary>
        /// Maze dimension: number of columns.
        /// </summary>
        public int XSize
        {
            get { return xSize; }
        }
        protected int xSize, ySize;

        /// <summary>
        /// Coordinates of the start and end point of the path through the maze.
        /// </summary>
        private int xStart, yStart, xEnd, yEnd;

        /// <summary>
        /// Travel direction: 0..3
        /// </summary>
        private MazeSquare.WallPosition direction;
        internal MazeSquare.WallPosition Direction
        {
            get { return direction; }
        }

        /// <summary>
        /// The source of random numbers specific to this maze.
        /// </summary>
        public Random Random
        {
            get { return this.random; }
        }
        protected Random random;

        /// <summary>
        /// The seed used to initialize this.random.
        /// </summary>
        public int Seed
        {
            get { return seed; }
        }
        private int seed;

        /// <summary>
        /// Position and dimensions of some reserved areas.
        /// </summary>
        private List<Rectangle> reservedAreas = new List<Rectangle>();

        /// <summary>
        /// A reserved area defined by the inside of an OutlineShape.
        /// </summary>
        private OutlineShape reservedShape = null;

        /// <summary>
        /// The shapes of some EmbeddedMazes (before they are actually created).
        /// </summary>
        private List<OutlineShape> embeddedMazeShapes = new List<OutlineShape>();

        private List<EmbeddedMaze> embeddedMazes = new List<EmbeddedMaze>();

        /// <summary>
        /// Most of the outline of this shape will be turned into closed walls.
        /// </summary>
        public OutlineShape OutlineShape
        {
            get
            {
                return this.outlineShape;
            }
            set
            {
                this.outlineShape = value;
            }
        }
        private OutlineShape outlineShape = null;

        /// <summary>
        /// The maze is formed by a two-dimensional array of squares.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public virtual MazeSquare this[int x, int y]
        {
            get { return squares[x, y]; }
        }
        private MazeSquare[,] squares;

        /// <summary>
        /// Returns the number of squares that are not reserved.
        /// This includes the squares of embedded mazes.
        /// </summary>
        public virtual int CountSquares
        {
            get
            {
                int result = xSize * ySize;
                
                foreach (MazeSquare sq in squares)
                {
                    if (sq.isReserved)
                    {
                        --result;
                    }
                }

                return result;
            }
        }

        IrregularMazeShape irregularMazeShape = null;

        /// <summary>
        /// When true, a uniform (completely random) maze is built.
        /// When false, specific path shapes are preferred.
        /// </summary>
        public bool Irregular
        {
            get
            {
                return (irregularMazeShape != null);
            }
            set
            {
                if (value == false)
                {
                    irregularMazeShape = null;
                }
                else
                {
                    irregularMazeShape = IrregularMazeShape.RandomInstance(this.random, this);
                }
            }
        }

        /// <summary>
        /// Percentage of cases when the irregularMazeShape is applied, range 0..100.
        /// </summary>
        public int Irregularity
        {
            get { return irregularity; }
            set { irregularity = value; }
        }
        private int irregularity = 80;

        #endregion

        #region Delegates

        /// <summary>
        /// A delegate type that implements the OutlineShape behavior.
        /// Returns true if the square at (x, y) is inside of the shape, false otherwise.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private delegate bool InsideShapeDelegate(int x, int y);

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// Create a maze with the given dimensions.
        /// </summary>
        /// <param name="xSize"></param>
        /// <param name="ySize"></param>
        public Maze(int xSize, int ySize)
            : this(xSize, ySize, MazeCode.DefaultCodeVersion)
        {
        }

        /// <summary>
        /// Constructor.
        /// Create a maze with the given dimensions.
        /// </summary>
        /// <param name="xSize"></param>
        /// <param name="ySize"></param>
        /// <param name="version"></param>
        public Maze(int xSize, int ySize, int version)
            : this(xSize, ySize, version, -1)
        {
        }

        /// <summary>
        /// Constructor.
        /// Create a maze with the given dimensions.
        /// </summary>
        /// <param name="xSize"></param>
        /// <param name="ySize"></param>
        /// <param name="version"></param>
        /// <param name="seed"></param>
        internal Maze(int xSize, int ySize, int version, int seed)
        {
            this.dimensionsObj = MazeDimensions.Instance(version);
            this.codeObj = MazeCode.Instance(version);

            this.xSize = Math.Max(dimensionsObj.MinSize, Math.Min(dimensionsObj.MaxXSize, xSize));
            this.ySize = Math.Max(dimensionsObj.MinSize, Math.Min(dimensionsObj.MaxYSize, ySize));

            // Get an initial random seed and use that to create the Random.
            if (seed < 0)
            {
                Random r = RandomFactory.CreateRandom();
                this.seed = r.Next(codeObj.SeedLimit);
            }
            else
            {
                this.seed = seed;
            }
            this.random = RandomFactory.CreateRandom(this.seed);
        }

        /// <summary>
        /// Constructor.
        /// Create a maze whose parameters are encoded in the given code (see property Code).
        /// </summary>
        /// <param name="code">a string of seven letters (case is ignored)</param>
        public Maze(string code)
        {
            int version = MazeCode.GetCodeVersion(code);
            this.dimensionsObj = MazeDimensions.Instance(version);
            this.codeObj = MazeCode.Instance(version);

            codeObj.Decode(code
                , out this.seed
                , out this.xSize, out this.ySize
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
            clone.reservedShape = this.reservedShape;
            clone.embeddedMazeShapes = this.embeddedMazeShapes;
            clone.embeddedMazes = new List<EmbeddedMaze>(embeddedMazes.Count);
            foreach (EmbeddedMaze em in embeddedMazes)
            {
#if false
                // TODO: em.Clone(clone)
                clone.embeddedMazes.Add((EmbeddedMaze)em.Clone());
#endif
            }

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

        /// <summary>
        /// A string that encodes the maze parameters.
        /// This code can be used to construct an identical maze.
        /// </summary>
        public string Code
        {
            get
            {
                return codeObj.Code(this);
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
                int y = random.Next(borderDistance, ySize - height - borderDistance);

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
        /// The area must not touch any other reserved areas.
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

            // The candidate, extended with two squares around all four edges.
            Rectangle extendedCandidate = new Rectangle(x - 2, y - 2, width + 4, height + 4);

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

        public bool ReserveShape(OutlineShape shape)
        {
            // TODO: Test if the remaining area is still connected.

            this.reservedShape = shape;

            return true;
        }

        public bool AddEmbeddedMaze(OutlineShape shape)
        {
            this.embeddedMazeShapes.Add(shape);
            return true;
        }

        public void CreateMaze()
        {
            // Create all MazeSquare objects.
            CreateSquares();

            // Fix reserved areas.
            FixReservedAreas();
            FixReservedShape();
            CloseWallsAroundReservedAreas(); // TODO: This might be discarded.
            
            // Divide the area into a main maze and several embedded mazes.
            FixEmbeddedMazes();
            
            // Put walls around the outline shape and the whole maze.
            FixOutlineShape();
            FixBorderWalls();

            // Construct the inner walls of the main maze and choose a start and tartget square.
            BuildMaze();
            PlaceEndpoints();

            // Do the same for all embedded mazes.
            foreach (EmbeddedMaze m in this.embeddedMazes)
            {
                m.BuildMaze();
                m.PlaceEndpoints();
            }
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
        private void PlaceEndpoints()
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

                edgeDistStart = Math.Min(edgeDistStart, dimensionsObj.MaxBorderDistance);
                edgeDistEnd = Math.Min(edgeDistEnd, dimensionsObj.MaxBorderDistance);

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

                // Verify that the endpoints are actually part of this maze.
                //
                reject = (this[xStart, yStart].MazeId != this.MazeId || this[xEnd, yEnd].MazeId != this.MazeId);

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
                if ((CountClosedWalls(this[xEnd, yEnd]) < MazeSquare.WP_NUM - 1) && (random.Next(100) < 90))
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
            // We hold a number of active squares in a stack.
            // Make the initial capacity sufficient to hold all squares.
            //
            Stack<MazeSquare> stack = new Stack<MazeSquare>(xSize * ySize);

            List<MazeSquare> outlineSquares = new List<MazeSquare>();
            List<MazeSquare.WallPosition> outlineWalls = new List<MazeSquare.WallPosition>();

            #region Start with a single random cell in the stack.
            
            while (true)
            {
                int x = random.Next(xSize);
                int y = random.Next(ySize);
                MazeSquare sq = this[x, y];
                if (sq.MazeId == this.MazeId)
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
                    switch (sq0[wp])
                    {
                        case MazeSquare.WallState.WS_MAYBE:
                            MazeSquare sq = sq0.NeighborSquare(wp);

                            if (sq.isConnected || sq.MazeId != sq0.MazeId)
                            {
                                sq0[wp] = sq[MazeSquare.OppositeWall(wp)] = MazeSquare.WallState.WS_CLOSED;
                            }
                            else
                            {
                                unresolvedWalls.Add(wp);
                            }
                            break; // WS_MAYBE

                        case MazeSquare.WallState.WS_OUTLINE:
                            outlineSquares.Add(sq0);
                            outlineWalls.Add(wp);
                            break; // WS_OUTLINE
                    }
                } // foreach wp

                // Discard this square if it has no unresolved walls.
                if (unresolvedWalls.Count == 0)
                {
                    // Note: This is the only place that may end the loop.
                    // If the stack is empty: Open one outline wall.
                    if (stack.Count == 0)
                    {
                        while (outlineSquares.Count > 0)
                        {
                            // Select a random square with an outline wall.
                            int p = random.Next(outlineSquares.Count);
                            MazeSquare sq = outlineSquares[p];
                            MazeSquare.WallPosition wp = outlineWalls[p];
                            outlineSquares.RemoveAt(p);
                            outlineWalls.RemoveAt(p);

                            if (sq[wp] == MazeSquare.WallState.WS_OUTLINE)
                            {
                                sq[wp] = MazeSquare.WallState.WS_MAYBE;
                                stack.Push(sq);
                                // This square will be used in the next iteration.
                                break; // from while(outlineSquares)
                            }
                        }
                    }

                    continue; // no walls to choose from
                }

                // Add the current cell to the stack.
                // Note: Do this before replacing unresolvedWalls with preferredWalls.
                if (unresolvedWalls.Count > 1)
                {
                    stack.Push(sq0);
                }

                // Use only preferred wall positions.
                if (unresolvedWalls.Count > 1 && irregularMazeShape != null && (random.Next(100) < this.irregularity))
                {
                    bool[] preferredPositions = irregularMazeShape.PreferredDirections(sq0);
                    List<MazeSquare.WallPosition> preferredWalls = new List<MazeSquare.WallPosition>(unresolvedWalls.Count);
                    foreach (MazeSquare.WallPosition p in unresolvedWalls)
                    {
                        if (preferredPositions[(int)p])
                        {
                            preferredWalls.Add(p);
                        }
                    }
                    if (preferredWalls.Count > 0)
                    {
                        unresolvedWalls = preferredWalls;
                    }
                }

                // Choose one wall.
                MazeSquare.WallPosition wp0 = unresolvedWalls[random.Next(unresolvedWalls.Count)];
                MazeSquare sq1 = sq0.NeighborSquare(wp0);

                // Open the wall.
                sq0[wp0] = sq1[MazeSquare.OppositeWall(wp0)] = MazeSquare.WallState.WS_OPEN;

                // Add the new cell to the stack.
                sq1.isConnected = true;
                stack.Push(sq1);

            } // while stack is not empty

            #endregion
        }

        /// <summary>
        /// Put closed walls around the maze.
        /// Next to reserved squares, the walls will be open instead of closed.
        /// </summary>
        private void FixBorderWalls()
        {
            int x1 = 0, x2 = xSize - 1, y1 = 0, y2 = ySize - 1;
            MazeSquare.WallState open = MazeSquare.WallState.WS_OPEN, closed = MazeSquare.WallState.WS_CLOSED;

            for (int x = 0; x < xSize; x++)
            {
                this.squares[x, y1][MazeSquare.WallPosition.WP_N] = (this.squares[x, y1].isReserved ? open : closed);
                this.squares[x, y2][MazeSquare.WallPosition.WP_S] = (this.squares[x, y2].isReserved ? open : closed);
            }
            for (int y = 0; y < ySize; y++)
            {
                this.squares[x1, y][MazeSquare.WallPosition.WP_W] = (this.squares[x1, y].isReserved ? open : closed);
                this.squares[x2, y][MazeSquare.WallPosition.WP_E] = (this.squares[x2, y].isReserved ? open : closed);
            }
        }

        /// <summary>
        /// Mark the squares inside the reserved areas.
        /// </summary>
        private void FixReservedAreas()
        {
            foreach (Rectangle rect in this.reservedAreas)
            {
                for (int x = rect.Left; x < rect.Right; x++)
                {
                    for (int y = rect.Top; y < rect.Bottom; y++)
                    {
                        this.squares[x, y].isReserved = true;
                    }
                }

#if false
                // Close the walls around the area but open the walls on the border.
                CloseWalls(rect.Left, rect.Right, rect.Top, rect.Bottom, MazeSquare.WallState.WS_OPEN);
#endif
            }
        }

        /// <summary>
        /// Mark the squares inside the reserved shape.
        /// </summary>
        /// Note: This method must not be called before FixReservedAreas().
        private void FixReservedShape()
        {
            if (reservedShape != null)
            {
                for (int x = 0; x < this.XSize; x++)
                {
                    for (int y = 0; y < this.YSize; y++)
                    {
                        if (reservedShape[x, y] == true)
                        {
                            this.squares[x, y].isReserved = true;
                        }
                    }
                }

#if false
                FixOutline(reservedShape, MazeSquare.WallState.WS_CLOSED);
#endif

                // TODO: open the walls on the border if touched by the reserved shape.
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

        /// <summary>
        /// Put closed walls around the reserved areas.
        /// </summary>
        private void CloseWallsAroundReservedAreas()
        {
            // We need a test that regards the reserved squares as the "inside" of a shape.
            InsideShapeDelegate test = delegate(int x, int y) { return this.squares[x, y].isReserved; };
            
            this.FixOutline(test, MazeSquare.WallState.WS_CLOSED);
        }

        /// <summary>
        /// Mark the defined outline walls.
        /// </summary>
        private void FixOutlineShape()
        {
            if (outlineShape != null)
            {
                // We need a test for the "inside" of an OutlineShape.
                InsideShapeDelegate test = delegate(int x, int y) { return outlineShape[x, y]; };

                FixOutline(test, MazeSquare.WallState.WS_OUTLINE);
            }
        }

        /// <summary>
        /// Apply the border of the given shape (where neighboring entries have opposite values) to the maze.
        /// The corresponding walls are switched from WS_MAYBE to WS_OUTLINE.
        /// </summary>
        /// <param name="shape">
        /// A two dimensional array.  The dimensions must not be greater than the maze itself.
        /// true means "inside", false means "outside".
        /// </param>
        private void FixOutline(InsideShapeDelegate shapeTest, MazeSquare.WallState wallState)
        {
            for (int x = 0; x < xSize; x++)
            {
                for (int y1 = 0, y2 = 1; y2 < ySize; y1++, y2++)
                {
                    bool b1 = shapeTest(x, y1), b2 = shapeTest(x, y2);
                    bool bothReserved = (this[x, y1].isReserved && this[x, y2].isReserved);
                    if (b1 != b2 && !bothReserved && this[x, y1][MazeSquare.WallPosition.WP_S] == MazeSquare.WallState.WS_MAYBE)
                    {
                        this[x, y1][MazeSquare.WallPosition.WP_S] = wallState;
                        this[x, y2][MazeSquare.WallPosition.WP_N] = wallState;
                    }
                }
            }
            for (int y = 0; y < ySize; y++)
            {
                for (int x1 = 0, x2 = 1; x2 < xSize; x1++, x2++)
                {
                    bool b1 = shapeTest(x1, y), b2 = shapeTest(x2, y);
                    bool bothReserved = (this[x1, y].isReserved && this[x2, y].isReserved);
                    if (b1 != b2 && !bothReserved && this[x1, y][MazeSquare.WallPosition.WP_E] == MazeSquare.WallState.WS_MAYBE)
                    {
                        this[x1, y][MazeSquare.WallPosition.WP_E] = wallState;
                        this[x2, y][MazeSquare.WallPosition.WP_W] = wallState;
                    }
                }
            }
        }

        #endregion

        #region Building embedded mazes

        private void FixEmbeddedMazes()
        {
            for (int i = 0; i < embeddedMazeShapes.Count; i++)
            {
                int embeddedMazeId = this.MazeId + 1 + i;

                if (embeddedMazeId > MazeSquare.MaxMazeId)
                {
                    break;
                }

                EmbeddedMaze embeddedMaze = new EmbeddedMaze(this, embeddedMazeId, embeddedMazeShapes[i]);

                this.embeddedMazes.Add(embeddedMaze);
            }

            // TODO: Make sure that every maze is totally connected.
        }

        #endregion

        #region Auxiliary methods

        private static int CountClosedWalls(MazeSquare sq)
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

        /// <summary>
        /// Returns the euclidian distance between two squares.
        /// </summary>
        /// <param name="sq1"></param>
        /// <param name="sq2"></param>
        /// <returns></returns>
        public static double Distance(MazeSquare sq1, MazeSquare sq2)
        {
            double dx = sq1.XPos - sq2.XPos;
            double dy = sq1.YPos - sq2.YPos;
            return Math.Sqrt((dx * dx) + (dy * dy));
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
            data.IrregularMaze = this.Irregular;
            data.Irregularity = this.Irregularity;
        }

        /// <summary>
        /// Take all modifyable parameters from the given data object.
        /// </summary>
        /// <param name="data"></param>
        public void TakeParametersFrom(AriadneSettingsData data)
        {
            // The Auto... flags for Width and Height have already been checked by the MazeUserControl.
            this.xSize = Math.Max(dimensionsObj.MinSize, Math.Min(dimensionsObj.MaxXSize, data.MazeWidth));
            this.ySize = Math.Max(dimensionsObj.MinSize, Math.Min(dimensionsObj.MaxYSize, data.MazeHeight));

            if (!data.AutoSeed)
            {
                this.seed = Math.Max(0, Math.Min(codeObj.SeedLimit - 1, data.Seed));
            }
            else
            {
                Random r = RandomFactory.CreateRandom();
                this.seed = r.Next(codeObj.SeedLimit);
            }
            this.random = RandomFactory.CreateRandom(seed);

            this.reservedAreas.Clear();
            this.outlineShape = null;
            this.embeddedMazeShapes.Clear();
            this.embeddedMazes.Clear();

            this.Irregular = data.IrregularMaze;
            this.Irregularity = data.Irregularity;

            // Decode(data.Code);
        }

        #endregion
    }
}
