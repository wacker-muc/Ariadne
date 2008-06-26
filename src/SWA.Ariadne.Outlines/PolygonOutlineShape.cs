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
        /// Numbers greater than 1 result in a star shape.
        /// </summary>
        private int windings;

        /// <summary>
        /// An angle by which the polygon is rotated.
        /// 0.0 will put one corner at the top: (0.0/sz).
        /// </summary>
        private double slant;

        /// <summary>
        /// Extension of the sector spanned from the shape's center to two of its corners.
        /// The full angle is 2 * PI / (number of corners).
        /// A slice is usually half a sector, but may be smaller in star shaped polygons.
        /// </summary>
        private double halfSectorAngle, fullSectorAngle, sliceAngle;

        /// <summary>
        /// Center and size (radius) in outline shape coordinates.
        /// </summary>
        protected double xc, yc, sz;

        /// <summary>
        /// For every slice, the number of rotations (by fullSectorAngle) to apply to the point before testing it.
        /// </summary>
        private int[] sliceRotationMap;

        /// <summary>
        /// Returns true if the given point is inside the shape.
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

                // Rotate by a multiple of 90 degrees.  This will bring one edge to the south.
                phi += (windings + 1.5) * Math.PI;

                // Rotate by the given slant.
                phi += this.slant;

#if false
                // Rotate to the "first" sector: [ 0 .. 2*pi/n )
                phi += 10 * 2.0 * Math.PI;
                double k = Math.Truncate(phi / fullSectorAngle);
                phi -= k * fullSectorAngle;

                // Rotate backwards by half the sector size: [ -pi/n .. +pi/n ]
                phi -= halfSectorAngle;
#else
                // Rotate by a multiple of the fullSectorAngle, depending on the slice this point is in.
                //phi += 2.0 * Math.PI;
                double k = Math.Truncate(phi / (2.0 * Math.PI));
                phi -= k * (2.0 * Math.PI);
                int slice = (int)(phi / sliceAngle);
                phi += sliceRotationMap[slice] * fullSectorAngle;

#if false
                if (slice == 11)
                {
                    return false;
                }
#endif
#endif

                // Convert back to rectangular coordinates.
                PolarToRect(r, phi, out dx, out dy);

                // Test if the resulting x coordinate is on the "inside", i.e. to the left of a vertical polygon edge.
                // For negative x, it should be to the right of the opposite edge. (Applies only to polygons with even number of edges.)
                bool result = (Math.Abs(dx) <= sz * Math.Cos(halfSectorAngle));

                return result;
            }
        }

        protected static void RectToPolar(double x, double y, out double r, out double phi)
        {
            r = Math.Sqrt(x * x + y * y);
            phi = Math.Atan2(y, x);
        }

        protected static void PolarToRect(double r, double phi, out double x, out double y)
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
        /// <param name="windings">1 for regular polygons, >1 for star shaped polygons; less than corners/2</param>
        /// <param name="slant"></param>
        /// <param name="xSize">width of the created shape</param>
        /// <param name="ySize">height of the created shape</param>
        /// <param name="centerX">X coordinate, relative to total width; 0.0 = top, 1.0 = bottom</param>
        /// <param name="centerY">Y coordinate, relative to total height; 0.0 = left, 1.0 = right</param>
        /// <param name="shapeSize">size, relative to distance of center from the border; 1.0 will touch the border</param>
        protected PolygonOutlineShape(int corners, int windings, double slant, int xSize, int ySize, double centerX, double centerY, double shapeSize)
        {
            this.corners = corners;
            this.windings = windings;
            this.slant = slant;

            this.sliceAngle = Math.PI / corners;
            this.halfSectorAngle = this.sliceAngle * windings;
            this.fullSectorAngle = 2.0 * halfSectorAngle;

            ConvertParameters(xSize, ySize, centerX, centerY, shapeSize, out this.xc, out this.yc, out this.sz);

            BuildSliceRotationMap();
        }

        /// <summary>
        /// Fills the sliceRotationMap.
        /// </summary>
        private void BuildSliceRotationMap()
        {
            // A slice is half a sector.
            int n = 2 * this.corners;
            sliceRotationMap = new int[n];

            // These sectors are naturally in their regular position.
            int p1 = windings - 1, p2 = (n - 1) - p1;

            // There are two cases for an odd or even number of corners
            if (corners % 2 == 1)
            {
                for (int k = 0; k < n / 2; k++)
                {
                    // Set the number of rotations required to bring these slices to their regular positions.
                    sliceRotationMap[p1] = sliceRotationMap[p2] = k;

                    // Rotate to the previous set of slices; these require one rotation more, i.e. k+1.
                    p1 = (p1 - 2 * windings + n) % n;
                    p2 = (p2 - 2 * windings + n) % n;
                }
            }
            else
            {
                int p3 = (n / 2 - 1) - p1, p4 = (n - 1) - p3;

                for (int k = 0; k < n / 4; k++)
                {
                    // Set the number of rotations required to bring these slices to their regular positions.
                    sliceRotationMap[p1] = sliceRotationMap[p2] = sliceRotationMap[p3] = sliceRotationMap[p4] = k;

                    // Rotate to the previous set of slices; these require one rotation more, i.e. k+1.
                    p1 = (p1 - 2 * windings + n) % n;
                    p2 = (p2 - 2 * windings + n) % n;
                    p3 = (p3 - 2 * windings + n) % n;
                    p4 = (p4 - 2 * windings + n) % n;
                }
            }
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
            int corners = 3 + r.Next(5);
            int windings;
            double slant = 2.0 * Math.PI * r.NextDouble();

#if false
            corners = 8;
            slant = 0;
#endif
            
            if (corners > 4 && r.Next(100) < 150)
            {
                // Build a star shaped polygon.
                windings = 2 + r.Next((corners - 3) / 2);
                //windings = 2;
            }
            else
            {
                // Build a regular polygon.
                windings = 1;
            }

            PolygonOutlineShape result = new PolygonOutlineShape(corners, windings, slant, xSize, ySize, centerX, centerY, shapeSize);

            return result;
        }

        #endregion
    }
}
