using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model;

namespace SWA.Ariadne.Logic
{
    #region Delegate definitions

    #endregion

    /// <summary>
    /// The public methods a MazeSolver must offer.
    /// </summary>
    public interface IMazeSolver
    {
        /// <summary>
        /// Reset to the initial state (before the maze is solved).
        /// </summary>
        void Reset();

        /// <summary>
        /// Travel from one visited square to a neighbor square (through an open wall).
        /// </summary>
        /// <param name="sq1">first (previously visited) square</param>
        /// <param name="sq2">next (neighbor) square</param>
        /// <param name="forward">true if the neighbor square was not visited previously</param>
        void Step(out MazeSquare sq1, out MazeSquare sq2, out bool forward);

        /// <summary>
        /// Find a path in the maze from the start to the end point.
        /// </summary>
        void Solve();

        /// <summary>
        /// Write state information to the given StringBuilder.
        /// </summary>
        /// <param name="message"></param>
        void FillStatusMessage(StringBuilder message);
    }
}
