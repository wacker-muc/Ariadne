using System;
using System.Collections.Generic;
using System.Text;

namespace SWA.Ariadne.Outlines
{
    /// <summary>
    /// A SmoothOutlineShape can evaluate double precision coordinates.
    /// </summary>
    internal abstract class SmoothOutlineShape : OutlineShape
    {
        /// <summary>
        /// Returns true if the given point is inside the shape.
        /// SmoothOutlineShapes use double instead of int parameters.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public abstract bool this[double x, double y] { get; }

        /// <summary>
        /// Returns true if the given point is inside the shape.
        /// Call the method with double parameters.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override bool this[int x, int y]
        {
            get { return this[(int)x, (int)y]; }
        }

        protected SmoothOutlineShape(int xSize, int ySize)
            : base(xSize, ySize)
        {
        }

        #region Methods for applying a distortion to the original shape

        /// <summary>
        /// Returns a DistortedOutlineShape based on the current shape and the given distortion.
        /// </summary>
        /// <param name="distortion"></param>
        /// <returns></returns>
        internal OutlineShape DistortedCopy(DistortedOutlineShape.Distortion distortion)
        {
            return new DistortedOutlineShape(XSize, YSize, this, distortion);
        }

        #endregion
    }
}
