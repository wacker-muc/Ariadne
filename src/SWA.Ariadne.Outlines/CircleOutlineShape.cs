using System;
using System.Collections.Generic;
using System.Text;
using SWA.Utilities;

namespace SWA.Ariadne.Outlines
{
    /// <summary>
    /// A simple OutlineShape in the form of a circle.
    /// </summary>
    internal class CircleOutlineShape : GeometricOutlineShape
    {
        #region Member variables and Properties

        /// <summary>
        /// Returns true if the given point is inside the shape.
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

                return (r <= sz);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Create an OutlineShape.
        /// </summary>
        /// <param name="xSize">width of the created shape</param>
        /// <param name="ySize">height of the created shape</param>
        /// <param name="centerX">X coordinate, relative to total width; 0.0 = top, 1.0 = bottom</param>
        /// <param name="centerY">Y coordinate, relative to total height; 0.0 = left, 1.0 = right</param>
        /// <param name="shapeSize">size, relative to distance of center from the border; 1.0 will touch the border</param>
        private CircleOutlineShape(int xSize, int ySize, double centerX, double centerY, double shapeSize)
            : base(xSize, ySize, centerX, centerY, shapeSize)
        {
        }

        #endregion

        #region OutlineShape implementation

        /// <summary>
        /// Returns the percentage of instances that should be distorted.
        /// The simple CircleOutlineShape should be distorted rather often.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        protected override int DistortedPercentage(int p)
        {
            return Math.Max(p, 80);
        }

        /// <summary>
        /// Returns a DistortedOutlineShape based on the current shape.
        /// If no distortion is applicable, returns the current shape unmodified.
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public override OutlineShape DistortedCopy(Random r)
        {
            return this.RadialWaveDistortedCopy(r);
        }

        /// <summary>
        /// Returns a DistortedOutlineShape based on the current shape and a SpiralDistortion.
        /// A regular polygon with more than six corners is returned unmodified.
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private OutlineShape RadialWaveDistortedCopy(Random r)
        {
            int n = 3 + r.Next(6);                      // number of corners
            double a = r.NextDouble();                  // rotation, 0..1
            double bMin = 0.2 + (n - 2) * 0.03;         // strongly indented sides
            double bMax = 0.85 + (n - 2) * 0.0166;      // almost flat sides
            double b = bMin + r.NextDouble() * (bMax - bMin);

            DistortedOutlineShape.Distortion distortion = DistortedOutlineShape.RadialWaveDistortion(this.xc, this.yc, n, a, b);
            return this.DistortedCopy(distortion);
        }

        #endregion

        #region Static methods for creating OutlineShapes

        public static OutlineShape Create(Random r, int xSize, int ySize, double centerX, double centerY, double shapeSize)
        {
            return new CircleOutlineShape(xSize, ySize, centerX, centerY, shapeSize);
        }

        #endregion
    }
}
