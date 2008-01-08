using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model;

namespace SWA.Ariadne.Logic
{
    /// <summary>
    /// Delegate method that paints dead branches.
    /// Only used by flooding strategies that only travel in forward direction.
    /// </summary>
    /// <param name="path">List of MazeSquares starting at the dead end and ending at the branching square (not dead)</param>
    public delegate void MarkDeadBranchDelegate(List<MazeSquare> path);

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
        /// A delegate for marking dead branches.  The caller should paint the path between the given squares.
        /// </summary>
        MarkDeadBranchDelegate MarkDeadBranchDelegate { set; }
    }
}
