using System;
using System.Collections.Generic;
using System.Text;

namespace SWA.Ariadne.Outlines
{
    /// <summary>
    /// Base class for outline shapes that are formed of a single geometric shape.
    /// E.g. circles, algebraic curves, polygons.
    /// </summary>
    abstract class GeometricOutlineShape : SmoothOutlineShape
    {
        #region Member variables

        /// <summary>
        /// Center and size (radius) in outline shape coordinates.
        /// </summary>
        protected double xc, yc, sz;

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
        protected GeometricOutlineShape(int xSize, int ySize, double centerX, double centerY, double shapeSize)
            : base(xSize, ySize)
        {
            ConvertParameters(xSize, ySize, centerX, centerY, shapeSize, out this.xc, out this.yc, out this.sz);
        }

        #endregion
    }
}
