using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SWA.Ariadne.Outlines
{
    /// <summary>
    /// An OutlineShape with a pattern based on repeating rectangular tiles.
    /// One tile will be centered in the size of the OutlineShape; identical tiles will extend in all directions.
    /// </summary>
    internal class TilesOutlineShape : OutlineShape
    {
        #region Class variables

        /// <summary>
        /// List of Property Methods that return bitmap images from the Resources file.
        /// </summary>
        private static List<System.Reflection.MethodInfo> BitmapProperties = SWA.Utilities.Resources.BitmapProperties(typeof(Resources.Tiles));

        #endregion

        #region Member variables and Properties

        /// <summary>
        /// The pattern is stored in an ExplicitOutlineShape.
        /// </summary>
        private ExplicitOutlineShape tile;

        /// <summary>
        /// Each column in the tile can be repeated more than once.
        /// Dimensions: tile.XSize, tile.YSize.
        /// </summary>
        private int[] xRepetitions, yRepetitions;

        /// <summary>
        /// The effective tile size, resulting from the repetitions.
        /// </summary>
        private int xTileSize, yTileSize;

        /// <summary>
        /// Mapping of shape coordinates to tile coordinates.
        /// Dimensions: xTileSize, yTileSize.
        /// </summary>
        private int[] xMap, yMap;

        public override bool this[int x, int y]
        {
            get
            {
                return tile[xMap[x % xTileSize], yMap[y % yTileSize]];
            }
        }
        public void SetValue(int x, int y, bool value)
        {
            tile.SetValue(x, y, value);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="xSize">The overall shape size.</param>
        /// <param name="ySize">The overall shape size.</param>
        /// <param name="xTileSize">The pattern tile size.</param>
        /// <param name="yTileSize">The pattern tile size.</param>
        private TilesOutlineShape(int xSize, int ySize, int xTileSize, int yTileSize)
            : this(xSize, ySize, new ExplicitOutlineShape(xTileSize, yTileSize))
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="xSize">The overall shape size.</param>
        /// <param name="ySize">The overall shape size.</param>
        /// <param name="template">A black and white bitmap pattern.</param>
        private TilesOutlineShape(int xSize, int ySize, Bitmap template)
            : this(xSize, ySize, new ExplicitOutlineShape(template))
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="xSize">The overall shape size.</param>
        /// <param name="ySize">The overall shape size.</param>
        /// <param name="tile">A shape that will be used as the repeating tile pattern.</param>
        private TilesOutlineShape(int xSize, int ySize, ExplicitOutlineShape tile)
            : base(xSize, ySize)
        {
            this.tile = tile;
            this.xRepetitions = new int[tile.XSize];
            this.yRepetitions = new int[tile.YSize];

            for (int i = 0; i < xRepetitions.Length; i++)
            {
                xRepetitions[i] = 1;
            }
            for (int i = 0; i < yRepetitions.Length; i++)
            {
                yRepetitions[i] = 1;
            }

            UpdateTileSize();
        }

        #endregion

        #region Static methods for creating OutlineShapes

        /// <summary>
        /// Returns one of several tiled shapes.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="xSize"></param>
        /// <param name="ySize"></param>
        /// <returns></returns>
        public static OutlineShape RandomInstance(Random r, int xSize, int ySize)
        {
#if false
            return FromBitmap(r, xSize, ySize);
            return Ribbons(r, xSize, ySize);
#endif

            // TODO: more patterns
            switch (r.Next(12))
            {
                default:
                case 0: case 1:
                    return StripesOrGrid(r, xSize, ySize);
                case 2: case 3: case 4:
                    return Ribbons(r, xSize, ySize);
                case 5: case 6:
                    return Pentominoes(r, xSize, ySize);
                case 7: case 8: case 9: case 10: case 11:
                    return FromBitmap(r, xSize, ySize);
            }
        }

        /// <summary>
        /// Builds a pattern of thin lines and broad stripes (horizontal or vertical).
        /// When the lines go in botth directions, they form a square or rectangular grid.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="xSize"></param>
        /// <param name="ySize"></param>
        /// <returns></returns>
        private static OutlineShape StripesOrGrid(Random r, int xSize, int ySize)
        {
            TilesOutlineShape result = new TilesOutlineShape(xSize, ySize, 2, 2);

            int k = r.Next(1, 4); // 1, 2, 3

            result.SetValue(0, 0, (k != 3 || r.Next(2) == 0));  // maybe a disconnected grid
            result.SetValue(0, 1, ((k & 1) != 0));              // horizontal lines
            result.SetValue(1, 0, ((k & 2) != 0));              // vertical lines
            result.SetValue(1, 1, false);

            result.SetRepetitions(0, 1);                        // one-square pinstripes
            if (r.Next(0) == 0)
            {
                result.SetRepetitions(1, r.Next(4, 16));        // same x and y width
            }
            else
            {
                while (Math.Abs(result.xRepetitions[1] - result.yRepetitions[1]) < 5)
                {
                    result.SetXRepetitions(1, r.Next(4, 21));   // different x and y width
                    result.SetYRepetitions(1, r.Next(4, 21));
                }
            }

            if (result.tile[0, 0] == false && result.xRepetitions[1] > 7 && result.yRepetitions[1] > 7)
            {
                result.SetRepetitions(0, r.Next(1, 3));         // thin stripes: one or two squares
            }

            return result;
        }

        /// <summary>
        /// Builds a pattern of interwoven ribbons.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="xSize"></param>
        /// <param name="ySize"></param>
        /// <returns></returns>
        private static OutlineShape Ribbons(Random r, int xSize, int ySize)
        {
            switch (r.Next(3))
            {
                default:
                case 0:
                    return Ribbons8(r, xSize, ySize);
                case 1:
                    return Ribbons6(r, xSize, ySize, true);
                case 2:
                    return Ribbons6(r, xSize, ySize, false);
            }
        }

        /// <summary>
        /// Builds a pattern of interwoven ribbons, effectually forming rectangular frames.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="xSize"></param>
        /// <param name="ySize"></param>
        /// <returns></returns>
        private static OutlineShape Ribbons8(Random r, int xSize, int ySize)
        {
            TilesOutlineShape result = new TilesOutlineShape(xSize, ySize, Resources.Resources.Ribbons8);

            #region Optionally, erase some lines completely; the shape will no longer be totally connected.

            if (r.Next(2) == 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    result.SetValue(1, i, false);
                    result.SetValue(5, i, false);
                }
            }
            if (r.Next(2) == 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    result.SetValue(i, 1, false);
                    result.SetValue(i, 5, false);
                }
            }

            // with one set erased: rectangular frames
            // with two sets erased: disconnected bars

            #endregion

            #region Extend the shape width.

            int r3 = r.Next(6, 21);                 // the broad ribbon width
            int r2 = 1 + r.Next(2);                 // the ribbon border
            int r1 = r3 / 6 + (r.Next(3) - 1);      // the distance between parallel ribbons

            for (int i = 2; i < 8; i += 4)
            {
                result.SetRepetitions(i - 1, r1);
                result.SetRepetitions(i + 1, r3);
            }

            for (int i = 0; i < 8; i += 2)
            {
                result.SetRepetitions(i, r2);
            }

            #endregion

            return result;
        }

        /// <summary>
        /// Builds a pattern of interwoven ribbons, effectually forming I-beam shapes.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="xSize"></param>
        /// <param name="ySize"></param>
        /// <param name="broadRibbon">When true/false, the middle beam is broader/slimmer than the end beams.</param>
        /// <returns></returns>
        private static OutlineShape Ribbons6(Random r, int xSize, int ySize, bool broadRibbon)
        {
            TilesOutlineShape result = new TilesOutlineShape(xSize, ySize, Resources.Resources.Ribbons6);

            #region Extend the shape width.

            int r1, r2;

            if (broadRibbon == true)
            {
                r1 = r.Next(6, 21);                                 // the broad ribbon width: 6..20
                r2 = Math.Max(1, Math.Min(3, r.Next(1 + r1 / 3)));  // the ribbon border: 1..3
            }
            else
            {
                r1 = r.Next(1, 5);                                  // the slim ribbon width / middle beam
                r2 = r.Next(6, 11);                                 // the ribbon border / end beam
            }

            result.SetRepetitions(r2);
            result.SetRepetitions(1, r1);
            result.SetRepetitions(4, r1);

            #endregion

            return result;
        }

        /// <summary>
        /// Builds a pattern made of pentominoes: five adjoining squares in different arrangements.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="xSize"></param>
        /// <param name="ySize"></param>
        /// <returns></returns>
        private static OutlineShape Pentominoes(Random r, int xSize, int ySize)
        {
            int squareWidth = r.Next(2, 7);
            int xDim = 3 + xSize / (1 + squareWidth), yDim = 3 + xSize / (1 + squareWidth);

            TilesOutlineShape result = new TilesOutlineShape(xSize, ySize, 1 + 2 * xDim, 1 + 2 * yDim);

            PentominoPattern pentominoPattern = new PentominoPattern(r, xDim, yDim);

            #region Start with filled squares in all but the (odd,odd) positions.

            for (int x = 0; x < result.xTileSize; x++)
            {
                for (int y = 0; y < result.yTileSize; y++)
                {
                    result.SetValue(x, y, (x % 2 == 0 || y % 2 == 0));
                }
            }

            // result: a dense grid
            /*
             *   x x x x x x x x
             *   x o x o x o x o
             *   x x x x x x x x
             *   x o x o x o x o
             *   x x x x x x x x
             *   x o x o x o x o
             *   x x x x x x x x
             *   x o x o x o x o
             */

            for (int x = 1; x < xDim; x++)
            {
                result.SetXRepetitions(1 + 2 * x, squareWidth);
            }
            for (int y = 1; y < yDim; y++)
            {
                result.SetYRepetitions(1 + 2 * y, squareWidth);
            }

            #endregion

            #region Open the positions between pentomino squares of the same id.

            for (int x = 0; x < xDim; x++)
            {
                for (int y = 0; y < yDim; y++)
                {
                    if (x + 1 < xDim && pentominoPattern[x, y] == pentominoPattern[x + 1, y])
                    {
                        result.SetValue(2 + 2 * x, 1 + 2 * y, false);
                    }
                    if (y + 1 < yDim && pentominoPattern[x, y] == pentominoPattern[x, y + 1])
                    {
                        result.SetValue(1 + 2 * x, 2 + 2 * y, false);
                    }
                }
            }

            #endregion

            return result;
        }

        /// <summary>
        /// Builds a pattern from a bitmap resource.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="xSize"></param>
        /// <param name="ySize"></param>
        /// <returns></returns>
        private static OutlineShape FromBitmap(Random r, int xSize, int ySize)
        {
            // Load a random bitmap image.
            Bitmap bitmap = SWA.Utilities.Resources.CreateBitmap(BitmapProperties, r);

            #region Choose a rotate/flip operation.

            RotateFlipType rft;
            switch (r.Next(3))
            {
                default:
                case 0:
                    rft = RotateFlipType.RotateNoneFlipNone;
                    break;
                case 1:
                    rft = RotateFlipType.Rotate90FlipNone;
                    break;
                case 2:
                    rft = RotateFlipType.RotateNoneFlipX;
                    break;
            }

            #endregion

            #region Choose a scale factor.

            int area = FromBitmap(3 * bitmap.Width, 3 * bitmap.Height, bitmap, RotateFlipType.RotateNoneFlipNone, 1).ConnectedSubset(null).Area;
            int scale = (int)Math.Round(Math.Sqrt((24 * 24) / (area * 2)));
#if false
            System.Console.Out.WriteLine("[TilesOutlineShape.FromBitmap] area = " + area.ToString() + ", scale = " + scale.ToString());
#endif
            scale = r.Next(1, Math.Max(1, scale) + 1);

            #endregion

            return FromBitmap(xSize, ySize, bitmap, rft, scale);
        }

        /// <summary>
        /// Builds a pattern from the given bitmap, rotated and scaled.
        /// </summary>
        /// <param name="xSize"></param>
        /// <param name="ySize"></param>
        /// <param name="bitmap"></param>
        /// <param name="rft"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        private static OutlineShape FromBitmap(int xSize, int ySize, Bitmap bitmap, RotateFlipType rft, int scale)
        {
#if false
            System.Console.Out.WriteLine("[TilesOutlineShape.FromBitmap] " + rft.ToString() + ", x" + scale.ToString());
#endif

#if false
            // Note: Some bitmaps are useless (all black or all white) after the RotateFlip() operation!
            Bitmap template = (Bitmap)bitmap.Clone();
            template.RotateFlip(rft);
            TilesOutlineShape result = new TilesOutlineShape(xSize, ySize, template);
#else
            OutlineShape tile = new ExplicitOutlineShape(bitmap).RotatedOrFlipped(rft);
            TilesOutlineShape result = new TilesOutlineShape(xSize, ySize, tile as ExplicitOutlineShape);
#endif
            result.SetRepetitions(scale);

            return result;
        }

        #endregion

        #region Auxiliary methods

        #region Methods for setting the repetition numbers

        /// <summary>
        /// Set the repetition number of the given tile position (x coordinate).
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="num"></param>
        private void SetXRepetitions(int pos, int num)
        {
            xRepetitions[pos] = Math.Max(0, num);
            UpdateTileSize();
        }

        /// <summary>
        /// Set the repetition number of the given tile position (y coordinate).
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="num"></param>
        private void SetYRepetitions(int pos, int num)
        {
            yRepetitions[pos] = Math.Max(0, num);
            UpdateTileSize();
        }

        /// <summary>
        /// Set both repetition numbers of the given tile position (x and y coordinate).
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="num"></param>
        private void SetRepetitions(int pos, int num)
        {
            xRepetitions[pos] = Math.Max(0, num);
            yRepetitions[pos] = Math.Max(0, num);
            UpdateTileSize();
        }

        /// <summary>
        /// Set the repetition number of all positions (x coordinate).
        /// </summary>
        /// <param name="num"></param>
        private void SetXRepetitions(int num)
        {
            for (int i = 0; i < xRepetitions.Length; i++)
            {
                xRepetitions[i] = num;
            }
            UpdateTileSize();
        }

        /// <summary>
        /// Set the repetition number of all positions (y coordinate).
        /// </summary>
        /// <param name="num"></param>
        private void SetYRepetitions(int num)
        {
            for (int i = 0; i < yRepetitions.Length; i++)
            {
                yRepetitions[i] = num;
            }
            UpdateTileSize();
        }

        /// <summary>
        /// Set the repetition number of all positions (x and y coordinates).
        /// </summary>
        /// <param name="num"></param>
        private void SetRepetitions(int num)
        {
            for (int i = 0; i < xRepetitions.Length; i++)
            {
                xRepetitions[i] = num;
            }
            for (int i = 0; i < yRepetitions.Length; i++)
            {
                yRepetitions[i] = num;
            }
            UpdateTileSize();
        }

        #endregion

        /// <summary>
        /// Calculates the xTileSize and yTileSize values and updates the xMap and yMap coordinate mappings.
        /// Is called whenever the repetition numbers are changed.
        /// </summary>
        private void UpdateTileSize()
        {
            xTileSize = yTileSize = 0;

            for (int i = 0; i < xRepetitions.Length; i++)
            {
                xTileSize += xRepetitions[i];
            }
            for (int i = 0; i < yRepetitions.Length; i++)
            {
                yTileSize += yRepetitions[i];
            }

            UpdateCoordinateMapping();
        }

        /// <summary>
        /// Updates the xMap and yMap coordinate mappings.
        /// Is called whenever the repetition numbers are changed.
        /// </summary>
        private void UpdateCoordinateMapping()
        {
            xMap = new int[xTileSize];
            yMap = new int[yTileSize];

            // Determine the origin of a tile centered in the drawing area.
            int x0 = (XSize - xTileSize) / 2, y0 = (YSize - yTileSize) / 2;

            // Make sure x0 and y0 are positive.
            if (x0 < 0)
            {
                x0 += xTileSize;
            }
            if (y0 < 0)
            {
                y0 += yTileSize;
            }

            #region Derive the xMap and yMap of a (wrapped) tile in the top left corner of the shape.

            for (int xt = 0, xm = 0; xt < tile.XSize; xt++)
            {
                for (int i = 0; i < xRepetitions[xt]; i++, xm++)
                {
                    xMap[(xm + x0) % xTileSize] = xt;
                }
            }
            for (int yt = 0, ym = 0; yt < tile.YSize; yt++)
            {
                for (int i = 0; i < yRepetitions[yt]; i++, ym++)
                {
                    yMap[(ym + y0) % yTileSize] = yt;
                }
            }

            #endregion
        }

        #endregion

        #region Auxiliary classes

        internal class PentominoPattern
        {
            private int[,] squares;

            public int this[int x, int y]
            {
                get
                {
                    if (x < 0 || y < 0 || x >= squares.GetLength(0) || y >= squares.GetLength(1))
                    {
                        return 0;
                    }
                    return squares[x, y]; 
                }
            }

            #region Constructor

            private PentominoPattern(int xDim, int yDim)
            {
                squares = new int[xDim, yDim];
            }

            public PentominoPattern(Random r, int xDim, int yDim)
                : this(xDim, yDim)
            {
                int five = 5;
                int nextId = 1;

                // Four lists of square positions.
                // Each list contains the squares with the same number of unmarked neighboring squares.
                List<System.Drawing.Point>[] candidates = new List<System.Drawing.Point>[4];
                for (int i = 0; i < 4; i++)
                {
                    candidates[i] = new List<System.Drawing.Point>(xDim * yDim / 4);
                }

                // For every id, the number of squares with that id.
                int[] idCount = new int[(xDim+2) * (yDim+2) / (five - 1)];

                // Start with the center square.
                SetId(xDim / 2, yDim / 2, nextId++, candidates, idCount);

                while (true)
                {
                    // Choose a candidate with minimum number of unmarked neighbors.
                    System.Drawing.Point pt = ChooseCandidate(r, candidates);
                    if (pt.X < 0)
                    {
                        break;
                    }

                    // Choose an id for the candidate.
                    int id = ChooseId(r, pt, idCount, five, ref nextId);

                    // Stop if the expected number of pentominoes is exceeded.
                    if (id >= idCount.Length)
                    {
                        break;
                    }

                    SetId(pt.X, pt.Y, id, candidates, idCount);
                }
            }

            #endregion

            #region Methods

            /// <summary>
            /// The square at (x,y) gets the given id.
            /// All neighbor squares are added to the candidates lists.
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="id"></param>
            /// <param name="candidates"></param>
            private void SetId(int x, int y, int id, List<System.Drawing.Point>[] candidates, int[] idCount)
            {
                // Remove the point from its current candidate list.
                bool found = candidates[-squares[x, y]].Remove(new System.Drawing.Point(x, y));

                // Mark the point with the given id.
                squares[x, y] = id;
                ++idCount[id];

                // Add the point's neighbors as new candidates.
                MaybeAddCandidate(x - 1, y, candidates);
                MaybeAddCandidate(x + 1, y, candidates);
                MaybeAddCandidate(x, y - 1, candidates);
                MaybeAddCandidate(x, y + 1, candidates);
            }

            /// <summary>
            /// Add the Point (x,y) to the candidates lists.
            /// If it is already a candidate, the point is moved to another list, as it has lost an unmarked neighbor.
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="candidates"></param>
            private void MaybeAddCandidate(int x, int y, List<System.Drawing.Point>[] candidates)
            {
                // Skip points outside the squares area.
                if (x < 0 || y < 0 || x >= squares.GetLength(0) || y >= squares.GetLength(1))
                {
                    return;
                }

                if (squares[x, y] > 0)
                {
                    // Skip squares that are already marked.
                    return;
                }
                else if (squares[x, y] < 0)
                {
                    // This square has one unmarked neighbor less.
                    bool found = candidates[-squares[x, y]].Remove(new System.Drawing.Point(x, y));
                    ++squares[x, y];
                    candidates[-squares[x, y]].Add(new System.Drawing.Point(x, y));
                }
                else
                {
                    // This square is a new candidate.
                    squares[x, y] = -3;                                 // usually 3 unmarked neighbors
                    if (x == 0 || x + 1 == squares.GetLength(0))
                    {
                        squares[x, y] += 1;                             // no east/west neighbor
                    }
                    if (y == 0 || y + 1 == squares.GetLength(1))        // no north/south neighbor
                    {
                        squares[x, y] += 1;
                    }
                    candidates[-squares[x, y]].Add(new System.Drawing.Point(x, y));
                }
            }

            /// <summary>
            /// Returns a random item from one of the lists.
            /// Prefers points with zero or one unmarked neighbor only.
            /// </summary>
            /// <param name="r"></param>
            /// <param name="candidates"></param>
            /// <returns></returns>
            private System.Drawing.Point ChooseCandidate(Random r, List<System.Drawing.Point>[] candidates)
            {
                int ci = -1;

                if (ci < 0 && candidates[0].Count > 0)
                {
                    // Choose a candidate with no unmarked neighbors.
                    // This should never happen, as candidates with one neighbor are already handled first.
                    ci = 0;
                }

                if (ci < 0 && candidates[1].Count > 0)
                {
                    // Choose a candidate with one unmarked neighbor.
                    // This should happen before the last neighbor is chosen and marked.
                    ci = 1;
                }

                if (ci < 0)
                {
                    // Choose any other candidate.
                    int n = candidates[2].Count + candidates[3].Count;
                    if (n > 0)
                    {
                        int k = r.Next(n);
                        if (k < candidates[2].Count)
                        {
                            ci = 2;
                        }
                        else
                        {
                            ci = 3;
                        }
                    }
                }

                if (ci < 0)
                {
                    // All candidate lists are empty.
                    return new System.Drawing.Point(-1, -1);
                }

                return candidates[ci][r.Next(candidates[ci].Count)];
            }

            /// <summary>
            /// Returns the id of one of the squares next to pt.
            /// From several choices, the one with least idCount is picked.
            /// If the idCount is already five, the nextId is returned and incremented.
            /// </summary>
            /// <param name="r"></param>
            /// <param name="pt"></param>
            /// <param name="idCount"></param>
            /// <param name="five"></param>
            /// <param name="nextId"></param>
            /// <returns></returns>
            private int ChooseId(Random r, System.Drawing.Point pt, int[] idCount, int five, ref int nextId)
            {
                List<int> list = new List<int>(4);

                for (int dx = -1; dx <= +1; dx++)
                {
                    for (int dy = -1; dy <= +1; dy++)
                    {
                        if ((dx != 0) != (dy != 0))
                        {
                            int id = this[pt.X + dx, pt.Y + dy];
                            if (id <= 0)
                            {
                                continue;
                            }
                            else if (list.Count == 0)
                            {
                                list.Add(id);
                            }
                            else
	                        {
                                if (idCount[id] < idCount[list[0]])
                                {
                                    list.Clear();
                                }
                                else if (idCount[id] > idCount[list[0]])
                                {
                                    // skip this id
                                    continue;
                                }
                                list.Add(id);
                            }
                        }
                    }
                }

                if (list.Count > 0 && idCount[list[0]] < five)
                {
                    return list[r.Next(list.Count)];
                }
                else
                {
                    return nextId++;
                }
            }

            #endregion
        }

        #endregion
    }
}
