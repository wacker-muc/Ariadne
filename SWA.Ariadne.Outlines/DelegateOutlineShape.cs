using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Outlines.Interfaces;

namespace SWA.Ariadne.Outlines
{
    /// <summary>
    /// An OutlineShape that is defined by a generating (delegate) function.
    /// </summary>
    internal class DelegateOutlineShape : OutlineShape
    {
        #region Member variables and Properties

        public override bool this[int x, int y]
        {
            get { return test(x, y); }
        }

        private InsideShapeDelegate test;

        #endregion

        #region Constructor

        public DelegateOutlineShape(int xSize, int ySize, InsideShapeDelegate test)
            : base(xSize, ySize)
        {
            this.test = test;
        }

        #endregion
    }
}
