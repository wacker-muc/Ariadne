using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model;

namespace SWA.Ariadne.Logic
{
    /// <summary>
    /// A MazeSolver will use these methods to draw the path while it is working on a solution.
    /// </summary>
    public interface IMazeDrawer
    {
        /// <summary>
        /// Draws a section of the path between the given (adjoining) squares.
        /// </summary>
        /// <param name="sq1"></param>
        /// <param name="sq2"></param>
        /// <param name="forward"></param>
        void DrawStep(MazeSquare sq1, MazeSquare sq2, bool forward);

        /// <summary>
        /// Draws the path between the given squares.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="forward"></param>
        void DrawPath(List<MazeSquare> path, bool forward);

        /// <summary>
        /// Paints a square to mark it as "dead".
        /// </summary>
        /// <param name="sq"></param>
        /// <param name="distance"></param>
        void DrawDeadSquare(MazeSquare sq);
    }
}
