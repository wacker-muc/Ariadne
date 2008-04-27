using System;
using System.Collections.Generic;
using System.Text;

namespace SWA.Ariadne.Model
{
    public class OutlineShape
    {
        #region Member variables and Properties

        public bool this[int x, int y]
        {
            get { return squares[x, y]; }
        }
        private bool[,] squares;

        #endregion

        #region Constructor

        private OutlineShape(int xSize, int ySize)
        {
            this.squares = new bool[xSize, ySize];
        }

        #endregion

        #region Static methods for creating OutlineShapes

        /// <summary>
        /// Create an outline shape.
        /// </summary>
        /// <param name="centerX">X coordinate, relative to total width; 0.0 = top, 1.0 = bottom</param>
        /// <param name="centerY">Y coordinate, relative to total height; 0.0 = left, 1.0 = right</param>
        /// <param name="radius">radius, relative to distance of center from the border; 1.0 will touch the border </param>
        /// <returns></returns>
        public static OutlineShape Circle(int xSize, int ySize, double centerX, double centerY, double radius)
        {
            OutlineShape result = new OutlineShape(xSize, ySize);

            double xc, yc, r;
            ConvertParameters(xSize, ySize, centerX, centerY, radius, out xc, out yc, out r);

            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    double dx = x - xc, dy = y - yc;
                    result.squares[x, y] = (dx * dx + dy * dy <= r * r);
                }
            }

            return result;
        }

        /// <summary>
        /// Create an outline shape.
        /// </summary>
        /// <param name="centerX">X coordinate, relative to total width; 0.0 = top, 1.0 = bottom</param>
        /// <param name="centerY">Y coordinate, relative to total height; 0.0 = left, 1.0 = right</param>
        /// <param name="radius">radius, relative to distance of center from the border; 1.0 will touch the border </param>
        /// <returns></returns>
        public static OutlineShape Diamond(int xSize, int ySize, double centerX, double centerY, double radius)
        {
            OutlineShape result = new OutlineShape(xSize, ySize);

            double xc, yc, r;
            ConvertParameters(xSize, ySize, centerX, centerY, radius, out xc, out yc, out r);

            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    double dx = Math.Abs(x - xc), dy = Math.Abs(y - yc);
                    result.squares[x, y] = (dx + dy <= r);
                }
            }

            return result;
        }

        private static void ConvertParameters(int xSize, int ySize, double centerX, double centerY, double radius, out double xc, out double yc, out double r)
        {
            // Determine center coordinates in the shape coordinate system.
            xc = xSize * centerX;
            yc = ySize * centerY;

            // Determine r so that a circle would touch the nearest border.
            // If the center is beyond a border, that border is not considered.
            r = double.MaxValue;
            if (xc > 0) r = Math.Min(r, xc);
            if (yc > 0) r = Math.Min(r, yc);
            if (xc < xSize) r = Math.Min(r, xSize - xc);
            if (yc < ySize) r = Math.Min(r, ySize - yc);

            // Multiply with the requested ratio.
            r *= radius;
        }

        #endregion
    }
}
