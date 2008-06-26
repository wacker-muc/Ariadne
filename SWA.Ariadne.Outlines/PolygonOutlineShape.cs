using System;
using System.Collections.Generic;
using System.Text;

namespace SWA.Ariadne.Outlines
{
    internal class PolygonOutlineShape : OutlineShape
    {
        #region Member variables and Properties

        /// <summary>
        /// Number of corners of the polygon.
        /// </summary>
        private int corners;

        /// <summary>
        /// An angle by which the polygon is rotated.
        /// 0.0 will put one corner at the top: (0.0/sz).
        /// </summary>
        private double slant;

        /// <summary>
        /// Center and size (radius) in outline shape coordinates.
        /// </summary>
        private double xc, yc, sz;

        /// <summary>
        /// Returns true if the given point is inside the polygon.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override bool this[int x, int y]
        {
            get
            {
                double dx = x - xc, dy = -1 * (y - yc);

                // Convert to polar coordinates.
                double r, phi;
                RectToPolar(dx, dy, out r, out phi);
                double halfSectorAngle = Math.PI / corners;
                double fullSectorAngle = 2.0 * halfSectorAngle;

                // Rotate by 90 degrees.  This will bring one corner to the top (0.0/sz).
                phi -= 0.5 * Math.PI;

                // Rotate by the given slant.
                phi += this.slant;

                // Rotate to the "first" sector: [ 0 .. 2*pi/n )
                phi += 2.0 * Math.PI;
                double k = Math.Truncate(phi / fullSectorAngle);
                phi -= k * fullSectorAngle;

                // Rotate backwards by half the sector size: [ -pi/n .. +pi/n ]
                phi -= halfSectorAngle;

                // Convert back to rectangular coordinates.
                PolarToRect(r, phi, out dx, out dy);

                // Test if the resulting x coordinate is on the "inside", i.e. to the left of a vertical polygon edge.
                bool result = dx < sz * Math.Cos(halfSectorAngle);

                return result;
            }
        }

        private void RectToPolar(double x, double y, out double r, out double phi)
        {
            r = Math.Sqrt(x * x + y * y);
            phi = Math.Atan2(y, x);
        }

        private void PolarToRect(double r, double phi, out double x, out double y)
        {
            x = r * Math.Cos(phi);
            y = r * Math.Sin(phi);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Create an OutlineShape based on a Bitmap image.
        /// </summary>
        /// <param name="corners"></param>
        /// <param name="slant"></param>
        /// <param name="xSize">width of the created shape</param>
        /// <param name="ySize">height of the created shape</param>
        /// <param name="centerX">X coordinate, relative to total width; 0.0 = top, 1.0 = bottom</param>
        /// <param name="centerY">Y coordinate, relative to total height; 0.0 = left, 1.0 = right</param>
        /// <param name="shapeSize">size, relative to distance of center from the border; 1.0 will touch the border</param>
        private PolygonOutlineShape(int corners, double slant, int xSize, int ySize, double centerX, double centerY, double shapeSize)
        {
            this.corners = corners;
            this.slant = slant;
            ConvertParameters(xSize, ySize, centerX, centerY, shapeSize, out this.xc, out this.yc, out this.sz);
        }

        #endregion

        #region Static methods for creating OutlineShapes

        /// <summary>
        /// Returns a polygon shape with 3 to 8 corners.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="xSize"></param>
        /// <param name="ySize"></param>
        /// <param name="centerX"></param>
        /// <param name="centerY"></param>
        /// <param name="shapeSize"></param>
        /// <returns></returns>
        public static OutlineShape Random(Random r, int xSize, int ySize, double centerX, double centerY, double shapeSize)
        {
            int corners = 3 + r.Next(6);
            double slant = 2.0 * Math.PI * r.NextDouble();
            return new PolygonOutlineShape(corners, slant, xSize, ySize, centerX, centerY, shapeSize);
        }

        #endregion
    }
}
