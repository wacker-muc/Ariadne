using System;
using System.Collections.Generic;
using System.Text;
using SWA.Utilities;

namespace SWA.Ariadne.Outlines
{
    /// <summary>
    /// An OutlineShape in polygon shape.
    /// In a regular polygon, the edges lead from one corner to the next (closest) neighbor.
    /// In star shaped polygons, one or more corners are skipped.
    /// </summary>
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
        /// X coordinate of a vertical edge that separates the polygon's inside and outside.
        /// </summary>
        private double xEdge;

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
                Geometry.RectToPolar(dx, dy, out r, out phi);        // phi > -PI

                // Rotate by a multiple of 90 degrees.  This will bring one edge to the south.
                phi += (windings + 1.5) * Math.PI;          // phi > +PI/2 > 0

                // Rotate by the given slant.
                phi += this.slant;

                // Rotate by a multiple of the sliceAngle, depending on the slice this point is in.
                // This brings the given point close to a vertical line that separates inside from outside.
                int slice = ((int)(phi / sliceAngle) % sliceRotationMap.Length);
                phi += sliceRotationMap[slice] * sliceAngle;

                // Convert back to rectangular coordinates.
                Geometry.PolarToRect(r, phi, out dx, out dy);

                // Test if the resulting x coordinate is on the "inside", i.e. to the left of a vertical polygon edge.
                return (dx <= xEdge);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Create an OutlineShape in polygon shape.
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

            // There are 2*corners slices.
            // The full sector is a triangle formed by the center and two corners connected by an edge.
            this.sliceAngle = Math.PI / corners;
            this.halfSectorAngle = this.sliceAngle * windings;
            this.fullSectorAngle = 2.0 * halfSectorAngle;

            ConvertParameters(xSize, ySize, centerX, centerY, shapeSize, out this.xc, out this.yc, out this.sz);

            // This is the X coordinate of the two corners of a vertical edge (after an adequate rotation).
            this.xEdge = this.sz * Math.Cos(halfSectorAngle);

            BuildSliceRotationMap();
        }

        #endregion

        #region Setup methods

        /// <summary>
        /// Fills the sliceRotationMap.
        /// </summary>
        private void BuildSliceRotationMap()
        {
            // A slice is half a sector.
            int n = 2 * corners;
            sliceRotationMap = new int[n];

            // These sectors are naturally in their regular position (next to the vertical edge).
            int p1 = windings - 1, p2 = (n - 1) - p1;

            int gcd = Geometry.GreatestCommonDivisor(corners, windings);

            // k is the number of full sector rotations required to bring a slice into a regular position...
            for (int k = 0; k < n / gcd / 2; k++)
            {
                // Set the number of full sector rotations required to bring these slices
                // to their regular positions.
                // k full sectors are equal to k * (2 * windings) slices.
                sliceRotationMap[p1] = sliceRotationMap[p2] = k * (2 * windings);

                // For gcd > 1, there are several identical subshapes
                // that can be rotated into the position of the first subshape.
                for (int s = 2; s < 2 * gcd; s += 2)
                {
                    int s1 = (p1 + s) % n, s2 = (p2 + s) % n;
                    sliceRotationMap[s1] = sliceRotationMap[s2] = k * (2 * windings) - s;
                }

                // Rotate to the previous set of slices; these require one rotation more, i.e. k+1.
                p1 = (p1 - 2 * windings + n) % n;
                p2 = (p2 - 2 * windings + n) % n;
            }
        }

        #endregion

        #region Static methods for creating OutlineShapes

        /// <summary>
        /// Returns a polygon shape with 3 to 12 corners.
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
            // We build polygons with up to 12 corners.
            int corners = 3 + r.Next(10);
            int windings;
            double slant = 0;

            // For high number of corners, we prefer star shapes.
            // 5 corners: 50% / 12 corners: 99%
            if (corners > 4 && r.Next(100) < 50 + (corners - 5) * 7)
            {
                // Build a star shaped polygon.
                windings = 2 + r.Next((corners - 3) / 2);
            }
            else
            {
                // Build a regular polygon.
                windings = 1;
            }

            // Apply a random rotation of the whole shape.
            // 3 corners: 55% / 12 corners: 10%
            if (r.Next(100) < 70 - corners * 5)
            {
                slant = 2.0 * Math.PI * r.NextDouble();
            }
            else
            {
                // Rotate by half the rotation symmetry angle.
                // This brings a corner (instead of an edge) to the south; applies to all shapes.
                if (r.Next(100) < 50)
                {
                    slant = 2.0 * Math.PI / corners / 2;
                }

                // Rotate by 90 degrees.
                // This has no effect when corners is a multiple of four.
                // For an even number of corners, the effect is the same as the previous rotation, i.e. neutral.
                // For an odd number of corners, the edge is rotated from south/north to east/west.
                if (r.Next(100) < 50)
                {
                    slant += Math.PI / 4.0;
                }
            }

            PolygonOutlineShape result = new PolygonOutlineShape(corners, windings, slant, xSize, ySize, centerX, centerY, shapeSize);

            return result;
        }

        #endregion
    }
}
