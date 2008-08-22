using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SWA.Ariadne.Outlines
{
    /// <summary>
    /// An OutlineShape created by repeating a certain geometric element in a grid pattern.
    /// </summary>
    internal class GridOutlineShape : OutlineShape
    {
        #region Member variables and Properties

        private ExplicitOutlineShape baseShape;

        public override bool this[int x, int y]
        {
            get { return baseShape[x, y]; }
        }

        #endregion

        #region Constructor

        private GridOutlineShape(int xSize, int ySize, GridElement tile)
            : base(xSize, ySize)
        {
            this.baseShape = new ExplicitOutlineShape(xSize, ySize);
            this.Apply(tile);
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Applies the given tile centered on the baseShape.
        /// </summary>
        /// <param name="tile"></param>
        private void Apply(GridElement tile)
        {
            int xRight = (XSize - tile.width) / 2;
            int yTop = (YSize - tile.height) / 2;
            Apply(tile, xRight, yTop);
        }

        /// <summary>
        /// Applies the given tile on the baseShape at the given location.
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="xRight"></param>
        /// <param name="yTop"></param>
        private void Apply(GridElement tile, int xRight, int yTop)
        {
            tile.Apply(baseShape, xRight, yTop);
        }

        /// <summary>
        /// Applies the given tile in a grid on the baseShape.
        /// </summary>
        /// <param name="gridTile"></param>
        private void ApplyCheckered(TileGridElement gridTile)
        {
            CheckeredGridElement shapeTile = new CheckeredGridElement(gridTile, false);
            this.Apply(shapeTile);
        }

        #endregion

        #region Static methods for creating OutlineShapes

        /// <summary>
        /// Returns one of several grid shapes.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="xSize"></param>
        /// <param name="ySize"></param>
        /// <returns></returns>
        public static OutlineShape CreateGridInstance(Random r, int xSize, int ySize)
        {
            GridOutlineShape result;
            TileGridElement gridTile;
            bool invertEveryOtherTile;
            List<TileGridElement> overlayTiles = new List<TileGridElement>();

            gridTile = ChooseGridElement(r, overlayTiles, out invertEveryOtherTile);

            result = CreateCheckeredInstance(xSize, ySize, gridTile, invertEveryOtherTile);
            foreach (TileGridElement overlayTile in overlayTiles)
            {
                result.ApplyCheckered(overlayTile);
            }

            return result;
        }

        /// <summary>
        /// Returns one of the defined TileGridElement patterns.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="overlayTiles"></param>
        /// <param name="invertEveryOtherTile"></param>
        /// <returns></returns>
        private static TileGridElement ChooseGridElement(Random r, List<TileGridElement> overlayTiles, out bool invertEveryOtherTile)
        {
            TileGridElement result;

            int width, height;
            double diameter;

            int choice = r.Next(15);
            invertEveryOtherTile = (choice % 2 == 0);

            switch (choice)
            {
                default:
                case 0: // simple checkered squares
                    width = r.Next(3, 12 + 1);

                    result = new BlackGridElement(width, width);
                    invertEveryOtherTile = true;
                    break;

                case 1: // large circles, slightly smaller than the checkered squares
                case 2:
                    width = r.Next(12, 24 + 1);
                    diameter = width - 4;

                    result = new CircleGridElement(width, width, diameter);
                    break;

                case 3: // large and small circles
                case 4:
                    width = r.Next(12, 24 + 1);
                    diameter = width - 2;

                    result = new CircleGridElement(width, width, diameter);
                    overlayTiles.Add(new CircleGridElement(width, width, 0.5 * diameter));
                    break;

                case 4 + 1: // halfed or quartered circles
                case 4 + 2:
                case 4 + 3:
                    width = height = r.Next(12, 24 + 1);
                    diameter = width - 1;
                    bool onVerticalEdge = ((choice & 1) != 0);
                    bool onHorizontalEdge = ((choice & 2) != 0);
                    if (onVerticalEdge && !onHorizontalEdge)
                    {
                        width += 2;
                        diameter -= 1;
                    }
                    else if (onHorizontalEdge && !onVerticalEdge)
                    {
                        height += 2;
                        diameter -= 1;
                    }

                    result = new CircleGridElement(width, height, diameter, onVerticalEdge, onHorizontalEdge);
                    invertEveryOtherTile = true;
                    break;

                case 8: // large overlapping circles almost touching each other diagonally
                    width = r.Next(20, 40 + 1);
                    // The circle should pass exactly between the squares (3,2) and (4,2) in the adjoining tile.
                    double x = 0.5 * (width - 1) - 3.0;
                    double y = 0.5 * (width - 1) + 1.5;
                    diameter = 2.0 * Math.Sqrt(x * x + y * y);

                    result = new CircleGridElement(width, width, diameter);
                    invertEveryOtherTile = false;
                    break;

                case 9: // rectangular lines, tightly boxed
                case 10:
                    width = r.Next(8, 16 + 1);
                    height = r.Next(8, 16 + 1);

                    result = new TightBoxesGridElement(width, height);
                    break;

                case 11: // large circles touching each other
                case 12:
                case 13:
                case 14:
                    width = height = r.Next(12, 24 + 1);
                    diameter = width - 0.4;

                    // Make one side approx. 50% longer than the other.
                    // The difference should be an even number.
                    if ((choice / 2) % 2 == 0)
                    {
                        width += 2 * (int)(0.25 * width);
                    }
                    {
                        height += 2 * (int)(0.25 * height);
                    }

                    result = new CircleGridElement(width, height, diameter);
                    break;
            }

            return result;
        }

        /// <summary>
        /// Returns the given tile applied in a grid.
        /// </summary>
        /// <param name="xSize"></param>
        /// <param name="ySize"></param>
        /// <param name="gridTile"></param>
        /// <param name="invertEveryOtherTile"></param>
        /// <returns></returns>
        private static GridOutlineShape CreateCheckeredInstance(int xSize, int ySize, TileGridElement gridTile, bool invertEveryOtherTile)
        {
            CheckeredGridElement shapeTile = new CheckeredGridElement(gridTile, invertEveryOtherTile);
            return new GridOutlineShape(xSize, ySize, shapeTile);
        }

        #endregion
    }

    #region GridElement classes

    #region Base classes

    /// <summary>
    /// A pattern of set and cleared squares that can be applied to an EcplicitOutlineShape.
    /// There are simple and repeated subclasses.
    /// </summary>
    internal abstract class GridElement
    {
        public readonly int width, height;

        public GridElement(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// The current GridElement is applied to the given target.
        /// Every set square is inverted.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="xRight"></param>
        /// <param name="yTop"></param>
        public abstract void Apply(ExplicitOutlineShape target, int xRight, int yTop);

        /// <summary>
        /// Returns true when the given square is set.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected abstract bool this[int x, int y] { get; }

        /// <summary>
        /// Inverts the target square at the given coordinates.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        protected void Invert(ExplicitOutlineShape target, int x, int y)
        {
            if (0 <= x && x < target.XSize && 0 <= y && y < target.YSize)
            {
                target.SetValue(x, y, !target[x, y]);
            }
        }
    }

    /// <summary>
    /// A simple GridElement that is applied only once in its tile.
    /// A repeating pattern may be formed by applying the same TileGridElement in many tiles.
    /// Note: The BoundingBox of a tile pattern may extend over the tile boundary.
    /// </summary>
    internal abstract class TileGridElement : GridElement
    {
        /// <summary>
        /// The area where the pattern is actually applied.
        /// This area is not necessarily confined to the tile area.
        /// </summary>
        public virtual Rectangle BoundingBox
        {
            get { return new Rectangle(0, 0, width, height); }
        }

        public TileGridElement(int width, int height)
            : base(width, height)
        {
        }

        /// <summary>
        /// The current GridElement is applied to the given target.
        /// Every set square is inverted.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="xRight"></param>
        /// <param name="yTop"></param>
        public override void Apply(ExplicitOutlineShape target, int xRight, int yTop)
        {
            Rectangle bbox = this.BoundingBox;
            for (int x = bbox.Left; x < bbox.Right; x++)
            {
                for (int y = bbox.Top; y < bbox.Bottom; y++)
                {
                    if (this[x, y] == true)
                    {
                        this.Invert(target, x + xRight, y + yTop);
                    }
                }
            }
        }
    }

    /// <summary>
    /// A GridElement that is applied repeatedly over the whole target shape.
    /// </summary>
    internal class CheckeredGridElement : GridElement
    {
        private readonly TileGridElement tile;
        private readonly bool invertEveryOtherTile;
        private readonly WhiteGridElement white;

        public CheckeredGridElement(TileGridElement tile, bool invertEveryOtherTile)
            : base(tile.width, tile.height)
        {
            this.tile = tile;
            this.invertEveryOtherTile = invertEveryOtherTile;
            this.white = new WhiteGridElement(this.width, this.height);
        }

        /// <summary>
        /// Apply the tile element in a repeating grid on the whole target shape.
        /// One tile (which will not be inverted) will be placed at the given location.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="xRight"></param>
        /// <param name="yTop"></param>
        public override void Apply(ExplicitOutlineShape target, int xRight, int yTop)
        {
            Apply(target, tile, xRight, yTop, true);

            if (invertEveryOtherTile)
            {
                Apply(target, white, xRight, yTop, false);
            }
        }

        private static void Apply(ExplicitOutlineShape target, TileGridElement tile, int xRight, int yTop, bool allLocations)
        {
            // Determine location of the center tile.
            int x = xRight;
            int y = yTop;

            // Determine bounding box of the center tile.
            Rectangle bbox = tile.BoundingBox;
            bbox.X += x;
            bbox.Y += y;

            // Move tile location to the left/top so that the bounding box is just inside of the shape.
            int i = bbox.Right / tile.width;
            int j = bbox.Bottom / tile.height;
            int k = (i + j) % 2;
            x -= i * tile.width;
            y -= j * tile.height;
            bbox.X -= i * tile.width;
            bbox.Y -= j * tile.height;

            // Apply the tile in all locations where the bounding box is still inside of the shape.
            for (i = 0; bbox.Left + i * tile.width < target.XSize; i++)
            {
                for (j = 0; bbox.Top + j * tile.height < target.YSize; j++)
                {
                    if (allLocations || (i + j + k) % 2 == 1)
                    {
                        tile.Apply(target, x + i * tile.width, y + j * tile.height);
                    }
                }
            }
        }

        protected override bool this[int x, int y]
        {
            get { throw new Exception("The method is not implemented."); }
        }
    }

    #endregion

    #region Trivial TileGridElements

    /// <summary>
    /// A GridElement that is always cleared.
    /// </summary>
    internal class BlackGridElement : TileGridElement
    {
        public BlackGridElement(int width, int height)
            : base(width, height)
        {
        }

        protected override bool this[int x, int y] { get { return false; } }
    }

    /// <summary>
    /// A GridElement that is always set.
    /// </summary>
    internal class WhiteGridElement : TileGridElement
    {
        public WhiteGridElement(int width, int height)
            : base(width, height)
        {
        }

        protected override bool this[int x, int y] { get { return true; } }
    }

    #endregion

    #region Non-trivial TileGridElements

    /// <summary>
    /// Defines the area of a circle.
    /// The circle may be centered in the tile, on one of the edges or on the corner.
    /// </summary>
    internal class CircleGridElement : TileGridElement
    {
        private double xc, yc, r;

        /// <summary>
        /// Constructs a circle centered in the tile.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="diameter"></param>
        public CircleGridElement(int width, int height, double diameter)
            : this(width, height, diameter, false, false)
        {
        }

        /// <summary>
        /// Constructs a circle at the given center location.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="diameter"></param>
        /// <param name="onVerticalEdge"></param>
        /// <param name="onHorizontalEdge"></param>
        public CircleGridElement(int width, int height, double diameter, bool onVerticalEdge, bool onHorizontalEdge)
            : base(width, height)
        {
            // The relevant coordinates are 0..w-1 and 0..h-1
            this.xc = (onVerticalEdge ? 0.0 : 0.5 * (width - 1));
            this.yc = (onHorizontalEdge ? 0.0 : 0.5 * (height - 1));

            this.r = 0.5 * diameter;

            double kx = (xc + r) - Math.Floor(xc + r);
            double ky = (yc + r) - Math.Floor(yc + r);
            if (kx < 0.02 || ky < 0.02) // (xc + r) or (yc + r) is very close to an integral number
            {
                // The radius is decreased a bit to get a flat curve instead of a single point.
                r -= 0.02;
            }
        }

        protected override bool this[int x, int y]
        {
            get
            {
                double dx = x - xc;
                double dy = y - yc;
                return (dx * dx + dy * dy <= r * r);
            }
        }

        public override Rectangle BoundingBox
        {
            get
            {
                int x0 = (int)Math.Truncate(xc - r);
                int x1 = (int)Math.Ceiling(xc + r);
                int y0 = (int)Math.Truncate(yc - r);
                int y1 = (int)Math.Ceiling(yc + r);
                return new Rectangle(x0, y0, x1 - x0, y1 - y0);
            }
        }
    }

    /// <summary>
    /// Defines a set of rectangular lines boxed into each other.
    /// </summary>
    internal class TightBoxesGridElement : TileGridElement
    {
        public TightBoxesGridElement(int width, int height)
            : base(width, height)
        {
        }

        protected override bool this[int x, int y]
        {
            get
            {
                // Find the closest distance to any of the tile borders.
                int dl = x - 0;
                int dr = width - 1 - x;
                int dt = y - 0;
                int db = height - 1 - y;
                int dx = Math.Min(dl, dr);
                int dy = Math.Min(dt, db);
                int d = Math.Min(dx, dy);

                // Return alternatingly true and false.
                return (d % 2 == 0);
            }
        }
    }

    #endregion

    #endregion
}
