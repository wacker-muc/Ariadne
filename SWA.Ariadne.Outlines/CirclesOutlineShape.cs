using System;
using System.Collections.Generic;
using System.Text;

namespace SWA.Ariadne.Outlines
{
    internal class CirclesOutlineShape : GeometricOutlineShape
    {
        #region Member variables and Properties

        private CircleOutlineShape[] circles;

        /// <summary>
        /// Returns true if the given point is inside the shape,
        /// i.e. if the point is covered by an odd number of circles.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override bool this[double x, double y]
        {
            get
            {
                int n = 0;
                for (int i = 0; i < circles.Length; i++)
                {
                    if (circles[i][x, y])
                    {
                        n++;
                    }
                }

                return (n % 2 == 1);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Create an OutlineShape.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="xSize">width of the created shape</param>
        /// <param name="ySize">height of the created shape</param>
        /// <param name="centerX">X coordinate, relative to total width; 0.0 = top, 1.0 = bottom</param>
        /// <param name="centerY">Y coordinate, relative to total height; 0.0 = left, 1.0 = right</param>
        /// <param name="shapeSize">size, relative to distance of center from the border; 1.0 will touch the border</param>
        private CirclesOutlineShape(Random r, int xSize, int ySize, double centerX, double centerY, double shapeSize)
            : base(xSize, ySize, centerX, centerY, shapeSize, true)
        {
            int n = r.Next(3, 7);
            circles = new CircleOutlineShape[n];

            double xccMin = -0.33 * this.sz, xccMax = xSize + 0.33 * this.sz;
            double yccMin = -0.33 * this.sz, yccMax = ySize + 0.33 * this.sz;
            double szcRange = 0.5;
            
            for (int i = 0; i < n; i++)
            {
                // Choose the circle parameters: center and radius.
                double xcc = xccMin + r.NextDouble() * (xccMax - xccMin);
                double ycc = yccMin + r.NextDouble() * (yccMax - yccMin);
                double szc = this.sz * ((1.0 - szcRange) + (2.0 * szcRange));

                // If the center is too far outside of the border, increase the radius.
                double borderDist = Math.Min(Math.Min(xcc, xSize - xcc), Math.Min(ycc, ySize - ycc));
                if (borderDist + szc < 0.5 * szc)
                {
                    szc = this.sz * (1.0 + 1.0 * szcRange);
                }
                
                circles[i] = new CircleOutlineShape(xSize, ySize, xcc, ycc, szc, false);
            }
        }

        #endregion

        #region Static methods for creating OutlineShapes

        public static OutlineShape CreateInstance(Random r, int xSize, int ySize, double centerX, double centerY, double shapeSize)
        {
            return new CirclesOutlineShape(r, xSize, ySize, centerX, centerY, shapeSize);
        }

        #endregion
    }
}
