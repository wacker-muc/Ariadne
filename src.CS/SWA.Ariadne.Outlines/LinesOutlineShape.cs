using System;
using System.Collections.Generic;
using System.Text;
using SWA.Utilities;

namespace SWA.Ariadne.Outlines
{
    internal class LinesOutlineShape : GeometricOutlineShape
    {
        #region Member variables and Properties

        private LineOutlineShape[] lines;

        /// <summary>
        /// Returns true if the given point is inside the shape,
        /// i.e. if the point is covered by an odd number of half planes.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override bool this[double x, double y]
        {
            get
            {
                int n = 0;
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i][x, y])
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
        private LinesOutlineShape(Random r, int xSize, int ySize)
            : base(xSize, ySize)
        {
            int n = r.Next(4, 7);
            lines = new LineOutlineShape[n];

            double xccMin = 0.15 * this.sz, xccMax = xSize - 0.15 * this.sz;
            double yccMin = 0.15 * this.sz, yccMax = ySize - 0.15 * this.sz;
            
            for (int i = 0; i < n; i++)
            {
                // Choose the line parameters: center and slant.
                double xcc = xccMin + r.NextDouble() * (xccMax - xccMin);
                double ycc = yccMin + r.NextDouble() * (yccMax - yccMin);
                double slant = r.NextDouble() * Math.PI;

                lines[i] = new LineOutlineShape(xSize, ySize, xcc, ycc, slant, false);
            }
        }

        #endregion

        #region Static methods for creating OutlineShapes

        public static OutlineShape CreateInstance(Random r, int xSize, int ySize)
        {
            return new LinesOutlineShape(r, xSize, ySize);
        }

        #endregion
    }

    internal class LineOutlineShape : GeometricOutlineShape
    {
        #region Member variables and Properties

        /// <summary>
        /// Slant of the normal vector.
        /// </summary>
        private readonly double normalPhi;

        /// <summary>
        /// Returns true if the given point is inside the shape,
        /// i.e. if it is on the same side of the line as the normal vector.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override bool this[double x, double y]
        {
            get
            {
                double dx = x - xc, dy = -1 * (y - yc);

                // Convert to polar coordinates.
                double r, phi;
                Geometry.RectToPolar(dx, dy, out r, out phi);

                double dPhi = phi - normalPhi + 3 * Math.PI;
                dPhi -= Math.Truncate(dPhi / (2.0 * Math.PI)) * 2.0 * Math.PI;

                return (dPhi <= Math.PI);
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
        /// <param name="normalPhi">Slant of the normal vector</param>
        internal LineOutlineShape(int xSize, int ySize, double centerX, double centerY, double normalPhi, bool relativeCoordinates)
            : base(xSize, ySize, centerX, centerY, 1.0, relativeCoordinates)
        {
            double x, y, r = 1.0;
            Geometry.PolarToRect(r, normalPhi, out x, out y);
            Geometry.RectToPolar(x, y, out r, out this.normalPhi);
        }

        #endregion
    }
}
