using System;
using System.Collections.Generic;
using System.Text;

namespace SWA.Ariadne.Outlines
{
    /// <summary>
    /// A simple OutlineShape in the form of a square standing on one of its corners.
    /// </summary>
    internal class DiamondOutlineShape : GeometricOutlineShape
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
                double dx = Math.Abs(x - xc), dy = Math.Abs(y - yc);

                return (dx + dy <= sz);
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
        private DiamondOutlineShape(int xSize, int ySize, double centerX, double centerY, double shapeSize)
            : base(xSize, ySize, centerX, centerY, shapeSize)
        {
        }

        #endregion

        #region Static methods for creating OutlineShapes

        public static OutlineShape Create(Random r, int xSize, int ySize, double centerX, double centerY, double shapeSize)
        {
            return new DiamondOutlineShape(xSize, ySize, centerX, centerY, shapeSize);
        }

        #endregion
    }
}
