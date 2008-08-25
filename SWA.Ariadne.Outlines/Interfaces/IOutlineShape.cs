using System;
using System.Collections.Generic;
using System.Text;

namespace SWA.Ariadne.Outlines.Interfaces
{
    #region Delegates

    /// <summary>
    /// A delegate type that implements the OutlineShape behavior.
    /// Returns true if the square at (x, y) is inside of the shape, false otherwise.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public delegate bool InsideShapeDelegate(int x, int y);

    #endregion
}
