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
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public abstract bool this[double x, double y] { get; }

        /// <summary>
        /// Returns true if the given point is inside the shape.
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
    }
}
