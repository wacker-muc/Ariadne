using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SWA.Ariadne.Outlines
{
    /// <summary>
    /// An OutlineShape defines a contour line dividing the shape's inside and outside.
    /// When used by the maze builder, the walls on the contour line should all be closed
    /// (with the exception of a single entry).
    /// </summary>
    public abstract class OutlineShape
    {
        #region Member variables and Properties

        /// <summary>
        /// Nominal size of the shape.
        /// </summary>
        private int xSize, ySize;

        internal int XSize
        {
            get { return this.xSize; }
        }

        internal int YSize
        {
            get { return this.ySize; }
        }

        /// <summary>
        /// Returns true if the given point is inside the shape.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public abstract bool this[int x, int y] { get; }

        #endregion

        #region Constructor

        protected OutlineShape(int xSize, int ySize)
        {
            this.xSize = xSize;
            this.ySize = ySize;
        }

        #endregion

        #region Delegates

        /// <summary>
        /// A delegate type that implements the OutlineShape behavior.
        /// Returns true if the square at (x, y) is inside of the shape, false otherwise.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public delegate bool InsideShapeDelegate(int x, int y);

        #endregion

        #region Static methods for creating OutlineShapes

        public delegate OutlineShape OutlineShapeBuilder(Random r, int xSize, int ySize, double centerX, double centerY, double radius);

        /// <summary>
        /// Returns a random OutlineShapeBuilder.
        /// The choice is based on few characteristics of each known OutlineShapeBuilder.
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public static OutlineShapeBuilder RandomOutlineShapeBuilder(Random r)
        {
            OutlineShapeBuilder[] shapeBuilderDelegates = {
                OutlineShape.Circle,
                OutlineShape.Diamond,
                OutlineShape.Polygon,
                OutlineShape.Function,
                OutlineShape.Character,
                OutlineShape.Symbol,
                OutlineShape.Bitmap,
                OutlineShape.Tiles,
                OutlineShape.Rectangles,
                OutlineShape.Grid,
                OutlineShape.GridElement,
            };
            int[] ratios = { // (number of items) * (novelty value) / (easyness of recognition)
                     7 * 20 / 3,
                     1 *  5 / 4,
              (2 * (10 + 8 + 6 + 4 + 2)) * 7 / 3,
                    (3 * 8 + 2) * 12 / 2,
                    15 * 10 / 2,
                     8 * 15 / 2,
                    25 * 15 / 1,
                     8 * 12 / 3,
                     3 *  6 / 3,
                     7 * 10 / 2,
                     7 *  8 / 2,
                };
            
            int n = 0;
            foreach (int k in ratios) { n += k; }
            int p = r.Next(n);
            //p = n - 1;
            
            OutlineShapeBuilder result = null;
            for (int i = 0; i < ratios.Length; i++)
            {
                if ((p -= ratios[i]) < 0)
                {
                    result = shapeBuilderDelegates[i];
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Creates a shape using a random OutlineShapeBuilder.
        /// If applicable, the shape may be distorted.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="xSize"></param>
        /// <param name="ySize"></param>
        /// <param name="offCenter"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static OutlineShape RandomInstance(Random r, int xSize, int ySize, double offCenter, double size)
        {
            OutlineShapeBuilder outlineShapeBuilder = RandomOutlineShapeBuilder(r);

            OutlineShape result = RandomInstance(r, outlineShapeBuilder, xSize, ySize, offCenter, size);

            // If applicable, replace the shape with a distorted version.
            if (r.Next(100) < result.DistortedPercentage(33))
            {
                result = result.DistortedCopy(r);
            }

            return result;
        }

        /// <summary>
        /// Creates a shape using the given OutlineShapeBuilder.
        /// This shape will not be distorted.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="outlineShapeBuilder"></param>
        /// <param name="xSize"></param>
        /// <param name="ySize"></param>
        /// <param name="offCenter"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static OutlineShape RandomInstance(Random r, OutlineShapeBuilder outlineShapeBuilder, int xSize, int ySize, double offCenter, double size)
        {
            double centerX = 0.5, centerY = 0.5;

            double dx = r.NextDouble() - 0.5, dy = r.NextDouble() - 0.5;
            centerX += offCenter * dx;
            centerY += offCenter * dy;

            // Reduce size when we are closer to the center than requested.
            double f = 1.0 - offCenter * 2.0 * (0.5 - Math.Max(Math.Abs(dx), Math.Abs(dy)));

            OutlineShape result = outlineShapeBuilder(r, xSize, ySize, centerX, centerY, size * f);

            return result;
        }

        /// <summary>
        /// Create an outline shape.
        /// </summary>
        /// <param name="r">a source of random numbers</param>
        /// <param name="xSize">width of the created shape</param>
        /// <param name="ySize">height of the created shape</param>
        /// <param name="centerX">X coordinate, relative to total width; 0.0 = top, 1.0 = bottom</param>
        /// <param name="centerY">Y coordinate, relative to total height; 0.0 = left, 1.0 = right</param>
        /// <param name="shapeSize">size, relative to distance of center from the border; 1.0 will touch the border </param>
        /// <returns></returns>
        public static OutlineShape Circle(Random r, int xSize, int ySize, double centerX, double centerY, double shapeSize)
        {
            return CircleOutlineShape.Create(r, xSize, ySize, centerX, centerY, shapeSize);
        }

        /// <summary>
        /// Create an outline shape.
        /// </summary>
        /// <param name="r">a source of random numbers</param>
        /// <param name="xSize">width of the created shape</param>
        /// <param name="ySize">height of the created shape</param>
        /// <param name="centerX">X coordinate, relative to total width; 0.0 = top, 1.0 = bottom</param>
        /// <param name="centerY">Y coordinate, relative to total height; 0.0 = left, 1.0 = right</param>
        /// <param name="shapeSize">size, relative to distance of center from the border; 1.0 will touch the border </param>
        /// <returns></returns>
        public static OutlineShape Diamond(Random r, int xSize, int ySize, double centerX, double centerY, double shapeSize)
        {
            return DiamondOutlineShape.Create(r, xSize, ySize, centerX, centerY, shapeSize);
        }

        /// <summary>
        /// Create an outline shape.
        /// </summary>
        /// <param name="r">a source of random numbers</param>
        /// <param name="xSize">width of the created shape</param>
        /// <param name="ySize">height of the created shape</param>
        /// <param name="centerX">X coordinate, relative to total width; 0.0 = top, 1.0 = bottom</param>
        /// <param name="centerY">Y coordinate, relative to total height; 0.0 = left, 1.0 = right</param>
        /// <param name="shapeSize">size, relative to distance of center from the border; 1.0 will touch the border </param>
        /// <returns></returns>
        public static OutlineShape Polygon(Random r, int xSize, int ySize, double centerX, double centerY, double shapeSize)
        {
            return PolygonOutlineShape.Random(r, xSize, ySize, centerX, centerY, shapeSize);
        }

        /// <summary>
        /// Create an outline shape.
        /// </summary>
        /// <param name="r">a source of random numbers</param>
        /// <param name="xSize">width of the created shape</param>
        /// <param name="ySize">height of the created shape</param>
        /// <param name="centerX">X coordinate, relative to total width; 0.0 = top, 1.0 = bottom</param>
        /// <param name="centerY">Y coordinate, relative to total height; 0.0 = left, 1.0 = right</param>
        /// <param name="shapeSize">size, relative to distance of center from the border; 1.0 will touch the border </param>
        /// <returns></returns>
        public static OutlineShape Function(Random r, int xSize, int ySize, double centerX, double centerY, double shapeSize)
        {
            return FunctionOutlineShape.RandomInstance(r, xSize, ySize, centerX, centerY, shapeSize);
        }

        /// <summary>
        /// Create an outline shape.
        /// </summary>
        /// <param name="r">a source of random numbers</param>
        /// <param name="xSize">width of the created shape</param>
        /// <param name="ySize">height of the created shape</param>
        /// <param name="centerX">X coordinate, relative to total width; 0.0 = top, 1.0 = bottom</param>
        /// <param name="centerY">Y coordinate, relative to total height; 0.0 = left, 1.0 = right</param>
        /// <param name="shapeSize">size, relative to distance of center from the border; 1.0 will touch the border</param>
        /// <returns></returns>
        public static OutlineShape Character(Random r, int xSize, int ySize, double centerX, double centerY, double shapeSize)
        {
            FontFamily fontFamily = new FontFamily("Helvetica");
            char[] shapeCharacters = {
                'C', 'O', 'S', 'V', 'X',            // no vertical or horizontal lines
                '3', '6', '8', '9', '?',            // no vertical or horizontal lines
                'K', 'R', 'Z', 'A', 'G',            // some vertical or horizontal lines
            };
            char ch = shapeCharacters[r.Next(shapeCharacters.Length)];

            return ExplicitOutlineShape.Char(xSize, ySize, centerX, centerY, shapeSize, ch, fontFamily);
        }

        /// <summary>
        /// Create an outline shape.
        /// </summary>
        /// <param name="r">a source of random numbers</param>
        /// <param name="xSize">width of the created shape</param>
        /// <param name="ySize">height of the created shape</param>
        /// <param name="centerX">X coordinate, relative to total width; 0.0 = top, 1.0 = bottom</param>
        /// <param name="centerY">Y coordinate, relative to total height; 0.0 = left, 1.0 = right</param>
        /// <param name="shapeSize">size, relative to distance of center from the border; 1.0 will touch the border</param>
        /// <returns></returns>
        public static OutlineShape Symbol(Random r, int xSize, int ySize, double centerX, double centerY, double shapeSize)
        {
            FontFamily fontFamily = new FontFamily("Times New Roman");
            char[] shapeCharacters = {
                //'\u0040',   // @
                '\u03C0',   // pi
                '\u05D0',   // aleph
                '\u263B',   // smiley
                '\u2660',   // spades
                '\u2663',   // clubs
                '\u2665',   // hearts
                '\u2666',   // diamonds
                '\u266A',   // musical note
            };
            char ch = shapeCharacters[r.Next(shapeCharacters.Length)];

            return ExplicitOutlineShape.Char(xSize, ySize, centerX, centerY, shapeSize, ch, fontFamily);
        }

        /// <summary>
        /// Create an outline shape.
        /// </summary>
        /// <param name="r">a source of random numbers</param>
        /// <param name="xSize">width of the created shape</param>
        /// <param name="ySize">height of the created shape</param>
        /// <param name="centerX">X coordinate, relative to total width; 0.0 = top, 1.0 = bottom</param>
        /// <param name="centerY">Y coordinate, relative to total height; 0.0 = left, 1.0 = right</param>
        /// <param name="shapeSize">size, relative to distance of center from the border; 1.0 will touch the border</param>
        /// <returns></returns>
        public static OutlineShape Bitmap(Random r, int xSize, int ySize, double centerX, double centerY, double shapeSize)
        {
            return BitmapOutlineShape.Random(r, xSize, ySize, centerX, centerY, shapeSize);
        }

        /// <summary>
        /// Create an outline shape.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="xSize"></param>
        /// <param name="ySize"></param>
        /// <param name="centerX"></param>
        /// <param name="centerY"></param>
        /// <param name="shapeSize"></param>
        /// <returns></returns>
        public static OutlineShape Tiles(Random r, int xSize, int ySize, double centerX, double centerY, double shapeSize)
        {
            return TilesOutlineShape.CreateInstance(r, xSize, ySize);
        }

        /// <summary>
        /// Create an outline shape.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="xSize"></param>
        /// <param name="ySize"></param>
        /// <param name="centerX"></param>
        /// <param name="centerY"></param>
        /// <param name="shapeSize"></param>
        /// <returns></returns>
        public static OutlineShape Rectangles(Random r, int xSize, int ySize, double centerX, double centerY, double shapeSize)
        {
            return RectanglesOutlineShape.CreateInstance(r, xSize, ySize, centerX, centerY);
        }

        /// <summary>
        /// Create an outline shape.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="xSize"></param>
        /// <param name="ySize"></param>
        /// <param name="centerX"></param>
        /// <param name="centerY"></param>
        /// <param name="shapeSize"></param>
        /// <returns></returns>
        public static OutlineShape Grid(Random r, int xSize, int ySize, double centerX, double centerY, double shapeSize)
        {
            return GridOutlineShape.CreateGridInstance(r, xSize, ySize);
        }

        /// <summary>
        /// Create an outline shape.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="xSize"></param>
        /// <param name="ySize"></param>
        /// <param name="centerX"></param>
        /// <param name="centerY"></param>
        /// <param name="shapeSize"></param>
        /// <returns></returns>
        public static OutlineShape GridElement(Random r, int xSize, int ySize, double centerX, double centerY, double shapeSize)
        {
            return GridOutlineShape.CreateSingleInstance(r, xSize, ySize, centerX, centerY, shapeSize);
        }

        #endregion

        #region OutlineShape implementation

        /// <summary>
        /// Returns a rectangle that tightly includes the shape.
        /// </summary>
        /// Note: This code is not very efficient, but it is not executed very often, either.
        /// Note: See also Maze.GetBoundingBox()
        public Rectangle BoundingBox
        {
            get
            {
                int xMin = xSize, xMax = 0, yMin = ySize, yMax = 0;

                for (int x = 0; x < xSize; x++)
                {
                    for (int y = 0; y < ySize; y++)
                    {
                        if (this[x, y] == true)
                        {
                            xMin = Math.Min(xMin, x);
                            xMax = Math.Max(xMax, x);
                            yMin = Math.Min(yMin, y);
                            yMax = Math.Max(yMax, y);
                        }
                    }
                }

                return new Rectangle(xMin, yMin, xMax - xMin + 1, yMax - yMin + 1);
            }
        }

        /// <summary>
        /// Returns the number of squares that are covered be the shape.
        /// </summary>
        public int Area
        {
            get
            {
                int result = 0;

                for (int x = 0; x < xSize; x++)
                {
                    for (int y = 0; y < ySize; y++)
                    {
                        if (this[x, y] == true)
                        {
                            ++result;
                        }
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Returns the largest subset of this shape whose squares are all connected to each other.
        /// </summary>
        /// <returns></returns>
        public OutlineShape ConnectedSubset(InsideShapeDelegate isReserved)
        {
            return ExplicitOutlineShape.ConnectedSubset(this, isReserved);
        }

        /// <summary>
        /// Returns this shape, augmented by all totally enclosed areas.
        /// </summary>
        /// <returns></returns>
        public OutlineShape Closure()
        {
            return ExplicitOutlineShape.Closure(this, null);
        }

        /// <summary>
        /// Returns this shape, augmented by all totally enclosed areas.
        /// Reserved areas define additional borders around enclosed areas.
        /// </summary>
        /// <param name="isReserved">defines the maze's reserved areas</param>
        /// <returns></returns>
        public OutlineShape Closure(InsideShapeDelegate isReserved)
        {
            return ExplicitOutlineShape.Closure(this, isReserved);
        }

        public OutlineShape Inverse()
        {
            InsideShapeDelegate test = delegate(int x, int y) { return !this[x, y]; };
            return new DelegateOutlineShape(this.XSize, this.YSize, test);
        }

        public OutlineShape RotatedOrFlipped(RotateFlipType rft)
        {
            // Note: flipX and flipY will be evaluated after swapping X and Y.
            bool swapXY = false, flipX = false, flipY = false;

            // Note: There are eight different rotate/flip operations but 16 names in the RotateFlipType.
            //       Every operation has two names which give the same enum value.
            switch (rft)
            {
                default:
                case RotateFlipType.RotateNoneFlipNone:
                //case RotateFlipType.Rotate180FlipXY:
                    break;
                case RotateFlipType.RotateNoneFlipX:
                //case RotateFlipType.Rotate180FlipY:
                    flipX = true;
                    break;
                case RotateFlipType.RotateNoneFlipY:
                //case RotateFlipType.Rotate180FlipX:
                    flipX = true;
                    break;
                case RotateFlipType.Rotate90FlipNone:
                //case RotateFlipType.Rotate270FlipXY:
                    swapXY = flipX = true;
                    break;
                case RotateFlipType.Rotate270FlipNone:
                //case RotateFlipType.Rotate90FlipXY:
                    swapXY = flipY = true;
                    break;
                case RotateFlipType.RotateNoneFlipXY:
                //case RotateFlipType.Rotate180FlipNone:
                    flipX = flipY = true;
                    break;
                case RotateFlipType.Rotate90FlipX:
                //case RotateFlipType.Rotate270FlipY:
                    swapXY = true;
                    break;
                case RotateFlipType.Rotate90FlipY:
                //case RotateFlipType.Rotate270FlipX:
                    swapXY = flipX = flipY = true;
                    break;
            }

            int resultXSize, resultYSize;
            if (swapXY)
            {
                resultXSize = this.ySize; resultYSize = this.xSize;
            }
            else
            {
                resultXSize = this.xSize; resultYSize = this.ySize;
            }

            InsideShapeDelegate test = delegate(int x, int y)
            {
                int xx, yy;

                if (swapXY) { xx = y; yy = x; } else { xx = x; yy = y; }
                if (flipX) { xx = this.xSize - 1 - xx; }
                if (flipY) { yy = this.ySize - 1 - yy; }

                return this[xx, yy];
            };

            // Note: We return an ExplicitOutlineShape because that is more efficiently evaluated.
            //       The TilesOutelineShape also requires an ExplicitOutlineShape for its tile pattern.
            return new ExplicitOutlineShape(new DelegateOutlineShape(resultXSize, resultYSize, test));
        }

        #endregion

        #region Methods for applying a distortion to the original shape

        /// <summary>
        /// Returns the percentage of instances that should be distorted.
        /// The default implementation returns the parameter p unmodified.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        protected virtual int DistortedPercentage(int p)
        {
            return p;
        }

        /// <summary>
        /// Returns a DistortedOutlineShape based on the current shape.
        /// If no distortion is applicable, returns the current shape unmodified.
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public virtual OutlineShape DistortedCopy(Random r)
        {
            // Subclasses may return something different.
            return this;
        }

        #endregion

        #region Auxiliary methods

        /// <summary>
        /// Calculate (absolute) [xSize x ySize] coordinates from the given (relative) values.
        /// </summary>
        /// <param name="xSize"></param>
        /// <param name="ySize"></param>
        /// <param name="centerX"></param>
        /// <param name="centerY"></param>
        /// <param name="shapeSize"></param>
        /// <param name="xc"></param>
        /// <param name="yc"></param>
        /// <param name="sz"></param>
        protected static void ConvertParameters(int xSize, int ySize, double centerX, double centerY, double shapeSize, out double xc, out double yc, out double sz)
        {
            // Determine center coordinates in the shape coordinate system.
            xc = xSize * centerX;
            yc = ySize * centerY;

            // Determine sz so that a circle with radius sz would touch the nearest border.
            // If the center is beyond a border, that border is not considered.
            sz = double.MaxValue;
            if (xc > 0) sz = Math.Min(sz, xc);
            if (yc > 0) sz = Math.Min(sz, yc);
            if (xc < xSize) sz = Math.Min(sz, xSize - xc);
            if (yc < ySize) sz = Math.Min(sz, ySize - yc);

            // Multiply with the requested ratio.
            sz *= shapeSize;
        }

        public override string ToString()
        {
            return string.Format("{0}: [{1}x{2}], bbox = {3}, area = {4}",
                this.GetType().ToString(),
                this.XSize, this.YSize,
                this.BoundingBox.ToString(),
                this.Area);
        }

        #endregion
    }
}
