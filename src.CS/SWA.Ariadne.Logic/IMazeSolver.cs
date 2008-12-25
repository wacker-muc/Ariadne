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

        /// <summary>
        /// Enable an algorithm for detecting areas unreachable from the end square.
        /// </summary>
        void MakeEfficient();

        /// <summary>
        /// Returns true if this MazeSolver can detect areas unreachable the end square.
        /// </summary>
        bool IsEfficientSolver { get; }

        /// <summary>
        /// Coordinate shared resources of this solver with the master solver.
        /// </summary>
        /// <param name="iMazeSolver"></param>
        void CoordinateWithMaster(IMazeSolver masterSolver);
    }
}
